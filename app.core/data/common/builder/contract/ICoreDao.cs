using app.core.data.common.core;

namespace app.core.data.common.contract
{
    public interface ICoreDao<TId, TEntity>
        where TEntity : Entity<TId, TEntity>
    {

    }
}
