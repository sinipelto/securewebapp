using SecureWebApp.Interfaces;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureWebApp.Services
{
    public class PasswdsApiCheckService: IBreachCheckService
    {
        public const string Name = "PasswdsApiService";

        private readonly IHttpClientFactory _httpClientFactory;

        public PasswdsApiCheckService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public Task<bool> CheckPasswordAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace.", nameof(password));
            }

            throw new NotImplementedException();
        }

        public Task<bool> CheckHashAsync<T>(T hash) where T : HashAlgorithm
        {
            throw new NotImplementedException();
        }
    }
}