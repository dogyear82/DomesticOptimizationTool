using Dot.DataAccess;
using Dot.DataAccess.Entities;

namespace Dot.Services
{
    public interface IAppSettings<T>
    {
        Task<T> GetAsync();
    }

    public class AppSettings<T> : IAppSettings<T>
    {
        private T _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private DateTime _lastCacheUpdate = DateTime.MinValue;
        private readonly IDatabase _db;

        public AppSettings(IDatabase db)
        {
            _db = db;
        }

        public async Task<T> GetAsync()
        {
            if (_cache is not null && (DateTime.UtcNow - _lastCacheUpdate) < _cacheDuration)
            {
                return _cache;
            }

            var settings = (await _db.ReadAsync<Setting<T>>("Type", typeof(T).Name.ToLower())).FirstOrDefault();
            if (settings is null)
            {
                throw new NullReferenceException($"Settings for system '{typeof(T).Name.ToLower()}' not found.");
            }

            _cache = settings.Value;
            _lastCacheUpdate = DateTime.UtcNow;
            return _cache;
        }
    }
}
