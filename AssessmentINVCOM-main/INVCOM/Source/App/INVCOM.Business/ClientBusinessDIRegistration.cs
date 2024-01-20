namespace INVCOM.Business
{
    using global::GraphQL;
    using global::GraphQL.Types;
    using INVCOM.Business.GraphQL;
    using INVCOM.Business.Transaction.Manager;
    using INVCOM.Business.Transaction.Types;
    using INVCOM.Business.Transaction.Validator;
    //using Framework.Business;
    //using INVCOM.Business.Country;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Defines the <see cref="ClientBusinessDIRegistration" />.
    /// </summary>
    public static class ClientBusinessDIRegistration
    {
        /// <summary>
        /// The ConfigureBusinessServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection ConfigureGraphQLServices(this IServiceCollection services)
        {

            services.ConfigureGraphQLTypes()
                    .ConfigureGraphQLSchema()
                    .ConfigureGraphQLQuery()
                    .ConfigureGraphQLValidator();
                    

            services.AddGraphQL(b => b
            .AddGraphTypes(typeof(AppSchema).Assembly) // schema            
                .AddSystemTextJson()).AddGraphQLUpload();

            return services;
        }

        private static IServiceCollection ConfigureGraphQLTypes(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<TransactionType>()
                .AddClasses(classes => classes.Where(type => type.IsClass))
                .AsSelf().WithScopedLifetime());

            return services;
        }

        private static IServiceCollection ConfigureGraphQLSchema(this IServiceCollection services)
        {
            services.AddScoped<ISchema, AppSchema>();
            services.AddScoped<AppSchema>();
            return services;
        }

        private static IServiceCollection ConfigureGraphQLQuery(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<TransactionQuery>() // Adjust assembly if needed
                .AddClasses(classes => classes.AssignableTo<ITopLevelQuery>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection ConfigureGraphQLValidator(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<TransactionCreateModelValidator>()
                .AddClasses(classes => classes.Where(type => type.IsClass))
                .AsSelf().WithScopedLifetime());
            return services;
        }
    }
}
