using System;


namespace PS5_NOR_Modifier.UserControls.UART.Events
{
    public class StatusUpdateEventArgs: EventArgs
    {
        private String _text;

        public String Text
        {
            get { return _text; }
        }

        public StatusUpdateEventArgs(String newStatus): base()
        {
            _text = newStatus;
        }
    }
}
