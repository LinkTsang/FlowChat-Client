using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace FlowChatApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChatViewModel : ViewModelBase
    {
        ViewModelLocator ViewModelLocator { get; } = new ViewModelLocator();
        IChatService ChatService => ViewModelLocator.ChatService;
        ViewModelBase _currentContentViewModel;

        public ViewModelBase CurrentContentViewModel
        {
            get => _currentContentViewModel;
            set => Set(ref _currentContentViewModel, value);
        }

        public ObservableCollection<Chat> Chats { get; } = new ObservableCollection<Chat>();

        Chat _currentChat;
        public Chat CurrentChat
        {
            get => _currentChat;
            set
            {
                if (value == _systemChat)
                {
                    CurrentContentViewModel = InvationListViewModel;
                }
                else
                {
                    CurrentContentViewModel = this;
                }
                Set(ref _currentChat, value);
            }
        }

        public ObservableCollection<Contract> Contracts { get; } = new ObservableCollection<Contract>();

        Contract _currentContract;
        public Contract CurrentContract
        {
            get => _currentContract;
            set
            {
                Set(ref _currentContract, value);
                ViewModelLocator.UserInfo.Contract = CurrentContract;
                CurrentContentViewModel = ViewModelLocator.UserInfo;
            }
        }

        public ObservableCollection<Group> Groups { get; } = new ObservableCollection<Group>();

        Group _currentGroup;
        public Group CurrentGroup
        {
            get => _currentGroup;
            set
            {
                Set(ref _currentGroup, value);
                ViewModelLocator.GroupInfo.Group = CurrentGroup;
                CurrentContentViewModel = ViewModelLocator.GroupInfo;
            }
        }

        public InvationListViewModel InvationListViewModel => ServiceLocator.Current.GetInstance<InvationListViewModel>();

        Account _account;
        public Account CurrentAccount
        {
            get => _account;
            set => Set(ref _account, value);
        }

        string _chatTitle;
        public string ChatTitle
        {
            get => CurrentChat == null ? "" : CurrentChat.PeerName;
            set => Set(ref _chatTitle, value);
        }

        string _contentToSend;
        public string ContentToSend
        {
            get => _contentToSend;
            set => Set(ref _contentToSend, value);
        }

        /// <summary>
        /// Initializes a new instance of the ChatViewModel class.
        /// </summary>
        public ChatViewModel()
        {
            CurrentContentViewModel = this;

            PropertyChanged += ChatViewModel_PropertyChanged;

            SendCommand = new RelayCommand(SendMessage, () => !string.IsNullOrEmpty(ContentToSend));
            ShowAccountInfoCommand = new RelayCommand(ShowAccountInfo);

            AddContractCommand = new RelayCommand(AddContract);
            AddGroupCommand = new RelayCommand(AddGroup);
            SetUpData();
        }

        PrivateChat GetPrivateChat(string username)
        {
            var chat = Chats
                .OfType<PrivateChat>()
                .FirstOrDefault(c => c.Contract.User.Username == username);
            if (chat == null)
            {
                var contract = Contracts.FirstOrDefault(c => c.User.Username == username);
                chat = new PrivateChat(contract);
            }
            return chat;
        }

        GroupChat GetGroupChat(long groupId)
        {
            var chat = Chats
                .OfType<GroupChat>()
                .FirstOrDefault(c => c.Group.Id == groupId);
            if (chat == null)
            {
                var group = Groups.FirstOrDefault(g => g.Id == groupId);
                chat = new GroupChat(group);
            }
            return chat;
        }

        SystemChat _systemChat = new SystemChat();
        async void SetUpData()
        {
            {
                Chats.Add(_systemChat);
                CurrentChat = _systemChat;
                InvationListViewModel.Invations.Clear();
                var contractInvationsResult = await ChatService.GetContractInvations();
                var invations = contractInvationsResult.Data;
                invations?.ForEach(i => InvationListViewModel.Invations.Add(i));
            }

            var result = await ChatService.GetAccountInfo();
            CurrentAccount = result.Data;

            var contracts = (await ChatService.GetContracts()).Data;
            contracts?.ForEach(c => Contracts.Add(c));

            var groups = (await ChatService.GetGroups()).Data;
            groups?.ForEach(c => Groups.Add(c));

            var chats = (await ChatService.GetChatHistory()).Data;
            chats?.ForEach(c => Chats.Add(c));

            ChatService.PrivateChatMessageReceived += (sender, message) =>
            {
                var peerName = message.Sender.Username == CurrentAccount.Username
                     ? message.Receiver.Username
                     : message.Sender.Username;
                var chat = GetPrivateChat(peerName);
                chat.AddMessage(message);
                if (CurrentChat == chat)
                {
                    Messenger.Default.Send(new NotificationMessage("ScrollIntoView"));
                }
            };

            ChatService.GroupChatMessageReceived += (sender, message) =>
            {
                var chat = GetGroupChat(message.Group.Id);
                chat.AddMessage(message);
                if (CurrentChat == chat)
                {
                    Messenger.Default.Send(new NotificationMessage("ScrollIntoView"));
                }
            };

            ChatService.ContactRequestMessagesUpdated += (sender, messages) =>
            {
                var invations = InvationListViewModel.Invations;
                foreach (var m in messages)
                {
                    var invation = invations.FirstOrDefault(i => {
                        if(i == null)
                        {
                            return false;
                        }
                        return i.RecordId == m.RecordId;
                    });
                    if (invation == null)
                    {
                        invations.Add(invation);
                    }
                    else
                    {
                        invation.Tag = m.Tag;
                    }
                }
            };

            ChatService.Handle();
        }

        AccountInfoViewModel AccountInfoViewModel => ServiceLocator.Current.GetInstance<AccountInfoViewModel>();
        void ChatViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ContentToSend):
                    SendCommand.RaiseCanExecuteChanged();
                    break;
                case nameof(CurrentAccount):
                    AccountInfoViewModel.Account = CurrentAccount;
                    break;
            }
        }

        public RelayCommand SendCommand { get; }

        void SendMessage()
        {
            if (string.IsNullOrEmpty(ContentToSend))
            {
                return;
            }
            switch (CurrentChat)
            {
                case PrivateChat c:
                    {
                        ChatService.SendMessage(c.Contract.User.Username, ContentToSend);
                        CurrentChat.AddMessage(new PrivateMessage(-1, DateTime.Now, CurrentAccount, c.Contract.User, ContentToSend));
                    }
                    break;
                case GroupChat c:
                    {
                        ChatService.SendGroupMessage(c.Group.Id, ContentToSend);
                        // CurrentChat.AddMessage(new GroupMessage(-1, DateTime.Now, CurrentAccount, c.Group, ContentToSend));
                    }
                    break;
            }
            ContentToSend = string.Empty;
        }

        public RelayCommand AddContractCommand { get; }
        AddContractViewModel AddContractViewModel => ServiceLocator.Current.GetInstance<AddContractViewModel>();

        void AddContract()
        {
            WindowService.ShowDialog(AddContractViewModel, _ =>
            {

            });
        }
        public RelayCommand AddGroupCommand { get; }
        AddGroupViewModel AddGroupViewModel => ServiceLocator.Current.GetInstance<AddGroupViewModel>();

        void AddGroup()
        {
            WindowService.ShowDialog(AddGroupViewModel, _ =>
            {
            });
        }
        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();

        MessageBoxViewModel ErrorMessageViewModel => ServiceLocator.Current.GetInstance<MessageBoxViewModel>();
        public RelayCommand ShowAccountInfoCommand { get; }
        public void ShowAccountInfo()
        {
            WindowService.ShowDialog(AccountInfoViewModel, async p =>
            {
                if (p == null)
                {
                    return;
                }
                var update = (bool)p;
                if (update)
                {
                    var result = await ChatService.UpdateAccountInfo(CurrentAccount);
                    if (result.Ok)
                    {
                        CurrentAccount = result.Data;
                    }
                    else
                    {
                        ErrorMessageViewModel.Message = result.Message;
                        WindowService.ShowDialog(ErrorMessageViewModel);
                    }
                }
                else
                {
                    var result = await ChatService.GetAccountInfo();
                    if (result.Ok)
                    {
                        CurrentAccount = result.Data;
                    }
                }
            });
        }
    }
}