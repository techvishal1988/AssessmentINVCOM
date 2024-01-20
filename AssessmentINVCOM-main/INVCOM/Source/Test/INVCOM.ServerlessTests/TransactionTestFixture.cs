using AutoMapper;
using Framework.Configuration;
using Framework.Configuration.Models;
using INVCOM.Business.Transaction.Validator;
using INVCOM.DataAccess.Repository;
using INVCOM.RESTService.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace INVCOM.ServerlessTests
{
    /// <summary>
    /// Defines the <see cref="TransactionTestFixture" />.
    /// </summary>
    public class TransactionTestFixture : IDisposable
    {
        public TransactionController _transactionController { get; set; }

        /// <summary>
        /// Controller TransactionTestFixture
        /// </summary>
        public TransactionTestFixture()
        {
            string[] args = new string[0];
            var host = CreateHostBuilder(args).Build();
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var transactionRepository = services.GetRequiredService<ITransactionRepository>();
            var transactionCreateModelValidator = services.GetRequiredService<TransactionCreateModelValidator>();
            var transactionUpdateModelValidator = services.GetRequiredService<TransactionUpdateModelValidator>();
            var mapper = services.GetRequiredService<IMapper>();
            _transactionController = new TransactionController(transactionRepository, transactionCreateModelValidator, transactionUpdateModelValidator, mapper);

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
