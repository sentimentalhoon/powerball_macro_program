using System;
using System.Text;

namespace BlueMango_PowerBall_Support_Program
{
    class LoginHandler
    {
        public String BettingSiteUserLoginCheck(string Url, string Id, string Pass, string version)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Url);
            stringBuilder.Append(":8085/auto/api/user_auth");
            stringBuilder.AppendFormat("?u={0}&p={1}&v={2}", Id, Pass, version);
            var rm = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
            return rm.Result;
        }

        public String LoginCheck(string id, string password, string macaddress, string timetoken)
        {
            string url = UtilModel.logincheck;                                         // 통신할 URL
            string msg = string.Format("login=&user_name={0}&user_password={1}&macaddress={2}&timetoken={3}", id, password, macaddress, timetoken); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = UtilModel.GetHttpPOST(msg, url, "POST", encodeStr, ref errorcode);
            return returnVal;
        }

        public String LoginComplete(string id, string user_ip, string macaddress, int programversion, string token, string password, string userprofile, string apikey, string usersite)
        {
            string url = UtilModel.logincomplete;                                         // 통신할 URL
            string UrlParam = string.Format("logincomplete=OK&user_name={0}&user_ip={1}&macaddress={2}&version={3}&timetoken={4}&password={5}&userprofile={6}&apikey={7}&usersite={8}",
                id, user_ip, macaddress, programversion, token, password, userprofile, apikey, usersite.Replace("http://www.", "").Replace(".com", "")); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = UtilModel.GetHttpPOST(UrlParam, url, "POST", encodeStr, ref errorcode);
            return returnVal;
        }

        public String LoginFailed(string id, string user_ip, string macaddress, string reason, int programversion)
        {
            string url = UtilModel.loginfailed;                                         // 통신할 URL
            string msg = string.Format("loginfailed=ERROR&user_name={0}&user_ip={1}&macaddress={2}&reason={3}&version={4}", id, user_ip, macaddress, reason, programversion); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = UtilModel.GetHttpPOST(msg, url, "POST", encodeStr, ref errorcode);
            return returnVal;
        }

        public string GetUserName(string id)
        {
            string name = id;
            return name;
        }

    }
}