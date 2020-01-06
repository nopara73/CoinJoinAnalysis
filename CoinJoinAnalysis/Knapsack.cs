using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class Knapsack
    {
        public decimal Precision { get; }
        public Mapping Mapping { get; }
        public Knapsack(Mapping mapping)
        {
            var precision = mapping.Precision;
            Precision = precision;
            if (precision != 0m)
            {
                throw new NotImplementedException("Knapsack mixing is only implemented for transactions with 0 fees.");
            }
            Mapping = Mix(mapping);
        }
        public Mapping Mix(Mapping mapping)
        {
            var currentKnapsackTransaction = mapping.SubSets.First();
            foreach (var subTransaction in mapping.SubSets.Skip(1))
            {
                currentKnapsackTransaction = MixTransactions(currentKnapsackTransaction, subTransaction);
            }


            return new Mapping(currentKnapsackTransaction);
        }

        /// <summary>
        /// Listing 2: Simple output splitting algorithm
        /// https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf
        /// </summary>
        private SubSet MixTransactions(SubSet tx1, SubSet tx2)
        {
            var allInputs = tx1.Inputs.Concat(tx2.Inputs);
            var taInputsSum = tx1.Inputs.Sum(x => x.Value);
            var tbInputsSum = tx2.Inputs.Sum(x => x.Value);

            if (taInputsSum.Almost(tbInputsSum, Precision))
            {
                return new SubSet(allInputs, tx1.Outputs.Concat(tx2.Outputs), Precision);
            }
            IEnumerable<Coin> newOutputs;
            if (taInputsSum > tbInputsSum)
            {
                var diff = taInputsSum - tbInputsSum;
                newOutputs = RealizeSubSum(tx1.Outputs, diff).Concat(tx2.Outputs);
            }
            else
            {
                var diff = tbInputsSum - taInputsSum;
                newOutputs = RealizeSubSum(tx2.Outputs, diff).Concat(tx1.Outputs);
            }

            return new SubSet(allInputs, newOutputs, Precision);
        }

        /// <summary>
        /// Listing 2: Simple output splitting algorithm
        /// https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf
        /// </summary>
        private IEnumerable<Coin> RealizeSubSum(IEnumerable<Coin> outputs, decimal diff)
        {
            var subSum = diff;
            foreach (var coin in outputs)
            {
                if (subSum == 0)
                {
                    yield return coin;
                }
                else if (coin.Value <= subSum)
                {
                    yield return coin;
                    subSum -= coin.Value;
                }
                else
                {
                    yield return Coin.Random(subSum);
                    yield return Coin.Random(coin.Value - subSum);
                    subSum = 0;
                }
            }
        }
    }
}
