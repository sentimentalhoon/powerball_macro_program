using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace PowerBallAutoMatinUserInformation
{
    class LoginHandler
    {
        public String LoginCheck(string id, string password, string timetoken)
        {
            string url = "http://www.pbserver.cf/pickster/userinformationLogin.php";                                         // 통신할 URL
            string msg = string.Format("login=&user_name={0}&user_password={1}&timetoken={2}", id, password, timetoken); // 전송할 Parameter
            string encodeStr = "UTF-8";                                          // 인코딩 방식
            int errorcode = 0;                                                     // 에러 전달받을 값
            string returnVal = "";
            returnVal = GetHttpPOST(msg, url, encodeStr, ref errorcode);
            return returnVal;
        }

        public string GetUserName(string id)
        {
            return id;
        }
        public static String GetHttpPOST(string reqstring, string url, string encode, ref int errcode)
        {
            String retValue = "";

            if (url.IndexOf("https://") >= 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            }

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.AllowAutoRedirect = true;
                Stream requestStream = null;
                byte[] sendData = null;
                if (reqstring != null)
                {
                    if (String.IsNullOrEmpty(encode) || encode == "UTF-8")
                    {
                        sendData = UTF8Encoding.UTF8.GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    }
                    else if (encode == "EUC-KR")
                    {
                        sendData = Encoding.GetEncoding("EUC-KR").GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=EUC-KR";
                    }
                    else
                    {
                        sendData = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=ks_c_5601-1987";
                    }
                    httpWebRequest.ContentLength = sendData.Length;
                    requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(sendData, 0, sendData.Length);
                }
                else
                {
                    httpWebRequest.ContentLength = 0;
                    requestStream = httpWebRequest.GetRequestStream();
                }
                requestStream.Flush();
                requestStream.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding(encode));
                retValue = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    errcode = (int)((HttpWebResponse)ex.Response).StatusCode;
                    retValue = ex.Message;
                }
                else
                {
                    retValue = ex.Message;
                    errcode = -1;
                }
            }
            return retValue;
        }
    }
}
