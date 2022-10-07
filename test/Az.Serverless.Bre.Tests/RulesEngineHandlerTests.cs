using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using FluentAssertions;
using Newtonsoft.Json;
using RulesEngine.Exceptions;
using RulesEngine.Interfaces;
using BRE = RulesEngine;

namespace Az.Serverless.Bre.Tests
{
    public class RulesEngineHandlerTests
    {
        private readonly IRulesEngineHandler _rulesEngineHandler;
        private readonly IRulesEngine _rulesEngine;

        public RulesEngineHandlerTests()
        {
            _rulesEngine = new BRE.RulesEngine(reSettings: null);

            _rulesEngineHandler = new RulesEngineHandler(_rulesEngine);
        }

        [Fact]
        public void RulesEngineHandler_Constructor_Should_Throw_Argument_Null_Exception_For_Null_Rules_Engine_Dependency_Injection()
        {
            //Act
            Action action = () =>
            {
                new RulesEngineHandler(null);
            };

            action.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'rulesEngine')");
        }

        [Theory]
        [InlineData("This is non json string")]
        [InlineData("\"somealue\":")]
        public void AddOrUpdateWorkflow_Should_Throw_JsonException_When_Config_String_Cannot_Be_Deserialized(string configString)
        {

            //Act
            Action action = () =>
            {
                _rulesEngineHandler.AddOrUpdateWorkflows(configString);
            };

            //Assert
            action.Should()
                .Throw<JsonException>(because: "The provided string is not a validjson");


        }

        [Theory]
        [InlineData("")]
        public void AddOrUpdateWorkflow_Should_Throw_Exception_When_Workflows_Are_Null(string configString)
        {
            //Act
            Action action = () =>
            {
                _rulesEngineHandler.AddOrUpdateWorkflows(configString);
            };

            //Assert
            action.Should().Throw<NullReferenceException>();

        }

        [Theory]
        [InlineData("[{}]")]
        public void AddOrUpdateWorkflow_Should_Throw_Exception_When_Workflows_Fails_Validation(string configString)
        {
            //Act
            Action action = () =>
            {
                _rulesEngineHandler.AddOrUpdateWorkflows(configString);
            };

            //Assert
            action.Should().Throw<RuleValidationException>();

        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Throw_ArgumentNullException_When_Rules_Config_Is_Empty_String()
        {
            //Act

            Func<Task> task = async () =>
            {
                await _rulesEngineHandler.ExecuteRulesAsync(string.Empty);
            };

            await task.Should().ThrowExactlyAsync<ArgumentNullException>()
                .WithMessage("The rules config should not be null or empty string");
        }
    }
}
