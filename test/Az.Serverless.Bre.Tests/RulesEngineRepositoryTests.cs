using Az.Serverless.Bre.Func01.Repositories.Implementations;
using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using BREModels = RulesEngine.Models;

namespace Az.Serverless.Bre.Tests
{
    [ExcludeFromCodeCoverage]
    public class RulesEngineRepositoryTests
    {
        private readonly IRulesEngineRepository _rulesEngineRepository;

        public RulesEngineRepositoryTests()
        {
            _rulesEngineRepository = new RulesEngineRepository();
        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_False_For_Null_Input()
        {
            //Arrange

            //Act

            bool isValid = await _rulesEngineRepository.ValidateDataInput(null);

            //Assert
            isValid.Should().BeFalse();


        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_False_For_Null_Value_Param_In_Input()
        {
            //Arrange

            var dataInputArray = new BREModels.RuleParameter[] { new BREModels.RuleParameter("input", null) };

            //Act
            bool isValid = await _rulesEngineRepository.ValidateDataInput(dataInputArray);

            //Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_False_For_Empty_Name_Param_In_Input()
        {
            //Arrange
            dynamic data = new ExpandoObject();
            data.name = "Test";

            var dataInputArray = new BREModels.RuleParameter[]
                {
                    new BREModels.RuleParameter(string.Empty, data)
                };

            //Act
            bool isValid = await _rulesEngineRepository.ValidateDataInput(dataInputArray);

            //Assert

            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_False_For_Empty_Name_And_Null_Value_Params_In_Input()
        {
            //Arrange
            var dataInputArray = new BREModels.RuleParameter[]
                {
                    new BREModels.RuleParameter(string.Empty, null)
                };

            //Act
            bool isValid = await _rulesEngineRepository.ValidateDataInput(dataInputArray);

            //Assert
            isValid.Should().BeFalse();

        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_False_For_Combination_Of_Valid_and_Invalid_Inputs()
        {
            //Arrange

            dynamic data = new ExpandoObject();
            data.name = "Test";

            var dataInputArray = new BREModels.RuleParameter[]
                {
                    new BREModels.RuleParameter("input", data),
                    new BREModels.RuleParameter("input1", null)
                };

            //Act
            bool isValid = await _rulesEngineRepository.ValidateDataInput(dataInputArray);

            //Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task Rules_Data_Input_Validation_Returns_True_For_Valid_Input()
        {

            //Arrange

            dynamic data = new ExpandoObject();
            data.name = "Test";

            var dataInputArray = new BREModels.RuleParameter[]
                {
                    new BREModels.RuleParameter("input", data)
                };

            //Act

            bool isValid = await _rulesEngineRepository.ValidateDataInput(dataInputArray);

            //Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task Rules_Config_Validation_Returns_False_For_Null_Rules_Config_Input()
        {
            //Arrange

            //Act
            bool isValid = await _rulesEngineRepository.ValidateRulesConfig(null);

            //Assert
            isValid.Should().BeFalse();


        }
    }
}
