using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Xml.Linq;

namespace AutoMartinProgram
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
        private void loadInformation()
        {
            try
            {
                String _uri = "http://bit.ly/36RbbSu";
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("setting"))
                {
                    UtilModel.updateverion = item.Element("updateversion").Value;
                    UtilModel.logincheck = item.Element("logincheck").Value;
                    UtilModel.logincomplete = item.Element("logincomplete").Value;
                    UtilModel.loginfailed = item.Element("loginfailed").Value;
                    UtilModel.servertime = item.Element("servertime").Value;
                    UtilModel.picksterlist = item.Element("picksterlist").Value;
                    UtilModel.wonderfulurl = item.Element("wonderfulurl").Value;
                    UtilModel.rdwUrl = item.Element("rdwUrl").Value;
                    UtilModel.updateuserstatus = item.Element("updateuserstatus").Value;
                    UtilModel.powerresult = item.Element("powerresult").Value;
                    UtilModel.userregist = item.Element("userregist").Value;
                    UtilModel.noticeUrl = item.Element("notice").Value;
                };
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        int programVersion = 20200106;
        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                LoginHandler loginHandler = new LoginHandler();
                if (ControlCheck())
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    UtilModel._timetoken = Hash(timestamp);
                    String returnMessage = loginHandler.LoginCheck(UsernameTextBox.Text, PasswordTextBox.Text, textBoxMacAddress.Text, UtilModel._timetoken);
                    var login = "";
                    var username = "";
                    var userprofile = "";
                    try
                    {
                        JObject jo = JObject.Parse(returnMessage);
                        login = jo.SelectToken("login").ToString();
                        username = jo.SelectToken("username").ToString();
                        UtilModel._userSite = jo.SelectToken("usersite").ToString();
                        userprofile = jo.SelectToken("userprofile").ToString();
                        UtilModel._apikey = jo.SelectToken("apikey").ToString();
                        UtilModel._id = jo.SelectToken("betid").ToString();
                        UtilModel._password = jo.SelectToken("betpassword").ToString();
                        UtilModel._allBettingEnable = int.Parse(jo.SelectToken("allbettingEnable").ToString());
                        UtilModel._limittime = jo.SelectToken("limittime").ToString();
                        UtilModel._multiconnect = int.Parse(jo.SelectToken("multiconnect").ToString());
                    }
                    catch (Exception _ex)
                    {
                        logger.Error(_ex.ToString());
                    }

                    UtilModel._macAddress = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
                    UtilModel._ip = UtilModel.GetExternalIPAddress();
                    if (login.Contains("Complete"))
                    {
                        UtilModel._programVersion = programVersion;
                        CheckVersion();

                        string userName = loginHandler.GetUserName(userprofile);
                        loginEventHandler(userprofile);
                        DialogResult = DialogResult.OK;

                        loginHandler.LoginComplete(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, programVersion);
                        logger.Info("로그인여부 : " + login + " / " + username);
                    }
                    else if (login.Contains("not active"))
                    {
                        logger.Error("로그인여부 : " + login + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show("아직 계정 활성화가 되지 않았습니다. 관리자에게 문의 부탁드립니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Close();
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
                        Close();
                    }
                    else
                    {
                        logger.Error("로그인여부 : " + returnMessage + " / " + username);
                        loginHandler.LoginFailed(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, login, programVersion);
                        MessageBox.Show(returnMessage, "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //UsernameTextBox.Clear();
                        PasswordTextBox.Clear();
                    }

                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        private static void CheckVersion()
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
                            DialogResult result = MessageBox.Show("버젼이 업데이트 되었습니다." +
                                "\r\n\r\n현재 버젼이 이용 가능하나 되도록이면" +
                                "\r\n\r\n업데이트를 꼭 해주시길 부탁드립니다.", "종료", MessageBoxButtons.YesNo);
                            //if (result == DialogResult.Yes)
                            //{
                            //    _Update();
                            //}
                        }
                    }
                }
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

        private void LoginForm_Load(object sender, EventArgs e)
        {
            textBoxMacAddress.Text = Hash(NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString());
            textBoxIpAddress.Text = Hash(UtilModel.GetExternalIPAddress());
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
    }
}
