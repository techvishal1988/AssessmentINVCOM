using INVCOM.DataAccess;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic; 

namespace INVCOM.Business.GraphQL
{
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider)
                : base(provider)
        {
            Query = provider.GetRequiredService<GraphQLQuery>();
            //Mutation = provider.GetRequiredService<GraphQLMutation>();
        }        
    }
}
