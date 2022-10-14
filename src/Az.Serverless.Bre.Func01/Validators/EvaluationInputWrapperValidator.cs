using Az.Serverless.Bre.Func01.Models;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Validators
{
    public class EvaluationInputWrapperValidator: AbstractValidator<EvaluationInputWrapper>
    {
        public EvaluationInputWrapperValidator()
        {
            RuleForEach(x => x.EvaluationInputs)
                .SetValidator(new EvaluationInputValidator());
        }
    }
}
