using GraphQL.Types;
using INVCOM.Business.Transaction.Models;

namespace INVCOM.Business.Transaction.Types
{
    public class TransactionType : ObjectGraphType<TransactionReadModel>
    {
        public TransactionType()
        {
            Field(x => x.ReferenceNumber, type: typeof(IdGraphType)); 
            Field(x => x.CustomerId, type: typeof(LongGraphType));
            Field(x => x.TransactionAmount, type: typeof(LongGraphType));
            Field(x => x.TransactionDate, type: typeof(DateTimeGraphType));
            Field(x => x.TransactionType, type: typeof(StringGraphType));          
            Field(x => x.IsSynced, type: typeof(BooleanGraphType));
            Field(x => x.CustomerName, type: typeof(StringGraphType));
            Field(x => x.IsActive, type: typeof(BooleanGraphType));
        }     
    }
}
