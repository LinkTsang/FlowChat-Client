using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class AddContractMessageViewModel : ViewModelBase
    {
        User _user;

        public User User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        public AddContractMessageViewModel()
        {
            if (IsInDesignMode)
            {
                User = new User(123, "flow@flowchat.com", "flow", "flow",
                    "China GuangZhou", "1234567890", "Hello Flow", Gender.Unknown, string.Empty);
                Message = "Hello";
            }

            FinishCommand = new RelayCommand<bool>(Finish);
        }

        public string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();

        public RelayCommand<bool> FinishCommand { get; }

        async void Finish(bool b)
        {
            if (!b) return;
            await ChatService.AddContact(User.Username, "", Message);
            WindowService.CloseAddContractWindow();
        }
    }
}
