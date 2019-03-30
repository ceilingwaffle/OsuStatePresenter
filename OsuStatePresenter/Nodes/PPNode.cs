using BMAPI.v1;
using BMAPI.v1.HitObjects;
using DependentValuePresentationFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "PPNow")]
    class PPNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            //  old PpCalculator code:
            //  See PpIfRestFcd() in https://github.com/Piotrekol/StreamCompanion/blob/master/plugins/OsuMemoryEventSource/RawMemoryDataProcessor.cs
            //SetCurrentMap(beatmap, mods, osuFileLocation, playMode);
            //_memoryReader.GetPlayData(Play);
            //double ppNow = PPIfBeatmapWouldEndNow();

            Preceders.TryGetValue(typeof(StatusNode), out var statusNode);
            var status = (string)statusNode?.GetValue();
            if (!status.Equals("Playing"))
                return null;

            Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);
            var beatmap = (BMAPI.v1.Beatmap)beatmapNode?.GetValue() ?? null;
            if (beatmap is null)
                return null;

            Preceders.TryGetValue(typeof(ModsNode), out var modsNode);
            var mods = (string)modsNode?.GetValue() ?? string.Empty;
            if (mods.Equals(String.Empty))
                return null;

            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            var currentMapTime = (int?)mapTimeNode?.GetValue() ?? -1;
            if (currentMapTime < 0)
                return null;

            var osuFileLocation = ((Beatmap)beatmapNode?.GetValue())?.Filename ?? string.Empty;
            if (osuFileLocation.Equals(string.Empty))
                return null;

            var playMode = (int?)_memoryReader.ReadPlayedGameMode() ?? -1;
            if (playMode < 0)
                return null;

            var OppaiCalc = new OppaiCalc(beatmap);
            double ppNow = OppaiCalc.CalculatePP(currentMapTime);

            return await Task.FromResult(ppNow);
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

        //private int ConvertPlayMode_BMAPIToSC(GameMode playMode)
        //{
        //    switch (playMode)
        //    {
        //        default:
        //            throw new ArgumentException("Invalid playMode provided.");
        //        case BMAPI.v1.GameMode.osu:
        //            return 0;
        //        case BMAPI.v1.GameMode.Taiko:
        //            return 1;
        //        case BMAPI.v1.GameMode.CatchtheBeat:
        //            return 2;
        //        case BMAPI.v1.GameMode.Mania:
        //            return 3;
        //    }
        //}
    }

    internal class OppaiCalc
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Beatmap beatmap;
        private TimeSpan endDelayTime = TimeSpan.FromMilliseconds(5000);

        public OppaiCalc(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        public double CalculatePP(int time = -1)
        {
            int currentObjectNumber = GetCurrentBeatmapObjectNumber(beatmap, time);

            // TODO: Optimize - don't re-launch the process every time
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = $"{Helpers.CurrentExeDirectory()}\\oppai.exe";
            // TODO: Include other playdata e.g. gameMode, mods, nx100, nx50, misses
            p.StartInfo.Arguments = $"\"{beatmap.Filename}\" -ojson -end{currentObjectNumber}";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();

            string jsonOutput = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            return GetPPFromOppaiOutput(jsonOutput);
        }

        private int GetCurrentBeatmapObjectNumber(Beatmap beatmap, int currentMapTime)
        {
            List<CircleObject> hitObjects = beatmap.HitObjects;

            if (hitObjects is null || hitObjects.Count < 1)
            {
                //_logger.Info("a");
                return 1;
            }

            if (currentMapTime < 0)
            {
                //_logger.Info($"b: {currentMapTime}");
                return 1;
            }

            if (currentMapTime + endDelayTime.Milliseconds >= hitObjects.Last().StartTime)
            {
                //_logger.Info($"c: {currentMapTime}, {hitObjects.Last().StartTime}, {hitObjects.Count}");
                return hitObjects.Count;
            }

            hitObjects.Reverse();

            for (int i = 1; i <= hitObjects.Count; i++)
            {
                var ho = hitObjects[i - 1];

                if (currentMapTime >= ho.StartTime)
                {
                    //_logger.Info($"d: {currentMapTime}, {ho.StartTime}, {i}, {hitObjects.Count}, {hitObjects.Count - i}");
                    return hitObjects.Count - i;
                }
            }

            return hitObjects.Count;
        }

        private double GetPPFromOppaiOutput(string jsonOutput)
        {
            // {"oppai_version":"3.2.0","code":200,"errstr":"no error","artist":"Mitsuki Kotono","artist_unicode":"þ¥Äµ£êþÉ┤Úƒ│","title":"Fuyu ni Saku Hana","title_unicode":"Õå¼Òü½ÕÆ▓ÒüÅÞÅ»","creator":"Nardoxyribonucleic","version":"Kantan","mods_str":"","mods":0,"od":3,"ar":3,"cs":5,"hp":5,"combo":380,"max_combo":380,"num_circles":380,"num_sliders":0,"num_spinners":1,"misses":0,"score_version":1,"stars":1.4633538722991943,"speed_stars":1.4633538722991943,"aim_stars":0,"aim_pp":0,"speed_pp":9.6786031723022461,"acc_pp":60.697185516357422,"pp":74.775711059570313}

            PPObject o = JsonConvert.DeserializeObject<PPObject>(jsonOutput);

            return o.pp;
        }

    }

    internal class PPObject
    {
        public double pp;
        public double acc_pp;
        public double speed_pp;
        public double aim_pp;
        public double speed_stars;
        public double aim_stars;
    }

}
