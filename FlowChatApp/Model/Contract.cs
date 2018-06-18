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
        public Contract(User user)
        {
            _user = user;
        }

        public Contract(User user, string alias) : this(user)
        {
            _alias = alias;
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

        }

        public ContractInvation(string id, string userId, string message)
        {
            _id = id;
            _userId = userId;
            _message = message;
        }

        string _id = string.Empty;
        public string Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        string _userId = string.Empty;
        public string UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }
        string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

    }
}
