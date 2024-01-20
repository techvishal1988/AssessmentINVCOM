namespace INVCOM.Serverless.Extensions
{
    using Framework.Configuration.Models;
    using Framework.Constant;
    using Framework.Service;
    using INVCOM.Business.Transaction.Models;
    using INVCOM.DataAccess;
    using INVCOM.DataSyncApi;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Defines the <see cref="ServicesConfiguration" />.
    /// </summary>
    public static class ServicesConfiguration
    {

        public const string AuthenticationScheme = "Bearer";

        /// <summary>
        /// The ConfigureClientServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection ConfigureClientServices(this IServiceCollection services)
        {
            return services
                .ConfigureAutoMapper()
                .ConfigureSwagger()
                //.AddManagers(typeof(CountryManager).Assembly)
                .ConfigureDbServices();

        }

        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddAutoMapper(typeof(TransactionMappingProfile).Assembly);
            return services;
        }

        /// <summary>
        /// The ConfigureSwagger.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            var swaggerAssemblies = new[] { typeof(Startup).Assembly, typeof(TransactionCreateModel).Assembly, typeof(TransactionCreateModel).Assembly };
            services.AddSwaggerWithComments(ApiConstants.ApiName, ApiConstants.ApiVersion, swaggerAssemblies);
            services.AddSwaggerWithComments(ApiConstants.JobsApiName, ApiConstants.JobsApiVersion, swaggerAssemblies);
            return services;
        }

        public static IServiceCollection ConfigureAwsCongnitoSecurity(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var applicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();

            services.AddCognitoIdentity();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = applicationOptions.CognitoAuthorityURL;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = applicationOptions.CognitoAuthorityURL,
                    ValidateLifetime=true,
                    LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    
                };
            });
            return services;
        }

        public static IServiceCollection ConfigureDataProvider(this IServiceCollection services)
        {
            services.AddTransient<TransactionTableCreationProvider>();
            return services;
        }
    }
}
