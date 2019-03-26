using RTSP.Core;
using RTSP.Osu.Nodes;
using System;

namespace RTSP.Osu
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var sp = new StatePresenter();

            // level 0 nodes (master nodes)
            var mapTimeNode = new MapTimeNode();
            var mapIdNode = new MapIdNode();
            var statusNode = new StatusNode();

            // level 1 nodes
            var beatmapNode = new BeatmapNode();
            var pausedNode = new PausedNode();
            var modsNode = new ModsNode();

            // level 2 nodes
            var mapBreakNode = new MapBreakNode();

            // level 3 nodes
            var bpmNode = new BpmNode();

            // attach to level 0 nodes
            mapIdNode.Precedes(beatmapNode);
            mapTimeNode.Precedes(bpmNode, pausedNode);
            statusNode.Precedes(modsNode);

            // attach to level 1 nodes
            modsNode.Precedes(bpmNode);

            // attach to level 2 nodes
            beatmapNode.Precedes(bpmNode);
            mapBreakNode.Follows(beatmapNode, mapTimeNode);

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
            _logger.Info("State created:\n{0}", state);
        }
    }
}
