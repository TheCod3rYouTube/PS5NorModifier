using System;
using System.Xml.Serialization;

namespace PS5_NOR_Modifier.UserControls.UART.Data
{
    [XmlRoot("errorCodes")]
    public class ErrorCodesList
    {
        public ErrorCodesList() 
        {
            Items = new List<ErrorCode>();
        }

        [XmlElement("errorCode")]
        public List<ErrorCode> Items { get; set; }
    }
}
