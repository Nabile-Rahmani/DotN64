using System.IO;

namespace N64Emu
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
                    default:
                        nintendo64.Insert(Cartridge.FromFile(new FileInfo(arg)));
                        break;
                }
            }

            nintendo64.Initialise();
        }
    }
}
