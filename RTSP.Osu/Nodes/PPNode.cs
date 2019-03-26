using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    class PPNode : OsuNode
    {
        public override Task<object> DetermineValueAsync()
        {
            //var pc = new OsuMemoryDataProvider.PlayContainer();

            //_memoryReader.GetPlayData(pc);

            // TODO: See PpIfRestFcd() in https://github.com/Piotrekol/StreamCompanion/blob/master/plugins/OsuMemoryEventSource/RawMemoryDataProcessor.cs

            return null;
        }
    }
}
