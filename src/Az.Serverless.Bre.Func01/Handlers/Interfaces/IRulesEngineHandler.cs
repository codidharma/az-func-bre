using Az.Serverless.Bre.Func01.Models;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Handlers.Interfaces
{
    public interface IRulesEngineHandler
    {
        public void AddOrUpdateWorkflows(string workflowString);

        public Task ExecuteRulesAsync(string rulesConfigFile, EvaluationInputParameter[] evaluationInputs);


    }
}
