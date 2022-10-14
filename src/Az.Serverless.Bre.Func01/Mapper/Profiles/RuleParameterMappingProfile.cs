using AutoMapper;
using Az.Serverless.Bre.Func01.Models;
using RulesEngine.Models;

namespace Az.Serverless.Bre.Func01.Mapper.Profiles
{
    public class RuleParameterMappingProfile : Profile
    {
        public RuleParameterMappingProfile()
        {
            CreateMap<EvaluationInput, RuleParameter>()
                .ConvertUsing(x => new RuleParameter(x.Name, x.StringifiedJsonMessage));
        }
    }
}
