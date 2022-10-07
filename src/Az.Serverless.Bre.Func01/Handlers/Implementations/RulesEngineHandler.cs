using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using Newtonsoft.Json;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;


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
    }
}
