using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PowerBallAutoMartinProgram
{
    public delegate void EventHandler(string name);
    public partial class LoginForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler LoginEventHandler;

        readonly int programVersion = 2021010401;
        public LoginForm()
        {
            InitializeComponent();
            LoadInformation();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            _programVersionLabel.Text = "Version | " + programVersion;
            UtilModel._programVersion = programVersion;

            LoadPropertiesSettings();

            if (CheckVersion())
            {
                programUpdatePanel.Visible = true;
            }

            DeleteServiceLogByDay(2);
        }
        string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }
        private void LoadPropertiesSettings()
        {
            if (!File.Exists("propertiesSettings.xml"))
            {
                try
                {
                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        Uri updateFileURL = new Uri(UtilModel.fileDownloadUrl + "propertiesSettings.xml");
                        webClient.DownloadFileAsync(updateFileURL, "propertiesSettings.xml");
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

            if (!File.Exists("settings.xml"))
            {
                try
                {
                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        Uri updateFileURL = new Uri(UtilModel.fileDownloadUrl + "settings.xml");
                        webClient.DownloadFileAsync(updateFileURL, "settings.xml");
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
            UtilModel.XMLLoadPropertiesSettings();
            UserUrlAddress.Text = UtilModel.UserSiteUrlAddress;
            UsernameTextBox.Text = UtilModel.UserId;
        }

        internal void DeleteServiceLogByDay(int keepDay)
        {
            try
            {
                DirectoryInfo screenShotFolder = new DirectoryInfo(Application.StartupPath + @"\screenshot");

                foreach (FileInfo file in screenShotFolder.GetFiles())
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-keepDay))
                    {
                        file.Delete();
                    }
                }

                DirectoryInfo logFolder = new DirectoryInfo(Application.StartupPath + @"\Log");

                foreach (FileInfo file in logFolder.GetFiles())
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-keepDay))
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private static void LoadInformation()
        {
            try
            {
                String _uri = "http://cherryapp.cafe24.com/Files/cherry/ServerSettingUrl.xml";
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
                    UtilModel.updateuserstatus = item.Element("updateuserstatus").Value;
                    UtilModel.powerresult = item.Element("powerBallResult").Value;
                    UtilModel.userregist = item.Element("userregist").Value;
                    UtilModel.noticeUrl = item.Element("notice").Value;
                    UtilModel.telegramChatUrl = item.Element("telegramChatUrl").Value;
                    UtilModel.loginVersion = item.Element("loginVersion").Value;
                    UtilModel.fileDownloadUrl = item.Element("fileDownloadUrl").Value;
                    UtilModel.configFileDownloadUrl = item.Element("configFileDownloadUrl").Value;
                    UtilModel.AuthSite = item.Element("authSite").Value;
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show("기본 설정을 불러오는데 실패하였습니다. 프로그램 종료 후 재실행하여 주시기 바랍니다..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.Error(_ex.ToString());
            }
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PasswordTextBox.Focus();
            }
        }
        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click();
            }
        }

        void btnLogin_Click()
        {
            try
            {
                LoginHandler loginHandler = new LoginHandler();
                if (ControlCheck())
                {
                    string timestamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

                    UtilModel.UserSiteUrlAddress = UserUrlAddress.Text.ToLower();

                    String returnMessage = loginHandler.BettingSiteUserLoginCheck(UserUrlAddress.Text.ToLower(), UsernameTextBox.Text, HttpUtility.UrlEncode(PasswordTextBox.Text), UtilModel.loginVersion);
                    int code = 0;
                    JObject jObject;
                    try
                    {
                        jObject = JObject.Parse(returnMessage);
                        code = int.Parse(jObject.SelectToken("code").ToString());
                        if (code == 1)
                        {
                            UtilModel.Bet_Api_Key = jObject.SelectToken("more_info").SelectToken("key").ToString();
                            UtilModel.UserProfile = jObject.SelectToken("more_info").SelectToken("user").SelectToken("nickname").ToString(); // 닉네임
                            UtilModel.UserOwnMoney = int.Parse(jObject.SelectToken("more_info").SelectToken("user").SelectToken("wallet").ToString()); // 보유금

                            UtilModel.UserSiteUrlAddress = UserUrlAddress.Text.ToLower();
                            UtilModel.UserId = UsernameTextBox.Text;

                            UtilModel._timetoken = Hash(timestamp);
                            UtilModel._ip = UtilModel.GetExternalIPAddress();
                            UtilModel._macAddress = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();

                            string loginInfo = loginHandler.LoginComplete(UsernameTextBox.Text, UtilModel._ip, UtilModel._macAddress, programVersion, UtilModel._timetoken, PasswordTextBox.Text, UtilModel.UserProfile, UtilModel.Bet_Api_Key, UtilModel.UserSiteUrlAddress);

                            jObject = JObject.Parse(loginInfo);
                            UtilModel.User_Telegram_Chat_Id = int.Parse(jObject.SelectToken("telegramChatId").ToString()); // 보유금
                            logger.Info(loginInfo);
                            if (UtilModel.User_Telegram_Chat_Id > 0)
                            {
                                StringBuilder sb = new StringBuilder();

                                using (TimeoutWebClient webClient = new TimeoutWebClient())
                                {
                                    webClient.Encoding = Encoding.UTF8;
                                    sb.Append(UtilModel.telegramChatUrl);
                                    sb.AppendFormat("?chatid={0}", UtilModel.User_Telegram_Chat_Id);
                                    sb.AppendFormat("&Message={0}", HttpUtility.UrlEncode("[" + UtilModel._ip + "][" + UtilModel.UserProfile + "님] 로그인 완료하였습니다"));
                                    webClient.DownloadString(sb.ToString());
                                }
                            }
                            string userName = loginHandler.GetUserName(UtilModel.UserProfile);
                            LoginEventHandler(UtilModel.UserProfile);
                            DialogResult = DialogResult.OK;

                            logger.Info("로그인여부 : " + UtilModel.UserProfile);
                        }
                        else if (code < 1)
                        {
                            MessageBox.Show(code + " | " + jObject.SelectToken("comment").ToString());
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception _ex)
                    {
                        MessageBox.Show("프로그램 로그인 도중 오류가 발생하였습니다. 프로그램 종료 후 재실행 부탁 드립니다.");
                    }
                    /*
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
                        UtilModel.PBGVisible = int.Parse(jo.SelectToken("pbgVisible").ToString());
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
                        UtilModel.UserId = UsernameTextBox.Text;

                            try
                            {
                                using (TimeoutWebClient webClient = new TimeoutWebClient())
                                {
                                    webClient.Encoding = Encoding.UTF8;
                                    UtilModel.notice = webClient.DownloadString(UtilModel.noticeUrl).Split(new char[] { '|' });
                                }

                                if (UtilModel.telegramChatId > 0)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                                    {
                                        webClient.Encoding = Encoding.UTF8;
                                        sb.Append(UtilModel.telegramChatUrl);
                                        sb.AppendFormat("?chatid={0}", UtilModel.telegramChatId);
                                        sb.AppendFormat("&Message={0}", HttpUtility.UrlEncode("[" + UtilModel._ip + "][" + UtilModel.betid + "님] 로그인 완료하였습니다"));
                                        webClient.DownloadString(sb.ToString());
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
                    */
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }
        private void OK_Click(object sender, EventArgs e)
        {
            btnLogin_Click();
        }

        private Boolean CheckVersion()
        {
            try
            {
                var req = HttpWebRequest.CreateHttp(UtilModel.fileDownloadUrl + "version.txt");

                using (var res = req.GetResponse())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            int remoteVersion = int.Parse(reader.ReadLine().TrimEnd());

                            if (UtilModel._programVersion < remoteVersion)
                            {
                                return true;
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        /// <summary>
        ///  Show the progress of the download in a progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // In case you don't have a progressBar Log the value instead 
            // Console.WriteLine(e.ProgressPercentage);
            fileDownLoadProgressBar.Value = e.ProgressPercentage;
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("The download has been cancelled");
                return;
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                MessageBox.Show("An error ocurred while trying to download file");

                return;
            }

            if (MessageBox.Show("프로그램 업데이트가 완료되었습니다. 확인 버튼을 눌러 주세요.", "Yes", MessageBoxButtons.OK) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("PowerBallAutoMartinProgram.exe");
                Application.DoEvents();
                Environment.Exit(0);
            }            
        }

        private void DownloadFiles()
        {
            try
            {
                ReplaceData("PowerBallAutoMartinProgram.exe", "system.userdata");

                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                    Uri updateFileURL = new Uri(UtilModel.fileDownloadUrl + "PowerBallAutoMartinProgram.exe");
                    webClient.DownloadFileAsync(updateFileURL, "PowerBallAutoMartinProgram.exe");
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

        private void agreeButton_Click(object sender, EventArgs e)
        {
            agreePanel.Visible = false;
        }

        private void programUpdateButton_Click(object sender, EventArgs e)
        {
            DownloadFiles();
        }
    }
}
