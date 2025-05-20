using System;
using System.Xml.Serialization;

namespace PS5_NOR_Modifier.UserControls.UART.Data
{
    public class ErrorCode
    {
        public ErrorCode() 
        {
            ErrorCodeNumber = String.Empty;
            Description= String.Empty;
        }

        [XmlElement(ElementName = "ErrorCode")]
        public string ErrorCodeNumber
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
