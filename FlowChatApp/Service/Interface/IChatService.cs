using FlowChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        Task<Result> AddContact(string username, string categoryName, string message);
        Task<Result> AddGroup(string groupName);
        Task<Result> AddGroupMember(string groupName, string userName);
        Task<Result> BlockContact(string id);
        Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName);
        void Connect(IPAddress ipAddress, int port);
        Task<Result> DeleteContact(string id);
        Task<Result> DeleteGroup(string groupName);
        Task<Result<User>> GetAccountInfo();
        Task<Result<byte[]>> GetAvator();
        Task<Result<List<string>>> GetBlocked();
        Task<Result<List<ChatMessage>>> GetChatHistory();
        Task<Result<List<Contract>>> GetContacts();
        Task<Result> GetContactStatus(params string[] ids);
        Task<Result<List<ContractInvation>>> GetContractInvation();
        Task<Result> RenameGroup(string oldName, string newName);
        Task<Result<List<Group>>> SearchAllGroup();
        Task<Result<List<Group>>> SearchGroup(string groupName);
        Task<Result<User>> SearchUser(SearchType type, string value);
        Task<Result> SendGroupMessage(string groupId, string content);
        Task<Result> SendMessage(string userId, string content);
        Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password);
        Task<Result> SignOutAsync();
        Task<Result> SignUpAsync(string email, string username, string nickname, string password);
        Task<Result> UnblockContact(string id);
        Task<Result<User>> UpdateUserInfo(User user);
        Task<Result> UploadAvator(string filename, byte[] avator);
    }
}
