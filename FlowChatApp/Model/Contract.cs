using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Model
{
    public class Contract : ViewModelBase
    {
        public enum State
        {
            Online,
            Offline
        }

        public Contract()
        {

        }
        public Contract(User user)
        {
            _user = user;
        }

        public Contract(User user, string alias, string category = "") : this(user)
        {
            _alias = alias;
            _category = category;
        }

        User _user;
        public User User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        string _alias = string.Empty;
        public string Alias
        {
            get => _alias;
            set => Set(ref _alias, value);
        }

        string _category = string.Empty;
        public string Category
        {
            get => _category;
            set => Set(ref _category, value);
        }
    }

    public class ContractInvation : ViewModelBase
    {
        public ContractInvation()
        {
            PropertyChanged += (sender, e) => {
                if(e.PropertyName == nameof(Tag))
                {
                    RaisePropertyChanged(nameof(IsAccepted));
                    RaisePropertyChanged(nameof(IsRejected));
                    RaisePropertyChanged(nameof(IsUnhandled));
                }
            };
        }

        public ContractInvation(long recordId, string friendName, string message)
        {
            _recordId = recordId;
            _friendName = friendName;
            _message = message;
        }

        long _recordId;
        public long RecordId
        {
            get => _recordId;
            set => Set(ref _recordId, value);
        }
        string _friendName = string.Empty;
        public string FriendName
        {
            get => _friendName;
            set => Set(ref _friendName, value);
        }
        string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        User _user;
        public User User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        int _tag;
        public int Tag
        {
            get => _tag;
            set => Set(ref _tag, value);
        }

        public bool IsAccepted
        {
            get => Tag == 2;
        }

        public bool IsRejected
        {
            get => Tag == 3;
        }

        public bool IsUnhandled
        {
            get => Tag == 1;
        }
    }

    public class InvationConfirmation : ViewModelBase
    {
        public InvationConfirmation()
        {

        }

        long _recordId;
        public long RecordId
        {
            get => _recordId;
            set => Set(ref _recordId, value);
        }

        string _friendName = string.Empty;
        public string FriendName
        {
            get => _friendName;
            set => Set(ref _friendName, value);
        }

        string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        int _tag;
        public int Tag
        {
            get => _tag;
            set => Set(ref _tag, value);
        }

        int _categoryName;
        public int CategoryName
        {
            get => _categoryName;
            set => Set(ref _categoryName, value);
        }
    }
}
