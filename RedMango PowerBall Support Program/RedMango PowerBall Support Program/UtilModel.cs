using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RedMango_PowerBall_Support_Program
{
    class UtilModel
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static int MAX_BET { get; set; }
        public static bool ResultServersNtry { get; set; }
        public static bool ResultServersUpdown { get; set; }
        public static bool ResultServersBepick { get; set; }
        public static bool ResultServersApiSite { get; set; }
        public static int LoginSiteType { get; set; }
        public static string Bet_Api_Key { get; set; }
        public static string ServerProgramVersion { get; set; }
        public static string ProgramVersion { get; set; }
        public static string SitePort { get; set; }
        public static int PBGVisible { get; set; }
        public static string _macAddress { get; set; }
        public static string _ip { get; set; }
        public static string _limittime { get; set; }
        public static int _multiconnect { get; set; }
        public static string UserProfile { get; set; }
        public static string updatefileurl { get; set; }
        public static string UpdateVersion { get; set; }
        public static string fileDownloadUrl { get; set; }
        public static string configFileDownloadUrl { get; set; }
        public static string AuthSite { get; set; }
        public static string logincheck { get; set; }
        public static string logincomplete { get; set; }
        public static string loginfailed { get; set; }
        public static string servertime { get; set; }
        public static string picksterlist { get; set; }
        public static string UserSiteUrlAddress { get; set; }
        public static string updateuserstatus { get; set; }
        public static string powerresult { get; set; }
        public static string userregist { get; set; }
        public static string noticeUrl { get; set; }
        public static string[] notice { get; set; }
        public static string telegramChatUrl { get; set; }
        public static string UserId { get; set; }
        public static string SamePerson { get; set; }
        public static string ResultMark { get; set; }
        public static string ErrorBeep { get; set; }
        public static string BettingFail { get; set; }
        public static string PatternBetNumber { get; set; }
        public static string UseAutoReverse { get; set; }
        public static string UseOverProfit { get; set; }
        public static string OverProfitValue { get; set; }
        public static string UseUnderProfit { get; set; }
        public static string UnderProfitValue { get; set; }
        public static int UserOwnMoney { get; set; }
        public static string[] termSelectNumber { get; set; }
        public static string[] termModePick { get; set; }
        public static string[] DecalMoney { get; set; }
        public static string[] TermMoney { get; set; }
        public static string[] Cruise_Bet_Money { get; set; }
        public static string[] PatternEqualSettingMoney { get; set; }
        public static string[] BoxPattern { get; set; }
        public static string[] BoxPatternAllPick { get; set; }
        public static string[] EosCruiseBetListLevel { get; set; }
        public static readonly int LoginSiteType_ERROR = -1;
        public static readonly int LoginSiteType_Click = 0;
        public static readonly int LoginSiteType_Royal = 1;
        public static readonly int LoginSiteType_Life = 2;

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
        public static string LoadInformation()
        {
            try
            {
                String returnMessage = String.Empty;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnMessage = webClient.DownloadString("http://say4m.cafe24.com/auth_site.json");
                }
                return returnMessage;
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
            return null;
        }
        public static void XMLLoadPropertiesSettings()
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("propertiesSettings.xml");

                // 첫노드를 잡아주고 하위 노드를 선택한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode("propertiesSettings");
                if (SubNode != null)
                {
                    XmlNode selectNode;
                    selectNode = SubNode.SelectSingleNode("UserSiteUrlAddress");
                    if (selectNode != null)
                    {
                        UserSiteUrlAddress = selectNode.InnerText;
                    }
                    else
                    {
                        UserSiteUrlAddress = "http://www.";
                    }

                    selectNode = SubNode.SelectSingleNode("id");
                    if (selectNode != null)
                    {
                        UserId = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("samePerson");
                    if (selectNode != null)
                    {
                        SamePerson = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("resultMark");
                    if (selectNode != null)
                    {
                        ResultMark = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("errorBeep");
                    if (selectNode != null)
                    {
                        ErrorBeep = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("bettingFail");
                    if (selectNode != null)
                    {
                        BettingFail = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("patternBetNumber");
                    if (selectNode != null)
                    {
                        PatternBetNumber = selectNode.InnerText;
                    }
                    else
                    {
                        PatternBetNumber = "0";
                    }
                    selectNode = SubNode.SelectSingleNode("useAutoReverse");
                    if (selectNode != null)
                    {
                        UseAutoReverse = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("useOverProfit");
                    if (selectNode != null)
                    {
                        UseOverProfit = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("OverProfitValue");
                    if (selectNode != null)
                    {
                        OverProfitValue = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("useUnderProfit");
                    if (selectNode != null)
                    {
                        UseUnderProfit = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("UnderProfitValue");
                    if (selectNode != null)
                    {
                        UnderProfitValue = selectNode.InnerText;
                    }
                    /* =======================================*/
                    selectNode = SubNode.SelectSingleNode("termSelectNumber");
                    if (selectNode != null)
                    {
                        termSelectNumber = selectNode.InnerText.Split(new char[] { ',' });
                    }
                    selectNode = SubNode.SelectSingleNode("termModePick");
                    if (selectNode != null)
                    {
                        termModePick = selectNode.InnerText.Split(new char[] { ',' });
                    }
                    selectNode = SubNode.SelectSingleNode("DecalMoney");
                    if (selectNode != null)
                    {
                        DecalMoney = selectNode.InnerText.Split(new char[] { ',' });
                    }
                    selectNode = SubNode.SelectSingleNode("TermMoney");
                    if (selectNode != null)
                    {
                        TermMoney = selectNode.InnerText.Split(new char[] { ',' });
                    }

                    selectNode = SubNode.SelectSingleNode("Cruise_Bet_Money");
                    if (selectNode != null)
                    {
                        Cruise_Bet_Money = selectNode.InnerText.Split(new char[] { ',' });
                    }

                    selectNode = SubNode.SelectSingleNode("PatternEqualSettingMoney");
                    if (selectNode != null)
                    {
                        PatternEqualSettingMoney = selectNode.InnerText.Split(new char[] { ',' });
                    }

                    selectNode = SubNode.SelectSingleNode("BoxPattern");
                    if (selectNode != null)
                    {
                        BoxPattern = selectNode.InnerText.Split(new char[] { ',' });
                    }

                    selectNode = SubNode.SelectSingleNode("BoxPatternAllPick");
                    if (selectNode != null)
                    {
                        BoxPatternAllPick = selectNode.InnerText.Split(new char[] { ',' });
                    }

                    selectNode = SubNode.SelectSingleNode("EosCruiseBetListLevel");
                    if (selectNode != null)
                    {
                        BoxPatternAllPick = selectNode.InnerText.Split(new char[] { ',' });
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
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

        private static Regex DECODING_REGEX =
            new Regex(@"\\u(?<Value>[a-fA-F0-9]{4})",
                RegexOptions.Compiled);
        private const string PLACEHOLDER = @"#!쀍쀍쀍!#";

        /// <summary>
        /// Unicode Decoding
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeToString(string str)
        {
            return UnicodeToString(str, 65001);
        }

        /// <summary>
        /// Unicode Decoding
        /// </summary>
        /// <param name="str"></param>
        /// <param name="codePage"></param>
        /// <returns></returns>
        public static string UnicodeToString(string str, int codePage)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return DECODING_REGEX.Replace(
                str.Replace(@"\\", PLACEHOLDER),
                new MatchEvaluator(CapText)).Replace(PLACEHOLDER, @"\\");
        }

        static string CapText(Match m)
        {
            // Get the matched string.
            return ((char)int.Parse(m.Groups["Value"].Value,
                NumberStyles.HexNumber)).ToString();
        }
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
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 2000;
            request.ReadWriteTimeout = 5000;
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
                httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                httpWebRequest.ContentType = "application/json";
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
                Message = Message + " // " + ex.Message;
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
                httpWebRequest.ReadWriteTimeout = 10000;
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
            return DateTime.Now.ToString("MM-dd HH:mm");
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
