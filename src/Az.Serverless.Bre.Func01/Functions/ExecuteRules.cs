using Az.Serverless.Bre.Func01.Factory;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Functions
{


    public class ExecuteRules
    {
        private const string contentType = "application/json";

        private readonly IRulesStoreRepository _rulesStoreRepository;
        private readonly IRulesEngineHandler _rulesEngineHandler;

        public ExecuteRules(IRulesStoreRepository rulesStoreRepository,
            IRulesEngineHandler rulesEngineHandler)
        {
            _rulesStoreRepository = rulesStoreRepository ??
                throw new ArgumentNullException(nameof(rulesStoreRepository));
            _rulesEngineHandler = rulesEngineHandler ??
                throw new ArgumentNullException(nameof(rulesEngineHandler));

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


            if (!request.HasFormContentType)
            {
                return ObjectResultFactory
                    .Create(
                    statusCode: 415,
                    contentType: contentType,
                    message: "Content-Type header is mandatory and should be 'multipart/form-data'"
                    );

            }

            var formData = await request.ReadFormAsync()
                .ConfigureAwait(false);

            if (formData == null || formData.Count == 0)
            {
                return ObjectResultFactory
                    .Create(
                    statusCode: 400,
                    contentType: contentType,
                    message: "Form Data is required"
                    );
            }

            var aggregatedValidationErrors = new List<List<ValidationResult>>();
            var evaluationInputs = new List<EvaluationInputParameter>();

            foreach (var formPart in formData)
            {
                var evaluationInput = new EvaluationInputParameter(formPart.Key, formPart.Value.ToString());

                bool isValid = evaluationInput.Validate(out List<ValidationResult> evaluationErrors);

                if (!isValid)
                    aggregatedValidationErrors.Add(evaluationErrors);

                else
                    evaluationInputs.Add(evaluationInput);
            }

            if (aggregatedValidationErrors.Count > 0)
            {
                return ObjectResultFactory.Create(
                    statusCode: StatusCodes.Status400BadRequest,
                    contentType: contentType,
                    message: aggregatedValidationErrors
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
                .ExecuteRulesAsync(rulesConfig, evaluationInputs.ToArray());

            return ObjectResultFactory.Create(
                statusCode: StatusCodes.Status200OK,
                contentType: contentType,
                message: rulesExecutionResult
                );

        }

    }
}
