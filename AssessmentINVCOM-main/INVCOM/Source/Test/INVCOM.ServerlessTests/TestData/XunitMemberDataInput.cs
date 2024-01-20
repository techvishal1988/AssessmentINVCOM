using INVCOM.Serverless.Model;

namespace INVCOM.ServerlessTests.TestData
{
    /// <summary>
    /// XunitMemberDataInput
    /// </summary>
    public class XunitMemberDataInput
    {
        public static IEnumerable<object[]> GraphQLModelData() =>
        new List<GraphQLModel[]>
          {
                    new GraphQLModel[]
                    {
                        new GraphQLModel{ Query="query transactions { transactions { referenceNumber customerId transactionAmount transactionDate transactionType isSynced customerName isActive} }",
                                          Variables=null
                        }
                    }

          };

    }
}
