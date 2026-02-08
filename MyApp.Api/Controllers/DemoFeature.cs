using Microsoft.AspNetCore.Mvc;
using MyApp.Application.service;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoFeature : Controller
    {
        private readonly AsyncDemoService _asyncDemoService;
        private readonly ClientCallService _clientCallService;
        private readonly FileDemoService _fileDemoService;
        private readonly SerializationDemoService _serializationDemoService;

        public DemoFeature(
            AsyncDemoService asyncDemoService, 
            ClientCallService clientCallService,
            FileDemoService fileDemoService,
            SerializationDemoService serializationDemoService)
        {
            _asyncDemoService = asyncDemoService;
            _clientCallService = clientCallService;
            _fileDemoService = fileDemoService;
            _serializationDemoService = serializationDemoService;
        }

        [HttpGet("async")]
        public async void AsyncDemo()
        {
            await _asyncDemoService.RunAsyncDemo();
        }

        [HttpGet("async2")]
        public async void AsyncDemo2()
        {
            await _asyncDemoService.RunAsyncDemo2();
        }

        [HttpGet("client")]
        public async Task<string> ClientCallDemo()
        {
            return await _clientCallService.GetDataFromApi();
        }

        [HttpGet("serialization/json")]
        public void SerializationJson()
        {
            _serializationDemoService.DemoJson();
        }

        [HttpGet("serialization/xml")]
        public void SerializationXml()
        {
            _serializationDemoService.DemoXml();
        }

        [HttpGet("fileDemo")]
        public void FileDemo()
        {
            _fileDemoService.DemoFile();
        }
    }
}
