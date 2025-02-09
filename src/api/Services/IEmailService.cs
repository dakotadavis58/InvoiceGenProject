namespace InvoiceGeneratorAPI.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    // We can add more email methods here later
}