using Az.Serverless.Bre.Func01.Factory;
using Az.Serverless.Bre.Func01.Functions;
using Az.Serverless.Bre.Func01.Models;
using Azure.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Az.Serverless.Bre.Tests
{
    public class ExecuteRulesAsyncAzureFuncTests
    {
        private readonly ExecuteRules _executeRules;

        private readonly ILogger _logger = (new NullLoggerFactory())
            .CreateLogger<ExecuteRulesAsyncAzureFuncTests>();

        private Mock<HttpRequest> _mockHttpRequest;

        public ExecuteRulesAsyncAzureFuncTests()
        {
            _executeRules = new ExecuteRules();
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
        public async Task Execute_Rules_Async_Should_Work_Without_Throwing_Error_When_WorkflowName_Is_Provided_In_Header()
        {
            //Arrange

            var httpRequest = MockHttpRequest(true, true, true);

            //Act
            var expectedResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            expectedResult.Should().BeNull();
            
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_Unsupported_Media_Type_If_Content_Type_Header_Is_Missing()
        {
            //Arrange
            var expectedResult = new ObjectResult("Content-Type header is mandatory and should be 'multipart/form-data'")
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
        public async Task Execute_Rules_Async_Should_Execute_Without_Throwing_Error_When_Correct_ContentType_Is_Provided_In_Header()
        {
            //Arrange

            var httpRequest = MockHttpRequest(true, true, true);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeNull();

        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_Bad_Request_When_Served_With_No_Form_Data_Body()
        {
            //Arrange
            var expectedResult = ObjectResultFactory
                .Create(
                statusCode: 400,
                contentType: "application/json",
                message: "Form Data is required"
                );

            var httpRequest = MockHttpRequest(true, true, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger)
                .ConfigureAwait(false);

            //Assert
            executionResult.Should().BeEquivalentTo(executionResult);
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Throw_bad_Object_Result_When_Form_Data_Cannot_Parse_to_Eval_input()
        {
            //Arrange
            var httpRequest = MockHttpRequest(true, true, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeOfType<ObjectResult>();
            ((ObjectResult)executionResult)
                .StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            ((ObjectResult)executionResult)
                .Value.Should().BeOfType<List<List<ValidationResult>>>();

        }

        private HttpRequest MockHttpRequest(bool provideWorkflowName, bool provideContentType, bool provideValidFormData)
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
                    .Returns("multipart/form-data");
                _mockHttpRequest.Setup(x => x.HasFormContentType)
                    .Returns(true);
            }

            string formKeyName = provideValidFormData ? "input" : string.Empty;

            var formCollection = new FormCollection(
                    new Dictionary<string, StringValues>
                    { {formKeyName, ""}}
                    );

            _mockHttpRequest.Setup(x => x.ReadFormAsync(default))
                .ReturnsAsync(formCollection);

            _mockHttpRequest.Setup(x => x.Headers)
                .Returns(headerDict);

            return _mockHttpRequest.Object;
        }

    }
}
