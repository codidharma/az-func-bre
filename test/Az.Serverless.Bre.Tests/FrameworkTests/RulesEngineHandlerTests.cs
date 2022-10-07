using Az.Serverless.Bre.Func01.Handlers.Implementations;
using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using FluentAssertions;
using Newtonsoft.Json;
using RulesEngine.Interfaces;
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

        public RulesEngineHandlerTests()
        {
            IRulesEngine rulesEngine = new RulesEngine.RulesEngine();

            _rulesEngineHandler = new RulesEngineHandler(rulesEngine);
        }

        [Fact]
        public void RulesEngineHandler_Constructor_Throws_Argument_Null_Exception_For_Null_Rules_Engine_Dependency_Injection()
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
        public void  AddOrUpdateWorkflow_Throws_Exception_When_Config_String_Cannot_Be_Deserialized(string configString)
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
    }
}
