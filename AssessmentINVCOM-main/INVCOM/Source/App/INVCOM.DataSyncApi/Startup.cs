using Framework.Business.ServiceProvider.Storage;
using Framework.Configuration.Models;
using INVCOM.Business;
using INVCOM.Business.GraphQL;
using INVCOM.Serverless.Extensions;
using Framework.Constant;
using Framework.Service;
using Framework.Service.Extension;
using Framework.Business.ServiceProvider.Queue;
using Amazon.SQS.Model;

namespace INVCOM.DataSyncApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddControllersWithViews().AddNewtonsoftJson();
        services.AddScoped(typeof(IQueueManager<AmazonSQSConfigurationOptions, List<Message>>), typeof(QueueManager));
        services.ConfigureClientServices();         
        services.ConfigureDataProvider();
        
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.AddProblemDetailsSupport();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger(new[]
                {
                      new SwaggerConfigurationModel(ApiConstants.ApiVersion, ApiConstants.ApiName, true),
                      new SwaggerConfigurationModel(ApiConstants.JobsApiVersion, ApiConstants.JobsApiName, false)
                    });

       

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();             
            //endpoints.MapGraphQL<AppSchema>().RequireAuthorization();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}