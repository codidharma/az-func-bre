﻿using Az.Serverless.Bre.Func01.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Functions
{
    

    public class ExecuteRules
    {
        private const string contentType = "application/json";

        [FunctionName($"{nameof(ExecuteRules)}Async")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(authLevel: AuthorizationLevel.Function, methods: "POST")] HttpRequest request,
            ILogger logger)
        {

            var requestHeaders = request.Headers;

            
            var xWorkflowNameHeader = requestHeaders["x-workflow-name"];

            if (string.IsNullOrEmpty(xWorkflowNameHeader))
            {
                return ObjectResultFactory
                    .Create(
                    statusCode: StatusCodes.Status400BadRequest,
                    contentType: contentType,
                    message: "x-workflow-name header is mandatory and should be non empty string"
                    );

            }

            var contentTypeHeader = request.ContentType;
            

            if (!request.HasFormContentType)
            {
                return ObjectResultFactory
                    .Create(
                    statusCode: 415,
                    contentType: contentType,
                    message: "Content-Type header is mandatory and should be 'multipart/form-data'"
                    );

            }

            return null;
        }

    }
}
