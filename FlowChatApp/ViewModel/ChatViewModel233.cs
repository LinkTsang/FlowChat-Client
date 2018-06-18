using CommonServiceLocator;
using FlowChatApp.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;

namespace FlowChatApp.ViewModel
{
    public class ChatViewModel233 : ViewModelBase
    {


        Chat _currentChat;
        public Chat CurrentChat
        {
            get => _currentChat;
            set => Set(ref _currentChat, value);
        }
        /// <summary>
        /// Initializes a new instance of the ChatViewModel class.
        /// </summary>
        public ChatViewModel233()
        {
            if (IsInDesignMode)
            {
                SetUpDesignData();
            }
            else
            {
                SetUpDesignData();
                // Code runs "for real"
            }
        }

        void SetUpDesignData()
        {
            var MainViewModel = ServiceLocator.Current.GetInstance<ChatViewModel>();
            CurrentChat = MainViewModel.CurrentChat;
        }


    }
}