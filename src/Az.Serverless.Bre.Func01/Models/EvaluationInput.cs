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
        public string Name { get; private set; }

        [Required]
        public object StringifiedJsonMessage { get; private set; }

        public EvaluationInput(string key, object value)
        {
            Name = key;

            if (value != null && value.GetType() == typeof(string))
            {
                StringifiedJsonMessage = ConvertJsonToExpandoObject(value);
            }
            else
            {
                StringifiedJsonMessage = value;
            }

            //Value = value;
        }

        public bool Validate(out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                instance: this,
                validationContext: new ValidationContext(this, null, null),
                validationResults: validationResults,
                validateAllProperties: true
                );

            return isValid;
        }

        private ExpandoObject ConvertJsonToExpandoObject(object value)
        {
            var convertor = new ExpandoObjectConverter();

            return JsonConvert.DeserializeObject<ExpandoObject>((string)value, convertor);
        }

    }
}
