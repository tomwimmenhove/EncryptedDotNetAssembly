using System.Text;
using System.Security.Cryptography;

namespace PasswordService
{
    public class MainPassStore
    {
        public byte[] Hash { get; set; } = Array.Empty<byte>();
        public byte[] Salt { get; set; } = Array.Empty<byte>();

        private static byte[] GenerateHash(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            passwordBytes.CopyTo(saltedPassword, 0);
            salt.CopyTo(saltedPassword, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(saltedPassword);
            }
        }

        public static MainPassStore Generate(string password)
        {
            var store = new MainPassStore();

            store.Salt = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(store.Salt);
            }

            using (var sha256 = SHA256.Create())
            {
                store.Hash = GenerateHash(password, store.Salt);
            }

            return store;
        }

        public bool Test(string password)
        {
            var saltedBytes = GenerateHash(password, Salt);

            return saltedBytes.SequenceEqual(Hash);
        }
    }
}
