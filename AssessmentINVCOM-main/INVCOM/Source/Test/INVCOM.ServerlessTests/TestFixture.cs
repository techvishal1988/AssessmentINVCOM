using Framework.Configuration;
using Framework.Configuration.Models;
using INVCOM.Service;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace INVCOM.ServerlessTests
{
    /// <summary>
    /// Defines the <see cref="TestFixture" />.
    /// </summary>
    public class TestFixture : IDisposable
    {
        public GraphQLController _graphQLController;
        /// <summary>
        /// Controller TestFixture
        /// </summary>
        public TestFixture()
        {
            string[] args = new string[0];
            var host = CreateHostBuilder(args).Build();
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var schema = services.GetRequiredService<ISchema>();
            var documentExecuter = services.GetRequiredService<IDocumentExecuter>();
            var logger = services.GetRequiredService<ILogger<GraphQLController>>();
            _graphQLController = new GraphQLController(schema, documentExecuter, logger);
        }

        /// <summary>
        /// The CreateHostBuilder.
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/>.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns> 
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .DefaultAppConfiguration(new[] { typeof(ApplicationOptions).Assembly }, args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        public void Dispose()
        {
            Dispose();
        }
    }
}
