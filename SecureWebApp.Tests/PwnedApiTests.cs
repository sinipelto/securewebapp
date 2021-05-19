using Moq;
using NUnit.Framework;
using SecureWebApp.Interfaces;
using SecureWebApp.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureWebApp.Tests
{
    public class PwnedApiTests
    {
        private Mock<IHttpClientFactory> _clientFactoryMock;

        private IBreachCheckService _pwnedService;

        [SetUp]
        public void Setup()
        {
            _clientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Default);
            _clientFactoryMock
                .Setup(i => i.CreateClient(PwnedApiCheckService.Name))
                .Returns(() =>
                {
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://api.pwnedpasswords.com"),
                        DefaultRequestVersion = new Version(2, 0),
                        Timeout = TimeSpan.FromSeconds(5)
                    };
                    client.DefaultRequestHeaders.Add("Accept", "text/plain");
                    client.DefaultRequestHeaders.Add("UserAgent", "SecureWebAppUnitTest/1.0.0");
                    return client;
                });

            _pwnedService = new PwnedApiCheckService(_clientFactoryMock.Object);
        }

        [Test]
        public async Task Test_Check_DefinitelyBreachedPasswords()
        {
            var pws = new [] {"Password", "Password123", "Password123!", "qwertyuiop123456!", "HelloWorld", "Salasana", "Passwd", "nothing"};

            foreach (var pw in pws)
            {
                var res = await _pwnedService.CheckPasswordAsync(pw);
                Assert.IsTrue(res);
            }
        }

        [Test]
        public async Task Test_Check_RandomGeneratedSecurePassword()
        {
            using var crypto = new RNGCryptoServiceProvider();
            var bytes = new byte[100]; // passwd max length = 100
            crypto.GetBytes(bytes);
            var pw = string.Concat(Encoding.UTF8.GetString(bytes).Where(i => char.IsLetterOrDigit(i) || char.IsPunctuation(i)));

            var res = await _pwnedService.CheckPasswordAsync(pw);
            Assert.IsFalse(res);
        }
    }
}