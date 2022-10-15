using Az.Serverless.Bre.Func01.Factory;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Az.Serverless.Bre.Func01.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Functions
{


    public class ExecuteRules
    {
        private const string contentType = "application/json";

        private readonly IRulesStoreRepository _rulesStoreRepository;
        private readonly IRulesEngineHandler _rulesEngineHandler;

        private readonly EvaluationInputWrapperValidator _evalInputWrapperValidator;

        public ExecuteRules(IRulesStoreRepository rulesStoreRepository,
            IRulesEngineHandler rulesEngineHandler,
            EvaluationInputWrapperValidator evalInputWrapperValidator)
        {
            _rulesStoreRepository = rulesStoreRepository ??
                throw new ArgumentNullException(nameof(rulesStoreRepository));
            _rulesEngineHandler = rulesEngineHandler ??
                throw new ArgumentNullException(nameof(rulesEngineHandler));

            _evalInputWrapperValidator = evalInputWrapperValidator
                ?? throw new ArgumentNullException(nameof(evalInputWrapperValidator));

        }

        [FunctionName($"{nameof(ExecuteRules)}Async")]
        [OpenApiSecurity(schemeName: "functions_key", schemeType: SecuritySchemeType.ApiKey, In = OpenApiSecurityLocationType.Header,
            Name = "x-functions-key")]
        [OpenApiOperation(operationId: "execuetrules", tags: new[] { "executerules" },
            Summary = "Processes the received input and executes the defined rules"
            , Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "x-workflow-name", Description = "Name of the rules config file which is stored in rules store",
            Required = true, Type = typeof(string), In = ParameterLocation.Header, Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EvaluationInputWrapper), Required = true,
           Description = "The data against which the rules defined in the config are executed")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnsupportedMediaType, contentType: "application/json",
            bodyType: typeof(object), Description = "Unsupported media type error response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
            bodyType: typeof(object), Description = "Bad request error response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
            bodyType: typeof(object), Description = "Provided rules config not found in rule store error response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(EvaluationOutput), Description = "Successful response")]
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

            if (string.IsNullOrEmpty(contentTypeHeader) || !contentTypeHeader.Equals(contentType))
            {
                return ObjectResultFactory
                    .Create(
                    statusCode: 415,
                    contentType: contentType,
                    message: $"Content-Type header is mandatory and should be '{contentType}'"
                    );

            }

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync()
                .ConfigureAwait(false);

            var evluationInputWrapper = JsonConvert.DeserializeObject<EvaluationInputWrapper>(requestBody);

            var modelValidationResult = await _evalInputWrapperValidator
                .ValidateAsync(evluationInputWrapper)
                .ConfigureAwait(false);

            if (!modelValidationResult.IsValid)
            {
                return ObjectResultFactory.Create(
                    statusCode: StatusCodes.Status400BadRequest,
                    contentType: contentType,
                    message: modelValidationResult.Errors
                    );
            }

            var rulesConfig = await _rulesStoreRepository.GetConfigAsStringAsync(xWorkflowNameHeader);

            if (rulesConfig == null)
            {
                return ObjectResultFactory.Create(
                    statusCode: StatusCodes.Status404NotFound,
                    contentType: contentType,
                    message: $"Unable to find {xWorkflowNameHeader} file in the rules store"
                    );
            }

            var rulesExecutionResult = await _rulesEngineHandler
                .ExecuteRulesAsync(rulesConfig, evluationInputWrapper.EvaluationInputs.ToArray());

            return ObjectResultFactory.Create(
                statusCode: StatusCodes.Status200OK,
                contentType: contentType,
                message: rulesExecutionResult
                );



        }

    }
}
