namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the map ID of the current beatmap.
    /// </summary>
    [StateProperty(enabled: true, name: "MapID")]
    public class MapIdNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the current map ID wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            int mapId = this.MemoryReader.GetMapId();

            return await Task.FromResult(mapId);
        }
    }
}
