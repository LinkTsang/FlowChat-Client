using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowChatApp.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowchat.Protos;
using FlowChatApp.Service2;

namespace FlowChatApp.Service2.Tests
{
    [TestClass()]
    public class ChatServiceTests
    {
        ChatService _chatService = new ChatService("http://localhost:8081/api/");

        [TestMethod()]
        public void ChatServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public async Task SignTest()
        {
            const string email = "test@flowchat.com";
            const string username = "admin";
            const string nickname = "test";
            const string password = "test";
            string userid;
            string token;
            {
                var request = new SignUpRequest()
                {
                    Email = email,
                    UserName = username,
                    NickName = nickname,
                    Password = password
                };
                var response = await _chatService.SignUpAsync(request);
                Trace.WriteLine(response.ToString());
                Assert.AreEqual(ResponseCode.Ok, response.Code);
                userid = response.UserId;
            }

            {
                var request = new SignInRequest()
                {
                    Email = email,
                    Password = password
                };
                var response = await _chatService.SignInAsync(request);
                Trace.WriteLine(response.ToString());
                Assert.AreEqual(ResponseCode.Ok, response.Code);
                Assert.AreEqual(response.UserId, userid);
                token = response.Token;
            }

            {
                var request = new SignOutRequest()
                {
                    Token = token
                };
                var response = await _chatService.SignOutAsync(request);
                Trace.WriteLine(response.ToString());
                Assert.AreEqual(ResponseCode.Ok, response.Code);
            }
        }

        [TestMethod()]
        public void GetUserInfoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SearchUserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetContactsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetContactStatusTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddContactTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteContactTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBlockedTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BlockContactTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UnblockContactTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendMessageTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetHistoryTest()
        {
            Assert.Fail();
        }
    }
}