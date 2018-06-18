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

namespace FlowChatApp.Service.Tests
{
    [TestClass()]
    public class ChatServiceTests
    {
        [TestMethod()]
        public void ChatServiceTest()
        {

        }

        [TestMethod()]
        public void SignInAsyncTest()
        {

        }

        [TestMethod()]
        public async Task SignUpAsyncTest()
        {
            //IChatService chatService = new ChatService("http://127.0.0.1:8081");
            //var result = await chatService.SignUpAsync("test@flowchat.com", "test", "test", "123456");
            IChatService chatService = new ChatService("http://127.0.0.1:8081");
            var user = new User()
            {
                Email = "jack@flowchat.com",
                NickName = "Jack",
                Gender = 0,
                Status = "Hello World!"
            };
            var signUpResult = await chatService.SignUpAsync(user.Email, user.UserName, user.NickName, "1231231");
            var signInResult = await chatService.SignInAsync(user.UserName, "1231231");

            var result = await chatService.UpdateUserInfo(user);
        }

        [TestMethod()]
        public async Task SignInTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");
            var signInResult = await chatService.SignInAsync("test0", "123");
            Assert.IsTrue(signInResult.Ok);
        }

        [TestMethod()]
        public async Task ContractsTest()
        {
            var chatService = new ChatService("http://127.0.0.1:8081");

            var signInResult = await chatService.SignInAsync("test0", "123");
            Assert.IsTrue(signInResult.Ok);

            var getContactsResult = await chatService.GetContacts();
            Assert.IsTrue(getContactsResult.Ok);
            var contracts = getContactsResult.Data;
            Trace.WriteLine(ObjectDumper.Dump(contracts));
        }
    }
}