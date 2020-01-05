using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class SubSet
    {
        public IEnumerable<decimal> Inputs { get; }
        public IEnumerable<decimal> Outputs { get; }
        public decimal Precision { get; }
        public SubSet(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs, decimal precision)
        {
            if (!inputs.Sum().Almost(outputs.Sum(), precision))
            {
                throw new InvalidOperationException("The sum of inputs must be equal to the sum of outputs.");
            }

            Inputs = inputs;
            Outputs = outputs;
            Precision = precision;
        }
    }
}
