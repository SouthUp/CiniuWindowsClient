using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CheckWordControl.Notify
{
    public class NotifyMessageViewModel
    {
        private readonly NotifyMessage _content;
        private readonly AnimateLocation _location;
        public readonly Action _closeAction;

        public NotifyMessageViewModel(NotifyMessage content, AnimateLocation location, Action closeAction)
        {
            this._content = content;
            this._location = location;
            this._closeAction = closeAction;
        }

        public NotifyMessage Message
        {
            get { return _content; }
        }

        public AnimateLocation Location
        {
            get { return _location; }
        }
    }
}
