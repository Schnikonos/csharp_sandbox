namespace MyApp.IdGenerator
{
    public class MyIdService
    {
        public string ComputeId(string name)
        {
            var generator = Ulid.NewUlid().ToString();
            return generator + "_" + name;
        }
    }
}
