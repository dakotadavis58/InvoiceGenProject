using Microsoft.Extensions.Options;
using InvoiceGeneratorAPI.Settings;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;

namespace InvoiceGeneratorAPI.Services;

public class EmailService : IEmailService
{
    private readonly IAmazonSimpleEmailServiceV2 _sesClient;
    private readonly string _fromEmail;

    public EmailService(IOptions<AwsSettings> awsSettings)
    {
        var settings = awsSettings.Value;
        _sesClient = new AmazonSimpleEmailServiceV2Client(
            settings.AccessKey,
            settings.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(settings.Region)
        );
        _fromEmail = settings.SES.FromEmail;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "Reset Your Password";
        var htmlBody = $@"
            <html>
                <body>
                    <h2>Reset Your Password</h2>
                    <p>You have requested to reset your password. Click the link below to set a new password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>If you didn't request this, you can safely ignore this email.</p>
                    <p>This link will expire in 24 hours.</p>
                </body>
            </html>";

        var textBody = $@"
            Reset Your Password

            You have requested to reset your password. Click the link below to set a new password:

            {resetLink}

            If you didn't request this, you can safely ignore this email.
            This link will expire in 24 hours.";

        var sendRequest = new SendEmailRequest
        {
            FromEmailAddress = _fromEmail,
            Destination = new Destination
            {
                ToAddresses = new List<string> { toEmail }
            },
            Content = new EmailContent
            {
                Raw = new RawMessage
                {
                    Data = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes($@"
Subject: {subject}
MIME-Version: 1.0
Content-Type: multipart/alternative; boundary=""boundary""

--boundary
Content-Type: text/plain; charset=UTF-8

{textBody}

--boundary
Content-Type: text/html; charset=UTF-8

{htmlBody}

--boundary--
"))
                }
            }
        };

        try
        {
            await _sesClient.SendEmailAsync(sendRequest);
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Failed to send email", ex);
        }
    }
}