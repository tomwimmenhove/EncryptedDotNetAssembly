using System.Linq;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Crypto
{
    public class KeyIvPair
    {
        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }
        public byte[] Salt { get; set; }

        public KeyIvPair() { }

        public KeyIvPair(byte[] key, byte[] iv, byte[] salt)
        {
            Key = key;
            Iv = iv;
            Salt = salt;
        }

        public KeyIvPair(byte[] bytes)
        {
            Key = bytes.Take(32).ToArray();
            Iv = bytes.Skip(32).Take(16).ToArray();
        }

        public KeyIvPair(string password, byte[] salt = null)
        {
            System.Diagnostics.Debug.Assert(salt == null || salt.Length == 8);
            
            var deriveBytes = salt != null
                ? new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256)
                : new Rfc2898DeriveBytes(password, 8, 1000, HashAlgorithmName.SHA256);

            Key = deriveBytes.GetBytes(32);
            Iv = deriveBytes.GetBytes(16);
            Salt = deriveBytes.Salt;
        }

        public static KeyIvPair SaltFromResource(Assembly assembly, string password, string saltResourceName)
        {
            var salt = GetSaltFromResource(assembly, saltResourceName);

            return new KeyIvPair(password, salt);
        }

        public byte[] GetAsBytes() => Key.Concat(Iv).ToArray();

        public bool Matches(KeyIvPair other) =>
            Key.SequenceEqual(other.Key) &&
            Iv.SequenceEqual(other.Iv);

        private static byte[] GetSaltFromResource(Assembly assembly, string resourceName)
        {
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
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
