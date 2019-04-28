namespace OsuStatePresenter
{
    using System;

    using DVPF.Core;

    using OsuStatePresenter.Nodes;

    /// <summary>
    /// Presents the osu! game state as a <see cref="DVPF.Core.State"/> object.
    /// </summary>
    public class OsuPresenter
    {
        // private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="OsuPresenter"/> class.
        /// </summary>
        /// <param name="stateCreatedHandler">
        /// A method to handle a newly created <see cref="DVPF.Core.State"/>.
        /// </param>
        public OsuPresenter(Action<State> stateCreatedHandler = null)
        {
            this.SetupStatePresenter(stateCreatedHandler);
            SetupDefaultOsuNodes();
        }

        /// <summary>
        /// Gets or sets the osu! state presenter.
        /// </summary>
        public StatePresenter StatePresenter { get; set; }

        /// <summary>
        /// <para>
        /// Starts the scanner to periodically read the osu! game memory and build the <see cref="DVPF.Core.State"/>.
        /// </para>
        /// <para>
        /// See: <seealso cref="DVPF.Core.StatePresenter.StartScannerLoop"/>
        /// </para>
        /// </summary>
        public void Start()
        {
            this.StatePresenter.StartScannerLoop();
        }

        /// <summary>
        /// Stops the scanner.
        /// </summary>
        public void Stop()
        {
            this.StatePresenter.StopScannerLoop();
        }

        /// <summary>
        /// <para>Gets the <see cref="Node"/> associated with the specified node <see cref="Type"/> stored in <see cref="Node.InitializedNodes"/></para>
        /// <para>See: <seealso cref="NodeCollection.TryGetValue"/></para>
        /// </summary>
        /// <param name="nodeType">
        /// The node type.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// true if the node exists in the collection, false if not.
        /// </returns>
        public bool TryGetNode(Type nodeType, out Node node)
        {
            return this.StatePresenter.NodeSupervisor.TryGetInitializedNode(nodeType, out node);
        }

        private static void SetupDefaultOsuNodes()
        {
            // TODO: UNFINISHED - All nodes disabled by default. User sets true to static field named Enabled? System enables parents as needed. 
            // This property is for actually scanning the node (different than the "StatePresentable" prop on StatePropAttribute).
            //
            // Program should initialize nodes by scanning this "enabled" field. Base Node should have abstract method 
            // Node.Present()/Hide() which sets the StatePropAttribute "enabled" to true/false.
            //
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
            var mapStartNode = new MapStartNode();

            // attach to level 0 nodes
            mapIdNode.Precedes(mapTimeNode, modsNode);
            mapTimeNode.Precedes(bpmNode, pausedNode); // , mapStartNode
            statusNode.Precedes(modsNode);

            // attach to level 1 nodes
            modsNode.Precedes(bpmNode);

            // attach to level 2 nodes
            beatmapNode.Precedes(bpmNode, pausedNode, mapStartNode);
            mapBreakNode.Follows(beatmapNode, mapTimeNode);
            ppNowNode.Follows(statusNode, modsNode, beatmapNode, mapTimeNode);
        }

        private void SetupStatePresenter(Action<State> stateCreatedHandler)
        {
            this.StatePresenter = new StatePresenter();

            if (stateCreatedHandler != null)
            {
                this.StatePresenter.AddEventHandler_NewStateCreated(stateCreatedHandler);
            }
        }
    }
}
