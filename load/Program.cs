using SharedInterfaces;

namespace load
{
    class Program
    {
        private const string ResourceName = "load.Resources.EncryptedAssembly.data";
        private const string SecretStuffClassName = "hello.SecretStuff";
        
        static void Main(string[] args)
        {
            var assembly = Loader.LoadAssemblyFromResource(ResourceName);

            var secretStuff = Loader.CreateTypeFromAssembly<ISecretInterface>(assembly, SecretStuffClassName, 42);

            secretStuff.DoCoolShit(1000);
        }
    }
}
