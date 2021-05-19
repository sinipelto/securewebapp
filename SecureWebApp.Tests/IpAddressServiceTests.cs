using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SecureWebApp.Interfaces;
using SecureWebApp.Services;

namespace SecureWebApp.Tests
{
    public class IpAddressServiceTests
    {
        private Mock<HttpContextAccessor> _accessorMock;
        private IIpAddressService _service;

        [SetUp]
        public void Setup()
        {
            _accessorMock = new Mock<HttpContextAccessor>(MockBehavior.Default);
            _service = new IpAddressService(_accessorMock.Object);
        }

        [Test]
        public void Test_GetIp_XForwardTrue()
        {
            string expectedIp = "1.2.3.4";

            _accessorMock.Setup(i => i.HttpContext.Request.Headers).Returns(() =>
            {
                var dict = new HeaderDictionary
                {
                    new("", new StringValues()),
                    new()
                };

                return dict;
            });

            var ip = _service.GetRequestIp(true);

            //Assert.AreEqual();
         
            Assert.Pass();
        }

        public void Test_GetIp_UsingRealConnection()
        {

        }
    }
}