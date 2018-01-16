using System.Diagnostics;
using System.Threading.Tasks;
using SharpEventGrid;
using SharpEventGridServer;

namespace SharpMappingOverrideSample {
    public class mySubject_SomeEventHandler : IEventGridHandler {
        public async Task ProcessEvent(Event eventItem) {
            Debug.WriteLine($"{nameof(mySubject_SomeEventHandler)} {eventItem.EventType}");
        }
    }
}
