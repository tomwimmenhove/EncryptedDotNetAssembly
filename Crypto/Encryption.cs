using System.IO;
using System.Security.Cryptography;

namespace Crypto
{
    public static class Encryption
    {
        public static void Encrypt(Stream inStream, Stream outStream, KeyIvPair keyIvPair)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyIvPair.Key;
                aesAlg.IV = keyIvPair.Iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csEncrypt = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                {
                    inStream.CopyTo(csEncrypt);
                }
            }
        }

        public static byte[] Encrypt(byte[] input, KeyIvPair keyIvPair)
        {
            using (var plainStream = new MemoryStream(input))
            using (var encryptedStream = new MemoryStream())
            {
                Encryption.Encrypt(plainStream, encryptedStream, keyIvPair);
                return encryptedStream.ToArray();
            }
        }

        public static void Decrypt(Stream inStream, Stream outStream, KeyIvPair keyIvPair)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyIvPair.Key;
                aesAlg.IV = keyIvPair.Iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csDecrypt = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.CopyTo(outStream);
                }
            }
        }
 
        public static byte[] Decrypt(byte[] input, KeyIvPair keyIvPair)
        {
            using (var encryptedStream = new MemoryStream(input))
            using (var plainStream = new MemoryStream())
            {
                Encryption.Decrypt(encryptedStream, plainStream, keyIvPair);
                return plainStream.ToArray();
            }
        }
    }
}