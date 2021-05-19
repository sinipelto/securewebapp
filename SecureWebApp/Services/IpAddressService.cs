using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SecureWebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureWebApp.Services
{
    public class IpAddressService : IIpAddressService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetRequestIp(bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = GetHeaderValueAs<string>("REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullOrWhitespace())
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }

        public T GetHeaderValueAs<T>(string headerName)
        {
            StringValues values = default;

            if (!(_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(headerName, out values) ?? false))
                return default;

            var rawValues = values.ToString();   // writes out as Csv when there are multiple.

            if (!rawValues.IsNullOrWhitespace())
                return (T)Convert.ChangeType(values.ToString(), typeof(T));

            return default;
        }
    }

    public static class StringExtensions
    {
        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }
    }
}