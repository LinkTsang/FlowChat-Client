﻿using FlowChatApp.Model;
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
        event EventHandler<InvationConfirmation> ContactConfirmationMessageReceived;
        event EventHandler<Result> BadRequestRaised;

        #region account
        Task<Result> SignUpAsync(string email, string username, string nickname, string password);
        Task<Result<ChatService.TokenClass>> SignInAsync(string username, string password);
        Task<Result> SignOutAsync();
        Task<Result<ChatService.TokenClass>> UpdateToken();
        Task<Result<Account>> UpdateAccountInfo(Account account);
        Task<Result<Account>> GetAccountInfo();
        Task<Result> UploadAvator(string filename, byte[] avator);
        #endregion

        #region contract
        Task<Result<List<Contract>>> GetContacts();
        Task<Result> AddContact(string username, string categoryName, string message);
        Task<Result> DeleteContact(long id);
        Task<Result<List<Contract>>> UpdateContact(string username, string alias, string categroy);
        Task<Result<List<ContractInvation>>> GetContractInvations();
        Task<Result<List<ContractInvation>>> ConfirmContractInvation(string recordId, string categoryName, bool accept);
        Task<Result<List<InvationConfirmation>>> GetInvationConfirmations();

        #endregion

        #region user
        Task<Result<User>> GetUserInfo(string username);
        Task<Result<List<User>>> SearchUser(SearchType type, string value);
        Task<Result<byte[]>> GetAvator(string url);
        #endregion

        #region group
        Task<Result<List<Group>>> GetGroups();
        Task<Result> JoinGroup(long groupId);
        Task<Result> CreateGroup(string groupName);
        Task<Result> LeaveGroup(string groupName);
        Task<Result<List<Group>>> SearchGroup(string groupName);
        Task<Result> AddGroupMember(string groupName, string userName);
        Task<Result> DeleteGroup(string groupName);
        Task<Result> RenameGroup(string oldName, string newName);
        #endregion

        #region chat
        Task<Result> SendMessage(string username, string content);
        Task<Result> SendGroupMessage(long groupId, string content);
        Task<Result<List<PrivateChat>>> GetPrivateChatHistory();
        Task<Result<List<GroupChat>>> GetGroupChatHistory();
        Task<Result<List<Chat>>> GetChatHistory();
        #endregion
    }
}
