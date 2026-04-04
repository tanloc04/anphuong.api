using anphuong.api.Extensions;
using anphuong.api.Hubs;
using anphuong.Core.Domains.DTOs.Config;
using anphuong.Repository.Context;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

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

builder.Configuration
    .AddEnvironmentVariables();

#region Environment configuration
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME");
    options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_APIKEY");
    options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_APISECRET");
});
#endregion

#region Jwt configuration 
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("Jwt:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

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
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
         };

         options.Events = new JwtBearerEvents
         {
             // Customize the 401 response
             OnChallenge = context =>
             {
                 // Skip the default response
                 context.HandleResponse();

                 // Customize the response
                 context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                 context.Response.ContentType = "application/json";

                 var result = JsonSerializer.Serialize(new
                 {
                     success = false,
                     message = "Access Denied. Token is missing or invalid."
                 });

                 return context.Response.WriteAsync(result);
             },

             // Customize the 403 response
             OnForbidden = context =>
             {
                 // Customize the response
                 context.Response.StatusCode = StatusCodes.Status403Forbidden;
                 context.Response.ContentType = "application/json";

                 var result = JsonSerializer.Serialize(new
                 {
                     success = false,
                     message = "Access Denied. You do not have permission to access this resource."
                 });

                 return context.Response.WriteAsync(result);
             },

             OnTokenValidated = context =>
             {
                 var emailClaim = context.Principal?.FindFirst("email")?.Value;
                 if (!string.IsNullOrEmpty(emailClaim))
                 {
                     var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                     claimsIdentity?.AddClaim(new Claim(ClaimTypes.Email, emailClaim));
                 }
                 return Task.CompletedTask;
             },

             OnMessageReceived = context =>
             {
                 if (context.Request.Cookies.ContainsKey("accessToken"))
                 {
                     context.Token = context.Request.Cookies["accessToken"];
                 }
                 return Task.CompletedTask;
             }
         };
     });
#endregion

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Register();
builder.Services.AddSignalR();

#region Allow Specific Email
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

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "An Phuong API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here. Example: Bearer {your token}"
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
                new string[] {}
            }
        });
});
#endregion

#region DBConnection


var connectionString = builder.Configuration.GetConnectionString("AnPhuongFurnitureDb");

builder.Services.AddDbContext<anphuongDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "An Phuong API v1");
    });
}
app.UseRouting();

app.UseCors("AllowReactApp");

app.MapHub<OrderHub>("/orderHub");

app.MapHub<ChatHub>("/chatHub");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
