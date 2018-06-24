using CommonServiceLocator;
using FlowChatApp.Service.Interface;
using FlowChatApp.Utility;
using FlowChatApp.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlowChatApp.Model
{
    public class ChatMessage : ObservableObject
    {
        protected IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();
        public ChatMessage()
        {
        }
        public ChatMessage(long id, DateTime time, User sender, string content)
        {
            _id = id;
            _time = time;
            _sender = sender;
            _content = content;
        }
        long _id;
        public long Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        DateTime _time;
        public DateTime Time
        {
            get => _time;
            set => Set(ref _time, value);
        }


        User _sender;
        public User Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                Set(ref _sender, value);
                RaisePropertyChanged(nameof(IsCurrentAccount));
            }
        }

        string _senderName;
        public string SenderName
        {
            get => _senderName;
            set => Set(ref _senderName, value);
        }

        string _content;
        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        bool _read;
        public bool IsRead
        {
            get => _read;
            set => Set(ref _read, value);
        }

        [JsonIgnore]
        public bool IsCurrentAccount
        {
            get
            {
                var main = SimpleIoc.Default.GetInstance<ChatViewModel>();
                if (main.CurrentAccount == null || Sender == null)
                {
                    return false;
                }
                return main.CurrentAccount.Id == Sender.Id;
            }
        }
    }
    public class PrivateMessage : ChatMessage
    {
        public PrivateMessage()
        {

        }
        public PrivateMessage(long id, DateTime time, User sender, User receiver, string content)
            : base(id, time, sender, content)
        {
            _receiver = receiver;
        }

        User _receiver;
        public User Receiver
        {
            get
            {
                return _receiver;
            }
            set => Set(ref _receiver, value);
        }

        string _receiverName;
        public string ReceiverName
        {
            get => _receiverName;
            set => Set(ref _receiverName, value);
        }
    }

    public class GroupMessage : ChatMessage
    {
        public GroupMessage()
        {

        }
        public GroupMessage(long id, DateTime time, User sender, Group group, string content)
            : base(id, time, sender, content)
        {
            _group = group;
        }

        Group _group;
        public Group Group
        {
            get
            {
                return _group;
            }
            set => Set(ref _group, value);
        }

        long _groupId;
        public long GroudId
        {
            get => _groupId;
            set => Set(ref _groupId, value);
        }
    }

    public abstract class Chat : ObservableObject
    {
        public virtual string PeerName { get; }
        int _count;
        public int UnreadCount
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public virtual ChatMessage RecentMessage { get; }

        public abstract void AddMessage(ChatMessage message);
    }

    public class PrivateChat : Chat
    {

        Contract _contract;
        public Contract Contract
        {
            get => _contract;
            set => Set(ref _contract, value);
        }

        public override string PeerName
        {
            get => Contract.User.Nickname;
        }
        public PrivateChat()
        {
        }

        public PrivateChat(Contract contract) : this()
        {
            Contract = contract;
            Contract.PropertyChanged += (sender, e) => RaisePropertyChanged(nameof(PeerName));
        }

        public ObservableCollection<PrivateMessage> Messages { get; } = new ObservableCollection<PrivateMessage>();

        public override ChatMessage RecentMessage
        {
            get
            {
                if (Messages.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Messages[Messages.Count - 1];
                }
            }
        }

        public override void AddMessage(ChatMessage message)
        {
            Messages.Add((PrivateMessage)message);
        }
    }

    public class GroupChat : Chat
    {
        Group _group;
        public Group Group
        {
            get => _group;
            set => Set(ref _group, value);
        }

        public override string PeerName
        {
            get => Group.Name;
        }

        public GroupChat()
        {
        }
        public GroupChat(Group group) : this()
        {
            Group = group;
            Group.PropertyChanged += (sender, e) => RaisePropertyChanged(nameof(PeerName));
        }

        public ObservableCollection<GroupMessage> Messages { get; } = new ObservableCollection<GroupMessage>();
        public override ChatMessage RecentMessage
        {
            get
            {
                if (Messages.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Messages[Messages.Count - 1];
                }
            }
        }

        public override void AddMessage(ChatMessage message)
        {
            Messages.Add((GroupMessage)message);
        }
    }
    
    public class SystemChat : Chat
    {
        public override void AddMessage(ChatMessage message)
        {
        }

        public ImageSource Avatar => User.DefaultAvatarImage;
    }
}
