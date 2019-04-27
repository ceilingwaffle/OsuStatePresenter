namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using BMAPI.v1;

    using DVPF.Core;

    using OsuStatePresenter.Nodes.Dependencies;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the current pp (performance points) updated in realtime as the player plays a map.
    /// </summary>
    [StateProperty(enabled: true, name: "PPNow")]
    // ReSharper disable once InconsistentNaming
    public class PPNode : OsuNode
    {
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

            var oppaiCalc = new OppaiCalc(beatmap);
            double ppNow = oppaiCalc.CalculatePP(currentMapTime);

            return await Task.FromResult(ppNow);
        }
    }
}
