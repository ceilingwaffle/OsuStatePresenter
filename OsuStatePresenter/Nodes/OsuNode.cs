namespace OsuStatePresenter.Nodes
{
    using DVPF.Core;

    using OsuMemoryDataProvider;

    /// <inheritdoc />
    /// <summary>
    /// The node to be derived from to create a specific node representing an osu! value.
    /// </summary>
    public abstract class OsuNode : Node
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:OsuStatePresenter.Nodes.OsuNode" /> class.
        /// </summary>
        protected OsuNode()
        {
            // using dll
            // ReSharper disable once CommentTypo
            // OsuMemoryDataProvider.DataProvider.Initalize();
            // _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;

            // using project reference
            // _memoryReader = new OsuMemoryReader(); // multi-instance
            this.MemoryReader = OsuMemoryReader.Instance; // shared instance
        }

        /// <summary>
        /// Gets the osu! memory reader.
        /// </summary>
        public IOsuMemoryReader MemoryReader { get; set; }

        //protected void ReinitialiseMemoryReader()
        //{
        //    this.MemoryReader = new OsuMemoryReader();
        //}
    }
}
