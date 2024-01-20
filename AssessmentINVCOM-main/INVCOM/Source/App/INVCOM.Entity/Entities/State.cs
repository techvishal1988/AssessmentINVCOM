namespace INVCOM.Entity.Entities
{
    using Amazon.DynamoDBv2.DataModel;
    using Framework.Entity;
    using Framework.Entity.Entities;
    using System;

    
    [DynamoDBTable("State")]
    public class State : EntityWithId<Guid>
    {
        [DynamoDBProperty("Name")]
        public string? Name { get; set; }

        [DynamoDBProperty("CountryId")]
        public Guid CountryId { get; set; }

    }
}
