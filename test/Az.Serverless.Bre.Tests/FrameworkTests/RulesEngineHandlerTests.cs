using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Tests.FrameworkTests
{
    public class RulesEngineHandlerTests
    {
        private readonly IRulesEngineHandler _rulesEngineHandler;

        [Fact]
        public async Task Execute_Rules_Throws_Exception_When_Rules_Config_Is_Null()
        {
            //Arrange

            //Act
            Func<Task> task = async () =>{
                await _rulesEngineHandler.ExecuteRulesAsync(null);
            };

            await task.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void RulesEngineHandler_Constructor_Throws_Argument_Null_Exception_For_Null_Rules_Engine_Dependency_Injection()
        {
            //Act
            Action action = () =>
            {
                new RulesEngineHandler(null);
            };

            action.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'rulesEngine')");
        }
    }
}
