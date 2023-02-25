using System;
using System.IO;
using System.Reflection;
using Crypto;

namespace MainApp
{
    public class Loader
    {
        private readonly KeyIvPair _keyIvPair;

        public Loader(KeyIvPair keyIvPair)
        {
            _keyIvPair = keyIvPair;
        }

        public Assembly LoadAssemblyFromResource(string resourceName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            using (var memStream = new MemoryStream())
            using (var resourceStream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found");
                }

                Encryption.Decrypt(resourceStream, memStream, _keyIvPair);

                var assembly = Assembly.Load(memStream.GetBuffer());
                if (assembly == null)
                {
                    throw new FormatException($"Failed to load assembly: {resourceName}");
                }

                return assembly;
            }
        }

        public T CreateTypeFromAssembly<T>(Assembly assembly, string typeName, params object[] args)
        {
            var type = assembly.GetType(typeName);
            if (type == null)
            {
                throw new FileNotFoundException($"Could not find type: {typeName}");
            }

            return (T) Activator.CreateInstance(type, args);
        }
    }
}
