using System;
using Xunit;
using CoinJoinAnalysis;

namespace CoinJoinAnalysis.Tests
{
    public class MappingTests
    {
        [Fact]
        public void CanCreateMapping()
        {
            new Mapping(new[] { 1m }, new[] { 1m }, 0m);
        }

        [Fact]
        public void AssertSubSetSum()
        {
            Assert.Throws<InvalidOperationException>(() => new Mapping(new[] { 1m }, new[] { 0.9m }, 0m));
        }

        [Fact]
        public void CanUsePrecision()
        {
            new Mapping(new[] { 1m }, new[] { 0.9m }, 1m);
        }
    }
}
