using Az.Serverless.Bre.Func01.Extensions;
using Az.Serverless.Bre.Func01.Models;
using FluentAssertions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Tests
{
    public class RuleResultTreeCollectionExtensionMethodTests
    {
        [Fact]
        public void ToDto_Should_Set_IsEvaluationSuccessful_Flag_In_EvaluationOutput_As_False_If_No_Rule_Is_Executed_Successfully()
        {
            //Arrange
            var ruleResultTreeList = new List<RuleResultTree>();


            //Act
            var evaluationOutput = ruleResultTreeList.ToDto();


            //Assert
            evaluationOutput.IsEvaluationSuccessful
                .Should().BeFalse();
        }

        [Fact]
        public void ToDto_Should_Set_IsEvaluationSuccessful_Flag_In_EvaluationOutput_As_True_If_Atlest_One_Rule_Is_Executed_Successfully()
        {
            //Arrange
            var ruleResultTreeList = new List<RuleResultTree>
            {
                new RuleResultTree
                {
                    IsSuccess = true
                }
            };

            //Act
            var evaluationOutput = ruleResultTreeList.ToDto();

            //Assert
            evaluationOutput.IsEvaluationSuccessful
                .Should().BeTrue();
        }

        [Fact]
        public void ToDto_Should_Set_Error_Message_When_No_Rule_Is_Executed_Successfully()
        {
            //Arrange
            var ruleResultTreeList = new List<RuleResultTree>();

            //Act
            var evaluationOutput = ruleResultTreeList.ToDto();

            //Assert
            evaluationOutput.ErrorMessage
                .Should().BeEquivalentTo("No rules were executed");

            
        }

        [Fact]
        public void ToDto_Should_Not_Set_Error_Message_When_Atleast_One_Rules_Is_Executed_Successfully()
        {
            //Arrange
            var ruleResultTreeList = new List<RuleResultTree>
            {
                new RuleResultTree
                {
                    IsSuccess = true
                }
            };

            //Act
            var evaluationOutput = ruleResultTreeList.ToDto();

            //Assert
            evaluationOutput.ErrorMessage
                .Should().BeNull();
        }

        [Fact]
        public void ToDto_Should_Set_ExecutionResult_List_With_One_Execution_Result_When_One_Rule_Is_Executed()
        {
            //Arrange
            var ruleResultTreeList = new List<RuleResultTree>
            {
                new RuleResultTree
                {
                    ActionResult = new ActionResult()
                    {Output = new ExecutionResult{ Result = 10.56} },
                    IsSuccess = true
                }

            };

            //Act

            var evaluationOutput = ruleResultTreeList.ToDto();

            //Assert
            evaluationOutput
                .ExecutionResults.Count().Should().Be(1);

            
        }

        
    }
}
