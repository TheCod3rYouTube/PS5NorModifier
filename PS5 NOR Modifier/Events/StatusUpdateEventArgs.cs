using System;


namespace PS5_NOR_Modifier.UserControls.Events
{
    public class StatusUpdateEventArgs : EventArgs
    {
        private string _text;

        public string Text
        {
            get { return _text; }
        }

        public StatusUpdateEventArgs(string newStatus) : base()
        {
            _text = newStatus;
        }
    }
}
