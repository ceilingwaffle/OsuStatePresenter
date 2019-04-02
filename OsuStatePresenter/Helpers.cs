using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

namespace OsuStatePresenter
{
    internal static class Helpers
    {
        static Random _rnd = new Random();

        internal static int Rand(int from, int to)
        {
            return _rnd.Next(from, to + 1);
        }

        /// <summary>
        /// Returns a random string from the given strings.
        /// </summary>
        /// <param name="strings">The strings to randomly choose one from.</param>
        /// <returns></returns>
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
            // TODO: Optimize - shouldn't need to iterate over every process.

            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
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
    }
}
