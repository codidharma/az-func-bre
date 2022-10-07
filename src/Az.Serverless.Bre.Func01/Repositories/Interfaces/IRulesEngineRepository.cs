using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RulesEngine.Models;

namespace Az.Serverless.Bre.Func01.Repositories.Interfaces
{
    public interface IRulesEngineRepository
    {
        /// <summary>
        /// Validates the data input parameters that will be passed to the rules engine
        /// </summary>
        /// <param name="ruleParameter">Array of the input parameters</param>
        /// <returns>Boolean result if the array of input parameters passes validation or not</returns>
        public Task<bool> ValidateDataInput(RuleParameter[] ruleParameter);


        public Task<bool> ValidateRulesConfig(object config);


    }
}
