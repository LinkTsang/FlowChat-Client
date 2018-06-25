using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel()
        {
            if (IsInDesignMode)
            {
                Title = "flowchat: ";
                Message = "oops...";
            }
        }
        string _message;

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
    }
}
