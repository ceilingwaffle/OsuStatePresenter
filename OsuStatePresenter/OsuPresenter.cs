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
            var mapTimeNode = new MapTimeNode();

            // level 1 nodes
            var beatmapNode = new BeatmapNode();

            // level 2 nodes
            var mapStartNode = new MapStartNode();
            var mapBreakNode = new MapBreakNode();
            var mapPausedNode = new PausedNode();

            // level 3
            var statusNode = new StatusNode();

            // level 4
            var modsNode = new ModsNode();

            // other
            var ppNowNode = new PPNode();
            var bpmNode = new BpmNode();

            // level 0
            mapIdNode.Precedes(beatmapNode);
            //mapIdNode.Precedes(leaderboardNode);
            mapTimeNode.Precedes(bpmNode, mapBreakNode, ppNowNode, mapPausedNode);

            // level 1
            beatmapNode.Precedes(bpmNode, mapStartNode, ppNowNode, mapBreakNode, mapPausedNode);

            // level 2
            mapPausedNode.Precedes(statusNode);
            mapStartNode.Precedes(statusNode);
            mapBreakNode.Precedes(statusNode);

            // level 3
            statusNode.Precedes(modsNode, ppNowNode);

            // level 4
            modsNode.Precedes(bpmNode);
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
