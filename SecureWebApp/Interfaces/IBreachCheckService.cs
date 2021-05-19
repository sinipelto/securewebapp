using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureWebApp.Interfaces
{
    public interface IBreachCheckService
    {
        /// <summary>
        /// Checks if a password is on a breached passwords list.
        /// Might use external services and/or databases to fetch the result, depending on the implementation.
        /// </summary>
        /// <param name="password">The user provided password to check for breaches</param>
        /// <returns></returns>
        Task<bool> CheckPasswordAsync(string password);

        /// <summary>
        /// Checks breaches for a password hash.
        /// Requires user to provide the hash implementation used for the password hash.
        /// </summary>
        /// <typeparam name="T">Type of the hash algorithm used for the password</typeparam>
        /// <param name="hash">The hash of the password to check</param>
        /// <returns></returns>
        Task<bool> CheckHashAsync<T>(T hash) where T : HashAlgorithm;
    }
}