using System;

namespace AutoMartinProgram
{
    class LoginHandler
    {
        public String LoginCheck(string id, string password, string macaddress, string timetoken)
        {
            string url = UtilModel.logincheck;                                         // 통신할 URL
            string msg = string.Format("login=&user_name={0}&user_password={1}&macaddress={2}&timetoken={3}", id, password, macaddress, timetoken); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = "";
            returnVal = UtilModel.GetHttpPOST(msg, url, encodeStr, ref errorcode);
            return returnVal;
        }

        public String LoginComplete(string id, string user_ip, string macaddress, int programversion)
        {
            string url = UtilModel.logincomplete;                                       // 통신할 URL
            string msg = string.Format("logincomplete=OK&user_name={0}&user_ip={1}&macaddress={2}&version={3}", id, user_ip, macaddress, programversion); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = "";
            returnVal = UtilModel.GetHttpPOST(msg, url, encodeStr, ref errorcode);
            return returnVal;
        }

        public String LoginFailed(string id, string user_ip, string macaddress, string reason, int programversion)
        {
            string url = UtilModel.loginfailed;                                         // 통신할 URL
            string msg = string.Format("loginfailed=ERROR&user_name={0}&user_ip={1}&macaddress={2}&reason={3}&version={4}", id, user_ip, macaddress, reason, programversion); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = "";
            returnVal = UtilModel.GetHttpPOST(msg, url, encodeStr, ref errorcode);
            return returnVal;
        }
        
        // 엑셀 VBA 함수 Split 처럼 함수로 만들어서 사용

        public string GetUserName(string id)
        {
            string name = id;
            return name;
        }

    }
}
