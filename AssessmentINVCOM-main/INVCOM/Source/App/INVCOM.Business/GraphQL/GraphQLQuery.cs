using Framework.DataAccess.Repository;
using Framework.DataAccess;
using Framework.Entity;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVCOM.Business.GraphQL
{
    public class GraphQLQuery : ObjectGraphType
    {
        public GraphQLQuery(IEnumerable<ITopLevelQuery> topLevelQueries)
        {
            foreach (var query in topLevelQueries)
            {
                query.RegisterField(this);
            }
        }
    }
}
