using INVCOM.Serverless.Model;
using INVCOM.ServerlessTests;
using INVCOM.ServerlessTests.Models;
using INVCOM.ServerlessTests.TestData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;
using ErrorResponse = INVCOM.ServerlessTests.Models.ErrorResponse;

namespace INVCOM.Service.Tests
{/// <summary>
 /// class GraphQLControllerTests
 /// </summary>
    public class GraphQLControllerTests : XunitMemberDataInput, IClassFixture<TestFixture>
    {
        private GraphQLController _graphQLController;
        private readonly TestFixture _fixture;

        /// <summary>
        /// Constuctor GraphQLControllerTests
        /// </summary>
        public GraphQLControllerTests(TestFixture testFixture)
        {
            _fixture = testFixture;
            _graphQLController = _fixture._graphQLController;

        }
        [Theory]
        [MemberData(nameof(GraphQLModelData))]
        public async Task Get_Transactions_Test(GraphQLModel graphQLModel, CancellationToken cancellationToken = default)
        {
            var controllerResult = await _graphQLController.HandleRequest(graphQLModel, cancellationToken);
            Assert.NotNull(controllerResult);
            Assert.Equal(200,((ObjectResult)controllerResult).StatusCode);
            var result = JsonConvert.DeserializeObject<ValidQueryResult>(JsonConvert.SerializeObject((((ObjectResult)controllerResult).Value)));
            Assert.NotNull(result);
            Assert.True(result.Transactions.Any() || result.Transactions.Count() == 0); 
        } 
    }
}