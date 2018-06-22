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
using CommonServiceLocator;
using FlowChatApp.Service.Interface;
using System.Windows;
using FlowChatApp.Utility;

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
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        public static readonly string DefaultAvatar = "pack://application:,,,/FlowChatApp;component/Images/ai.png";
        public static readonly string DefaultBoyAvatar = "pack://application:,,,/FlowChatApp;component/Images/boy.png";
        public static readonly string DefaultGirlAvatar = "pack://application:,,,/FlowChatApp;component/Images/girl.png";

        public static readonly ImageSource DefaultAvatarImage;
        public static readonly ImageSource DefaultBoyAvatarImage;
        public static readonly ImageSource DefaultGirlAvatarImage;

        static ImageSource GenDefaultAvatar(string resourcePath)
        {
            var info = Application.GetResourceStream(new Uri(resourcePath));
            var data = ReadFully(info.Stream);
            return GenImageSource(data);
        }
        static ImageSource GenImageSource(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var memory = new MemoryStream(imageData))
            {
                memory.Position = 0;
                image.BeginInit();
                image.StreamSource = memory;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        static User()
        {
            //DefaultAvatarImage = GenDefaultAvatar(DefaultAvatar);
            //DefaultBoyAvatarImage = GenDefaultAvatar(DefaultBoyAvatar);
            //DefaultGirlAvatarImage = GenDefaultAvatar(DefaultGirlAvatar);
        }

        public User()
        {
            PropertyChanged += User_PropertyChanged;
        }

        void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //case nameof(HeadUrl):
                //    {
                //        RaisePropertyChanged(nameof(Avatar));
                //        break;
                //    }
            }
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

        public void MergeFrom(User user)
        {
            Id = user.Id;
            Email = user.Email;
            UserName = user.UserName;
            NickName = user.NickName;
            Region = user.Region;
            Phone = user.Phone;
            Status = user.Status;
            Gender = user.Gender;
            HeadUrl = user.HeadUrl;
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

        async Task<ImageSource> LoadAvatar()
        {
            var response = await ChatService.GetAvator(UserName);
            if (response.Ok && response.Data != null)
            {
                return GenImageSource(response.Data);
            }
            return null;
        }

        ImageSource _avatar;
        [JsonIgnore]
        public ImageSource Avatar
        {
            get
            {
                if (_avatar == null)
                {
                    UpdateAvatar();
                }
                return _avatar;
            }
            set => Set(ref _avatar, value);
        }

        public void UpdateAvatar()
        {
            DispatcherFetcher.Dispatcher.InvokeAsync(async () =>
            {
                var avatar = await LoadAvatar();
                if (avatar != null)
                {
                    Avatar = avatar;
                }
                else
                {
                    switch (Gender)
                    {
                        case Gender.Boy:
                            Avatar = DefaultBoyAvatarImage;
                            break;
                        case Gender.Girl:
                            Avatar = DefaultGirlAvatarImage;
                            break;
                        default:
                            Avatar = DefaultAvatarImage;
                            break;
                    }
                }
            });
        }
    }
}
