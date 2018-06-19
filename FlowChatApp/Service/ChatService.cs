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
        public class TokenClass
        {
            public string Token { get; set; }
        }
        readonly HttpClient _httpClient;
        readonly TcpClient _tcpClient;
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

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void Connect(IPAddress ipAddress, int port)
        {
            await _tcpClient.ConnectAsync(ipAddress, port);
            new Task(ChatServiceLoop, _cts.Token, TaskCreationOptions.LongRunning).Start();
        }

        void ChatServiceLoop()
        {
            using (var networkStream = _tcpClient.GetStream())
            {
                while (!_cancellationToken.IsCancellationRequested)
                {

                }
            }

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

        public event EventHandler<ChatMessage> ChatMessageReceived;
        public event EventHandler<ContractInvation> ContactRequsetMessageReceived;
        public event EventHandler<Result> BadRequestRaised;

        #region sign service
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

        public async Task<Result> SignOutAsync()
        {
            return await PostRequestAsync("/api/service/logout");
        }
        #endregion


        #region account service


        public async Task<Result<Account>> GetAccountInfo()
        {
            return await PostRequestAsync<Account>("/api/auth/searchUserInfo");
        }

        public async Task<Result<Account>> UpdateAccountInfo(Account account)
        {
            return await PostRequestAsync<Account>("/api/auth/modifyUserInfo", JObject.FromObject(account));
        }

        public async Task<Result> UploadAvator(string filename, byte[] avator)
        {
            var content = new ByteArrayContent(avator);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Headers.Add("token", _token);
            multipartContent.Add(content, "file", filename);

            try
            {
                var response = await _httpClient.PostAsync("/api/auth/uploadDownload/uploadImage", multipartContent, _cancellationToken);
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

        public async Task<Result<byte[]>> GetAvator()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/auth/uploadDownload/downloadImage", _cancellationToken);
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
                    return new Result<byte[]>((ResultCode)code, message);
                }
                else
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
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


        public async Task<Result<List<User>>> SearchUser(SearchType type, string value)
        {
            throw new NotImplementedException();
        }

        #region contact service

        public async Task<Result<List<Contract>>> GetContacts()
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
                    var user = member.ToObject<User>();
                    var contract = new Contract(user, "", catetoryName);
                    list.Add(contract);
                }
            }
            return new Result<List<Contract>>(ResultCode.Ok, "OK", list);
        }

        public async Task<Result> GetContactStatus(params string[] ids)
        {
            throw new NotImplementedException();
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

        public async Task<Result<List<ContractInvation>>> GetContractInvation()
        {
            return await PostRequestAsync<List<ContractInvation>>("/api/auth/searchcontactinvation");
        }

        public async Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName)
        {
            var jObject = new JObject()
            {
                ["recordId"] = recordId,
                ["categoryName"] = categoryName,
            };
            return await PostRequestAsync<List<ContractInvation>>("/api/auth/confirmcontactinvation", jObject);
        }

        public async Task<Result> DeleteContact(long id)
        {
            var jObject = new JObject()
            {
                ["memberId"] = id,
            };
            return await PostRequestAsync("/api/auth/deleteFriend", jObject);
        }

        public Task<Result<List<string>>> GetBlocked()
        {
            throw new NotImplementedException();
        }

        public Task<Result> BlockContact(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UnblockContact(string id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region group

        public async Task<Result> AddGroup(string groupName)
        {
            var jObject = new JObject()
            {
                ["groupName"] = groupName
            };
            return await PostRequestAsync("/api/auth/addGroup", jObject);
        }

        public async Task<Result> RenameGroup(string oldName, string newName)
        {
            var jObject = new JObject()
            {
                ["oldName"] = oldName,
                ["newName"] = newName
            };
            return await PostRequestAsync("/api/auth/modifyGroup", jObject);
        }

        public async Task<Result> AddGroupMember(string groupName, string userName)
        {
            var jObject = new JObject()
            {
                ["groupName"] = groupName,
                ["userName"] = userName
            };
            return await PostRequestAsync("/api/auth/addGroupMember", jObject);
        }

        public async Task<Result> DeleteGroup(string groupName)
        {
            var jObject = new JObject()
            {
                ["groupName"] = groupName
            };
            return await PostRequestAsync("/api/auth/deleteGroup", jObject);
        }

        public async Task<Result<List<Group>>> SearchGroup(string groupName)
        {
            var jObject = new JObject()
            {
                ["groupName"] = groupName
            };
            var result = await PostRequestAsync("/api/auth/searchGroup", jObject);
            if (result.HasError)
            {
                return new Result<List<Group>>(result.Code, result.Message);
            }

            if (!(result.Data is JArray group))
            {
                return Result<List<Group>>.ErrorMessage("Error");
            }

            var list = new List<Group>();
            foreach (var cateory in group)
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
                list.Add(g);
            }
            return new Result<List<Group>>(ResultCode.Ok, "OK", list);
        }

        public async Task<Result<List<Group>>> GetGroups()
        {
            var result = await PostRequestAsync("/api/auth/searchGroup");
            if (result.HasError)
            {
                return new Result<List<Group>>(result.Code, result.Message);
            }

            if (!(result.Data is JArray group))
            {
                return Result<List<Group>>.ErrorMessage("Error");
            }

            var list = new List<Group>();
            foreach (var cateory in group)
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
                list.Add(g);
            }
            return new Result<List<Group>>(ResultCode.Ok, "OK", list);
        }
        #endregion

        public Task<Result> SendMessage(string username, string content)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendGroupMessage(long groupId, string content)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<PrivateChat>>> GetPrivateChatHistory()
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<GroupChat>>> GetGroupChatHistory()
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<Chat>>> GetChatHistory()
        {
            throw new NotImplementedException();
        }
    }
}
