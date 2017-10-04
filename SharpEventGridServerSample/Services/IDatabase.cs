using System.Threading.Tasks;

namespace SharpEventGridServerSample {
    public interface IDatabase {
        Task SaveAsync(object item);
    }
}