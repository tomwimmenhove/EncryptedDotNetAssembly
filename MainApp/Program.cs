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
        //private const string PasswordStoreClientUrl = "http://localhost:5102"; 
        private const string PasswordStoreClientUrl = "https://ycejk6mcstm2n5ob3ilzzjz5ky0obbki.lambda-url.eu-central-1.on.aws/"; 

        private const string AssemblyResourceName = "MainApp.Resources.EncryptedAssembly.data";
        private const string SecretStuffClassName = "SecretAssembly.SecretStuff";

        private const string AssemblyFileName = @"../SecretAssembly/bin/release/net5.0/SecretAssembly.dll";
        private const string AssemblyResourceFileName = "Resources/EncryptedAssembly.data";

        static async Task Configure(PasswordStoreClient client)
        {
            Console.Write("Current main password: ");
            var mainPas = Console.ReadLine();
            Console.Write("New main password: ");
            var newMainPass = Console.ReadLine();

            Console.WriteLine("");
            Console.Write("Create user: ");
            var username = Console.ReadLine();
            Console.Write("User's password: ");
            var userPass = Console.ReadLine();

            await client.SetPasswordAsync(
                new SetPasswordDto { Old = mainPas, New = newMainPass });
            await client.AddUserAsync(
                new AddUserDto { MainPass = newMainPass, UserName = username, UserPass = userPass });

            using var inFile = File.Open(AssemblyFileName, FileMode.Open);
            using var outFile = File.Open(AssemblyResourceFileName, FileMode.Create);

            var keyIvPair = await GetMainIvPair(client, username, userPass);

            Encryption.Encrypt(inFile, outFile, keyIvPair);
        }

        public static async Task<KeyIvPair> GetMainIvPair(PasswordStoreClient client, string username, string password)
        {
            var userEntry = await client.GetUserAsync(username);

            var keyIvPair = new KeyIvPair(password, userEntry.UserPassSalt);
            var decryptedMainKeyIvPair = Crypto.Encryption.Decrypt(userEntry.EncryptedMainKeyIvPair, keyIvPair);

            return new KeyIvPair(decryptedMainKeyIvPair);
        }

        static async Task Main(string[] args)
        {
            var client = new PasswordStoreClient(PasswordStoreClientUrl, new HttpClient());

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

            Console.Write("User: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var userPass = Console.ReadLine();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var mainKeyIvPair = await GetMainIvPair(client, username, userPass);

            var loader = new Loader(mainKeyIvPair);
            var assembly = loader.LoadAssemblyFromResource(AssemblyResourceName);

            var secretStuff = loader.CreateTypeFromAssembly<ISecretInterface>(assembly, SecretStuffClassName, 42);

            secretStuff.DoCoolShit(1000);
        }
    }
}
