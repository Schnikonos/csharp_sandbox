using Microsoft.Extensions.Logging;
using MyApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace MyApp.Application.service
{
    public class SerializationDemoService
    {
        private readonly ILogger<SerializationDemoService> _logger;

        public SerializationDemoService(ILogger<SerializationDemoService> logger)
        {
            _logger = logger;
        }

        public void DemoJson()
        {
            SerializDemoObject serial = new SerializDemoObject { Name = "Name1", Descripcion = "Description 1" };
            string serialStr = JsonSerializer.Serialize(serial);
            SerializDemoObject serial2 = JsonSerializer.Deserialize<SerializDemoObject>(serialStr);
            _logger.LogInformation("Serialization: \n{} \n\n{}", serialStr, serial2);
        }

        public void DemoXml()
        {
            SerializDemoXml serial = new SerializDemoXml { Name = "Name1", Description = "Description 1", Quantity = 4 };
            
            XmlSerializer serializer = new XmlSerializer(typeof(SerializDemoXml));
            
            TextWriter writer = new StringWriter();
            serializer.Serialize(writer, serial);
            string serialStr = writer.ToString();

            TextReader reader = new StringReader(serialStr);
            SerializDemoXml serial2 = (SerializDemoXml)serializer.Deserialize(reader);

            _logger.LogInformation("Serialization: \n{} \n\n{}", serialStr, serial2);
        }
    }
}
