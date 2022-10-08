using Az.Serverless.Bre.Func01.Models;
using Az.Serverless.Bre.Func01.RuleEngineCustomizations;
using FluentAssertions;
using RulesEngine.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Tests
{
    public class ExecutionResultCustomActionTests
    {

        [Fact]
        public void Run_Should_Return_A_Object_Of_Type_ExecutionResult()
        {
            //Arrange
            var resultCustomAction = new ExecutionResultCustomAction();
            var actionContext = new ActionContext(
                context: new Dictionary<string, object> { { "ExecutionResultCustomActionInput", 10.56 } },
                parentResult: null
                );
            var expectedResult = new ExecutionResult { Result = 10.56 };
            //Act
            var resultValueTask = resultCustomAction.Run(actionContext, null);
            var actualExecutionResult = resultValueTask.Result;
            
            //Assert
            actualExecutionResult.Should().BeOfType<ExecutionResult>();
            actualExecutionResult.Should().BeEquivalentTo(expectedResult);
            ((ExecutionResult)actualExecutionResult).Result
                .Should().BeOfType<double>();


        } 
    }
}
