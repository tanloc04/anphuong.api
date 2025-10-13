namespace anphuong.Core.Interfaces.Services.External
{
    public interface IJwtService
    {
        string GenerateToken(string? userId, string? userEmail);

    }
}
