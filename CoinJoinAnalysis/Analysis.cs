using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinJoinAnalysis
{
    public class Analysis
    {
        public IEnumerable<Mapping> Mappings { get; }
        public Mapping NonDerivedMapping { get; }
        public IEnumerable<CoinAnalysis> InputAnalyses { get; }
        public IEnumerable<CoinAnalysis> OutputAnalyses { get; }

        public Analysis(IEnumerable<Mapping> mappings)
        {
            Mappings = mappings;
            NonDerivedMapping = mappings.Single(x => x.SubSets.Count() == 1);

            var mappingCount = mappings.Count();
            var inputAnalyses = new List<CoinAnalysis>();
            foreach (var input in NonDerivedMapping.SubSets.Single().inputs)
            {
                var inputDistances = new List<(decimal value, decimal distance)>();
                foreach (var input2 in NonDerivedMapping.SubSets.Single().inputs.Except(new[] { input }))
                {
                    var commonMappingCount = Mappings.Count(x => x.SubSets.Any(y => y.inputs.Contains(input) && y.inputs.Contains(input2)));
                    inputDistances.Add((input2, (decimal)commonMappingCount / mappingCount));
                }

                var outputDistances = new List<(decimal value, decimal distance)>();
                foreach (var output in NonDerivedMapping.SubSets.Single().outputs)
                {
                    var commonMappingCount = Mappings.Count(x => x.SubSets.Any(y => y.inputs.Contains(input) && y.outputs.Contains(output)));
                    outputDistances.Add((output, (decimal)commonMappingCount / mappingCount));
                }

                inputAnalyses.Add(new CoinAnalysis(input, inputDistances, outputDistances));
            }
            InputAnalyses = inputAnalyses;

            var outputAnalyses = new List<CoinAnalysis>();
            foreach (var output in NonDerivedMapping.SubSets.Single().outputs)
            {
                var outputDistances = new List<(decimal value, decimal distance)>();
                foreach (var output2 in NonDerivedMapping.SubSets.Single().outputs.Except(new[] { output }))
                {
                    var commonMappingCount = Mappings.Count(x => x.SubSets.Any(y => y.outputs.Contains(output) && y.outputs.Contains(output2)));
                    outputDistances.Add((output2, (decimal)commonMappingCount / mappingCount));
                }

                var inputDistances = new List<(decimal value, decimal distance)>();
                foreach (var input in NonDerivedMapping.SubSets.Single().inputs)
                {
                    var commonMappingCount = Mappings.Count(x => x.SubSets.Any(y => y.outputs.Contains(output) && y.inputs.Contains(input)));
                    inputDistances.Add((input, (decimal)commonMappingCount / mappingCount));
                }

                outputAnalyses.Add(new CoinAnalysis(output, outputDistances, inputDistances));
            }
            OutputAnalyses = outputAnalyses;
        }
    }
}
