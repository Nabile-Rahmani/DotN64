using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using DotN64.Desktop;

[assembly: AssemblyTitle(nameof(DotN64))]
[assembly: AssemblyDescription("Nintendo 64 emulator.")]
[assembly: AssemblyVersion("0.0.*")]
[assembly: AssemblyReleaseStream("master")]
namespace DotN64.Desktop
{
    using Diagnostics;
    using SDL;

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
                    case "--ipl":
                        options.IPL = args[++i];
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
                                    Console.WriteLine(stream + (stream == ReleaseStream ? " (default)" : string.Empty));
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
                    case "--no-video":
                        options.NoVideo = true;
                        break;
                    case "--video":
                    case "-v":
                        var resolution = args[++i].Split('x').Select(v => int.Parse(v));
                        options.VideoResolution = new Point(resolution.First(), resolution.Last());

                        switch (args[++i])
                        {
                            case "fullscreen":
                            case "f":
                                options.FullScreenVideo = true;
                                break;
                            case "borderless":
                            case "b":
                                options.BorderlessWindow = true;
                                break;
                            case "windowed":
                            case "w":
                                options.FullScreenVideo = false;
                                break;
                        }
                        break;
                    case "--info":
                        ShowCartridgeInfo(args, ref i);
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

        private static Cartridge LoadCartridge(string path)
        {
            var file = new FileInfo(path);

            if (file.Extension.ToLower() == ".zip")
            {
                using (var input = ZipFile.OpenRead(file.FullName).Entries.First().Open())
                using (var output = new MemoryStream())
                {
                    input.CopyTo(output);
                    return new Cartridge(output.ToArray());
                }
            }

            return new Cartridge(file);
        }

        private static void ShowCartridgeInfo(string[] args, ref int i)
        {
            var cartridge = LoadCartridge(args[++i]);

            Console.WriteLine($"Image name: {cartridge.ImageName}");
            Console.WriteLine($"ID: {cartridge.ID}");
            Console.WriteLine($"Version: {1.0f + (cartridge.Version & ((1 << 4) - 1) >> 4) + (cartridge.Version & ((1 << 4) - 1)) * 0.1f:0.0}");
            Console.WriteLine($"Media format: {cartridge.Format}");
            Console.WriteLine($"Region: {cartridge.Region}");
            Console.WriteLine($"Size: {cartridge.ROM.Length / (float)0x100000:0.##} MB");
            Console.WriteLine($"CRC: 0x{cartridge.CRC[0]:X8}, 0x{cartridge.CRC[1]:X8}");
            Console.WriteLine($"Boot address: 0x{cartridge.BootAddress:X8}");
            Console.WriteLine($"Clock rate: {cartridge.ClockRate}");
            Console.WriteLine($"Release: {cartridge.Release}");
        }

        private static void Run(Options options)
        {
            var nintendo64 = new Nintendo64();

            if (options.UseDebugger)
                nintendo64.Debugger = new Debugger(nintendo64);

            if (!options.NoVideo)
            {
                var window = new Window(nintendo64, size: options.VideoResolution)
                {
                    IsFullScreen = options.FullScreenVideo,
                    IsBorderless = options.BorderlessWindow
                };
                nintendo64.VideoOutput = window;
                nintendo64.CartridgeSwapped += (n, c) => window.Title = nameof(DotN64) + (c != null ? $" - {c.ImageName.Trim()}" : string.Empty);
            }

            if (options.IPL != null)
                nintendo64.PIF.IPL = File.ReadAllBytes(options.IPL);

            if (options.Cartridge != null)
                nintendo64.Cartridge = LoadCartridge(options.Cartridge);

            nintendo64.PowerOn();
            nintendo64.Run();

            (nintendo64.VideoOutput as IDisposable)?.Dispose();
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
            Console.WriteLine("\t--ipl <path>: Loads the PIF's boot ROM into the machine.");
            Console.WriteLine("\t-d, --debug: Launches the debugger for the Nintendo 64's CPU.");
            Console.WriteLine("\t-u, --update [action]: Updates the program.");
            Console.WriteLine("\t\t[action = 'check', 'c']: Checks for a new update.");
            Console.WriteLine("\t\t[action = 'list', 'l']: Lists the release streams available for download.");
            Console.WriteLine("\t\t[action = 'stream', 's'] <stream>: Downloads an update from the specified release stream.");
            Console.WriteLine("\t-r, --repair: Repairs the installation by redownloading the full program.");
            Console.WriteLine("\t-h, --help: Shows this help.");
            Console.WriteLine("\t-v, --video <width>x<height> <mode = 'fullscreen', 'f', 'borderless', 'b', 'windowed', 'w'>: Sets the window mode.");
            Console.WriteLine("\t--no-video: Disables the video output.");
            Console.WriteLine("\t--info <ROM image>: Displays header information for a game.");
        }
        #endregion

        #region Structures
        private struct Options
        {
            #region Fields
            public bool UseDebugger, NoVideo, FullScreenVideo, BorderlessWindow;
            public string IPL, Cartridge;
            public Point? VideoResolution;
            #endregion
        }
        #endregion
    }
}
