using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.OpenAPIConfigurations
{
    public class BREFunc01OpenAPIConfigOptions : IOpenApiConfigurationOptions
    {
        public bool ForceHttp { get; set; }
        public bool ForceHttps { get; set; }

        OpenApiInfo IOpenApiConfigurationOptions.Info 
        {
            get
            {
                return new OpenApiInfo
                {
                    Version = "1.0.0",
                    Contact = new OpenApiContact
                    {
                        Email = "mandartest@testmail.com",
                        Name = "Mandar Dharmadhikari",
                        Url = new Uri("https://codidharma.com")

                    },
                    Title = "Serverless Business Rules Engine with Azure Functions and Json Rules Engine",
                    Description = "This api is built using the azure functions and json rules engine and demonstartes how a business rules engine can be" +
            "built using serverless components",
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT")
                    }

                };
            }
            set { }
        }
        List<OpenApiServer> IOpenApiConfigurationOptions.Servers
        {
            get 
            {
                return new List<OpenApiServer>();
            }
            set { }
        }

        OpenApiVersionType IOpenApiConfigurationOptions.OpenApiVersion
        {
            get
            {
                return OpenApiVersionType.V3;
            }
            set { }
        }

        bool IOpenApiConfigurationOptions.IncludeRequestingHostName
        {
            get { return false; }
            set { }
        }
        //bool IOpenApiConfigurationOptions.ForceHttp 
        //{
        //    get { return true; }
        //    set { }
        //}
        //bool IOpenApiConfigurationOptions.ForceHttps {
        //    get { return true; }
        //    set { }
        //}
    }
}
