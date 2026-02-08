using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MyApp.Domain
{
    [XmlRootAttribute("DemoXml", Namespace = "http://www.cpandl.com", IsNullable = false)]
    public class SerializDemoXml
    {
        public string Name { get; set; } = default!;
        
        [XmlAttribute("QuantityAbc")]
        public int Quantity { get; set; }
        
        [XmlElement("DescriptionAbc")]
        public string Description { get; set; } = default!;
    }
}
