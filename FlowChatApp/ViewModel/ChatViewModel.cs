using CommonServiceLocator;
using FlowChatApp.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

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
                CurrentContentViewModel = this;
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
                ViewModelLocator.UserInfo.Contract = CurrentContract;
                CurrentContentViewModel = ViewModelLocator.UserInfo;
                Set(ref _currentContract, value);
            }
        }

        public ObservableCollection<Group> Groups { get; } = new ObservableCollection<Group>();

        Group _currentGroup;
        public Group CurrentGroup
        {
            get => _currentGroup;
            set
            {
                ViewModelLocator.GroupInfo.Group = CurrentGroup;
                CurrentContentViewModel = ViewModelLocator.GroupInfo;
                Set(ref _currentGroup, value);
            }
        }

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
            SendCommand = new RelayCommand(SendMessage, () => !string.IsNullOrEmpty(ContentToSend));

            SetUpDesignData();
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
                        var message = new PrivateMessage("123", DateTime.Now, CurrentAccount, c.Peer, ContentToSend);
                        c.Messages.Add(message);
                    }
                    break;
                case GroupChat c:
                    {
                        var message = new GroupMessage("123", DateTime.Now, CurrentAccount, c.Group, ContentToSend);
                        c.Messages.Add(message);
                    }
                    break;
            }
            ContentToSend = string.Empty;
        }

        public RelayCommand ChatWithContractCommand { get; }

        void ShowChatInfo()
        {

        }

        public RelayCommand ChatWithGroupCommand { get; }

        void ShowContractInfo()
        {

        }

        void SetUpDesignData()
        {
            var userInfo = SimpleIoc.Default.GetInstance<UserInfoViewModel>();
            var groupInfo = SimpleIoc.Default.GetInstance<GroupInfoViewModel>();

            var user0 = new Account()
            {
                Id = 0,
                Email = "jack@flowchat.com",
                UserName = "jack",
                NickName = "Jack",
                Gender = Gender.Unknown,
                Status = "Hello World!",
            };

            CurrentAccount = user0;

            var user1 = new User()
            {
                Id = 1,
                Email = "mei@flowchat.com",
                UserName = "mei",
                NickName = "Mei",
                Gender = Gender.Girl,
                Status = "Have Fun Coding!",
                Phone = "1234567890",
                Region = "China Guangzhou"
            };

            var user2 = new User()
            {
                Id = 2,
                Email = "jimmy@flowchat.com",
                UserName = "jimmy",
                NickName = "jimmy",
                Gender = Gender.Boy,
                Status = "No Errors, No Bugs!"
            };

            var contract1 = new Contract(user1, "Han Mei");
            var contract2 = new Contract(user2);

            Contracts.Add(contract1);
            Contracts.Add(contract2);

            CurrentContract = contract1;

            var group0 = new Group()
            {
                Name = "Group Zero",
                Id = "00000",
                Owner = user0
            };

            group0.Members.Add(user0);
            group0.Members.Add(user1);
            group0.Members.Add(user2);

            var group1 = new Group()
            {
                Name = "Group One",
                Id = "00001",
                Owner = user1
            };

            group1.Members.Add(user0);
            group1.Members.Add(user1);
            group1.Members.Add(user2);

            var group2 = new Group()
            {
                Name = "Group Two",
                Id = "00002",
                Owner = user2
            };

            group2.Members.Add(user0);
            group2.Members.Add(user1);
            group2.Members.Add(user2);

            Groups.Add(group0);
            Groups.Add(group1);
            Groups.Add(group2);

            CurrentGroup = group0;

            var privateChat = new PrivateChat(user1);
            Array.ForEach(new[]
            {
                new PrivateMessage("0", DateTime.Now, user0, user1, "Hello!"),
                new PrivateMessage("1", DateTime.Now, user1, user0, "Hijack~"),
            }, (e) => privateChat.Messages.Add(e));

            var groupChat = new GroupChat(group0);
            Array.ForEach(new[]
            {
                new GroupMessage("0", DateTime.Now, user0, group0, "Hello!"),
                new GroupMessage("1", DateTime.Now, user1, group0, "Hey!~"),
            }, (e) => groupChat.Messages.Add(e));

            Chats.Add(privateChat);
            Chats.Add(groupChat);

            CurrentChat = privateChat;

            ContentToSend = "Haha~";


            userInfo.Contract = CurrentContract;
            groupInfo.Group = CurrentGroup;
        }
    }
}