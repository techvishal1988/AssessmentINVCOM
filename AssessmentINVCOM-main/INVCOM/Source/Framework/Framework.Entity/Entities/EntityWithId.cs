using Amazon.DynamoDBv2.DataModel;

namespace Framework.Entity.Entities
{
    public abstract class EntityWithId<TId> : BaseEntity, IEntityWithId<TId>
    {
        [DynamoDBHashKey("Id")]
        public TId Id { get; set; }
    }
}
