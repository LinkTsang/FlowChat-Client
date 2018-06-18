using FlowChatApp.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Model
{
    public class ChatMessage : ObservableObject
    {
        public ChatMessage(string id, DateTime time, User sender, string content)
        {
            _id = id;
            _time = time;
            _sender = sender;
            _content = content;
        }
        string _id;
        public string Id
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
            get => _sender;
            set => Set(ref _sender, value);
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

        public bool IsCurrentAccount
        {
            get
            {
                var main = SimpleIoc.Default.GetInstance<ChatViewModel>();
                return main.CurrentAccount.Id == Sender.Id;
            }
        }
    }
    public class PrivateMessage : ChatMessage
    {
        public PrivateMessage(string id, DateTime time, User sender, User receiver, string content)
            : base(id, time, sender, content)
        {
            _receiver = receiver;
        }

        User _receiver;
        public User Receiver
        {
            get => _receiver;
            set => Set(ref _receiver, value);
        }
    }

    public class GroupMessage : ChatMessage
    {
        public GroupMessage(string id, DateTime time, User sender, Group group, string content)
            : base(id, time, sender, content)
        {
            _group = group;
        }

        Group _group;
        public Group Group
        {
            get => _group;
            set => Set(ref _group, value);
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

        User _user;
        public User Peer
        {
            get => _user;
            set => Set(ref _user, value);
        }
        public override string PeerName
        {
            get => Peer.NickName;
        }
        public PrivateChat(User user)
        {
            Peer = user;
            Peer.PropertyChanged += (sender, e) => RaisePropertyChanged(nameof(PeerName));
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

        public GroupChat(Group group)
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
}
