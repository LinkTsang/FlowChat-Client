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
        event EventHandler<Result> BadRequestRaised;

        Task<Result> AddContact(string username, string categoryName, string message);
        Task<Result> AddGroup(string groupName);
        Task<Result> JoinGroup(long groupId);
        Task<Result> AddGroupMember(string groupName, string userName);
        Task<Result> BlockContact(string id);
        Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName);
        void Connect(IPAddress ipAddress, int port);
        Task<Result> DeleteContact(long id);
        Task<Result> DeleteGroup(string groupName);
        Task<Result<Account>> GetAccountInfo();
        Task<Result<byte[]>> GetAvator();
        Task<Result<List<string>>> GetBlocked();
        Task<Result<List<PrivateChat>>> GetPrivateChatHistory();
        Task<Result<List<GroupChat>>> GetGroupChatHistory();
        Task<Result<List<Chat>>> GetChatHistory();

        Task<Result<List<Contract>>> GetContacts();
        Task<Result> GetContactStatus(params string[] ids);
        Task<Result<List<ContractInvation>>> GetContractInvation();
        Task<Result> RenameGroup(string oldName, string newName);
        Task<Result<List<Group>>> GetGroups();
        Task<Result<List<Group>>> SearchGroup(string groupName);
        Task<Result<List<User>>> SearchUser(SearchType type, string value);
        Task<Result> SendGroupMessage(long groupId, string content);
        Task<Result> SendMessage(string username, string content);
        Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password);
        Task<Result> SignOutAsync();
        Task<Result> SignUpAsync(string email, string username, string nickname, string password);
        Task<Result> UnblockContact(string id);
        Task<Result<Account>> UpdateAccountInfo(Account account);
        Task<Result> UploadAvator(string filename, byte[] avator);
    }
}
