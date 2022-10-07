using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Models;
using FluentAssertions;
using Newtonsoft.Json;
using RulesEngine.Exceptions;
using RulesEngine.Interfaces;
using System.Dynamic;
using System.Text;
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
        public void RulesEngineHandler_Constructor_Should_Throw_ArgumentNullException_For_Null_Rules_Engine_Dependency_Injection()
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
        public void AddOrUpdateWorkflow_Should_Throw_NullReferenceException_When_Workflows_Are_Null(string configString)
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
        public void AddOrUpdateWorkflow_Should_Throw_RuleValidationException_When_Workflows_Fails_Validation(string configString)
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
            //Arrange
            dynamic data = new ExpandoObject();
            data.age = 65;

            var evaluationInputs = new EvaluationInputParameter[]
                {
                    new EvaluationInputParameter(name: "input", value: data)
                };


            //Act

            Func<Task> task = async () =>
            {
                await _rulesEngineHandler.ExecuteRulesAsync(string.Empty, evaluationInputs);
            };

            await task.Should().ThrowExactlyAsync<ArgumentNullException>()
                .WithMessage("The rules config should not be null or empty string");
        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Throw_ArgumentNullException_When_EvaluationInputParam_Is_Null()
        {
            //Arrange
            var rulesConfig = "[{}]";

            //Act
            Func<Task> task = async () =>
            {
                await _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, null);
            };

            await task.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("EvaluationInputParamerters can not be null or empty");
        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Add_Or_Update_Workflow_Without_Throwing_Exception()
        {
            //Arrange
            var rulesConfigPath = "..\\..\\..\\TestData\\RuleConfigs\\FDInterestRates.json";
            var rulesConfig = GetRulesConfig(rulesConfigPath);

            dynamic data = new ExpandoObject();
            data.age = 65;
            data.durationInMonths = 12;

            var evaluationInputs = new EvaluationInputParameter[] {
                new EvaluationInputParameter(name: "input", value: data)
            };

            //Act
            await _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, evaluationInputs);


        }

        private string GetRulesConfig(string filePath)
        {
           var bytes = File.ReadAllBytes(filePath);
           
           return Encoding.UTF8.GetString(bytes);


        }
    }
}
