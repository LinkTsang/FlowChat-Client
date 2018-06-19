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

        ChatViewModel ChatViewModel => ServiceLocator.Current.GetInstance<ChatViewModel>();
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

        string _username;
        public string Username
        {
            get => _username;
            set => Set(() => Username, ref _username, value);
        }

        public SignInViewModel()
        {
            SetUpDesignData();
        }

        void SetUpDesignData()
        {
            Username = "test0";
        }
        ICommand _signInCommand;
        public ICommand SignInCommand =>
            _signInCommand ?? (_signInCommand = new RelayCommand(async () => await OnSignIn(), () => !_isLogging));

        async Task OnSignIn()
        {
            var password = SignInView.GetPassword();
            IsLogging = true;
            var response = await ChatService.SignInAsync(Username, password);
            if (response.HasError)
            {
                WindowService.ShowMessage("Flow Chat", response.Message);
                IsLogging = false;
                return;
            }
            ChatViewModel.CurrentAccount = (await ChatService.GetAccountInfo()).Data;
            IsLogging = false;
            Messenger.Default.Send(new NotificationMessage("LoginSuccessfully"));
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
