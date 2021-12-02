using System;
using System.IO;

namespace load
{
    class Program
    {
        private static byte[] key = Convert.FromBase64String("N8xir1+t3Bti4HiFePITBb2+ejcn3Qb2BYQdbCIEQSU=");
        private static byte[] iv = Convert.FromBase64String("LGlG8Q5B3rU3Epr1SDAlCQ==");

        static void Main(string[] args)
        {
            var sourceAssemblyFileName = @"../hello/bin/Release/net5.0/hello.dll";
            var resourseFileName = "../load/Resources/EncryptedAssembly.data";

            var inFile = File.Open(sourceAssemblyFileName, FileMode.Open);
            var outFile = File.Open(resourseFileName, FileMode.Create);

            Encryptor.Encrypt(inFile, outFile, key, iv);        }
    }
}
