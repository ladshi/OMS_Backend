namespace OMS_Backend.Services;

public interface IEmailService
{
    Task SendCustomerCredentialsAsync(string email, string firstName, string password);
    Task SendPasswordResetLinkAsync(string email, string resetToken);
}
