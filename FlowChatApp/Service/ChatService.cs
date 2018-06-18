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


        async Task<Result> PostRequestAsync(string requestUri)
        {
            var content = new StringContent(string.Empty, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("token", _token);
            var response = await _httpClient.PostAsync(requestUri, content, _cancellationToken);
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
            return Result.BadRequest;
        }

        async Task<Result<T>> PostRequestAsync<T>(string requestUri) where T : class
        {
            var response = await PostRequestAsync(requestUri);
            if (response == null)
            {
                return null;
            }
            var result = new Result<T>(response.Code, response.Message);
            if (response.Data != null)
            {
                result.Data = response.Data.ToObject<T>();
            }
            return result;
        }

        async Task<Result> PostRequestAsync(string requestUri, JObject jObject)
        {
            var content = new StringContent(jObject.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("token", _token);
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
                    var result = JObject.Parse(s);
                    var code = result["code"].Value<int>();
                    var message = result["msg"].Value<string>();
                    var data = result["data"] as JObject;
                    return new Result((ResultCode) code, message, data);
                }
            }
            catch (HttpRequestException e)
            {
                Trace.WriteLine(e.Message);
                return new Result(ResultCode.Bad, e.Message, null);
            }

            return Result.BadRequest;
        }

        async Task<Result<T>> PostRequestAsync<T>(string requestUri, JObject jObject) where T : class
        {
            var response = await PostRequestAsync(requestUri, jObject);
            if (response == null)
            {
                return null;
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

        public async Task<Result<User>> GetAccountInfo()
        {
            return await PostRequestAsync<User>("/api/auth/searchUserInfo");
        }

        //public Task<Result<User>> GetAccountInfo(params string[] ids)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Result<User>> UpdateUserInfo(User user)
        {
            return await PostRequestAsync<User>("/api/auth/modifyUserInfo", JObject.FromObject(user));
        }

        public async Task<Result<User>> SearchUser(SearchType type, string value)
        {
            var jObject = new JObject()
            {
                ["userid"] = value,
            };
            return await PostRequestAsync<User>("/api/service/searchUserInfo", jObject);
        }

        public Task<Result<Contract>> GetContacts()
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetContactStatus(params string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AddContact(string username, string categoryName, string message)
        {
            //var jObject = new JObject()
            //{
            //    ["username"] = username,
            //    ["categoryName"] = categoryName,
            //    ["message"] = message,
            //};
            //return await PostRequestAsync("/api/service/searchUserInfo", jObject);
            throw new NotImplementedException();

        }

        public Task<Result<List<ContractInvation>>> GetContractInvation()
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteContact(string id)
        {
            throw new NotImplementedException();
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

        public Task<Result> SendMessage(string userId, string content)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendGroupMessage(string groupId, string content)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ChatMessage>>> GetChatHistory()
        {
            throw new NotImplementedException();
        }
    }
}
