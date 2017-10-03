using SharpEventGrid;
using System.Threading.Tasks;

namespace SharpEventGridServer {
    public interface IEventGridHandler {
        Task ProcessEvent(Event eventItem);
    }
}