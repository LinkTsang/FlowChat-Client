using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommonServiceLocator;
using FlowChatApp.Frameworks;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using FlowChatApp.View.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace FlowChatApp.ViewModel
{
    public class SignInViewModel : ViewModelBase
    {
        ViewModelLocator ViewModelLocator { get; } = new ViewModelLocator();

        static Dispatcher Dispatcher => Application.Current.Dispatcher;

        ISignInView SignInView => ServiceLocator.Current.GetInstance<ISignInView>();
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();

        public Account Account { get; } = new Account();

        bool _isLogging;
        public bool IsLogging
        {
            get => _isLogging;
            set
            {
                Set(() => IsLogging, ref _isLogging, value);
                RaisePropertyChanged(nameof(SignInCommand));
            }
        }

        string _email;
        public string Email
        {
            get => _email;
            set => Set(() => Email, ref _email, value);
        }

        public SignInViewModel()
        {
        }

        ICommand _signInCommand;
        public ICommand SignInCommand =>
            _signInCommand ?? (_signInCommand = new RelayCommand(async () => await OnSignIn(), () => !_isLogging));

        async Task OnSignIn()
        {
            var password = SignInView.GetPassword();
            IsLogging = true;
            var response = await ChatService.SignInAsync(Email, password);
            if (response.Ok)
            {
                Messenger.Default.Send(new NotificationMessage("LoginSuccessfully"));
            }
            else
            {
                WindowService.ShowMessage("Flow Chat", response.Message);
            }
            IsLogging = false;
        }

        ICommand _signUpCommand;
        public ICommand SignUpCommand =>
            _signUpCommand ?? (_signUpCommand = new RelayCommand(OnSignUp));

        void OnSignUp()
        {
            ViewModelLocator.Login.CurrentLoginViewModel = ViewModelLocator.SignUp;
        }
    }
}
