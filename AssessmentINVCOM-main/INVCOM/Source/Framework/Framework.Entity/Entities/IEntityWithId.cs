namespace Framework.Entity.Entities
{
    public interface IEntityWithId<TId> : IBaseEntity
    {
        public TId Id { get; set; }
    }
}
