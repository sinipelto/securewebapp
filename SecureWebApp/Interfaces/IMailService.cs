using Microsoft.AspNetCore.Identity;
using SecureWebApp.Models;
using SecureWebApp.Services;
using System.Threading.Tasks;

namespace SecureWebApp.Interfaces
{
    public interface IMailService
    {
        /// <summary>
        /// Send a email fully provided by the caller.
        /// Use the corresponding email model to construct the email from scratch.
        /// </summary>
        /// <param name="message">The email to be sent.</param>
        Task SendEmailAsync(EmailMessage message);

        /// <summary>
        /// Send account lockout unlock email. Contains information about the user account lockout
        /// and generates an unlock link in the email to unlock the user account, constructed by the given parameter values.
        /// </summary>
        /// <param name="user">The user identity to send the unlock email to.</param>
        /// <param name="recoveryToken">The token value to insert into the unlock link.</param>
        /// <returns>The email sent to the given user.</returns>
        Task<EmailMessage> SendAccountUnlockEmailAsync(IdentityUser user, string recoveryToken);

        /// <summary>
        /// Retrieves a specific pre-defined email template string
        /// Can be used to send pre-defined emails with customized content using given parameters.
        /// </summary>
        /// <param name="template">The email template to choose from.</param>
        /// <param name="input">The input parameters to insert into the template. NOTE: The parameter count and order depends on the used template</param>
        /// <returns>The email content template as string</returns>
        public Task<string> GetEmailTemplateAsync(EmailSenderService.Templates template, params string[] input);
    }
}