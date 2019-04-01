﻿using DVPF.Core;
using System;
using OsuStatePresenter.Nodes;

namespace OsuStatePresenter
{
    public class Presenter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public StatePresenter StatePresenter { get; set; }

        public Presenter(Action<State> stateCreatedHandler)
        {
            _SetupStatePresenter(stateCreatedHandler);
            _SetupDefaultOsuNodes();
        }

        private void _SetupStatePresenter(Action<State> stateCreatedHandler)
        {
            StatePresenter = new StatePresenter();
            StatePresenter.AddEventHandler_NewStateCreated(stateCreatedHandler);
        }

        private void _SetupDefaultOsuNodes()
        {
            // TODO: All nodes disabled by default. User sets true to static field named Enabled? System enables parents as needed. This property is for actually scanning the node (different than the "StatePresentable" prop on StatePropAttribute)
            // Program should initialize nodes by scanning this "enabled" field. Base Node should have abstract method Node.Present()/Hide() which sets the StatePropAttribute "enabled" to true/false.
            // Base node should also have method Node.Rename(string) which renames the StatePropAttribute "name" prop.

            // level 0 nodes (master nodes)
            var mapIdNode = new MapIdNode();
            var statusNode = new StatusNode();

            // level 1 nodes
            var mapTimeNode = new MapTimeNode();
            var beatmapNode = new BeatmapNode();
            var pausedNode = new PausedNode();
            var modsNode = new ModsNode();

            // level 2 nodes
            var mapBreakNode = new MapBreakNode();
            var bpmNode = new BpmNode();
            var ppNowNode = new PPNode();

            // attach to level 0 nodes
            mapIdNode.Precedes(beatmapNode, mapTimeNode, modsNode);
            mapTimeNode.Precedes(bpmNode, pausedNode);
            statusNode.Precedes(modsNode);

            // attach to level 1 nodes
            modsNode.Precedes(bpmNode);

            // attach to level 2 nodes
            beatmapNode.Precedes(bpmNode);
            mapBreakNode.Follows(beatmapNode, mapTimeNode);
            ppNowNode.Follows(statusNode, modsNode, beatmapNode, mapTimeNode);
        }

        public void Start()
        {
            // start scanning
            var scannerTask = StatePresenter.StartAsync();

        }

    }
}