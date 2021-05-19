using Microsoft.AspNetCore.Hosting;
[assembly: HostingStartup(typeof(SecureWebApp.Areas.Identity.IdentityHostingStartup))]
namespace SecureWebApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}