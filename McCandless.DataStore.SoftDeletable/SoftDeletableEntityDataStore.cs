namespace McCandless.DataStore.SoftDeletable
{
    using System;

    using McCandless.DataStore.Exceptions;

    public abstract class SoftDeletableEntityDataStore<TEntityBase, TIdentity> : IEntityDataStore<TEntityBase, TIdentity> where TEntityBase : SoftDeletableEntityBase<TIdentity>
    {
        private readonly IEntityDataStore<TEntityBase, TIdentity> inner;

        public SoftDeletableEntityDataStore(IEntityDataStore<TEntityBase, TIdentity> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public TEntity Create<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(Create));

            bool exists = false;
            try
            {
                TEntity _ = GetCore<TEntity>(entity.GetIdentity(), context, out exists);

                // IIF an exception was not thrown, the entity exists in a non-deleted state
                throw DataStoreExceptions.Conflict;
            }
            catch (DataStoreException ex) when (ex.ErrorCode == DataStoreErrorCode.NotFound.ErrorCode) { } // no-op

            // clear out any soft-deletion metadata
            entity.IsDeleted = false;
            entity.DeletedBy = null;
            entity.DeletedDateTime = null;

            return exists
                ? inner.Update(entity, context)
                : inner.Create(entity, context);
        }

        public TEntity Delete<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            TEntity existing = GetCore<TEntity>(identity, context);

            existing.IsDeleted = true;
            existing.DeletedBy = context.UserAgent;
            existing.DeletedDateTime = DateTime.UtcNow;

            return inner.Update(existing, context);
        }

        public TEntity Get<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return GetCore<TEntity>(identity, context);
        }

        public TEntity Update<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(Update));

            TEntity _ = GetCore<TEntity>(entity.GetIdentity(), context);

            return inner.Update(entity, context);
        }

        public TEntity Upsert<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context, nameof(Upsert));

            // clear out any soft-deletion metadata
            entity.IsDeleted = false;
            entity.DeletedBy = null;
            entity.DeletedDateTime = null;

            return inner.Upsert(entity, context);
        }

        private TEntity GetCore<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase => GetCore<TEntity>(identity, context, out bool _);

        private TEntity GetCore<TEntity>(TIdentity identity, OperationContext context, out bool isSoftDeleted) where TEntity : TEntityBase
        {
            isSoftDeleted = false;
            TEntity? entity = inner.Get<TEntity>(identity, context);

            if (entity is null)
            {
                throw DataStoreExceptions.NotFound;
            }
            else if (entity.IsDeleted == true)
            {
                isSoftDeleted = true;

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
