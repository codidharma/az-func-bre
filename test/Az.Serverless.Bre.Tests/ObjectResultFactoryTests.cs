using Az.Serverless.Bre.Func01.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;


namespace Az.Serverless.Bre.Tests
{
    public class ObjectResultFactoryTests
    {
        [Theory]
        [InlineData(200, "application/json", "")]
        [InlineData(400, "application/json", "Bad request")]
        [InlineData(475, "application/json", "Provided content type is not valid")]
        public void ObjectFactory_Should_Return_Correct_Object(int statusCode, string contentType, string message)
        {
            //Arrange
            var expectedObject = new ObjectResult(message)
            {
                StatusCode = statusCode,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue(contentType)
                }
            };

            //Act
            var actualObject = ObjectResultFactory.Create(statusCode, contentType, message);

            //Assert
            actualObject
                .Should().BeEquivalentTo(expectedObject);

            
        }
    }
}
