using CommonServiceLocator;
using FlowChatApp.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class UserInfoViewModel : ViewModelBase
    {
        Contract _contract;

        public Contract Contract
        {
            get => _contract;
            set => Set(ref _contract, value);
        }

        public UserInfoViewModel()
        {
            if (IsInDesignMode)
            {
                var user = new User(123, "flow@flowchat.com", "flow", "flow",
                    "China GuangZhou", "1234567890", "Hello Flow", Gender.Unknown, string.Empty);
                Contract = new Contract(user, "Flow");
            }

            ChatWithContractCommand = new RelayCommand(ChatWithContract);
        }

        public RelayCommand ChatWithContractCommand { get; }

        void ChatWithContract()
        {
            var curentContract = Contract;
            var chat = ServiceLocator.Current.GetInstance<ChatViewModel>();
            var result = chat.Chats
                .OfType<PrivateChat>()
                .FirstOrDefault(c => c.Contract.User.Username == curentContract.User.Username);
            if(result == null)
            {
                var c = new PrivateChat(curentContract);
                chat.Chats.Add(c);
                result = c;
            }
            chat.CurrentChat = result;
            chat.CurrentContentViewModel = chat;
        }
    }
}
