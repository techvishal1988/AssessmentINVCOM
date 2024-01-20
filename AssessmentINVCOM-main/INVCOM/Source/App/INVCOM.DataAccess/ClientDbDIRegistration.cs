namespace INVCOM.DataAccess
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using Framework.DataAccess;
    using INVCOM.DataAccess.Repository;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Defines the <see cref="ClientDbDIRegistration" />.
    /// </summary>
    public static class ClientDbDIRegistration
    {
        /// <summary>
        /// Configures the client db context services.
        /// </summary>
        /// <param name="services">A <see cref="IServiceCollection"/> to add the client services to.</param>
        /// <param name="connectionString">The connectionString<see cref="string"/>.</param>
        /// <param name="readOnlyConnectionString">The readOnlyConnectionString<see cref="string"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection ConfigureDbServices(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            services.AddRepositories(typeof(TransactionRepository).Assembly);
            return services;
        }
    }
}
