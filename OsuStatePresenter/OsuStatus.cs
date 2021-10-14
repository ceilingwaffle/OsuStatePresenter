namespace OsuStatePresenter
{
    /// <summary>
    /// Osu Memory Status (copied from OsuMemoryDataProvider.OsuMemoryStatus)
    /// </summary>
    public enum OsuStatus
    {
        NotRunning = -1,
        MainMenu = 0,
        EditingMap = 1,
        Playing = 2,
        GameShutdownAnimation = 3,
        SongSelectEdit = 4,
        SongSelect = 5,
        WIP_NoIdeaWhatThisIs = 6,
        ResultsScreen = 7,
        GameStartupAnimation = 10,
        MultiplayerRooms = 11,
        MultiplayerRoom = 12,
        MultiplayerSongSelect = 13,
        MultiplayerResultsscreen = 14,
        OsuDirect = 15,
        RankingTagCoop = 17,
        RankingTeam = 18,
        ProcessingBeatmaps = 19,
        Tourney = 22,
        /// <summary>
        /// Indicates that status read in osu memory is not defined in OsuMemoryStatus
        /// </summary>
        Unknown = -2,

        // my own below:
        SongPaused = 101,
        InMapBreak = 102,
        MapStart = 103,
    }

    public static class OsuStatusDescriptions
    {
        public static string ToFriendlyString(this OsuStatus status)
        {
            switch (status)
            {
                case OsuStatus.NotRunning:
                    return "Not Running";
                case OsuStatus.MainMenu:
                    return "Main Menu";
                case OsuStatus.EditingMap:
                    return "Editing Map";
                case OsuStatus.Playing:
                    return "Playing";
                case OsuStatus.GameShutdownAnimation:
                    return "Game Shutdown Animation";
                case OsuStatus.SongSelectEdit:
                    return "Song Select Edit";
                case OsuStatus.SongSelect:
                    return "Song Select";
                case OsuStatus.WIP_NoIdeaWhatThisIs:
                    return "WIP_NoIdeaWhatThisIs";
                case OsuStatus.ResultsScreen:
                    return "Results Screen";
                case OsuStatus.GameStartupAnimation:
                    return "Game Startup Animation";
                case OsuStatus.MultiplayerRooms:
                    return "Multiplayer Rooms";
                case OsuStatus.MultiplayerRoom:
                    return "Multiplayer Room";
                case OsuStatus.MultiplayerSongSelect:
                    return "Multiplayer Song Select";
                case OsuStatus.MultiplayerResultsscreen:
                    return "Multiplayer Results Screen";
                case OsuStatus.OsuDirect:
                    return "OsuDirect";
                case OsuStatus.RankingTagCoop:
                    return "Ranking Tag Coop";
                case OsuStatus.RankingTeam:
                    return "Ranking Team";
                case OsuStatus.ProcessingBeatmaps:
                    return "Processing Beatmaps";
                case OsuStatus.Tourney:
                    return "Tourney";
                case OsuStatus.Unknown:
                    return "Unknown";
                case OsuStatus.SongPaused:
                    return "Song Paused";
                case OsuStatus.InMapBreak:
                    return "In Map Break";
                case OsuStatus.MapStart:
                    return "Map Start";
                default:
                    return "Unknown";
            }
        }
    }
}
