using System;
using System.CodeDom;
using System.Threading.Tasks;
using DVPF.Core;
using OsuStatePresenter;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "Mods", strictValue: true)]
    public class ModsNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(StatusNode), out var statusNode);

            if (statusNode is null)
            {
                // always read mods if we don't have a Status node defined (not recommended; slow)
                return await Task.FromResult(ReadMods());
            }

            var statusNow = statusNode.GetValue();
            var statusBefore = statusNode.GetPreviousValue();

            //_logger.Info($"{statusBefore} -> {statusNow}");

            // TODO: Sometimes mods doesn't update on the state. Maybe because a previous mods update task is waiting to finish?

            // only read mods if status is "playing" and the previous status was not "playing".
            if (statusNow.Equals("Playing") && !statusNow.Equals(statusBefore))
            {
                return await Task.FromResult(ReadMods());
            }
            else
            {
                // use existing mods value stored on this Node
                var mods = GetValue();

                if (mods is null)
                {
                    return await Task.FromResult(Mods.None.ToString());
                }

                // just keep whatever the current value is (last good value)
                return await Task.FromResult(mods);
            }

            // TODO: Win32Exception not being caught from OsuMemory DLL when using osu Cutting Edge version

        }

        private string ReadMods()
        {
            _logger.Debug("Calculating mods.........");

            int modsBitwise = _memoryReader.GetMods();
            string mods = ((Mods)modsBitwise).ToString();

            return mods;
        }
    }
}
