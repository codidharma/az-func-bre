using Az.Serverless.Bre.Func01.Handlers.Interfaces;
using RulesEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



    }
}
