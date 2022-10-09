﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Models
{
    public class EvaluationOutput
    {
        public bool IsEvaluationSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public List<ExecutionResult> ExecutionResults { get; set; }
    }
}