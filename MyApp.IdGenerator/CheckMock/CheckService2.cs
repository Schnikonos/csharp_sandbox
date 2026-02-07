using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.CheckMock
{
    public interface ICheckService2
    {
        int Add(int a, int b);
    }

    public class CheckService2 : ICheckService2
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
