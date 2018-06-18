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
    public class GroupInfoViewModel : ViewModelBase
    {
        Group _group;

        public Group Group {
            get => _group;
            set => Set(ref _group, value);
        }
        
        public GroupInfoViewModel()
        {
            ChatWithGroupCommand = new RelayCommand(ChatWithContract);
        }

        
        public RelayCommand ChatWithGroupCommand { get; }

        void ChatWithContract()
        {
            var chat = ServiceLocator.Current.GetInstance<ChatViewModel>();
            var result = chat.Chats
                .OfType<GroupChat>()
                .FirstOrDefault( c => c.Group.Name == Group.Name);
            if(result == null)
            {
                var c = new GroupChat(Group);
                chat.Chats.Add(c);
            }
            chat.CurrentChat = result;
            chat.CurrentContentViewModel = chat;
        }
    }
}
