using System.Threading.Tasks;
using SharpEventGridServer;
using SharpEventGrid;
using System.Diagnostics;

namespace SharpEventGridServerSample {
    public class SomeEventHandler : IEventGridHandler {
        public async Task ProcessEvent(Event eventItem) {
            Debug.WriteLine($"{nameof(SomeEventHandler)} {eventItem.EventType}");
        }
    }
}
