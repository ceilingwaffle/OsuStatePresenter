namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the status of the game (e.g. MainMenu, Playing, ResultsScreen, etc.)
    /// </summary>
    [StateProperty(enabled: true, name: "GameStatus")]
    public class StatusNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the game status wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            //this.Preceders.TryGetValue(typeof(BeatmapNode), out Node node);
            this.Preceders.TryGetValue(typeof(PausedNode), out Node mapPausedNode);
            this.Preceders.TryGetValue(typeof(MapBreakNode), out Node mapBreakNode);
            this.Preceders.TryGetValue(typeof(MapStartNode), out Node mapStartNode);

            OsuStatus status = this.GetOsuStatus((PausedNode)mapPausedNode, (MapBreakNode)mapBreakNode, (MapStartNode)mapStartNode);

            string statusString = System.Enum.GetName(typeof(OsuStatus), status);

            return await Task.FromResult(statusString);
        }

        /// <summary>
        ///// Osu Memory Reader Status types: https://i.imgur.com/Q49H5Lk.png
        ///// </summary>
        ///// <returns>A string representing the game status</returns>
        //private string GetMemoryStatus()
        //{
        //    this.MemoryReader.GetCurrentStatus(out int statusNumber);

        //    var status = (OsuStatus)statusNumber;

        //    return status.ToString();
        //}

        private OsuStatus GetOsuStatus(PausedNode pausedNode, MapBreakNode mapBreakNode, MapStartNode mapStartNode)
        {
            // var osuStatusFromMemory = this.GetMemoryStatus();

            var omStatus = this.MemoryReader.GetCurrentStatus(out int statusNumber);

            var isPaused = (bool)pausedNode.GetValue();
            var isMapBreak = (bool)mapBreakNode.GetValue();
            var isMapStart = (bool)mapStartNode.GetValue();

            if (isPaused == false && omStatus.Equals(OsuStatus.Unknown))
            {
                return OsuStatus.Unknown;
            }

            if (isPaused == false && omStatus.Equals("Playing") && isMapBreak == true)
            {
                return OsuStatus.InMapBreak;
            }

            if (omStatus.Equals("Playing") && isPaused == true)
            {
                return OsuStatus.SongPaused;
            }

            if (omStatus.Equals("Playing") && isMapStart == true)
            {
                return OsuStatus.MapStart;
            }

            //if (status.Length < 1)
            //{
            //    return OsuStatus.Unknown;
            //}

            return (OsuStatus)omStatus;

            // try to get the enum property name as a string
            // return !System.Enum.TryParse(status, ignoreCase: true, out OsuStatus osuStatus) ? OsuStatus.Unknown : osuStatus;
        }

    }
}
