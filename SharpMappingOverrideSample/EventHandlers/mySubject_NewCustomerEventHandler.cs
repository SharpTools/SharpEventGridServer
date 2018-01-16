using System.Diagnostics;
using System.Threading.Tasks;
using SharpEventGrid;
using SharpEventGridServer;

namespace SharpMappingOverrideSample {
    public class mySubject_NewCustomerEventHandler : IEventGridHandler {

        private IDatabase _database;
        public mySubject_NewCustomerEventHandler(IDatabase database) {
            _database = database;
        }

        public async Task ProcessEvent(Event eventItem) {
            var newCustomerEvent = eventItem.DeserializeEvent<NewCustomerEvent>();
            await _database.SaveAsync(newCustomerEvent);
            Debug.WriteLine($"{nameof(mySubject_NewCustomerEventHandler)} {eventItem.EventType}");
        }
    }
}
