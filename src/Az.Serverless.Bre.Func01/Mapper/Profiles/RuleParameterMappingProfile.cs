using AutoMapper;
using Az.Serverless.Bre.Func01.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;
using System.Dynamic;

namespace Az.Serverless.Bre.Func01.Mapper.Profiles
{
    public class RuleParameterMappingProfile : Profile
    {
        public RuleParameterMappingProfile()
        {
            
            CreateMap<EvaluationInput, RuleParameter>()
                .ConstructUsing(x => new RuleParameter(x.Name,
                JsonConvert.DeserializeObject<ExpandoObject>(
                x.StringifiedJsonMessage,
                new ExpandoObjectConverter()
                )));
        }
    }
}
