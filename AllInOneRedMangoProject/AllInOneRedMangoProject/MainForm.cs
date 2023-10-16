using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace AllInOneRedMangoProject
{
    public partial class MainForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        LoginForm loginForm;
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            loginForm = new LoginForm();
            loginForm.LoginEventHandler += new EventHandler(LoginSuccess);

            switch (loginForm.ShowDialog())
            {
                case DialogResult.OK:
                    loginForm.Close();
                    break;
                case DialogResult.Cancel:
                    Dispose();
                    break;
            }

            Game_CruiseBetMoneyPercentSettingComboBox.Text = "100";
            BetMoneySetting();
            Game_CruiseBetListView.DoubleBuffered(true);
            clearLevelSet.Text = "30";

            this.Text = "All In One RedMango Support Program For All || " + UtilModel.UserId + " || " + UtilModel._ip + " || " + UtilModel.ProgramVersion;

            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
        }
        private void LoginSuccess(string name)
        {
            MessageBox.Show(name + "님 반갑습니다.\r\n\r\n해당 프로그램은 고객님의 배팅에 \r\n\r\n도움을 주기 위해 만들어진 프로그램입니다. \r\n\r\n해당 프로그램을 맹신하지 말아주시기 바랍니다." +
                "\r\n\r\n프로그램 만료일 : " + UtilModel._limittime,
                            name + "님 반갑습니다.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
        }
        // 종료시 처리 자동으로 금액 저장
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("정말로 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    XMLModifierPropertiesSettings();
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        // <summary>
        // 수정 삭제하기
        // </summary>
        private void XMLModifierPropertiesSettings()
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("propertiesSettings.xml");

                // 첫노드를 잡아주고 하위 노드를 서냍ㄱ한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode("propertiesSettings");

                if (SubNode != null)
                {
                    XmlNode DeleteNode;

                    DeleteNode = SubNode.SelectSingleNode("id");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "id", UtilModel.UserId));

                    DeleteNode = SubNode.SelectSingleNode("UserSiteUrlAddress");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "UserSiteUrlAddress", UtilModel.UserSiteUrlAddress));

                    DeleteNode = SubNode.SelectSingleNode("samePerson");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "samePerson", UtilModel.SamePerson));

                    DeleteNode = SubNode.SelectSingleNode("resultMark");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "resultMark", UtilModel.ResultMark));

                    DeleteNode = SubNode.SelectSingleNode("errorBeep");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "errorBeep", UtilModel.ErrorBeep));

                    DeleteNode = SubNode.SelectSingleNode("bettingFail");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "bettingFail", UtilModel.BettingFail));

                    DeleteNode = SubNode.SelectSingleNode("patternBetNumber");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "patternBetNumber", UtilModel.PatternBetNumber));

                    DeleteNode = SubNode.SelectSingleNode("useAutoReverse");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "useAutoReverse", UtilModel.UseAutoReverse));

                    DeleteNode = SubNode.SelectSingleNode("useOverProfit");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "useOverProfit", UtilModel.UseOverProfit));

                    DeleteNode = SubNode.SelectSingleNode("OverProfitValue");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "OverProfitValue", UtilModel.OverProfitValue));

                    DeleteNode = SubNode.SelectSingleNode("useUnderProfit");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "useUnderProfit", UtilModel.UseUnderProfit));

                    DeleteNode = SubNode.SelectSingleNode("UnderProfitValue");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "UnderProfitValue", UtilModel.UnderProfitValue));
                    XmlDoc.Save("propertiesSettings.xml");
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        // <summary>
        // 자식노드 생성하고 값넣기
        // </summary>
        // <param name="xmlDoc">
        // <param name="name">
        // <param name="innerXml">
        // <return></returns>

        protected XmlNode CreateNode(XmlDocument xmlDoc, string name, string innerXml)
        {
            XmlNode node = xmlDoc.CreateElement(string.Empty, name, string.Empty);
            node.InnerXml = innerXml;

            return node;
        }

        private void Game_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BetMoneySetting();
        }

        private void BetMoneySetting()
        {
            Game_CruiseBetListView.Items.Clear();
            ListViewItem Game_CruiseBetListViewSubItem;
            double ValueSum = 0;
            string[] sarray = Game_CruiseBetMoneySettingTextBox.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int numberI = 1; numberI <= Game_CruiseBetMoneySettingTextBox.Lines.Length; numberI++)
            {
                if (ValueSum < 100000000)
                {
                    int.TryParse(Regex.Replace(sarray[numberI - 1], @"\D", ""), out int outValue);
                    outValue = (int)(outValue * int.Parse(Regex.Replace(Game_CruiseBetMoneyPercentSettingComboBox.Text, @"\D", "")) * 0.01);
                    ValueSum += outValue;
                    Game_CruiseBetListViewSubItem = new ListViewItem(numberI.ToString());
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged(outValue)); // 1차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95))); // 2차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95 * 1.95))); // 3차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)ValueSum)); // 총 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)((outValue * 1.95 * 1.95 * 1.95) - ValueSum))); // 당첨 이익금
                    Game_CruiseBetListView.Items.Add(Game_CruiseBetListViewSubItem);
                    UtilModel.Game_CruiseAllBetMoney[numberI, 0] = 0;
                    UtilModel.Game_CruiseAllBetMoney[numberI, 1] = outValue;
                    UtilModel.Game_CruiseAllBetMoney[numberI, 2] = (int)(outValue * 1.95);
                    UtilModel.Game_CruiseAllBetMoney[numberI, 3] = (int)(outValue * 1.95 * 1.95);
                }
                else
                {
                    break;
                }
            }
        }
    }
    public static class Extensions
    {
        public static void DoubleBuffered(this Control control, bool enabled)

        {
            var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, enabled, null);
        }
    }
}

