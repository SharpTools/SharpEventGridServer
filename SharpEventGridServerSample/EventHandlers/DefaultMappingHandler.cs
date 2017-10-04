using System.Threading.Tasks;
using SharpEventGridServer;
using SharpEventGrid;
using System.Diagnostics;

namespace SharpEventGridServerSample {
    public class DefaultMappingHandler : IEventGridHandler {
        public async Task ProcessEvent(Event eventItem) {
            Debug.WriteLine($"{nameof(DefaultMappingHandler)} {eventItem.EventType}");
        }
    }
}
