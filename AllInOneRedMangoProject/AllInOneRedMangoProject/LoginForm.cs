using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace AllInOneRedMangoProject
{
    public delegate void EventHandler(string name);
    public partial class LoginForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler LoginEventHandler;

        public LoginForm()
        {
            InitializeComponent();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadInformation();

            UtilModel.LoginSiteType = UtilModel.LoginSiteType_Royal;

            UtilModel.ProgramVersion = "2022102416";
            _programVersionLabel.Text = "Version | " + UtilModel.ProgramVersion;

            LoadPropertiesSettings();
            if (!CheckVersion())
            {
                programUpdatePanel.Visible = true;
            }
        }
        /*
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
        */
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
            UtilModel.XMLLoadPropertiesSettings();
            UserUrlAddress.Text = UtilModel.UserSiteUrlAddress;
            UsernameTextBox.Text = UtilModel.UserId;
        }

        /*********************************************
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
        
*****************************/
        static JToken jTokenCustomers;
        private static void LoadInformation()
        {
            JObject jObject = JObject.Parse(UtilModel.LoadInformation());
            UtilModel.UpdateVersion = jObject.SelectToken("setting.UpdateVersion").ToString();

            UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
            UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
            UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
            UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
            UtilModel.ResultServersMarukhan = jObject.SelectToken("ResultServers.marukhan").Value<bool>();
            jTokenCustomers = jObject["Customers"];
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                passwordTextBox.Focus();
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

                    Boolean siteIdB = false;
                    foreach (JToken members in jTokenCustomers)
                    {
                        if (UserUrlAddress.Text.ToLower().Contains(members["site"].ToString()) 
                            && UsernameTextBox.Text.Contains(members["id"].ToString()))
                        {
                            UtilModel.MAX_BET = int.Parse(members["maxbet"].ToString());
                            siteIdB = true;
                            break;
                        }
                    }
                    if (siteIdB == false)
                    {
                        MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                        return;
                    }

                    UtilModel.UserSiteUrlAddress = UserUrlAddress.Text.ToLower();
                    UtilModel.UserId = UsernameTextBox.Text;

                    StringBuilder stringBuilder = new StringBuilder();
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click || UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        stringBuilder.AppendFormat("{0}:{1}/auto/api/user_auth?u={2}&p={3}&v={4}", UserUrlAddress.Text.ToLower(), UtilModel.SitePort, UsernameTextBox.Text, HttpUtility.UrlEncode(passwordTextBox.Text), UtilModel.ServerProgramVersion);
                        String returnMessage = loginHandler.BettingSiteUserLoginCheck(stringBuilder.ToString());
                        logger.Info(returnMessage);
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

                                //UtilModel._timetoken = Hash(timestamp);
                                UtilModel._ip = UtilModel.GetExternalIPAddress();
                                //UtilModel._macAddress = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
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
                            MessageBox.Show(_ex + "\r\n" + "프로그램 로그인 도중 오류가 발생하였습니다. 프로그램 종료 후 재실행 부탁 드립니다.");
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        stringBuilder.Append(UserUrlAddress.Text.ToLower());
                        stringBuilder.Append("/game/auto.asp");
                        stringBuilder.AppendFormat("?id={0}&pw={1}", UsernameTextBox.Text, passwordTextBox.Text);
                        String returnMessage = loginHandler.BettingSiteUserLoginCheck(stringBuilder.ToString());
                        logger.Info(UtilModel.UnicodeToString(returnMessage));
                        int code = 0;
                        JObject jObject;
                        try
                        {
                            jObject = JObject.Parse(returnMessage);
                            code = int.Parse(jObject.SelectToken("err").ToString());
                            if (code == 0)
                            {
                                UtilModel.Bet_Api_Key = jObject.SelectToken("data.ApiKey").ToString();
                                UtilModel.UserProfile = UsernameTextBox.Text;
                                UtilModel.UserOwnMoney = int.Parse(jObject.SelectToken("data.GameMoney").ToString()); // 보유금
                                UtilModel._ip = UtilModel.GetExternalIPAddress();

                                string userName = loginHandler.GetUserName(UtilModel.UserProfile);
                                LoginEventHandler(UtilModel.UserProfile);
                                DialogResult = DialogResult.OK;

                                logger.Info("로그인여부 : " + UtilModel.UserProfile);
                            }
                            else if (code > 0)
                            {
                                MessageBox.Show(code + " | " + jObject.SelectToken("msg").ToString());
                            }
                            else
                            {
                                Environment.Exit(0);
                            }
                        }
                        catch (Exception _ex)
                        {
                            MessageBox.Show(_ex + "\r\n" + "프로그램 로그인 도중 오류가 발생하였습니다. 프로그램 종료 후 재실행 부탁 드립니다.");
                        }
                    }
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
            if (int.Parse(UtilModel.ProgramVersion) >= int.Parse(UtilModel.UpdateVersion))
            {
                return true;
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
                System.Diagnostics.Process.Start("AllInOneRedMangoProject.exe");
                Application.DoEvents();
                Environment.Exit(0);
            }
        }

        private void DownloadFiles()
        {
            try
            {
                ReplaceData("AllInOneRedMangoProject.exe", "system.userdata");

                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                    Uri updateFileURL = new Uri("http://say4m.cafe24.com/AllInOneRedMangoProject.exe");
                    webClient.DownloadFileAsync(updateFileURL, "AllInOneRedMangoProject.exe");
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
            else if (String.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
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
        public string SHA256Hash(string data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }

        private void programClosed_Click(object sender, EventArgs e)
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

        private void RoyalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UtilModel.LoginSiteType = UtilModel.LoginSiteType_Royal;
        }

        private void ClickRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UtilModel.SitePort = "8085";
            UtilModel.ServerProgramVersion = "1.4.6";
            UtilModel.LoginSiteType = UtilModel.LoginSiteType_Click;
        }

        private void LifeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UtilModel.SitePort = "8087";
            UtilModel.ServerProgramVersion = "2.2.2";
            UtilModel.LoginSiteType = UtilModel.LoginSiteType_Life;
        }
    }
}