namespace anphuong.Core.Interfaces.Services.External
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
