using Az.Serverless.Bre.Func01.Models;
using FluentValidation;

namespace Az.Serverless.Bre.Func01.Validators
{
    public class EvaluationInputWrapperValidator : AbstractValidator<EvaluationInputWrapper>
    {
        public EvaluationInputWrapperValidator()
        {
            RuleForEach(x => x.EvaluationInputs)
                .SetValidator(new EvaluationInputValidator());
        }
    }
}
