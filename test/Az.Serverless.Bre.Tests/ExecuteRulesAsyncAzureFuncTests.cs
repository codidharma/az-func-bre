using Az.Serverless.Bre.Func01.Factory;
using Az.Serverless.Bre.Func01.Functions;
using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Mapper.Configuration;
using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Repositories.Implementations;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Az.Serverless.Bre.Func01.RuleEngineCustomizations;
using Az.Serverless.Bre.Func01.Validators;
using Az.Serverless.Bre.Tests.Utilities;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Net.Http.Headers;
using Moq;
using Newtonsoft.Json;
using RulesEngine.Models;
using System.Text;
using BRE = RulesEngine;

namespace Az.Serverless.Bre.Tests
{
    public class ExecuteRulesAsyncAzureFuncTests 
    {
        private readonly ExecuteRules _executeRules;

        private readonly ILogger _logger = (new NullLoggerFactory())
            .CreateLogger<ExecuteRulesAsyncAzureFuncTests>();

        private Mock<HttpRequest>? _mockHttpRequest;
        private MemoryStream? _memoryStream;

        public ExecuteRulesAsyncAzureFuncTests()
        {
            IRulesStoreRepository rulesStoreRepository = SetupBlobRepositoryInstance();
            IRulesEngineHandler rulesEngineHandler = SetupRulesEngineHandlerInstance();
            var evalInputWrapperValidator = new EvaluationInputWrapperValidator();

            _executeRules = new ExecuteRules(rulesStoreRepository, rulesEngineHandler, evalInputWrapperValidator);
        }


