using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Functions
{
    public class ExecuteRules
    {
        [FunctionName($"{nameof(ExecuteRules)}Async")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(authLevel:AuthorizationLevel.Function, methods:"POST")]HttpRequest request,
            ILogger logger)
        {
            
            var requestHeaders = request.GetTypedHeaders();
            var xWorkflowNameHeader = requestHeaders.Get<string>("x-workflow-name");

            if (string.IsNullOrEmpty(xWorkflowNameHeader))
            {
                return new BadRequestObjectResult("x-workflow-name header is mandatory")
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ContentTypes = new MediaTypeCollection
                    {
                        new MediaTypeHeaderValue("application/json")
                    }

                };
            }

            return null;
        }

    }
}
