namespace OsuStatePresenter.Nodes.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using BMAPI.v1;
    using BMAPI.v1.HitObjects;

    using Newtonsoft.Json;

    // ReSharper disable InconsistentNaming
    internal class OppaiCalc
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool oppaiFilesCopies;
        private readonly Beatmap beatmap;
        private readonly TimeSpan endDelayTime = TimeSpan.FromMilliseconds(value: 5000);

        public OppaiCalc(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        public double CalculatePP(int time = -1)
        {
            int currentObjectNumber = this.GetCurrentObjectNumber(this.beatmap, time);

            if (!oppaiFilesCopies)
            {
                CopyOppaiResourcesToOutputDir();
                oppaiFilesCopies = true;
            }

            // TODO: OPTIMIZE - don't re-launch the process every time
            var p = new Process
            {
                StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = $"{Helpers.CurrentExeDirectory()}\\oppai.exe",
                        Arguments = $"\"{this.beatmap.Filename}\" -ojson -end{currentObjectNumber}",
                        CreateNoWindow = true,
                        RedirectStandardError = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true
                    }
            };

            // TODO: UNFINISHED - Include other play-data e.g. gameMode, mods, nx100, nx50, misses

            // ReSharper disable once CommentTypo
            // _logger.Info($"PP command: \"{_beatmap.Filename}\" -ojson -end{currentObjectNumber}");
            p.Start();

            string jsonOutput = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            return GetPPFromOppaiOutput(jsonOutput);
        }

        private static double GetPPFromOppaiOutput(string jsonOutput)
        {
            // ReSharper disable CommentTypo
            // {"oppai_version":"3.2.0","code":200,"errstr":"no error","artist":"Mitsuki Kotono","artist_unicode":"þ¥Äµ£êþÉ┤Úƒ│","title":"Fuyu ni Saku Hana","title_unicode":"Õå¼Òü½ÕÆ▓ÒüÅÞÅ»","creator":"Nardoxyribonucleic","version":"Kantan","mods_str":"","mods":0,"od":3,"ar":3,"cs":5,"hp":5,"combo":380,"max_combo":380,"num_circles":380,"num_sliders":0,"num_spinners":1,"misses":0,"score_version":1,"stars":1.4633538722991943,"speed_stars":1.4633538722991943,"aim_stars":0,"aim_pp":0,"speed_pp":9.6786031723022461,"acc_pp":60.697185516357422,"pp":74.775711059570313}
            var o = JsonConvert.DeserializeObject<PPObject>(jsonOutput);

            return o.PP;
        }

        private static void CopyOppaiResourcesToOutputDir()
        {
            Logger.Debug("Copying oppai resource files...");

            // byte[] oppaiDll = Properties.Resources.OppaiDLL;
            // byte[] oppaiExe = Properties.Resources.OppaiExe;
            // byte[] oppaiLib = Properties.Resources.OppaiLib;
            string path = Path.Combine(Helpers.CurrentExeDirectory(), "oppai.exe");
            File.WriteAllBytes(path, Properties.Resources.OppaiExe);

            path = Path.Combine(Helpers.CurrentExeDirectory(), "oppai.dll");
            File.WriteAllBytes(path, Properties.Resources.OppaiDLL);

            path = Path.Combine(Helpers.CurrentExeDirectory(), "oppai.lib");
            File.WriteAllBytes(path, Properties.Resources.OppaiLib);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private int GetCurrentObjectNumber(Beatmap targetBeatmap, int currentMapTime)
        {
            var hitObjects = new List<CircleObject>(targetBeatmap.HitObjects);

            if (hitObjects.Count < 1)
            {
                return 1;
            }

            if (currentMapTime <= 0)
            {
                return 1;
            }

            // TODO: UNFINISHED (hacky temp solution) - Some other solution for this problem instead of delaying the end time.
            if (currentMapTime + this.endDelayTime.Milliseconds >= hitObjects.Last().StartTime)
            {
                // _logger.Info($"c: {currentMapTime}, {hitObjects.Last().StartTime}, {hitObjects.Count}");
                return hitObjects.Count;
            }

            hitObjects.Reverse();

            for (var i = 0; i < hitObjects.Count; i++)
            {
                CircleObject ho = hitObjects[i];

                if (currentMapTime >= ho.StartTime)
                {
                    // _logger.Info($"d: {currentMapTime}, {ho.StartTime}, {i}, {hitObjects.Count}, {hitObjects.Count - i}");
                    return hitObjects.Count - i;
                }
            }

            return 1;
        }
    }
}