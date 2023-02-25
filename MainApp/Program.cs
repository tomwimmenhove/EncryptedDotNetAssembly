using SharedInterfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;
using Crypto;

namespace MainApp
{
    class Program
    {
        private const string DefaultPassword = "admin";
        private const string Password = "SomeVerySecretPassword";

        private const string ExampleUser = "SomeUser";
        private const string ExamplePass = "SomePassword";

        private const string AssemblyResourceName = "MainApp.Resources.EncryptedAssembly.data";
        private const string SaltResourceName = "MainApp.Resources.EncryptedAssemblySalt.data";
        private const string SecretStuffClassName = "SecretAssembly.SecretStuff";

        private const string AssemblyFileName = @"../SecretAssembly/bin/release/net5.0/SecretAssembly.dll";
        private const string AssemblyResourceFileName = "Resources/EncryptedAssembly.data";
        private const string SaltResourceFileName = "Resources/EncryptedAssemblySalt.data";

        static async Task Configure(PasswordStoreClient client)
        {
            await client.SetPasswordAsync(
                new SetPasswordDto { Old = DefaultPassword, New = Password });
            await client.AddPasswordAsync(
                new AddPasswordDto { MainPass = Password, UserName = ExampleUser, UserPass = ExamplePass });

            using var inFile = File.Open(AssemblyFileName, FileMode.Open);
            using var outFile = File.Open(AssemblyResourceFileName, FileMode.Create);
            using var saltFile = File.Open(SaltResourceFileName, FileMode.Create);

            var keyIvPair = await GetMainIvPair(client, ExampleUser, ExamplePass);

            saltFile.Write(keyIvPair.Salt);
            Encryption.Encrypt(inFile, outFile, keyIvPair);
        }

        public static async Task<KeyIvPair> GetMainIvPair(PasswordStoreClient client, string username, string password)
        {
            var userEntry = await client.GetUserAsync(username);

            var keyIvPair = new KeyIvPair(password, userEntry.UserPassSalt);
            var decryptedMainKeyIvPair = Crypto.Encryption.Decrypt(userEntry.EncryptedMainKeyIvpair, keyIvPair);

            return new KeyIvPair(decryptedMainKeyIvPair);
        }

        static async Task Main(string[] args)
        {
            var client = new PasswordStoreClient("http://localhost:5102/", new HttpClient());

            if (args.Length > 0)
            {
                if (args.Length == 1 && args[0] == "configure")
                {
                    await Configure(client);
                    return;
                }

                Console.Error.WriteLine("Invalid command line option. Only \"configure\" is currently supported");
                return;
            }

            var executingAssembly = Assembly.GetExecutingAssembly();
            var mainKeyIvPair = await GetMainIvPair(client, ExampleUser, ExamplePass);

            var loader = new Loader(mainKeyIvPair);
            var assembly = loader.LoadAssemblyFromResource(AssemblyResourceName);

            var secretStuff = loader.CreateTypeFromAssembly<ISecretInterface>(assembly, SecretStuffClassName, 42);

            secretStuff.DoCoolShit(1000);
        }
    }
}
