using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowChatApp.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using FlowChatAppTests;
using System.Windows;

namespace FlowChatApp.Service.Tests
{
    [TestClass()]
    public class ChatServiceTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            var _ = new ViewModel.ViewModelLocator();
        }

        [TestMethod()]
        public void ChatServiceTest()
        {

        }

        [TestMethod()]
        public async Task UpdateAccountInfoTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getAccountInfoResult = await chatService.GetAccountInfo();
            Assert.IsTrue(getAccountInfoResult.Ok);

            var account = getAccountInfoResult.Data;
            Trace.WriteLine(ObjectDumper.Dump(account));

            {
                account.Gender = Gender.Boy;
                var updateAccountInfoResult = await chatService.UpdateAccountInfo(account);
                Assert.IsTrue(updateAccountInfoResult.Ok);

                Assert.AreEqual(Gender.Boy, updateAccountInfoResult.Data.Gender);
                Trace.WriteLine(ObjectDumper.Dump(account));

            }


            {
                account.Gender = Gender.Girl;
                var updateAccountInfoResult = await chatService.UpdateAccountInfo(account);
                Assert.IsTrue(updateAccountInfoResult.Ok);
                Assert.AreEqual(Gender.Girl, updateAccountInfoResult.Data.Gender);
                Trace.WriteLine(ObjectDumper.Dump(account));

            }

        }

        [TestMethod()]
        public async Task SignInTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");
            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);
        }

        [TestMethod()]
        public async Task GetContractsTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getContactsResult = await chatService.GetContacts();
            Assert.IsTrue(getContactsResult.Ok);
            var contracts = getContactsResult.Data;
            Trace.WriteLine(ObjectDumper.Dump(contracts));

        }

        [TestMethod()]
        public async Task GetGroupsTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getGroupsResult = await chatService.GetGroups();
            Assert.IsTrue(getGroupsResult.Ok);
            var groups = getGroupsResult.Data;
            Trace.WriteLine(ObjectDumper.Dump(groups));
        }

        [TestMethod()]
        public async Task GetGroupTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var result = await chatService.GetGroup(1);
            Assert.IsTrue(result.Ok);
            var group = result.Data;
            Trace.WriteLine(ObjectDumper.Dump(group));
        }

        [TestMethod()]
        public async Task GetUserTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getUserInfoResult = await chatService.GetUserInfo("test2");
            Assert.IsTrue(getUserInfoResult.Ok);
            var user = getUserInfoResult.Data;
            Trace.WriteLine(ObjectDumper.Dump(user));
        }

        [TestMethod()]
        public async Task GetPrivateChatMessageTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getPrivateChatHistoryResult = await chatService.GetPrivateChatHistory();
            Assert.IsTrue(getPrivateChatHistoryResult.Ok);
            var chats = getPrivateChatHistoryResult.Data;
            chats.ForEach(c =>
            {
                Trace.WriteLine(c.PeerName);
                Trace.Indent();
                foreach (var m in c.Messages)
                {
                    Trace.WriteLine($"{m.SenderName} -> {m.ReceiverName}");
                    Trace.WriteLine(m.Content);
                }
                Trace.Unindent();
            });
        }

        [TestMethod()]
        public async Task GetUnreadPrivateChatHistoryTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getPrivateChatHistoryResult = await chatService.GetUnreadPrivateChatHistory();
            Assert.IsTrue(getPrivateChatHistoryResult.Ok);
            var chats = getPrivateChatHistoryResult.Data;
            chats.ForEach(c =>
            {
                Trace.WriteLine(c.PeerName);
                Trace.Indent();
                foreach (var m in c.Messages)
                {
                    Trace.WriteLine($"{m.SenderName} -> {m.ReceiverName}");
                    Trace.WriteLine(m.Content);
                }
                Trace.Unindent();
            });
        }

        [TestMethod()]
        public async Task GetGroupChatMessageTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getGroupChatHistoryResult = await chatService.GetGroupChatHistory();
            Assert.IsTrue(getGroupChatHistoryResult.Ok);
            var chats = getGroupChatHistoryResult.Data;
            chats.ForEach(c =>
            {
                Trace.WriteLine(c.PeerName);
                Trace.Indent();
                foreach (var m in c.Messages)
                {
                    Trace.WriteLine($"{m.SenderName}:");
                    Trace.WriteLine(m.Content);
                }
                Trace.Unindent();
            });
        }

        [TestMethod()]
        public async Task GetUnreadGroupChatMessageTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test1", "123");
            Assert.IsTrue(signInResult.Ok);

            var getUnreadGroupChatHistoryResult = await chatService.GetUnreadGroupChatHistory();
            Assert.IsTrue(getUnreadGroupChatHistoryResult.Ok);
            var chats = getUnreadGroupChatHistoryResult.Data;
            chats.ForEach(c =>
            {
                Trace.WriteLine(c.PeerName);
                Trace.Indent();
                foreach (var m in c.Messages)
                {
                    Trace.WriteLine($"{m.SenderName}:");
                    Trace.WriteLine(m.Content);
                }
                Trace.Unindent();
            });
        }
    }
}