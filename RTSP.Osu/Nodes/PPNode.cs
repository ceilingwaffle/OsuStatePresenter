using BMAPI.v1;
using OsuMemoryDataProvider;
using RTSP.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "PPNow")]
    class PPNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(StatusNode), out var statusNode);
            Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);
            Preceders.TryGetValue(typeof(ModsNode), out var modsNode);

            var beatmap = (BMAPI.v1.Beatmap)beatmapNode?.GetValue() ?? null;
            var mods = (string)modsNode?.GetValue() ?? string.Empty;
            var osuFileLocation = ((BMAPI.v1.Beatmap) beatmapNode?.GetValue())?.Filename ?? string.Empty;
            var playMode = (int?)_memoryReader.ReadPlayedGameMode() ?? -1;

            //SetCurrentMap(beatmap, mods, osuFileLocation, playMode);
            //_memoryReader.GetPlayData(Play);
            //var ppNow = PPIfBeatmapWouldEndNow();

            // TODO: See PpIfRestFcd() in https://github.com/Piotrekol/StreamCompanion/blob/master/plugins/OsuMemoryEventSource/RawMemoryDataProcessor.cs

            double ppNow = 0.0;

            //_logger.Info($"PP Now: {ppNow}");

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
