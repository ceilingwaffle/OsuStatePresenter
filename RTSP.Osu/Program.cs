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
            mapIdNode.Precedes(beatmapNode);
            mapTimeNode.Precedes(bpmNode, pausedNode);
            statusNode.Precedes(modsNode, pausedNode);

            // attach to level 1 nodes
            beatmapNode.Precedes(redLinesNode);
            modsNode.Precedes(bpmNode);

            // attach to level 2 nodes
            redLinesNode.Precedes(bpmNode);

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
}
