using System.Threading.Tasks;
using DVPF.Core;
using System;
using System.IO;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: false, name: "Beatmap")]
    public class BeatmapNode : OsuNode
    {
        // TODO: Read path from config
        // TODO: Read osu path from process
        private readonly string _osuSongsFolderPath = @"C:\osu!\Songs\";
        //private readonly string _osuSongsFolderPath = @"C:\Users\waffle\AppData\Local\osu!\Songs\";

        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(MapIdNode), out Node mapIdNode);

            if (mapIdNode is null)
                return null;

            string mapFolderName = _memoryReader.GetMapFolderName();
            string mapFileName = _memoryReader.GetOsuFileName();
            string fullMapFilePath = BuildMapPathString(mapFolderName, mapFileName);

            if (!File.Exists(fullMapFilePath))
            {
                _logger.Warn($"Beatmap file not found: {fullMapFilePath}");
                return null;
            }

            BMAPI.v1.Beatmap beatmap = BuildBeatmapFromFile(fullMapFilePath);

            return await Task.FromResult(beatmap);
        }

        private string BuildMapPathString(string mapFolderName, string mapFileName)
        {
            return string.Concat(_osuSongsFolderPath, mapFolderName, @"\", mapFileName);
        }

        private BMAPI.v1.Beatmap BuildBeatmapFromFile(string fullMapFilePath)
        {
            // TODO: Build decorated custom-beatmap class object (to reduce external class coupling to BMAPI).
            return new BMAPI.v1.Beatmap(fullMapFilePath);
        }
    }
}
