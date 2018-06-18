using FlowChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Service.Interface
{
    public enum SearchType
    {
        ByEmail,
        ByUserName,
        ByNickName,
    }
    public interface IChatService
    {
        event EventHandler<ChatMessage> ChatMessageReceived;
        event EventHandler<ContractInvation> ContactRequsetMessageReceived;

        Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password);
        Task<Result> SignUpAsync(string email, string username, string nickname, string password);
        Task<Result> SignOutAsync();

        Task<Result<User>> GetAccountInfo();
        //Task<Result<User>> GetAccountInfo(params string[] ids);
        Task<Result<User>> UpdateUserInfo(User user);
        Task<Result<User>> SearchUser(SearchType type, string value);

        Task<Result<Contract>> GetContacts();
        Task<Result> GetContactStatus(params string[] ids);
        Task<Result> AddContact(string username, string categoryName, string message);
        Task<Result<List<ContractInvation>>> GetContractInvation();
        Task<Result> DeleteContact(string id);
        Task<Result<List<string>>> GetBlocked();
        Task<Result> BlockContact(string id);
        Task<Result> UnblockContact(string id);

        Task<Result> SendMessage(string userId, string content);
        Task<Result> SendGroupMessage(string groupId, string content);

        Task<Result<List<ChatMessage>>> GetChatHistory();


    }
}
