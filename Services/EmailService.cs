using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OMS_Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendCustomerCredentialsAsync(string email, string firstName, string password)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OMS Admin", _configuration["Email:From"] ?? "admin@oms.com"));
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "Welcome to OMS - Your Account Credentials";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2>Welcome to OMS!</h2>
                    <p>Dear {firstName},</p>
                    <p>Your account has been created successfully. Please use the following credentials to login:</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Password:</strong> {password}</p>
                    <p>Please login and change your password after first login.</p>
                    <p>Best regards,<br>OMS Admin Team</p>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["Email:Username"] ?? "",
                _configuration["Email:Password"] ?? "");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Credentials email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send credentials email to {email}");
            throw;
        }
    }

    public async Task SendPasswordResetLinkAsync(string email, string resetToken)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OMS Admin", _configuration["Email:From"] ?? "admin@oms.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset Request";

            var resetUrl = $"{_configuration["FrontendUrl"]}/reset-password?token={resetToken}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href=""{resetUrl}"">Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Best regards,<br>OMS Admin Team</p>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["Email:Username"] ?? "",
                _configuration["Email:Password"] ?? "");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Password reset email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send password reset email to {email}");
            throw;
        }
    }
}
