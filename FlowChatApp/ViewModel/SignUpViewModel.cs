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

namespace FlowChatApp.ViewModel
{
    public class SignUpViewModel : ViewModelBase
    {
        ViewModelLocator ViewModelLocator { get; } = new ViewModelLocator();

        static Dispatcher Dispatcher => Application.Current.Dispatcher;

        ISignUpView SignUpView => ServiceLocator.Current.GetInstance<ISignUpView>();
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();

        string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        string _username;
        public string UserName
        {
            get => _username;
            set => Set(ref _username, value);
        }


        string _nickname;
        public string NickName
        {
            get => _nickname;
            set => Set(ref _nickname, value);
        }

        bool _isLogging;
        public bool IsLogging
        {
            get => _isLogging;
            set
            {
                Set(() => IsLogging, ref _isLogging, value);
                RaisePropertyChanged(nameof(SignUpCommand));
            }
        }

        string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }
        public SignUpViewModel()
        {
            Email = "admin@flowchat.com";
            UserName = "admin";
            NickName = "admin";
        }

        ICommand _signUpCommand;
        public ICommand SignUpCommand
            => _signUpCommand
            ?? (_signUpCommand = new RelayCommand(OnSignUp, () => !_isLogging));

        async void OnSignUp()
        {
            var password = SignUpView.GetPassword();
            var response = await ChatService.SignUpAsync(Email, UserName, NickName, password);
            WindowService.ShowMessage("Flow Chat", response.Message);
        }

        ICommand _signInCommand;
        public ICommand SignInCommand
            => _signInCommand
               ?? (_signInCommand = new RelayCommand(OnSignIn));

        async void OnSignIn()
        {
            ViewModelLocator.Login.CurrentLoginViewModel = ViewModelLocator.SignIn;
        }
    }
}
