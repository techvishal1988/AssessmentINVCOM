using System;

namespace INVCOM.Business.Transaction.Models
{
    /// <summary>
    /// Defines the <see cref="TransactionReadModel" />.
    /// </summary>
    public class TransactionReadModel
    {
        public Guid ReferenceNumber { get; set; }
        public string CustomerName { get; set; }
        public long TransactionAmount { get; set; }
        public long CustomerId { get; set; }  
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionUpdatedDate { get; set; }
        public string TransactionType { get; set; }        
        public bool IsSynced { get; set; }
        public bool IsActive { get; set; }
    }
}
