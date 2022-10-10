using System.Collections.Generic;

namespace Az.Serverless.Bre.Func01.Models
{
    public class EvaluationOutput
    {
        public bool IsEvaluationSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public List<ExecutionResult> ExecutionResults { get; set; }
    }
}
