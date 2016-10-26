using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNet4.Utilities;
using Newtonsoft.Json;

namespace JSMS
{
	#region error ack
	public class ErrMsg
	{
		public int code { get; set; }

		public string message { get; set; }
	}
	public class AckError
	{//"{\"error\":{\"code\":50013,\"message\":\"invalid temp_id\"}}"

		public ErrMsg error;
	}
	#endregion

	#region OkACK from JSMS
	public class AckSmsOk
	{//"{\"msg_id\":\"f4e9dcaa-d89a-43a7-ac7a-ace0dbe0ca12\"}"
		public string msg_id;
	}
	#endregion
	public class SendSmsAck
	{
		public HttpStatusCode httpCode { get; set; }
		public string msg_id;
		public ErrMsg err { get; set; }
	}

	public class ValidResult
	{
		public HttpStatusCode httpCode { get; set; }
		public bool is_valid { get; set; }
		public ErrMsg error { get; set; }
		/*
    "is_valid": false,
    "error": {
        "code": "***",
        "message": "***"
    }*/
	}



	public class JSMS
	{
		static string BASE_URL = "https://api.sms.jpush.cn/v1/";
		static string CODE_URL = "codes";
		static string authKey = "appKey:masterSecret";
		static public string GetKey()
		{
			return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authKey));
		}
		static public SendSmsAck SendSms(String phone,String tempId)
		{
			HttpHelper http = new HttpHelper();
			var oo = new
			{
				mobile = phone,
				temp_id = tempId
			};
			HttpItem item = new HttpItem()
			{
				URL = BASE_URL + CODE_URL,
				Method = "POST",
				Postdata = JsonConvert.SerializeObject(oo)
			};
			
			item.Header.Add("Authorization",GetKey());
			var ack = http.GetHtml(item);
			SendSmsAck result = new SendSmsAck();
			result.httpCode = ack.StatusCode;
			if (ack.StatusCode == HttpStatusCode.OK)
			{
				var ackOk = JsonConvert.DeserializeObject<AckSmsOk>(ack.Html);
				result.msg_id = ackOk.msg_id;
			}
			else
			{
				var err = JsonConvert.DeserializeObject<AckError>(ack.Html);
				result.err = err.error;
			}
			return result;
		}
		static public ValidResult ValidSms(String smsId, String code)
		{
			HttpHelper http = new HttpHelper();
			var oo = new
			{
				code = code
			};
			HttpItem item = new HttpItem()
			{
				URL = BASE_URL + CODE_URL+"/"+smsId+"/valid",
				Method = "POST",
				Postdata = JsonConvert.SerializeObject(oo)
			};

			item.Header.Add("Authorization", GetKey());
			var ack = http.GetHtml(item);

			ValidResult result = JsonConvert.DeserializeObject<ValidResult>(ack.Html);
			result.httpCode = ack.StatusCode;

			return result;
		}

	}
}
