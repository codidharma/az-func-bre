using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Models
{
    public class EvaluationInputParameter
    {   

        [Required]
        [MinLength(1)]
        public string Name { get; private set; }

        [Required]
        public object Value { get; private set; }

        public EvaluationInputParameter(string name, object value)
        {
            Name = name;

            if (value != null && value.GetType() == typeof(string))
            {
                Value = ConvertJsonToExpandoObject(value);
            }
            else
            {
                Value = value;
            }

            //Value = value;
        }

        public bool Validate(out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                instance: this,
                validationContext: new ValidationContext(this,null, null),
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
