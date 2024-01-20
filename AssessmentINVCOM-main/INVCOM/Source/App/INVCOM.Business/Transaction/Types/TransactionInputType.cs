using GraphQL.Types;
using System;

namespace INVCOM.Business.Transaction.Types
{
    public class TransactionCreateInputType : InputObjectGraphType
    {
        public TransactionCreateInputType()
        {
            Name = "transactionCreateInput";
            Field<NonNullGraphType<IdGraphType>>("customerId");
            Field<NonNullGraphType<IntGraphType>>("transactionAmount");
            Field<NonNullGraphType<DateGraphType>>("trasctionDate");
            Field<NonNullGraphType<StringGraphType>>("transactionType");
            Field<NonNullGraphType<IdGraphType>>("referenceNumber");
            //Field<NonNullGraphType<StringGraphType>>("trasctionStatus");
            //Field<NonNullGraphType<BooleanGraphType>>("isSync");
            //Field<IdGraphType>("countryId");
        }
    }

    public class TransactionUpdateInputType : InputObjectGraphType
    {
        public TransactionUpdateInputType()
        {
            Name = "ctrasctionUpdateInput";
            Field<IdGraphType>("referenceNumber");
            Field<NonNullGraphType<IdGraphType>>("customerId");
            Field<NonNullGraphType<IntGraphType>>("trasctionAmount");
            Field<NonNullGraphType<DateGraphType>>("trasctionDate");
            Field<NonNullGraphType<StringGraphType>>("trasctionType");
            Field<NonNullGraphType<IdGraphType>>("referenceNumber");            
            Field<NonNullGraphType<BooleanGraphType>>("isSync");
        }
    }
 
}
