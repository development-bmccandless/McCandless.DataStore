namespace McCandless.DataStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AsyncEntityDataStore<TEntityBase, TIdentity> : IAsyncEntityDataStore<TEntityBase, TIdentity> where TEntityBase : EntityBase<TIdentity>
    {
        private readonly IAsyncEntityDataStore<TEntityBase, TIdentity> inner;

        public AsyncEntityDataStore(IAsyncEntityDataStore<TEntityBase, TIdentity> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public Task<TEntity> CreateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy = context.UserAgent;
            entity.CreatedDateTime = DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.CreateAsync(entity, context, cancellationToken);
        }

        public Task<TEntity> DeleteAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return inner.DeleteAsync<TEntity>(identity, context, cancellationToken);
        }

        public Task<TEntity> GetAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return inner.GetAsync<TEntity>(identity, context, cancellationToken);
        }

        public Task<TEntity> UpdateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.UpdateAsync(entity, context, cancellationToken);
        }

        public Task<TEntity> UpsertAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy ??= context.UserAgent;
            entity.CreatedDateTime ??= DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.CreateAsync(entity, context, cancellationToken);
        }

        private static void ValidateInput<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (context is null) throw new ArgumentNullException(nameof(context));
        }

        private static void ValidateInput(TIdentity identity, OperationContext context)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));
            if (context is null) throw new ArgumentNullException(nameof(context));
        }
    }
}
