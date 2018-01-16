using System.Diagnostics;
using System.Threading.Tasks;
using SharpEventGrid;
using SharpEventGridServer;

namespace SharpMappingOverrideSample {
    public class DefaultMappingHandler : IEventGridHandler {
        public async Task ProcessEvent(Event eventItem) {
            Debug.WriteLine($"{nameof(DefaultMappingHandler)} {eventItem.EventType}");
        }
    }
}
