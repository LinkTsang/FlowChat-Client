using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FlowChatApp.Service
{
    public enum ResultCode
    {
        Ok = 0,
        Error = 1,
        Bad = 9999
    }

    public class Result<T> where T : class
    {
        public Result(ResultCode code, string message, T data = null)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public ResultCode Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public bool Ok => Code == ResultCode.Ok;
        public bool Error => Code != ResultCode.Ok;

    }

    public class Result : Result<JObject>
    {
        public Result(ResultCode code, string message, JObject data)
            : base(code, message, data)
        {
        }

        public static Result BadRequest = new Result(ResultCode.Bad, "请求错误", null);
    }
}
