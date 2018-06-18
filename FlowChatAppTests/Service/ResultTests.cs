using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowChatApp.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FlowChatApp.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlowChatApp.Service.Tests
{
    [TestClass()]
    public class ResultTests
    {
        private string json = @"{
    'code': 200,
    'message': 'OK',
    'data': {
        'token': '123'
    }
}";
        DefaultContractResolver _contractResolver;

        JsonSerializerSettings _settings;

        [TestInitialize()]
        public void TestInitialize()
        {
            _contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            _settings = new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                Formatting = Formatting.Indented
            };
        }

        [TestMethod()]
        public void ResultTest()
        {
            var jObject = JObject.Parse(json);
            var code = jObject["code"].Value<int>();
            Assert.AreEqual(200, code);
            var message = jObject["message"].Value<string>();
            Assert.AreEqual("OK", message);
            var data = (JObject)jObject["data"];
            var result = new Result((ResultCode)code, message, data);
            Assert.AreEqual("123", result.Data["token"].ToString());
        }

        [TestMethod()]
        public void JsonTest()
        {

            var account = new Account()
            {
                Email = "test@flowchat.com",
                UserName = "test",
                Password = "123456"
            };
            var result = JsonConvert.SerializeObject(account, _settings);
            Trace.WriteLine(result);
            var jObject = JObject.Parse(result);
            Trace.WriteLine(jObject.ToString());
            Assert.AreEqual("test@flowchat.com", jObject["email"].ToString());
        }

        [TestMethod()]
        public void JsonObjectTest()
        {
            var jObject = new JObject
            {
                ["a"] = "a",
                ["b"] = "c"
            };
            var result = JsonConvert.SerializeObject(jObject, _settings);
            Trace.WriteLine(result);
        }

        [TestMethod]
        public void JsonAonymousObject()
        {
            var @object = new
            {
                test = "123",
            };
            var result = JsonConvert.SerializeObject(@object, _settings);
            Trace.WriteLine(result);
        }
    }
}