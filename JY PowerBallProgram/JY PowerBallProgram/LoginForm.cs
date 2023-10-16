using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace JY_PowerBallProgram
{
    public delegate void EventHandler(string name);
    public partial class LoginForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler loginEventHandler;
        public LoginForm()
        {
            loadInformation();
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            _programVersionLabel.Text = "Version | " + programVersion;
            UtilModel._programVersion = programVersion;
            CheckVersion();
        }

        public static void loadInformation()
        {
            try
            {
                String _uri = "http://bit.ly/30r6FrV";
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("setting"))
                {
                    UtilModel.updateverion = item.Element("updateversion").Value;
                    UtilModel.updatefileurl = item.Element("updatefileurl").Value;
                    UtilModel.logincheck = item.Element("logincheck").Value;
                    UtilModel.logincomplete = item.Element("logincomplete").Value;
                    UtilModel.loginfailed = item.Element("loginfailed").Value;
                    UtilModel.servertime = item.Element("servertime").Value;
                    UtilModel.picksterlist = item.Element("picksterlist").Value;
                    UtilModel.vegaurl = item.Element("vegaurl").Value;
                    UtilModel.gongurl = item.Element("gongurl").Value;
                    UtilModel.gtmUrl = item.Element("gtmUrl").Value;
                    UtilModel.ariUrl = item.Element("ariUrl").Value;
                    UtilModel.dcbUrl = item.Element("dcbUrl").Value;
                    UtilModel.factUrl = item.Element("factUrl").Value;
                    UtilModel.aceUrl = item.Element("aceUrl").Value;
                    UtilModel.rdwUrl = item.Element("rdwUrl").Value;
                    UtilModel.updateuserstatus = item.Element("updateuserstatus").Value;
                    UtilModel.powerresult = item.Element("powerresult").Value;
                    UtilModel.userregist = item.Element("userregist").Value;
                    UtilModel.noticeUrl = item.Element("notice").Value;
                    UtilModel.telegramChatUrl = item.Element("telegramChatUrl").Value;
                };
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        int programVersion = 2020020418;

        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                LoginHandler loginHandler = new LoginHandler();
                if (ControlCheck())
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    UtilModel._timetoken = Hash(timestamp);
                    UtilModel._ip = UtilModel.GetExternalIPAddress();
                    UtilModel._macAddress = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
                    String returnMessage = loginHandler.LoginCheck(UsernameTextBox.Text, PasswordTextBox.Text, UtilModel._macAddress, UtilModel._timetoken);
                    var login = "";
                    var username = "";
                    try
                    {
                        JObject jo = JObject.Parse(returnMessage);
                        login = jo.SelectToken("login").ToString();
                        username = jo.SelectToken("username").ToString();
                        UtilModel._userSite = jo.SelectToken("usersite").ToString();
                        UtilModel._userprofile = jo.SelectToken("userprofile").ToString();
                        UtilModel._apikey = jo.SelectToken("apikey").ToString();
                        UtilModel.betid = jo.SelectToken("betid").ToString();
                        UtilModel._password = jo.SelectToken("betpassword").ToString();
                        UtilModel._allBettingEnable = int.Parse(jo.SelectToken("allbettingEnable").ToString());
                        UtilModel._limittime = jo.SelectToken("limittime").ToString();
                        UtilModel._multiconnect = int.Parse(jo.SelectToken("multiconnect").ToString());
                        UtilModel.distributor = jo.SelectToken("distributor").ToString();
                        UtilModel.telegramChatId = int.Parse(jo.SelectToken("telegramChatId").ToString());
                    }
                    catch (Exception _ex)
                    {
                        logger.Error(_ex.ToString());
                        login = "Exception";
                    }

                    if (login.Contains("Complete"))
                    {
                        try
                        {
                            using (TimeoutWebClient webClient = new TimeoutWebClient())
                            {
                                webClient.Encoding = Encoding.UTF8;
                                UtilModel.notice = webClient.DownloadString(UtilModel.noticeUrl).Split(new char[] { '|' });
                            }

                            if (UtilModel.telegramChatId > 0)
                            {
                                using (TimeoutWebClient webClient = new TimeoutWebClient())
                                {
                                    webClient.Encoding = Encoding.UTF8;
                                    webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(UtilModel.betid + "님 로그인 완료하였습니다"));
                                }
                            }
                            loginHandler.LoginComplete(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, programVersion);
                        }
                        catch (Exception _ex)
                        {
                            logger.Error(_ex.ToString());
                        }

                        string userName = loginHandler.GetUserName(UtilModel._userprofile);
                        loginEventHandler(UtilModel._userprofile);
                        DialogResult = DialogResult.OK;

                        logger.Info("로그인여부 : " + login + " / " + username);
                    }
                    else if (login.Contains("not active"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("아직 계정 활성화가 되지 않았습니다. 관리자에게 문의 부탁드립니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (login.Contains("Failed"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("로그인에 실패하였습니다. 아이디와 비밀번호를 확인하여 입력해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        PasswordTextBox.Clear();
                    }
                    else if (login.Contains("Please enter your ID"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("아이디 입력이 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (login.Contains("Please enter a password"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("비밀번호 입력이 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (login.Contains("macAddress"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("처음 설치한 컴퓨터와 다른 컴퓨터에서 접속하였습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        logger.Error("로그인여부 : " + returnMessage + " / " + username + " / " + login);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show(returnMessage, "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        PasswordTextBox.Clear();
                    }

                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        private void CheckVersion()
        {
            var req = HttpWebRequest.CreateHttp(UtilModel.updateverion);

            using (var res = req.GetResponse())
            {
                using (var stream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        int remoteVersion = int.Parse(reader.ReadLine().TrimEnd());

                        if (UtilModel._programVersion < remoteVersion)
                        {
                            DownloadFiles();
                            MessageBox.Show("버젼이 업데이트 되었습니다." +
                                "\r\n\r\n확인을 누르시면 업데이트가 시작되어 프로그램이 종료됩니다.", "종료", MessageBoxButtons.OK);
                            System.Diagnostics.Process.Start("JYPowerBallProgram.exe");
                            Application.DoEvents();
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        private void DownloadFiles()

        {
            try
            {
                ReplaceData("JYPowerBallProgram.exe", "system.userdata");

                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    Uri updateFileURL = new Uri(UtilModel.updatefileurl);
                    webClient.DownloadFileAsync(updateFileURL, "JYPowerBallProgram.exe");
                }
                Application.DoEvents();
            }
            catch (WebException ex)
            {
                logger.Info(ex.Message);
            }
            catch (UriFormatException ex)
            {
                logger.Info(ex.Message);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
            }
        }
        ///

        ///Application업데이트

        ///

        ///

        ///

        ///

        private void ReplaceData(string setName, string changeName)
        {
            if (File.Exists(changeName))
            {
                File.Delete(changeName);
            }

            if (File.Exists(setName))
            {
                File.Move(setName, changeName);
            }
        }
        private bool ControlCheck()
        {
            if (String.IsNullOrEmpty(UsernameTextBox.Text))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UsernameTextBox.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PasswordTextBox.Focus();
                return false;
            }
            return true;
        }

        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }

        private void _programClosed_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void userRegist_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(UtilModel.userregist);
        }

        private void updateFileDownload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(UtilModel.updatefileurl);
            Environment.Exit(0);
        }
    }
}
