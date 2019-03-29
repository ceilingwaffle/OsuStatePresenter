using System.Threading.Tasks;
using DependentValuePresentationFramework;
using System;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "GameStatus")]
    class StatusNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO: Create custom object for OsuStatus

            string status = _GetMemoryStatus();

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
