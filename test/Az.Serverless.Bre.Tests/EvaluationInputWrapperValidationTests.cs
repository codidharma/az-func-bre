using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Validators;
using FluentAssertions;
using System.Formats.Asn1;

namespace Az.Serverless.Bre.Tests
{
    public class EvaluationInputWrapperValidationTests
    {
        [Fact]
        public async Task Validator_Should_Return_Failed_Validation_Results_For_Incorrect_Evaluation_Inputs()
        {
            //Arrange
            var evaluationInputWrapper = new EvaluationInputWrapper();
            evaluationInputWrapper.EvaluationInputs = new List<EvaluationInput>
            {
                new EvaluationInput("input", null),
                new EvaluationInput(null, null)
            };

            var validator = new EvaluationInputWrapperValidator();

            //Act
            var result = await validator.ValidateAsync(evaluationInputWrapper);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3);

        }

        [Fact]
        public async Task Validator_Should_Return_Failed_Validation_If_Evaluation_Input_Collection_IsMissing()
        {
            //Arrange 
            var evaluationInputWrapper = new EvaluationInputWrapper();

            var validator = new EvaluationInputWrapperValidator();

            //Act
            var result = await validator.ValidateAsync(evaluationInputWrapper);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);

            
        }
    }
}
