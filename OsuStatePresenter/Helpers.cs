﻿namespace OsuStatePresenter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;

    internal static class Helpers
    {
        private static readonly Random Rnd = new Random();

        internal static int Rand(int from, int to)
        {
            return Rnd.Next(from, to + 1);
        }

        internal static string RandomStringFrom(params string[] strings)
        {
            int n = strings.Length;

            int randIndex = Rand(0, n - 1);

            return strings[randIndex];
        }

        internal static string CurrentExeDirectory()
        {
            return Path.Combine(Environment.CurrentDirectory);
        }

        internal static string GetProcessDirectory(string processName)
        {
            // TODO: BUG - Fix: https://stackoverflow.com/a/5497123 - "There is one catch with this API, if you are running this code in 32 bit application, you'll not be able to access 64-bit application paths, so you'd have to compile and run you app as 64-bit application (Project Properties → Build → Platform Target → x64)."
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (process.ProcessName.Equals(processName))
                {
                    string fullPath = process.MainModule.FileName;

                    return Path.GetDirectoryName(fullPath) + "\\";
                }
            }

            return string.Empty;
        }

        internal static string GetProcessDirectoryFor32And64BitPrograms(string processName)
        {
            // TODO: OPTIMIZE - shouldn't need to iterate over every process.
            const string WmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(WmiQueryString))
            using (ManagementObjectCollection results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                {
                    // Do what you want with the Process, Path, and CommandLine
                    if (item.Process.ProcessName.Equals(processName))
                    {
                        return Path.GetDirectoryName(item.Path) + "\\";
                    }
                }
            }

            return string.Empty;
        }

        internal static bool FloatIsEquivalentToInt(float f, int i, double precision = 0.0000001)
        {
            return Math.Abs(f - i) < precision;
        }
    }
}
