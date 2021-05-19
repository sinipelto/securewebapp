using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SecureWebApp.Security
{
    public class KeyRing : ILookupProtectorKeyRing
    {
        private readonly IDictionary<string, string> _keyDictionary = new Dictionary<string, string>();

        public KeyRing(IHostingEnvironment hostingEnvironment)
        {
            // Create the keyring directory if one doesn't exist.
            var keyRingDirectory = Path.Combine(hostingEnvironment.ContentRootPath, "keyring");
            Directory.CreateDirectory(keyRingDirectory);

            var directoryInfo = new DirectoryInfo(keyRingDirectory);
            if (directoryInfo.GetFiles("*.key").Length == 0)
            {
                ProtectorAlgorithmHelper.GetAlgorithms(
                    ProtectorAlgorithmHelper.DefaultAlgorithm,
                    out SymmetricAlgorithm encryptionAlgorithm,
                    out KeyedHashAlgorithm signingAlgorithm,
                    out int derivationCount);
                encryptionAlgorithm.GenerateKey();

                var keyAsString = Convert.ToBase64String(encryptionAlgorithm.Key);
                var keyId = Guid.NewGuid().ToString();
                var keyFileName = Path.Combine(keyRingDirectory, keyId + ".key");
                using (var file = File.CreateText(keyFileName))
                {
                    file.WriteLine(keyAsString);
                }

                _keyDictionary.Add(keyId, keyAsString);

                CurrentKeyId = keyId;

                encryptionAlgorithm.Clear();
                encryptionAlgorithm.Dispose();
                signingAlgorithm.Dispose();
            }
            else
            {
                var filesOrdered = directoryInfo.EnumerateFiles()
                                    .OrderByDescending(d => d.CreationTime)
                                    .Select(d => d.Name)
                                    .ToList();

                foreach (var fileName in filesOrdered)
                {
                    var keyFileName = Path.Combine(keyRingDirectory, fileName);
                    var key = File.ReadAllText(keyFileName);
                    var keyId = Path.GetFileNameWithoutExtension(fileName);
                    _keyDictionary.Add(keyId, key);
                    CurrentKeyId = keyId;
                }
            }
        }

        public string this[string keyId] => _keyDictionary[keyId];

        public string CurrentKeyId
        {
            get;
        }

        public IEnumerable<string> GetAllKeyIds()
        {
            return _keyDictionary.Keys;
        }
    }
}