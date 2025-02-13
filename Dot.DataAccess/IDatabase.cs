namespace Dot.DataAccess
{
    public interface IDatabase
    {
        Task<T> CreateAsync<T>(T record);

        Task<List<T>> ReadAsync<T>();
        Task<T> ReadAsync<T>(string id);
        Task<List<T>> ReadAsync<T>(string filterName, string filterValue);
        Task<bool> UpdateAsync<T>(string id, T record);
    }
}
