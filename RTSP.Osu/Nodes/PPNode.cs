using Newtonsoft.Json.Linq;
using RTSP.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "PPNow")]
    class PPNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(StatusNode), out var statusNode);
            var status = (string)statusNode?.GetValue();

            if (!status.Equals("Playing"))
                return null;

            Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);
            Preceders.TryGetValue(typeof(ModsNode), out var modsNode);
            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);

            var beatmap = (BMAPI.v1.Beatmap)beatmapNode?.GetValue() ?? null;
            var mods = (string)modsNode?.GetValue() ?? string.Empty;
            var currentMapTime = (int?)mapTimeNode?.GetValue() ?? -1;
            var osuFileLocation = ((BMAPI.v1.Beatmap)beatmapNode?.GetValue())?.Filename ?? string.Empty;
            var playMode = (int?)_memoryReader.ReadPlayedGameMode() ?? -1;

            //SetCurrentMap(beatmap, mods, osuFileLocation, playMode);
            //_memoryReader.GetPlayData(Play);
            //var ppNow = PPIfBeatmapWouldEndNow();

            // TODO: See PpIfRestFcd() in https://github.com/Piotrekol/StreamCompanion/blob/master/plugins/OsuMemoryEventSource/RawMemoryDataProcessor.cs

            int currentObjectNumber = GetCurrentBeatmapObjectNumber(beatmap, currentMapTime);


            // TODO: Include other playdata like nx100 nx50, mods, misses
            // TODO: Extract this out and don't re-launch the process every time

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = @"X:\Users\waffle\Documents\Code\RTSP\rtspv5\RTSP.Osu\vendor\oppai\3.2.0\oppai.exe";
            p.StartInfo.Arguments = $"\"{beatmap.Filename}\" -ojson -end{currentObjectNumber}";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.

            string jsonOutput = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            double ppNow = GetPPFromOppaiOutput(jsonOutput);

            return await Task.FromResult(ppNow);
        }

        private double GetPPFromOppaiOutput(string jsonOutput)
        {
            /*
{"oppai_version":"3.2.0","code":200,"errstr":"no error","artist":"Mitsuki Kotono","artist_unicode":"þ¥Äµ£êþÉ┤Úƒ│","title":"Fuyu ni Saku Hana","title_unicode":"Õå¼Òü½ÕÆ▓ÒüÅÞÅ»","creator":"Nardoxyribonucleic","version":"Kantan","mods_str":"","mods":0,"od":3,"ar":3,"cs":5,"hp":5,"combo":380,"max_combo":380,"num_circles":380,"num_sliders":0,"num_spinners":1,"misses":0,"score_version":1,"stars":1.4633538722991943,"speed_stars":1.4633538722991943,"aim_stars":0,"aim_pp":0,"speed_pp":9.6786031723022461,"acc_pp":60.697185516357422,"pp":74.775711059570313}
             */
            dynamic data = JObject.Parse(jsonOutput);
            var pp = data?.pp?.Value;

            return pp;
        }

        private int GetCurrentBeatmapObjectNumber(BMAPI.v1.Beatmap beatmap, int currentMapTime)
        {
            // TODO: FIx broken min/max edge cases

            var hitObjects = beatmap.HitObjects;
            hitObjects.Reverse();

            int o = hitObjects.Count - 1;

            for (int i = 1; i <= hitObjects.Count; i++)
            {
                var hitObject = hitObjects[i-1];

                if (currentMapTime >= hitObject.StartTime)
                {
                    o = i;
                    break;
                }
            }

            if (o <= 1 || o > hitObjects.Count)
                return 1;

            //_logger.Info($"{hitObjects.Count} - {o} = {hitObjects.Count - o}");

            return hitObjects.Count - o; 
        }







        //private PpCalculator.PpCalculator _ppCalculator = null;
        //public PlayContainer Play { get; set; } = new PlayContainer();
        //private Beatmap _currentBeatmap = null;
        //public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        //public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        //public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        //public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        //Dictionary<string, double> attribs = new Dictionary<string, double>();

        //public double PPIfBeatmapWouldEndNow()
        //{

        //    if (_ppCalculator != null && Play.Time > 0)
        //        try
        //        {
        //            _ppCalculator.Goods = Play.C100;
        //            _ppCalculator.Mehs = Play.C50;
        //            _ppCalculator.Misses = Play.CMiss;
        //            _ppCalculator.Combo = Play.MaxCombo;
        //            _ppCalculator.Score = Play.Score;
        //            var pp = _ppCalculator.Calculate(Play.Time, attribs);

        //            switch (_currentBeatmap.Mode)
        //            {
        //                case GameMode.Taiko:
        //                case GameMode.Mania:
        //                    StrainPPIfBeatmapWouldEndNow = attribs["Strain"];
        //                    AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
        //                    AimPPIfBeatmapWouldEndNow = double.NaN;
        //                    SpeedPPIfBeatmapWouldEndNow = double.NaN;
        //                    break;
        //                default:
        //                    AimPPIfBeatmapWouldEndNow = attribs["Aim"];
        //                    SpeedPPIfBeatmapWouldEndNow = attribs["Speed"];
        //                    AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
        //                    break;
        //            }

        //            attribs.Clear();

        //            return pp;
        //        }
        //        catch { }
        //    AimPPIfBeatmapWouldEndNow = double.NaN;
        //    SpeedPPIfBeatmapWouldEndNow = double.NaN;
        //    AccPPIfBeatmapWouldEndNow = double.NaN;
        //    StrainPPIfBeatmapWouldEndNow = double.NaN;
        //    return double.NaN;
        //}

        //private string _currentMods;
        //private string _currentOsuFileLocation = null;

        //public void SetCurrentMap(Beatmap beatmap, string mods, string osuFileLocation, int playMode)
        //{
        //    if (beatmap == null)
        //    {
        //        _ppCalculator = null;
        //        return;
        //    }

        //    _currentBeatmap = beatmap;
        //    _currentMods = mods;
        //    _currentOsuFileLocation = osuFileLocation;

        //    if (playMode < 0)
        //        return;

        //    // _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, _ppCalculator);
        //    //int playModeConverted = ConvertPlayMode_BMAPIToSC(playMode);
        //    _ppCalculator = PpCalculator.PpCalculatorHelpers.GetPpCalculator(playMode, _ppCalculator);

        //    if (_ppCalculator == null)
        //        return;

        //    _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //    _ppCalculator.PreProcess(osuFileLocation);
        //}

        ////private int ConvertPlayMode_BMAPIToSC(GameMode playMode)
        ////{
        ////    switch (playMode)
        ////    {
        ////        default:
        ////            throw new ArgumentException("Invalid playMode provided.");
        ////        case BMAPI.v1.GameMode.osu:
        ////            return 0;
        ////        case BMAPI.v1.GameMode.Taiko:
        ////            return 1;
        ////        case BMAPI.v1.GameMode.CatchtheBeat:
        ////            return 2;
        ////        case BMAPI.v1.GameMode.Mania:
        ////            return 3;
        ////    }
        ////}
    }
}
