using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class Knapsack
    {
        public Mapping Mapping { get; }
        public Knapsack(Mapping mapping)
        {
            if (mapping.Precision != 0m)
            {
                throw new NotImplementedException("Knapsack mixing is only implemented for transactions with 0 fees.");
            }
            Mapping = Mix(mapping);
        }
        public Mapping Mix(Mapping mapping)
        {
            var currentKnapsackTransaction = mapping.SubSets.First();
            foreach(var subTransaction in mapping.SubSets.Skip(1))
            {
                currentKnapsackTransaction = MixTransactions(currentKnapsackTransaction, subTransaction);
            }

            return new Mapping(currentKnapsackTransaction.inputs, currentKnapsackTransaction.outputs, mapping.Precision);
        }

        /// <summary>
        /// Listing 2: Simple output splitting algorithm
        /// https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf
        /// </summary>
        private (IEnumerable<decimal> inputs, IEnumerable<decimal> outputs) MixTransactions((IEnumerable<decimal> inputs, IEnumerable<decimal> outputs) tx1, (IEnumerable<decimal> inputs, IEnumerable<decimal> outputs) tx2)
        {
            var taInputsSum = tx1.inputs.Sum();
            var tbInputsSum = tx2.inputs.Sum();

            if (taInputsSum == tbInputsSum)
            {
                return (tx1.inputs.Concat(tx2.inputs), tx1.outputs.Concat(tx2.outputs));
            }

            var newInputs = tx1.inputs.Concat(tx2.inputs);
            IEnumerable<decimal> newOutputs;
            if (taInputsSum > tbInputsSum)
            {
                var diff = taInputsSum - tbInputsSum;
                newOutputs = RealizeSubSum(tx1.inputs, diff).Concat(tx2.inputs);
            }
            else
            {
                var diff = tbInputsSum - taInputsSum;
                newOutputs = RealizeSubSum(tx2.inputs, diff).Concat(tx1.inputs);
            }

            return (newInputs, newOutputs);
        }

        /// <summary>
        /// Listing 2: Simple output splitting algorithm
        /// https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf
        /// </summary>
        private IEnumerable<decimal> RealizeSubSum(IEnumerable<decimal> inputs, decimal diff)
        {
            var subSum = diff;
            foreach (var coin in inputs)
            {
                if (subSum == 0)
                {
                    yield return coin;
                }
                else if (coin <= subSum)
                {
                    yield return coin;
                    subSum -= coin;
                }
                else
                {
                    yield return subSum;
                    yield return coin - subSum;
                    subSum = 0;
                }
            }
        }
    }
}
