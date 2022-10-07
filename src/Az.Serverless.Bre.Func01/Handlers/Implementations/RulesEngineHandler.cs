using AutoMapper;
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
        private readonly IMapper _mapper;

        public RulesEngineHandler(IRulesEngine rulesEngine, IMapper mapper)
        {
            _rulesEngine = rulesEngine ??
                throw new ArgumentNullException(nameof(rulesEngine));

            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
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

            var ruleInputs = _mapper.Map<RuleParameter[]>(evaluationInputs);

            var result = await _rulesEngine.ExecuteAllRulesAsync(
                workflowName: GetWorkflowName(rulesConfigFile),
                ruleParams: ruleInputs
                ).ConfigureAwait(false);

        }

        public string GetWorkflowName(string rulesCongfig)
        {
            var workflows = JsonConvert.DeserializeObject<List<Workflow>>(rulesCongfig);

            return workflows[0].WorkflowName;
        }
    }
}
