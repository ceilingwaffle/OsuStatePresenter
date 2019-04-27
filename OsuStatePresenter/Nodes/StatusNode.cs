namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the status of the game (e.g. MainMenu, Playing, ResultsScreen, etc.)
    /// </summary>
    [StateProperty(enabled: true, name: "GameStatus")]
    public class StatusNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the game status wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            // TODO: REFACTOR - Create custom object for OsuStatus
            string status = this.GetMemoryStatus();
            return await Task.FromResult(status);
        }

        /// <summary>
        /// Osu Memory Reader Status types: https://i.imgur.com/Q49H5Lk.png
        /// </summary>
        /// <returns>A string representing the game status</returns>
        private string GetMemoryStatus()
        {
            this.MemoryReader.GetCurrentStatus(out int statusNumber);

            var status = (OsuMemoryDataProvider.OsuMemoryStatus)statusNumber;

            return status.ToString();
        }
    }
}
