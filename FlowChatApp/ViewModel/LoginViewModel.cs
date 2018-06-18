using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace FlowChatApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        ViewModelLocator ViewModelLocator { get; } = new ViewModelLocator();

        ViewModelBase _currentLoginViewModel;

        public ViewModelBase CurrentLoginViewModel
        {
            get => _currentLoginViewModel;
            set => Set(ref _currentLoginViewModel, value);
        }

        public LoginViewModel()
        {
            CurrentLoginViewModel = ViewModelLocator.SignIn;
        }
    }
}
