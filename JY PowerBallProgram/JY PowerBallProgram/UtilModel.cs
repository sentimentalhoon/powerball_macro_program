using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace JY_PowerBallProgram
{
    class UtilModel
    {
        public static string betid { get; set; }
        public static string _password { get; set; }
        public static string _timetoken { get; set; }
        public static string _userSite { get; set; }
        public static string distributor { get; set; }
        public static int telegramChatId { get; set; }
        public static string _apikey { get; set; }
        public static int _programVersion { get; set; }
        public static int _allBettingEnable { get; set; }
        public static string _macAddress { get; set; }
        public static string _ip { get; set; }
        public static string _limittime { get; set; }
        public static int _multiconnect { get; set; }
        public static string _userprofile { get; set; }
        public static string updateverion { get; set; }
        public static string updatefileurl { get; set; }
        public static string logincheck { get; set; }
        public static string logincomplete { get; set; }
        public static string loginfailed { get; set; }
        public static string servertime { get; set; }
        public static string picksterlist { get; set; }
        public static string vegaurl { get; set; }
        public static string gongurl { get; set; }
        public static string gtmUrl { get; set; }
        public static string ariUrl { get; set; }
        public static string dcbUrl { get; set; }
        public static string factUrl { get; set; }
        public static string aceUrl { get; set; }
        public static string rdwUrl { get; set; }
        public static string updateuserstatus { get; set; }
        public static string powerresult { get; set; }
        public static string userregist { get; set; }
        public static string noticeUrl { get; set; }
        public static string[] notice { get; set; }
        public static string telegramChatUrl { get; set; }
        //public static System.Threading.Mutex dup;
        //public static Boolean _createNew = false;

        public static void TypingOnlyNumber(object sender, KeyPressEventArgs e, bool includePoint, bool includeMinus)
        {
            bool isValidInput = false;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if (includePoint == true) { if (e.KeyChar == '.') isValidInput = true; }
                if (includeMinus == true) { if (e.KeyChar == '-') isValidInput = true; }

                if (isValidInput == false) e.Handled = true;
            }

            if (includePoint == true)
            {
                if (e.KeyChar == '.' && (string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('.') > -1)) e.Handled = true;
            }
            if (includeMinus == true)
            {
                if (e.KeyChar == '-' && (!string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('-') > -1)) e.Handled = true;
            }
        }

        public static String StringFormatChanged(int number)
        {
            return string.Format("{0:#,##0}", number);
        }

        public static string GetExternalIPAddress()
        {
            string externalip = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();

            if (String.IsNullOrWhiteSpace(externalip))
            {
                externalip = GetInternalIPAddress();//null경우 Get Internal IP를 가져오게 한다.
            }

            return externalip;
        }

        public static string GetInternalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static String JPOST(String _url, String _param)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 3000;
            var result = "";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(_param);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                Console.WriteLine(result.ToString());
            }
            return result;
        }
        public static String GetHttpGet(String _url)
        {
            String _stringMessage = "";
            using (TimeoutWebClient client = new TimeoutWebClient())
            {
                client.Encoding = Encoding.UTF8;
                // 결과값을 Json 문자열로 받아온다.
                _stringMessage = client.DownloadString(_url);
            }
            return _stringMessage;
        }

        public static String GetHttpPOST(string reqstring, string url, string method, string encode, ref int errcode)
        {
            String retValue = "";

            if (url.IndexOf("https://") >= 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            }

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = method;
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
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
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
        public static String getDateTime()
        {
            return "[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] ";
        }
        public static String getDateTime2()
        {
            return DateTime.Now.ToString("MM-dd HH:mm:ss");
        }
        public static String getTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
        }

        public static String getTimeHour()
        {
            return DateTime.Now.ToString("HH");
        }
        // 배팅 금액 설정 부분 한글 표기화

        public static string NumToString(Int64 x)
        {
            string HAmt = ""; string EAmt = "";
            Int64 KLen = 0; Int64 ELen = 0;
            int j = 0; int k = 0;
            string W = ""; string Y = "";

            try
            {

                EAmt = x.ToString();
                ELen = EAmt.Length;
                k = 0;
                for (j = 0; j < ELen; j++)
                {
                    KLen = Convert.ToInt64(EAmt.Substring(j, 1));
                    switch (KLen)
                    {
                        case 1:
                            W = "일";
                            break;
                        case 2:
                            W = "이";
                            break;
                        case 3:
                            W = "삼";
                            break;
                        case 4:
                            W = "사";
                            break;
                        case 5:
                            W = "오";
                            break;
                        case 6:
                            W = "육";
                            break;
                        case 7:
                            W = "칠";
                            break;
                        case 8:
                            W = "팔";
                            break;
                        case 9:
                            W = "구";
                            break;
                        case 0:
                            W = "영";
                            break;
                    }
                    switch (ELen)
                    {
                        case 10:
                            Y = "십억천백십만천백십원";
                            break;
                        case 9:
                            Y = "억천백십만천백십원";
                            break;
                        case 8:
                            Y = "천백십만천백십원";
                            break;
                        case 7:
                            Y = "백십만천백십원";
                            break;
                        case 6:
                            Y = "십만천백십원";
                            break;
                        case 5:
                            Y = "만천백십원";
                            break;
                        case 4:
                            Y = "천백십원";
                            break;
                        case 3:
                            Y = "백십원";
                            break;
                        case 2:
                            Y = "십원";
                            break;
                        case 1:
                            Y = "원";
                            break;
                    }
                    if (W != "영")
                    {
                        HAmt = HAmt + (W + Y.Substring(k, 1));
                    }
                    if (Y.Substring(k, 1) == "억")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "억";
                        }
                    }
                    else if (Y.Substring(k, 1) == "만")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "만";
                        }
                    }
                    else if (Y.Substring(k, 1) == "원")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "원";
                        }
                    }

                    k = k + 1;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return HAmt;

        }
    }

}
