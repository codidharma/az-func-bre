using Az.Serverless.Bre.Func01.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Validators
{
    public class EvaluationInputValidator: AbstractValidator<EvaluationInput>
    {
        public EvaluationInputValidator()
        {
            RuleFor(evalInput => evalInput.Name)
                .NotEmpty();
            RuleFor(x => x.StringifiedJsonMessage)
                .NotEmpty();
            
        }
    }
}
