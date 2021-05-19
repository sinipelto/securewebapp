using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SecureWebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class UnlockAccountModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UnlockAccountModel> _logger;

        public UnlockAccountModel(UserManager<IdentityUser> userManager, 
            ILogger<UnlockAccountModel> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = false;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                result = await _userManager.VerifyUserTokenAsync(user, "AccountUnlockTokenProvder", "AccountUnlock", code);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse input code.");
            }

            StatusMessage = result ? "Account unlocked successfully." : "Error unlocking account: Unlock token verification failed.";

            if (!result)
            {
                ModelState.AddModelError("VerificationFailed", "Could not verify unlock token. Double check the unlock link is correct.");
                return Page();
            }

            // Set lockout end date to epoch (0) and unlock the account
            var unlockDate = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);

            if (unlockDate.Succeeded) return Page();

            _logger.LogError("ERROR: Failed to set user unlock date in the past.");

            foreach (var error in unlockDate.Errors)
            {
                ModelState.AddModelError("SetUserLockoutEndDateFailed", error.Description);
            }

            return Page();
        }
    }
}
