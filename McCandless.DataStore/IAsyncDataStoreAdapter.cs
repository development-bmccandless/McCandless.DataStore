namespace McCandless.DataStore
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncDataStoreAdapter<TEntityBase, TIdentity> where TEntityBase : EntityBase<TIdentity>
    {
        Task<TEntity> CreateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase;

        Task<TEntity> DeleteAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase;

        Task<TEntity> GetAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase;

        Task<TEntity> UpdateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase;

        Task<TEntity> UpsertAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase;
    }
}
