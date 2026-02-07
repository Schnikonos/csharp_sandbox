using Moq;
using MyApp.IdGenerator.CheckMock;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.Tests
{
    public class CheckService1Test
    {
        private readonly Mock<ICheckService2> mock;
        private readonly CheckService1 checkService1;
        public CheckService1Test() {
            mock = new Mock<ICheckService2>();
            checkService1 = new CheckService1(mock.Object);
        }

        [Fact]
        public void Test1()
        {
            mock.Setup(m => m.Add(2, 10)).Returns(42);
            var result = checkService1.SomeCalculation(2);
            Assert.Equal(42, result);
        }

        [Fact]
        public void Test2()
        {
            // using Callback to verify the parameters passed to Add method
            mock.Setup(m => m.Add(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((a, b) =>
                {
                    Assert.Equal(2, a);
                    Assert.Equal(10, b);
                })
                .Returns(42);
            var result = checkService1.SomeCalculation(2);
            Assert.Equal(42, result);
        }
    }
}
