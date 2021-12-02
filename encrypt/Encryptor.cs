using System;
using System.IO;
using System.Security.Cryptography;

namespace load
{
    public class Encryptor
    {
        public static void Encrypt(Stream inStream, Stream outStream, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csEncrypt = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                {
                    inStream.CopyTo(csEncrypt);
                }
            }
        }
    }
}