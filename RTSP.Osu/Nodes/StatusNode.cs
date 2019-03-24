using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "GameStatus")]
    class StatusNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            string status = _GetMemoryStatus();

            // TODO: Create custom object for OsuStatus

            return await Task.FromResult(status);
        }

        /// <summary>
        /// Osu Memory Reader Status types: https://i.imgur.com/Q49H5Lk.png
        /// </summary>
        /// <returns></returns>
        private string _GetMemoryStatus()
        {
            _memoryReader.GetCurrentStatus(out int statusNumber);

            var status = (OsuMemoryDataProvider.OsuMemoryStatus)statusNumber;

            return status.ToString();
        }
    }
}
