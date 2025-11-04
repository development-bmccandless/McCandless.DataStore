namespace McCandless.DataStore
{
    public interface IEntityDataStore<TEntityBase, TIdentity> where TEntityBase : EntityBase<TIdentity>
    {
        TEntity Create<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase;

        TEntity Delete<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase;

        TEntity Get<TEntity>(TIdentity identity, OperationContext context) where TEntity : TEntityBase;

        TEntity Update<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase;

        TEntity Upsert<TEntity>(TEntity entity, OperationContext context) where TEntity : TEntityBase;
    }
}
