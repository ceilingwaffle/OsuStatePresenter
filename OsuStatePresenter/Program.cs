﻿using DependentValuePresentationFramework;
using OsuStatePresenter.Nodes;
using System;

namespace OsuStatePresenter
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // TODO: All nodes disabled by default. User sets true to static field named Enabled? System enables parents as needed. This property is for actually scanning the node (different than the "StatePresentable" prop on StatePropAttribute)
            // Program should initialize nodes by scanning this "enabled" field. Base Node should have abstract method Node.Present()/Hide() which sets the StatePropAttribute "enabled" to true/false.
            // Base node should also have method Node.Rename(string) which renames the StatePropAttribute "name" prop.

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
            var bpmNode = new BpmNode();
            var ppNowNode = new PPNode();

            // attach to level 0 nodes
            mapIdNode.Precedes(beatmapNode);
            mapTimeNode.Precedes(bpmNode, pausedNode);
            statusNode.Precedes(modsNode);

            // attach to level 1 nodes
            modsNode.Precedes(bpmNode);

            // attach to level 2 nodes
            beatmapNode.Precedes(bpmNode);
            mapBreakNode.Follows(beatmapNode, mapTimeNode);
            ppNowNode.Follows(statusNode, modsNode, beatmapNode, mapTimeNode);

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
