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
        public void Validate_Should_Return_False_And_Validation_Error_When_Key_Property_IsNullOrEmpty(string name)
        {
            //Arrange
            EvaluationInput evaluationInput = new(
                key: null, value: new ExpandoObject()
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
        public void Validate_Should_Return_False_And_Validation_Error_When_Value_Property_Is_Null()
        {
            //Arrange
            EvaluationInput evaluationInputParameter = new(
                key: "Input", value: null);

            List<ValidationResult> errors;

            //Act
            bool isValid = evaluationInputParameter.Validate(out errors);

            //Assert

            isValid.Should().BeFalse();
            errors.Count.Should().Be(1);
            errors[0].ErrorMessage
                .Should().BeEquivalentTo("The StringifiedJsonMessage field is required.");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void
            Validate_Should_Return_False_And_validation_Errors_When_Both_Name_And_Value_Props_Are_Null(string name, object value)
        {
            //Arrange

            EvaluationInput evaluationInputParameter = new(
                key: null, value: null
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

            EvaluationInput evaluationInputParameter = new(
                key: "Input", value: "{}"
                );

            List<ValidationResult> errors;

            //Act
            bool isValid = evaluationInputParameter.Validate(out errors);

            //Assert

            isValid.Should().BeTrue();
            errors.Count.Should().Be(0);

        }

        [Fact]
        public void EvaluationInputParameter_Constructor_Should_Convert_Json_String_To_Expando_Object()
        {
            //Arrange
            EvaluationInput evaluationInput = new(
                key: "Input", value: "{}");


            //Act

            evaluationInput.StringifiedJsonMessage
                .Should().BeOfType<ExpandoObject>();

        }

    }
}
