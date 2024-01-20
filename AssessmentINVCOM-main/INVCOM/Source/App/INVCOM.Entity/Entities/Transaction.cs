using Amazon.DynamoDBv2.DataModel;
using System;

namespace INVCOM.Entity.Entities
{
    [DynamoDBTable("InvTransaction")]

    public class Transaction
    {
        [DynamoDBHashKey("ReferenceNumber")]
        public Guid ReferenceNumber { get; set; }

        [DynamoDBProperty("CustomerId")]
        public long CustomerId { get; set; }

        [DynamoDBProperty("CustomerName")]
        public string CustomerName { get; set; }    

        [DynamoDBProperty("TransactionAmount")]
        public long TransactionAmount { get; set; }

        [DynamoDBProperty("TransactionDate")]
        public DateTime TransactionDate { get; set; }

        [DynamoDBProperty("TransactionType")]
        public string TransactionType { get; set; }    

        [DynamoDBProperty("IsSynced")]
        public bool IsSynced { get; set; }

        [DynamoDBProperty("TransactionUpdatedDate")]
        public DateTime TransactionUpdatedDate { get; set; }

        [DynamoDBProperty("IsActive")]
        public bool IsActive { get; set; }

    }
}
