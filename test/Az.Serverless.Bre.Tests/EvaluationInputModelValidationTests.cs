using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Validators;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Az.Serverless.Bre.Tests
{
    public class EvaluationInputModelValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Validator_Should_Return_Failed_Validation_Details_When_Name_Property_Is_NullOrEmpty(string name)
        {
            //Arrange 
            StringBuilder builder = new StringBuilder("{");
            builder.Append("\"age\":65");
            builder.Append(",");
            builder.Append("\"durationInMonths\":12");
            builder.Append("}");

            var evaluationInput = new EvaluationInput
            {
                Name = name,
                StringifiedJsonMessage = builder.ToString()
            };
                
            var validator = new EvaluationInputValidator();

            //Act
            var result = await validator.ValidateAsync(evaluationInput);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("'Name' must not be empty.");


        }

        [Fact]
        public async Task Validator_Should_Return_Failed_Validation_Details_When_StringifiedJsonMessage_Property_Is_Null()
        {
            //Arrange
            var evaluationInput = new EvaluationInput
            {
                Name = "input",
                StringifiedJsonMessage = null
            };
            var validator = new EvaluationInputValidator();

            //Act
            var result = await validator.ValidateAsync(evaluationInput);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("'Stringified Json Message' must not be empty.");

        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        public async Task Validator_Should_Return_Failed_Validation_Details_When_Both_Name_And_StringifiedJsonMessage_Property_Are_Null
            (string name, string jsonMessage)
        {
            //Arrange
            var evaluationInput = new EvaluationInput
            {
                Name = name,
                StringifiedJsonMessage = jsonMessage
            };

            var validator = new EvaluationInputValidator();

            //Act
            var results = await validator.ValidateAsync(evaluationInput);

            //Assert
            results.IsValid.Should().BeFalse();
            results.Errors.Should().HaveCount(2);
            results.Errors[0].ErrorMessage
                .Should().BeEquivalentTo("'Name' must not be empty.");
            results.Errors[1].ErrorMessage
                .Should().BeEquivalentTo("'Stringified Json Message' must not be empty.");
        }


    }
}
