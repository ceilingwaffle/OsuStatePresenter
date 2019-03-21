using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Example
{
    class Program
    {
        /*
         * TODO: Replace Debug.WriteLine(...) with NLog
         */

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var sp = new StatePresenter();

            // level 0 nodes (master nodes)
            var mapIdNode = new MapIdNode();
            var mapTimeNode = new MapTimeNode();
            var statusNode = new StatusNode();

            // level 1 nodes
            var beatmapNode = new BeatmapNode();
            var pausedNode = new PausedNode();
            var modsNode = new ModsNode();

            // level 2 nodes
            var redLinesNode = new RedLinesNode();

            // level 3 nodes
            var bpmNode = new BpmNode();

            // attach to level 0 nodes
            mapIdNode.AddChildren(beatmapNode);
            mapTimeNode.AddChildren(bpmNode, pausedNode);
            statusNode.AddChildren(modsNode, pausedNode);

            // attach to level 1 nodes
            beatmapNode.AddChildren(redLinesNode);
            modsNode.AddChildren(bpmNode);

            // attach to level 2 nodes
            redLinesNode.AddChildren(bpmNode);

            sp.AddEventHandler_NewStateCreated(StateCreatedHandler);

            // start scanning
            var scannerTask = sp.StartAsync();

            // keep the console open
            while (true)
            {
                Console.ReadKey();

                //var a = sp;
            }
        }

        protected static void StateCreatedHandler(State state)
        {
            // TODO
            _logger.Debug("State created:\n{0}", state);
        }
    }

    // should be listed in EnabledNodes
    [StateProperty(enabled: true, name: "MapID")]
    class MapIdNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            //// fetch data
            //Debug.WriteLine($"{T()} Fetching data...", LogCategory.Event, this);
            //await Task.Delay(TimeSpan.FromMilliseconds(800));
            //var fetchedDataTs = Helpers.UnixTimestamp();
            //Debug.WriteLine($"{T()} Completed: FetchData().", LogCategory.Event, this);

            //await Task.Delay(TimeSpan.FromMilliseconds(800));
            //var calculatedValue = Helpers.UnixTimestamp() - fetchedDataTs;
            ////_SetValue(calculatedValue);
            //_SetValue(Helpers.Rand(1, 2));
            //Debug.WriteLine($"{T()} Completed: CalculateValue(fetchedData).", LogCategory.Event, this);

            return await Task.FromResult(1234);
        }
    }

    // should not be listed in EnabledNodes
    [StateProperty(enabled: true, name: "MapTime")]
    class MapTimeNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            return await Task.FromResult(180);
        }
    }

    // should be listed in EnabledNodes
    [StateProperty(enabled: true, name: "GameStatus")]
    class StatusNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(600));

            return await Task.FromResult("Playing");
        }
    }

    // should be listed in EnabledNodes
    [StateProperty(enabled: true, name: "CurrentBPM")]
    class BpmNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(700));

            Parents.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            Parents.TryGetValue(typeof(ModsNode), out var modsNode);
            Parents.TryGetValue(typeof(RedLinesNode), out var redLinesNode);

            var mapTime = mapTimeNode.GetValue();
            var mods = modsNode.GetValue();
            var redLineTimingPoints = redLinesNode.GetValue();

            return await Task.FromResult(200);
        }
    }

    // should be listed in EnabledNodes
    [StateProperty(enabled: false, name: "Mods")]
    class ModsNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(800));

            return await Task.FromResult("HDHR");
        }
    }

    // should not be listed in EnabledNodes
    [StateProperty(enabled: false, name: "RedLinesTimingPoints")]
    class RedLinesNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(900));

            return await Task.FromResult("TODO: RedLinesTimingPoints object");
        }
    }

    // should not be listed in EnabledNodes
    [StateProperty(enabled: false, name: "Beatmap")]
    class BeatmapNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            return await Task.FromResult("TODO: Beatmap object");
        }
    }

    // should be listed in EnabledNodes
    [StateProperty(enabled: true, name: "IsPaused")]
    class PausedNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1100));

            return await Task.FromResult(false);
        }
    }
}
