using GraphQL.Types;
using System.Collections.Generic;

namespace INVCOM.Business.GraphQL
{
    public class GraphQLMutation : ObjectGraphType       
    {
        public GraphQLMutation(IEnumerable<ITopLevelMutation> topLevelMutations)
        {
            foreach (var mutation in topLevelMutations)
            {
                mutation.RegisterField(this);
            }
        }
    }
}
