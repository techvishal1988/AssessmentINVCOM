using GraphQL.Types;

namespace INVCOM.Business.GraphQL
{
    public interface ITopLevelMutation
    {
        void RegisterField(ObjectGraphType graphType);
    }
}