using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xunit.Abstractions;

namespace MyApp.Application.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subset = from i in list where i % 2 == 0 select i * 2;
            var subset2 = list.Where(i => i % 2 == 0).Select(i => i * 2);
            var isPair = subset.All(i => i % 2 == 0);
            output.WriteLine("This is output from {0}", string.Join(", ", subset));
            Assert.Equal(new List<int> { 4, 8 }, subset.ToList());
        }
    }
}
