using INVCOM.Serverless.Model;
using GraphQL;
using GraphQL.Execution;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using GraphQL.NewtonsoftJson;
using NuGet.Protocol;
using Newtonsoft.Json;

namespace INVCOM.Service
{
    [Route("api/graphql")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class GraphQLController : ControllerBase
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly ILogger<GraphQLController> _logger;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter, ILogger<GraphQLController> logger)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HandleRequest([FromBody] GraphQLModel query, CancellationToken cancellationToken)
        {
            if (query.Query == null)
            {
                _logger.LogWarning("Empty GraphQL Query");
                return BadRequest();
            }

            var timedToken = GetTimedToken(cancellationToken);
            var executionOptions = new ExecutionOptions
            {
                Variables = new GraphQL.NewtonsoftJson.GraphQLSerializer().Deserialize<Inputs>(query.Variables.ToJson()),
                Schema = _schema,
                Query = query.Query,
                CancellationToken = timedToken,
                EnableMetrics = false,
                //UserContext = new GraphQLRequestContext(User, Request),
                //ComplexityConfiguration = new ComplexityConfiguration
                //{
                //    MaxDepth = 3
                //}
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);

            if (result.Errors?.Count > 0)
            {
                _logger.LogError("Error in GraphQL Query Excecution {@Data}", JsonConvert.SerializeObject(result.Errors));
                return BadRequest(new { errors = result.Errors.Select(j => j.Message) });
            }

            _logger.LogInformation("GraphQL Query Excection Successful {@Data}", new { query });
            return Ok(((ExecutionNode)result.Data).ToValue());
        }

        private static CancellationToken GetTimedToken(CancellationToken cancellationToken)
        {
            var timedToken = new CancellationTokenSource(5000);
            var joinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timedToken.Token);
            return timedToken.Token;
        }
    }
}