        [Fact]
        public async void Execute_Rules_Sync_AzFunction_Should_Accept_HTTP_Request_And_ILogger_Instances()
        {
            //Arrange
            var httpRequest = MockHttpRequest(false, false, false);

            //Act
            await _executeRules.RunAsync(httpRequest, _logger);
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Return_Bad_Object_Result_When_No_WorkFlowName_Is_Provided_In_Header()
        {
            //Arrange

            var expectedResult = new BadRequestObjectResult("x-workflow-name header is mandatory and should be non empty string")
            {
                StatusCode = 400,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };

            var httpRequest = MockHttpRequest(false, false, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            //executionResult.Should().BeOfType<BadRequestObjectResult>();
            executionResult
                .Should().BeEquivalentTo<BadRequestObjectResult>(expectedResult);

        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_Bad_Object_Result_When_Empty_WorkflowName_Is_Provided_In_Headers()
        {
            //Arrange
            var expectedResult = new BadRequestObjectResult("x-workflow-name header is mandatory and should be non empty string")
            {
                StatusCode = 400,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };

            var httpRequest = MockHttpRequest(false, false, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should()
                .BeEquivalentTo<BadRequestObjectResult>(expectedResult);
        }


        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_Unsupported_Media_Type_If_Content_Type_Header_Is_Missing()
        {
            //Arrange
            var expectedResult = new ObjectResult("Content-Type header is mandatory and should be 'application/json'")
            {
                StatusCode = StatusCodes.Status415UnsupportedMediaType,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")

                }
            };

            var httpRequest = MockHttpRequest(true, false, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeEquivalentTo(expectedResult);

        }


        [Fact]
        public async Task Execute_Rules_Should_Throw_Bad_Request_Error_When_Model_Validation_Fails()
        {
            //Arrange

            var httpRequest = MockHttpRequest(true, true, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger)
                .ConfigureAwait(false);

            //Assert
            ((ObjectResult)executionResult).StatusCode
                .Should().Be(StatusCodes.Status400BadRequest);

        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Throw_Bad_Object_Result_When_Input_Validation_Fails()
        {
            //Arrange
            int expectedStatusCode = 400;

            var httpRequest = MockHttpRequest(true, true, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger)
                .ConfigureAwait(false);

            //Assert
            ((ObjectResult)executionResult).StatusCode
                .Should().Be(expectedStatusCode);
            ((ObjectResult)executionResult).Value
                .Should().BeOfType<List<ValidationFailure>>();

        }

        [Fact]
        public void ExecuteRules_Constructor_Should_Throw_Argument_Null_Exeception_When_No_Rules_Repository_Is_Injected()
        {
            //Act
            Action action = () =>
            {
                new ExecuteRules(null, null, null);
            };

            action.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'rulesStoreRepository')");

        }

        [Fact]
        public void ExecuteRules_Constructor_Should_Throw_Argument_Null_Exeception_When_No_RulesEngine_Handler_Is_Injected()
        {
            var containerClient = BlobUtils.MockBlobContainerClient(null);

            var rulesStoreRepository = new BlobRulesStoreRepository(containerClient);

            Action action = () =>
            {
                new ExecuteRules(rulesStoreRepository, null, null);
            };

            //Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'rulesEngineHandler')");

        }

        [Fact]
        public void ExecuteRules_Constructor_Should_Throw_Argument_Null_Exeception_When_No_ModelValidator_Is_Injected()
        {
            IRulesStoreRepository rulesStoreRepository = SetupBlobRepositoryInstance();
            IRulesEngineHandler rulesEngineHandler = SetupRulesEngineHandlerInstance();

            Action action = () =>
            {
                new ExecuteRules(rulesStoreRepository, rulesEngineHandler, null);
            };

            action.Should().ThrowExactly<ArgumentNullException>()
               .WithMessage("Value cannot be null. (Parameter 'evalInputWrapperValidator')");
        }



        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_Not_Found_Result_When_Rules_Config_Is_Not_Found_In_Rules_Store()
        {
            //Arrange
            var containerClient = BlobUtils.MockBlobContainerClient(null);

            var rulesStoreRepository = new BlobRulesStoreRepository(containerClient);
            var rulesEngineHandler = SetupRulesEngineHandlerInstance();
            var evalInputWrapperValidator = new EvaluationInputWrapperValidator();

            var executeRules = new ExecuteRules(rulesStoreRepository, rulesEngineHandler, evalInputWrapperValidator);

            var httpRequest = MockHttpRequest(true, true, true);

            var expectedResult = ObjectResultFactory.Create(
                statusCode: StatusCodes.Status404NotFound,
                contentType: "application/json",
                message: $"Unable to find FDInterestRates.json file in the rules store"
                );

            //Act
            var executionResult = await executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Work_Successfully_Without_Throwing_Error()
        {
            //Arrange

            var rulesResult = new List<ExecutionResult> { new ExecutionResult { Result = 10.56 } };

            var evaluationOutput = new EvaluationOutput
            {
                ErrorMessage = null,
                IsEvaluationSuccessful = true,
                ExecutionResults = rulesResult
            };

            var expectedResult = ObjectResultFactory.Create(
                statusCode: StatusCodes.Status200OK,
                contentType: "application/json",
                message: evaluationOutput
                );

            var httpRequest = MockHttpRequest(true, true, true);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger)
                .ConfigureAwait(false);

            //Assert
            executionResult.Should().BeEquivalentTo(expectedResult);
        }

        private HttpRequest MockHttpRequest(bool provideWorkflowName, bool provideContentType, bool provideValidBody)
        {
            _mockHttpRequest = new Mock<HttpRequest>();


            var headerDict = new HeaderDictionary();
            headerDict.Add("dummyHeader", "dummyValue");

            if (provideWorkflowName)
            {
                headerDict.Add("x-workflow-name", "FDInterestRates.json");
            }

            if (provideContentType)
            {
                _mockHttpRequest.Setup(x => x.ContentType)
                    .Returns("application/json");

            }

            string body = string.Empty;

            if (provideValidBody)
            {
                StringBuilder builder = new StringBuilder("{");
                builder.Append("\"age\":65");
                builder.Append(",");
                builder.Append("\"durationInMonths\":12");
                builder.Append("}");

                body = builder.ToString();
            }

            var evaluationWrapper = new EvaluationInputWrapper
            {
                EvaluationInputs = new List<EvaluationInput>
                {
                    new EvaluationInput
                    {
                        Name = "input",
                        StringifiedJsonMessage = body
                    }
                }
            };

            var jsonMessage = JsonConvert.SerializeObject(evaluationWrapper);
            var bytesArray = Encoding.ASCII.GetBytes(jsonMessage);

            _memoryStream = new MemoryStream(bytesArray);
            _memoryStream.Flush();
            _memoryStream.Position = 0;

            _mockHttpRequest.Setup(x => x.Body)
                .Returns(_memoryStream);


            _mockHttpRequest.Setup(x => x.Headers)
                .Returns(headerDict);

            return _mockHttpRequest.Object;
        }


        #region PrivateMethods
        private static BlobRulesStoreRepository SetupBlobRepositoryInstance()
        {
            string rulesConfigFileName = "FDInterestRates.json";
            string ruleConfigPath = Path.GetFullPath($"..\\..\\..\\TestData\\RuleConfigs\\{rulesConfigFileName}");

            var containerClient = BlobUtils.MockBlobContainerClient(ruleConfigPath);

            var rulesStoreRepository = new BlobRulesStoreRepository(containerClient);
            return rulesStoreRepository;
        }

        private static RulesEngineHandler SetupRulesEngineHandlerInstance()
        {
            var rulesEngineSetting = new ReSettings
            {
                CustomActions = new Dictionary<string, Func<BRE.Actions.ActionBase>>
                {
                    { "ExecutionResultCustomAction", () => new ExecutionResultCustomAction()}
                }
            };

            var rulesEngine = new BRE.RulesEngine(reSettings: rulesEngineSetting);

            var mapper = AutoMapperConfiguration.Initialize();

            return new RulesEngineHandler(rulesEngine, mapper);

        }
        #endregion

    }
}
