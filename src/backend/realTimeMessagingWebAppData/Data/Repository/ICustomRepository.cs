using System.Linq.Expressions;

namespace realTimeMessagingWebAppData.Repository
{
    public interface ICustomRepository<T>
    {
        // I will not be implementing a full repository pattern here, just custom weird bits I want

        public Task<T?> GetFullEntityAsync(Context context, Guid id, bool reload = true);
        public Task<T?> GetFullEntityAsync(Context context, Guid id, bool reload = true, params Expression<Func<T, object>>[] includes);
    }
}
