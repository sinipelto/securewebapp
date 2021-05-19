using Microsoft.AspNetCore.Identity;
using SecureWebApp.Models;
using System.Threading.Tasks;

namespace SecureWebApp.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailMessage message);

        Task<EmailMessage> SendAccountUnlockEmailAsync(IdentityUser user, string recoveryToken);
    }
}