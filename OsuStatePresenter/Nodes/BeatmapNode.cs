﻿using System.Threading.Tasks;
using DVPF.Core;
using System;
using System.IO;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: false, name: "Beatmap")]
    public class BeatmapNode : OsuNode
    {
        private string _osuSongsFolderPath = BuildOsuSongsFolderPath(Helpers.GetProcessDirectory("osu!"));

        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(MapIdNode), out Node mapIdNode);

            if (mapIdNode is null)
                return null;

            // TODO: Optimize - Find a better way of caching the value of _osuSongsFolderPath (shouldn't have to re-scan for the osu process path multiple times).
            string fullMapFilePath = BuildFullMapFilePath();
            if (!File.Exists(fullMapFilePath))
            {
                // try re-scanning the osu process directory (e.g. if a different instance of osu! was opened from another location).
                _osuSongsFolderPath = BuildOsuSongsFolderPath(Helpers.GetProcessDirectory("osu!"));
                fullMapFilePath = BuildFullMapFilePath();
                if (!File.Exists(fullMapFilePath))
                {
                    _logger.Warn($"Beatmap file not found: {fullMapFilePath}");
                    return null;
                }
            }

            BMAPI.v1.Beatmap beatmap = BuildBeatmapFromFile(fullMapFilePath);

            return await Task.FromResult(beatmap);
        }

        private string BuildFullMapFilePath()
        {
            string mapFolderName = _memoryReader.GetMapFolderName();
            string mapFileName = _memoryReader.GetOsuFileName();
            string fullMapFilePath = BuildMapPathString(mapFolderName, mapFileName);
            return fullMapFilePath;
        }

        private static string BuildOsuSongsFolderPath(string osuProcessDirectory)
        {
            // TODO: Get "Songs" folder name from user config file in osu dir (e.g. osu!.waffle.cfg)
            return Path.Combine(osuProcessDirectory, "Songs") + Path.DirectorySeparatorChar.ToString();
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
