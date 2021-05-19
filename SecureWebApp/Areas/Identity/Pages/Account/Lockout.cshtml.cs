using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SecureWebApp.Models;

namespace SecureWebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {
        public EmailMessage EmailMessage { get; set; }

        public void OnGet()
        {
            if (TempData.TryGetValue("UnlockEmailMsg", out var data))
            {
                EmailMessage = JsonConvert.DeserializeObject<EmailMessage>(data.ToString());
            }
        }
    }
}
