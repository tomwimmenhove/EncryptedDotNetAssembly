using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace MainApp
{
    public class KeyIvPair
    {
        public byte[] Key { get; }
        public byte[] Iv { get; }
        public byte[] Salt { get; }

        public KeyIvPair(byte[] key, byte[] iv, byte[] salt)
        {
            Key = key;
            Iv = iv;
            Salt = salt;
        }

        public KeyIvPair(string password, byte[] salt = null)
        {
            var deriveBytes = salt != null
                ? new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256)
                : new Rfc2898DeriveBytes(password, 8, 1000, HashAlgorithmName.SHA256);

            Key = deriveBytes.GetBytes(32);
            Iv = deriveBytes.GetBytes(16);
            Salt = deriveBytes.Salt;
        }

        public static KeyIvPair SaltFromResource(string password, string saltResourceName)
        {
            var salt = GetSaltFromResource(saltResourceName);

            return new KeyIvPair(password, GetSaltFromResource(saltResourceName));
        }

        private static byte[] GetSaltFromResource(string resourceName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            using (var resourceStream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found");
                }

                using var reader = new BinaryReader(resourceStream);

                return reader.ReadBytes((int)resourceStream.Length);
            }
        }
    }
}
