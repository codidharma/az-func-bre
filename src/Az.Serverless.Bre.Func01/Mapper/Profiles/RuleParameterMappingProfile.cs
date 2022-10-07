using AutoMapper;
using Az.Serverless.Bre.Func01.Models;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Mapper.Profiles
{
    public class RuleParameterMappingProfile: Profile
    {
        public RuleParameterMappingProfile()
        {
            CreateMap<EvaluationInputParameter, RuleParameter>();
        }
    }
}
