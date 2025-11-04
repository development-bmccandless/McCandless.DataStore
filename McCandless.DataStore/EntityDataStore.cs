namespace McCandless.DataStore
{
    using System;

    public abstract class EntityDataStore<TEntityBase, TIdentity> : IEntityDataStore<TEntityBase, TIdentity> where TEntityBase : EntityBase<TIdentity>
    {
        private readonly IEntityDataStore<TEntityBase, TIdentity> inner;

        public EntityDataStore(IEntityDataStore<TEntityBase, TIdentity> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public TEntity Create<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy = context.UserAgent;
            entity.CreatedDateTime = DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.Create(entity, context);
        }

        public TEntity Delete<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return inner.Delete<TEntity>(identity, context);
        }

        public TEntity Get<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(identity, context);

            return inner.Get<TEntity>(identity, context);
        }

        public TEntity Update<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.Update(entity, context);
        }

        public TEntity Upsert<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase
        {
            ValidateInput(entity, context);

            entity.CreatedBy ??= context.UserAgent;
            entity.CreatedDateTime ??= DateTime.UtcNow;
            entity.UpdatedBy = context.UserAgent;
            entity.UpdatedDateTime = DateTime.UtcNow;

            return inner.Create(entity, context);
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
