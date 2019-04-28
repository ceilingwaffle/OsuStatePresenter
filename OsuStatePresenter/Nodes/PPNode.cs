namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using BMAPI.v1;
    using OppaiWNet.Wrap;
    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the current pp (performance points) updated in realtime as the player plays a map.
    /// </summary>
    [StateProperty(enabled: true, name: "PPNow")]
    // ReSharper disable once InconsistentNaming
    public class PPNode : OsuNode
    {
        private readonly OsuMemoryDataProvider.PlayContainer playContainer = new OsuMemoryDataProvider.PlayContainer();
        private Beatmap beatmapCache;
        private Ezpp ezppCache;

        /// <inheritdoc />
        /// <summary>
        /// Returns the current pp wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            // old PpCalculator code:
            // See PpIfRestFcd() in https://github.com/Piotrekol/StreamCompanion/blob/master/plugins/OsuMemoryEventSource/RawMemoryDataProcessor.cs
            // SetCurrentMap(beatmap, mods, osuFileLocation, playMode);
            // _memoryReader.GetPlayData(Play);
            // double ppNow = PPIfBeatmapWouldEndNow();
            this.Preceders.TryGetValue(typeof(StatusNode), out Node statusNode);
            var status = (string)statusNode?.GetValue();
            if (status != null && !status.Equals("Playing"))
            {
                return null;
            }

            this.Preceders.TryGetValue(typeof(BeatmapNode), out Node beatmapNode);
            var beatmap = (Beatmap)beatmapNode?.GetValue();
            if (beatmap is null)
            {
                this.beatmapCache = null;
                return null;
            }

            this.Preceders.TryGetValue(typeof(ModsNode), out Node modsNode);
            string mods = (string)modsNode?.GetValue() ?? string.Empty;
            if (mods.Equals(string.Empty))
            {
                return null;
            }

            this.Preceders.TryGetValue(typeof(MapTimeNode), out Node mapTimeNode);
            int currentMapTime = (int?)mapTimeNode?.GetValue() ?? -1;
            if (currentMapTime < 0)
            {
                return null;
            }

            string osuFileLocation = ((Beatmap)beatmapNode.GetValue())?.Filename ?? string.Empty;
            if (osuFileLocation.Equals(string.Empty))
            {
                return null;
            }

            var playMode = (int?)this.MemoryReader.ReadPlayedGameMode();
            if (playMode < 0)
            {
                return null;
            }

            // TODO: REFACTOR - create inferface named IPPCalc and extract the oppai stuff to a new class
            // TODO: OPTIMIZE - get beatmap from memory somehow, and pass the beatmap byte array into the Ezpp constructor.
            // TODO: BUG - pp always 0 for osu mania
            Ezpp ezpp = ezppCache;
            if (ezpp == null || !beatmap.Equals(beatmapCache))
            {
                this.beatmapCache = beatmap;
                Logger.Info($"Updated beatmap file cache in {this.GetType()} ({beatmap.Filename})");

                ezpp = new Ezpp(beatmap.Filename);
                this.ezppCache = ezpp;
                Logger.Info($"Updated ezpp cache in {this.GetType()}");
            }

            //var oppaiCalc = new OppaiExeCalc(beatmap);
            //double ppNow = oppaiCalc.CalculatePP(currentMapTime);

            MemoryReader.GetPlayData(this.playContainer);

            //var ezpp = new OppaiWNet.Wrap.Ezpp(beatmap.Filename);
            ezpp.Count100 = this.playContainer.C100;
            ezpp.Count50 = this.playContainer.C50;
            ezpp.CountMiss = this.playContainer.CMiss;
            ezpp.Mode = MemoryReader.ReadPlayedGameMode();
            ezpp.Mods = (OppaiWNet.Wrap.Mods)MemoryReader.GetMods();
            ezpp.SetEndTime(currentMapTime);
            ezpp.ApplyChange();

            var pp = double.Parse(ezpp.PP.ToString());

            //Logger.Debug($"Acc: {ezpp.Acc}");
            //Logger.Debug($"PP: {pp}");

            return await Task.FromResult(pp);
        }
    }
}
