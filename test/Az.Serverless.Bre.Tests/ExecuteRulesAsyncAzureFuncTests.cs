using Az.Serverless.Bre.Func01.Functions;
using Azure.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Net.Http.Headers;
using Moq;
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
            var httpRequest = MockHttpRequest(false, false);

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

            var httpRequest = MockHttpRequest(false, false);

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

            var httpRequest = MockHttpRequest(false, false);

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

            var httpRequest = MockHttpRequest(true, true);

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

            var httpRequest = MockHttpRequest(true, false);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeEquivalentTo(expectedResult);
            
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Execute_Without_Throwing_Error_When_Correct_ContentType_Is_Provided_In_Header()
        {
            //Arrange

            var httpRequest = MockHttpRequest(true, true);

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            executionResult.Should().BeNull();

        }

        private HttpRequest MockHttpRequest(bool provideWorkflowName, bool provideContentType)
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



            _mockHttpRequest.Setup(x => x.Headers)
                .Returns(headerDict);

            return _mockHttpRequest.Object;
        }

    }
}
