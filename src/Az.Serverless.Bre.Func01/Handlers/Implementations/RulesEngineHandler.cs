using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Az.Serverless.Bre.Func01.Models;
using Newtonsoft.Json;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Handlers.Implementations
{
    public class RulesEngineHandler : IRulesEngineHandler
    {
        private readonly IRulesEngine _rulesEngine;
        public RulesEngineHandler(IRulesEngine rulesEngine)
        {
            _rulesEngine = rulesEngine ??
                throw new ArgumentNullException(nameof(rulesEngine));
        }

        public void AddOrUpdateWorkflows(string workflowString)
        {
            var workflows = JsonConvert.DeserializeObject<List<Workflow>>(workflowString);

            _rulesEngine.AddOrUpdateWorkflow(workflows.ToArray());

        }

        public async Task ExecuteRulesAsync(string rulesConfigFile, EvaluationInputParameter[] evaluationInputs)
        {
            if (string.IsNullOrEmpty(rulesConfigFile))
                throw new 
                    ArgumentNullException(message: "The rules config should not be null or empty string", null);

            if (evaluationInputs == null || evaluationInputs.Length == 0)
                throw new 
                    ArgumentNullException(message: "EvaluationInputParamerters can not be null or empty", null);
            
            this.AddOrUpdateWorkflows(rulesConfigFile);

            
        }
    }
}
