using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace PowerBallAutoMatinUserInformation
{
    public delegate void EventHandler(string id, string password, string name, string token);
    public partial class LoginForm : Form
    {
        public event EventHandler loginEventHandler;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LoginHandler loginHandler = new LoginHandler();
                if (ControlCheck())
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    String _timetoken = Hash(timestamp);
                    String returnMessage = loginHandler.LoginCheck(UsernameTextBox.Text, PasswordTextBox.Text, _timetoken);
                    var login = "";
                    var username = "";
                    var userprofile = "";
                    var is_admin = "";
                    try
                    {
                        JObject jo = JObject.Parse(returnMessage);

                        login = jo.SelectToken("login").ToString();
                        username = jo.SelectToken("username").ToString();
                        userprofile = jo.SelectToken("userprofile").ToString();
                        is_admin = jo.SelectToken("is_admin").ToString();
                    }
                    catch (Exception _ex)
                    {

                    }

                    //MessageBox.Show(MainForm._userSite + " / " + MainForm._apikey + " / " + MainForm._id + " / " + MainForm._password);

                    if (login.Contains("Complete"))
                    {
                        if (int.Parse(is_admin) >= 1)
                        {
                            string userName = loginHandler.GetUserName(userprofile);
                            loginEventHandler(UsernameTextBox.Text, PasswordTextBox.Text, userprofile, _timetoken);
                            DialogResult = DialogResult.OK;
                        }                        
                    }
                    else if (login.Contains("Failed"))
                    {
                        MessageBox.Show("로그인에 실패하였습니다. 아이디와 비밀번호를 확인하여 입력해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        PasswordTextBox.Clear();
                    }
                    else
                    {
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
    }
}
