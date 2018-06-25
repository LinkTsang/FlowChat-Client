using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlowChatApp.Service
{
    public class DemoChatService : IChatService
    {
        long _groupId;
        long _userId;
        long _messageId;
        long _contractInvationId;
        long _InvationId;
        long GenGroupId()
        {
            return _groupId++;
        }
        long GenUserId()
        {
            return _userId++;
        }
        long GenMessageId()
        {
            return _messageId++;
        }

        long GenContractInvationId()
        {
            return _contractInvationId++;
        }

        long GenInvationId()
        {
            return _InvationId++;
        }
        public event EventHandler<PrivateMessage> PrivateChatMessageReceived;
        public event EventHandler<GroupMessage> GroupChatMessageReceived;
        public event EventHandler<List<ContractInvation>> ContactRequestMessagesUpdated;
        public event EventHandler<List<ContractInvation>> ContactConfirmationMessageReceived;
        public event EventHandler<Result> BadRequestRaised;

        #region account
        public async Task<Result> SignUpAsync(string email, string username, string nickname, string password)
        {
            var result = Result.BadRequest;
            return result;
        }

        public async Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password)
        {
            var result = new Result<ChatService.TokenClass>(ResultCode.Ok, "OK", new ChatService.TokenClass());
            return result;
        }

        public async Task<Result> SignOutAsync()
        {
            var result = new Result(ResultCode.Ok, "OK", null);
            return result;
        }
        public async Task<Result<ChatService.TokenClass>> UpdateToken()
        {
            var result = new Result<ChatService.TokenClass>(ResultCode.Ok, "OK", new ChatService.TokenClass());
            return result;
        }

        public async Task<Result<Account>> UpdateAccountInfo(Account account)
        {
            CurrentAccount.MergeFrom(account);
            var result = new Result<Account>(ResultCode.Ok, "OK", CurrentAccount);
            return result;
        }

        public async Task<Result<Account>> GetAccountInfo()
        {
            Result<Account> result = new Result<Account>(ResultCode.Ok, "OK", CurrentAccount);
            return result;
        }

        public async Task<Result> UploadAvator(string filename, byte[] avator)
        {
            var fileUrl = "\\images\\" + filename;
            var jObject = new JObject()
            {
                ["fileUrl"] = fileUrl,
            };
            CurrentAccount.HeadUrl = fileUrl;
            _avatarDict[CurrentAccount.Username] = avator;
            var result = new Result(ResultCode.Ok, "OK", jObject);
            return result;
        }

        #endregion

        #region contract
        public async Task<Result<List<Contract>>> GetContracts()
        {
            var result = new Result<List<Contract>>(ResultCode.Ok, "OK", Contracts);
            return result;
        }
        public async Task<Result> AddContact(string username, string categoryName, string message)
        {
            var result = Result.BadRequest;
            if (Contracts.FirstOrDefault(c => c.User.Username == username) == null)
            {
                var user = Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    var invation = new ContractInvation(GenContractInvationId(), username, message);
                    var list = new List<ContractInvation>();
                    list.Add(invation);
                    ContactRequestMessagesUpdated?.Invoke(this, list);
                    return Result.OKRequest;
                }
            }
            return result;
        }
        public async Task<Result> DeleteContact(string username)
        {
            var result = Result.BadRequest;
            var contract = Contracts.FirstOrDefault(c => c.User.Username == username);
            if (contract != null)
            {
                Contracts.Remove(contract);
                result = Result.OKRequest;
            }
            return result;
        }
        public async Task<Result> UpdateContact(string username, string alias)
        {
            var result = new Result(ResultCode.Ok, "OK", null);
            return result;
        }

        public async Task<Result<List<ContractInvation>>> GetContractInvations()
        {
            var result = new Result<List<ContractInvation>>(ResultCode.Ok, "OK", null);
            return result;
        }
        public async Task<Result> ConfirmContractInvation(long recordId, string categoryName, bool accept)
        {
            var result = new Result(ResultCode.Ok, "OK", null);
            return result;
        }
        public async Task<Result<List<ContractInvation>>> GetInvationConfirmations()
        {
            var result = new Result<List<ContractInvation>>(ResultCode.Ok, "OK", null);
            return result;
        }

        #endregion

        #region user
        public async Task<Result<User>> GetUserInfo(string username)
        {
            var result = Result<User>.ErrorMessage("BadRequest");
            var user = Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                result = new Result<User>(ResultCode.Ok, "OK", user);
            }
            return result;
        }

        public async Task<Result<List<User>>> SearchUser(SearchType type, string value)
        {
            var result = new Result<List<User>>(ResultCode.Ok, "OK", Users);
            return result;
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
        public async Task<Result<byte[]>> GetAvator(string username)
        {
            var result = new Result<byte[]>(ResultCode.Ok, "OK", _avatarDict[username]);
            return result;
        }

        public async Task<User> QueryUser(string username)
        {
            return (await GetUserInfo(username)).Data;
        }
        #endregion

        #region group
        public async Task<Result> CreateGroup(string groupName)
        {
            var result = Result.BadRequest;
            if (Groups.FirstOrDefault(g => g.Name == groupName) != null)
            {
                Groups.Add(new Group(GenGroupId(), groupName, CurrentAccount));
                result = Result.OKRequest;
            }
            return result;
        }
        public async Task<Result> DeleteGroup(long groupId)
        {
            var result = Result.BadRequest;
            var groups = Groups.FirstOrDefault(g => g.Id == groupId);
            if (groups != null)
            {
                Groups.Remove(groups);
                result = Result.OKRequest;
            }
            return result;
        }
        public async Task<Result<Group>> GetGroup(long groupId)
        {
            var result = new Result<Group>(ResultCode.Bad, "BadRequest");
            var group = Groups.FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                result.Data = await Task.Run(() => group);
            }
            return result;
        }
        public async Task<Result<List<Group>>> GetGroups()
        {
            var result = new Result<List<Group>>(ResultCode.Ok, "OK", JoinedGroups);
            return result;
        }
        public async Task<Result> JoinGroup(long groupId)
        {
            var result = Result.BadRequest;
            var group = Groups.FirstOrDefault(g => g.Id == groupId);
            var member = group.Members.FirstOrDefault(m => m.Username == CurrentAccount.Username);
            if (group != null && member == null)
            {
                group.Members.Add(CurrentAccount);
                result = new Result(ResultCode.Ok, "OK", null);
            }
            return result;
        }
        public async Task<Result> RenameGroup(long groupId, string newName)
        {
            var result = Result.BadRequest;
            var oldGroup = Groups.FirstOrDefault(g => g.Id == groupId);
            var newGroup = Groups.FirstOrDefault(g => g.Name == newName);
            if (oldGroup != null && newGroup == null)
            {
                oldGroup.Name = newName;
                result = Result.OKRequest;
            }
            return result;
        }
        public async Task<Result> LeaveGroup(long groupId)
        {
            var result = Result.BadRequest;
            var group = JoinedGroups.FirstOrDefault(g => g.Id == groupId);
            var member = group.Members.FirstOrDefault(u => u.Username == CurrentAccount.Username);
            if (group != null && member != null)
            {
                JoinedGroups.Remove(group);
                group.Members.Remove(member);
                result = Result.OKRequest;
            }
            return result;
        }
        public async Task<Result> AddGroupMember(string groupName, string userName)
        {
            var result = Result.BadRequest;
            var group = Groups.FirstOrDefault(g => g.Name == groupName);
            var user = Users.FirstOrDefault(u => u.Username == userName);
            if (group != null && user != null)
            {
                if (!group.Members.Any(u => u.Username == userName))
                {
                    group.Members.Add(user);
                    result = Result.OKRequest;
                }
            }
            return result;
        }
        public async Task<Result<List<Group>>> SearchGroups(string name)
        {
            var result = new Result<List<Group>>(ResultCode.Ok, "OK", Groups);
            return result;
        }

        public async Task<Group> QueryGroup(long groupId)
        {
            return (await GetGroup(groupId)).Data;
        }
        #endregion



        #region chat
        public async Task<Result> SendGroupMessage(long groupId, string content)
        {
            var result = Result.BadRequest;
            var group = JoinedGroups.FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                var chat = Chats.OfType<GroupChat>().FirstOrDefault(c => c.Group.Id == groupId);
                if (chat == null)
                {
                    chat = new GroupChat(group);
                    Chats.Add(chat);
                }
                var m0 = new GroupMessage(GenMessageId(), DateTime.Now, CurrentAccount, group, content);
                chat.AddMessage(m0);
                GroupChatMessageReceived?.Invoke(this, m0);

                int i = 0;
                var r = group.Members.Where(u => u.Username != CurrentAccount.Username);
                foreach (var m in r)
                {
                    var msg = new GroupMessage(GenMessageId(), DateTime.Now, m, group, $"{content} + {++i}");
                    chat.AddMessage(msg);
                    GroupChatMessageReceived?.Invoke(this, msg);
                }
            }
            return result;
        }

        public async Task<Result> SendMessage(string username, string content)
        {
            var result = Result.BadRequest;
            var contract = Contracts.FirstOrDefault(c => c.User.Username == username);
            if (contract != null)
            {
                var chat = Chats.OfType<PrivateChat>().FirstOrDefault(c => c.Contract.User.Username == username);
                if (chat == null)
                {
                    chat = new PrivateChat(contract);
                    Chats.Add(chat);
                }
                var m0 = new PrivateMessage(GenMessageId(), DateTime.Now, CurrentAccount, contract.User, content);
                chat.AddMessage(m0);
                PrivateChatMessageReceived?.Invoke(this, m0);

                var m1 = new PrivateMessage(GenMessageId(), DateTime.Now, contract.User, CurrentAccount, $"I know you said: {content}");
                chat.AddMessage(m1);
                PrivateChatMessageReceived?.Invoke(this, m1);
            }
            return result;
        }
        public async Task<Result<List<PrivateChat>>> GetPrivateChatHistory()
        {
            string json = JsonConvert.SerializeObject(Chats.OfType<PrivateChat>());
            var chats = JsonConvert.DeserializeObject<List<PrivateChat>>(json);
            var result = new Result<List<PrivateChat>>(ResultCode.Ok, "OK", chats);
            return result;
        }

        public async Task<Result<List<GroupChat>>> GetGroupChatHistory()
        {
            string json = JsonConvert.SerializeObject(Chats.OfType<GroupChat>());
            var chats = JsonConvert.DeserializeObject<List<GroupChat>>(json);
            var result = new Result<List<GroupChat>>(ResultCode.Ok, "OK", chats);
            return result;
        }
        public async Task<Result<List<Chat>>> GetChatHistory()
        {
            var chats = new List<Chat>();

            string json = JsonConvert.SerializeObject(Chats.OfType<PrivateChat>());
            var privateChats = JsonConvert.DeserializeObject<List<PrivateChat>>(json);
            chats.AddRange(privateChats);

            json = JsonConvert.SerializeObject(Chats.OfType<GroupChat>());
            var groupChats = JsonConvert.DeserializeObject<List<GroupChat>>(json);
            chats.AddRange(groupChats);

            var result = new Result<List<Chat>>(ResultCode.Ok, "OK", chats);
            return result;
        }
        #endregion

        public void Connect(IPAddress ipAddress, int port)
        {
            return;
        }

        public DemoChatService()
        {
            SetUpUsers();
            SetUpGroups();

            SetUpAccount();
            SetUpContracts();
            SetUpJoinedGroups();
            SetUpChats();
        }

        List<User> Users = new List<User>();
        List<Group> Groups = new List<Group>();

        Account CurrentAccount;
        List<Contract> Contracts = new List<Contract>();
        List<Group> JoinedGroups = new List<Group>();
        List<Chat> Chats = new List<Chat>();

        List<ContractInvation> contractInvations = new List<ContractInvation>();

        Dictionary<string, byte[]> _avatarDict = new Dictionary<string, byte[]>();

        #region set sample data

        static byte[] LoadResource(string resourcePath)
        {
            var info = Application.GetResourceStream(new Uri(resourcePath));
            return ReadFully(info.Stream);
        }

        void SetUpUsers()
        {
            Users.AddRange(new[]
            {
                new User()
                {
                    Id = GenUserId(),
                    Email = "jack@flowchat.com",
                    Username = "jack",
                    Nickname = "Jack",
                    Gender = Gender.Unknown,
                    Status = "Hello World!",
                },
                new User() {
                    Id = GenUserId(),
                    Email = "mei@flowchat.com",
                    Username = "mei",
                    Nickname = "Mei",
                    Gender = Gender.Girl,
                    Status = "Have Fun Coding!",
                    Phone = "1234567890",
                    Region = "China Guangzhou"
                },
                new User()
                {
                    Id = GenUserId(),
                    Email = "jimmy@flowchat.com",
                    Username = "jimmy",
                    Nickname = "jimmy",
                    Gender = Gender.Boy,
                    Status = "No Errors, No Bugs!"
                },
                new User()
                {
                    Id = GenUserId(),
                    Email = "test@flowchat.com",
                    Username = "test",
                    Nickname = "test",
                    Gender = Gender.Boy,
                    Status = "test!"
                }
            });

            _avatarDict[Users[0].Username] = LoadResource(User.DefaultBoyAvatar);
            _avatarDict[Users[1].Username] = LoadResource(User.DefaultAvatar);
            _avatarDict[Users[2].Username] = LoadResource(User.DefaultGirlAvatar);
            _avatarDict[Users[3].Username] = LoadResource(User.DefaultGirlAvatar);
        }

        void SetUpGroups()
        {
            var group0 = new Group()
            {
                Name = "Group Zero",
                Id = GenGroupId(),
                Owner = Users[0]
            };
            group0.Members.Add(Users[0]);
            group0.Members.Add(Users[1]);
            group0.Members.Add(Users[2]);

            var group1 = new Group()
            {
                Name = "Group One",
                Id = GenGroupId(),
                Owner = Users[1]
            };

            group1.Members.Add(Users[0]);
            group1.Members.Add(Users[1]);

            var group2 = new Group()
            {
                Name = "Group Two",
                Id = GenGroupId(),
                Owner = Users[2]
            };

            group2.Members.Add(Users[0]);
            group2.Members.Add(Users[1]);
            group2.Members.Add(Users[2]);
            group2.Members.Add(Users[3]);

            Groups.Add(group0);
            Groups.Add(group1);
            Groups.Add(group2);
        }
        void SetUpAccount()
        {
            CurrentAccount = new Account(Users[0]);
        }
        void SetUpContracts()
        {
            Contracts.AddRange(new[] {
                new Contract(Users[1], "Han Mei"),
                new Contract(Users[2])
            });
        }
        void SetUpJoinedGroups()
        {
            JoinedGroups.AddRange(Groups);
        }
        void SetUpChats()
        {
            var privateChat = new PrivateChat(Contracts[0]);
            Array.ForEach(new[]
            {
                new PrivateMessage(GenMessageId(), DateTime.Now, Users[0], Users[1], "Hello!"),
                new PrivateMessage(GenMessageId(), DateTime.Now, Users[1], Users[0], "Hijack~"),
            }, (e) => privateChat.Messages.Add(e));

            var groupChat = new GroupChat(Groups[0]);
            Array.ForEach(new[]
            {
                new GroupMessage(GenMessageId(), DateTime.Now, Users[0], Groups[0], "Hello!"),
                new GroupMessage(GenMessageId(), DateTime.Now, Users[1], Groups[0], "Hey!~"),
                new GroupMessage(GenMessageId(), DateTime.Now, Users[2], Groups[0], "Hey!~"),
            }, (e) => groupChat.Messages.Add(e));

            Chats.Add(privateChat);
            Chats.Add(groupChat);
        }
        void SetUpContractInvations()
        {
            contractInvations.AddRange(new[] {
                new ContractInvation(GenContractInvationId(), Users[3].Username, "HeyHey!")
            });
        }

        public Task<Result<List<PrivateChat>>> GetUnreadPrivateChatHistory()
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<GroupChat>>> GetUnreadGroupChatHistory()
        {
            throw new NotImplementedException();
        }

        public void Handle()
        {

        }


        #endregion

    }
}
