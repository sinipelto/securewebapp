using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SecureWebApp.Interfaces;
using SecureWebApp.Services;
using System;
using System.Threading.Tasks;

namespace SecureWebApp.Tests
{
    public class IpAddressServiceTests
    {
        private Mock<IHttpContextAccessor> _accessorMock;
        private IIpAddressService _service;

        [SetUp]
        public void Setup()
        {
            _accessorMock = new Mock<IHttpContextAccessor>(MockBehavior.Default);
            _service = new IpAddressService(_accessorMock.Object);
        }

        [Test]
        public void Test_GetIp_XForwardTrue()
        {
            const string expectedIp = "1.2.3.4";
            const string otherIp = "5.6.7.8";

            var ctx = new DefaultHttpContext();
            ctx.Request.Headers["X-Forwarded-For"] = expectedIp;
            ctx.Request.Headers["HTTP-X-Forwarded-For"] = otherIp;
            ctx.Request.Headers["REMOTE_ADDR"] = otherIp;

            _accessorMock.Setup(_ => _.HttpContext).Returns(ctx);

            var ip = _service.GetRequestIp();

            Assert.AreEqual(expectedIp, ip);
        }

        [Test]
        public void Test_GetIp_XForwardFalse()
        {
            const string expectedIp = "1.2.3.4";
            const string otherIp = "5.6.7.8";

            var ctx = new DefaultHttpContext();
            ctx.Request.Headers["X-Forwarded-For"] = otherIp;
            ctx.Request.Headers["HTTP-X-Forwarded-For"] = otherIp;
            ctx.Request.Headers["REMOTE_ADDR"] = expectedIp;

            _accessorMock.Setup(_ => _.HttpContext).Returns(ctx);

            var ip = _service.GetRequestIp(false);

            Assert.AreEqual(expectedIp, ip);
        }

        [Test]
        public void Test_GetIp_NotAvailable()
        {
            Assert.Throws<Exception>(() =>
            {
                _service.GetRequestIp(true);
            });

            Assert.Throws<Exception>(() =>
            {
                _service.GetRequestIp(false);
            });
        }
    }
}