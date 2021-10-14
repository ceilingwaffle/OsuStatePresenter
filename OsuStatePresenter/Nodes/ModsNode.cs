namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the osu! mods of the current beatmap.
    /// </summary>
    [StateProperty(enabled: true, name: "Mods", strictValue: true)]
    public class ModsNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns a value representing the osu! mods wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            this.Preceders.TryGetValue(typeof(StatusNode), out Node statusNode);

            if (statusNode is null)
            {
                // always read mods if we don't have a Status node defined (not recommended; slow)
                return await Task.FromResult(this.ReadMods());
            }

            OsuStatus statusNow = (OsuStatus)statusNode.GetValue();
            var x = statusNode.GetPreviousValue();
            OsuStatus statusBefore = x == null ? OsuStatus.Unknown : (OsuStatus)x;

            // _logger.Info($"{statusBefore} -> {statusNow}");

            // only read mods if status is "playing" and the previous status was not "playing".
            if (statusNow == OsuStatus.Playing && !statusNow.Equals(statusBefore))
            {
                return await Task.FromResult(this.ReadMods());
            }
            else
            {
                // use existing mods value stored on this Node
                object mods = this.GetValue();

                if (mods is null)
                {
                    return await Task.FromResult(Mods.None.ToString());
                }

                // just keep whatever the current value is (last good value)
                return await Task.FromResult(mods);
            }

            // TODO: BUG - Win32Exception not being caught from OsuMemory DLL when using osu Cutting Edge version
        }

        private string ReadMods()
        {
            Logger.Debug("Calculating mods...");
            int modsBitwise = this.MemoryReader.GetMods();
            string mods = ((Mods)modsBitwise).ToString();
            return mods;
        }
    }
}
