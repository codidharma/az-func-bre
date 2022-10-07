using AutoMapper;
using Az.Serverless.Bre.Func01.Mapper.Configuration;
using Az.Serverless.Bre.Func01.Models;
using FluentAssertions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Tests
{
    public class RuleParameterMappingTests
    {
        private readonly IMapper _mapper;

        public RuleParameterMappingTests()
        {
            _mapper = AutoMapperConfiguration.Initialize();
        }
        [Fact]
        public void Mapper_Should_Map_EvaluationInput_To_RuleParameter_Without_Throwing_Exception()
        {
            //Arrange
            EvaluationInputParameter evaluationInput = new(
                name: "Input", value: "[{}]"
                );

            var ruleParam = _mapper.Map<RuleParameter>(evaluationInput);

            ruleParam.Should().BeEquivalentTo(evaluationInput);
            
        }
    }
}
