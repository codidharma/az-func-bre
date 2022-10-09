using Az.Serverless.Bre.Func01.Functions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Net.Http.Headers;
using Moq;

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
            var httpRequest = MockHttpRequest();

            //Act
            await _executeRules.RunAsync(httpRequest, _logger);
        }

        [Fact]
        public async Task Execute_Rules_Async_Should_Return_Bad_Object_Result_When_No_WorkFlowName_Is_Provided_In_Header()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedResult = new BadRequestObjectResult("x-workflow-name header is mandatory")
            {
                StatusCode = 400,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };

            var httpRequest = MockHttpRequest();

            //Act
            var executionResult = await _executeRules.RunAsync(httpRequest, _logger);

            //Assert
            //executionResult.Should().BeOfType<BadRequestObjectResult>();
            executionResult
                .Should().BeEquivalentTo<BadRequestObjectResult>(expectedResult);
            
        }


        private HttpRequest MockHttpRequest()
        {
            _mockHttpRequest = new Mock<HttpRequest>();


            var headerDict = new HeaderDictionary();
            headerDict.Add("dummyHeader", "dummyValue");


            _mockHttpRequest.Setup(x => x.Headers)
                .Returns(headerDict);
            
            return _mockHttpRequest.Object;
        }

    }
}
