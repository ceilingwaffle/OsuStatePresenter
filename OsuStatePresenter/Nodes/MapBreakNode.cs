using DVPF.Core;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "IsMapBreak")]
    public class MapBreakNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO
            return await Task.FromResult(false);
        }
    }
}