using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebAppInfra.Persistence.Data.Repository;

public class GroupChatRepositry : ICustomRepository<GroupChat>
{
    // Basic version, no including nav props
    public async Task<GroupChat?> GetFullEntityAsync(Context context, Guid id, bool reload = true)
        => await GetInternalAsync(context, id, reload, []);

    // Dynamic includes version
    public async Task<GroupChat?> GetFullEntityAsync(Context context, Guid id, bool reload = true, params Expression<Func<GroupChat, object>>[] includes)
        => await GetInternalAsync(context, id, reload, includes);

    async Task<GroupChat?> GetInternalAsync(
        Context context,
        Guid id,
        bool reload,
        Expression<Func<GroupChat, object>>[] includes)
    {
        var tracked = context.ChangeTracker.Entries<GroupChat>()
            .FirstOrDefault(e => e.Entity.GroupChatId == id);

        if (tracked is not null)
        {
            if (!reload && includes.Length == 0)
            {
                return tracked.Entity;
            }
            // Detach so Includes execute against a clean query // what the hell is going on here
            tracked.State = EntityState.Detached;
        }

        var query = (IQueryable<GroupChat>)context.GroupChats; // requires explicit cast

        if (includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync(g => g.GroupChatId == id);
    }
}
