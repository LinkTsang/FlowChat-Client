using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Service
{
    public class DemoChatService : IChatService
    {
        long _groupId;
        long _userId;
        long _messageId;
        long _contractInvationId;
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

        public event EventHandler<ChatMessage> ChatMessageReceived;
        public event EventHandler<ContractInvation> ContactRequsetMessageReceived;
        public event EventHandler<Result> BadRequestRaised;

        public async Task<Result> AddContact(string username, string categoryName, string message)
        {
            var result = Result.BadRequest;
            if (Contracts.FirstOrDefault(c => c.User.UserName == username) == null)
            {
                var user = Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    return Result.OKRequest;
                }
            }
            return result;
        }

        public async Task<Result> AddGroup(string groupName)
        {
            var result = Result.BadRequest;
            if (Groups.FirstOrDefault(g => g.Name == groupName) != null)
            {
                Groups.Add(new Group(GenGroupId(), groupName, CurrentAccount));
                result = Result.OKRequest;
            }
            return result;
        }

        public async Task<Result> AddGroupMember(string groupName, string userName)
        {
            var result = Result.BadRequest;
            var group = Groups.FirstOrDefault(g => g.Name == groupName);
            var user = Users.FirstOrDefault(u => u.UserName == userName);
            if (group != null && user != null)
            {
                if (!group.Members.Any(u => u.UserName == userName))
                {
                    group.Members.Add(user);
                    result = Result.OKRequest;
                }
            }
            return result;
        }

        public async Task<Result> BlockContact(string id)
        {
            var result = Result.BadRequest;
            return result;
        }

        public Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName)
        {
            return null;
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            return;
        }

        public async Task<Result> DeleteContact(long id)
        {
            var result = Result.BadRequest;
            var contract = Contracts.FirstOrDefault(c => c.User.Id == id);
            if (contract != null)
            {
                Contracts.Remove(contract);
                result = Result.OKRequest;
            }
            return result;
        }

        public async Task<Result> DeleteGroup(string groupName)
        {
            var result = Result.BadRequest;
            var groups = Groups.FirstOrDefault(g => g.Name == groupName);
            if (groups != null)
            {
                Groups.Remove(groups);
                result = Result.OKRequest;
            }
            return result;
        }

        public async Task<Result<Account>> GetAccountInfo()
        {
            Result<Account> result = new Result<Account>(ResultCode.Ok, "OK", CurrentAccount);
            return result;
        }

        public async Task<Result<byte[]>> GetAvator()
        {
            var result = new Result<byte[]>(ResultCode.Ok, "OK");
            return result;
        }

        public async Task<Result<List<string>>> GetBlocked()
        {
            var result = new Result<List<string>>(ResultCode.Ok, "OK");
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
        public async Task<Result<List<Contract>>> GetContacts()
        {
            var result = new Result<List<Contract>>(ResultCode.Ok, "OK", Contracts);
            return result;
        }

        public async Task<Result> GetContactStatus(params string[] ids)
        {
            var result = Result.BadRequest;
            return result;
        }

        public async Task<Result<List<ContractInvation>>> GetContractInvation()
        {
            var result = new Result<List<ContractInvation>>(ResultCode.Ok, "OK", contractInvations);
            return result;
        }

        public async Task<Result> RenameGroup(string oldName, string newName)
        {
            var result = Result.BadRequest;
            var oldGroup = Groups.FirstOrDefault(g => g.Name == oldName);
            var newGroup = Groups.FirstOrDefault(g => g.Name == newName);
            if (oldGroup != null && newGroup == null)
            {
                oldGroup.Name = newName;
            }
            return result;
        }

        public async Task<Result<List<Group>>> GetGroups()
        {
            var result = new Result<List<Group>>(ResultCode.Ok, "OK", JoinedGroups);
            return result;
        }

        public async Task<Result<List<Group>>> SearchGroup(string groupName)
        {
            var result = new Result<List<Group>>(ResultCode.Ok, "OK", JoinedGroups);
            return result;
        }
        public async Task<Result> JoinGroup(long groupId)
        {
            var result = new Result(ResultCode.Ok, "OK", null);
            return result;
        }
        public async Task<Result<List<User>>> SearchUser(SearchType type, string value)
        {
            var result = new Result<List<User>>(ResultCode.Ok, "OK", Users);
            return result;
        }

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
                var message = new GroupMessage(GenMessageId(), DateTime.Now, CurrentAccount, group, content);
                chat.AddMessage(message);
                ChatMessageReceived?.Invoke(this, message);
            }
            return result;
        }

        public async Task<Result> SendMessage(string username, string content)
        {
            var result = Result.BadRequest;
            var contract = Contracts.FirstOrDefault(c => c.User.UserName == username);
            if (contract != null)
            {
                var chat = Chats.OfType<PrivateChat>().FirstOrDefault(c => c.Contract.User.UserName == username);
                if (chat == null)
                {
                    chat = new PrivateChat(contract);
                    Chats.Add(chat);
                }
                var message = new PrivateMessage(GenMessageId(), DateTime.Now, CurrentAccount, contract.User, content);
                chat.AddMessage(message);
                ChatMessageReceived?.Invoke(this, message);
            }
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

        public async Task<Result> SignUpAsync(string email, string username, string nickname, string password)
        {
            var result = Result.BadRequest;
            return result;
        }

        public async Task<Result> UnblockContact(string id)
        {
            var result = Result.BadRequest;
            return result;
        }

        public async Task<Result<Account>> UpdateAccountInfo(Account account)
        {
            CurrentAccount.MergeFrom(account);
            var result = new Result<Account>(ResultCode.Ok, "OK", CurrentAccount);
            return result;
        }

        public async Task<Result> UploadAvator(string filename, byte[] avator)
        {
            var result = Result.BadRequest;
            return result;
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

        void SetUpUsers()
        {
            Users.AddRange(new[]
            {
                new User()
                {
                    Id = GenUserId(),
                    Email = "jack@flowchat.com",
                    UserName = "jack",
                    NickName = "Jack",
                    Gender = Gender.Unknown,
                    Status = "Hello World!",
                },
                new User() {
                    Id = GenUserId(),
                    Email = "mei@flowchat.com",
                    UserName = "mei",
                    NickName = "Mei",
                    Gender = Gender.Girl,
                    Status = "Have Fun Coding!",
                    Phone = "1234567890",
                    Region = "China Guangzhou"
                },
                new User()
                {
                    Id = GenUserId(),
                    Email = "jimmy@flowchat.com",
                    UserName = "jimmy",
                    NickName = "jimmy",
                    Gender = Gender.Boy,
                    Status = "No Errors, No Bugs!"
                },
                new User()
                {
                    Id = GenUserId(),
                    Email = "test@flowchat.com",
                    UserName = "test",
                    NickName = "test",
                    Gender = Gender.Boy,
                    Status = "test!"
                }
            });
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
            var privateChat = new PrivateChat(Contracts[1]);
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
                new ContractInvation(GenContractInvationId(), Users[3].UserName, "HeyHey!")
            });
        }
    }
}
