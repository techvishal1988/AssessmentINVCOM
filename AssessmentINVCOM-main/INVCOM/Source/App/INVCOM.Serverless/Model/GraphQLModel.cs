using Newtonsoft.Json.Linq;

namespace INVCOM.Serverless.Model
{
    public class GraphQLModel
    {
        public string Query { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
    }
}
