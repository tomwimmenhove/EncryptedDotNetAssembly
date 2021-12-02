using System;
using System.IO;
using System.Reflection;

namespace load
{
    public static class Loader
    {
        private static byte[] key = Convert.FromBase64String("N8xir1+t3Bti4HiFePITBb2+ejcn3Qb2BYQdbCIEQSU=");
        private static byte[] iv = Convert.FromBase64String("LGlG8Q5B3rU3Epr1SDAlCQ==");

        public static Assembly LoadAssemblyFromResource(string resourceName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            using (var memStream = new MemoryStream())
            using (var resourceStream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                Decryptor.Decrypt(resourceStream, memStream, key, iv);

                var assembly = Assembly.Load(memStream.GetBuffer());
                if (assembly == null)
                {
                    throw new FileNotFoundException($"Could not find resource: {resourceName}");
                }

                return assembly;
            }
        }

        public static T CreateTypeFromAssembly<T>(Assembly assembly, string typeName, params object[] args)
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
