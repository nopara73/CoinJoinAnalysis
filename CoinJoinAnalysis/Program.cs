using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoinJoinAnalysis
{
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Provide inputs or transaction ID! Example: 21,12,36,28.1 or 0f9f3b68f369b3b95779284d4d0607cb8f5051055c2e1b1813848370496e95aa");
            var firstReply = Console.ReadLine().Trim();
            // To solve the subset sum problem in simplified transactions where we disregard the mining fee, the precision can be zero.
            // However real transactions have mining fees, so the precision should be the mining fee, as the maximum as one peer's balance changed.
            // Even more ambiguity is introduced with joinmarket or wasabi transactions where additional fees makes the precision larger.
            decimal precision;
            IEnumerable<decimal> inputs;
            IEnumerable<decimal> outputs;
            if (uint256.TryParse(firstReply, out uint256 txid))
            {
                var workingParams = await GetParametersFromSmartBitAsync(txid);
                precision = workingParams.precision;
                inputs = workingParams.inputs;
                outputs = workingParams.outputs;
            }
            else
            {
                var workingParams = GetParametersFromUser(firstReply);
                precision = workingParams.precision;
                inputs = workingParams.inputs;
                outputs = workingParams.outputs;
            }

            var nonDerivedMapping = new Mapping(inputs, outputs, precision);
            var mappings = nonDerivedMapping.AnalyzeWithNopara73Algorithm().ToArray();
            var analysis = nonDerivedMapping.Analysis;
            DisplayAnalysis(mappings, analysis);

            // If there is only a single derived mapping, then Knapsack it.
            if (mappings.Length == 2)
            {
                var derivedMapping = mappings.Last();

                // Knapsack mixing for precision != 0 is not implemented.
                if (derivedMapping.Precision == 0m)
                {
                    var knapsack = new Knapsack(derivedMapping);
                    var knapsackMappings = knapsack.Mapping.AnalyzeWithNopara73Algorithm().ToArray();
                    var knapsackAnalysis = knapsack.Mapping.Analysis;

                    Console.WriteLine();
                    Console.WriteLine("Mixing the derived sub transactions with Knapsack algorithm...");
                    DisplayAnalysis(knapsackMappings, knapsackAnalysis);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }

        private static void DisplayAnalysis(IEnumerable<Mapping> mappings, Analysis analysis)
        {
            Console.WriteLine();
            Console.WriteLine("Sub mappings:");
            foreach (var mapping in mappings)
            {
                Console.WriteLine(mapping);
            }

            Console.WriteLine();
            Console.WriteLine("Input match probabilities:");
            foreach (var anal in analysis.InputAnalyses)
            {
                Console.WriteLine(anal);
            }

            Console.WriteLine();
            Console.WriteLine("Output match probabilities:");
            foreach (var anal in analysis.OutputAnalyses)
            {
                Console.WriteLine(anal);
            }
        }

        private static async Task<(IEnumerable<decimal> inputs, IEnumerable<decimal> outputs, decimal precision)> GetParametersFromSmartBitAsync(uint256 txid)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://api.smartbit.com.au/v1/blockchain/tx/{txid.ToString()}");
            var contentString = await response.Content.ReadAsStringAsync();
            var contentJsonDoc = JsonDocument.Parse(contentString);
            var txJsonElem = contentJsonDoc.RootElement.GetProperty("transaction");
            var inputsJsonElem = txJsonElem.GetProperty("inputs");
            var outputsJsonElem = txJsonElem.GetProperty("outputs");

            var inputs = ParseValues(inputsJsonElem);
            var outputs = ParseValues(outputsJsonElem);

            var feeString = txJsonElem.GetProperty("fee").GetString();
            var precision = decimal.Parse(feeString);

            return (inputs, outputs, precision);
        }

        private static (IEnumerable<decimal> inputs, IEnumerable<decimal> outputs, decimal precision) GetParametersFromUser(string firstReply)
        {
            Console.WriteLine("Provide outputs! Example: 25,8,50,14.1");
            var outputsString = Console.ReadLine().Trim();

            var inputs = ParseValues(firstReply);
            var outputs = ParseValues(outputsString);

            var precision = 0m;

            return (inputs, outputs, precision);
        }

        private static IEnumerable<decimal> ParseValues(JsonElement jsonElem)
        {
            foreach (var elem in jsonElem.EnumerateArray())
            {

                var valueString = elem.GetProperty("value").GetString();
                yield return decimal.Parse(valueString);
            }
        }

        private static IEnumerable<decimal> ParseValues(string coinsString)
        {
            foreach (var coinString in coinsString.Split(','))
            {
                yield return decimal.Parse(coinString);
            }
        }
    }
}
