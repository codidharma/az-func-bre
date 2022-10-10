using Az.Serverless.Bre.Func01.Models;
using RulesEngine.Actions;
using RulesEngine.Models;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.RuleEngineCustomizations
{
    public class ExecutionResultCustomAction : ActionBase
    {
        public ExecutionResultCustomAction()
        {

        }
        public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            var customInput = context.GetContext<object>("ExecutionResultCustomActionInput");

            return new ValueTask<object>(new ExecutionResult { Result = customInput });

        }
    }
}
