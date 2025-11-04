namespace McCandless.DataStore
{
    using System;

    public abstract class EntityDataStore<TEntityBase, TIdentity> : IEntityDataStore<TEntityBase, TIdentity> where TEntityBase : EntityBase<TIdentity>
    {
        private readonly IDataStoreAdapter<TEntityBase, TIdentity> adapter;

        public EntityDataStore(IDataStoreAdapter<TEntityBase, TIdentity> adapter)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public TEntity Create<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy = context.UserAgent;
            entity.CreatedDateTime = DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return adapter.Create(entity, context);
        }

        public TEntity Delete<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return adapter.Delete<TEntity>(identity, context);
        }

        public TEntity Get<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return adapter.Get<TEntity>(identity, context);
        }

        public TEntity Update<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return adapter.Update(entity, context);
        }

        public TEntity Upsert<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy ??= context.UserAgent;
            entity.CreatedDateTime ??= DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return adapter.Create(entity, context);
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
