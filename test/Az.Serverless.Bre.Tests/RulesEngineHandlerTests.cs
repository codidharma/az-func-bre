using AutoMapper;
using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Mapper.Configuration;
using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.RuleEngineCustomizations;
using FluentAssertions;
using Newtonsoft.Json;
using RulesEngine.Exceptions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System.Dynamic;
using System.Text;
using BRE = RulesEngine;

namespace Az.Serverless.Bre.Tests
{
    public class RulesEngineHandlerTests
    {
        private readonly IRulesEngineHandler _rulesEngineHandler;
        private readonly IRulesEngine _rulesEngine;
        private readonly IMapper _mapper = AutoMapperConfiguration.Initialize();

        public RulesEngineHandlerTests()
        {
            var rulesEngineSetting = new ReSettings
            {
                CustomActions = new Dictionary<string, Func<BRE.Actions.ActionBase>>
                {
                    { "ExecutionResultCustomAction", () => new ExecutionResultCustomAction()}
                }
            };

            _rulesEngine = new BRE.RulesEngine(reSettings: rulesEngineSetting);


            _rulesEngineHandler = new RulesEngineHandler(_rulesEngine, _mapper);
        }

        [Fact]
        public void RulesEngineHandler_Constructor_Should_Throw_ArgumentNullException_For_Null_Rules_Engine_Dependency_Injection()
        {
            //Act
            Action action = () =>
            {
                new RulesEngineHandler(null, _mapper);
            };

            action.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'rulesEngine')");
        }

        [Fact]
        public void RulesEngineHandler_Constructor_Should_Throw_ArgumentNullException_For_Null_IMapper_Dependency_Injection()
        {
            //Act

            Action action = () =>
            {
                new RulesEngineHandler(_rulesEngine, null);
            };

            //Assert

            action.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'mapper')");
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
            string req = "{\"age\":65}";

            var evaluationInputs = new EvaluationInput[]
                {
                    new EvaluationInput(key: "input", value: req)
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
            string rulesConfig;
            EvaluationInput[] evaluationInputs;

            SetupTestData(out rulesConfig, out evaluationInputs);

            //Act
            await _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, evaluationInputs);


        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Map_EvaluationInput_To_RuleParameter_Without_Throwing_Exception()
        {
            //Arrange
            string rulesConfig;
            EvaluationInput[] evaluationInputs;

            SetupTestData(out rulesConfig, out evaluationInputs);

            //Act

            await _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, evaluationInputs);

        }

        [Fact]
        public void GetWorkflowName_Should_Throw_NullReferenceException_When_Empty_Rules_Config_Passed()
        {
            //Arrange

            //Act
            Action action = () =>
            {
                _rulesEngineHandler.GetWorkflowName(string.Empty);
            };

            //Assert

            action.Should().ThrowExactly<NullReferenceException>();




        }

        [Fact]
        public void GetWorkflowName_Should_Determine_Name_Of_Workflow_In_Rules_Config_String()
        {
            //Arrange

            string rulesConfig = GetRulesConfig();
            string expecectedWorkflowName = "FDInterestRates";


            //Act
            string workflowName = _rulesEngineHandler.GetWorkflowName(rulesConfig);

            //Assert

            workflowName.Should().BeEquivalentTo(expecectedWorkflowName);

        }

        [Fact]
        public async Task ExecuteRuleAsync_Should_Evaluate_Rules_Without_Throwing_Exception()
        {
            //Arrange
            string rulesConfig;
            EvaluationInput[] evaluationInputs;

            SetupTestData(out rulesConfig, out evaluationInputs);

            //Act

            EvaluationOutput evaluationOutput =
                await _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, evaluationInputs)
                .ConfigureAwait(false);

            //Assert
            evaluationOutput.Should().NotBeNull();
            evaluationOutput.ErrorMessage
                .Should().BeNull();
            evaluationOutput.IsEvaluationSuccessful
                .Should().BeTrue();

        }

        [Fact]
        public async Task ExecuteRulesAsync_Should_Return_AnArray_With_One_Result_For_One_Rule_Execution()
        {
            //Arrange
            string rulesConfig;
            EvaluationInput[] evaluationInputs;
            SetupTestData(out rulesConfig, out evaluationInputs);

            //Act
            var evaluationOutput = await
                _rulesEngineHandler.ExecuteRulesAsync(rulesConfig, evaluationInputs)
                .ConfigureAwait(false);

            //Assert
            evaluationOutput.ExecutionResults.Count().Should().Be(1);
            evaluationOutput.ExecutionResults[0].Result.Should().Be(10.56);
        }


        private void SetupTestData(out string rulesConfig, out EvaluationInput[] evaluationInputs)
        {
            rulesConfig = GetRulesConfig();

            evaluationInputs = SetupInput();
        }

        private EvaluationInput[] SetupInput()
        {
            EvaluationInput[] evaluationInputs;

            StringBuilder builder = new StringBuilder("{");
            builder.Append("\"age\":65");
            builder.Append(",");
            builder.Append("\"durationInMonths\":12");
            builder.Append("}");

            //dynamic data = new ExpandoObject();
            //data.age = 65;
            //data.durationInMonths = 12;

            evaluationInputs = new EvaluationInput[] {
                new EvaluationInput(key: "input", value: builder.ToString())
            };
            return evaluationInputs;
        }

        private string GetRulesConfig()
        {
            string rulesConfig;
            var rulesConfigPath = "..\\..\\..\\TestData\\RuleConfigs\\FDInterestRates.json";

            rulesConfig = GetRulesConfig(rulesConfigPath);
            return rulesConfig;
        }

        private string GetRulesConfig(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);

            return Encoding.UTF8.GetString(bytes);


        }
    }
}
