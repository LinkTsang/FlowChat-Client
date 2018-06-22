using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class ErrorMessageViewModel : ViewModelBase
    {
        public ErrorMessageViewModel()
        {
            if (IsInDesignMode)
            {
                Message = "oops...";
            }
        }
        string _message;

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }
    }
}
