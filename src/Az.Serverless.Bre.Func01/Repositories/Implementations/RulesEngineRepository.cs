using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Repositories.Implementations
{
    public class RulesEngineRepository : IRulesEngineRepository
    {
        public async Task<bool> ValidateDataInput(RuleParameter[] ruleParameter)
        {
            return await Task.Factory.StartNew(() => {

                bool retVal = true;

                if (ruleParameter == null || ruleParameter.Length == 0)
                {
                    retVal = false;
                    return retVal;
                }

                var inputsWithInValidData =
                    ruleParameter
                    .Where(x => x.Value == null || string.IsNullOrEmpty(x.Name)).ToArray();

                if (inputsWithInValidData.Length > 0)
                    retVal = false;

                return retVal;
            }).ConfigureAwait(false);
        }

        public async Task<bool> ValidateRulesConfig(object config)
        {
            return await Task.Factory.StartNew(() => {
                
                bool retVal = true;

                if(config == null)
                    retVal = false;

                return retVal;
            });
        }
    }
}
