using SecureWebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureWebApp.Services
{
    public class PwnedApiCheckService : IBreachCheckService
    {
        public const string Name = "PwnedApiService";

        private readonly IHttpClientFactory _httpClientFactory;

        public PwnedApiCheckService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CheckPasswordAsync(string password)
        {
            var hasher = SHA1.Create();

            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashStr = hash.Aggregate(string.Empty, (current, b) => current + b.ToString("X2"));
            var prefix = hashStr[..5];

            var request = new HttpRequestMessage(HttpMethod.Get, "range/" + prefix);

            using var client = _httpClientFactory.CreateClient(Name);
            var result = await client.SendAsync(request);
            result.EnsureSuccessStatusCode();

            var hashes = ProcessResponse(await result.Content.ReadAsStringAsync());
            var hashList = hashes.Keys.Select(i => prefix + i);

            return hashList.Contains(hashStr);
        }

        public Task<bool> CheckHashAsync<T>(T hash) where T : HashAlgorithm
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, string> ProcessResponse(string response)
        {
            return response
                .Split(new[] { ' ', '\n', '\t' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(i =>
                {
                    var kvp = i.Split(":");
                    return new KeyValuePair<string, string>(kvp[0], kvp[1]);
                }).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}