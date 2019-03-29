using DependentValuePresentationFramework;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "IsMapBreak")]
    class MapBreakNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO
            return await Task.FromResult(false);
        }
    }
}