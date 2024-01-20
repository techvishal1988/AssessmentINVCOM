using GraphQL.Types;

namespace INVCOM.Business.Transaction.Types
{
    public class TransactionSearchInputType  : InputObjectGraphType
    {
        public TransactionSearchInputType()
        {
            Name = "TransactionSearchInput";
            Field<ListGraphType<LongGraphType>>("customerIds");
            Field<DateTimeGraphType>("fromDate");
            Field<DateTimeGraphType>("toDate");
            Field<IdGraphType>("referenceNumber");
        }
    }
}
