using Newtonsoft.Json;
using System.Collections.Generic;

namespace Az.Serverless.Bre.Func01.Models
{
    public class EvaluationInputWrapper
    {
        [JsonProperty(PropertyName = "evaluationInputs")]
        public List<EvaluationInput> EvaluationInputs { get; set; }
    }
}
