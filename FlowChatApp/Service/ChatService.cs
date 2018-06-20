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
        public event EventHandler<InvationConfirmation> ContactConfirmationMessageReceived;

        #region account
        public async Task<Result> SignUpAsync(string email, string username, string nickname, string password)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> SignOutAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<ChatService.TokenClass>> UpdateToken()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<Account>> UpdateAccountInfo(Account account)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<Account>> GetAccountInfo()
        {
            throw new NotImplementedException();
        }
        public async Task<Result> UploadAvator(string filename, byte[] avator)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region contract
        public async Task<Result<List<Contract>>> GetContacts()
        {
            throw new NotImplementedException();
        }
        public async Task<Result> AddContact(string username, string categoryName, string message)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> DeleteContact(long id)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<Contract>>> UpdateContact(string username, string alias, string categroy)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<ContractInvation>>> GetContractInvations()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName, bool accept)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<InvationConfirmation>>> GetInvationConfirmations()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region user
        public async Task<Result<User>> GetUserInfo(string username)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<User>>> SearchUser(SearchType type, string value)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<byte[]>> GetAvator(string url)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region group
        public async Task<Result<List<Group>>> GetGroups()
        {
            throw new NotImplementedException();
        }
        public async Task<Result> JoinGroup(long groupId)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> CreateGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> LeaveGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<Group>>> SearchGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> AddGroupMember(string groupName, string userName)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> DeleteGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> RenameGroup(string oldName, string newName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region chat
        public async Task<Result> SendMessage(string username, string content)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> SendGroupMessage(long groupId, string content)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<PrivateChat>>> GetPrivateChatHistory()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<GroupChat>>> GetGroupChatHistory()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<List<Chat>>> GetChatHistory()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
