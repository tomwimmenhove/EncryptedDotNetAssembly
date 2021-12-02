using System;
using System.IO;
using System.Security.Cryptography;

namespace load
{
    public class Decryptor
    {
        public static void Decrypt(Stream inStream, Stream outStream, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csDecrypt = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.CopyTo(outStream);
                }
            }
        }
    }
}