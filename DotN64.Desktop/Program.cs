using System;
using System.IO;
using System.Reflection;
using DotN64.Desktop;

[assembly: AssemblyTitle(nameof(DotN64))]
[assembly: AssemblyDescription("Nintendo 64 emulator.")]
[assembly: AssemblyVersion("0.0.*")]
[assembly: AssemblyReleaseStream("master")]
namespace DotN64.Desktop
{
    using Diagnostics;

    internal static class Program
    {
        #region Properties
        private static DateTime BuildDate
        {
            get
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;

                return new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            }
        }

        public static string ReleaseStream => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyReleaseStreamAttribute>().Stream;

        public static Uri Website { get; } = new Uri("https://nabile.duckdns.org/DotN64");
        #endregion

        #region Methods
        private static void Main(string[] args)
        {
            var options = new Options();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch (arg)
                {
                    case "--pif-rom":
                        options.BootROM = args[++i];
                        break;
                    case "--debug":
                    case "-d":
                        options.UseDebugger = true;
                        break;
                    case "--update":
                    case "-u":
                        switch (args.Length - 1 > i ? args[++i] : null)
                        {
                            case "check":
                            case "c":
                                Check();
                                return;
                            case "list":
                            case "l":
                                foreach (var stream in Updater.Streams)
                                {
                                    Console.WriteLine($"{stream}{(stream == ReleaseStream ? " (current)" : string.Empty)}");
                                }
                                return;
                            case "stream":
                            case "s":
                                arg = args[++i];

                                Update(arg, arg != ReleaseStream);
                                break;
                            default:
                                Update();
                                break;
                        }
                        break;
                    case "--repair":
                    case "-r":
                        Repair();
                        break;
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return;
                    default:
                        options.Cartridge = arg;
                        break;
                }
            }

            if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory).Length <= 1) // Fresh install.
                Repair();

            Run(options);
        }

        private static bool Check(string releaseStream = null)
        {
            Console.Write("Checking for updates... ");

            var newVersion = Updater.Check(releaseStream);

            if (newVersion == null)
            {
                Console.WriteLine("Already up to date.");
                return false;
            }

            Console.WriteLine($"New version available: {newVersion}.");
            return true;
        }

        private static void Update(string releaseStream = null, bool force = false)
        {
            if (!force && !Check(releaseStream))
                return;

            Console.WriteLine("Downloading update...");
            Updater.Download(releaseStream);

            Console.WriteLine("Applying update...");
            Updater.Apply();
        }

        private static void Repair() => Update(force: true);

        private static void Run(Options options)
        {
            var nintendo64 = new Nintendo64();
            Debugger debugger = null;

            if (options.BootROM != null)
                nintendo64.PIF.BootROM = File.ReadAllBytes(options.BootROM);

            if (options.UseDebugger)
                debugger = new Debugger(nintendo64);

            if (options.Cartridge != null)
                nintendo64.Cartridge = Cartridge.FromFile(new FileInfo(options.Cartridge));

            nintendo64.PowerOn();

            if (debugger == null)
                nintendo64.Run();
            else
                debugger.Run(true);
        }

        private static void ShowInfo()
        {
            var assembly = Assembly.GetEntryAssembly();
            var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            var version = assembly.GetName().Version;
            var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

            Console.WriteLine($"{title} v{version} ({BuildDate}){(description != null ? $": {description}" : string.Empty)}");
            Console.WriteLine(Website);
        }

        private static void ShowHelp()
        {
            ShowInfo();
            Console.WriteLine();
            Console.WriteLine($"Usage: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} [Options] [ROM image]");
            Console.WriteLine();
            Console.WriteLine("ROM image: Opens the file as a game cartridge.");
            Console.WriteLine("Options:");
            Console.WriteLine("\t--pif-rom <path>: Loads the PIF's boot ROM into the machine.");
            Console.WriteLine("\t-d, --debug: Launches the debugger for the Nintendo 64's CPU.");
            Console.WriteLine("\t-u, --update [action]: Updates the program.");
            Console.WriteLine("\t\t[action = 'check', 'c']: Checks for a new update.");
            Console.WriteLine("\t\t[action = 'list', 'l']: Lists the release streams available for download.");
            Console.WriteLine("\t\t[action = 'stream', 's'] <stream>: Downloads an update from the specified release stream.");
            Console.WriteLine("\t-r, --repair: Repairs the installation by redownloading the full program.");
            Console.WriteLine("\t-h, --help: Shows this help.");
        }
        #endregion

        #region Structures
        private struct Options
        {
            #region Fields
            public bool UseDebugger;
            public string BootROM, Cartridge;
            #endregion
        }
        #endregion
    }
}
