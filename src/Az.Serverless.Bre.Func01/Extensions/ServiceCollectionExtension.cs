using Az.Serverless.Bre.Func01.Configuration;
using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Repositories.Implementations;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Az.Serverless.Bre.Func01.RuleEngineCustomizations;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBlobRulesStore(this IServiceCollection services)
        {
            services.AddOptions<BlobRulesStoreConfiguration>()
                .Configure<IConfiguration>((settings, config) =>
                {
                    config.GetSection(nameof(BlobRulesStoreConfiguration))
                    .Bind(settings);

                });

            var serviceProvider = services.BuildServiceProvider();

            var blobStoreConfig = (serviceProvider.GetService<IOptions<BlobRulesStoreConfiguration>>())
                .Value;

            services.AddSingleton<BlobContainerClient>(x => 
                new BlobContainerClient(
                    blobContainerName: blobStoreConfig.ContainerName,
                    connectionString: blobStoreConfig.ConnectionString)
            );

            services.AddScoped<IRulesStoreRepository, BlobRulesStoreRepository>();

            return services;
            
        }

        public static IServiceCollection AddRulesEngine(this IServiceCollection services)
        {
            var rulesEngineSettings = new ReSettings
            {
                CustomActions = new Dictionary<string, Func<RulesEngine.Actions.ActionBase>>
                {
                    { nameof(ExecutionResultCustomAction), () => new ExecutionResultCustomAction()}
                }
            };

            services.AddScoped<IRulesEngine>(x => new RulesEngine.RulesEngine(reSettings: rulesEngineSettings));
            services.AddScoped<IRulesEngineHandler, RulesEngineHandler>();

            return services;
        }
    }
}
