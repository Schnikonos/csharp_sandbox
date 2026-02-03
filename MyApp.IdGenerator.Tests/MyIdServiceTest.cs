namespace MyApp.IdGenerator.Tests
{
    public class MyIdServiceTest
    {
        [Fact]
        public void Test1()
        {
            MyIdService service = new MyIdService();
            var a = service.ComputeId("aaa");
            Assert.EndsWith("aaa", a);
        }
    }
}
