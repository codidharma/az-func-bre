using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Az.Serverless.Bre.Func01.Models
{
    public class EvaluationInput
    {

        [Required]
        [MinLength(1)]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "stringifiedJsonMessage")]
        public string StringifiedJsonMessage { get; set; }

    }
}
