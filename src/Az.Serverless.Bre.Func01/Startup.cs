
using AutoMapper;
using Az.Serverless.Bre.Func01.Extensions;
using Az.Serverless.Bre.Func01.Mapper.Configuration;
using Az.Serverless.Bre.Func01.OpenAPIConfigurations;
using Az.Serverless.Bre.Func01.Validators;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Az.Serverless.Bre.Func01.Startup))]
namespace Az.Serverless.Bre.Func01
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IMapper mapper = AutoMapperConfiguration.Initialize();
            EvaluationInputWrapperValidator validator = new EvaluationInputWrapperValidator();

            builder.Services
                .AddSingleton<IOpenApiConfigurationOptions>(x => new BREFunc01OpenAPIConfigOptions())
                .AddSingleton(mapper)
                .AddSingleton(validator)
                .AddBlobRulesStore()
                .AddRulesEngine();



        }
    }
}
