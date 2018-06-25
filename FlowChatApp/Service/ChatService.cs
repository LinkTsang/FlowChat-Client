using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FlowChatApp.Service
{
    public class ChatService : IChatService
    {
        Dictionary<string, User> _userDict = new Dictionary<string, User>();
        Dictionary<long, Group> _groupDict = new Dictionary<long, Group>();
        Account _account;

        public async Task<User> QueryUser(string username)
        {
            return (await GetUserInfo(username)).Data;
        }


        public class TokenClass
        {
            public string Token { get; set; }
        }
        readonly HttpClient _httpClient;
        readonly TcpClient _tcpClient;
        readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        string _token = string.Empty;

        readonly CancellationTokenSource _cts = new CancellationTokenSource();
        readonly CancellationToken _cancellationToken;

        static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = ContractResolver,
            Formatting = Formatting.Indented
        };

        readonly JsonLoadSettings JsonLoadSettings = new JsonLoadSettings
        {
        };
        public ChatService(string baseUri)
        {
            _cancellationToken = _cts.Token;

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUri)
            };
            _tcpClient = new TcpClient();

            // _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            // _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
        }

        public void Handle()
        {
            _dispatcherTimer.Start();
        }

        async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var unreadPrivateMessageResult = await GetUnreadPrivateChatHistory();
            if (unreadPrivateMessageResult.Ok)
            {
                unreadPrivateMessageResult.Data.ForEach(c =>
                {
                    foreach (var chatMessage in c.Messages)
                    {
                        PrivateChatMessageReceived?.Invoke(this, chatMessage);
                    }
                });
            }
            var unreadGroupMessageResult = await GetUnreadGroupChatHistory();
            if (unreadGroupMessageResult.Ok)
            {
                unreadGroupMessageResult.Data.ForEach(c =>
                {
                    foreach (var chatMessage in c.Messages)
                    {
                        GroupChatMessageReceived?.Invoke(this, chatMessage);
                    }
                });
            }
            var getContractInvationsResult = await GetContractInvations();
            if (getContractInvationsResult.Ok && getContractInvationsResult.Data.Count != 0)
            {
                ContactRequestMessagesUpdated?.Invoke(this, getContractInvationsResult.Data);
            }

            var getInvationConfirmationsResult = await GetInvationConfirmations();
            if (getInvationConfirmationsResult.Ok)
            {
                ContactConfirmationMessageReceived?.Invoke(this, getInvationConfirmationsResult.Data);
            }
        }

        void ChatServiceLoop()
        {


        }
        async Task<Result> GetRequestImplAsync(string requestUri)
        {
            Result result = Result.BadRequest;
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                requestMessage.Headers.Add("token", _token);
                var response = await _httpClient.SendAsync(requestMessage, _cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine(responseContent);
                }
                else if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var s = await response.Content.ReadAsStringAsync();
                    var r = JObject.Parse(s);
                    var code = r["code"].Value<int>();
                    var message = r["msg"].Value<string>();
                    var data = r["data"];
                    result = new Result((ResultCode)code, message, data);
                }
            }
            catch (HttpRequestException e)
            {
                Trace.WriteLine(e.Message);
                result = new Result(ResultCode.Bad, e.Message, null);
            }
            return result;
        }
        async Task<Result<T>> GetRequestAsync<T>(string requestUri) where T : class
        {
            var response = await GetRequestImplAsync(requestUri);
            if (response.HasError)
            {
                BadRequestRaised?.Invoke(this, response);
            }
            var result = new Result<T>(response.Code, response.Message);
            if (response.Data != null)
            {
                result.Data = response.Data.ToObject<T>();
            }
            return result;
        }
        async Task<Result> GetRequestAsync(string requestUri)
        {
            var result = await GetRequestImplAsync(requestUri);
            if (result.HasError)
            {
                BadRequestRaised?.Invoke(this, result);
            }
            return result;
        }

        async Task<Result> PostRequestImplAsync(string requestUri, JObject jObject)
        {
            var content = new StringContent(jObject == null ? string.Empty : jObject.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("token", _token);
            Result result = Result.BadRequest;
            try
            {
                var response = await _httpClient.PostAsync(requestUri, content, _cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine(responseContent);
                }
                else if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var s = await response.Content.ReadAsStringAsync();
                    var r = JObject.Parse(s);
                    var code = r["code"].Value<int>();
                    var message = r["msg"].Value<string>();
                    var data = r["data"];
                    result = new Result((ResultCode)code, message, data);
                }
            }
            catch (HttpRequestException e)
            {
                Trace.WriteLine(e.Message);
                result = new Result(ResultCode.Bad, e.Message, null);
            }
            return result;
        }
        async Task<Result> PostRequestAsync(string requestUri, JObject jObject = null)
        {
            var result = await PostRequestImplAsync(requestUri, jObject);
            if (result.HasError)
            {
                BadRequestRaised?.Invoke(this, result);
            }
            return result;
        }

        async Task<Result<T>> PostRequestAsync<T>(string requestUri, JObject jObject = null) where T : class
        {
            var response = await PostRequestImplAsync(requestUri, jObject);
            if (response.HasError)
            {
                BadRequestRaised?.Invoke(this, response);
            }
            var result = new Result<T>(response.Code, response.Message);
            if (response.Data != null)
            {
                result.Data = response.Data.ToObject<T>();
            }
            return result;
        }

        User MergeUserInfo(User user)
        {
            if (user == null)
            {
                return null;
            }
            if (_userDict.ContainsKey(user.Username))
            {
                _userDict[user.Username].MergeFrom(user);
            }
            else
            {
                _userDict[user.Username] = user;
            }
            return _userDict[user.Username];
        }

        Group MergeGroupInfo(Group group)
        {
            if (_groupDict.ContainsKey(group.Id))
            {
                var g = _groupDict[group.Id];
                g.Name = group.Name;
                g.Owner = MergeUserInfo(group.Owner);
                g.Members.Clear();
                foreach (var m in group.Members)
                {
                    g.Members.Add(MergeUserInfo(m));
                }
            }
            else
            {
                for (int i = 0; i < group.Members.Count; i++)
                {
                    User m = group.Members[i];
                    group.Members[i] = MergeUserInfo(m);
                }
                _groupDict[group.Id] = group;
            }
            return _groupDict[group.Id];
        }
        Account MergeAccountInfo(Account account)
        {
            MergeUserInfo(account);
            if (_account == null)
            {
                _account = account;
            }
            else
            {
                _account.MergeFrom(account);
            }
            return _account;
        }
        public event EventHandler<PrivateMessage> PrivateChatMessageReceived;
        public event EventHandler<GroupMessage> GroupChatMessageReceived;
        public event EventHandler<List<ContractInvation>> ContactRequestMessagesUpdated;
        public event EventHandler<List<ContractInvation>> ContactConfirmationMessageReceived;
        public event EventHandler<Result> BadRequestRaised;

        #region account
        public async Task<Result> SignUpAsync(string email, string username, string nickname, string password)
        {
            var jObject = new JObject
            {
                ["email"] = email,
                ["username"] = username,
                ["nickname"] = nickname,
                ["password"] = password,
            };
            return await PostRequestAsync("/api/service/register", jObject);
        }
        public async Task<Result<TokenClass>> SignInAsync(string username, string password)
        {
            var jObject = new JObject
            {
                ["username"] = username,
                ["password"] = password,
            };
            var result = await PostRequestAsync<TokenClass>("/api/service/login", jObject);
            if (result.Ok)
            {
                _token = result.Data.Token;
            }
            return result;
        }
        public async Task<Result> SignOutAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<TokenClass>> UpdateToken()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<Account>> UpdateAccountInfo(Account account)
        {
            var result = await PostRequestAsync<Account>("/api/auth/modifyAccountInfo", JObject.FromObject(account));
            if (result.Ok && result.Data != null)
            {
                // FIXME
                result.Data.Username = account.Username;
                result.Data = MergeAccountInfo(result.Data);
            }
            return result;
        }
        public async Task<Result<Account>> GetAccountInfo()
        {
            var result = await PostRequestAsync<Account>("/api/auth/searchAccoutInfo");
            var account = result.Data;
            if (result.Ok)
            {
                result.Data = MergeAccountInfo(result.Data);
            }
            result.Data = _account;
            return result;
        }
        public async Task<Result> UploadAvator(string filename, byte[] avator)
        {
            var content = new ByteArrayContent(avator);
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Headers.Add("token", _token);
            multipartContent.Add(content, "file", filename);
            try
            {
                var response = await _httpClient.PostAsync("/api/auth/uploadImage", multipartContent, _cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine(responseContent);
                }
                else if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var s = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(s);
                    var code = result["code"].Value<int>();
                    var message = result["msg"].Value<string>();
                    var data = result["data"] as JObject;
                    return new Result((ResultCode)code, message, data);
                }
            }
            catch (SocketException e)
            {
                Trace.WriteLine(e);
                return new Result(ResultCode.Bad, e.Message, null);
            }

            return Result.BadRequest;
        }
        #endregion

        #region contract
        public async Task<Result<List<Contract>>> GetContracts()
        {
            var result = await PostRequestAsync("/api/auth/searchAllCategorys");
            if (result.HasError)
            {
                return new Result<List<Contract>>(result.Code, result.Message);
            }

            if (!(result.Data is JArray catetories))
            {
                return Result<List<Contract>>.ErrorMessage("Error");
            }

            var list = new List<Contract>();
            foreach (var cateory in catetories)
            {
                if (!(cateory is JObject c))
                {
                    return Result<List<Contract>>.ErrorMessage("Error");
                }
                var catetoryName = c["catetoryName"].Value<string>();
                if (!(c["categoryMemberInfos"] is JArray categoryMemberInfos))
                {
                    return Result<List<Contract>>.ErrorMessage("Error");
                }
                foreach (var member in categoryMemberInfos)
                {
                    var user = MergeUserInfo(member.ToObject<User>());
                    var contract = new Contract(user, "", catetoryName);
                    list.Add(contract);
                }
            }
            return new Result<List<Contract>>(ResultCode.Ok, "OK", list);
        }
        public async Task<Result> AddContact(string username, string categoryName, string message)
        {
            var jObject = new JObject()
            {
                ["username"] = username,
                ["categoryName"] = categoryName,
                ["message"] = message,
            };
            return await PostRequestAsync("/api/auth/addFriend", jObject);
        }
        public async Task<Result> DeleteContact(string friendName)
        {
            return await GetRequestAsync($"/api/auth/deleteFriend?friendName={friendName}");
        }
        public async Task<Result> UpdateContact(string username, string alias)
        {
            return await GetRequestAsync($"/api/auth/modifyAliaName?aliaName={alias}&friendName={username}");
        }
        public async Task<Result<List<ContractInvation>>> GetContractInvations()
        {
            var result = await PostRequestAsync<List<ContractInvation>>("/api/auth/searchcontactinvation");
            if (result.Ok)
            {
                foreach (var c in result.Data)
                {
                    c.User = await QueryUser(c.FriendName);
                }
            }
            return result;
        }
        public async Task<Result> ConfirmContractInvation(long recordId, string categoryName, bool accept)
        {
            int type = accept ? 0 : 1;
            var result = await GetRequestAsync($"/api/auth/confirmcontactinvation" +
                $"?recordId={recordId}" +
                $"&categoryName={categoryName}" +
                $"&type={type}");
            return result;
        }
        public async Task<Result<List<ContractInvation>>> GetInvationConfirmations()
        {
            var result = await PostRequestAsync<List<ContractInvation>>("/api/auth/searchContactInvationToOthers");
            if (result.Ok)
            {
                foreach (var c in result.Data)
                {
                    c.User = await QueryUser(c.FriendName);
                }
            }
            return result;
        }

        #endregion

        #region user
        public async Task<Result<User>> GetUserInfo(string username)
        {
            var result = await GetRequestAsync<User>($"/api/auth/searchUserInfo?username={username}");
            if (result.Ok)
            {
                result.Data = MergeUserInfo(result.Data);
            }
            return result;
        }
        public async Task<Result<List<User>>> SearchUser(SearchType type, string value)
        {
            var result = await GetRequestAsync<List<User>>($"/api/auth/findUser?username={value}");
            return result;
        }
        public async Task<Result<byte[]>> GetAvator(string username)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/auth/downloadImage?username={username}");
                requestMessage.Headers.Add("token", _token);
                var response = await _httpClient.SendAsync(requestMessage, _cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine(responseContent);
                }
                else
                {
                    var vvv = await response.Content.ReadAsStringAsync();
                    var content = await response.Content.ReadAsByteArrayAsync();
                    if (content.Length == 0)
                    {
                        content = null;
                    }
                    return new Result<byte[]>(ResultCode.Ok, "OK", content);
                }
            }
            catch (SocketException e)
            {
                Trace.WriteLine(e);
                return Result<byte[]>.ErrorMessage(e.Message);
            }

            return Result<byte[]>.ErrorMessage("Failed to get the avator");
        }
        #endregion

        #region group
        public async Task<Result<List<Group>>> GetGroups()
        {
            var result = await GetRequestAsync("/api/auth/searchAllGroups");
            if (result.HasError)
            {
                return new Result<List<Group>>(result.Code, result.Message);
            }

            if (!(result.Data is JArray groups_))
            {
                return Result<List<Group>>.ErrorMessage("Error");
            }

            var list = new List<Group>();
            foreach (var group_ in groups_)
            {
                if (!(group_ is JObject group))
                {
                    return Result<List<Group>>.ErrorMessage("Error");
                }
                var g = new Group
                {
                    Name = group["groupName"].Value<string>(),
                    Id = group["groupId"].Value<long>()
                };
                if (!(group["groupMemberInfos"] is JArray groupMemberInfos))
                {
                    return Result<List<Group>>.ErrorMessage("Error");
                }
                foreach (var member in groupMemberInfos)
                {
                    var user = member.ToObject<User>();
                    g.Members.Add(user);
                }
                list.Add(MergeGroupInfo(g));
            }
            return new Result<List<Group>>(ResultCode.Ok, "OK", list);
        }
        public async Task<Result> JoinGroup(long groupId)
        {
            return await GetRequestAsync($"/api/auth/joinGroup?groupId={groupId}");
        }
        public async Task<Result> CreateGroup(string groupName)
        {
            return await GetRequestAsync($"/api/auth/addGroup?groupName={groupName}");
        }
        public async Task<Result> LeaveGroup(long groupId)
        {
            return await PostRequestAsync($"/api/auth/leaveGroup?groupId={groupId}");
        }
        public async Task<Result<Group>> GetGroup(long groupId)
        {
            var result = await GetRequestAsync($"/api/auth/searchGroup?groupId={groupId}");
            if (result.HasError)
            {
                return new Result<Group>(result.Code, result.Message);
            }

            if (!(result.Data is JObject group))
            {
                return Result<Group>.ErrorMessage("Error");
            }

            var g = new Group
            {
                Name = group["groupName"].Value<string>(),
                Id = groupId
            };
            if (!(group["groupMemberInfos"] is JArray groupMemberInfos))
            {
                return Result<Group>.ErrorMessage("Error");
            }
            foreach (var member in groupMemberInfos)
            {
                var user = member.ToObject<User>();
                g.Members.Add(user);
            }
            g = MergeGroupInfo(g);
            return new Result<Group>(result.Code, result.Message, g);

        }
        public async Task<Result> AddGroupMember(string groupName, string userName)
        {
            return await GetRequestAsync($"/api/auth/addGroupMember?groupName={groupName}&userName={userName}");
        }
        public async Task<Result> DeleteGroup(long groupId)
        {
            return await PostRequestAsync($"/api/auth/deleteGroup?groupId={groupId}");
        }
        public async Task<Result> RenameGroup(long groupId, string newName)
        {
            return await GetRequestAsync($"/api/auth/modifyGroup?groupId={groupId}&newName={newName}");
        }
        public async Task<Result<List<Group>>> SearchGroups(string name)
        {
            var result = await GetRequestAsync($"/api/auth/searchGroups?groupName={name}");
            if (result.HasError)
            {
                return new Result<List<Group>>(result.Code, result.Message);
            }

            if (!(result.Data is JArray groups))
            {
                return Result<List<Group>>.ErrorMessage("Error");
            }

            var list = new List<Group>();
            foreach (var cateory in groups)
            {
                if (!(cateory is JObject c))
                {
                    return Result<List<Group>>.ErrorMessage("Error");
                }
                var g = new Group { Name = c["groupName"].Value<string>() };
                if (!(c["groupMemberInfos"] is JArray groupMemberInfos))
                {
                    return Result<List<Group>>.ErrorMessage("Error");
                }
                foreach (var member in groupMemberInfos)
                {
                    var user = member.ToObject<User>();
                    g.Members.Add(user);
                }
                g = MergeGroupInfo(g);
                list.Add(g);
            }
            return new Result<List<Group>>(ResultCode.Ok, "OK", list);
        }
        public async Task<Group> QueryGroup(long groupId)
        {
            if (!_groupDict.ContainsKey(groupId))
            {
                _groupDict.Add(groupId, (await GetGroup(groupId)).Data);
            }
            return _groupDict[groupId];
        }
        #endregion

        #region chat
        public async Task<Result> SendMessage(string username, string content)
        {
            var jObject = new JObject
            {
                ["userName"] = username,
                ["content"] = content
            };
            return await PostRequestAsync("/api/auth/sendPrivateMessage", jObject);
        }
        public async Task<Result<List<PrivateChat>>> GetPrivateChatHistory()
        {
            var result = new Result<List<PrivateChat>>(ResultCode.Ok, "OK");
            var chats = new List<PrivateChat>();
            result.Data = chats;
            var response = await GetRequestAsync("/api/auth/searchChatMessages");
            if (response.Ok)
            {
                var chats_ = response.Data as JObject;
                foreach (var chat_ in chats_)
                {
                    var messages = chat_.Value.ToObject<List<PrivateMessage>>();
                    if (messages.Count == 0)
                    {
                        continue;
                    }
                    var peerName = chat_.Key;
                    var peer = await QueryUser(peerName);
                    var contract = new Contract(peer);
                    var chat = new PrivateChat(contract);
                    foreach (var m in messages)
                    {
                        m.Sender = await QueryUser(m.SenderName);
                        m.Receiver = await QueryUser(m.ReceiverName);
                        chat.Messages.Add(m);
                    }
                    chats.Add(chat);
                }
            }
            return result;
        }
        public async Task<Result<List<PrivateChat>>> GetUnreadPrivateChatHistory()
        {
            var result = new Result<List<PrivateChat>>(ResultCode.Ok, "OK");
            var chats = new List<PrivateChat>();
            result.Data = chats;
            var response = await GetRequestAsync("/api/auth/searchUnreadChatMessages");
            if (response.Ok)
            {
                var chats_ = response.Data as JObject;
                foreach (var chat_ in chats_)
                {
                    var messages = chat_.Value.ToObject<List<PrivateMessage>>();
                    if (messages.Count == 0)
                    {
                        continue;
                    }
                    var peerName = chat_.Key;
                    var peer = await QueryUser(peerName);
                    var contract = new Contract(peer);
                    var chat = new PrivateChat(contract);
                    foreach (var m in messages)
                    {
                        m.Sender = await QueryUser(m.SenderName);
                        m.Receiver = await QueryUser(m.ReceiverName);
                        chat.Messages.Add(m);
                    }
                    chats.Add(chat);
                }
            }
            return result;
        }
        public async Task<Result> SendGroupMessage(long groupId, string content)
        {
            var jObject = new JObject
            {
                ["groupId"] = groupId,
                ["content"] = content,
                ["type"] = 0
            };
            return await PostRequestAsync("/api/auth/sendGroupMessage", jObject);
        }
        public async Task<Result<List<GroupChat>>> GetGroupChatHistory()
        {
            var result = new Result<List<GroupChat>>(ResultCode.Ok, "OK");
            var chats = new List<GroupChat>();
            result.Data = chats;
            var response = await GetRequestAsync("/api/auth/searchGroupMessages");
            if (response.Ok)
            {
                var chats_ = response.Data as JArray;
                foreach (var chat_ in chats_)
                {
                    var messages = chat_["messages"].ToObject<List<GroupMessage>>();
                    if (messages.Count == 0)
                    {
                        continue;
                    }
                    var groupId = chat_["groupId"].ToObject<int>();
                    var group = (await GetGroup(groupId)).Data;
                    var chat = new GroupChat(group);
                    foreach (var m in messages)
                    {
                        m.Sender = await QueryUser(m.SenderName);
                        m.Group = group;
                        chat.Messages.Add(m);
                    }
                    chats.Add(chat);
                }
            }
            return result;
        }
        public async Task<Result<List<GroupChat>>> GetUnreadGroupChatHistory()
        {
            var result = new Result<List<GroupChat>>(ResultCode.Ok, "OK");
            var chats = new List<GroupChat>();
            result.Data = chats;
            var response = await GetRequestAsync("/api/auth/searchUnreadGroupMessages");
            if (response.Ok)
            {
                var chats_ = response.Data as JArray;
                foreach (var chat_ in chats_)
                {
                    var messages = chat_["messages"].ToObject<List<GroupMessage>>();
                    if (messages.Count == 0)
                    {
                        continue;
                    }
                    var groupId = chat_["groupId"].ToObject<int>();
                    var group = (await GetGroup(groupId)).Data;
                    var chat = new GroupChat(group);
                    foreach (var m in messages)
                    {
                        m.Sender = await QueryUser(m.SenderName);
                        m.Group = group;
                        chat.Messages.Add(m);
                    }
                    chats.Add(chat);
                }
            }
            return result;
        }
        public async Task<Result<List<Chat>>> GetChatHistory()
        {
            var chats = new List<Chat>();

            var privateChats = (await GetPrivateChatHistory()).Data;
            chats.AddRange(privateChats);

            var groupChats = (await GetGroupChatHistory()).Data;
            chats.AddRange(groupChats);

            var result = new Result<List<Chat>>(ResultCode.Ok, "OK", chats);
            return result;
        }
        #endregion
    }
}
