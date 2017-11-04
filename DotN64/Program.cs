using System.IO;

namespace DotN64
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var nintendo64 = new Nintendo64();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg)
                {
                    case "--pif-rom":
                        nintendo64.RCP.PI.BootROM = File.ReadAllBytes(args[++i]);
                        break;
                    default:
                        nintendo64.Cartridge = Cartridge.FromFile(new FileInfo(arg));
                        break;
                }
            }

            nintendo64.PowerOn();
        }
    }
}
