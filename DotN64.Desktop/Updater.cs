using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;

namespace DotN64.Desktop
{
    public static class Updater
    {
        #region Fields
        private const string StagingDirectory = ".update", ProjectName = nameof(DotN64), NewFileExtension = ".new";
        #endregion

        #region Properties
        private static string InstallDirectory => AppDomain.CurrentDomain.BaseDirectory;

        private static string PlatformSuffix => string.Join(".", Environment.OSVersion.Platform, Environment.Is64BitOperatingSystem ? "64" : "32");

        /// <summary>
        /// Gets the available release streams.
        /// </summary>
        public static IEnumerable<string> Streams
        {
            get
            {
                using (var client = new WebClient())
                using (var reader = new StreamReader(client.OpenRead(new Uri(Program.Website, $"{ProjectName}/Download/streams"))))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks for a new update.
        /// </summary>
        public static Version Check(string releaseStream = null)
        {
            if (releaseStream == null)
                releaseStream = Program.ReleaseStream;

            using (var client = new WebClient())
            {
                var remoteVersion = new Version(client.DownloadString(new Uri(Program.Website, $"{ProjectName}/Download/{releaseStream}/version")));
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                return remoteVersion > currentVersion ? remoteVersion : null;
            }
        }

        /// <summary>
        /// Downloads the latest update.
        /// </summary>
        public static void Download(string releaseStream = null)
        {
            if (releaseStream == null)
                releaseStream = Program.ReleaseStream;

            var updateDirectory = new DirectoryInfo(Path.Combine(InstallDirectory, StagingDirectory));

            if (updateDirectory.Exists)
                updateDirectory.Delete(true);

            updateDirectory.Create();
            updateDirectory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            using (var client = new WebClient())
            {
                using (var archive = new ZipArchive(client.OpenRead(new Uri(Program.Website, $"{ProjectName}/Download/{releaseStream}/{ProjectName}.zip"))))
                {
                    archive.ExtractToDirectory(updateDirectory.FullName);
                }

                try
                {
                    using (var archive = new ZipArchive(client.OpenRead(new Uri(Program.Website, $"{ProjectName}/Download/{releaseStream}/{ProjectName}.{PlatformSuffix}.zip"))))
                    {
                        archive.ExtractToDirectory(updateDirectory.FullName);
                    }
                }
                catch (WebException e) when ((e.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound) { }
            }
        }

        /// <summary>
        /// Applies the update.
        /// </summary>
        public static void Apply()
        {
            var updateDirectory = new DirectoryInfo(Path.Combine(InstallDirectory, StagingDirectory));
            var executableName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            var isWindow = Environment.OSVersion.Platform == PlatformID.Win32NT;
            var shouldRestart = false;

            foreach (var file in updateDirectory.GetFiles())
            {
                var destination = Path.Combine(InstallDirectory, file.Name);

                if (isWindow && file.Name == executableName)
                {
                    file.MoveTo(destination + NewFileExtension);

                    shouldRestart = true;
                    continue;
                }

                File.Delete(destination);
                file.MoveTo(destination);
            }

            updateDirectory.Delete(true);

            if (isWindow && shouldRestart)
                ApplyWindowsUpdate(executableName);
        }

        private static void ApplyWindowsUpdate(string executableName)
        {
            var newExecutableName = executableName + NewFileExtension;
            var scriptFile = new FileInfo("CompleteUpdate.cmd");
            var processID = Process.GetCurrentProcess().Id;

            using (var writer = scriptFile.CreateText())
            {
                writer.WriteLine(":CHECK");
                writer.WriteLine("timeout /t 1");
                writer.WriteLine($"tasklist /fi \"pid eq {processID}\" | find \"{processID}\"");

                writer.WriteLine("if errorlevel 1 goto UPDATE");

                writer.WriteLine("goto CHECK");

                writer.WriteLine(":UPDATE");
                writer.WriteLine($"move /Y \"{newExecutableName}\" \"{executableName}\"");
                writer.WriteLine($"start \"\" \"{executableName}\"");
                writer.WriteLine($"del /A:H %0");
            }

            scriptFile.Attributes |= FileAttributes.Hidden;

            Process.Start(new ProcessStartInfo(scriptFile.FullName)
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
            Environment.Exit(0);
        }
        #endregion
    }
}
