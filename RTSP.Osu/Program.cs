using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Example
{
    class Program
    {
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
            System.Diagnostics.Debug.WriteLine($"------------------New state created.", state);
        }
    }

    class MapIdNode : Node
    {
        public override string StatePropertyName => "MapID";

        public override async Task<object> DetermineValueAsync()
        {
            //// fetch data
            //Debug.WriteLine($"{T()} Fetching data...", LogCategory.Event, this);
            //await Task.Delay(TimeSpan.FromMilliseconds(800));
            //var fetchedDataTs = Helpers.UnixTimestamp();
            //Debug.WriteLine($"{T()} Completed: FetchData().", LogCategory.Event, this);

            //await Task.Delay(TimeSpan.FromMilliseconds(200));
            //var calculatedValue = Helpers.UnixTimestamp() - fetchedDataTs;
            ////_SetValue(calculatedValue);
            //_SetValue(Helpers.Rand(1, 2));
            //Debug.WriteLine($"{T()} Completed: CalculateValue(fetchedData).", LogCategory.Event, this);

            return await Task.Run(() => { return new object(); });
        }
    }

    class MapTimeNode : Node
    {
        // should not be listed in EnabledNodes
        public override string StatePropertyName => null;

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class StatusNode : Node
    {
        // should be listed in EnabledNodes
        public override string StatePropertyName => "GameStatus";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class BpmNode : Node
    {
        public override string StatePropertyName => "BPM";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class ModsNode : Node
    {
        public override string StatePropertyName => "Mods";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class RedLinesNode : Node
    {
        // should not be listed in EnabledNodes
        //public override string StatePropertyName => "RedLineTimingPoints";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class BeatmapNode : Node
    {
        // should not be listed in EnabledNodes
        //public override string StatePropertyName => "Beatmap";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }

    class PausedNode : Node
    {
        public override string StatePropertyName => "IsPaused";

        public override Task<object> DetermineValueAsync()
        {
            return Task.Run(() => { return new object(); });
        }
    }
}
