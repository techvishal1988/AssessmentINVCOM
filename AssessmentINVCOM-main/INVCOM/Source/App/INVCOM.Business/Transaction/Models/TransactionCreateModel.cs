using Amazon.DynamoDBv2.DataModel;
using System;

namespace INVCOM.Business.Transaction.Models
{
    /// <summary>
    /// Defines the <see cref="TransactionCreateModel" />.
    /// </summary>
    public class TransactionCreateModel
    {        
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long TransactionAmount { get; set; } 
        public DateTime TransactionDate { get; set; } 
        public string TransactionType { get; set; }         
        public bool IsSynced { get; set; }
        public bool IsActive { get; set; }
    }
}
