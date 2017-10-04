using System.Threading.Tasks;
using SharpEventGridServer;
using SharpEventGrid;
using System.Diagnostics;

namespace SharpEventGridServerSample {
    public class NewCustomerEventHandler : IEventGridHandler {

        private IDatabase _database;
        public NewCustomerEventHandler(IDatabase database) {
            _database = database;
        }

        public async Task ProcessEvent(Event eventItem) {
            var newCustomerEvent = eventItem.DeserializeEvent<NewCustomerEvent>();
            await _database.SaveAsync(newCustomerEvent);
            Debug.WriteLine($"{nameof(NewCustomerEventHandler)} {eventItem.EventType}");
        }
    }
}
