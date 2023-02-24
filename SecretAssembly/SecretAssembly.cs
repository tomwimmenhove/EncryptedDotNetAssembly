using System;
using SharedInterfaces;

namespace SecretAssembly
{
    public class SecretStuff : ISecretInterface
    {
        private int _x;

        public SecretStuff(int x)
        {
            _x = x;
        }

        public void DoCoolShit(int y)
        {
            Console.WriteLine($"Hello World! (x={_x}, y={y})");
        }
    }
}
