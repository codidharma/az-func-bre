using Az.Serverless.Bre.Func01.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Az.Serverless.Bre.Tests
{
    public class EvaluationInputModelValidationTests
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ModelValidator_Should_Return_False_And_Validation_Error_When_Name_Property_IsNullOrEmpty(string name)
        {
            //Arrange
            EvaluationInputParameter evaluationInput = new(
                name: null, value: new ExpandoObject()
                );

            List<ValidationResult> errors;

            //Act
            
            bool isValid = evaluationInput.Validate(out errors);

            //Assert

            isValid.Should().BeFalse();
            errors.Count.Should().Be(1);
            errors[0].ErrorMessage
                .Should().BeEquivalentTo("The Name field is required.");

        }

        [Fact]
        public void ModelValidator_Should_Return_False_And_Validation_Error_When_Value_Property_Is_Null()
        {
            //Arrange
            EvaluationInputParameter evaluationInputParameter = new(
                name: "Input", value: null);

            List<ValidationResult> errors;

            //Act
            bool isValid = evaluationInputParameter.Validate(out errors);

            //Assert

            isValid.Should().BeFalse();
            errors.Count.Should().Be(1);
            errors[0].ErrorMessage
                .Should().BeEquivalentTo("The Value field is required.");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void 
            ModelValidator_Should_Return_False_And_validation_Errors_When_Both_Name_And_Value_Props_Are_Null(string name, object value)
        {
            //Arrange

            EvaluationInputParameter evaluationInputParameter = new(
                name:null, value:null
                );

            List<ValidationResult> errors;

            //Act

            bool isValid = evaluationInputParameter.Validate(out errors);

            //Assert

            isValid.Should().BeFalse();
            errors.Count.Should().Be(2);
        }

        [Fact]
        public void Validate_Should_Return_True_And_No_Validation_Errors_For_Valid_Name_And_Value_Properties()

        {
            //Arrange

            EvaluationInputParameter evaluationInputParameter = new(
                name:"Input", value: "{}"
                );

            List<ValidationResult> errors;

            //Act
            bool isValid = evaluationInputParameter.Validate(out errors);

            //Assert

            isValid.Should().BeTrue();
            errors.Count.Should().Be(0);

        }

    }
}
