using System.Threading.Tasks;

namespace SharpMappingOverrideSample {
    public interface IDatabase {
        Task SaveAsync(object item);
    }
}