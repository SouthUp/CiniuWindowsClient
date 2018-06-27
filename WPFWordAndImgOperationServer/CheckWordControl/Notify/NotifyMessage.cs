using System;
namespace CheckWordControl.Notify
{
    public class NotifyMessage
    {
        private readonly string _headerText;
        private readonly string _bodyText;
        private readonly Action<string> _clickAction;

        public NotifyMessage(string headerText, string bodyText, Action<string> clickAction)
        {
            _headerText = headerText;
            _bodyText = bodyText;
            _clickAction = clickAction;
        }

        public NotifyMessage(string bodyText, Action<string> clickAction)
        {
            _headerText = "词牛通知";
            _bodyText = bodyText;
            _clickAction = clickAction;
        }

        public string HeaderText
        {
            get { return _headerText; }
        }

        public string BodyText
        {
            get { return _bodyText; }
        }

        public Action<string> ClickAction
        {
            get { return _clickAction; }
        }
    }
}
