using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AutoMartinProgram
{
    class UtilModel
    {
        public static string _id;
        public static string _password;
        public static string _timetoken;
        public static string _userSite;
        public static string _apikey;
        public static int _programVersion;
        public static int _allBettingEnable = 0;
        public static string _macAddress;
        public static string _ip;
        public static string _limittime;
        public static int _multiconnect = 0;

        public static string updateverion;
        public static string logincheck;
        public static string logincomplete;
        public static string loginfailed;
        public static string servertime;
        public static string picksterlist;
        public static string wonderfulurl;
        public static string rdwUrl;
        public static string updateuserstatus;
        public static string powerresult;
        public static string userregist;
        public static string noticeUrl;

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
            string externalip = new WebClient().DownloadString("http://ipinfo.io/ip").Trim(); //http://icanhazip.com

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
