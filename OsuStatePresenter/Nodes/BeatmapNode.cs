namespace OsuStatePresenter.Nodes
{
    using System.IO;
    using System.Threading.Tasks;

    using DVPF.Core;

    using OsuParsers.Beatmaps;
    using OsuParsers.Decoders;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the osu! beatmap.
    /// </summary>
    [StateProperty(enabled: false, name: "Beatmap")]
    public class BeatmapNode : OsuNode
    {
        private string osuSongsFolderPath = BuildOsuSongsFolderPath(Helpers.GetProcessDirectory("osu!"));

        private string previousBeatmapFilePath = string.Empty;
        private Beatmap cachedBeatmap = null;

        /// <inheritdoc />
        /// <summary>
        /// Returns the Beatmap wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            //this.Preceders.TryGetValue(typeof(MapIdNode), out Node mapIdNode);
            //if (mapIdNode is null) return null;

            string fullMapFilePath = this.BuildFullMapFilePath();
            Beatmap beatmap = this.cachedBeatmap;

            if (!File.Exists(fullMapFilePath))
            {
                // TODO: OPTIMIZE - Find a better way of caching the value of _osuSongsFolderPath (shouldn't have to re-scan for the osu process path multiple times).
                // try re-scanning the osu process directory (e.g. if a different instance of osu! was opened from another location).
                this.osuSongsFolderPath = BuildOsuSongsFolderPath(Helpers.GetProcessDirectory("osu!"));
                fullMapFilePath = this.BuildFullMapFilePath();
                if (!File.Exists(fullMapFilePath))
                {
                    // TODO: Log only once per fullMapFilePath
                    Logger.Warn($"Beatmap file not found: {fullMapFilePath}");

                    cachedBeatmap = null;

                    return null;
                }
            }

            if (!fullMapFilePath.Equals(this.previousBeatmapFilePath) || beatmap == null)
            {
                beatmap = this.BuildBeatmapFromFile(fullMapFilePath);
                this.UpdateBeatmapCache(beatmap);

                string msg = $"Updated beatmap file cache in {this.GetType()} ({fullMapFilePath})";
                Logger.Info(msg);
            }

            this.previousBeatmapFilePath = fullMapFilePath;

            return await Task.FromResult(beatmap);
        }

        /// <summary>
        /// Gets the map time of the occurrence of the first hit object in the given <paramref name="beatmap"/> (float.MaxValue if not found).
        /// </summary>
        /// <param name="beatmap">
        /// The beatmap.
        /// </param>
        /// <returns>
        /// The map time as a <see cref="float"/>, float.MaxValue if the beatmap contains no hit objects or if the beatmap was null.
        /// </returns>
        public float GetTimeOfFirstHitObject(Beatmap beatmap)
        {
            if (beatmap?.HitObjects == null)
            {
                return float.MaxValue;
            }

            var t = beatmap.HitObjects?[index: 0]?.StartTime;

            return t ?? float.MaxValue;
        }

        private static string BuildOsuSongsFolderPath(string osuProcessDirectory)
        {
            // TODO: UNFINISHED - Get "Songs" folder name from user config file in osu dir (e.g. osu!.waffle.cfg)
            return Path.Combine(osuProcessDirectory, "Songs") + Path.DirectorySeparatorChar.ToString();
        }

        private Beatmap BuildBeatmapFromFile(string fullMapFilePath)
        {
            // TODO: REFACTOR - Build decorated custom-beatmap class object (to reduce external class coupling to BMAPI).
            return BeatmapDecoder.Decode(fullMapFilePath);
        }

        private string BuildFullMapFilePath()
        {
            string mapFolderName = this.MemoryReader.GetMapFolderName();
            string mapFileName = this.MemoryReader.GetOsuFileName();
            string fullMapFilePath = this.BuildMapPathString(mapFolderName, mapFileName);
            return fullMapFilePath;
        }

        private void UpdateBeatmapCache(Beatmap beatmap)
        {
            this.cachedBeatmap = beatmap;
        }

        private string BuildMapPathString(string mapFolderName, string mapFileName)
        {
            return string.Concat(this.osuSongsFolderPath, mapFolderName, @"\", mapFileName);
        }
    }
}
