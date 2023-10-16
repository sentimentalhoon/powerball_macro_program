using log4net;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WaterMelonBettingProgram
{
    class UtilModel
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string ApiKey { get; set; }
        public static string BettingUrlAddress { get; set; }
        public static string UserId { get; set; }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static System.DateTime getDate(string st)
        {
            try
            {
                if (st == null)
                    return DateTime.Now;
                if (string.IsNullOrEmpty(st))
                    return DateTime.Now;
                return Convert.ToDateTime(st);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static int getInteger(string st)
        {
            try
            {
                if (st == null)
                    return 0;
                if (string.IsNullOrEmpty(st))
                    return 0;
                return Convert.ToInt32(st);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool getBoolean(string st)
        {
            try
            {
                st = st.Trim();
                if (st == "true" | st == "1")
                {
                    return true;
                }
                else if (st.ToLower() == "false" | st == "0")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string getBooleanString(Boolean bl)
        {
            try
            {
                if (bl)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                return "false";
            }
        }

        /*
        public static void XMLAttributesAppend(XmlDocument xDoc, XmlElement el, string sStr, string sValue)
        {
            XmlAttribute at = null;
            at = xDoc.CreateAttribute(sStr);
            at.Value = sValue;
            el.Attributes.Append(at);
        }

        public static XmlElement XMLInnerText(XmlNode root, XmlDocument XDoc, string sStr, string sValue = "")
        {
            XmlElement el = null;
            el = XDoc.CreateElement(sStr);
            if (!string.IsNullOrEmpty(sValue))
                el.InnerText = sValue;
            root.LastChild.AppendChild(el);
            return el;
        }

        public static XmlNodeList FindNodeList(XmlNodeList sNode, string sStr)
        {
            for (int i = 0; i <= sNode.Count - 1; i++)
            {
                if (sNode.Item(i).Name.ToLower() == sStr.ToLower())
                {
                    return sNode.Item(i).ChildNodes;
                }
            }
            return null;
        }

        public static XmlNode FindNode(XmlNodeList sNode, string sStr)
        {
            for (int i = 0; i <= sNode.Count - 1; i++)
            {
                if (sNode.Item(i).Name.ToLower() == sStr.ToLower())
                {
                    return sNode.Item(i);
                }
            }
            return null;
        }

        public static string FindNodeInnerText(XmlNodeList sNode, string sStr, string tmpString = "")
        {

            try
            {
                for (int i = 0; i <= sNode.Count - 1; i++)
                {
                    if (sNode.Item(i).Name.ToLower() == sStr.ToLower())
                    {
                        return sNode.Item(i).InnerText;
                    }
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(tmpString))
                    return tmpString;
                else
                    return "";
            }
            if (!string.IsNullOrEmpty(tmpString))
                return tmpString;
            else
                return "";

        }

        public static string FindAttributeText(XmlNode sNode, string sStr, string tmpString = "")
        {
            try
            {
                XmlAttributeCollection rAtt = sNode.Attributes;
                for (int i = 0; i <= rAtt.Count - 1; i++)
                {
                    if (rAtt.Item(i).Name.ToLower() == sStr.ToLower())
                    {
                        return rAtt.Item(i).Value;
                    }
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(tmpString))
                    return tmpString;
                else
                    return "";
            }
            if (!string.IsNullOrEmpty(tmpString))
                return tmpString;
            else
                return "";

        }
        */
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
        /// <summary>
        /// Delay 함수 MS
        /// </summary>
        /// <param name="MS">(단위 : MS)
        ///
        public static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
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

        // Define other methods and classes here
        public static Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 5000;
            request.Proxy = null;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult), (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
        public static String GetHttp(Uri Url, String method, String Param)
        {
            String Message = null;
            try
            {
                var httpWebRequest = HttpWebRequest.CreateHttp(Url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                httpWebRequest.Method = method;
                httpWebRequest.Timeout = 1500;
                if (method.Equals("POST"))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(Param);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                using (var res = httpWebRequest.GetResponse())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            Message = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    logger.Info(((int)((HttpWebResponse)ex.Response).StatusCode));
                    logger.Info(ex.Message);
                }
                else
                {
                    logger.Info(ex.Message);
                }
                Message = "ERROR";
            }
            return Message;
        }
        public static String GetHttp(String _url)
        {
            String _stringMessage = null;
            try
            {
                using (TimeoutWebClient client = new TimeoutWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    _stringMessage = client.DownloadString(_url);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    logger.Info(((int)((HttpWebResponse)ex.Response).StatusCode));
                    logger.Info(ex.Message);
                }
                else
                {
                    logger.Info(ex.Message);
                }
                _stringMessage = null;
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
                httpWebRequest.Timeout = 2000;
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

        public static String RegexOnlyNumber(String textNumber)
        {
            int outValue = 0;
            bool _b = int.TryParse(Regex.Replace(textNumber, @"\D", ""), out outValue);

            return outValue.ToString(); ;
        }
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
