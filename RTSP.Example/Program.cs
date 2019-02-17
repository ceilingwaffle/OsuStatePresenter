using RTSP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            // add master nodes
            sp.NodeSupervisor.AddMasterNodes(mapIdNode, mapTimeNode, statusNode);
            
            // attach to level 0 nodes
            mapIdNode.AddChildren(beatmapNode);
            mapTimeNode.AddChildren(bpmNode, pausedNode);
            statusNode.AddChildren(modsNode, pausedNode);

            // attach to level 1 nodes
            beatmapNode.AddChildren(redLinesNode);
            modsNode.AddChildren(bpmNode);

            // attach to level 2 nodes
            redLinesNode.AddChildren(bpmNode);

            // start scanning
            var scannerTask = sp.StartAsync();

            // keep the console open
            while (true)
            {
                Console.ReadKey();
            }
        }
    }

    class MapIdNode : Node
    {
    }

    class MapTimeNode : Node
    {
    }

    class StatusNode : Node
    {
    }

    class BpmNode : Node
    {
    }

    class ModsNode : Node
    {
    }

    class RedLinesNode : Node
    {
    }

    class BeatmapNode : Node
    {
    }

    class PausedNode : Node
    {
    }
}
