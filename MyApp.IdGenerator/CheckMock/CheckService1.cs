using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.CheckMock
{
    public class CheckService1(ICheckService2 service2)
    {
        private readonly ICheckService2 service2 = service2;

        public int SomeCalculation(int a)
        {
            return service2.Add(a, 10);
        }
    }
}
