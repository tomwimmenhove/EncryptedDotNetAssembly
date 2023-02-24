using SharedInterfaces;
using System.IO;

namespace load
{
    class Program
    {
        private const string Password = "SomeVerySecretPassword";
        private const string AssemblyResourceName = "load.Resources.EncryptedAssembly.data";
        private const string SaltResourceName = "load.Resources.EncryptedAssemblySalt.data";
        private const string SecretStuffClassName = "hello.SecretStuff";

        private const string AssemblyFileName = @"../hello/bin/release/net5.0/hello.dll";
        private const string AssemblyResourceFileName = "../load/Resources/EncryptedAssembly.data";
        private const string SaltResourceFileName = "../load/Resources/EncryptedAssemblySalt.data";

        static void Encrypt()
        {
            using var inFile = File.Open(AssemblyFileName, FileMode.Open);
            using var outFile = File.Open(AssemblyResourceFileName, FileMode.Create);
            using var saltFile = File.Open(SaltResourceFileName, FileMode.Create);

            Encryption.Encrypt(inFile, outFile, saltFile, new KeyIvPair(Password));
        }

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "encrypt")
            {
                Encrypt();
                return;
            }

            var loader = new Loader(KeyIvPair.SaltFromResource(Password, SaltResourceName));
            var assembly = loader.LoadAssemblyFromResource(AssemblyResourceName);

            var secretStuff = loader.CreateTypeFromAssembly<ISecretInterface>(assembly, SecretStuffClassName, 42);

            secretStuff.DoCoolShit(1000);
        }
    }
}
