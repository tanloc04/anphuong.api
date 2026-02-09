using System.Security.Claims;
using System.Text;
using System.Text.Json;
using anphuong.api.Extensions;
using anphuong.Core.Domains.DTOs.Config;
using anphuong.Repository.Context;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// 1. Load Environment Variables (.env file)
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Load Configuration sources (Environment variables are added automatically by CreateBuilder)
builder.Configuration.AddEnvironmentVariables();

// --- SERVICES REGISTRATION ---

#region CORS Configuration
// Giữ nguyên theo yêu cầu của bạn để tiện testing.
// LƯU Ý: Khi deploy production hoặc tích hợp FE có gửi Cookie (withCredentials: true),
// bạn SẼ KHÔNG THỂ dùng AllowAnyOrigin(). Lúc đó phải đổi sang WithOrigins("http://domain-fe.com").
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});
#endregion

#region Cloudinary Configuration
// Tối ưu: Dùng builder.Configuration thay vì Environment.GetEnvironmentVariable trực tiếp
// để tận dụng cơ chế config binding của .NET
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = builder.Configuration["CLOUDINARY_CLOUDNAME"];
    options.ApiKey = builder.Configuration["CLOUDINARY_APIKEY"];
    options.ApiSecret = builder.Configuration["CLOUDINARY_APISECRET"];
});
#endregion

#region JWT Authentication Configuration
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "")),
        ClockSkew = TimeSpan.Zero // Loại bỏ độ trễ mặc định 5 phút khi token hết hạn
    };

    options.Events = new JwtBearerEvents
    {
        // --- QUAN TRỌNG: Logic lấy Token từ Cookie ---
        OnMessageReceived = context =>
        {
            // Kiểm tra xem token có trong Cookie "accessToken" không
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        },
        // ---------------------------------------------

        // Customize 401 Unauthorized Response
        OnChallenge = async context =>
        {
            // Bỏ qua logic mặc định để không ghi đè response
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Access Denied. Token is missing, invalid, or expired."
            });

            await context.Response.WriteAsync(result);
        },

        // Customize 403 Forbidden Response
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Access Denied. You do not have permission to access this resource."
            });

            await context.Response.WriteAsync(result);
        },

        // Map Email Claim when Token Validated
        OnTokenValidated = context =>
        {
            var emailClaim = context.Principal?.FindFirst("email")?.Value ??
                             context.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(emailClaim))
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                // Đảm bảo claim type chuẩn ClaimTypes.Email được set
                if (claimsIdentity != null && !claimsIdentity.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, emailClaim));
                }
            }
            return Task.CompletedTask;
        }
    };
});
#endregion

#region Database Connection
var connectionString = builder.Configuration.GetConnectionString("AnPhuongFurnitureDb");
builder.Services.AddDbContext<anphuongDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Authorization Policies
var allowedEmail = builder.Configuration["ALLOWED_EMAILS"]?.Trim();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowSpecificEmail", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == ClaimTypes.Email || c.Type == "email")
                && c.Value.Equals(allowedEmail, StringComparison.OrdinalIgnoreCase))));
});
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Register(); // Register custom services (DI)

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "An Phuong API", Version = "v1" });

    // Cấu hình JWT cho Swagger (Nút Authorize)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

// --- BUILD APP ---
var app = builder.Build();

// --- MIDDLEWARE PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "An Phuong API v1");
    });
}

app.UseRouting();

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["accessToken"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }
    await next();
});

// CORS phải đặt giữa Routing và Authentication
app.UseCors("AllowReactApp");

app.UseAuthentication(); // Xác thực (Ai đang đăng nhập? Check Header/Cookie)
app.UseAuthorization();  // Phân quyền (Có được phép vào không?)

app.MapControllers();

app.Run();