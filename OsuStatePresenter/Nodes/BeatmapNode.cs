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
        private bool fileWatcherIsInitialised = false;


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

            string fullMapFilePath = this.GetMapFilePath();
            Beatmap beatmap = this.cachedBeatmap;

            if (!fileWatcherIsInitialised)
            {
                InitialiseBeatmapFileWatcher();
            }

            if (!File.Exists(fullMapFilePath))
            {
                // TODO: OPTIMIZE - Find a better way of caching the value of _osuSongsFolderPath (shouldn't have to re-scan for the osu process path multiple times).
                // try re-scanning the osu process directory (e.g. if a different instance of osu! was opened from another location).
                this.osuSongsFolderPath = BuildOsuSongsFolderPath(Helpers.GetProcessDirectory("osu!"));
                fullMapFilePath = this.GetMapFilePath();
                if (!File.Exists(fullMapFilePath))
                {
                    // TODO: Log only once per fullMapFilePath
                    Logger.Warn($"Beatmap file not found: {fullMapFilePath}");
                    this.cachedBeatmap = null;
                    // return null;
                    return new Beatmap();
                }
            }

            //if (!fullMapFilePath.Equals(this.previousBeatmapFilePath) || beatmap == null)
            //{
            //    beatmap = UpdateBeatmapCacheAndGetBeatmap(fullMapFilePath);
            //}

            beatmap = this.BuildBeatmapFromFile(fullMapFilePath);

            // this.previousBeatmapFilePath = fullMapFilePath;

            return await Task.FromResult(beatmap);
        }

        //private Beatmap UpdateBeatmapCacheAndGetBeatmap(string fullMapFilePath)
        //{
        //    // this.ReinitialiseMemoryReader();
        //    Beatmap beatmap = this.BuildBeatmapFromFile(fullMapFilePath);
        //    this.cachedBeatmap = beatmap;
        //    string msg = $"Updated beatmap file cache in {this.GetType()} ({fullMapFilePath})";
        //    Logger.Info(msg);
        //    return beatmap;
        //}

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

            var t = beatmap.HitObjects?[0]?.StartTime;

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

        private string GetMapFolderPath()
        {
            return string.Concat(this.osuSongsFolderPath, this.GetMapFolderName(), @"\");
        }

        private string GetMapFilePath()
        {
            string mapFolderName = this.GetMapFolderName();
            string mapFileName = this.GetMapFileName();
            string fullMapFilePath = this.BuildMapPathString(mapFolderName, mapFileName);
            return fullMapFilePath;
        }

        private string GetMapFolderName()
        {
            return this.MemoryReader.GetMapFolderName();
        }

        private string GetMapFileName()
        {
            return this.MemoryReader.GetOsuFileName();
        }

        private string BuildMapPathString(string mapFolderName, string mapFileName)
        {
            return string.Concat(this.osuSongsFolderPath, mapFolderName, @"\", mapFileName);
        }

        private void InitialiseBeatmapFileWatcher()
        {
            var path = this.GetMapFolderPath();
            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.osu";

            watcher.Changed += OnOsuFileChanged;
            // watcher.Created += OnOsuFileChanged;
            // watcher.Deleted += OnOsuFileChanged;
            // watcher.Renamed += new FileSystemEventHandler(OnChanged);
            watcher.Error += OnOsuFileWatcherError;

            //watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            fileWatcherIsInitialised = true;
        }

        private void OnOsuFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;

            var filePath = e.FullPath;
            var fileName = e.Name;

            Logger.Debug($"osu file Changed: '{e.FullPath}'");
            Logger.Debug($"Refreshing beatmap file cache...");

            this.cachedBeatmap = null;
        }

        private void OnOsuFileWatcherError(object sender, ErrorEventArgs e)
        {
            Logger.Error(e.GetException());
        }
    }
}
