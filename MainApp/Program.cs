using SharedInterfaces;
using System.IO;

namespace MainApp
{
    class Program
    {
        private const string Password = "SomeVerySecretPassword";
        private const string AssemblyResourceName = "MainApp.Resources.EncryptedAssembly.data";
        private const string SaltResourceName = "MainApp.Resources.EncryptedAssemblySalt.data";
        private const string SecretStuffClassName = "SecretAssembly.SecretStuff";

        private const string AssemblyFileName = @"../SecretAssembly/bin/release/net5.0/SecretAssembly.dll";
        private const string AssemblyResourceFileName = "Resources/EncryptedAssembly.data";
        private const string SaltResourceFileName = "Resources/EncryptedAssemblySalt.data";

        static void Encrypt()
        {
            using var inFile = File.Open(AssemblyFileName, FileMode.Open);
            using var outFile = File.Open(AssemblyResourceFileName, FileMode.Create);
            using var saltFile = File.Open(SaltResourceFileName, FileMode.Create);

            var keyIvPair = new KeyIvPair(Password);

            saltFile.Write(keyIvPair.Salt);
            Encryption.Encrypt(inFile, outFile, keyIvPair);
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
