using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class CoinAnalysis
    {
        public decimal Value { get; }
        public IEnumerable<(decimal value, decimal distance)> Inputs { get; }
        public IEnumerable<(decimal value, decimal distance)> Outputs { get; }

        public CoinAnalysis(decimal value, IEnumerable<(decimal value, decimal distance)> inputs, IEnumerable<(decimal value, decimal distance)> outputs)
        {
            Value = value;
            Inputs = inputs;
            Outputs = outputs;
        }

        public override string ToString()
        {
            return $"{Value} - inputs: {string.Join(' ', Inputs.Select(x => x.value + "(" + decimal.Round(x.distance, 2, MidpointRounding.AwayFromZero) + ")"))} | outputs: {string.Join(' ', Outputs.Select(x => x.value + "(" + decimal.Round(x.distance, 2, MidpointRounding.AwayFromZero) + ")"))}";
        }
    }
}
