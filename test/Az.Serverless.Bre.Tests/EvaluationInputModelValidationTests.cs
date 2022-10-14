using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.Validators;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
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

            var evaluationInput = new EvaluationInput(name, builder.ToString());
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
            var evaluationInput = new EvaluationInput("input", null);
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
            var evaluationInput = new EvaluationInput(name, jsonMessage);

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

        //[Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //public void Validate_Should_Return_False_And_Validation_Error_When_Key_Property_IsNullOrEmpty(string name)
        //{
        //    //Arrange
        //    EvaluationInput evaluationInput = new(
        //        key: null, value: new ExpandoObject()
        //        );

        //    List<ValidationResult> errors;

        //    //Act

        //    bool isValid = evaluationInput.Validate(out errors);

        //    //Assert

        //    isValid.Should().BeFalse();
        //    errors.Count.Should().Be(1);
        //    errors[0].ErrorMessage
        //        .Should().BeEquivalentTo("The Name field is required.");

        //}

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

        

    }
}
