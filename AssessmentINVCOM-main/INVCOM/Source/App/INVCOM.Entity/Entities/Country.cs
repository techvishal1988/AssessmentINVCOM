namespace INVCOM.Entity.Entities
{
    using Amazon.DynamoDBv2.DataModel;
    using Framework.Entity.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Defines the <see cref="Country" />.
    /// </summary>
    [DynamoDBTable("Country")]
    public class Country : EntityWithId<Guid>
    {
        public Country()
        {
            Files = new List<string>();
        }
        [DynamoDBProperty("Name")]
        public string Name { get; set; }

        [DynamoDBProperty("IsoCode")]
        public string IsoCode { get; set; }

        [DynamoDBProperty("Continent")]
        public string Continent { get; set; }

        [DynamoDBIgnore]
        public IEnumerable<State> States { get; set; }

        [DynamoDBProperty("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [DynamoDBIgnore]
        public IEnumerable<string> Files { get; set; }

    }

}
