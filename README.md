# MySandbox

## Syntax LINK
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };
var subset = from i in list where i % 2 == 0 select i * 2;
var subset2 = list.Where(i => i % 2 == 0).Select(i => i * 2);
var isPair = subset.All(i => i % 2 == 0);
output.WriteLine("This is output from {0}", string.Join(", ", subset));
```

## DB
See AppDbContext + [appsettings.Development.json](MyApp.Api/appsettings.Development.json) (for connection string)

Declaration in Program.cs:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("Default"),
    b => b.MigrationsAssembly("MyApp.Infrastructure")
));
```

Usage: call in a service or controller:
```csharp
appDbContext.Authors.Add(author);
appDbContext.SaveChanges();
return [.. appDbContext.Authors]; // can also use async methods, e.g. appDbContext.Authors.ToListAsync()
```

## Web
Put static files in wwwroot, e.g. wwwroot/index.html, and they will be served at http://localhost:5000/index.html
In Program.cs, we have:
```csharp
app.UseStaticFiles();
```

## Events
### Classic
see [CEEventArgs.cs](MyApp.IdGenerator/ClassicEvents/CEEventArgs.cs) / [CEServiceSender.cs](MyApp.IdGenerator/ClassicEvents/CEServiceSender.cs) / [CEServiceListener.cs](MyApp.IdGenerator/ClassicEvents/CEServiceListener.cs)

In Program.cs
```chsarp
builder.Services.AddSingleton<CEServiceSender>();  // should be singleton, otherwise we will have multiple instances of sender and listener
builder.Services.AddTransient<CEServiceListener>();  // should be transient, otherwise we will have multiple instances of sender and listener

....

// Force-create one CEServiceListener so it subscribes to the singleton sender. Mandatory if it's not created in the controller, otherwise the event will never be listened to.
app.Services.GetRequiredService<CEServiceListener>();
```

### MediatR
See [CEEventArgs.cs](MyApp.IdGenerator/ClassicEvents/CEEventArgs.cs) / [CEServiceSender.cs](MyApp.IdGenerator/ClassicEvents/CEServiceSender.cs) / [CEServiceListener.cs](MyApp.IdGenerator/ClassicEvents/CEServiceListener.cs)

In Program.cs
```csharp

// Need to scan the assembly where the handlers are located, otherwise they won't be registered and the events won't be listened to.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MyApp.IdGenerator.AssemblyMarker).Assembly));

builder.Services.AddSingleton<AEEventSender>();
builder.Services.AddTransient<AEEventListener>();
```

## Logging
Use of Serilog, configured in Program.cs:
```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
```

And in appsettings.json:
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{TraceId}|{SpanId}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{TraceId}|{SpanId}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
          "path": "logs/app-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Then in class:
```csharp
public class MyClass
{
  private readonly ILogger<MyClass> _logger;
  
  public MyClass(ILogger<MyClass> _logger)
  {
    this.logger = logger;
  }
  
    public void MyMethod()
    {
        _logger.LogInformation("This is an info message");
        _logger.LogWarning("This is a warning message");
        _logger.LogError("This is an error message");
    }
}
```

## Templates
in csproj of API:
```xml
<PropertyGroup>
	<PreserveCompilationContext>true</PreserveCompilationContext>
</PropertyGroup>
```

In csproj of Template project:
```xml
<ItemGroup>
	<EmbeddedResource Include="Templates/**/*.cshtml" />
</ItemGroup>
```

See [HtmlTemplateRenderer.cs](MyApp.Templating/HtmlTemplateRenderer.cs)

Put template in Templates folder, e.g. [TestTemplate.cshtml](MyApp.Templating/Templates/TestTemplate.cshtml)

Then render in code:
```csharp
var html = await renderer.RenderAsync("Templates.AuthorInfo.cshtml", authors);
```

## Unittests
### Basic
```csharp
[Fact]
public void Test1()
{
    MyIdService service = new MyIdService();
    var a = service.ComputeId("aaa");
    Assert.EndsWith("aaa", a);
}
```

### Serialization
See [SerializationDemoService.cs](MyApp.Application/service/SerializationDemoService.cs)
#### JSON

#### XML
Import <PackageReference Include="System.Xml.XmlSerializer" Version="4.3.0" />


### File IO
Use of File.WriteAllText and File.ReadAllText for simple file operations. See [FileDemoService.cs](MyApp.Application/service/FileDemoService.cs)

### Mock
Import Moq package, then: [CheckService1Test.cs](MyApp.IdGenerator.Tests/CheckService1Test.cs)

Or more complex : [BookServiceTest.cs](MyApp.Application.Tests/BookServiceTest.cs)

### Integration test with WebApplicationFactory
Import Microsoft.AspNetCore.Mvc.Testing

See [WeatherForecastControllerTest.cs](MyApp.Api.Tests/WeatherForecastControllerTest.cs)
or more complex [AuthorControllerTest.cs](MyApp.Api.Tests/AuthorControllerTest.cs)

## Async programming
See [AsyncDemoService.cs](MyApp.Application/service/AsyncDemoService.cs)

Basically just use async/await and Task<T> as return type, and make sure to call async methods all the way down to avoid blocking threads.

## ClientCall
1) declare HttpClient in Program.cs:
```csharp
builder.Services.AddHttpClient("demoApiClient", client =>
{
    client.BaseAddress = new Uri("https://127.0.0.1:1234/");
    client.Timeout = TimeSpan.FromSeconds(10);
})
```

2) inject IHttpClientFactory and create client:
```csharp
public class ClientCallService
{
    private readonly HttpClient _httpClient;

    public ClientCallService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("demoApiClient");
    }

    public async Task<string> GetDataFromApi()
    {
        var response = await _httpClient.GetAsync("/api/data");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
```

## Naming conventions
### Core conventions
- **PascalCase** — Types and members
  - Classes, structs, interfaces, enums, public properties, public methods.
  - Example: CustomerOrder, CalculateTotal(), OrderId
- **camelCase** — Local variables and private fields
  - Example: orderCount, totalPrice
- **_camelCase** — Private fields with underscore prefix
  - Widely used in modern C#.
  - Example: _logger, _cacheService
- **ALL_CAPS** — Constants
  - Example: MAX_RETRY_COUNT

### Specific categories
Classes, structs, records
- **PascalCase**
  - Example: InvoiceService, UserProfile
  - Interfaces
- **PascalCase with I prefix**
  - Example: IRepository, ILogger
  - Methods
- **PascalCase, verb‑based**
  - Example: GetUser(), SaveChanges()
  - Properties
- **PascalCase, noun-based**
  - Example: FirstName, CreatedAt
  - Fields
- **Private: _camelCase**
- **Public: PascalCase**
  - Example: _connectionString, BufferSize
  - Local variables & parameters
- **camelCase**
  - Example: userId, filePath
  - Enums
- **Enum type: PascalCase**
- **Enum members: PascalCase**
  - Example:
  - enum OrderStatus { Pending, Shipped, Delivered }


### Namespaces
- **PascalCase, usually Company.Product.Feature**
  - Example: Contoso.Payments.Api
  - Events
- **PascalCase, often ending with Event or using OnXxx for raisers**
  - Example: OrderPlaced, OnOrderPlaced()

### Additional guidelines
- Avoid abbreviations unless they are universally understood (Http, Xml, Id).
- Async methods end with “Async”
  - Example: GetUserAsync()
- File names match type names
  - UserService.cs → contains UserService class.
- Use meaningful names
  - Avoid tmp, data, obj unless context is trivial.
