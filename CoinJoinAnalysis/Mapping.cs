using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class Mapping
    {
        public IEnumerable<(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs)> SubSets { get; }
        public decimal Precision { get; }

        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Create a non-derived mapping.
        /// </summary>
        public Mapping(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs, decimal precision)
          : this(new List<(IEnumerable<decimal>, IEnumerable<decimal>)> { (inputs, outputs) }, precision)
        {

        }

        private Mapping(IEnumerable<(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs)> subSets, decimal precision)
        {
            foreach (var (inputs, outputs) in subSets)
            {
                if (!inputs.Sum().Almost(outputs.Sum(), precision))
                {
                    throw new ArgumentException("The sum of inputs must be equal to the sum of outputs.");
                }
            }

            SubSets = subSets;
            Precision = precision;
        }

        public override string ToString()
        {
            return string.Join(" | ", SubSets.Select(x => $"{string.Join(',', x.inputs)} -> {string.Join(',', x.outputs)}"));
        }

        /// <summary>
        /// Loosly optimized. Has no recursion.
        /// </summary>
        public IEnumerable<object> AnalyzeWithNopara73Algorithm()
        {
            var mappings = new List<Mapping>();

            foreach (var (inputs, outputs) in SubSets)
            {
                var outputPartitions = Partitioning.GetAllPartitions(outputs.ToArray());
                var inputPartitions = Partitioning.GetAllPartitions(inputs.ToArray());

                foreach (var inputPartition in inputPartitions)
                {
                    foreach (var outputPartition in outputPartitions.Where(x => x.Length == inputPartition.Length))
                    {
                        var remainingOutputPartition = outputPartition;
                        var validPartition = true;
                        var subSetsBuilder = new List<(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs)>();
                        foreach (var inputPartitionPart in inputPartition)
                        {
                            var foundValidOutputPartitionPart = remainingOutputPartition.FirstOrDefault(x => x.Sum().Almost(inputPartitionPart.Sum(), Precision));
                            // https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf
                            // input partitions that include a set
                            // with a sum that is not a sub sum of the outputs cannot
                            // be part of a mapping
                            if (foundValidOutputPartitionPart is null)
                            {
                                validPartition = false;
                                break;
                            }
                            else
                            {
                                subSetsBuilder.Add((inputPartitionPart, foundValidOutputPartitionPart));
                            }
                        }

                        if (validPartition)
                        {
                            var mapping = new Mapping(subSetsBuilder, Precision);
                            mappings.Add(mapping);
                            yield return mapping;
                            break;
                        }
                    }
                }
            }

            Analysis = new Analysis(mappings);
        }
    }
}
