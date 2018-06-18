using FlowChatApp.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace FlowChatApp.Model
{
    public enum Gender
    {
        Unknown = 0,
        Boy = 1,
        Girl = 2
    }

    public class User : ObservableObject
    {
        public static readonly string DefaultAvatar = "pack://application:,,,/FlowChatApp;component/Images/ai.png";
        public static readonly string DefaultBoyAvatar = "pack://application:,,,/FlowChatApp;component/Images/boy.png";
        public static readonly string DefaultGirlAvatar = "pack://application:,,,/FlowChatApp;component/Images/girl.png";
        public User()
        {
            _avatar = DefaultBoyAvatar;
        }
        public User(long id, string email, string username, string nickname,
            string region, string phone, string status, Gender gender, string headUrl) : this()
        {
            _id = id;
            _email = email;
            _userName = username;
            _nickName = nickname;
            _region = region;
            _phone = phone;
            _status = status;
            _gender = gender;
            _headUrl = headUrl;
        }

        long _id = 0;
        public long Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }

        string _nickName = string.Empty;
        public string NickName
        {
            get => _nickName;
            set => Set(ref _nickName, value);
        }

        string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set => Set(ref _phone, value);
        }

        string _region = string.Empty;
        public string Region
        {
            get => _region;
            set => Set(ref _region, value);
        }

        Gender _gender = Model.Gender.Unknown;
        public Gender Gender
        {
            get => _gender;
            set => Set(ref _gender, value);
        }

        string _status = string.Empty;
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        string _headUrl = string.Empty;
        public string HeadUrl
        {
            get => _headUrl;
            set => Set(ref _headUrl, value);
        }

        string _avatar = string.Empty;
        [JsonIgnore]
        public string Avatar
        {
            get
            {
                switch (Gender)
                {
                    case Gender.Boy:
                        return DefaultBoyAvatar;
                    case Gender.Girl:
                        return DefaultGirlAvatar;
                    default:
                        return DefaultBoyAvatar;

                }
            }
            set => Set(ref _avatar, value);
        }
    }
}
