using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using System.Xml;

namespace WaterMelonBettingProgram
{
    public delegate void EventHandler(string name);
    public partial class UserLogin : MetroFramework.Forms.MetroForm
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler loginEventHandler;
        public UserLogin()
        {
            InitializeComponent();
            XMLLoadPropertiesSettings();
        }

        private void loginCheck_Click(object sender, EventArgs e)
        {
            try
            {
                LoginHandler loginHandler = new LoginHandler();
                if (ControlCheck())
                {
                    String returnMessage = loginHandler.LoginCheck(programUrl.Text, programUserID.Text, programUserPassword.Text);
                    int code = 0;
                    JObject jObject;
                    try
                    {
                        jObject = JObject.Parse(returnMessage);
                        code = int.Parse(jObject.SelectToken("code").ToString());
                        if (code == 1)
                        {
                            UtilModel.ApiKey = jObject.SelectToken("more_info").SelectToken("key").ToString();
                            UtilModel.BettingUrlAddress = programUrl.Text;
                            UtilModel.UserId = programUserID.Text;
                            DialogResult = DialogResult.OK;
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
                        Environment.Exit(0);
                    }
                } else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        private void XMLLoadPropertiesSettings()
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("config.xml");

                // 첫노드를 잡아주고 하위 노드를 선택한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode("propertiesSettings");
                if (SubNode != null)
                {
                    XmlNode selectNode;

                    selectNode = SubNode.SelectSingleNode("site");
                    if (selectNode != null)
                    {
                        programUrl.Text = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("userid");
                    if (selectNode != null)
                    {
                        programUserID.Text = selectNode.InnerText;
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        private bool ControlCheck()
        {
            string url = "http://104.198.92.168/pickster/user_login.php";
            var rm = UtilModel.MakeAsyncRequest(url, "application/x-www-form-urlencoded; charset=UTF-8");
            if (rm.Result.Contains("Complete"))
            {
                return true;
            }
            return false;
        }
    }
}