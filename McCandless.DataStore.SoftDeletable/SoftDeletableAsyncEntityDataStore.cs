namespace McCandless.DataStore.SoftDeletable
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using McCandless.DataStore.Exceptions;

    public abstract class SoftDeletableAsyncEntityDataStore<TEntityBase, TIdentity> : IAsyncEntityDataStore<TEntityBase, TIdentity> where TEntityBase : SoftDeletableEntityBase<TIdentity>
    {
        private readonly IAsyncEntityDataStore<TEntityBase, TIdentity> inner;

        public SoftDeletableAsyncEntityDataStore(IAsyncEntityDataStore<TEntityBase, TIdentity> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<TEntity> CreateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(CreateAsync));

            bool exists = false;
            try
            {
                TEntity _ = await GetCoreAsync<TEntity>(entity.GetIdentity(), context, () => exists = true, cancellationToken);

                // IIF an exception was not thrown, the entity exists in a non-deleted state
                throw DataStoreExceptions.Conflict;
            }
            catch (DataStoreException ex) when (ex.ErrorCode == DataStoreErrorCode.NotFound.ErrorCode) { } // no-op

            // clear out any soft-deletion metadata
            entity.IsDeleted = false;
            entity.DeletedBy = null;
            entity.DeletedDateTime = null;

            return exists
                ? await inner.UpdateAsync(entity, context, cancellationToken)
                : await inner.CreateAsync(entity, context, cancellationToken);
        }

        public async Task<TEntity> DeleteAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            TEntity existing = await GetCoreAsync<TEntity>(identity, context, cancellationToken);

            existing.IsDeleted = true;
            existing.DeletedBy = context.UserAgent;
            existing.DeletedDateTime = DateTime.UtcNow;

            return await inner.UpdateAsync(existing, context, cancellationToken);
        }

        public Task<TEntity> GetAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return GetCoreAsync<TEntity>(identity, context, cancellationToken);
        }

        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(UpdateAsync));

            TEntity _ = await GetCoreAsync<TEntity>(entity.GetIdentity(), context, cancellationToken);

            return await inner.UpdateAsync(entity, context, cancellationToken);
        }

        public Task<TEntity> UpsertAsync<TEntity>(TEntity entity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(UpsertAsync));

            // clear out any soft-deletion metadata
            entity.IsDeleted = false;
            entity.DeletedBy = null;
            entity.DeletedDateTime = null;

            return inner.UpsertAsync(entity, context, cancellationToken);
        }

        private Task<TEntity> GetCoreAsync<TEntity>(TIdentity identity, OperationContext context, CancellationToken cancellationToken) where TEntity : TEntityBase => GetCoreAsync<TEntity>(identity, context, null, cancellationToken);

        private async Task<TEntity> GetCoreAsync<TEntity>(TIdentity identity, OperationContext context, Action? softDeletedCallback, CancellationToken cancellationToken) where TEntity : TEntityBase
        {
            TEntity? entity = await inner.GetAsync<TEntity>(identity, context, cancellationToken);

            if (entity is null)
            {
                throw DataStoreExceptions.NotFound;
            }
            else if (entity.IsDeleted == true)
            {
                if (softDeletedCallback != null)
                {
                    softDeletedCallback();
                }

                throw DataStoreExceptions.NotFound;
            }

            return entity;
        }

        private static void ValidateInput<TEntity>(TEntity entity, OperationContext context, string operation) where TEntity : TEntityBase
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (entity.IsDeleted == true) throw new ArgumentException($"{nameof(SoftDeletableEntityBase<TIdentity>.IsDeleted)} cannot be set to true in {operation}", nameof(entity));
        }

        private static void ValidateInput(TIdentity identity, OperationContext context)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));
            if (context is null) throw new ArgumentNullException(nameof(context));
        }
    }
}
