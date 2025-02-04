namespace Dot.DataAccess
{
    public interface IDatabase
    {
        Task<bool> CreateAsync<T>(T record);

        Task<List<T>> ReadAsync<T>();
        Task<T> ReadAsync<T>(string id);
        Task<bool> UpdateAsync<T>(string id, T record);
    }
}
