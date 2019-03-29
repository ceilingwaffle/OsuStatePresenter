using DependentValuePresentationFramework;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "MapID")]
    class MapIdNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            int mapId = _memoryReader.GetMapId();

            return await Task.FromResult(mapId);
        }
    }
}
