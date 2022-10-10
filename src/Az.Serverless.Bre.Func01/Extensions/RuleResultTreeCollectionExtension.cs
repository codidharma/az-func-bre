using Az.Serverless.Bre.Func01.Models;
using RulesEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace Az.Serverless.Bre.Func01.Extensions
{
    public static class RuleResultTreeCollectionExtension
    {
        public static EvaluationOutput ToDto(this List<RuleResultTree> ruleResultTrees)
        {
            bool isSuccess = false;
            string errorMessage = "No rules were executed";
            List<ExecutionResult> executionResults = new();

            var successfullyExecutedRules =
                ruleResultTrees.Where(x => x.IsSuccess).ToList();

            if (successfullyExecutedRules.Count > 0)
            {
                isSuccess = true;
                errorMessage = null;

                foreach (var rule in successfullyExecutedRules)
                {
                    if (rule.ActionResult != null && rule.ActionResult.Output != null)
                    {

                        executionResults.Add((ExecutionResult)rule.ActionResult.Output);
                    }


                }

            }

            return new EvaluationOutput
            {
                IsEvaluationSuccessful = isSuccess,
                ErrorMessage = errorMessage,
                ExecutionResults = executionResults

            };


        }
    }
}
