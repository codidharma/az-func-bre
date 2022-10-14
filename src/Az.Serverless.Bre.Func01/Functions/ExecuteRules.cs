using Az.Serverless.Bre.Func01.Factory;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Az.Serverless.Bre.Func01.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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

            if(string.IsNullOrEmpty(contentTypeHeader) || !contentTypeHeader.Equals(contentType))
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
