using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace RedMango_PowerBall_Support_Program
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
            BetMoneySetting(Game1_CruiseBetListView, Game1_CruiseBetMoneySettingTextBox, Game_CruiseBetMoneyPercentSettingComboBox, Game_1_CruiseAllBetMoney);
            Game1_CruiseBetListView.DoubleBuffered(true);
            Game1_CruiseBetRegistListView.DoubleBuffered(true);
            Game1_Clear_Level_50.DoubleBuffered(true);

            BetMoneySetting(Game3_CruiseBetListView, Game3_CruiseBetMoneySettingTextBox, Game_CruiseBetMoneyPercentSettingComboBox, Game_3_CruiseAllBetMoney);
            Game3_CruiseBetListView.DoubleBuffered(true);
            Game3_CruiseBetRegistListView.DoubleBuffered(true);
            Game3_Clear_Level_50.DoubleBuffered(true);

            BetMoneySetting(Game5_CruiseBetListView, Game5_CruiseBetMoneySettingTextBox, Game_CruiseBetMoneyPercentSettingComboBox, Game_5_CruiseAllBetMoney);
            Game5_CruiseBetListView.DoubleBuffered(true);
            Game5_CruiseBetRegistListView.DoubleBuffered(true);
            Game5_Clear_Level_50.DoubleBuffered(true);

            BetMoneySetting(Game6_CruiseBetListView, Game6_CruiseBetMoneySettingTextBox, Game_CruiseBetMoneyPercentSettingComboBox, Game_6_CruiseAllBetMoney);
            Game6_CruiseBetListView.DoubleBuffered(true);
            Game6_CruiseBetRegistListView.DoubleBuffered(true);
            Game6_Clear_Level_50.DoubleBuffered(true);

            Game2_CruiseBetMoneyPercentSettingComboBox.Text = "100";
            BetMoneySetting(Game2_CruiseBetListView, Game2_CruiseBetMoneySettingTextBox, Game2_CruiseBetMoneyPercentSettingComboBox, Game_2_CruiseAllBetMoney);
            Game2_CruiseBetListView.DoubleBuffered(true);
            Game2_CruiseBetRegistListView.DoubleBuffered(true);
            Game2_Clear_Level_50.DoubleBuffered(true);

            clearLevelSet.Text = "30";
            profitLimitComboBox.Text = "0";

            this.Text = "RedMango Support Program For All || " + UtilModel.UserId + " || " + UtilModel._ip + " || " + UtilModel.ProgramVersion;

            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
            Game1_RemainingTimer.Interval = 1000;
            Game1_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game1_Timer_Elapsed);
            /*****************************************************************************
            Game2_RemainingTimer.Interval = 1000;
            Game2_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game2_Timer_Elapsed);
            *****************************************************************************/
            Game3_RemainingTimer.Interval = 1000;
            Game3_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game3_Timer_Elapsed);
            /*****************************************************************************
            Game4_RemainingTimer.Interval = 1000;
            Game4_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game4_Timer_Elapsed);
            *****************************************************************************/
            Game5_RemainingTimer.Interval = 1000;
            Game5_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game5_Timer_Elapsed);

            Game6_RemainingTimer.Interval = 1000;
            Game6_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game6_Timer_Elapsed);
        }
        private void BetMoneySetting(ListView BetListView, TextBox BetMoneySetting, ComboBox BetMoneyPercentSetting, double[,] CruiseAllBetMoney)
        {
            BetListView.Items.Clear();
            ListViewItem Game_CruiseBetListViewSubItem;
            double ValueSum = 0;
            string[] sarray = BetMoneySetting.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int numberI = 1; numberI <= BetMoneySetting.Lines.Length; numberI++)
            {
                if (ValueSum < 100000000)
                {
                    int.TryParse(Regex.Replace(sarray[numberI - 1], @"\D", ""), out int outValue);
                    outValue = (int)(outValue * int.Parse(Regex.Replace(BetMoneyPercentSetting.Text, @"\D", "")) * 0.01);
                    ValueSum += outValue;
                    Game_CruiseBetListViewSubItem = new ListViewItem(numberI.ToString());
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged(outValue)); // 1차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95))); // 2차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95 * 1.95))); // 3차 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)ValueSum)); // 총 배팅금
                    Game_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)((outValue * 1.95 * 1.95 * 1.95) - ValueSum))); // 당첨 이익금
                    BetListView.Items.Add(Game_CruiseBetListViewSubItem);

                    CruiseAllBetMoney[numberI, 0] = outValue;
                    CruiseAllBetMoney[numberI, 1] = outValue * 1.95;
                    CruiseAllBetMoney[numberI, 2] = outValue * 1.95 * 1.95;
                }
                else
                {
                    break;
                }
            }
        }

        private void EosPowerBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game1_PowerBallOddEvenUseCheck.Checked)
            {
                Game1_CruisePowerBallOddEvenBetListLevel1.ReadOnly = true;
                Game1_CruisePowerBallOddEvenBetListLevel2.ReadOnly = true;
                Game1_CruisePowerBallOddEvenBetListLevel3.ReadOnly = true;
                Game1_CruisePowerBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game1_CruisePowerBallOddEvenBetListLevel1.ReadOnly = false;
                Game1_CruisePowerBallOddEvenBetListLevel2.ReadOnly = false;
                Game1_CruisePowerBallOddEvenBetListLevel3.ReadOnly = false;
                Game1_CruisePowerBallOddEvenLevelChange.Text = "0";
            }
            Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game1_CruisePowerBallOddEvenBetPickLevel1.Text = "통";
            Game1_CruisePowerBallOddEvenBetPickLevel2.Text = "통";
            Game1_CruisePowerBallOddEvenBetPickLevel3.Text = "통";
        }

        private void EosPowerBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game1_PowerBallUnderOverUseCheck.Checked)
            {
                Game1_CruisePowerBallUnderOverBetListLevel1.ReadOnly = true;
                Game1_CruisePowerBallUnderOverBetListLevel2.ReadOnly = true;
                Game1_CruisePowerBallUnderOverBetListLevel3.ReadOnly = true;
                Game1_CruisePowerBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game1_CruisePowerBallUnderOverBetListLevel1.ReadOnly = false;
                Game1_CruisePowerBallUnderOverBetListLevel2.ReadOnly = false;
                Game1_CruisePowerBallUnderOverBetListLevel3.ReadOnly = false;
                Game1_CruisePowerBallUnderOverLevelChange.Text = "0";
            }
            Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game1_CruisePowerBallUnderOverBetPickLevel1.Text = "통";
            Game1_CruisePowerBallUnderOverBetPickLevel2.Text = "통";
            Game1_CruisePowerBallUnderOverBetPickLevel3.Text = "통";

        }

        private void EosNormalBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game1_NormalBallOddEvenUseCheck.Checked)
            {
                Game1_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = true;
                Game1_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = true;
                Game1_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = true;
                Game1_CruiseNormalBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game1_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = false;
                Game1_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = false;
                Game1_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = false;
                Game1_CruiseNormalBallOddEvenLevelChange.Text = "0";
            }
            Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game1_CruiseNormalBallOddEvenBetPickLevel1.Text = "통";
            Game1_CruiseNormalBallOddEvenBetPickLevel2.Text = "통";
            Game1_CruiseNormalBallOddEvenBetPickLevel3.Text = "통";
        }

        private void EossNormalBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game1_NormalBallUnderOverUseCheck.Checked)
            {
                Game1_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = true;
                Game1_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = true;
                Game1_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = true;
                Game1_CruiseNormalBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game1_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = false;
                Game1_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = false;
                Game1_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = false;
                Game1_CruiseNormalBallUnderOverLevelChange.Text = "0";
            }
            Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game1_CruiseNormalBallUnderOverBetPickLevel1.Text = "통";
            Game1_CruiseNormalBallUnderOverBetPickLevel2.Text = "통";
            Game1_CruiseNormalBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game3_PowerBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game3_PowerBallOddEvenUseCheck.Checked)
            {
                Game3_CruisePowerBallOddEvenBetListLevel1.ReadOnly = true;
                Game3_CruisePowerBallOddEvenBetListLevel2.ReadOnly = true;
                Game3_CruisePowerBallOddEvenBetListLevel3.ReadOnly = true;
                Game3_CruisePowerBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game3_CruisePowerBallOddEvenBetListLevel1.ReadOnly = false;
                Game3_CruisePowerBallOddEvenBetListLevel2.ReadOnly = false;
                Game3_CruisePowerBallOddEvenBetListLevel3.ReadOnly = false;
                Game3_CruisePowerBallOddEvenLevelChange.Text = "0";
            }
            Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game3_CruisePowerBallOddEvenBetPickLevel1.Text = "통";
            Game3_CruisePowerBallOddEvenBetPickLevel2.Text = "통";
            Game3_CruisePowerBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game3_PowerBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game3_PowerBallUnderOverUseCheck.Checked)
            {
                Game3_CruisePowerBallUnderOverBetListLevel1.ReadOnly = true;
                Game3_CruisePowerBallUnderOverBetListLevel2.ReadOnly = true;
                Game3_CruisePowerBallUnderOverBetListLevel3.ReadOnly = true;
                Game3_CruisePowerBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game3_CruisePowerBallUnderOverBetListLevel1.ReadOnly = false;
                Game3_CruisePowerBallUnderOverBetListLevel2.ReadOnly = false;
                Game3_CruisePowerBallUnderOverBetListLevel3.ReadOnly = false;
                Game3_CruisePowerBallUnderOverLevelChange.Text = "0";
            }
            Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game3_CruisePowerBallUnderOverBetPickLevel1.Text = "통";
            Game3_CruisePowerBallUnderOverBetPickLevel2.Text = "통";
            Game3_CruisePowerBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game3_NormalBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game3_NormalBallOddEvenUseCheck.Checked)
            {
                Game3_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = true;
                Game3_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = true;
                Game3_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = true;
                Game3_CruiseNormalBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game3_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = false;
                Game3_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = false;
                Game3_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = false;
                Game3_CruiseNormalBallOddEvenLevelChange.Text = "0";
            }
            Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game3_CruiseNormalBallOddEvenBetPickLevel1.Text = "통";
            Game3_CruiseNormalBallOddEvenBetPickLevel2.Text = "통";
            Game3_CruiseNormalBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game3_NormalBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game3_NormalBallUnderOverUseCheck.Checked)
            {
                Game3_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = true;
                Game3_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = true;
                Game3_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = true;
                Game3_CruiseNormalBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game3_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = false;
                Game3_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = false;
                Game3_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = false;
                Game3_CruiseNormalBallUnderOverLevelChange.Text = "0";
            }
            Game3_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game3_CruiseNormalBallUnderOverBetPickLevel1.Text = "통";
            Game3_CruiseNormalBallUnderOverBetPickLevel2.Text = "통";
            Game3_CruiseNormalBallUnderOverBetPickLevel3.Text = "통";
        }


        private void Game5_PowerBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game5_PowerBallOddEvenUseCheck.Checked)
            {
                Game5_CruisePowerBallOddEvenBetListLevel1.ReadOnly = true;
                Game5_CruisePowerBallOddEvenBetListLevel2.ReadOnly = true;
                Game5_CruisePowerBallOddEvenBetListLevel3.ReadOnly = true;
                Game5_CruisePowerBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game5_CruisePowerBallOddEvenBetListLevel1.ReadOnly = false;
                Game5_CruisePowerBallOddEvenBetListLevel2.ReadOnly = false;
                Game5_CruisePowerBallOddEvenBetListLevel3.ReadOnly = false;
                Game5_CruisePowerBallOddEvenLevelChange.Text = "0";
            }
            Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game5_CruisePowerBallOddEvenBetPickLevel1.Text = "통";
            Game5_CruisePowerBallOddEvenBetPickLevel2.Text = "통";
            Game5_CruisePowerBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game5_PowerBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game5_PowerBallUnderOverUseCheck.Checked)
            {
                Game5_CruisePowerBallUnderOverBetListLevel1.ReadOnly = true;
                Game5_CruisePowerBallUnderOverBetListLevel2.ReadOnly = true;
                Game5_CruisePowerBallUnderOverBetListLevel3.ReadOnly = true;
                Game5_CruisePowerBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game5_CruisePowerBallUnderOverBetListLevel1.ReadOnly = false;
                Game5_CruisePowerBallUnderOverBetListLevel2.ReadOnly = false;
                Game5_CruisePowerBallUnderOverBetListLevel3.ReadOnly = false;
                Game5_CruisePowerBallUnderOverLevelChange.Text = "0";
            }
            Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game5_CruisePowerBallUnderOverBetPickLevel1.Text = "통";
            Game5_CruisePowerBallUnderOverBetPickLevel2.Text = "통";
            Game5_CruisePowerBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game5_NormalBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game5_NormalBallOddEvenUseCheck.Checked)
            {
                Game5_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = true;
                Game5_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = true;
                Game5_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = true;
                Game5_CruiseNormalBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game5_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = false;
                Game5_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = false;
                Game5_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = false;
                Game5_CruiseNormalBallOddEvenLevelChange.Text = "0";
            }
            Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game5_CruiseNormalBallOddEvenBetPickLevel1.Text = "통";
            Game5_CruiseNormalBallOddEvenBetPickLevel2.Text = "통";
            Game5_CruiseNormalBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game5_NormalBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game5_NormalBallUnderOverUseCheck.Checked)
            {
                Game5_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = true;
                Game5_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = true;
                Game5_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = true;
                Game5_CruiseNormalBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game5_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = false;
                Game5_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = false;
                Game5_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = false;
                Game5_CruiseNormalBallUnderOverLevelChange.Text = "0";
            }
            Game5_CruiseBetNormalBallUnderOverSubLevel = 1;
            Game5_CruiseNormalBallUnderOverBetPickLevel1.Text = "통";
            Game5_CruiseNormalBallUnderOverBetPickLevel2.Text = "통";
            Game5_CruiseNormalBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game6_PowerBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game6_PowerBallOddEvenUseCheck.Checked)
            {
                Game6_CruisePowerBallOddEvenBetListLevel1.ReadOnly = true;
                Game6_CruisePowerBallOddEvenBetListLevel2.ReadOnly = true;
                Game6_CruisePowerBallOddEvenBetListLevel3.ReadOnly = true;
                Game6_CruisePowerBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game6_CruisePowerBallOddEvenBetListLevel1.ReadOnly = false;
                Game6_CruisePowerBallOddEvenBetListLevel2.ReadOnly = false;
                Game6_CruisePowerBallOddEvenBetListLevel3.ReadOnly = false;
                Game6_CruisePowerBallOddEvenLevelChange.Text = "0";
            }
            Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game6_CruisePowerBallOddEvenBetPickLevel1.Text = "통";
            Game6_CruisePowerBallOddEvenBetPickLevel2.Text = "통";
            Game6_CruisePowerBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game6_PowerBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game6_PowerBallUnderOverUseCheck.Checked)
            {
                Game6_CruisePowerBallUnderOverBetListLevel1.ReadOnly = true;
                Game6_CruisePowerBallUnderOverBetListLevel2.ReadOnly = true;
                Game6_CruisePowerBallUnderOverBetListLevel3.ReadOnly = true;
                Game6_CruisePowerBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game6_CruisePowerBallUnderOverBetListLevel1.ReadOnly = false;
                Game6_CruisePowerBallUnderOverBetListLevel2.ReadOnly = false;
                Game6_CruisePowerBallUnderOverBetListLevel3.ReadOnly = false;
                Game6_CruisePowerBallUnderOverLevelChange.Text = "0";
            }
            Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game6_CruisePowerBallUnderOverBetPickLevel1.Text = "통";
            Game6_CruisePowerBallUnderOverBetPickLevel2.Text = "통";
            Game6_CruisePowerBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game6_NormalBallOddEvenUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game6_NormalBallOddEvenUseCheck.Checked)
            {
                Game6_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = true;
                Game6_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = true;
                Game6_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = true;
                Game6_CruiseNormalBallOddEvenLevelChange.Text = "1";
            }
            else
            {
                Game6_CruiseNormalBallOddEvenBetListLevel1.ReadOnly = false;
                Game6_CruiseNormalBallOddEvenBetListLevel2.ReadOnly = false;
                Game6_CruiseNormalBallOddEvenBetListLevel3.ReadOnly = false;
                Game6_CruiseNormalBallOddEvenLevelChange.Text = "0";
            }
            Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game6_CruiseNormalBallOddEvenBetPickLevel1.Text = "통";
            Game6_CruiseNormalBallOddEvenBetPickLevel2.Text = "통";
            Game6_CruiseNormalBallOddEvenBetPickLevel3.Text = "통";
        }

        private void Game6_NormalBallUnderOverUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game6_NormalBallUnderOverUseCheck.Checked)
            {
                Game6_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = true;
                Game6_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = true;
                Game6_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = true;
                Game6_CruiseNormalBallUnderOverLevelChange.Text = "1";
            }
            else
            {
                Game6_CruiseNormalBallUnderOverBetListLevel1.ReadOnly = false;
                Game6_CruiseNormalBallUnderOverBetListLevel2.ReadOnly = false;
                Game6_CruiseNormalBallUnderOverBetListLevel3.ReadOnly = false;
                Game6_CruiseNormalBallUnderOverLevelChange.Text = "0";
            }
            Game6_CruiseBetNormalBallUnderOverSubLevel = 1;
            Game6_CruiseNormalBallUnderOverBetPickLevel1.Text = "통";
            Game6_CruiseNormalBallUnderOverBetPickLevel2.Text = "통";
            Game6_CruiseNormalBallUnderOverBetPickLevel3.Text = "통";
        }

        private void Game2_FirstUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game2_FirstUseCheck.Checked)
            {
                Game2_FirstBetListLevel1.ReadOnly = true;
                Game2_FirstBetListLevel2.ReadOnly = true;
                Game2_FirstBetListLevel3.ReadOnly = true;
                Game2_FirstLevelChange.Text = "1";
            }
            else
            {
                Game2_FirstBetListLevel1.ReadOnly = false;
                Game2_FirstBetListLevel2.ReadOnly = false;
                Game2_FirstBetListLevel3.ReadOnly = false;
                Game2_FirstLevelChange.Text = "0";
            }
            Game2_FirstBetSubLevel = 1;

            Game2_FirstBetPickLevel1.Text = "통";
            Game2_FirstBetPickLevel2.Text = "통";
            Game2_FirstBetPickLevel3.Text = "통";
        }

        private void Game2_SecondUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game2_SecondUseCheck.Checked)
            {
                Game2_SecondBetListLevel1.ReadOnly = true;
                Game2_SecondBetListLevel2.ReadOnly = true;
                Game2_SecondBetListLevel3.ReadOnly = true;
                Game2_SecondLevelChange.Text = "1";
            }
            else
            {
                Game2_SecondBetListLevel1.ReadOnly = false;
                Game2_SecondBetListLevel2.ReadOnly = false;
                Game2_SecondBetListLevel3.ReadOnly = false;
                Game2_SecondLevelChange.Text = "0";
            }
            Game2_SecondBetSubLevel = 1;

            Game2_SecondBetPickLevel1.Text = "통";
            Game2_SecondBetPickLevel2.Text = "통";
            Game2_SecondBetPickLevel3.Text = "통";
        }

        private void Game2_ThirdUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Game2_ThirdUseCheck.Checked)
            {
                Game2_ThirdBetListLevel1.ReadOnly = true;
                Game2_ThirdBetListLevel2.ReadOnly = true;
                Game2_ThirdBetListLevel3.ReadOnly = true;
                Game2_ThirdLevelChange.Text = "1";
            }
            else
            {
                Game2_ThirdBetListLevel1.ReadOnly = false;
                Game2_ThirdBetListLevel2.ReadOnly = false;
                Game2_ThirdBetListLevel3.ReadOnly = false;
                Game2_ThirdLevelChange.Text = "0";
            }
            Game2_ThirdBetSubLevel = 1;

            Game2_ThirdBetPickLevel1.Text = "통";
            Game2_ThirdBetPickLevel2.Text = "통";
            Game2_ThirdBetPickLevel3.Text = "통";
        }

        private void Game1_CruisePowerBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text), 0]);
            Game1_CruisePowerBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text), 1]);
            Game1_CruisePowerBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text), 2]);
        }

        private void Game1_CruisePowerBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text), 0]);
            Game1_CruisePowerBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text), 1]);
            Game1_CruisePowerBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text), 2]);
        }

        private void Game1_CruiseNormalBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game1_CruiseNormalBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text), 0]);
            Game1_CruiseNormalBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text), 1]);
            Game1_CruiseNormalBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text), 2]);
        }

        private void Game1_CruiseNormalBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game1_CruiseNormalBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text), 0]);
            Game1_CruiseNormalBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text), 1]);
            Game1_CruiseNormalBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_1_CruiseAllBetMoney[int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text), 2]);
        }



        private void Game2_FirstLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game2_FirstBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_FirstLevelChange.Text), 0]);
            Game2_FirstBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_FirstLevelChange.Text), 1]);
            Game2_FirstBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_FirstLevelChange.Text), 2]);
        }

        private void Game2_SecondLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game2_SecondBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_SecondLevelChange.Text), 0]);
            Game2_SecondBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_SecondLevelChange.Text), 1]);
            Game2_SecondBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_SecondLevelChange.Text), 2]);
        }

        private void Game2_ThirdLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game2_ThirdBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_ThirdLevelChange.Text), 0]);
            Game2_ThirdBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_ThirdLevelChange.Text), 1]);
            Game2_ThirdBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_2_CruiseAllBetMoney[int.Parse(Game2_ThirdLevelChange.Text), 2]);
        }

        private void Game3_CruisePowerBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text), 0]);
            Game3_CruisePowerBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text), 1]);
            Game3_CruisePowerBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text), 2]);
        }

        private void Game3_CruisePowerBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text), 0]);
            Game3_CruisePowerBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text), 1]);
            Game3_CruisePowerBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text), 2]);
        }

        private void Game3_CruiseNormalBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game3_CruiseNormalBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text), 0]);
            Game3_CruiseNormalBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text), 1]);
            Game3_CruiseNormalBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text), 2]);
        }

        private void Game3_CruiseNormalBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game3_CruiseNormalBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text), 0]);
            Game3_CruiseNormalBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text), 1]);
            Game3_CruiseNormalBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_3_CruiseAllBetMoney[int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text), 2]);
        }
        private void Game5_CruisePowerBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text), 0]);
            Game5_CruisePowerBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text), 1]);
            Game5_CruisePowerBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text), 2]);
        }

        private void Game5_CruisePowerBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text), 0]);
            Game5_CruisePowerBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text), 1]);
            Game5_CruisePowerBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text), 2]);
        }

        private void Game5_CruiseNormalBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game5_CruiseNormalBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text), 0]);
            Game5_CruiseNormalBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text), 1]);
            Game5_CruiseNormalBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text), 2]);
        }

        private void Game5_CruiseNormalBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game5_CruiseNormalBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text), 0]);
            Game5_CruiseNormalBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text), 1]);
            Game5_CruiseNormalBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_5_CruiseAllBetMoney[int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text), 2]);
        }

        private void Game6_CruisePowerBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text), 0]);
            Game6_CruisePowerBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text), 1]);
            Game6_CruisePowerBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text), 2]);
        }

        private void Game6_CruisePowerBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text), 0]);
            Game6_CruisePowerBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text), 1]);
            Game6_CruisePowerBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text), 2]);
        }

        private void Game6_CruiseNormalBallOddEvenLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game6_CruiseNormalBallOddEvenBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text), 0]);
            Game6_CruiseNormalBallOddEvenBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text), 1]);
            Game6_CruiseNormalBallOddEvenBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text), 2]);
        }

        private void Game6_CruiseNormalBallUnderOverLevelChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game6_CruiseNormalBallUnderOverBetMoneyLevel1.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text), 0]);
            Game6_CruiseNormalBallUnderOverBetMoneyLevel2.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text), 1]);
            Game6_CruiseNormalBallUnderOverBetMoneyLevel3.Text = UtilModel.StringFormatChanged((int)Game_6_CruiseAllBetMoney[int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text), 2]);
        }
        private void Game_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (UtilModel.MAX_BET < int.Parse(combo.Text))
            {
                combo.Text = UtilModel.MAX_BET.ToString();
                return;
            }
            BetMoneySetting(Game1_CruiseBetListView, Game1_CruiseBetMoneySettingTextBox, combo, Game_1_CruiseAllBetMoney);
            BetMoneySetting(Game3_CruiseBetListView, Game3_CruiseBetMoneySettingTextBox, combo, Game_3_CruiseAllBetMoney);
            BetMoneySetting(Game5_CruiseBetListView, Game5_CruiseBetMoneySettingTextBox, combo, Game_5_CruiseAllBetMoney);
            BetMoneySetting(Game6_CruiseBetListView, Game6_CruiseBetMoneySettingTextBox, combo, Game_6_CruiseAllBetMoney);
        }

        private void Game2_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BetMoneySetting(Game2_CruiseBetListView, Game2_CruiseBetMoneySettingTextBox, Game2_CruiseBetMoneyPercentSettingComboBox, Game_2_CruiseAllBetMoney);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_1 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_1 = Click_GameCodePowerBall[GameNumber_1];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_1 = Life_GameCodePowerBall[GameNumber_1];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_1 = Royal_GameCodePowerBall[GameNumber_1];
            }
            Game1_CruiseBetRegistListView.Items.Clear();

            Game1_PowerBallOddEvenUseCheck.Checked = true;
            Game1_PowerBallUnderOverUseCheck.Checked = true;
            Game1_NormalBallOddEvenUseCheck.Checked = true;
            Game1_NormalBallUnderOverUseCheck.Checked = true;
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_2 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_2 = Click_GameCodeLadder[GameNumber_2];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_2 = Life_GameCodeLadder[GameNumber_2];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_2 = Royal_GameCodeLadder[GameNumber_2];
            }
            Game2_CruiseBetRegistListView.Items.Clear();

            Game2_FirstUseCheck.Checked = true;
            Game2_SecondUseCheck.Checked = true;
            Game2_ThirdUseCheck.Checked = true;
        }

        private void Game3_GameSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_3 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_3 = Click_GameCodePowerBall[GameNumber_3];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_3 = Life_GameCodePowerBall[GameNumber_3];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_3 = Royal_GameCodePowerBall[GameNumber_3];
            }
            Game3_CruiseBetRegistListView.Items.Clear();

            Game3_PowerBallOddEvenUseCheck.Checked = true;
            Game3_PowerBallUnderOverUseCheck.Checked = true;
            Game3_NormalBallOddEvenUseCheck.Checked = true;
            Game3_NormalBallUnderOverUseCheck.Checked = true;
        }

        private void Game4_GameSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_4 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_4 = Click_GameCodeLadder[GameNumber_4];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_4 = Life_GameCodeLadder[GameNumber_4];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_4 = Royal_GameCodeLadder[GameNumber_4];
            }
            Game4_CruiseBetRegistListView.Items.Clear();
        }
        private void Game5_GameSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_5 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_5 = Click_GameCodePowerBall[GameNumber_5];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_5 = Life_GameCodePowerBall[GameNumber_5];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_5 = Royal_GameCodePowerBall[GameNumber_5];
            }
            Game5_CruiseBetRegistListView.Items.Clear();

            Game5_PowerBallOddEvenUseCheck.Checked = true;
            Game5_PowerBallUnderOverUseCheck.Checked = true;
            Game5_NormalBallOddEvenUseCheck.Checked = true;
            Game5_NormalBallUnderOverUseCheck.Checked = true;
        }

        private void Game6_GameSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_6 = outValue - 1;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode_6 = Click_GameCodePowerBall[GameNumber_6];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode_6 = Life_GameCodePowerBall[GameNumber_6];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode_6 = Royal_GameCodePowerBall[GameNumber_6];
            }
            Game6_CruiseBetRegistListView.Items.Clear();

            Game6_PowerBallOddEvenUseCheck.Checked = true;
            Game6_PowerBallUnderOverUseCheck.Checked = true;
            Game6_NormalBallOddEvenUseCheck.Checked = true;
            Game6_NormalBallUnderOverUseCheck.Checked = true;
        }
        int Random_Nonce_1;
        int Random_Nonce_2;
        int Random_Nonce_3;
        int Random_Nonce_4;
        int Random_Nonce_5;
        int Random_Nonce_6;

        private void Game1Stop(Button button)
        {
            button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
            button.Text = "배팅이 정지되었습니다.";
            StartGame_1 = false;

            Game1_Result_1 = string.Empty;
            Game1_Result_2 = string.Empty;
            Game1_Result_3 = string.Empty;
            Game1_Result_4 = string.Empty;

            Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game1_Betting_Mode_Result_Process = false;

            Game_1_Betting_Complete_Status = false;

            Game_1_Result_Load_Complete = false;

            Game_1_Check_Complete = false;

            Game1_BetMoney = new int[] { 0, 0, 0, 0 };

            Game1_BetPick = new string[] { null, null, null, null };

            Game1_CruisePowerBallOddEvenLevelChange.Text = "1";
            Game1_CruisePowerBallUnderOverLevelChange.Text = "1";
            Game1_CruiseNormalBallOddEvenLevelChange.Text = "1";
            Game1_CruiseNormalBallUnderOverLevelChange.Text = "1";

            game1_selectpanel.Enabled = true;

            Game1_RemainingTimer.Stop();
        }
        /********************************첫페이지 게임 시작 및 정지 버튼*************************************/
        private void Game1_Start_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_1))
            {
                txtLogAdd(txtLog1, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_1)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                    return;
                }
                Boolean siteIdB = false;
                foreach (JToken members in jTokenCustomers)
                {
                    if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                        && UtilModel.UserId.Contains(members["id"].ToString()))
                    {
                        siteIdB = true;
                        break;
                    }
                }
                if (siteIdB == false)
                {
                    MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                    return;
                }

                Game1_RemainingTimer.Start();

                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_1 = true;
                StartDateTime_1 = DateTime.Now;

                Game1_CruiseBetRegistListView.Items.Clear();

                Game1_Random_Result_Load_Time_For_PowerBall = rand.Next(betTime(GameCode_1) - 50, betTime(GameCode_1) - 20);
                Game1_Random_Bet_Regist_Time_For_PowerBall = rand.Next(betTime(GameCode_1) - 70, betTime(GameCode_1) - 40);

                Random_Nonce_1 = rand.Next(99999);
                game1_selectpanel.Enabled = false;
            }
            else
            {
                if (MessageBox.Show("게임을 정말로 종료하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Game1Stop(button);
                }
                else
                {
                    txtLogAdd(txtLog1, "취소되었습니다.", Color.Red);
                }
            }
        }
        private void Game3Stop(Button button)
        {

            button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
            button.Text = "배팅이 정지되었습니다.";
            StartGame_3 = false;

            Game3_Result_1 = string.Empty;
            Game3_Result_2 = string.Empty;
            Game3_Result_3 = string.Empty;
            Game3_Result_4 = string.Empty;

            Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game3_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game3_Betting_Mode_Result_Process = false;

            Game_3_Betting_Complete_Status = false;

            Game_3_Result_Load_Complete = false;

            Game_3_Check_Complete = false;

            Game3_BetMoney = new int[] { 0, 0, 0, 0 };

            Game3_BetPick = new string[] { null, null, null, null };

            Game3_CruisePowerBallOddEvenLevelChange.Text = "1";
            Game3_CruisePowerBallUnderOverLevelChange.Text = "1";
            Game3_CruiseNormalBallOddEvenLevelChange.Text = "1";
            Game3_CruiseNormalBallUnderOverLevelChange.Text = "1";
            game3_selectpanel.Enabled = true;
            Game3_RemainingTimer.Stop();
        }
        private void Game3_Start_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_3))
            {
                txtLogAdd(txtLog3, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_3)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                    return;
                }
                Boolean siteIdB = false;
                foreach (JToken members in jTokenCustomers)
                {
                    if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                        && UtilModel.UserId.Contains(members["id"].ToString()))
                    {
                        siteIdB = true;
                        break;
                    }
                }
                if (siteIdB == false)
                {
                    MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                    return;
                }

                Game3_RemainingTimer.Start();
                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_3 = true;
                StartDateTime_3 = DateTime.Now;
                Game3_CruiseBetRegistListView.Items.Clear();
                Game3_Random_Result_Load_Time_For_PowerBall = rand.Next(betTime(GameCode_3) - 50, betTime(GameCode_3) - 20);
                Game3_Random_Bet_Regist_Time_For_PowerBall = rand.Next(betTime(GameCode_3) - 70, betTime(GameCode_3) - 40);
                game3_selectpanel.Enabled = false;
                Random_Nonce_3 = rand.Next(99999);
            }
            else
            {
                if (MessageBox.Show("게임을 정말로 종료하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Game3Stop(button);
                }
                else
                {
                    txtLogAdd(txtLog3, "취소되었습니다.", Color.Red);
                }
            }
        }

        private void Game5Stop(Button button)
        {

            button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
            button.Text = "배팅이 정지되었습니다.";
            StartGame_5 = false;

            Game5_Result_1 = string.Empty;
            Game5_Result_2 = string.Empty;
            Game5_Result_3 = string.Empty;
            Game5_Result_4 = string.Empty;

            Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game5_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game5_Betting_Mode_Result_Process = false;

            Game_5_Betting_Complete_Status = false;

            Game_5_Result_Load_Complete = false;

            Game_5_Check_Complete = false;

            Game5_BetMoney = new int[] { 0, 0, 0, 0 };

            Game5_BetPick = new string[] { null, null, null, null };

            Game5_CruisePowerBallOddEvenLevelChange.Text = "1";
            Game5_CruisePowerBallUnderOverLevelChange.Text = "1";
            Game5_CruiseNormalBallOddEvenLevelChange.Text = "1";
            Game5_CruiseNormalBallUnderOverLevelChange.Text = "1";
            game5_selectpanel.Enabled = true;
            Game5_RemainingTimer.Stop();
        }
        private void Game5_Start_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_5))
            {
                txtLogAdd(txtLog5, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_5)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                    return;
                }
                Boolean siteIdB = false;
                foreach (JToken members in jTokenCustomers)
                {
                    if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                        && UtilModel.UserId.Contains(members["id"].ToString()))
                    {
                        siteIdB = true;
                        break;
                    }
                }
                if (siteIdB == false)
                {
                    MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                    return;
                }

                Game5_RemainingTimer.Start();

                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_5 = true;
                StartDateTime_5 = DateTime.Now;
                Game5_CruiseBetRegistListView.Items.Clear();
                Game5_Random_Result_Load_Time_For_PowerBall = rand.Next(betTime(GameCode_5) - 50, betTime(GameCode_5) - 20);
                Game5_Random_Bet_Regist_Time_For_PowerBall = rand.Next(betTime(GameCode_5) - 70, betTime(GameCode_5) - 40);
                game5_selectpanel.Enabled = false;
                Random_Nonce_5 = rand.Next(99999);
            }
            else
            {
                if (MessageBox.Show("게임을 정말로 종료하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Game5Stop(button);
                }
                else
                {
                    txtLogAdd(txtLog3, "취소되었습니다.", Color.Red);
                }
            }
        }
        private void Game6Stop(Button button)
        {
            button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
            button.Text = "배팅이 정지되었습니다.";
            StartGame_6 = false;

            Game6_Result_1 = string.Empty;
            Game6_Result_2 = string.Empty;
            Game6_Result_3 = string.Empty;
            Game6_Result_4 = string.Empty;

            Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
            Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
            Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
            Game6_CruiseBetNormalBallUnderOverSubLevel = 1;

            Game6_Betting_Mode_Result_Process = false;

            Game_6_Betting_Complete_Status = false;

            Game_6_Result_Load_Complete = false;

            Game_6_Check_Complete = false;

            Game6_BetMoney = new int[] { 0, 0, 0, 0 };

            Game6_BetPick = new string[] { null, null, null, null };

            Game6_CruisePowerBallOddEvenLevelChange.Text = "1";
            Game6_CruisePowerBallUnderOverLevelChange.Text = "1";
            Game6_CruiseNormalBallOddEvenLevelChange.Text = "1";
            Game6_CruiseNormalBallUnderOverLevelChange.Text = "1";
            game6_selectpanel.Enabled = true;
            Game6_RemainingTimer.Stop();
        }

        private void Game6_Start_Button_Click(object sender, EventArgs e)
        {

            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_6))
            {
                txtLogAdd(txtLog6, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_6)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                    return;
                }
                Boolean siteIdB = false;
                foreach (JToken members in jTokenCustomers)
                {
                    if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                        && UtilModel.UserId.Contains(members["id"].ToString()))
                    {
                        siteIdB = true;
                        break;
                    }
                }
                if (siteIdB == false)
                {
                    MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                    return;
                }

                Game6_RemainingTimer.Start();

                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_6 = true;
                StartDateTime_6 = DateTime.Now;
                Game6_CruiseBetRegistListView.Items.Clear();
                Game6_Random_Result_Load_Time_For_PowerBall = rand.Next(betTime(GameCode_6) - 50, betTime(GameCode_6) - 20);
                Game6_Random_Bet_Regist_Time_For_PowerBall = rand.Next(betTime(GameCode_6) - 70, betTime(GameCode_6) - 40);
                game6_selectpanel.Enabled = false;
                Random_Nonce_6 = rand.Next(99999);
            }
            else
            {
                if (MessageBox.Show("게임을 정말로 종료하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Game6Stop(button);
                }
                else
                {
                    txtLogAdd(txtLog6, "취소되었습니다.", Color.Red);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_2))
            {
                txtLogAdd(txtLog2, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_2)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jTokenCustomers == null || jTokenCustomers.Type == JTokenType.Null)
                {
                    MessageBox.Show("정보를 읽어오는데 실패하였습니다. 재시도 하여 주시기 바랍니다.");
                    return;
                } else
                {
                    if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                    {
                        MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                        return;
                    }
                    foreach (JToken members in jTokenCustomers)
                    {
                        if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                            && UtilModel.UserId.Equals(members["id"].ToString()))
                        {
                            MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                            return;
                        }
                    }
                }
                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_2 = true;
                StartDateTime_2 = DateTime.Now;

                Game2_Random_Result_Load_Time_For_Ladder = rand.Next(betTime(GameCode_1) - 40, betTime(GameCode_1) - 20);
                Game2_Random_Bet_Regist_Time_For_Ladder = rand.Next(betTime(GameCode_1) - 70, betTime(GameCode_1) - 40);
                Random_Nonce_2 = rand.Next(99999);
            }
            else
            {
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                button.Text = "배팅이 정지되었습니다.";
                StartGame_2 = false;

                Game2_Result_1 = string.Empty;
                Game2_Result_2 = string.Empty;
                Game2_Result_3 = string.Empty;

                Game2_FirstBetSubLevel = 1;
                Game2_SecondBetSubLevel = 1;
                Game2_ThirdBetSubLevel = 1;

                Game2_Betting_Mode_Result_Process = false;

                Game_2_Betting_Complete_Status = false;

                Game_2_Result_Load_Complete = false;

                Game_2_Check_Complete = false;

                Game2_BetMoney = new int[] { 0, 0, 0, 0 };

                Game2_BetPick = new string[] { null, null, null, null };

                Game2_FirstLevelChange.Text = "1";
                Game2_SecondLevelChange.Text = "1";
                Game2_ThirdLevelChange.Text = "1";
            }
        }

        private void Game4_Start_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode_4))
            {
                txtLogAdd(txtLog4, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_4)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
                    return;
                }
                foreach (JToken members in jTokenCustomers)
                {
                    if (UtilModel.UserSiteUrlAddress.ToLower().Contains(members["site"].ToString())
                        && UtilModel.UserId.Equals(members["id"].ToString()))
                    {
                        MessageBox.Show("허용되지 않은 아이디입니다.\r\n관리자에게 문의하십시요!");
                        return;
                    }
                }
                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_4 = true;
                StartDateTime_4 = DateTime.Now;
                Game4_Money_Set();
                Game4_Random_Result_Load_Time_For_Ladder = rand.Next(betTime(GameCode_4) - 40, betTime(GameCode_4) - 20);
                Game4_Random_Bet_Regist_Time_For_Ladder = rand.Next(betTime(GameCode_4) - 70, betTime(GameCode_4) - 40);
                Game4_BetMoney = new int[] { 0, 0, 0, 0 };
                Game4_BetPick = new string[] { null, null, null, null };
                Game4_BetMoneySettingTextBox.ReadOnly = true;
                Random_Nonce_4 = rand.Next(99999);
            }
            else
            {
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                button.Text = "배팅이 정지되었습니다.";
                StartGame_4 = false;
                Game4_BetMoneySettingTextBox.ReadOnly = false;
                Game4_Result_1 = string.Empty;
                Game4_Result_2 = string.Empty;
                Game4_Result_3 = string.Empty;

                Game4_Betting_Mode_Result_Process = false;

                Game_4_Betting_Complete_Status = false;

                Game_4_Result_Load_Complete = false;

                Game_4_Check_Complete = false;

                Game4_BetMoney = new int[] { 0, 0, 0, 0 };
                Game4_BetPick = new string[] { null, null, null, null };
            }
        }
        private void Game4_Money_Set()
        {
            string[] sarray = Game4_BetMoneySettingTextBox.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int numberI = 0; numberI < Game4_BetMoneySettingTextBox.Lines.Length; numberI++)
            {
                int.TryParse(Regex.Replace(sarray[numberI], @"\D", ""), out int outValue);
                Game4_BetMoneyList[numberI] = outValue;
            }
        }
        private void Game1_Bet_Processing_AllSum()
        {
            TextBox Game1_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game1_CruisePowerBallOddEvenBetPickLevel" + Game1_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game1_CruisePowerBallOddEvenBetMoneyLevel = (Controls.Find("Game1_CruisePowerBallOddEvenBetMoneyLevel" + Game1_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game1_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game1_CruisePowerBallUnderOverBetPickLevel" + Game1_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game1_CruisePowerBallUnderOverBetMoneyLevel = (Controls.Find("Game1_CruisePowerBallUnderOverBetMoneyLevel" + Game1_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game1_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game1_CruiseNormalBallOddEvenBetPickLevel" + Game1_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game1_CruiseNormalBallOddEvenBetMoneyLevel = (Controls.Find("Game1_CruiseNormalBallOddEvenBetMoneyLevel" + Game1_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game1_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game1_CruiseNormalBallUnderOverBetPickLevel" + Game1_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game1_CruiseNormalBallUnderOverBetMoneyLevel = (Controls.Find("Game1_CruiseNormalBallUnderOverBetMoneyLevel" + Game1_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            if (Game1_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game1_BetPick[0] = Game1_CruisePowerBallOddEvenBetPickLevel.Text;
                Game1_BetMoney[0] = 0;
            }
            else
            {
                Game1_BetPick[0] = Game1_CruisePowerBallOddEvenBetPickLevel.Text;
                Game1_BetMoney[0] = int.Parse(Regex.Replace(Game1_CruisePowerBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game1_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game1_BetPick[1] = Game1_CruisePowerBallUnderOverBetPickLevel.Text;
                Game1_BetMoney[1] = 0;
            }
            else
            {
                Game1_BetPick[1] = Game1_CruisePowerBallUnderOverBetPickLevel.Text;
                Game1_BetMoney[1] = int.Parse(Regex.Replace(Game1_CruisePowerBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game1_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game1_BetPick[2] = Game1_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game1_BetMoney[2] = 0;
            }
            else
            {
                Game1_BetPick[2] = Game1_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game1_BetMoney[2] = int.Parse(Regex.Replace(Game1_CruiseNormalBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game1_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game1_BetPick[3] = Game1_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game1_BetMoney[3] = 0;
            }
            else
            {
                Game1_BetPick[3] = Game1_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game1_BetMoney[3] = int.Parse(Regex.Replace(Game1_CruiseNormalBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game2_Bet_Processing_AllSum()
        {
            TextBox Game2_FirstBetPickLevel = Controls.Find("Game2_FirstBetPickLevel" + Game2_FirstBetSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game2_FirstBetMoneyLevel = (Controls.Find("Game2_FirstBetMoneyLevel" + Game2_FirstBetSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game2_SecondBetPickLevel = Controls.Find("Game2_SecondBetPickLevel" + Game2_SecondBetSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game2_SecondBetMoneyLevel = (Controls.Find("Game2_SecondBetMoneyLevel" + Game2_SecondBetSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game2_ThirdBetPickLevel = Controls.Find("Game2_ThirdBetPickLevel" + Game2_ThirdBetSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game2_ThirdBetMoneyLevel = (Controls.Find("Game2_ThirdBetMoneyLevel" + Game2_ThirdBetSubLevel.ToString(), true)[0] as TextBox);

            if (Game2_FirstBetPickLevel.Text.Contains("통"))
            {
                Game2_BetPick[0] = Game2_FirstBetPickLevel.Text;
                Game2_BetMoney[0] = 0;
            }
            else
            {
                Game2_BetPick[0] = Game2_FirstBetPickLevel.Text;
                Game2_BetMoney[0] = int.Parse(Regex.Replace(Game2_FirstBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game2_SecondBetPickLevel.Text.Contains("통"))
            {
                Game2_BetPick[1] = Game2_SecondBetPickLevel.Text;
                Game2_BetMoney[1] = 0;
            }
            else
            {
                Game2_BetPick[1] = Game2_SecondBetPickLevel.Text;
                Game2_BetMoney[1] = int.Parse(Regex.Replace(Game2_SecondBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game2_ThirdBetPickLevel.Text.Contains("통"))
            {
                Game2_BetPick[2] = Game2_ThirdBetPickLevel.Text;
                Game2_BetMoney[2] = 0;
            }
            else
            {
                Game2_BetPick[2] = Game2_ThirdBetPickLevel.Text;
                Game2_BetMoney[2] = int.Parse(Regex.Replace(Game2_ThirdBetMoneyLevel.Text, @"\D", ""));
            }
        }

        private void Game3_Bet_Processing_AllSum()
        {
            TextBox Game3_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game3_CruisePowerBallOddEvenBetPickLevel" + Game3_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game3_CruisePowerBallOddEvenBetMoneyLevel = (Controls.Find("Game3_CruisePowerBallOddEvenBetMoneyLevel" + Game3_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game3_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game3_CruisePowerBallUnderOverBetPickLevel" + Game3_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game3_CruisePowerBallUnderOverBetMoneyLevel = (Controls.Find("Game3_CruisePowerBallUnderOverBetMoneyLevel" + Game3_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game3_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game3_CruiseNormalBallOddEvenBetPickLevel" + Game3_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game3_CruiseNormalBallOddEvenBetMoneyLevel = (Controls.Find("Game3_CruiseNormalBallOddEvenBetMoneyLevel" + Game3_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game3_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game3_CruiseNormalBallUnderOverBetPickLevel" + Game3_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game3_CruiseNormalBallUnderOverBetMoneyLevel = (Controls.Find("Game3_CruiseNormalBallUnderOverBetMoneyLevel" + Game3_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            if (Game3_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game3_BetPick[0] = Game3_CruisePowerBallOddEvenBetPickLevel.Text;
                Game3_BetMoney[0] = 0;
            }
            else
            {
                Game3_BetPick[0] = Game3_CruisePowerBallOddEvenBetPickLevel.Text;
                Game3_BetMoney[0] = int.Parse(Regex.Replace(Game3_CruisePowerBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game3_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game3_BetPick[1] = Game3_CruisePowerBallUnderOverBetPickLevel.Text;
                Game3_BetMoney[1] = 0;
            }
            else
            {
                Game3_BetPick[1] = Game3_CruisePowerBallUnderOverBetPickLevel.Text;
                Game3_BetMoney[1] = int.Parse(Regex.Replace(Game3_CruisePowerBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game3_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game3_BetPick[2] = Game3_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game3_BetMoney[2] = 0;
            }
            else
            {
                Game3_BetPick[2] = Game3_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game3_BetMoney[2] = int.Parse(Regex.Replace(Game3_CruiseNormalBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game3_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game3_BetPick[3] = Game3_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game3_BetMoney[3] = 0;
            }
            else
            {
                Game3_BetPick[3] = Game3_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game3_BetMoney[3] = int.Parse(Regex.Replace(Game3_CruiseNormalBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            //Game3_CruiseBetRegistListView.EnsureVisible(Game3_CruiseBetRegistListView.Items.Count - 1);
        }
        int[] Game4_BetMoneyList = new int[100];
        int Game4_BetMoneyLevel = 0;
        private void Game4_Bet_Processing_AllSum()
        {
            Game4_BetMoney = new int[] { 0, 0, 0, 0 };
            Game4_BetPick = new string[] { null, null, null, null };

            // 우사홀, 좌사짝, 우삼짝, 좌삼홀
            if (Game4_Bet_Type == 1)
            {
                Game4_BetPick[0] = "우";
                Game4_BetPick[1] = "사";
                Game4_BetPick[2] = "홀";
                Game4_BetMoney[0] = Game4_BetMoney[1] = Game4_BetMoney[2] = Game4_BetMoneyList[Game4_BetMoneyLevel];
            }
            else if (Game4_Bet_Type == 2)
            {
                Game4_BetPick[0] = "좌";
                Game4_BetPick[1] = "사";
                Game4_BetPick[2] = "짝";
                Game4_BetMoney[0] = Game4_BetMoney[1] = Game4_BetMoney[2] = Game4_BetMoneyList[Game4_BetMoneyLevel];
            }
            else if (Game4_Bet_Type == 3)
            {
                Game4_BetPick[0] = "우";
                Game4_BetPick[1] = "삼";
                Game4_BetPick[2] = "짝";
                Game4_BetMoney[0] = Game4_BetMoney[1] = Game4_BetMoney[2] = Game4_BetMoneyList[Game4_BetMoneyLevel];
            }
            else if (Game4_Bet_Type == 4)
            {
                Game4_BetPick[0] = "좌";
                Game4_BetPick[1] = "삼";
                Game4_BetPick[2] = "홀";
                Game4_BetMoney[0] = Game4_BetMoney[1] = Game4_BetMoney[2] = Game4_BetMoneyList[Game4_BetMoneyLevel];
            }
        }

        private void Game5_Bet_Processing_AllSum()
        {
            TextBox Game5_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game5_CruisePowerBallOddEvenBetPickLevel" + Game5_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game5_CruisePowerBallOddEvenBetMoneyLevel = (Controls.Find("Game5_CruisePowerBallOddEvenBetMoneyLevel" + Game5_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game5_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game5_CruisePowerBallUnderOverBetPickLevel" + Game5_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game5_CruisePowerBallUnderOverBetMoneyLevel = (Controls.Find("Game5_CruisePowerBallUnderOverBetMoneyLevel" + Game5_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game5_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game5_CruiseNormalBallOddEvenBetPickLevel" + Game5_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game5_CruiseNormalBallOddEvenBetMoneyLevel = (Controls.Find("Game5_CruiseNormalBallOddEvenBetMoneyLevel" + Game5_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game5_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game5_CruiseNormalBallUnderOverBetPickLevel" + Game5_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game5_CruiseNormalBallUnderOverBetMoneyLevel = (Controls.Find("Game5_CruiseNormalBallUnderOverBetMoneyLevel" + Game5_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            if (Game5_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game5_BetPick[0] = Game5_CruisePowerBallOddEvenBetPickLevel.Text;
                Game5_BetMoney[0] = 0;
            }
            else
            {
                Game5_BetPick[0] = Game5_CruisePowerBallOddEvenBetPickLevel.Text;
                Game5_BetMoney[0] = int.Parse(Regex.Replace(Game5_CruisePowerBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game5_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game5_BetPick[1] = Game5_CruisePowerBallUnderOverBetPickLevel.Text;
                Game5_BetMoney[1] = 0;
            }
            else
            {
                Game5_BetPick[1] = Game5_CruisePowerBallUnderOverBetPickLevel.Text;
                Game5_BetMoney[1] = int.Parse(Regex.Replace(Game5_CruisePowerBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game5_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game5_BetPick[2] = Game5_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game5_BetMoney[2] = 0;
            }
            else
            {
                Game5_BetPick[2] = Game5_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game5_BetMoney[2] = int.Parse(Regex.Replace(Game5_CruiseNormalBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game5_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game5_BetPick[3] = Game5_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game5_BetMoney[3] = 0;
            }
            else
            {
                Game5_BetPick[3] = Game5_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game5_BetMoney[3] = int.Parse(Regex.Replace(Game5_CruiseNormalBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
        }

        private void Game6_Bet_Processing_AllSum()
        {
            TextBox Game6_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game6_CruisePowerBallOddEvenBetPickLevel" + Game6_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game6_CruisePowerBallOddEvenBetMoneyLevel = (Controls.Find("Game6_CruisePowerBallOddEvenBetMoneyLevel" + Game6_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game6_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game6_CruisePowerBallUnderOverBetPickLevel" + Game6_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game6_CruisePowerBallUnderOverBetMoneyLevel = (Controls.Find("Game6_CruisePowerBallUnderOverBetMoneyLevel" + Game6_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game6_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game6_CruiseNormalBallOddEvenBetPickLevel" + Game6_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game6_CruiseNormalBallOddEvenBetMoneyLevel = (Controls.Find("Game6_CruiseNormalBallOddEvenBetMoneyLevel" + Game6_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox);

            TextBox Game6_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game6_CruiseNormalBallUnderOverBetPickLevel" + Game6_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
            TextBox Game6_CruiseNormalBallUnderOverBetMoneyLevel = (Controls.Find("Game6_CruiseNormalBallUnderOverBetMoneyLevel" + Game6_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox);

            if (Game6_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game6_BetPick[0] = Game6_CruisePowerBallOddEvenBetPickLevel.Text;
                Game6_BetMoney[0] = 0;
            }
            else
            {
                Game6_BetPick[0] = Game6_CruisePowerBallOddEvenBetPickLevel.Text;
                Game6_BetMoney[0] = int.Parse(Regex.Replace(Game6_CruisePowerBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game6_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game6_BetPick[1] = Game6_CruisePowerBallUnderOverBetPickLevel.Text;
                Game6_BetMoney[1] = 0;
            }
            else
            {
                Game6_BetPick[1] = Game6_CruisePowerBallUnderOverBetPickLevel.Text;
                Game6_BetMoney[1] = int.Parse(Regex.Replace(Game6_CruisePowerBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game6_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
            {
                Game6_BetPick[2] = Game6_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game6_BetMoney[2] = 0;
            }
            else
            {
                Game6_BetPick[2] = Game6_CruiseNormalBallOddEvenBetPickLevel.Text;
                Game6_BetMoney[2] = int.Parse(Regex.Replace(Game6_CruiseNormalBallOddEvenBetMoneyLevel.Text, @"\D", ""));
            }
            if (Game6_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
            {
                Game6_BetPick[3] = Game6_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game6_BetMoney[3] = 0;
            }
            else
            {
                Game6_BetPick[3] = Game6_CruiseNormalBallUnderOverBetPickLevel.Text;
                Game6_BetMoney[3] = int.Parse(Regex.Replace(Game6_CruiseNormalBallUnderOverBetMoneyLevel.Text, @"\D", ""));
            }
        }

        private void Game1_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game1_StackMoney.Text, @"\D", ""), out outValue);
            bool isContains = false;

            for (int i = 0; i < Game1_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game1_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_1.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(dayroundNo_1.ToString());

            if (!String.IsNullOrEmpty(Game1_BetPick[0]))
            {
                if (Game1_BetPick[0].Contains("통"))
                {
                    Game1_BetPick[0] = "";
                    Game1_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game1_BetMoney[0];

                    item.SubItems.Add(Game1_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game1_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game1_BetPick[0] = "";
                Game1_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game1_BetPick[1]))
            {
                if (Game1_BetPick[1].Contains("통"))
                {
                    Game1_BetPick[1] = "";
                    Game1_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game1_BetMoney[1];

                    item.SubItems.Add(Game1_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game1_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game1_BetPick[1] = "";
                Game1_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game1_BetPick[2]))
            {
                if (Game1_BetPick[2].Contains("통"))
                {
                    Game1_BetPick[2] = "";
                    Game1_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game1_BetMoney[2];

                    item.SubItems.Add(Game1_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game1_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game1_BetPick[2] = "";
                Game1_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game1_BetPick[3]))
            {
                if (Game1_BetPick[3].Contains("통"))
                {
                    Game1_BetPick[3] = null;
                    Game1_BetMoney[3] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game1_BetMoney[3];

                    item.SubItems.Add(Game1_BetPick[3]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game1_BetMoney[3]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game1_BetPick[3] = null;
                Game1_BetMoney[3] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            Game1_StackMoney.Text = UtilModel.StringFormatChanged(outValue);
            Game1_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog1, dayroundNo_1 + "회차 등록이 완료[게임1]", Color.OrangeRed);
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game2_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game2_StackMoney.Text, @"\D", ""), out outValue);

            bool isContains = false;

            for (int i = 0; i < Game2_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game2_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_2.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add((dayroundNo_2.ToString()));
            if (!String.IsNullOrEmpty(Game2_BetPick[0]))
            {
                if (Game2_BetPick[0].Contains("통"))
                {
                    Game2_BetPick[0] = null;
                    Game2_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game2_BetMoney[0];

                    item.SubItems.Add(Game2_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game2_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game2_BetPick[0] = null;
                Game2_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game2_BetPick[1]))
            {
                if (Game2_BetPick[1].Contains("통"))
                {
                    Game2_BetPick[1] = null;
                    Game2_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game2_BetMoney[1];

                    item.SubItems.Add(Game2_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game2_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game2_BetPick[1] = null;
                Game2_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game2_BetPick[2]))
            {
                if (Game2_BetPick[2].Contains("통"))
                {
                    Game2_BetPick[2] = null;
                    Game2_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game2_BetMoney[2];

                    item.SubItems.Add(Game2_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game2_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game2_BetPick[2] = null;
                Game2_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            Game2_StackMoney.Text = UtilModel.StringFormatChanged(outValue);

            Game2_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog2, dayroundNo_2 + "회차 등록이 완료[게임2]", Color.OrangeRed);
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }
        private void Game3_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game3_StackMoney.Text, @"\D", ""), out outValue);
            bool isContains = false;

            for (int i = 0; i < Game3_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game3_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_3.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(dayroundNo_3.ToString());

            if (!String.IsNullOrEmpty(Game3_BetPick[0]))
            {
                if (Game3_BetPick[0].Contains("통"))
                {
                    Game3_BetPick[0] = "";
                    Game3_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game3_BetMoney[0];

                    item.SubItems.Add(Game3_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game3_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game3_BetPick[0] = "";
                Game3_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game3_BetPick[1]))
            {
                if (Game3_BetPick[1].Contains("통"))
                {
                    Game3_BetPick[1] = "";
                    Game3_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game3_BetMoney[1];

                    item.SubItems.Add(Game3_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game3_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game3_BetPick[1] = "";
                Game3_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game3_BetPick[2]))
            {
                if (Game3_BetPick[2].Contains("통"))
                {
                    Game3_BetPick[2] = "";
                    Game3_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game3_BetMoney[2];

                    item.SubItems.Add(Game3_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game3_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game3_BetPick[2] = "";
                Game3_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game3_BetPick[3]))
            {
                if (Game3_BetPick[3].Contains("통"))
                {
                    Game3_BetPick[3] = null;
                    Game3_BetMoney[3] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game3_BetMoney[3];

                    item.SubItems.Add(Game3_BetPick[3]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game3_BetMoney[3]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game3_BetPick[3] = null;
                Game3_BetMoney[3] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            Game3_StackMoney.Text = UtilModel.StringFormatChanged(outValue);
            Game3_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog3, dayroundNo_3 + "회차 등록이 완료[게임3]", Color.OrangeRed);
        }
        private void Game4_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game4_StackMoney.Text, @"\D", ""), out outValue);
            bool isContains = false;

            for (int i = 0; i < Game4_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game4_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_4.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add((dayroundNo_4.ToString()));
            if (!String.IsNullOrEmpty(Game4_BetPick[0]))
            {
                if (Game4_BetPick[0].Contains("통"))
                {
                    Game4_BetPick[0] = null;
                    Game4_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game4_BetMoney[0];

                    item.SubItems.Add(Game4_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game4_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game4_BetPick[0] = null;
                Game4_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game4_BetPick[1]))
            {
                if (Game4_BetPick[1].Contains("통"))
                {
                    Game4_BetPick[1] = null;
                    Game4_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game4_BetMoney[1];

                    item.SubItems.Add(Game4_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game4_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game4_BetPick[1] = null;
                Game4_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game4_BetPick[2]))
            {
                if (Game4_BetPick[2].Contains("통"))
                {
                    Game4_BetPick[2] = null;
                    Game4_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game4_BetMoney[2];

                    item.SubItems.Add(Game4_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game4_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game4_BetPick[2] = null;
                Game4_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            Game4_StackMoney.Text = UtilModel.StringFormatChanged(outValue);

            Game4_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog4, dayroundNo_4 + "회차 등록이 완료[게임4]", Color.OrangeRed);
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game5_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game5_StackMoney.Text, @"\D", ""), out outValue);
            bool isContains = false;

            for (int i = 0; i < Game5_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game5_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_5.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(dayroundNo_5.ToString());

            if (!String.IsNullOrEmpty(Game5_BetPick[0]))
            {
                if (Game5_BetPick[0].Contains("통"))
                {
                    Game5_BetPick[0] = "";
                    Game5_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game5_BetMoney[0];

                    item.SubItems.Add(Game5_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game5_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game5_BetPick[0] = "";
                Game5_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game5_BetPick[1]))
            {
                if (Game5_BetPick[1].Contains("통"))
                {
                    Game5_BetPick[1] = "";
                    Game5_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game5_BetMoney[1];

                    item.SubItems.Add(Game5_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game5_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game5_BetPick[1] = "";
                Game5_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game5_BetPick[2]))
            {
                if (Game5_BetPick[2].Contains("통"))
                {
                    Game5_BetPick[2] = "";
                    Game5_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game5_BetMoney[2];

                    item.SubItems.Add(Game5_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game5_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game5_BetPick[2] = "";
                Game5_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game5_BetPick[3]))
            {
                if (Game5_BetPick[3].Contains("통"))
                {
                    Game5_BetPick[3] = null;
                    Game5_BetMoney[3] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game5_BetMoney[3];

                    item.SubItems.Add(Game5_BetPick[3]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game5_BetMoney[3]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game5_BetPick[3] = null;
                Game5_BetMoney[3] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            Game5_StackMoney.Text = UtilModel.StringFormatChanged(outValue);
            Game5_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog5, dayroundNo_5 + "회차 등록이 완료[게임5]", Color.OrangeRed);
            //Game5_CruiseBetRegistListView.EnsureVisible(Game5_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game6_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game6_StackMoney.Text, @"\D", ""), out outValue);
            bool isContains = false;

            for (int i = 0; i < Game6_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game6_CruiseBetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayroundNo_6.ToString());

                if (isContains)
                {
                    break;
                }
            }

            if (isContains)
            {
                return;
            }

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(dayroundNo_6.ToString());

            if (!String.IsNullOrEmpty(Game6_BetPick[0]))
            {
                if (Game6_BetPick[0].Contains("통"))
                {
                    Game6_BetPick[0] = "";
                    Game6_BetMoney[0] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game6_BetMoney[0];

                    item.SubItems.Add(Game6_BetPick[0]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game6_BetMoney[0]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game6_BetPick[0] = "";
                Game6_BetMoney[0] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            if (!String.IsNullOrEmpty(Game6_BetPick[1]))
            {
                if (Game6_BetPick[1].Contains("통"))
                {
                    Game6_BetPick[1] = "";
                    Game6_BetMoney[1] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game6_BetMoney[1];

                    item.SubItems.Add(Game6_BetPick[1]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game6_BetMoney[1]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game6_BetPick[1] = "";
                Game6_BetMoney[1] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game6_BetPick[2]))
            {
                if (Game6_BetPick[2].Contains("통"))
                {
                    Game6_BetPick[2] = "";
                    Game6_BetMoney[2] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game6_BetMoney[2];

                    item.SubItems.Add(Game6_BetPick[2]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game6_BetMoney[2]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game6_BetPick[2] = "";
                Game6_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            if (!String.IsNullOrEmpty(Game6_BetPick[3]))
            {
                if (Game6_BetPick[3].Contains("통"))
                {
                    Game6_BetPick[3] = null;
                    Game6_BetMoney[3] = 0;
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");
                }
                else
                {
                    outValue += Game6_BetMoney[3];

                    item.SubItems.Add(Game6_BetPick[3]);
                    item.SubItems.Add("");
                    item.SubItems.Add(UtilModel.StringFormatChanged(Game6_BetMoney[3]));
                    item.SubItems.Add("");
                }
            }
            else
            {
                Game6_BetPick[3] = null;
                Game6_BetMoney[3] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }

            Game6_StackMoney.Text = UtilModel.StringFormatChanged(outValue);
            Game6_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog6, dayroundNo_6 + "회차 등록이 완료[게임6]", Color.OrangeRed);
            //Game6_CruiseBetRegistListView.EnsureVisible(Game6_CruiseBetRegistListView.Items.Count - 1);
        }
        private Boolean Game_Bet_Processing_Final(String[] pick, int[] money, Double gameInning, int remainTime, RichTextBox logBox, int endSec, string GameCode, int nonce)
        {
            int All_Betting_Money = money[0] + money[1] + money[2] + money[3];
            if (All_Betting_Money >= 1)
            {
                StringBuilder Game_Url_Param_String = new StringBuilder();
                Game_Url_Param_String.Append(UtilModel.UserSiteUrlAddress);
                if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click || UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                {
                    Game_Url_Param_String.AppendFormat(":{0}/api/bet?userid={1}&key={2}&gm={3}&tdate={4}&rno={5}", UtilModel.SitePort, UtilModel.UserId, UtilModel.Bet_Api_Key, GameCode, DateTime.Now.ToString("yyyyMMdd"), gameInning.ToString());

                    if (!String.IsNullOrEmpty(pick[0]))
                    {
                        if (pick[0].Contains("홀") || pick[0].Contains("좌"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp1={0}", money[0]);
                        }
                        else if (pick[0].Contains("짝") || pick[0].Contains("우"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp2={0}", money[0]);
                        }
                    }
                    if (!String.IsNullOrEmpty(pick[1]))
                    {
                        if (pick[1].Contains("언") || pick[1].Contains("삼"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp3={0}", money[1]);
                        }
                        else if (pick[1].Contains("오") || pick[1].Contains("사"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp4={0}", money[1]);
                        }
                    }

                    if (!String.IsNullOrEmpty(pick[2]))
                    {
                        if (pick[2].Contains("홀"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp5={0}", money[2]);
                        }
                        else
                        if (pick[2].Contains("짝"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp6={0}", money[2]);
                        }
                    }

                    if (!String.IsNullOrEmpty(pick[3]))
                    {
                        if (pick[3].Contains("언"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp7={0}", money[3]);
                        }
                        if (pick[3].Contains("오"))
                        {
                            Game_Url_Param_String.AppendFormat("&pp8={0}", money[3]);
                        }
                    }
                    Game_Url_Param_String.AppendFormat("&nonce={0}", nonce);
                }
                else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                {
                    Game_Url_Param_String.Append("/game/bet.asp");
                    Game_Url_Param_String.AppendFormat("?id={0}", UtilModel.UserId);
                    Game_Url_Param_String.AppendFormat("&key={0}", UtilModel.Bet_Api_Key);

                    Game_Url_Param_String.AppendFormat("&type={0}", GameCode);
                    Game_Url_Param_String.AppendFormat("&round={0}", gameInning.ToString());
                    if (!String.IsNullOrEmpty(pick[0]))
                    {
                        if (pick[0].Contains("홀") || pick[0].Contains("좌"))
                        {
                            Game_Url_Param_String.AppendFormat("&p1={0}", money[0]);
                        }
                        else if (pick[0].Contains("짝") || pick[0].Contains("우"))
                        {
                            Game_Url_Param_String.AppendFormat("&p2={0}", money[0]);
                        }
                    }
                    if (!String.IsNullOrEmpty(pick[1]))
                    {
                        if (pick[1].Contains("언") || pick[1].Contains("삼"))
                        {
                            Game_Url_Param_String.AppendFormat("&p3={0}", money[1]);
                        }
                        else if (pick[1].Contains("오") || pick[1].Contains("사"))
                        {
                            Game_Url_Param_String.AppendFormat("&p4={0}", money[1]);
                        }
                    }

                    if (!String.IsNullOrEmpty(pick[2]))
                    {
                        if (pick[2].Contains("홀"))
                        {
                            Game_Url_Param_String.AppendFormat("&p5={0}", money[2]);
                        }
                        else
                        if (pick[2].Contains("짝"))
                        {
                            Game_Url_Param_String.AppendFormat("&p6={0}", money[2]);
                        }
                    }

                    if (!String.IsNullOrEmpty(pick[3]))
                    {
                        if (pick[3].Contains("언"))
                        {
                            Game_Url_Param_String.AppendFormat("&p7={0}", money[3]);
                        }
                        if (pick[3].Contains("오"))
                        {
                            Game_Url_Param_String.AppendFormat("&p8={0}", money[3]);
                        }
                    }
                }
                string returnMessage = null;
                int CountResult = 0;
                while (true)
                {
                    try
                    {
                        logger.Info(Game_Url_Param_String.ToString());
                        //var returnMessage = UtilModel.MakeAsyncRequest(Game_Url_Param_String.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");                       
                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            returnMessage = webClient.DownloadString(Game_Url_Param_String.ToString());
                        }
                        logger.Info(UtilModel.UnicodeToString(returnMessage));
                        if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click || UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                        {
                            if (returnMessage.Contains("ret_code"))
                            {
                                break;
                            }
                            else
                            {
                                CountResult++;
                                string errorMessage = "재배팅 시도 횟수 : [" + CountResult + "] 회";
                                txtLogAdd(logBox, errorMessage, Color.OrangeRed);
                                if (CountResult > 40)
                                {
                                    break;
                                }
                                if (remainTime < endSec)
                                {
                                    break;
                                }
                                UtilModel.Delay(5000);
                                continue;
                            }
                        }
                        else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                        {
                            if (returnMessage.Contains("err"))
                            {
                                break;
                            }
                            else
                            {
                                CountResult++;
                                string errorMessage = "재배팅 시도 횟수 : [" + CountResult + "] 회";
                                txtLogAdd(logBox, errorMessage, Color.OrangeRed);
                                logger.Info(UtilModel.UnicodeToString(returnMessage));
                                if (CountResult > 10)
                                {
                                    break;
                                }
                                if (remainTime < endSec)
                                {
                                    break;
                                }
                                UtilModel.Delay(5000);
                                continue;
                            }
                        }
                    }
                    catch (Exception _ex)
                    {
                        CountResult++;
                        string errorMessage = "재배팅 시도 횟수 : [" + CountResult + "] 회";
                        txtLogAdd(logBox, errorMessage, Color.OrangeRed);
                        if (CountResult > 10)
                        {
                            break;
                        }
                        if (remainTime < endSec)
                        {
                            break;
                        }
                        logger.Error(_ex.ToString());
                        UtilModel.Delay(5000);
                        continue;
                    }
                }
                try
                {
                    JObject jo = JObject.Parse(returnMessage);
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click || UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        int ret_code = int.Parse(jo.SelectToken("ret_code").ToString());
                        var ret_message = jo.SelectToken("comment").ToString();
                        if (ret_code == 1)
                        {
                            UtilModel.UserOwnMoney = int.Parse(jo.SelectToken("more_info.balance").ToString());

                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);

                            txtLogAdd(logBox, "[" + gameInning + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                            logger.Info("[" + gameInning + "] 정상 배팅 등록 완료.");
                            return true;
                        }
                        else if (ret_code < 0)
                        {
                            if (ret_code == -1009)
                            {
                                return true;
                            }
                            txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + ret_message, Color.OrangeRed);
                            logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                            UtilModel.Delay(3000);
                            return false;
                        }
                        else
                        {
                            txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + ret_message, Color.OrangeRed);
                            logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                            UtilModel.Delay(3000);
                            return false;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        int ret_code = int.Parse(jo.SelectToken("err").ToString());
                        if (ret_code > 0)
                        {
                            var ret_message = jo.SelectToken("msg").ToString();
                            txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message), Color.OrangeRed);
                            logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message));
                            return false;
                        }
                        else
                        {
                            var ret_message = jo.SelectToken("msg").ToString();
                            if (ret_code == 0)
                            {
                                UtilModel.UserOwnMoney = int.Parse(jo.SelectToken("data.GameMoney").ToString());

                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);

                                txtLogAdd(logBox, "[" + gameInning + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                                logger.Info("[" + gameInning + "] " + UtilModel.UnicodeToString(ret_message));
                                return true;
                            }
                            else
                            {
                                txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message), Color.OrangeRed);
                                logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message));
                                UtilModel.Delay(5000);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception _ex)
                {
                    txtLogAdd(logBox, _ex.ToString(), Color.OrangeRed);
                    logger.Info(_ex.ToString());
                }
            }
            return false;
        }
        /***********************************************************************
         * 스코어게임 
         * https://game.dr-score.com/api/coinpowerball3/get           
         * 
         * {
"DAYROUND": 383,
"RESULTNUM": "5,14,26,6,16",
"PB": 2,
"HASH": 334756139,
"SUM": 67,
"PBODDEVEN": 2,
"PBUNDEROVER": 1,
"NUMODDEVEN": 1,
"NUMUNDEROVER": 1,
"NUMDJS": 2,
"TIME": "2022-10-18 19:09:00",
"DATENUM": 20221018
},
        **********************************************************************/
        private Boolean loadDreamScoreResultForCoinPowerBallGame(int gameType, string gamecode, int gameInning)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://game.dr-score.com/api/{0}/get ", DreamScoreGameCode(gamecode));
                string returnVal = null;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnVal = webClient.DownloadString(stringBuilder.ToString());
                }
                logger.Info(returnVal);
                if (returnVal.Contains("date"))
                {
                    JObject jObject = JObject.Parse(returnVal);
                    JToken jDatas = jObject["datas"];
                    foreach (JToken members in jDatas)
                    {
                        if (gamecode.Contains(members["DATENUM"].ToString())
                            && gameInning.ToString().Equals(members["DAYROUND"].ToString()))
                        {
                            break;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadUpDownResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        /***********************************************************************
         * 
         * 스코어게임 
         * 
        // eos 게임 엔트리

        // https://ntry.com/data/json/games/eos_powerball/5min/result.json
        {
        "date": "2022-10-15",
        "times": 273314434,
        "date_round": 246,
        "ball": [
        17,
        18,
        10,
        13,
        8,
        "7"
        ],
        "pow_ball_oe": "홀",
        "pow_ball_unover": "오버",
        "def_ball_sum": "66",
        "def_ball_oe": "짝",
        "def_ball_unover": "언더",
        "def_ball_size": "중",
        "def_ball_section": "E",
        "fixed_date_round": "C63AB"
        }
        //https://ntry.com/data/json/games/eos_powerball/3min/result.json
        {
        "date": "2022-10-15",
        "times": 273314434,
        "date_round": 410,
        "ball": [
        24,
        15,
        4,
        21,
        6,
        "5"
        ],
        "pow_ball_oe": "홀",
        "pow_ball_unover": "오버",
        "def_ball_sum": "70",
        "def_ball_oe": "짝",
        "def_ball_unover": "언더",
        "def_ball_size": "중",
        "def_ball_section": "E",
        "fixed_date_round": "C63AB"
        }
        ***********************************************************************/

        private Boolean loadNtryEOSResultForPowerBallGame(int gameType, string gamecode, int gameInning)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://ntry.com/data/json/games/eos_powerball/{0}/result.json", NtryGameCode(gamecode));
                string returnVal = null;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnVal = webClient.DownloadString(stringBuilder.ToString());
                }
                logger.Info(returnVal);
                if (returnVal.Contains("date"))
                {
                    JObject jo = JObject.Parse(returnVal);
                    if (jo.SelectToken("date_round").ToString().Equals(gameInning.ToString()))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = jo.SelectToken("pow_ball_oe").ToString();
                            Game1_Result_2 = jo.SelectToken("pow_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            Game1_Result_3 = jo.SelectToken("def_ball_oe").ToString();
                            Game1_Result_4 = jo.SelectToken("def_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog1, "NTRY [" + (gameInning) + "회] " + Game1_Result_1 + " || " + Game1_Result_2 + " || " + Game1_Result_3 + " || " + Game1_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = jo.SelectToken("pow_ball_oe").ToString();
                            Game3_Result_2 = jo.SelectToken("pow_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            Game3_Result_3 = jo.SelectToken("def_ball_oe").ToString();
                            Game3_Result_4 = jo.SelectToken("def_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog3, "NTRY [" + (gameInning) + "회] " + Game3_Result_1 + " || " + Game3_Result_2 + " || " + Game3_Result_3 + " || " + Game3_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = jo.SelectToken("pow_ball_oe").ToString();
                            Game5_Result_2 = jo.SelectToken("pow_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            Game5_Result_3 = jo.SelectToken("def_ball_oe").ToString();
                            Game5_Result_4 = jo.SelectToken("def_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog5, "NTRY [" + (gameInning) + "회] " + Game5_Result_1 + " || " + Game5_Result_2 + " || " + Game5_Result_3 + " || " + Game5_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = jo.SelectToken("pow_ball_oe").ToString();
                            Game6_Result_2 = jo.SelectToken("pow_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            Game6_Result_3 = jo.SelectToken("def_ball_oe").ToString();
                            Game6_Result_4 = jo.SelectToken("def_ball_unover").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog6, "NTRY [" + (gameInning) + "회] " + Game6_Result_1 + " || " + Game6_Result_2 + " || " + Game6_Result_3 + " || " + Game6_Result_4, Color.Black);
                            return true;
                        }
                    }
                    logger.Error(stringBuilder.ToString());
                    logger.Error("회차 정보 다름 loadNtryEosResultForPowerBallGame data_gameInning : " + jo.SelectToken("date_round").ToString() + " || " + gameInning.ToString());
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadUpDownResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        private Boolean loadAPISiteResultGame(int gameType, string gamecode, int dayroundno)
        {
            try
            {
                string dateNow = DateTime.Now.ToString("yyyyMMdd");
                if (gamecode.Equals("EPB3") || gamecode.Equals("DSCP3"))
                {
                    if (dayroundno == 480)
                    {
                        dateNow = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    }
                }
                else if (gamecode.Equals("EPB") || gamecode.Equals("DSCP5"))
                {
                    if (dayroundno == 288)
                    {
                        dateNow = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    }
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}:{1}/auto/api/get_pushed_result?gm={2}&d={3}&r={4}&k={5}", UtilModel.UserSiteUrlAddress, UtilModel.SitePort, gamecode, dateNow, dayroundno, UtilModel.Bet_Api_Key);

                string returnMessage;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnMessage = webClient.DownloadString(stringBuilder.ToString());
                }
                logger.Info(stringBuilder.ToString());
                logger.Info(returnMessage);
                if (returnMessage.Contains("code"))
                {
                    JObject jo = JObject.Parse(returnMessage);
                    int ret_code = int.Parse(jo.SelectToken("code").ToString());
                    if (ret_code == 1)
                    {
                        if (gamecode.Equals("EPB") || gamecode.Equals("EPB3") || gamecode.Equals("DSCP3") || gamecode.Equals("DSCP5"))
                        {
                            if (!jo.SelectToken("more_info").ToString().Contains("pball"))
                            {
                                int[] ballSum = new int[6];
                                var jArray = JArray.Parse(jo.SelectToken("more_info").ToString());
                                int iNumber = 0;
                                foreach (var item in jArray.Children())
                                {
                                    ballSum[iNumber] = int.Parse(item.Value<string>().ToString());
                                    iNumber++;
                                }

                                if (ballSum[5] > 4)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_2 = "오";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_2 = "오";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_2 = "오";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_2 = "오";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_2 = "언";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_2 = "언";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_2 = "언";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_2 = "언";
                                    }
                                }
                                if (ballSum[5] % 2 == 1)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_1 = "홀";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_1 = "홀";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_1 = "홀";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_1 = "홀";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_1 = "짝";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_1 = "짝";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_1 = "짝";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_1 = "짝";
                                    }
                                }

                                if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] > 72)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_4 = "오";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_4 = "오";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_4 = "오";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_4 = "오";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_4 = "언";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_4 = "언";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_4 = "언";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_4 = "언";
                                    }
                                }
                                if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] % 2 == 1)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_3 = "홀";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_3 = "홀";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_3 = "홀";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_3 = "홀";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_3 = "짝";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_3 = "짝";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_3 = "짝";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_3 = "짝";
                                    }
                                }
                                if (gameType == 1)
                                {
                                    txtLogAdd(txtLog1, "[" + (dayroundno) + "회] " + Game1_Result_1 + " | " + Game1_Result_2 + " || " + Game1_Result_3 + " | " + Game1_Result_4, Color.Black);
                                }
                                else if (gameType == 3)
                                {
                                    txtLogAdd(txtLog3, "[" + (dayroundno) + "회] " + Game3_Result_1 + " | " + Game3_Result_2 + " || " + Game3_Result_3 + " | " + Game3_Result_4, Color.Black);
                                }
                                else if (gameType == 5)
                                {
                                    txtLogAdd(txtLog5, "[" + (dayroundno) + "회] " + Game5_Result_1 + " | " + Game5_Result_2 + " || " + Game5_Result_3 + " | " + Game5_Result_4, Color.Black);
                                }
                                else if (gameType == 6)
                                {
                                    txtLogAdd(txtLog6, "[" + (dayroundno) + "회] " + Game6_Result_1 + " | " + Game6_Result_2 + " || " + Game6_Result_3 + " | " + Game6_Result_4, Color.Black);
                                }
                                return true;
                            }
                            else
                            {
                                // 파워볼 합계
                                int.TryParse(jo.SelectToken("more_info.pball").ToString(), out int pball);
                                if (pball > 4)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_2 = "오";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_2 = "오";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_2 = "오";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_2 = "오";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_2 = "언";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_2 = "언";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_2 = "언";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_2 = "언";
                                    }
                                }
                                if (pball % 2 == 1)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_1 = "홀";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_1 = "홀";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_1 = "홀";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_1 = "홀";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_1 = "짝";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_1 = "짝";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_1 = "짝";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_1 = "짝";
                                    }
                                }
                                // 일반볼 합계
                                int.TryParse(jo.SelectToken("more_info.sum").ToString(), out int sum);
                                if (sum > 72)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_4 = "오";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_4 = "오";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_4 = "오";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_4 = "오";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_4 = "언";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_4 = "언";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_4 = "언";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_4 = "언";
                                    }
                                }
                                if (sum % 2 == 1)
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_3 = "홀";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_3 = "홀";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_3 = "홀";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_3 = "홀";
                                    }
                                }
                                else
                                {
                                    if (gameType == 1)
                                    {
                                        Game1_Result_3 = "짝";
                                    }
                                    else if (gameType == 3)
                                    {
                                        Game3_Result_3 = "짝";
                                    }
                                    else if (gameType == 5)
                                    {
                                        Game5_Result_3 = "짝";
                                    }
                                    else if (gameType == 6)
                                    {
                                        Game6_Result_3 = "짝";
                                    }
                                }
                                if (gameType == 1)
                                {
                                    txtLogAdd(txtLog1, "[" + (dayroundno) + "회] " + Game1_Result_1 + " | " + Game1_Result_2 + " || " + Game1_Result_3 + " | " + Game1_Result_4, Color.Black);
                                }
                                else if (gameType == 3)
                                {
                                    txtLogAdd(txtLog3, "[" + (dayroundno) + "회] " + Game3_Result_1 + " | " + Game3_Result_2 + " || " + Game3_Result_3 + " | " + Game3_Result_4, Color.Black);
                                }
                                else if (gameType == 5)
                                {
                                    txtLogAdd(txtLog5, "[" + (dayroundno) + "회] " + Game5_Result_1 + " | " + Game5_Result_2 + " || " + Game5_Result_3 + " | " + Game5_Result_4, Color.Black);
                                }
                                else if (gameType == 6)
                                {
                                    txtLogAdd(txtLog6, "[" + (dayroundno) + "회] " + Game6_Result_1 + " | " + Game6_Result_2 + " || " + Game6_Result_3 + " | " + Game6_Result_4, Color.Black);
                                }
                                return true;
                            }
                        }
                        else if (gamecode.Equals("CSA3") || gamecode.Equals("CSA5"))
                        {
                            return false;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
                return false;
            }
        }
        /*
        private Boolean loadEPickResultForLadder(int gameType, string gamecode, int inning, RichTextBox richTextBox)
        {
            try
            {
                string returnVal = UtilModel.MakeAsyncRequest("http://e-pick.net/api/v1/gameInfo", "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);
                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("true"))
                {
                    string epickCode = EPICKGameCode(gamecode);
                    JObject jo = JObject.Parse(returnVal);
                    if (jo.SelectToken("data.NAME_" + epickCode).ToString().Contains("사다리"))
                    {
                        string gameInning = jo.SelectToken("data.GAME_" + epickCode).ToString();

                        if (!gameInning.Equals(inning.ToString()))
                        {
                            return false;
                        }
                        int num = int.Parse(jo.SelectToken("data.NUM1_" + epickCode).ToString());
                        if (num % 2 == 0) // 짝수 = 우
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_1 = "우";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_1 = "우";
                            }
                        }
                        else
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_1 = "좌";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_1 = "좌";
                            }
                        }
                        if (num > 14) // 짝수 = 우
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_2 = "사";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_2 = "사";
                            }
                        }
                        else
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_2 = "삼";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_2 = "삼";
                            }
                        }

                        if (Game2_Result_1.Equals("우") && Game2_Result_2.Equals("삼") || Game4_Result_1.Equals("우") && Game4_Result_2.Equals("삼"))
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_3 = "홀";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_3 = "홀";
                            }
                        }
                        else if (Game2_Result_1.Equals("우") && Game2_Result_2.Equals("사") || Game4_Result_1.Equals("우") && Game4_Result_2.Equals("사"))
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_3 = "짝";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_3 = "짝";
                            }
                        }
                        else if (Game2_Result_1.Equals("좌") && Game2_Result_2.Equals("삼") || Game4_Result_1.Equals("좌") && Game4_Result_2.Equals("삼"))
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_3 = "짝";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_3 = "짝";
                            }
                        }
                        else if (Game2_Result_1.Equals("좌") && Game2_Result_2.Equals("사") || Game4_Result_1.Equals("좌") && Game4_Result_2.Equals("사"))
                        {
                            if (gameType == 2)
                            {
                                Game2_Result_3 = "홀";
                            }
                            else if (gameType == 4)
                            {
                                Game4_Result_3 = "홀";
                            }
                        }
                        if (gameType == 2)
                        {
                            txtLogAdd(richTextBox, "[" + gameInning + "] [" + (inning) + "회] 사다리 : " + Game2_Result_1 + " || " + Game2_Result_2 + " || " + Game2_Result_3, Color.Black);
                        }
                        else if (gameType == 4)
                        {
                            txtLogAdd(richTextBox, "[" + gameInning + "] [" + (inning) + "회] 사다리 : " + Game4_Result_1 + " || " + Game4_Result_2 + " || " + Game4_Result_3, Color.Black);
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadEPickResultForLadder || " + _ex.ToString());
                return false;
            }
        }
        private Boolean loadEPickResultForPowerBallGame(int gameType, string gamecode, int gameInning)
        {
            try
            {
                string returnVal = UtilModel.MakeAsyncRequest("http://e-pick.net/api/v1/gameInfo", "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);

                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("true"))
                {
                    string epickCode = EPICKGameCode(gamecode);
                    JObject jo = JObject.Parse(returnVal);
                    // "GAME_CNBALL3": 304,
                    string data_gameInning = jo.SelectToken("data.GAME_" + epickCode).ToString();
                    if (data_gameInning.Equals(gameInning.ToString()))
                    {
                        logger.Error("회차 정보 다름 data_gameInning : " + data_gameInning + " || " + gameInning.ToString());
                        return false;
                    }
                    int normal = int.Parse(jo.SelectToken("data.NUM1_" + epickCode).ToString())
                        + int.Parse(jo.SelectToken("data.NUM2_" + epickCode).ToString())
                        + int.Parse(jo.SelectToken("data.NUM3_" + epickCode).ToString())
                        + int.Parse(jo.SelectToken("data.NUM4_" + epickCode).ToString())
                        + int.Parse(jo.SelectToken("data.NUM5_" + epickCode).ToString());
                    int power = int.Parse(jo.SelectToken("data.NUM6_" + epickCode).ToString());

                    if (power % 2 == 0) // 짝수 = 우
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = "짝";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = "짝";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = "짝";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = "짝";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = "홀";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = "홀";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = "홀";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = "홀";
                        }
                    }
                    if (power > 4) // 짝수 = 우
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_2 = "오";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_2 = "오";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_2 = "오";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_2 = "오";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_2 = "언";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_2 = "언";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_2 = "언";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_2 = "언";
                        }
                    }
                    if (normal % 2 == 0) // 짝수 = 우
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_3 = "짝";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_3 = "짝";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_3 = "짝";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_3 = "짝";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_3 = "홀";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_3 = "홀";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_3 = "홀";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_3 = "홀";
                        }
                    }
                    if (normal > 72) // 짝수 = 우
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_4 = "오";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_4 = "오";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_4 = "오";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_4 = "오";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_4 = "언";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_4 = "언";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_4 = "언";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_4 = "언";
                        }
                    }
                    if (gameType == 1)
                    {
                        txtLogAdd(txtLog1, "[" + gameInning + "회] 파워볼 : " + Game1_Result_1 + " || " + Game1_Result_2 + " || " + Game1_Result_3 + " || " + Game1_Result_4, Color.Black);
                    }
                    else if (gameType == 3)
                    {
                        txtLogAdd(txtLog3, "[" + gameInning + "회] 파워볼 : " + Game3_Result_1 + " || " + Game3_Result_2 + " || " + Game3_Result_3 + " || " + Game3_Result_4, Color.Black);
                    }
                    else if (gameType == 5)
                    {
                        txtLogAdd(txtLog5, "[" + gameInning + "회] 파워볼 : " + Game5_Result_1 + " || " + Game5_Result_2 + " || " + Game5_Result_3 + " || " + Game5_Result_4, Color.Black);
                    }
                    else if (gameType == 6)
                    {
                        txtLogAdd(txtLog6, "[" + gameInning + "회] 파워볼 : " + Game6_Result_1 + " || " + Game6_Result_2 + " || " + Game6_Result_3 + " || " + Game6_Result_4, Color.Black);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                logger.Error("loadEPickResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        */
        private Boolean loadBepickResultForLadder(int gameType, string gamecode, int gameInning)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://bepick.net/live/result/{0}", BepickGameCode(gamecode));
                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;

                logger.Info(returnVal);
                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("fd1"))
                {
                    JObject jo = JObject.Parse(returnVal);

                    if (!jo.SelectToken("round").ToString().Equals(gameInning.ToString()))
                    {
                        return false;
                    }
                    if (jo.SelectToken("fd1").ToString().Equals("1"))
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_1 = "좌";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_1 = "좌";
                        }
                    }
                    else
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_1 = "우";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_1 = "우";
                        }
                    }
                    if (jo.SelectToken("fd2").ToString().Equals("1"))
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_2 = "삼";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_2 = "삼";
                        }
                    }
                    else
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_2 = "사";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_2 = "사";
                        }
                    }
                    if (jo.SelectToken("fd3").ToString().Equals("1"))
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_3 = "홀";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_3 = "홀";
                        }
                    }
                    else
                    {
                        if (gameType == 2)
                        {
                            Game2_Result_3 = "짝";
                        }
                        else if (gameType == 4)
                        {
                            Game4_Result_3 = "짝";
                        }
                    }
                    if (gameType == 2)
                    {
                        txtLogAdd(txtLog2, "[" + (gameInning) + "회] 사다리 : " + Game2_Result_1 + " || " + Game2_Result_2 + " || " + Game2_Result_3, Color.Black);
                    }
                    else if (gameType == 4)
                    {
                        txtLogAdd(txtLog4, "[" + (gameInning) + "회] 사다리 : " + Game4_Result_1 + " || " + Game4_Result_2 + " || " + Game4_Result_3, Color.Black);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadBepickResultForLadder || " + _ex.ToString());
                return false;
            }
        }

        private Boolean loadBepickResultForPowerBallGame(int gameType, string gamecode, int gameInning)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://bepick.net/live/result/{0}", BepickGameCode(gamecode));
                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;

                logger.Info(returnVal);
                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("fd1"))
                {
                    JObject jo = JObject.Parse(returnVal);
                    if (!jo.SelectToken("round").ToString().Equals(gameInning.ToString()))
                    {
                        logger.Error("회차 정보 다름 loadBepickResultForPowerBallGame data_gameInning : " + jo.SelectToken("round").ToString() + " || " + gameInning.ToString());
                        return false;
                    }
                    if (jo.SelectToken("fd1").ToString().Equals("1"))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = "홀";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = "홀";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = "홀";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = "홀";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = "짝";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = "짝";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = "짝";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = "짝";
                        }
                    }
                    if (jo.SelectToken("fd2").ToString().Equals("1"))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_2 = "언";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_2 = "언";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_2 = "언";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_2 = "언";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_2 = "오";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_2 = "오";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_2 = "오";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_2 = "오";
                        }
                    }
                    if (jo.SelectToken("fd3").ToString().Equals("1"))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_3 = "홀";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_3 = "홀";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_3 = "홀";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_3 = "홀";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_3 = "짝";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_3 = "짝";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_3 = "짝";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_3 = "짝";
                        }
                    }
                    if (jo.SelectToken("fd4").ToString().Equals("1"))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_4 = "언";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_4 = "언";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_4 = "언";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_4 = "언";
                        }
                    }
                    else
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_4 = "오";
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_4 = "오";
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_4 = "오";
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_4 = "오";
                        }
                    }
                    if (gameType == 1)
                    {
                        txtLogAdd(txtLog1, "[" + (gameInning) + "회] " + Game1_Result_1 + " || " + Game1_Result_2 + " || " + Game1_Result_3 + " || " + Game1_Result_4, Color.Black);
                    }
                    else if (gameType == 3)
                    {
                        txtLogAdd(txtLog3, "[" + (gameInning) + "회] " + Game3_Result_1 + " || " + Game3_Result_2 + " || " + Game3_Result_3 + " || " + Game3_Result_4, Color.Black);
                    }
                    else if (gameType == 5)
                    {
                        txtLogAdd(txtLog5, "[" + (gameInning) + "회] " + Game5_Result_1 + " || " + Game5_Result_2 + " || " + Game5_Result_3 + " || " + Game5_Result_4, Color.Black);
                    }
                    else if (gameType == 6)
                    {
                        txtLogAdd(txtLog6, "[" + (gameInning) + "회] " + Game6_Result_1 + " || " + Game6_Result_2 + " || " + Game6_Result_3 + " || " + Game6_Result_4, Color.Black);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                logger.Error("loadBepickResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        /******************************************************************************
        private Boolean loadScoreGameResultForPowerBallGame(string gamecode, int gameInning)
        {
            // EOS 게임이 없다
            // https://www.scoregame.co.kr/api/hounslow-3-powerball/round?&apiId=id_xhfqfnna
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://www.scoregame.co.kr/api/{0}/round?&apiId=id_xhfqfnna", ScoreGameGameCode(gamecode));

                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);
                if (returnVal.Contains("idx"))
                {
                    JObject jo = JObject.Parse(returnVal);

                    if (jo.SelectToken("todayRound").ToString().Equals(gameInning.ToString()))
                    {
                        Game1_Result_1 = jo.SelectToken("powerballOddEven.title").ToString();
                        Game1_Result_2 = jo.SelectToken("powerballUnderOver.title").ToString().Replace("더", "").Replace("버", "");
                        Game1_Result_3 = jo.SelectToken("normalballOddEven.title").ToString();
                        Game1_Result_4 = jo.SelectToken("normalballUnderOver.title").ToString().Replace("더", "").Replace("버", "");
                        txtLogAdd(txtLog1, "[" + (gameInning) + "회] 파워볼 : " + Game1_Result_1 + " || " + Game1_Result_2 + " || " + Game1_Result_3 + " || " + Game1_Result_4, Color.Black);
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadScoreGameResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        private Boolean loadScoreGameResultForLadder(string gamecode, int gameInning)
        {
            // EOS 게임이 없다
            // https://www.scoregame.co.kr/api/hounslow-3-powerball/round?&apiId=id_xhfqfnna
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://www.scoregame.co.kr/api/{0}/round?&apiId=id_xhfqfnna", ScoreGameGameCode(gamecode));
                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);
                if (returnVal.Contains("idx"))
                {
                    JObject jo = JObject.Parse(returnVal);

                    if (jo.SelectToken("todayRound").ToString().Equals(gameInning.ToString()))
                    {
                        Game2_Result_1 = jo.SelectToken("ladderStart.title").ToString();
                        if (jo.SelectToken("ladderLine").ToString().Equals("3"))
                        {
                            Game2_Result_2 = "삼";
                        }
                        else
                        {
                            Game2_Result_2 = "사";
                        }
                        Game2_Result_3 = jo.SelectToken("ladderResult.title").ToString();
                        txtLogAdd(txtLog1, "[" + (gameInning) + "회] 사다리게임: " + Game2_Result_1 + " || " + Game2_Result_2 + " || " + Game2_Result_3, Color.Black);
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadScoreGameResultForLadder || " + _ex.ToString());
                return false;
            }
        }
        *********************************************************************/
        private Boolean loadUpDownResultForLadder(int gameType, string gamecode, int gameInning)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://updown2.com/api/last?g_type={0}&_={1}", UpDownGameCode(gamecode), DateTimeOffset.Now.ToUnixTimeSeconds());
                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);
                if (returnVal.Contains("성공"))
                {
                    JObject jo = JObject.Parse(returnVal);

                    if (jo.SelectToken("date_round").ToString().Equals(gameInning.ToString()))
                    {
                        if (jo.SelectToken("lr").ToString().Equals("좌"))
                        {
                            Game2_Result_1 = "좌";
                        }
                        else
                        {
                            Game2_Result_1 = "우";
                        }
                        if (jo.SelectToken("line").ToString().Equals("3"))
                        {
                            Game2_Result_2 = "삼";
                        }
                        else
                        {
                            Game2_Result_2 = "사";
                        }
                        if (jo.SelectToken("oe").ToString().Equals("홀"))
                        {
                            Game2_Result_3 = "홀";
                        }
                        else
                        {
                            Game2_Result_3 = "짝";
                        }
                        txtLogAdd(txtLog2, "[" + (gameInning) + "회] 사다리 : " + Game2_Result_1 + " || " + Game2_Result_2 + " || " + Game2_Result_3, Color.Black);
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadUpDownResultForLadder || " + _ex.ToString());
                return false;
            }
        }
        private Boolean loadUpDownResultForPowerBallGame(int gameType, string gamecode, int gameInning)
        {
            try
            {
                // https://updown2.com/api/last?g_type=coinladder3&_=1663507276707
                // {"error":false,"msg":"성공","g_date":"2022-09-18","date_round":448,"lr":"좌","line":"3","oe":"짝"}
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://updown2.com/api/last?g_type={0}&_={1}", UpDownGameCode(gamecode), DateTimeOffset.Now.ToUnixTimeSeconds());
                string returnVal = null;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnVal = webClient.DownloadString(stringBuilder.ToString());
                }
                logger.Info(returnVal);
                if (returnVal.Contains("성공"))
                {
                    JObject jo = JObject.Parse(returnVal);
                    if (jo.SelectToken("date_round").ToString().Equals(gameInning.ToString()))
                    {
                        if (gameType == 1)
                        {
                            Game1_Result_1 = jo.SelectToken("p_oe").ToString();
                            Game1_Result_2 = jo.SelectToken("p_uo").ToString().Replace("더", "").Replace("버", "");
                            Game1_Result_3 = jo.SelectToken("n_oe").ToString();
                            Game1_Result_4 = jo.SelectToken("n_uo").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog1, "[" + (gameInning) + "회] " + Game1_Result_1 + " || " + Game1_Result_2 + " || " + Game1_Result_3 + " || " + Game1_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 3)
                        {
                            Game3_Result_1 = jo.SelectToken("p_oe").ToString();
                            Game3_Result_2 = jo.SelectToken("p_uo").ToString().Replace("더", "").Replace("버", "");
                            Game3_Result_3 = jo.SelectToken("n_oe").ToString();
                            Game3_Result_4 = jo.SelectToken("n_uo").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog3, "[" + (gameInning) + "회] " + Game3_Result_1 + " || " + Game3_Result_2 + " || " + Game3_Result_3 + " || " + Game3_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 5)
                        {
                            Game5_Result_1 = jo.SelectToken("p_oe").ToString();
                            Game5_Result_2 = jo.SelectToken("p_uo").ToString().Replace("더", "").Replace("버", "");
                            Game5_Result_3 = jo.SelectToken("n_oe").ToString();
                            Game5_Result_4 = jo.SelectToken("n_uo").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog5, "[" + (gameInning) + "회] " + Game5_Result_1 + " || " + Game5_Result_2 + " || " + Game5_Result_3 + " || " + Game5_Result_4, Color.Black);
                            return true;
                        }
                        else if (gameType == 6)
                        {
                            Game6_Result_1 = jo.SelectToken("p_oe").ToString();
                            Game6_Result_2 = jo.SelectToken("p_uo").ToString().Replace("더", "").Replace("버", "");
                            Game6_Result_3 = jo.SelectToken("n_oe").ToString();
                            Game6_Result_4 = jo.SelectToken("n_uo").ToString().Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog6, "[" + (gameInning) + "회] " + Game6_Result_1 + " || " + Game6_Result_2 + " || " + Game6_Result_3 + " || " + Game6_Result_4, Color.Black);
                            return true;
                        }
                    }
                    logger.Error(stringBuilder.ToString());
                    logger.Error("회차 정보 다름 loadUpDownResultForPowerBallGame data_gameInning : " + jo.SelectToken("date_round").ToString() + " || " + gameInning.ToString());
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error("loadUpDownResultForPowerBallGame || " + _ex.ToString());
                return false;
            }
        }
        delegate void TimerEventFiredDelegate();
        void Game1_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game1_Timer_Tick));
        }
        /******************************************************************************
        void Game2_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game2_Timer_Tick));
        }
        *****************************************************************************/
        void Game3_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game3_Timer_Tick));
        }
        /******************************************************************************
        void Game4_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game4_Timer_Tick));
        }
        *****************************************************************************/
        void Game5_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game5_Timer_Tick));
        }
        void Game6_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game6_Timer_Tick));
        }

        private int callResultInning(int gameType, String GCode)
        {
            int round = 0;
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                if (gameType == 1 && dayroundNo_1 == 1
                    || gameType == 2 && dayroundNo_2 == 1
                    || gameType == 3 && dayroundNo_3 == 1
                    || gameType == 4 && dayroundNo_4 == 1
                    || gameType == 5 && dayroundNo_5 == 1
                    || gameType == 6 && dayroundNo_6 == 1)
                {
                    if (GCode.Equals("EPB"))
                    {
                        return 288;
                    }
                    else if (GCode.Contains("2"))
                    {
                        return 720;
                    }
                    else if (GCode.Contains("3"))
                    {
                        return 480;
                    }
                    else if (GCode.Contains("5"))
                    {
                        return 288;
                    }
                    else
                    {
                        return 288;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("EPB") || GCode.Equals("DSCP5") || GCode.Equals("EPB3") || GCode.Equals("DSCP3") || GCode.Equals("HSP3") || GCode.Equals("HSP5") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else if (GCode.Equals("CSA3") || GCode.Equals("CSA5") || GCode.Equals("HSPSA3") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else
                    {
                        return 288;
                    }
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                if (gameType == 1 && dayroundNo_1 == 1
                    || gameType == 2 && dayroundNo_2 == 1
                    || gameType == 3 && dayroundNo_3 == 1
                    || gameType == 4 && dayroundNo_4 == 1
                    || gameType == 5 && dayroundNo_5 == 1
                    || gameType == 6 && dayroundNo_6 == 1)
                {
                    if (GCode.Equals("EPB"))
                    {
                        return 288;
                    }
                    else if (GCode.Contains("2"))
                    {
                        return 720;
                    }
                    else if (GCode.Contains("3"))
                    {
                        return 480;
                    }
                    else if (GCode.Contains("5"))
                    {
                        return 288;
                    }
                    else
                    {
                        return 288;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("EPB") || GCode.Equals("DSCP5") || GCode.Equals("EPB3") || GCode.Equals("DSCP3") || GCode.Equals("HSP3") || GCode.Equals("HSP5") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else if (GCode.Equals("CSA3") || GCode.Equals("CSA5") || GCode.Equals("HSPSA3") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else
                    {
                        return 288;
                    }
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                if (gameType == 1 && dayroundNo_1 == 1
                    || gameType == 2 && dayroundNo_2 == 1
                    || gameType == 3 && dayroundNo_3 == 1
                    || gameType == 4 && dayroundNo_4 == 1
                    || gameType == 5 && dayroundNo_5 == 1
                    || gameType == 6 && dayroundNo_6 == 1)
                {
                    if (GCode.Contains("2"))
                    {
                        return 720;
                    }
                    else if (GCode.Contains("3"))
                    {
                        return 480;
                    }
                    else if (GCode.Contains("5"))
                    {
                        return 288;
                    }
                    else
                    {
                        return 480;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("DSCB3_NM_PB") || GCode.Equals("DSCB5_NM_PB") || GCode.Equals("EOSP3_NM_PB") || GCode.Equals("EOSP5_NM_PB") || GCode.Equals("HSPB3_NM_PB") || GCode.Equals("HSPB5_NM_PB") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5") || GCode.Equals("BEXB_NM_PB"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else if (GCode.Equals("DSCL3_NM") || GCode.Equals("DSCL5_NM") || GCode.Equals("HSPL3_NM") || GCode.Equals("HSPL5_NM") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5") || GCode.Equals("BEXL_NM"))
                    {
                        if (gameType == 1)
                        {
                            round = dayroundNo_1 - 1;
                        }
                        else if (gameType == 2)
                        {
                            round = dayroundNo_2 - 1;
                        }
                        else if (gameType == 3)
                        {
                            round = dayroundNo_3 - 1;
                        }
                        else if (gameType == 4)
                        {
                            round = dayroundNo_4 - 1;
                        }
                        else if (gameType == 5)
                        {
                            round = dayroundNo_5 - 1;
                        }
                        else if (gameType == 6)
                        {
                            round = dayroundNo_6 - 1;
                        }
                        return round;
                    }
                    else
                    {
                        return 480;
                    }
                }
            }
            return -1;
        }

        private int betTime(string GCode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                if (GCode.Equals("DSCP3") || GCode.Equals("EPB3") || GCode.Equals("HSP3") || GCode.Equals("CSA3") || GCode.Equals("HSPSA3"))
                {
                    return 180;
                }
                else if (GCode.Equals("DSCP5") || GCode.Equals("EPB") || GCode.Equals("HSP5") || GCode.Equals("KLAY5")
                    || GCode.Equals("CSA5") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA5"))
                {
                    return 300;
                }
                else
                {
                    return 120;
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                if (GCode.Equals("DSCP3") || GCode.Equals("EPB3") || GCode.Equals("HSP3") || GCode.Equals("CSA3") || GCode.Equals("HSPSA3"))
                {
                    return 180;
                }
                else if (GCode.Equals("DSCP5") || GCode.Equals("EPB") || GCode.Equals("HSP5") || GCode.Equals("KLAY5")
                    || GCode.Equals("CSA5") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA5"))
                {
                    return 300;
                }
                else
                {
                    return 120;
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                if (GCode.Equals("DSCB3_NM_PB") || GCode.Equals("EOSP3_NM_PB") || GCode.Equals("HSPB3_NM_PB") || GCode.Equals("BEXB_NM_PB")
                    || GCode.Equals("DSCL3_NM") || GCode.Equals("HSPL3_NM") || GCode.Equals("BEXL_NM"))
                {
                    return 180;
                }
                else if (GCode.Equals("DSCB5_NM_PB") || GCode.Equals("EOSP5_NM_PB") || GCode.Equals("HSPB5_NM_PB") || GCode.Equals("KLAY5")
                    || GCode.Equals("DSCL5_NM") || GCode.Equals("HSPL5_NM") || GCode.Equals("KALYSA5"))
                {
                    return 300;
                }
                else
                {
                    return 120;
                }
            }
            return -1;
        }

        Random rand = new Random();
        int Game1_Random_Result_Load_Time_For_PowerBall = 0;
        int Game1_Random_Bet_Regist_Time_For_PowerBall = 0;

        int Game2_Random_Result_Load_Time_For_Ladder = 0;
        int Game2_Random_Bet_Regist_Time_For_Ladder = 0;

        int Game3_Random_Result_Load_Time_For_PowerBall = 0;
        int Game3_Random_Bet_Regist_Time_For_PowerBall = 0;

        int Game4_Random_Result_Load_Time_For_Ladder = 0;
        int Game4_Random_Bet_Regist_Time_For_Ladder = 0;

        int Game5_Random_Result_Load_Time_For_PowerBall = 0;
        int Game5_Random_Bet_Regist_Time_For_PowerBall = 0;

        int Game6_Random_Result_Load_Time_For_PowerBall = 0;
        int Game6_Random_Bet_Regist_Time_For_PowerBall = 0;

        /* 1초마다 체크할 사항
        **************************************
        가장 중요한 부분이다.
        **************************************
        *
        */
        private void Game3_Timer_Tick()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            // txtLog1. txtLog2 삭제
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 3600 == 0)
            {
                txtLog3.Clear();
            }

            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 360 == 120)
            {
                try
                {
                    Boolean siteIdB = false;
                    String serverCheckTime = String.Empty;
                    JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                    JToken jTokenServers = jObject["Servers"];

                    if (jTokenServers == null || jTokenServers.Type == JTokenType.Null)
                    {
                        txtLogAdd(txtLog6, "정보를 읽어오는데 실패", Color.Black);
                    }
                    else
                    {
                        foreach (JToken members in jTokenServers)
                        {
                            if (UtilModel.UserSiteUrlAddress.Contains(members["site"].ToString())
                                && members["servercheck"].ToString().Equals("1"))
                            {
                                serverCheckTime = members["serverchecktime"].ToString();
                                siteIdB = true;
                                break;
                            }
                        }
                        if (siteIdB == true)
                        {
                            serverCheckTimeLabel.Text = serverCheckTime;
                        }
                        else
                        {
                            serverCheckTimeLabel.Text = "";
                        }

                        UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
                        UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
                        UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
                        UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            int totalSeconds = (int)time.TotalSeconds;
            if (StartGame_3)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_3;
                Game3_PastTimer.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                Game3_Start_Button.Text = "[" + roundNo_3 + "] 회 배팅이 진행 중입니다.";

                if (Game_WinToStopCheckBox.Checked
                    && !Game3_PowerBallOddEvenUseCheck.Checked
                        && !Game3_PowerBallUnderOverUseCheck.Checked
                        && !Game3_NormalBallOddEvenUseCheck.Checked
                        && !Game3_NormalBallUnderOverUseCheck.Checked)
                {
                    logger.Info("[프로그램 정지] 모든 게임 당첨으로 종료");
                    Game3_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    Game3_Start_Button.Text = "배팅이 정지되었습니다.";
                    StartGame_3 = false;

                    Game3_Result_3 = string.Empty;
                    Game3_Result_2 = string.Empty;
                    Game3_Result_3 = string.Empty;
                    Game3_Result_4 = string.Empty;

                    Game3_CruisePowerBallOddEvenLevelChange.Text = "1";
                    Game3_CruisePowerBallUnderOverLevelChange.Text = "1";
                    Game3_CruiseNormalBallOddEvenLevelChange.Text = "1";
                    Game3_CruiseNormalBallUnderOverLevelChange.Text = "1";

                    Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
                    Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
                    Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
                    Game3_CruiseBetNormalBallUnderOverSubLevel = 1;

                    Game3_Betting_Mode_Result_Process = false;
                    Game_3_Betting_Complete_Status = false;
                    Game_3_Result_Load_Complete = false;
                    Game_3_Check_Complete = false;
                    Game3_BetMoney = new int[] { 0, 0, 0, 0 };
                    Game3_BetPick = new string[] { null, null, null, null };

                    Game3_RemainingTimer.Stop();
                    //MessageBox.Show("[프로그램 정지] " + Game3_GameSelectComboBox.Text + " || " + roundNo_3 + " || 당첨시 사용정지로 인해 프로그램을 정지합니다.", "[파워볼게임] 모든 게임 당첨");
                }

                if (string.IsNullOrEmpty(GameCode_3))
                {
                    BetRemainingTime_3 = 300 - (int)DateTime.Now.TimeOfDay.TotalSeconds % 300;
                    roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_3, BetRemainingTime_3);
                    txtLogAdd(txtLog3, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인파워볼 3분
                        if (GameCode_3.Contains("DSCP3"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_3.Contains("DSCP5"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_3.Contains("EPB3"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_3.Contains("EPB"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_3.Contains("HSP3"))
                        {
                            BetRemainingTime_3 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_3.Contains("HSP5"))
                        {
                            BetRemainingTime_3 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_3.Contains("KLAY2"))
                        {
                            BetRemainingTime_3 = 120 - totalSeconds % 120 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_3.Contains("KLAY5"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_3.Contains("BEXB"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_3 = 10 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인파워볼 3분
                        if (GameCode_3.Contains("DSCP3"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_3.Contains("DSCP5"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_3.Contains("EPB3"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_3.Contains("EPB"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_3.Contains("HSP3"))
                        {
                            BetRemainingTime_3 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_3.Contains("HSP5"))
                        {
                            BetRemainingTime_3 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_3.Contains("KLAY2"))
                        {
                            BetRemainingTime_3 = 120 - totalSeconds % 120 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_3.Contains("KLAY5"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_3.Contains("BEXB"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_3 = 10 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인파워볼 3분
                        if (GameCode_3.Contains("DSCB3_NM_PB"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_3.Contains("DSCB5_NM_PB"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_3.Contains("EOSP3_NM_PB"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_3.Contains("EOSP5_NM_PB"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_3.Contains("HSPB3_NM_PB"))
                        {
                            BetRemainingTime_3 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_3.Contains("HSPB5_NM_PB"))
                        {
                            BetRemainingTime_3 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_3.Contains("KLAY2"))
                        {
                            BetRemainingTime_3 = 120 - totalSeconds % 120 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_3.Contains("KLAY5"))
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_3.Contains("BEXB_NM_PB"))
                        {
                            BetRemainingTime_3 = 180 - totalSeconds % 180 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_3 = 300 - totalSeconds % 300 + 1;
                            roundNo_3 = dayroundNo_3 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime_3, BetRemainingTime_3);

                    if (BetRemainingTime_3 > (betTime(GameCode_3) - 10))
                    {
                        Game3_Betting_Mode_Result_Process = false;
                        Game_3_Result_Load_Complete = false;
                        Game_3_Check_Complete = false;
                        Game_3_Betting_Complete_Status = false;
                    }

                    // 게임 1의 결과값을 불러온다.
                    if (!Game_3_Result_Load_Complete && (BetRemainingTime_3 % 3 == 0) && (BetRemainingTime_3 < Game3_Random_Result_Load_Time_For_PowerBall) && (BetRemainingTime_3 > BetEndSec_3))
                    {
                        Game_3_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_3_Betting_Complete_Status && Game_3_Result_Load_Complete && !Game_3_Check_Complete && (BetRemainingTime_3 > BetEndSec_3))
                    {
                        Game_3_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_3_Betting_Complete_Status && Game_3_Result_Load_Complete && (BetRemainingTime_3 % 5 == 0) && (BetRemainingTime_3 < Game3_Random_Bet_Regist_Time_For_PowerBall) && (BetRemainingTime_3 > BetEndSec_3))
                    {
                        Game3_Bet_Processing_AllSum();
                        if (virtualMoney.Checked)
                        {
                            Game3_Bet_Processing_RegistListView();
                            Game_3_Betting_Complete_Status = true;
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game3_BetMoney[0] - Game3_BetMoney[1] - Game3_BetMoney[2] - Game3_BetMoney[3];
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                        else
                        {
                            if (Game_Bet_Processing_Final(Game3_BetPick, Game3_BetMoney, roundNo_3, BetRemainingTime_3, txtLog3, BetEndSec_3, GameCode_3, Random_Nonce_3))
                            {
                                Game3_Bet_Processing_RegistListView();
                                Game_3_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_3_Betting_Complete_Status && BetRemainingTime_3 < BetEndSec_3)
                    {
                        int sum = Game3_BetMoney[0] + Game3_BetMoney[1] + Game3_BetMoney[2] + Game3_BetMoney[3];
                        if (sum > 0)
                        {
                            logger.Info("[프로그램 정지] " + Game3_GameSelectComboBox.Text + " || " + roundNo_3 + " || " + Game3_BetPick[0] + " || " + Game3_BetMoney[0] + " || " + Game3_BetPick[1] + " || " + Game3_BetMoney[1] + " || " + Game3_BetPick[2] + " || " + Game3_BetMoney[2] + " || " + Game3_BetPick[3] + " || " + Game3_BetMoney[3]);
                            Game3_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            Game3_Start_Button.Text = "배팅이 정지되었습니다.";
                            StartGame_3 = false;

                            Game3_Result_1 = string.Empty;
                            Game3_Result_2 = string.Empty;
                            Game3_Result_3 = string.Empty;
                            Game3_Result_4 = string.Empty;

                            Game3_CruisePowerBallOddEvenLevelChange.Text = "1";
                            Game3_CruisePowerBallUnderOverLevelChange.Text = "1";
                            Game3_CruiseNormalBallOddEvenLevelChange.Text = "1";
                            Game3_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game3_CruiseBetNormalBallUnderOverSubLevel = 1;

                            Game3_Betting_Mode_Result_Process = false;

                            Game_3_Betting_Complete_Status = false;

                            Game_3_Result_Load_Complete = false;

                            Game_3_Check_Complete = false;

                            Game3_BetMoney = new int[] { 0, 0, 0, 0 };

                            Game3_BetPick = new string[] { null, null, null, null };

                            Game3_RemainingTimer.Stop();
                            MessageBox.Show("[프로그램 정지] " + comboBox1.Text + " || " + roundNo_3 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[파워볼게임] 배팅실패");
                        }
                        else
                        {
                            Game3_BetMoney = new int[] { 0, 0, 0, 0 };
                            Game3_BetPick = new string[] { null, null, null, null }; ;
                            Game3_Bet_Processing_RegistListView();
                            Game_3_Betting_Complete_Status = true;
                        }
                    }
                }
            }
        }
        private void Game5_Timer_Tick()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            // txtLog1. txtLog2 삭제
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 3600 == 0)
            {
                txtLog5.Clear();
            }
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 360 == 240)
            {
                try
                {
                    Boolean siteIdB = false;
                    String serverCheckTime = String.Empty;
                    JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                    JToken jTokenServers = jObject["Servers"];

                    if (jTokenServers == null || jTokenServers.Type == JTokenType.Null)
                    {
                        txtLogAdd(txtLog6, "정보를 읽어오는데 실패", Color.Black);
                    }
                    else
                    {
                        foreach (JToken members in jTokenServers)
                        {
                            if (UtilModel.UserSiteUrlAddress.Contains(members["site"].ToString())
                                && members["servercheck"].ToString().Equals("1"))
                            {
                                serverCheckTime = members["serverchecktime"].ToString();
                                siteIdB = true;
                                break;
                            }
                        }
                        if (siteIdB == true)
                        {
                            serverCheckTimeLabel.Text = serverCheckTime;
                        }
                        else
                        {
                            serverCheckTimeLabel.Text = "";
                        }

                        UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
                        UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
                        UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
                        UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            int totalSeconds = (int)time.TotalSeconds;
            if (StartGame_5)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_5;
                Game5_PastTimer.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                Game5_Start_Button.Text = "[" + roundNo_5 + "] 회 배팅이 진행 중입니다.";

                if (Game_WinToStopCheckBox.Checked
                    && !Game5_PowerBallOddEvenUseCheck.Checked
                        && !Game5_PowerBallUnderOverUseCheck.Checked
                        && !Game5_NormalBallOddEvenUseCheck.Checked
                        && !Game5_NormalBallUnderOverUseCheck.Checked)
                {
                    logger.Info("[프로그램 정지] 모든 게임 당첨으로 종료");
                    Game5_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    Game5_Start_Button.Text = "배팅이 정지되었습니다.";
                    StartGame_5 = false;

                    Game5_Result_3 = string.Empty;
                    Game5_Result_2 = string.Empty;
                    Game5_Result_3 = string.Empty;
                    Game5_Result_4 = string.Empty;

                    Game5_CruisePowerBallOddEvenLevelChange.Text = "1";
                    Game5_CruisePowerBallUnderOverLevelChange.Text = "1";
                    Game5_CruiseNormalBallOddEvenLevelChange.Text = "1";
                    Game5_CruiseNormalBallUnderOverLevelChange.Text = "1";

                    Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
                    Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
                    Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
                    Game5_CruiseBetNormalBallUnderOverSubLevel = 1;

                    Game5_Betting_Mode_Result_Process = false;
                    Game_5_Betting_Complete_Status = false;
                    Game_5_Result_Load_Complete = false;
                    Game_5_Check_Complete = false;
                    Game5_BetMoney = new int[] { 0, 0, 0, 0 };
                    Game5_BetPick = new string[] { null, null, null, null };

                    Game5_RemainingTimer.Stop();
                }

                if (string.IsNullOrEmpty(GameCode_5))
                {
                    BetRemainingTime_5 = 300 - (int)DateTime.Now.TimeOfDay.TotalSeconds % 300;
                    roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_5, BetRemainingTime_5);
                    txtLogAdd(txtLog5, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인파워볼 3분
                        if (GameCode_5.Contains("DSCP3"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_5.Contains("DSCP5"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_5.Contains("EPB3"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_5.Contains("EPB"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_5.Contains("HSP3"))
                        {
                            BetRemainingTime_5 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_5.Contains("HSP5"))
                        {
                            BetRemainingTime_5 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_5.Contains("KLAY2"))
                        {
                            BetRemainingTime_5 = 120 - totalSeconds % 120 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_5.Contains("KLAY5"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_5.Contains("BEXB"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_5 = 10 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인파워볼 3분
                        if (GameCode_5.Contains("DSCP3"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_5.Contains("DSCP5"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_5.Contains("EPB3"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_5.Contains("EPB"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_5.Contains("HSP3"))
                        {
                            BetRemainingTime_5 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_5.Contains("HSP5"))
                        {
                            BetRemainingTime_5 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_5.Contains("KLAY2"))
                        {
                            BetRemainingTime_5 = 120 - totalSeconds % 120 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_5.Contains("KLAY5"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_5.Contains("BEXB"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_5 = 10 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인파워볼 3분
                        if (GameCode_5.Contains("DSCB3_NM_PB"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_5.Contains("DSCB5_NM_PB"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_5.Contains("EOSP3_NM_PB"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_5.Contains("EOSP5_NM_PB"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_5.Contains("HSPB3_NM_PB"))
                        {
                            BetRemainingTime_5 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_5.Contains("HSPB5_NM_PB"))
                        {
                            BetRemainingTime_5 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_5.Contains("KLAY2"))
                        {
                            BetRemainingTime_5 = 120 - totalSeconds % 120 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_5.Contains("KLAY5"))
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_5.Contains("BEXB_NM_PB"))
                        {
                            BetRemainingTime_5 = 180 - totalSeconds % 180 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_5 = 300 - totalSeconds % 300 + 1;
                            roundNo_5 = dayroundNo_5 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime_5, BetRemainingTime_5);

                    if (BetRemainingTime_5 > (betTime(GameCode_5) - 10))
                    {
                        Game5_Betting_Mode_Result_Process = false;
                        Game_5_Result_Load_Complete = false;
                        Game_5_Check_Complete = false;
                        Game_5_Betting_Complete_Status = false;
                    }

                    // 게임 1의 결과값을 불러온다.
                    if (!Game_5_Result_Load_Complete && (BetRemainingTime_5 % 3 == 0) && (BetRemainingTime_5 < Game5_Random_Result_Load_Time_For_PowerBall) && (BetRemainingTime_5 > BetEndSec_5))
                    {
                        Game_5_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_5_Betting_Complete_Status && Game_5_Result_Load_Complete && !Game_5_Check_Complete && (BetRemainingTime_5 > BetEndSec_5))
                    {
                        Game_5_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_5_Betting_Complete_Status && Game_5_Result_Load_Complete && (BetRemainingTime_5 % 5 == 0) && (BetRemainingTime_5 < Game5_Random_Bet_Regist_Time_For_PowerBall) && (BetRemainingTime_5 > BetEndSec_5))
                    {
                        Game5_Bet_Processing_AllSum();
                        if (virtualMoney.Checked)
                        {
                            Game5_Bet_Processing_RegistListView();
                            Game_5_Betting_Complete_Status = true;
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game5_BetMoney[0] - Game5_BetMoney[1] - Game5_BetMoney[2] - Game5_BetMoney[3];
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                        else
                        {
                            if (Game_Bet_Processing_Final(Game5_BetPick, Game5_BetMoney, roundNo_5, BetRemainingTime_5, txtLog5, BetEndSec_5, GameCode_5, Random_Nonce_5))
                            {
                                Game5_Bet_Processing_RegistListView();
                                Game_5_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_5_Betting_Complete_Status && BetRemainingTime_5 < BetEndSec_5)
                    {
                        int sum = Game5_BetMoney[0] + Game5_BetMoney[1] + Game5_BetMoney[2] + Game5_BetMoney[3];
                        if (sum > 0)
                        {
                            logger.Info("[프로그램 정지] " + Game5_GameSelectComboBox.Text + " || " + roundNo_5 + " || " + Game5_BetPick[0] + " || " + Game5_BetMoney[0] + " || " + Game5_BetPick[1] + " || " + Game5_BetMoney[1] + " || " + Game5_BetPick[2] + " || " + Game5_BetMoney[2] + " || " + Game5_BetPick[3] + " || " + Game5_BetMoney[3]);
                            Game5_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            Game5_Start_Button.Text = "배팅이 정지되었습니다.";
                            StartGame_5 = false;

                            Game5_Result_3 = string.Empty;
                            Game5_Result_2 = string.Empty;
                            Game5_Result_3 = string.Empty;
                            Game5_Result_4 = string.Empty;

                            Game5_CruisePowerBallOddEvenLevelChange.Text = "1";
                            Game5_CruisePowerBallUnderOverLevelChange.Text = "1";
                            Game5_CruiseNormalBallOddEvenLevelChange.Text = "1";
                            Game5_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game5_CruiseBetNormalBallUnderOverSubLevel = 1;

                            Game5_Betting_Mode_Result_Process = false;

                            Game_5_Betting_Complete_Status = false;

                            Game_5_Result_Load_Complete = false;

                            Game_5_Check_Complete = false;

                            Game5_BetMoney = new int[] { 0, 0, 0, 0 };

                            Game5_BetPick = new string[] { null, null, null, null };

                            Game5_RemainingTimer.Stop();
                            MessageBox.Show("[프로그램 정지] " + Game5_GameSelectComboBox.Text + " || " + roundNo_5 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[파워볼게임] 배팅실패");
                        }
                        else
                        {
                            Game5_BetMoney = new int[] { 0, 0, 0, 0 };
                            Game5_BetPick = new string[] { null, null, null, null }; ;
                            Game5_Bet_Processing_RegistListView();
                            Game_5_Betting_Complete_Status = true;
                        }
                    }
                }
            }
        }
        private void Game6_Timer_Tick()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            // txtLog1. txtLog2 삭제
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 3600 == 0)
            {
                txtLog6.Clear();
            }
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 360 == 300)
            {
                try
                {
                    Boolean siteIdB = false;
                    String serverCheckTime = String.Empty;
                    JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                    JToken jTokenServers = jObject["Servers"];

                    if (jTokenServers == null || jTokenServers.Type == JTokenType.Null)
                    {
                        txtLogAdd(txtLog6, "정보를 읽어오는데 실패", Color.Black);
                    }
                    else
                    {
                        foreach (JToken members in jTokenServers)
                        {
                            if (UtilModel.UserSiteUrlAddress.Contains(members["site"].ToString())
                                && members["servercheck"].ToString().Equals("1"))
                            {
                                serverCheckTime = members["serverchecktime"].ToString();
                                siteIdB = true;
                                break;
                            }
                        }
                        if (siteIdB == true)
                        {
                            serverCheckTimeLabel.Text = serverCheckTime;
                        }
                        else
                        {
                            serverCheckTimeLabel.Text = "";
                        }

                        UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
                        UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
                        UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
                        UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            int totalSeconds = (int)time.TotalSeconds;
            if (StartGame_6)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_5;
                Game6_PastTimer.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                Game6_Start_Button.Text = "[" + roundNo_6 + "] 회 배팅이 진행 중입니다.";

                if (Game_WinToStopCheckBox.Checked
                    && !Game6_PowerBallOddEvenUseCheck.Checked
                        && !Game6_PowerBallUnderOverUseCheck.Checked
                        && !Game6_NormalBallOddEvenUseCheck.Checked
                        && !Game6_NormalBallUnderOverUseCheck.Checked)
                {
                    logger.Info("[프로그램 정지] 모든 게임 당첨으로 종료");
                    Game6_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    Game6_Start_Button.Text = "배팅이 정지되었습니다.";
                    StartGame_6 = false;

                    Game6_Result_3 = string.Empty;
                    Game6_Result_2 = string.Empty;
                    Game6_Result_3 = string.Empty;
                    Game6_Result_4 = string.Empty;

                    Game6_CruisePowerBallOddEvenLevelChange.Text = "1";
                    Game6_CruisePowerBallUnderOverLevelChange.Text = "1";
                    Game6_CruiseNormalBallOddEvenLevelChange.Text = "1";
                    Game6_CruiseNormalBallUnderOverLevelChange.Text = "1";

                    Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
                    Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
                    Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
                    Game6_CruiseBetNormalBallUnderOverSubLevel = 1;

                    Game6_Betting_Mode_Result_Process = false;
                    Game_6_Betting_Complete_Status = false;
                    Game_6_Result_Load_Complete = false;
                    Game_6_Check_Complete = false;
                    Game6_BetMoney = new int[] { 0, 0, 0, 0 };
                    Game6_BetPick = new string[] { null, null, null, null };

                    Game6_RemainingTimer.Stop();
                }

                if (string.IsNullOrEmpty(GameCode_6))
                {
                    BetRemainingTime_6 = 300 - (int)DateTime.Now.TimeOfDay.TotalSeconds % 300;
                    roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_6, BetRemainingTime_6);
                    txtLogAdd(txtLog6, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인파워볼 3분
                        if (GameCode_6.Contains("DSCP3"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_6.Contains("DSCP5"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_6.Contains("EPB3"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_6.Contains("EPB"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_6.Contains("HSP3"))
                        {
                            BetRemainingTime_6 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_6.Contains("HSP5"))
                        {
                            BetRemainingTime_6 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_6.Contains("KLAY2"))
                        {
                            BetRemainingTime_6 = 120 - totalSeconds % 120 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_6.Contains("KLAY5"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_6.Contains("BEXB"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_6 = 10 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인파워볼 3분
                        if (GameCode_6.Contains("DSCP3"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_6.Contains("DSCP5"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_6.Contains("EPB3"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_6.Contains("EPB"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_6.Contains("HSP3"))
                        {
                            BetRemainingTime_6 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_6.Contains("HSP5"))
                        {
                            BetRemainingTime_6 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_6.Contains("KLAY2"))
                        {
                            BetRemainingTime_6 = 120 - totalSeconds % 120 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_6.Contains("KLAY5"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_6.Contains("BEXB"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_6 = 10 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인파워볼 3분
                        if (GameCode_6.Contains("DSCB3_NM_PB"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_6.Contains("DSCB5_NM_PB"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_6.Contains("EOSP3_NM_PB"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_6.Contains("EOSP5_NM_PB"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_6.Contains("HSPB3_NM_PB"))
                        {
                            BetRemainingTime_6 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_6.Contains("HSPB5_NM_PB"))
                        {
                            BetRemainingTime_6 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_6.Contains("KLAY2"))
                        {
                            BetRemainingTime_6 = 120 - totalSeconds % 120 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_6.Contains("KLAY5"))
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_6.Contains("BEXB_NM_PB"))
                        {
                            BetRemainingTime_6 = 180 - totalSeconds % 180 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_6 = 300 - totalSeconds % 300 + 1;
                            roundNo_6 = dayroundNo_6 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime_6, BetRemainingTime_6);

                    if (BetRemainingTime_6 > (betTime(GameCode_6) - 10))
                    {
                        Game6_Betting_Mode_Result_Process = false;
                        Game_6_Result_Load_Complete = false;
                        Game_6_Check_Complete = false;
                        Game_6_Betting_Complete_Status = false;
                    }

                    // 게임 1의 결과값을 불러온다.
                    if (!Game_6_Result_Load_Complete && (BetRemainingTime_6 % 3 == 0) && (BetRemainingTime_6 < Game6_Random_Result_Load_Time_For_PowerBall) && (BetRemainingTime_6 > BetEndSec_6))
                    {
                        Game_6_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_6_Betting_Complete_Status && Game_6_Result_Load_Complete && !Game_6_Check_Complete && (BetRemainingTime_6 > BetEndSec_6))
                    {
                        Game_6_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_6_Betting_Complete_Status && Game_6_Result_Load_Complete && (BetRemainingTime_6 % 5 == 0) && (BetRemainingTime_6 < Game6_Random_Bet_Regist_Time_For_PowerBall) && (BetRemainingTime_6 > BetEndSec_6))
                    {
                        Game6_Bet_Processing_AllSum();
                        if (virtualMoney.Checked)
                        {
                            Game6_Bet_Processing_RegistListView();
                            Game_6_Betting_Complete_Status = true;
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game6_BetMoney[0] - Game6_BetMoney[1] - Game6_BetMoney[2] - Game6_BetMoney[3];
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                        else
                        {
                            if (Game_Bet_Processing_Final(Game6_BetPick, Game6_BetMoney, roundNo_6, BetRemainingTime_6, txtLog6, BetEndSec_6, GameCode_6, Random_Nonce_6))
                            {
                                Game6_Bet_Processing_RegistListView();
                                Game_6_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_6_Betting_Complete_Status && BetRemainingTime_6 < BetEndSec_6)
                    {
                        int sum = Game6_BetMoney[0] + Game6_BetMoney[1] + Game6_BetMoney[2] + Game6_BetMoney[3];
                        if (sum > 0)
                        {
                            logger.Info("[프로그램 정지] " + Game6_GameSelectComboBox.Text + " || " + roundNo_6 + " || " + Game6_BetPick[0] + " || " + Game6_BetMoney[0] + " || " + Game6_BetPick[1] + " || " + Game6_BetMoney[1] + " || " + Game6_BetPick[2] + " || " + Game6_BetMoney[2] + " || " + Game6_BetPick[3] + " || " + Game6_BetMoney[3]);
                            Game6_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            Game6_Start_Button.Text = "배팅이 정지되었습니다.";
                            StartGame_6 = false;

                            Game6_Result_3 = string.Empty;
                            Game6_Result_2 = string.Empty;
                            Game6_Result_3 = string.Empty;
                            Game6_Result_4 = string.Empty;

                            Game6_CruisePowerBallOddEvenLevelChange.Text = "1";
                            Game6_CruisePowerBallUnderOverLevelChange.Text = "1";
                            Game6_CruiseNormalBallOddEvenLevelChange.Text = "1";
                            Game6_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game6_CruiseBetNormalBallUnderOverSubLevel = 1;

                            Game6_Betting_Mode_Result_Process = false;

                            Game_6_Betting_Complete_Status = false;

                            Game_6_Result_Load_Complete = false;

                            Game_6_Check_Complete = false;

                            Game6_BetMoney = new int[] { 0, 0, 0, 0 };

                            Game6_BetPick = new string[] { null, null, null, null };

                            Game6_RemainingTimer.Stop();

                            MessageBox.Show("[프로그램 정지] " + Game6_GameSelectComboBox.Text + " || " + roundNo_6 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[파워볼게임] 배팅실패");
                        }
                        else
                        {
                            Game6_BetMoney = new int[] { 0, 0, 0, 0 };
                            Game6_BetPick = new string[] { null, null, null, null }; ;
                            Game6_Bet_Processing_RegistListView();
                            Game_6_Betting_Complete_Status = true;
                        }
                    }
                }
            }
        }
        private void Game1_Timer_Tick()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            // txtLog1. txtLog2 삭제
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 3600 == 0)
            {
                txtLog1.Clear();
                txtLog2.Clear();
                txtLog4.Clear();
            }


            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 360 == 60)
            {
                try
                {
                    Boolean siteIdB = false;
                    String serverCheckTime = String.Empty;
                    JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                    JToken jTokenServers = jObject["Servers"];

                    if (jTokenServers == null || jTokenServers.Type == JTokenType.Null)
                    {
                        txtLogAdd(txtLog6, "정보를 읽어오는데 실패", Color.Black);
                    }
                    else
                    {
                        foreach (JToken members in jTokenServers)
                        {
                            if (UtilModel.UserSiteUrlAddress.Contains(members["site"].ToString())
                                && members["servercheck"].ToString().Equals("1"))
                            {
                                serverCheckTime = members["serverchecktime"].ToString();
                                siteIdB = true;
                                break;
                            }
                        }
                        if (siteIdB == true)
                        {
                            serverCheckTimeLabel.Text = serverCheckTime;
                        }
                        else
                        {
                            serverCheckTimeLabel.Text = "";
                        }

                        UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
                        UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
                        UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
                        UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            int totalSeconds = (int)time.TotalSeconds;
            /***************************************
            첫번째 페이지 게임 진행 : 파워볼 게임 종류
            ***************************************/
            if (StartGame_1)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_1;
                Game1_PastTimer.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                Game1_Start_Button.Text = "[" + roundNo_1 + "] 회 배팅이 진행 중입니다.";

                if (Game_WinToStopCheckBox.Checked
                    && !Game1_PowerBallOddEvenUseCheck.Checked
                        && !Game1_PowerBallUnderOverUseCheck.Checked
                        && !Game1_NormalBallOddEvenUseCheck.Checked
                        && !Game1_NormalBallUnderOverUseCheck.Checked)
                {
                    logger.Info("[프로그램 정지] 모든 게임 당첨으로 종료");
                    Game1_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    Game1_Start_Button.Text = "배팅이 정지되었습니다.";
                    StartGame_1 = false;

                    Game1_Result_1 = string.Empty;
                    Game1_Result_2 = string.Empty;
                    Game1_Result_3 = string.Empty;
                    Game1_Result_4 = string.Empty;

                    Game1_CruisePowerBallOddEvenLevelChange.Text = "1";
                    Game1_CruisePowerBallUnderOverLevelChange.Text = "1";
                    Game1_CruiseNormalBallOddEvenLevelChange.Text = "1";
                    Game1_CruiseNormalBallUnderOverLevelChange.Text = "1";

                    Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
                    Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
                    Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
                    Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

                    Game1_Betting_Mode_Result_Process = false;
                    Game_1_Betting_Complete_Status = false;
                    Game_1_Result_Load_Complete = false;
                    Game_1_Check_Complete = false;
                    Game1_BetMoney = new int[] { 0, 0, 0, 0 };
                    Game1_BetPick = new string[] { null, null, null, null };

                    Game1_RemainingTimer.Stop();

                    //MessageBox.Show("[프로그램 정지] " + comboBox1.Text + " || " + roundNo_1 + " || 당첨시 사용정지로 인해 프로그램을 정지합니다.", "[파워볼게임] 모든 게임 당첨");
                }

                if (string.IsNullOrEmpty(GameCode_1))
                {
                    BetRemainingTime_1 = 300 - (int)DateTime.Now.TimeOfDay.TotalSeconds % 300;
                    roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_1, BetRemainingTime_1);
                    txtLogAdd(txtLog1, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인파워볼 3분
                        if (GameCode_1.Contains("DSCP3"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(time.TotalSeconds / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_1.Contains("DSCP5"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(time.TotalSeconds / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_1.Contains("EPB3"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(time.TotalSeconds / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_1.Contains("EPB"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(time.TotalSeconds / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_1.Contains("HSP3"))
                        {
                            BetRemainingTime_1 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_1.Contains("HSP5"))
                        {
                            BetRemainingTime_1 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_1.Contains("KLAY2"))
                        {
                            BetRemainingTime_1 = 120 - totalSeconds % 120 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_1.Contains("KLAY5"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_1.Contains("BEXB"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_1 = 10 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인파워볼 3분
                        if (GameCode_1.Contains("DSCP3"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_1.Contains("DSCP5"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_1.Contains("EPB3"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_1.Contains("EPB"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_1.Contains("HSP3"))
                        {
                            BetRemainingTime_1 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_1.Contains("HSP5"))
                        {
                            BetRemainingTime_1 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_1.Contains("KLAY2"))
                        {
                            BetRemainingTime_1 = 120 - totalSeconds % 120 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_1.Contains("KLAY5"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_1.Contains("BEXB"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_1 = 10 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인파워볼 3분
                        if (GameCode_1.Contains("DSCB3_NM_PB"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode_1.Contains("DSCB5_NM_PB"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode_1.Contains("EOSP3_NM_PB"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode_1.Contains("EOSP5_NM_PB"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode_1.Contains("HSPB3_NM_PB"))
                        {
                            BetRemainingTime_1 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode_1.Contains("HSPB5_NM_PB"))
                        {
                            BetRemainingTime_1 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode_1.Contains("KLAY2"))
                        {
                            BetRemainingTime_1 = 120 - totalSeconds % 120 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode_1.Contains("KLAY5"))
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode_1.Contains("BEXB_NM_PB"))
                        {
                            BetRemainingTime_1 = 180 - totalSeconds % 180 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_1 = 300 - totalSeconds % 300 + 1;
                            roundNo_1 = dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime_1, BetRemainingTime_1);

                    if (BetRemainingTime_1 > (betTime(GameCode_1) - 10))
                    {
                        Game1_Betting_Mode_Result_Process = false;
                        Game_1_Result_Load_Complete = false;
                        Game_1_Check_Complete = false;
                        Game_1_Betting_Complete_Status = false;
                        Game1_CruiseBetGroupBox.Text = "[ " + roundNo_1 + "회 ] 크루즈 배팅 ]";
                    }

                    // 게임 1의 결과값을 불러온다.
                    if (!Game_1_Result_Load_Complete && (BetRemainingTime_1 % 3 == 0) && (BetRemainingTime_1 < Game1_Random_Result_Load_Time_For_PowerBall) && (BetRemainingTime_1 > BetEndSec_1))
                    {
                        Game_1_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_1_Betting_Complete_Status && Game_1_Result_Load_Complete && !Game_1_Check_Complete && (BetRemainingTime_1 > BetEndSec_1))
                    {
                        Game_1_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_1_Betting_Complete_Status && Game_1_Result_Load_Complete && (BetRemainingTime_1 % 5 == 0) && (BetRemainingTime_1 < Game1_Random_Bet_Regist_Time_For_PowerBall) && (BetRemainingTime_1 > BetEndSec_1))
                    {
                        Game1_Bet_Processing_AllSum();
                        if (virtualMoney.Checked)
                        {
                            Game1_Bet_Processing_RegistListView();
                            Game_1_Betting_Complete_Status = true;
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game1_BetMoney[0] - Game1_BetMoney[1] - Game1_BetMoney[2] - Game1_BetMoney[3];
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                        else
                        {
                            if (Game_Bet_Processing_Final(Game1_BetPick, Game1_BetMoney, roundNo_1, BetRemainingTime_1, txtLog1, BetEndSec_1, GameCode_1, Random_Nonce_1))
                            {
                                Game1_Bet_Processing_RegistListView();
                                Game_1_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_1_Betting_Complete_Status && BetRemainingTime_1 < BetEndSec_1)
                    {
                        int sum = Game1_BetMoney[0] + Game1_BetMoney[1] + Game1_BetMoney[2] + Game1_BetMoney[3];
                        if (sum > 0)
                        {
                            logger.Info("[프로그램 정지] " + comboBox1.Text + " || " + roundNo_1 + " || " + Game1_BetPick[0] + " || " + Game1_BetMoney[0] + " || " + Game1_BetPick[1] + " || " + Game1_BetMoney[1] + " || " + Game1_BetPick[2] + " || " + Game1_BetMoney[2] + " || " + Game1_BetPick[3] + " || " + Game1_BetMoney[3]);
                            Game1_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            Game1_Start_Button.Text = "배팅이 정지되었습니다.";
                            StartGame_1 = false;

                            Game1_Result_1 = string.Empty;
                            Game1_Result_2 = string.Empty;
                            Game1_Result_3 = string.Empty;
                            Game1_Result_4 = string.Empty;

                            Game1_CruisePowerBallOddEvenLevelChange.Text = "1";
                            Game1_CruisePowerBallUnderOverLevelChange.Text = "1";
                            Game1_CruiseNormalBallOddEvenLevelChange.Text = "1";
                            Game1_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

                            Game1_Betting_Mode_Result_Process = false;

                            Game_1_Betting_Complete_Status = false;

                            Game_1_Result_Load_Complete = false;

                            Game_1_Check_Complete = false;

                            Game1_BetMoney = new int[] { 0, 0, 0, 0 };

                            Game1_BetPick = new string[] { null, null, null, null };
                            Game1_RemainingTimer.Stop();
                            MessageBox.Show("[프로그램 정지] " + comboBox1.Text + " || " + roundNo_1 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[파워볼게임] 배팅실패");
                        }
                        else
                        {
                            Game1_BetMoney = new int[] { 0, 0, 0, 0 };
                            Game1_BetPick = new string[] { null, null, null, null }; ;
                            Game1_Bet_Processing_RegistListView();
                            Game_1_Betting_Complete_Status = true;
                        }
                    }
                }
            }
            /***************************************
            두번째 페이지 게임 진행 : 사다리 게임 종류
            if (StartGame_2)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_2;
                label10.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                button3.Text = "[" + roundNo_2 + "] 회 배팅이 진행 중입니다.";

                if (Game2_WinToStopCheckBox.Checked
                    && !Game2_FirstUseCheck.Checked
                        && !Game2_SecondUseCheck.Checked
                        && !Game2_ThirdUseCheck.Checked)
                {
                    logger.Info("[프로그램 정지] 모든 게임 당첨으로 종료");
                    button3.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    button3.Text = "배팅이 정지되었습니다.";
                    StartGame_2 = false;

                    Game2_Result_1 = string.Empty;
                    Game2_Result_2 = string.Empty;
                    Game2_Result_3 = string.Empty;

                    Game2_FirstLevelChange.Text = "1";
                    Game2_SecondLevelChange.Text = "1";
                    Game2_ThirdLevelChange.Text = "1";

                    Game2_FirstBetSubLevel = 1;
                    Game2_SecondBetSubLevel = 1;
                    Game2_ThirdBetSubLevel = 1;

                    Game2_Betting_Mode_Result_Process = false;
                    Game_2_Betting_Complete_Status = false;
                    Game_2_Result_Load_Complete = false;
                    Game_2_Check_Complete = false;
                    Game2_BetMoney = new int[] { 0, 0, 0, 0 };
                    Game2_BetPick = new string[] { null, null, null, null };
                    //MessageBox.Show("[프로그램 정지] " + comboBox1.Text + " || " + roundNo_1 + " || 당첨시 사용정지로 인해 프로그램을 정지합니다.", "[파워볼게임] 모든 게임 당첨");
                }

                if (string.IsNullOrEmpty(GameCode_2))
                {
                    BetRemainingTime_2 = 300 - totalSeconds % 300;
                    roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_2, BetRemainingTime_2);
                    txtLogAdd(txtLog2, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인사다리 3분
                        if (GameCode_2.Contains("CSA3"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_2.Contains("CSA5"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_2.Equals("HSPSA3"))
                        {
                            BetRemainingTime_2 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_2.Equals("HSPSA5"))
                        {
                            BetRemainingTime_2 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_2.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_2 = 120 - totalSeconds % 120 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_2.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_2.Equals("BEXL"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인사다리 3분
                        if (GameCode_2.Contains("CSA3"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_2.Contains("CSA5"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_2.Equals("HSPSA3"))
                        {
                            BetRemainingTime_2 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_2.Equals("HSPSA5"))
                        {
                            BetRemainingTime_2 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_2.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_2 = 120 - totalSeconds % 120 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_2.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_2.Equals("BEXL"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인사다리 3분
                        if (GameCode_2.Contains("DSCL3_NM"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_2.Contains("DSCL5_NM"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_2.Equals("HSPL3_NM"))
                        {
                            BetRemainingTime_2 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_2.Equals("HSPL5_NM"))
                        {
                            BetRemainingTime_2 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_2.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_2 = 120 - totalSeconds % 120 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_2.Equals("KALYSA5"))
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_2.Equals("BEXL_NM"))
                        {
                            BetRemainingTime_2 = 180 - totalSeconds % 180 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_2 = 300 - totalSeconds % 300 + 1;
                            roundNo_2 = dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime_2, BetRemainingTime_2);
                    if (BetRemainingTime_2 > 0)
                    {
                        if ((BetRemainingTime_2 % 4 == 0) && (BetRemainingTime_2 > (betTime(GameCode_2) - 10)))
                        {
                            Game2_Betting_Mode_Result_Process = false;
                            Game_2_Result_Load_Complete = false;
                            Game_2_Check_Complete = false;
                            Game_2_Betting_Complete_Status = false;
                            Game2_CruiseBetGroupBox.Text = "[ " + roundNo_2 + "회 ] 크루즈 배팅 ]";
                        }
                        // 게임 2의 결과값을 불러온다.
                        if (!Game_2_Result_Load_Complete && (BetRemainingTime_2 % 5 == 0) && (BetRemainingTime_2 < Game2_Random_Result_Load_Time_For_Ladder) && (BetRemainingTime_2 > BetEndSec_2))
                        {
                            Game_2_Result_Load();
                        }

                        // 배팅을 점검하여 패턴과 맞는지 확인한다.
                        if (!Game_2_Betting_Complete_Status && Game_2_Result_Load_Complete && !Game_2_Check_Complete && (BetRemainingTime_2 > BetEndSec_2))
                        {
                            Game_2_PatternCheck();
                        }

                        // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                        if (!Game_2_Betting_Complete_Status && Game_2_Result_Load_Complete && (BetRemainingTime_2 % 5 == 0) && (BetRemainingTime_2 < Game2_Random_Bet_Regist_Time_For_Ladder) && (BetRemainingTime_2 > BetEndSec_2))
                        {
                            Game2_Bet_Processing_AllSum();
                            if (virtualMoney.Checked)
                            {
                                Game2_Bet_Processing_RegistListView();
                                Game_2_Betting_Complete_Status = true;

                                UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game2_BetMoney[0] - Game2_BetMoney[1] - Game2_BetMoney[2] - Game2_BetMoney[3];
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                            }
                            else
                            {
                                if (Game_Bet_Processing_Final(Game2_BetPick, Game2_BetMoney, roundNo_2, BetRemainingTime_2, txtLog2, BetEndSec_2, GameCode_2, Random_Nonce_2))
                                {
                                    Game2_Bet_Processing_RegistListView();
                                    Game_2_Betting_Complete_Status = true;
                                }
                            }
                        }

                        if (!Game_2_Betting_Complete_Status && BetRemainingTime_2 < BetEndSec_2)
                        {
                            int sum = Game2_BetMoney[0] + Game2_BetMoney[1] + Game2_BetMoney[2] + Game2_BetMoney[3];
                            if (sum > 0)
                            {
                                logger.Info("[프로그램 정지] " + comboBox7.Text + " || " + roundNo_2 + " || " + Game2_BetPick[0] + " || " + Game2_BetMoney[0] + " || " + Game2_BetPick[1] + " || " + Game2_BetMoney[1] + " || " + Game2_BetPick[2] + " || " + Game2_BetMoney[2] + " || " + Game2_BetPick[3] + " || " + Game2_BetMoney[3]);
                                button3.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                button3.Text = "배팅이 정지되었습니다.";
                                StartGame_2 = false;

                                Game2_Result_1 = string.Empty;
                                Game2_Result_2 = string.Empty;
                                Game2_Result_3 = string.Empty;

                                Game2_FirstBetSubLevel = 1;
                                Game2_SecondBetSubLevel = 1;
                                Game2_ThirdBetSubLevel = 1;

                                Game2_Betting_Mode_Result_Process = false;

                                Game_2_Betting_Complete_Status = false;

                                Game_2_Result_Load_Complete = false;

                                Game_2_Check_Complete = false;

                                Game2_BetMoney = new int[] { 0, 0, 0, 0 };

                                Game2_BetPick = new string[] { null, null, null, null };

                                Game2_FirstLevelChange.Text = "1";
                                Game2_SecondLevelChange.Text = "1";
                                Game2_ThirdLevelChange.Text = "1";

                                MessageBox.Show("[프로그램 정지] " + comboBox7.Text + " || " + roundNo_2 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[사다리게임] 배팅실패");
                            }
                            else
                            {
                                Game2_BetMoney = new int[] { 0, 0, 0, 0 };
                                Game2_BetPick = new string[] { null, null, null, null }; ;
                                Game2_Bet_Processing_RegistListView();
                                Game_2_Betting_Complete_Status = true;
                            }
                        }
                    }
                }
            }
            4번째 게임 진행 : 사다리 게임 종류
            if (StartGame_4)
            {
                TimeSpan diff = DateTime.Now - StartDateTime_4;
                label14.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                Game4_Start_Button.Text = "[" + roundNo_4 + "] 회 배팅이 진행 중입니다.";

                if (string.IsNullOrEmpty(GameCode_4))
                {
                    BetRemainingTime_4 = 300 - totalSeconds % 300;
                    roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                    setTimeRemaining(BetRemainTime_4, BetRemainingTime_4);
                    txtLogAdd(txtLog4, "게임이 선택되지 않았습니다.", Color.Red);
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인사다리 3분
                        if (GameCode_4.Contains("CSA3"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_4.Contains("CSA5"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_4.Equals("HSPSA3"))
                        {
                            BetRemainingTime_4 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_4.Equals("HSPSA5"))
                        {
                            BetRemainingTime_4 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_4.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_4 = 120 - totalSeconds % 120 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_4.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_4.Equals("BEXL"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인사다리 3분
                        if (GameCode_4.Contains("CSA3"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_4.Contains("CSA5"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_4.Equals("HSPSA3"))
                        {
                            BetRemainingTime_4 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_4.Equals("HSPSA5"))
                        {
                            BetRemainingTime_4 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_4.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_4 = 120 - totalSeconds % 120 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_4.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_4.Equals("BEXL"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인사다리 3분
                        if (GameCode_4.Contains("DSCL3_NM"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인사다리 5분
                        else if (GameCode_4.Contains("DSCL5_NM"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 사다리 3분
                        else if (GameCode_4.Equals("HSPL3_NM"))
                        {
                            BetRemainingTime_4 = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 사다리 5분
                        else if (GameCode_4.Equals("HSPL5_NM"))
                        {
                            BetRemainingTime_4 = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        // 클레이 사다리 2분
                        else if (GameCode_4.Equals("KLAYSA2"))
                        {
                            BetRemainingTime_4 = 120 - totalSeconds % 120 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 사다리 5분
                        else if (GameCode_4.Equals("KALYSA5"))
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 벡스볼 사다리 3분
                        else if (GameCode_4.Equals("BEXL_NM"))
                        {
                            BetRemainingTime_4 = 180 - totalSeconds % 180 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime_4 = 300 - totalSeconds % 300 + 1;
                            roundNo_4 = dayroundNo_4 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }
                    setTimeRemaining(BetRemainTime_4, BetRemainingTime_4);
                    if (BetRemainingTime_4 > 0)
                    {
                        if ((BetRemainingTime_4 % 4 == 0) && (BetRemainingTime_4 > (betTime(GameCode_4) - 10)))
                        {
                            Game4_Betting_Mode_Result_Process = false;
                            Game_4_Result_Load_Complete = false;
                            Game_4_Check_Complete = false;
                            Game_4_Betting_Complete_Status = false;
                        }
                        // 게임 2의 결과값을 불러온다.
                        if (!Game_4_Result_Load_Complete && (BetRemainingTime_4 % 3 == 0) && (BetRemainingTime_4 < Game4_Random_Result_Load_Time_For_Ladder) && (BetRemainingTime_4 > BetEndSec_4))
                        {
                            Game_4_Result_Load();
                        }
                        // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                        if (!Game_4_Betting_Complete_Status && Game_4_Result_Load_Complete && (BetRemainingTime_4 % 5 == 0) && (BetRemainingTime_4 < Game4_Random_Bet_Regist_Time_For_Ladder) && (BetRemainingTime_4 > BetEndSec_4))
                        {
                            if (Game4_BetMoneyLevel > Game4_BetMoneyList.Length)
                            {
                                logger.Info("[프로그램 정지] " + Game4_GameSelectComboBox.Text + " || " + roundNo_4 + " || " + Game4_BetPick[0] + " || " + Game4_BetMoney[0] + " || " + Game4_BetPick[1] + " || " + Game4_BetMoney[1] + " || " + Game4_BetPick[2] + " || " + Game4_BetMoney[2] + " || " + Game4_BetPick[3] + " || " + Game4_BetMoney[3]);
                                Game4_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                Game4_Start_Button.Text = "배팅이 정지되었습니다.";
                                StartGame_4 = false;

                                Game4_Result_1 = string.Empty;
                                Game4_Result_2 = string.Empty;
                                Game4_Result_3 = string.Empty;

                                Game4_Betting_Mode_Result_Process = false;

                                Game_4_Betting_Complete_Status = false;

                                Game_4_Result_Load_Complete = false;

                                Game_4_Check_Complete = false;

                                Game4_BetMoney = new int[] { 0, 0, 0, 0 };

                                Game4_BetPick = new string[] { null, null, null, null };

                                MessageBox.Show("[프로그램 정지] " + Game4_GameSelectComboBox.Text + " || " + roundNo_4 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[사다리게임] 배팅실패");
                            }
                            else
                            {
                                Game4_Bet_Processing_AllSum();
                                if (virtualMoney.Checked)
                                {
                                    Game4_Bet_Processing_RegistListView();
                                    Game_4_Betting_Complete_Status = true;

                                    UtilModel.UserOwnMoney = UtilModel.UserOwnMoney - Game4_BetMoney[0] - Game4_BetMoney[1] - Game4_BetMoney[2] - Game4_BetMoney[3];
                                    lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                                }
                                else
                                {
                                    if (Game_Bet_Processing_Final(Game4_BetPick, Game4_BetMoney, roundNo_4, BetRemainingTime_4, txtLog4, BetEndSec_4, GameCode_4, Random_Nonce_4))
                                    {
                                        Game4_Bet_Processing_RegistListView();
                                        Game_4_Betting_Complete_Status = true;
                                    }
                                }
                            }
                        }

                        if (!Game_4_Betting_Complete_Status && BetRemainingTime_4 < BetEndSec_4)
                        {
                            int sum = Game4_BetMoney[0] + Game4_BetMoney[1] + Game4_BetMoney[2] + Game4_BetMoney[3];
                            if (sum > 0)
                            {
                                logger.Info("[프로그램 정지] " + Game4_GameSelectComboBox.Text + " || " + roundNo_4 + " || " + Game4_BetPick[0] + " || " + Game4_BetMoney[0] + " || " + Game4_BetPick[1] + " || " + Game4_BetMoney[1] + " || " + Game4_BetPick[2] + " || " + Game4_BetMoney[2] + " || " + Game4_BetPick[3] + " || " + Game4_BetMoney[3]);
                                Game4_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                Game4_Start_Button.Text = "배팅이 정지되었습니다.";
                                StartGame_4 = false;

                                Game4_Result_1 = string.Empty;
                                Game4_Result_2 = string.Empty;
                                Game4_Result_3 = string.Empty;

                                Game4_Betting_Mode_Result_Process = false;

                                Game_4_Betting_Complete_Status = false;

                                Game_4_Result_Load_Complete = false;

                                Game_4_Check_Complete = false;

                                Game4_BetMoney = new int[] { 0, 0, 0, 0 };

                                Game4_BetPick = new string[] { null, null, null, null };

                                MessageBox.Show("[프로그램 정지] " + Game4_GameSelectComboBox.Text + " || " + roundNo_4 + " || 배팅에 실패하여 프로그램을 정지합니다.", "[사다리게임] 배팅실패");
                            }
                            else
                            {
                                Game4_BetMoney = new int[] { 0, 0, 0, 0 };
                                Game4_BetPick = new string[] { null, null, null, null }; ;
                                Game4_Bet_Processing_RegistListView();
                                Game_4_Betting_Complete_Status = true;
                            }
                        }
                    }
                }
            }
            *****************************************************************************************************************/
        }
        private Color ReturnLevelForeColor(int level)
        {
            if (level > 40)
            {
                return Color.White;
            }
            else if (level > 30)
            {
                return Color.DarkOrange;
            }
            else if (level > 20)
            {
                return Color.DarkViolet;
            }
            return Color.Black;
        }
        private Color ReturnLevelBackColor(int level)
        {

            if (level > 60)
            {
                return Color.Red;
            }
            else if (level > 50)
            {
                return Color.DarkRed;
            }
            else if (level > 40)
            {
                return Color.Black;
            }
            else if (level > 30)
            {
                return Color.Wheat;
            }
            else if (level > 20)
            {
                return Color.Lavender;
            }
            return System.Drawing.SystemColors.Control;
        }
        private void Game_1_PatternCheck()
        {
            game1_1money.Text = checkGameCheck(Game1_PowerBallOddEvenUseCheck, "Game1_CruisePowerBallOddEvenBetListLevel", "Game1_CruisePowerBallOddEvenBetPickLevel", "Game1_CruisePowerBallOddEvenBetMoneyLevel", Game1_CruiseBetPowerBallOddEvenSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 3);
            game1_2money.Text = checkGameCheck(Game1_PowerBallUnderOverUseCheck, "Game1_CruisePowerBallUnderOverBetListLevel", "Game1_CruisePowerBallUnderOverBetPickLevel", "Game1_CruisePowerBallUnderOverBetMoneyLevel", Game1_CruiseBetPowerBallUnderOverSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 7);
            game1_3money.Text = checkGameCheck(Game1_NormalBallOddEvenUseCheck, "Game1_CruiseNormalBallOddEvenBetListLevel", "Game1_CruiseNormalBallOddEvenBetPickLevel", "Game1_CruiseNormalBallOddEvenBetMoneyLevel", Game1_CruiseBetNormalBallOddEvenSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 11);
            game1_4money.Text = checkGameCheck(Game1_NormalBallUnderOverUseCheck, "Game1_CruiseNormalBallUnderOverBetListLevel", "Game1_CruiseNormalBallUnderOverBetPickLevel", "Game1_CruiseNormalBallUnderOverBetMoneyLevel", Game1_CruiseBetNormalBallUnderOverSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 15);

            game1_1money.ForeColor = ReturnLevelForeColor(int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text));
            game1_2money.ForeColor = ReturnLevelForeColor(int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text));
            game1_3money.ForeColor = ReturnLevelForeColor(int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text));
            game1_4money.ForeColor = ReturnLevelForeColor(int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text));

            game1_1money.BackColor = ReturnLevelBackColor(int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text));
            game1_2money.BackColor = ReturnLevelBackColor(int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text));
            game1_3money.BackColor = ReturnLevelBackColor(int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text));
            game1_4money.BackColor = ReturnLevelBackColor(int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text));

            TimeSpan differentTime;
            for (int i = 0; i < Game1_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game1_CruiseBetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game1_CruiseBetRegistListView.Items.Remove(Game1_CruiseBetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_1_Check_Complete = true;
            //txtLogAdd(txtLog1, "패턴 점검 완료", Color.Red);
        }


        private void Game_2_PatternCheck()
        {
            game2_1money.Text = checkGameCheck(Game2_FirstUseCheck, "Game2_FirstBetListLevel", "Game2_FirstBetPickLevel", "Game2_FirstBetMoneyLevel", Game2_FirstBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 3);
            game2_2money.Text = checkGameCheck(Game2_SecondUseCheck, "Game2_SecondBetListLevel", "Game2_SecondBetPickLevel", "Game2_SecondBetMoneyLevel", Game2_SecondBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 7);
            game2_3money.Text = checkGameCheck(Game2_ThirdUseCheck, "Game2_ThirdBetListLevel", "Game2_ThirdBetPickLevel", "Game2_ThirdBetMoneyLevel", Game2_ThirdBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 11);

            TimeSpan differentTime;
            for (int i = 0; i < Game2_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game2_CruiseBetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game2_CruiseBetRegistListView.Items.Remove(Game2_CruiseBetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_2_Check_Complete = true;
            //txtLogAdd(txtLog1, "패턴 점검 완료", Color.Red);
        }

        private void Game_3_PatternCheck()
        {
            game3_1money.Text = checkGameCheck(Game3_PowerBallOddEvenUseCheck, "Game3_CruisePowerBallOddEvenBetListLevel", "Game3_CruisePowerBallOddEvenBetPickLevel", "Game3_CruisePowerBallOddEvenBetMoneyLevel", Game3_CruiseBetPowerBallOddEvenSubLevel, Game3_CruiseBetRegistListView, GameCode_3, 3);
            game3_2money.Text = checkGameCheck(Game3_PowerBallUnderOverUseCheck, "Game3_CruisePowerBallUnderOverBetListLevel", "Game3_CruisePowerBallUnderOverBetPickLevel", "Game3_CruisePowerBallUnderOverBetMoneyLevel", Game3_CruiseBetPowerBallUnderOverSubLevel, Game3_CruiseBetRegistListView, GameCode_3, 7);
            game3_3money.Text = checkGameCheck(Game3_NormalBallOddEvenUseCheck, "Game3_CruiseNormalBallOddEvenBetListLevel", "Game3_CruiseNormalBallOddEvenBetPickLevel", "Game3_CruiseNormalBallOddEvenBetMoneyLevel", Game3_CruiseBetNormalBallOddEvenSubLevel, Game3_CruiseBetRegistListView, GameCode_3, 11);
            game3_4money.Text = checkGameCheck(Game3_NormalBallUnderOverUseCheck, "Game3_CruiseNormalBallUnderOverBetListLevel", "Game3_CruiseNormalBallUnderOverBetPickLevel", "Game3_CruiseNormalBallUnderOverBetMoneyLevel", Game3_CruiseBetNormalBallUnderOverSubLevel, Game3_CruiseBetRegistListView, GameCode_3, 15);

            game3_1money.ForeColor = ReturnLevelForeColor(int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text));
            game3_2money.ForeColor = ReturnLevelForeColor(int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text));
            game3_3money.ForeColor = ReturnLevelForeColor(int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text));
            game3_4money.ForeColor = ReturnLevelForeColor(int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text));

            game3_1money.BackColor = ReturnLevelBackColor(int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text));
            game3_2money.BackColor = ReturnLevelBackColor(int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text));
            game3_3money.BackColor = ReturnLevelBackColor(int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text));
            game3_4money.BackColor = ReturnLevelBackColor(int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text));
            TimeSpan differentTime;
            for (int i = 0; i < Game3_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game3_CruiseBetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game3_CruiseBetRegistListView.Items.Remove(Game3_CruiseBetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_3_Check_Complete = true;
            //txtLogAdd(txtLog1, "패턴 점검 완료", Color.Red);
        }
        private void Game_5_PatternCheck()
        {
            game5_1money.Text = checkGameCheck(Game5_PowerBallOddEvenUseCheck, "Game5_CruisePowerBallOddEvenBetListLevel", "Game5_CruisePowerBallOddEvenBetPickLevel", "Game5_CruisePowerBallOddEvenBetMoneyLevel", Game5_CruiseBetPowerBallOddEvenSubLevel, Game5_CruiseBetRegistListView, GameCode_5, 3);
            game5_2money.Text = checkGameCheck(Game5_PowerBallUnderOverUseCheck, "Game5_CruisePowerBallUnderOverBetListLevel", "Game5_CruisePowerBallUnderOverBetPickLevel", "Game5_CruisePowerBallUnderOverBetMoneyLevel", Game5_CruiseBetPowerBallUnderOverSubLevel, Game5_CruiseBetRegistListView, GameCode_5, 7);
            game5_3money.Text = checkGameCheck(Game5_NormalBallOddEvenUseCheck, "Game5_CruiseNormalBallOddEvenBetListLevel", "Game5_CruiseNormalBallOddEvenBetPickLevel", "Game5_CruiseNormalBallOddEvenBetMoneyLevel", Game5_CruiseBetNormalBallOddEvenSubLevel, Game5_CruiseBetRegistListView, GameCode_5, 11);
            game5_4money.Text = checkGameCheck(Game5_NormalBallUnderOverUseCheck, "Game5_CruiseNormalBallUnderOverBetListLevel", "Game5_CruiseNormalBallUnderOverBetPickLevel", "Game5_CruiseNormalBallUnderOverBetMoneyLevel", Game5_CruiseBetNormalBallUnderOverSubLevel, Game5_CruiseBetRegistListView, GameCode_5, 15);

            game5_1money.ForeColor = ReturnLevelForeColor(int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text));
            game5_2money.ForeColor = ReturnLevelForeColor(int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text));
            game5_3money.ForeColor = ReturnLevelForeColor(int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text));
            game5_4money.ForeColor = ReturnLevelForeColor(int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text));

            game5_1money.BackColor = ReturnLevelBackColor(int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text));
            game5_2money.BackColor = ReturnLevelBackColor(int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text));
            game5_3money.BackColor = ReturnLevelBackColor(int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text));
            game5_4money.BackColor = ReturnLevelBackColor(int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text));

            TimeSpan differentTime;
            for (int i = 0; i < Game5_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game5_CruiseBetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game5_CruiseBetRegistListView.Items.Remove(Game5_CruiseBetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_5_Check_Complete = true;
        }
        private void Game_6_PatternCheck()
        {
            game6_1money.Text = checkGameCheck(Game6_PowerBallOddEvenUseCheck, "Game6_CruisePowerBallOddEvenBetListLevel", "Game6_CruisePowerBallOddEvenBetPickLevel", "Game6_CruisePowerBallOddEvenBetMoneyLevel", Game6_CruiseBetPowerBallOddEvenSubLevel, Game6_CruiseBetRegistListView, GameCode_6, 3);
            game6_2money.Text = checkGameCheck(Game6_PowerBallUnderOverUseCheck, "Game6_CruisePowerBallUnderOverBetListLevel", "Game6_CruisePowerBallUnderOverBetPickLevel", "Game6_CruisePowerBallUnderOverBetMoneyLevel", Game6_CruiseBetPowerBallUnderOverSubLevel, Game6_CruiseBetRegistListView, GameCode_6, 7);
            game6_3money.Text = checkGameCheck(Game6_NormalBallOddEvenUseCheck, "Game6_CruiseNormalBallOddEvenBetListLevel", "Game6_CruiseNormalBallOddEvenBetPickLevel", "Game6_CruiseNormalBallOddEvenBetMoneyLevel", Game6_CruiseBetNormalBallOddEvenSubLevel, Game6_CruiseBetRegistListView, GameCode_6, 11);
            game6_4money.Text = checkGameCheck(Game6_NormalBallUnderOverUseCheck, "Game6_CruiseNormalBallUnderOverBetListLevel", "Game6_CruiseNormalBallUnderOverBetPickLevel", "Game6_CruiseNormalBallUnderOverBetMoneyLevel", Game6_CruiseBetNormalBallUnderOverSubLevel, Game6_CruiseBetRegistListView, GameCode_6, 15);

            game6_1money.ForeColor = ReturnLevelForeColor(int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text));
            game6_2money.ForeColor = ReturnLevelForeColor(int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text));
            game6_3money.ForeColor = ReturnLevelForeColor(int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text));
            game6_4money.ForeColor = ReturnLevelForeColor(int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text));

            game6_1money.BackColor = ReturnLevelBackColor(int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text));
            game6_2money.BackColor = ReturnLevelBackColor(int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text));
            game6_3money.BackColor = ReturnLevelBackColor(int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text));
            game6_4money.BackColor = ReturnLevelBackColor(int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text));

            TimeSpan differentTime;
            for (int i = 0; i < Game6_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game6_CruiseBetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game6_CruiseBetRegistListView.Items.Remove(Game6_CruiseBetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_6_Check_Complete = true;
        }
        private void Game_1_Result_Load()
        {
            Game1_Result_1 = String.Empty;
            Game1_Result_2 = String.Empty;
            Game1_Result_3 = String.Empty;
            Game1_Result_4 = String.Empty;


            if (UtilModel.ResultServersApiSite)
            {
                if (loadAPISiteResultGame(1, GameCode_1, callResultInning(1, GameCode_1)))
                {
                    Game_1_Result_Processing();
                    Game_1_Result_Load_Complete = true;
                    txtLogAdd(txtLog1, "[성공] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersBepick)
            {
                if (loadBepickResultForPowerBallGame(1, GameCode_1, callResultInning(1, GameCode_1)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_1_Result_Processing();
                    Game_1_Result_Load_Complete = true;
                    txtLogAdd(txtLog1, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersUpdown)
            {
                if (loadUpDownResultForPowerBallGame(1, GameCode_1, callResultInning(1, GameCode_1)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_1_Result_Processing();
                    Game_1_Result_Load_Complete = true;
                    txtLogAdd(txtLog1, "[Updown] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersNtry)
            {

            }

            Game_1_Result_Load_Complete = false;
            txtLogAdd(txtLog1, "[Game1] 결과값을 불러오는데 실패", Color.Red);
        }
        /*****************************
         * 출발점은 일반볼 첫번째 숫자가 홀일 경우 좌출발, 짝일 경우 우출발입니다.
           줄갯수는 일반볼 첫번째 숫자가 1~14일 경우 3줄, 15~28일 경우 4줄입니다.
           좌3짝 / 좌4홀 / 우3홀 / 우4짝
         */

        private void Game_2_Result_Load()
        {
            Game2_Result_1 = String.Empty;
            Game2_Result_2 = String.Empty;
            Game2_Result_3 = String.Empty;

            if (loadBepickResultForLadder(2, GameCode_2, callResultInning(2, GameCode_2)))
            {
                // https://bepick.net/live/result/coinladder3?_=325233
                Game_2_Result_Processing();
                Game_2_Result_Load_Complete = true;
                txtLogAdd(txtLog2, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                return;
            }

            Game_2_Result_Load_Complete = false;
            txtLogAdd(txtLog2, "[Game2] 결과값을 불러오는데 실패", Color.Red);
        }

        private void Game_3_Result_Load()
        {
            Game3_Result_3 = String.Empty;
            Game3_Result_2 = String.Empty;
            Game3_Result_3 = String.Empty;
            Game3_Result_4 = String.Empty;

            if (UtilModel.ResultServersApiSite)
            {
                if (loadAPISiteResultGame(3, GameCode_3, callResultInning(3, GameCode_3)))
                {
                    Game_3_Result_Processing();
                    Game_3_Result_Load_Complete = true;
                    txtLogAdd(txtLog3, "[성공] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersBepick)
            {
                if (loadBepickResultForPowerBallGame(3, GameCode_3, callResultInning(3, GameCode_3)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_3_Result_Processing();
                    Game_3_Result_Load_Complete = true;
                    txtLogAdd(txtLog3, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersUpdown)
            {
                if (loadUpDownResultForPowerBallGame(3, GameCode_3, callResultInning(3, GameCode_3)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_3_Result_Processing();
                    Game_3_Result_Load_Complete = true;
                    txtLogAdd(txtLog3, "[Updown] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersNtry)
            {

            }
            Game_3_Result_Load_Complete = false;
            txtLogAdd(txtLog3, "[Game3] 결과값을 불러오는데 실패", Color.Red);
        }
        private void Game_4_Result_Load()
        {
            Game4_Result_1 = String.Empty;
            Game4_Result_2 = String.Empty;
            Game4_Result_3 = String.Empty;

            if (loadBepickResultForLadder(4, GameCode_4, callResultInning(4, GameCode_4)))
            {
                // https://bepick.net/live/result/coinladder3?_=325233
                Game_4_Result_Processing();
                Game_4_Result_Load_Complete = true;
                txtLogAdd(txtLog4, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                return;
            }
            Game_4_Result_Load_Complete = false;
            txtLogAdd(txtLog4, "[Game4] 결과값을 불러오는데 실패", Color.Red);
        }


        private void Game_5_Result_Load()
        {
            Game5_Result_1 = String.Empty;
            Game5_Result_2 = String.Empty;
            Game5_Result_3 = String.Empty;
            Game5_Result_4 = String.Empty;

            if (UtilModel.ResultServersNtry)
            {
                if (GameCode_5.Contains("EPB"))
                {
                    if (loadNtryEOSResultForPowerBallGame(5, GameCode_5, callResultInning(5, GameCode_5)))
                    {
                        Game_5_Result_Processing();
                        Game_5_Result_Load_Complete = true;
                        txtLogAdd(txtLog5, "[성공] 결과값을 불러오는데 성공", Color.Red);
                        return;
                    }
                }
            }
            if (UtilModel.ResultServersApiSite)
            {
                if (loadAPISiteResultGame(5, GameCode_5, callResultInning(5, GameCode_5)))
                {
                    Game_5_Result_Processing();
                    Game_5_Result_Load_Complete = true;
                    txtLogAdd(txtLog5, "[성공] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersBepick)
            {
                if (loadBepickResultForPowerBallGame(5, GameCode_5, callResultInning(5, GameCode_5)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_5_Result_Processing();
                    Game_5_Result_Load_Complete = true;
                    txtLogAdd(txtLog5, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersUpdown)
            {
                if (loadUpDownResultForPowerBallGame(5, GameCode_5, callResultInning(5, GameCode_5)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_5_Result_Processing();
                    Game_5_Result_Load_Complete = true;
                    txtLogAdd(txtLog5, "[Updown] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }

            Game_5_Result_Load_Complete = false;
            txtLogAdd(txtLog5, "[Game5] 결과값을 불러오는데 실패", Color.Red);
        }

        private void Game_6_Result_Load()
        {
            Game6_Result_1 = String.Empty;
            Game6_Result_2 = String.Empty;
            Game6_Result_3 = String.Empty;
            Game6_Result_4 = String.Empty;

            if (UtilModel.ResultServersNtry)
            {
                if (GameCode_6.Contains("EPB"))
                {
                    if (loadNtryEOSResultForPowerBallGame(6, GameCode_6, callResultInning(6, GameCode_6)))
                    {
                        Game_6_Result_Processing();
                        Game_6_Result_Load_Complete = true;
                        txtLogAdd(txtLog6, "[성공] 결과값을 불러오는데 성공", Color.Red);
                        return;
                    }
                }
            }
            if (UtilModel.ResultServersApiSite)
            {
                if (loadAPISiteResultGame(6, GameCode_6, callResultInning(6, GameCode_6)))
                {
                    Game_6_Result_Processing();
                    Game_6_Result_Load_Complete = true;
                    txtLogAdd(txtLog6, "[성공] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersBepick)
            {
                if (loadBepickResultForPowerBallGame(6, GameCode_6, callResultInning(6, GameCode_6)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_6_Result_Processing();
                    Game_6_Result_Load_Complete = true;
                    txtLogAdd(txtLog6, "[BePick] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersUpdown)
            {
                if (loadUpDownResultForPowerBallGame(6, GameCode_6, callResultInning(6, GameCode_6)))
                {
                    // https://bepick.net/live/result/coinladder3?_=325233
                    Game_6_Result_Processing();
                    Game_6_Result_Load_Complete = true;
                    txtLogAdd(txtLog6, "[Updown] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            Game_6_Result_Load_Complete = false;
            txtLogAdd(txtLog6, "[Game6] 결과값을 불러오는데 실패", Color.Red);
        }

        private void Game_1_Result_Processing()
        {
            if (!Game1_Betting_Mode_Result_Process)
            {
                Game1_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game1_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game1_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(1, GameCode_1).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string Pick_4 = item.SubItems[14].Text;
                    item.SubItems[3].Text = Game1_Result_1;
                    item.SubItems[7].Text = Game1_Result_2;
                    item.SubItems[11].Text = Game1_Result_3;
                    item.SubItems[15].Text = Game1_Result_4;

                    int.TryParse(Regex.Replace(item.SubItems[4].Text, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(item.SubItems[8].Text, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(item.SubItems[12].Text, @"\D", ""), out int Out_BetMoney_3);
                    int.TryParse(Regex.Replace(item.SubItems[16].Text, @"\D", ""), out int Out_BetMoney_4);

                    //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);

                    /*********** 파워볼 홀짝 당첨 여부 **************/
                    if (Game1_Result_1.Equals(Pick_1))
                    {
                        if (Game1_Result_1.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (Game1_Result_1.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game1_WinMoney.Text, @"\D", ""), out outValue1);
                        Game1_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[2].Text.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        else if (item.SubItems[2].Text.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        if (item.SubItems[3].Text.Equals("홀"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (item.SubItems[3].Text.Equals("짝"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 파워볼 언오버 당첨 여부 **************/
                    if (Game1_Result_2.Equals(Pick_2))
                    {
                        if (Game1_Result_2.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (Game1_Result_2.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game1_WinMoney.Text, @"\D", ""), out outValue2);
                        Game1_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[6].Text.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        else if (item.SubItems[6].Text.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        if (item.SubItems[7].Text.Equals("언"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (item.SubItems[7].Text.Equals("오"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 홀짝 당첨 여부 **************/
                    if (Game1_Result_3.Equals(Pick_3))
                    {
                        if (Game1_Result_3.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (Game1_Result_3.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game1_WinMoney.Text, @"\D", ""), out outValue3);
                        Game1_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[10].Text.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        else if (item.SubItems[10].Text.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        if (item.SubItems[11].Text.Equals("홀"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (item.SubItems[11].Text.Equals("짝"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 언오버 당첨 여부 **************/
                    if (Game1_Result_4.Equals(Pick_4))
                    {
                        if (Game1_Result_4.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (Game1_Result_4.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_4 * 1.95);

                        int outValue4;
                        int.TryParse(Regex.Replace(Game1_WinMoney.Text, @"\D", ""), out outValue4);
                        Game1_WinMoney.Text = UtilModel.StringFormatChanged(outValue4 + winMoney);

                        item.SubItems[17].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[14].Text.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        else if (item.SubItems[14].Text.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        if (item.SubItems[15].Text.Equals("언"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (item.SubItems[15].Text.Equals("오"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        item.SubItems[17].Text = "0";
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game1_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            if (GameProfitMoneySet(Game1_WinMoney, Game1_StackMoney, Game1_ProfitMoney))
            {
                Game1Stop(Game1_Start_Button);
            }
        }

        private Boolean GameProfitMoneySet(Label Game_Winmoney, Label Game_StackMoney, Label Game_ProfitMoney)
        {
            int.TryParse(Regex.Replace(Game_Winmoney.Text, @"\D", ""), out int Game_WinMoney_Out_Value);
            int.TryParse(Regex.Replace(Game_StackMoney.Text, @"\D", ""), out int Game_StackMoney_Out_Value);
            Game_ProfitMoney.Text = UtilModel.StringFormatChanged(Game_WinMoney_Out_Value - Game_StackMoney_Out_Value);

            if (Game_WinMoney_Out_Value - Game_StackMoney_Out_Value < 0)
            {
                Game_ProfitMoney.ForeColor = Color.Red;
            }
            else if (Game_WinMoney_Out_Value - Game_StackMoney_Out_Value > 0)
            {
                Game_ProfitMoney.ForeColor = Color.Blue;

                int.TryParse(Regex.Replace(profitLimitComboBox.Text, @"\D", ""), out int profitLimint);
                if (profitLimint > 0 && Game_WinMoney_Out_Value - Game_StackMoney_Out_Value > profitLimint)
                {
                    return true;
                }
            }
            return false;
        }

        private void Game_2_Result_Processing()
        {

            if (!Game2_Betting_Mode_Result_Process)
            {
                Game2_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game2_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game2_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(2, GameCode_2).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string BetMoney_1 = item.SubItems[4].Text;
                    string BetMoney_2 = item.SubItems[8].Text;
                    string BetMoney_3 = item.SubItems[12].Text;

                    item.SubItems[3].Text = Game2_Result_1;
                    item.SubItems[7].Text = Game2_Result_2;
                    item.SubItems[11].Text = Game2_Result_3;

                    int.TryParse(Regex.Replace(BetMoney_1, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(BetMoney_2, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(BetMoney_3, @"\D", ""), out int Out_BetMoney_3);

                    //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);


                    if (item.SubItems[2].Text.Equals("좌"))
                    {
                        item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[2].ForeColor = Color.White;
                    }
                    else if (item.SubItems[2].Text.Equals("우"))
                    {
                        item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[2].ForeColor = Color.White;
                    }
                    if (item.SubItems[3].Text.Equals("좌"))
                    {
                        item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[3].ForeColor = Color.White;
                    }
                    else if (item.SubItems[3].Text.Equals("우"))
                    {
                        item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[3].ForeColor = Color.White;
                    }


                    if (item.SubItems[6].Text.Equals("삼"))
                    {
                        item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[6].ForeColor = Color.White;
                    }
                    else if (item.SubItems[6].Text.Equals("사"))
                    {
                        item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[6].ForeColor = Color.White;
                    }
                    if (item.SubItems[7].Text.Equals("삼"))
                    {
                        item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[7].ForeColor = Color.White;
                    }
                    else if (item.SubItems[7].Text.Equals("사"))
                    {
                        item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[7].ForeColor = Color.White;
                    }


                    if (item.SubItems[10].Text.Equals("홀"))
                    {
                        item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[10].ForeColor = Color.White;
                    }
                    else if (item.SubItems[10].Text.Equals("짝"))
                    {
                        item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[10].ForeColor = Color.White;
                    }
                    if (item.SubItems[11].Text.Equals("홀"))
                    {
                        item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[11].ForeColor = Color.White;
                    }
                    else if (item.SubItems[11].Text.Equals("짝"))
                    {
                        item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[11].ForeColor = Color.White;
                    }

                    /*********** 사다리 좌우 **************/
                    if (Game2_Result_1.Equals(Pick_1))
                    {
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game2_WinMoney.Text, @"\D", ""), out outValue1);
                        Game2_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }

                    /*********** 사다리 삼사줄 **************/
                    if (Game2_Result_2.Equals(Pick_2))
                    {
                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game2_WinMoney.Text, @"\D", ""), out outValue2);
                        Game2_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 사다리 홀짝 당첨 여부 **************/
                    if (Game2_Result_3.Equals(Pick_3))
                    {
                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game2_WinMoney.Text, @"\D", ""), out outValue3);
                        Game2_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game2_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            if (GameProfitMoneySet(Game2_WinMoney, Game2_StackMoney, Game2_ProfitMoney))
            {

            }
        }

        private void Game_3_Result_Processing()
        {
            if (!Game3_Betting_Mode_Result_Process)
            {
                Game3_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game3_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game3_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(3, GameCode_3).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string Pick_4 = item.SubItems[14].Text;
                    string BetMoney_1 = item.SubItems[4].Text;
                    string BetMoney_2 = item.SubItems[8].Text;
                    string BetMoney_3 = item.SubItems[12].Text;
                    string BetMoney_4 = item.SubItems[16].Text;

                    item.SubItems[3].Text = Game3_Result_1;
                    item.SubItems[7].Text = Game3_Result_2;
                    item.SubItems[11].Text = Game3_Result_3;
                    item.SubItems[15].Text = Game3_Result_4;

                    int.TryParse(Regex.Replace(BetMoney_1, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(BetMoney_2, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(BetMoney_3, @"\D", ""), out int Out_BetMoney_3);
                    int.TryParse(Regex.Replace(BetMoney_4, @"\D", ""), out int Out_BetMoney_4);
                    /*********** 파워볼 홀짝 당첨 여부 **************/
                    if (Game3_Result_1.Equals(Pick_1))
                    {
                        if (Game3_Result_1.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));

                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (Game3_Result_1.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));

                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game3_WinMoney.Text, @"\D", ""), out outValue1);
                        Game3_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[2].Text.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        else if (item.SubItems[2].Text.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        if (item.SubItems[3].Text.Equals("홀"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (item.SubItems[3].Text.Equals("짝"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 파워볼 언오버 당첨 여부 **************/
                    if (Game3_Result_2.Equals(Pick_2))
                    {
                        if (Game3_Result_2.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (Game3_Result_2.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game3_WinMoney.Text, @"\D", ""), out outValue2);
                        Game3_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[6].Text.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        else if (item.SubItems[6].Text.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        if (item.SubItems[7].Text.Equals("언"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (item.SubItems[7].Text.Equals("오"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 홀짝 당첨 여부 **************/
                    if (Game3_Result_3.Equals(Pick_3))
                    {
                        if (Game3_Result_3.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (Game3_Result_3.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game3_WinMoney.Text, @"\D", ""), out outValue3);
                        Game3_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[10].Text.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        else if (item.SubItems[10].Text.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        if (item.SubItems[11].Text.Equals("홀"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (item.SubItems[11].Text.Equals("짝"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 언오버 당첨 여부 **************/
                    if (Game3_Result_4.Equals(Pick_4))
                    {
                        if (Game3_Result_4.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (Game3_Result_4.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_4 * 1.95);

                        int outValue4;
                        int.TryParse(Regex.Replace(Game3_WinMoney.Text, @"\D", ""), out outValue4);
                        Game3_WinMoney.Text = UtilModel.StringFormatChanged(outValue4 + winMoney);

                        item.SubItems[17].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[14].Text.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        else if (item.SubItems[14].Text.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        if (item.SubItems[15].Text.Equals("언"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (item.SubItems[15].Text.Equals("오"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        item.SubItems[17].Text = "0";
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game3_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            if (GameProfitMoneySet(Game3_WinMoney, Game3_StackMoney, Game3_ProfitMoney))
            {
                Game3Stop(Game3_Start_Button);
            }
        }
        private void Game_4_Result_Processing()
        {
            if (!Game4_Betting_Mode_Result_Process)
            {
                Game4_Betting_Mode_Result_Processing();
                button5.Text = (Game4_BetMoneyLevel + 1).ToString();
            }
            for (int i = 0; i < Game4_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game4_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(4, GameCode_4).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string BetMoney_1 = item.SubItems[4].Text;
                    string BetMoney_2 = item.SubItems[8].Text;
                    string BetMoney_3 = item.SubItems[12].Text;

                    item.SubItems[3].Text = Game4_Result_1;
                    item.SubItems[7].Text = Game4_Result_2;
                    item.SubItems[11].Text = Game4_Result_3;

                    int.TryParse(Regex.Replace(BetMoney_1, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(BetMoney_2, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(BetMoney_3, @"\D", ""), out int Out_BetMoney_3);

                    //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);


                    if (item.SubItems[2].Text.Equals("좌"))
                    {
                        item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[2].Text.Equals("우"))
                    {
                        item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }
                    if (item.SubItems[3].Text.Equals("좌"))
                    {
                        item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[3].Text.Equals("우"))
                    {
                        item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }


                    if (item.SubItems[6].Text.Equals("삼"))
                    {
                        item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[6].Text.Equals("사"))
                    {
                        item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }
                    if (item.SubItems[7].Text.Equals("삼"))
                    {
                        item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[7].Text.Equals("사"))
                    {
                        item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }


                    if (item.SubItems[10].Text.Equals("홀"))
                    {
                        item.SubItems[10].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[10].Text.Equals("짝"))
                    {
                        item.SubItems[10].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }
                    if (item.SubItems[11].Text.Equals("홀"))
                    {
                        item.SubItems[11].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                    }
                    else if (item.SubItems[11].Text.Equals("짝"))
                    {
                        item.SubItems[11].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                    }

                    /*********** 사다리 좌우 **************/
                    if (Game4_Result_1.Equals(Pick_1))
                    {
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game4_WinMoney.Text, @"\D", ""), out outValue1);
                        Game4_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }

                    /*********** 사다리 삼사줄 **************/
                    if (Game4_Result_2.Equals(Pick_2))
                    {
                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game4_WinMoney.Text, @"\D", ""), out outValue2);
                        Game4_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 사다리 홀짝 당첨 여부 **************/
                    if (Game4_Result_3.Equals(Pick_3))
                    {
                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game4_WinMoney.Text, @"\D", ""), out outValue3);
                        Game4_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game4_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            GameProfitMoneySet(Game4_WinMoney, Game4_StackMoney, Game4_ProfitMoney);
        }

        private void Game_5_Result_Processing()
        {
            if (!Game5_Betting_Mode_Result_Process)
            {
                Game5_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game5_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game5_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(5, GameCode_5).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string Pick_4 = item.SubItems[14].Text;
                    item.SubItems[3].Text = Game5_Result_1;
                    item.SubItems[7].Text = Game5_Result_2;
                    item.SubItems[11].Text = Game5_Result_3;
                    item.SubItems[15].Text = Game5_Result_4;

                    int.TryParse(Regex.Replace(item.SubItems[4].Text, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(item.SubItems[8].Text, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(item.SubItems[12].Text, @"\D", ""), out int Out_BetMoney_3);
                    int.TryParse(Regex.Replace(item.SubItems[16].Text, @"\D", ""), out int Out_BetMoney_4);

                    //Game5_CruiseBetRegistListView.EnsureVisible(Game5_CruiseBetRegistListView.Items.Count - 1);

                    /*********** 파워볼 홀짝 당첨 여부 **************/
                    if (Game5_Result_1.Equals(Pick_1))
                    {
                        if (Game5_Result_1.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (Game5_Result_1.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game5_WinMoney.Text, @"\D", ""), out outValue1);
                        Game5_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[2].Text.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        else if (item.SubItems[2].Text.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        if (item.SubItems[3].Text.Equals("홀"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (item.SubItems[3].Text.Equals("짝"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 파워볼 언오버 당첨 여부 **************/
                    if (Game5_Result_2.Equals(Pick_2))
                    {
                        if (Game5_Result_2.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (Game5_Result_2.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game5_WinMoney.Text, @"\D", ""), out outValue2);
                        Game5_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[6].Text.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        else if (item.SubItems[6].Text.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        if (item.SubItems[7].Text.Equals("언"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (item.SubItems[7].Text.Equals("오"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 홀짝 당첨 여부 **************/
                    if (Game5_Result_3.Equals(Pick_3))
                    {
                        if (Game5_Result_3.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (Game5_Result_3.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game5_WinMoney.Text, @"\D", ""), out outValue3);
                        Game5_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[10].Text.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        else if (item.SubItems[10].Text.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        if (item.SubItems[11].Text.Equals("홀"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (item.SubItems[11].Text.Equals("짝"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 언오버 당첨 여부 **************/
                    if (Game5_Result_4.Equals(Pick_4))
                    {
                        if (Game5_Result_4.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (Game5_Result_4.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_4 * 1.95);

                        int outValue4;
                        int.TryParse(Regex.Replace(Game5_WinMoney.Text, @"\D", ""), out outValue4);
                        Game5_WinMoney.Text = UtilModel.StringFormatChanged(outValue4 + winMoney);

                        item.SubItems[17].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[14].Text.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        else if (item.SubItems[14].Text.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        if (item.SubItems[15].Text.Equals("언"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (item.SubItems[15].Text.Equals("오"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        item.SubItems[17].Text = "0";
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game5_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            if (GameProfitMoneySet(Game5_WinMoney, Game5_StackMoney, Game5_ProfitMoney))
            {
                Game5Stop(Game5_Start_Button);
            }
        }


        private void Game_6_Result_Processing()
        {
            if (!Game6_Betting_Mode_Result_Process)
            {
                Game6_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game6_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game6_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(6, GameCode_6).ToString());

                if (isContains)
                {
                    string Pick_1 = item.SubItems[2].Text;
                    string Pick_2 = item.SubItems[6].Text;
                    string Pick_3 = item.SubItems[10].Text;
                    string Pick_4 = item.SubItems[14].Text;
                    item.SubItems[3].Text = Game6_Result_1;
                    item.SubItems[7].Text = Game6_Result_2;
                    item.SubItems[11].Text = Game6_Result_3;
                    item.SubItems[15].Text = Game6_Result_4;

                    int.TryParse(Regex.Replace(item.SubItems[4].Text, @"\D", ""), out int Out_BetMoney_1);
                    int.TryParse(Regex.Replace(item.SubItems[8].Text, @"\D", ""), out int Out_BetMoney_2);
                    int.TryParse(Regex.Replace(item.SubItems[12].Text, @"\D", ""), out int Out_BetMoney_3);
                    int.TryParse(Regex.Replace(item.SubItems[16].Text, @"\D", ""), out int Out_BetMoney_4);

                    //Game6_CruiseBetRegistListView.EnsureVisible(Game6_CruiseBetRegistListView.Items.Count - 1);

                    /*********** 파워볼 홀짝 당첨 여부 **************/
                    if (Game6_Result_1.Equals(Pick_1))
                    {
                        if (Game6_Result_1.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (Game6_Result_1.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        int winMoney = (int)(Out_BetMoney_1 * 1.95);

                        int outValue1;
                        int.TryParse(Regex.Replace(Game6_WinMoney.Text, @"\D", ""), out outValue1);
                        Game6_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[2].Text.Equals("홀"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        else if (item.SubItems[2].Text.Equals("짝"))
                        {
                            item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[2].ForeColor = Color.White;
                        }
                        if (item.SubItems[3].Text.Equals("홀"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        else if (item.SubItems[3].Text.Equals("짝"))
                        {
                            item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[3].ForeColor = Color.White;
                        }
                        item.SubItems[5].Text = "0";
                        item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 파워볼 언오버 당첨 여부 **************/
                    if (Game6_Result_2.Equals(Pick_2))
                    {
                        if (Game6_Result_2.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (Game6_Result_2.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_2 * 1.95);

                        int outValue2;
                        int.TryParse(Regex.Replace(Game6_WinMoney.Text, @"\D", ""), out outValue2);
                        Game6_WinMoney.Text = UtilModel.StringFormatChanged(outValue2 + winMoney);

                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[6].Text.Equals("언"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        else if (item.SubItems[6].Text.Equals("오"))
                        {
                            item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[6].ForeColor = Color.White;
                        }
                        if (item.SubItems[7].Text.Equals("언"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        else if (item.SubItems[7].Text.Equals("오"))
                        {
                            item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[7].ForeColor = Color.White;
                        }
                        item.SubItems[9].Text = "0";
                        item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 홀짝 당첨 여부 **************/
                    if (Game6_Result_3.Equals(Pick_3))
                    {
                        if (Game6_Result_3.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (Game6_Result_3.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_3 * 1.95);

                        int outValue3;
                        int.TryParse(Regex.Replace(Game6_WinMoney.Text, @"\D", ""), out outValue3);
                        Game6_WinMoney.Text = UtilModel.StringFormatChanged(outValue3 + winMoney);

                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[10].Text.Equals("홀"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        else if (item.SubItems[10].Text.Equals("짝"))
                        {
                            item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[10].ForeColor = Color.White;
                        }
                        if (item.SubItems[11].Text.Equals("홀"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        else if (item.SubItems[11].Text.Equals("짝"))
                        {
                            item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[11].ForeColor = Color.White;
                        }
                        item.SubItems[13].Text = "0";
                        item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                    /*********** 일반볼 언오버 당첨 여부 **************/
                    if (Game6_Result_4.Equals(Pick_4))
                    {
                        if (Game6_Result_4.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (Game6_Result_4.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }

                        int winMoney = (int)(Out_BetMoney_4 * 1.95);

                        int outValue4;
                        int.TryParse(Regex.Replace(Game6_WinMoney.Text, @"\D", ""), out outValue4);
                        Game6_WinMoney.Text = UtilModel.StringFormatChanged(outValue4 + winMoney);

                        item.SubItems[17].Text = UtilModel.StringFormatChanged(winMoney);
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));

                        if (virtualMoney.Checked)
                        {
                            UtilModel.UserOwnMoney = UtilModel.UserOwnMoney + winMoney;
                            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
                        }
                    }
                    else
                    {
                        if (item.SubItems[14].Text.Equals("언"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        else if (item.SubItems[14].Text.Equals("오"))
                        {
                            item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[14].ForeColor = Color.White;
                        }
                        if (item.SubItems[15].Text.Equals("언"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        else if (item.SubItems[15].Text.Equals("오"))
                        {
                            item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            item.SubItems[15].ForeColor = Color.White;
                        }
                        item.SubItems[17].Text = "0";
                        item.SubItems[17].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                    }
                }
                else
                {
                    break;
                }
            }

            SetRowBackgroundColor(Game6_CruiseBetRegistListView, Color.LightGray, Color.OrangeRed);
            if (GameProfitMoneySet(Game6_WinMoney, Game6_StackMoney, Game6_ProfitMoney))
            {
                Game6Stop(Game6_Start_Button);
            }
        }
        private void Game1_Betting_Mode_Result_Processing()
        {
            // All_Win_Bet_Money
            // EosCruisePowerBallOddEvenBetMoneyLevel1
            Game1_Betting_Mode_Result_Process = true;

            if (Game1_PowerBallOddEvenUseCheck.Checked)
            {
                TextBox Game1_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game1_CruisePowerBallOddEvenBetPickLevel" + Game1_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game1_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game1_Result_1.Contains(Game1_CruisePowerBallOddEvenBetPickLevel.Text))
                    {
                        if (Game1_CruiseBetPowerBallOddEvenSubLevel == 1)
                        {
                            Game1_CruiseBetPowerBallOddEvenSubLevel = 2;
                        }
                        else if (Game1_CruiseBetPowerBallOddEvenSubLevel == 2)
                        {
                            Game1_CruiseBetPowerBallOddEvenSubLevel = 3;
                        }
                        else if (Game1_CruiseBetPowerBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_1 - 1).ToString());
                                item.SubItems.Add(Game1_PowerBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game1_CruisePowerBallOddEvenLevelChange.Text);
                                Game1_Clear_Level_50.Items.Add(item);
                            }
                            Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game1_CruisePowerBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game1_PowerBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
                        Game1_CruisePowerBallOddEvenLevelChange.Text = (int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game1_CruisePowerBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game1_CruisePowerBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }

            if (Game1_PowerBallUnderOverUseCheck.Checked)
            {
                TextBox Game1_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game1_CruisePowerBallUnderOverBetPickLevel" + Game1_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game1_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game1_Result_2.Contains(Game1_CruisePowerBallUnderOverBetPickLevel.Text))
                    {
                        if (Game1_CruiseBetPowerBallUnderOverSubLevel == 1)
                        {
                            Game1_CruiseBetPowerBallUnderOverSubLevel = 2;
                        }
                        else if (Game1_CruiseBetPowerBallUnderOverSubLevel == 2)
                        {
                            Game1_CruiseBetPowerBallUnderOverSubLevel = 3;
                        }
                        else if (Game1_CruiseBetPowerBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_1 - 1).ToString());
                                item.SubItems.Add(Game1_PowerBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game1_CruisePowerBallUnderOverLevelChange.Text);
                                Game1_Clear_Level_50.Items.Add(item);
                            }

                            Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game1_CruisePowerBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game1_PowerBallUnderOverUseCheck.Checked = false;
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox18.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
                        Game1_CruisePowerBallUnderOverLevelChange.Text = (int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game1_CruisePowerBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game1_CruisePowerBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game1_NormalBallOddEvenUseCheck.Checked)
            {
                TextBox Game1_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game1_CruiseNormalBallOddEvenBetPickLevel" + Game1_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game1_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game1_Result_3.Contains(Game1_CruiseNormalBallOddEvenBetPickLevel.Text))
                    {
                        if (Game1_CruiseBetNormalBallOddEvenSubLevel == 1)
                        {
                            Game1_CruiseBetNormalBallOddEvenSubLevel = 2;
                        }
                        else if (Game1_CruiseBetNormalBallOddEvenSubLevel == 2)
                        {
                            Game1_CruiseBetNormalBallOddEvenSubLevel = 3;
                        }
                        else if (Game1_CruiseBetNormalBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_1 - 1).ToString());
                                item.SubItems.Add(Game1_NormalBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game1_CruiseNormalBallOddEvenLevelChange.Text);
                                Game1_Clear_Level_50.Items.Add(item);
                            }

                            Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game1_CruiseNormalBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game1_NormalBallOddEvenUseCheck.Checked = false;
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox17.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
                        Game1_CruiseNormalBallOddEvenLevelChange.Text = (int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game1_CruiseNormalBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game1_CruiseNormalBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game1_NormalBallUnderOverUseCheck.Checked)
            {
                TextBox Game1_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game1_CruiseNormalBallUnderOverBetPickLevel" + Game1_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game1_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game1_Result_4.Contains(Game1_CruiseNormalBallUnderOverBetPickLevel.Text))
                    {
                        if (Game1_CruiseBetNormalBallUnderOverSubLevel == 1)
                        {
                            Game1_CruiseBetNormalBallUnderOverSubLevel = 2;
                        }
                        else if (Game1_CruiseBetNormalBallUnderOverSubLevel == 2)
                        {
                            Game1_CruiseBetNormalBallUnderOverSubLevel = 3;
                        }
                        else if (Game1_CruiseBetNormalBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_1 - 1).ToString());
                                item.SubItems.Add(Game1_NormalBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game1_CruiseNormalBallUnderOverLevelChange.Text);
                                Game1_Clear_Level_50.Items.Add(item);
                            }

                            Game1_CruiseBetNormalBallUnderOverSubLevel = 1;
                            Game1_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game1_NormalBallUnderOverUseCheck.Checked = false;
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox15.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game1_CruiseBetNormalBallUnderOverSubLevel = 1;
                        Game1_CruiseNormalBallUnderOverLevelChange.Text = (int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game1_CruiseNormalBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game1_CruiseNormalBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }
        }

        private void Game2_Betting_Mode_Result_Processing()
        {
            Game2_Betting_Mode_Result_Process = true;

            if (Game2_FirstUseCheck.Checked)
            {
                TextBox Game2_FirstBetPickLevel = Controls.Find("Game2_FirstBetPickLevel" + Game2_FirstBetSubLevel.ToString(), true)[0] as TextBox;
                if (!Game2_FirstBetPickLevel.Text.Contains("통"))
                {
                    if (Game2_Result_1.Contains(Game2_FirstBetPickLevel.Text))
                    {
                        if (Game2_FirstBetSubLevel == 1)
                        {
                            Game2_FirstBetSubLevel = 2;
                        }
                        else if (Game2_FirstBetSubLevel == 2)
                        {
                            Game2_FirstBetSubLevel = 3;
                        }
                        else if (Game2_FirstBetSubLevel == 3)
                        {
                            if (int.Parse(Game2_FirstLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_2 - 1).ToString());
                                item.SubItems.Add(Game2_FirstUseCheck.Text);
                                item.SubItems.Add(Game2_FirstLevelChange.Text);
                                Game2_Clear_Level_50.Items.Add(item);
                            }

                            Game2_FirstBetSubLevel = 1;
                            Game2_FirstLevelChange.Text = "1";

                            if (Game2_WinToStopCheckBox.Checked)
                            {
                                Game2_FirstUseCheck.Checked = false;
                                txtLogAdd(txtLog2, "클리어 후 정지 기능으로 [" + groupBox8.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game2_FirstBetSubLevel = 1;
                        Game2_FirstLevelChange.Text = (int.Parse(Game2_FirstLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game2_FirstBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game2_FirstBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game2_SecondUseCheck.Checked)
            {
                TextBox Game2_SecondBetPickLevel = Controls.Find("Game2_SecondBetPickLevel" + Game2_SecondBetSubLevel.ToString(), true)[0] as TextBox;
                if (!Game2_SecondBetPickLevel.Text.Contains("통"))
                {
                    if (Game2_Result_2.Contains(Game2_SecondBetPickLevel.Text))
                    {
                        if (Game2_SecondBetSubLevel == 1)
                        {
                            Game2_SecondBetSubLevel = 2;
                        }
                        else if (Game2_SecondBetSubLevel == 2)
                        {
                            Game2_SecondBetSubLevel = 3;
                        }
                        else if (Game2_SecondBetSubLevel == 3)
                        {
                            if (int.Parse(Game2_SecondLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_2 - 1).ToString());
                                item.SubItems.Add(Game2_SecondUseCheck.Text);
                                item.SubItems.Add(Game2_SecondLevelChange.Text);
                                Game2_Clear_Level_50.Items.Add(item);
                            }

                            Game2_SecondBetSubLevel = 1;
                            Game2_SecondLevelChange.Text = "1";
                            if (Game2_WinToStopCheckBox.Checked)
                            {
                                Game2_SecondUseCheck.Checked = false;
                                txtLogAdd(txtLog2, "클리어 후 정지 기능으로 [" + groupBox7.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game2_SecondBetSubLevel = 1;
                        Game2_SecondLevelChange.Text = (int.Parse(Game2_SecondLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game2_SecondBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game2_SecondBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game2_ThirdUseCheck.Checked)
            {
                TextBox Game2_ThirdBetPickLevel = Controls.Find("Game2_ThirdBetPickLevel" + Game2_ThirdBetSubLevel.ToString(), true)[0] as TextBox;
                if (!Game2_ThirdBetPickLevel.Text.Contains("통"))
                {
                    if (Game2_Result_3.Contains(Game2_ThirdBetPickLevel.Text))
                    {
                        if (Game2_ThirdBetSubLevel == 1)
                        {
                            Game2_ThirdBetSubLevel = 2;
                        }
                        else if (Game2_ThirdBetSubLevel == 2)
                        {
                            Game2_ThirdBetSubLevel = 3;
                        }
                        else if (Game2_ThirdBetSubLevel == 3)
                        {
                            if (int.Parse(Game2_ThirdLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_2 - 1).ToString());
                                item.SubItems.Add(Game2_ThirdUseCheck.Text);
                                item.SubItems.Add(Game2_ThirdLevelChange.Text);
                                Game2_Clear_Level_50.Items.Add(item);
                            }

                            Game2_ThirdBetSubLevel = 1;
                            Game2_ThirdLevelChange.Text = "1";
                            if (Game2_WinToStopCheckBox.Checked)
                            {
                                Game2_ThirdUseCheck.Checked = false;
                                txtLogAdd(txtLog2, "클리어 후 정지 기능으로 [" + groupBox6.Text + "] 게임이 종료되었습니다.", Color.Black);
                            }
                        }
                    }
                    else
                    {
                        Game2_ThirdBetSubLevel = 1;
                        Game2_ThirdLevelChange.Text = (int.Parse(Game2_ThirdLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game2_ThirdBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game2_ThirdBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }
        }

        private void Game3_Betting_Mode_Result_Processing()
        {
            // All_Win_Bet_Money
            // EosCruisePowerBallOddEvenBetMoneyLevel1
            Game3_Betting_Mode_Result_Process = true;

            if (Game3_PowerBallOddEvenUseCheck.Checked)
            {
                TextBox Game3_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game3_CruisePowerBallOddEvenBetPickLevel" + Game3_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game3_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game3_Result_1.Contains(Game3_CruisePowerBallOddEvenBetPickLevel.Text))
                    {
                        if (Game3_CruiseBetPowerBallOddEvenSubLevel == 1)
                        {
                            Game3_CruiseBetPowerBallOddEvenSubLevel = 2;
                        }
                        else if (Game3_CruiseBetPowerBallOddEvenSubLevel == 2)
                        {
                            Game3_CruiseBetPowerBallOddEvenSubLevel = 3;
                        }
                        else if (Game3_CruiseBetPowerBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_3 - 1).ToString());
                                item.SubItems.Add(Game3_PowerBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game3_CruisePowerBallOddEvenLevelChange.Text);
                                Game3_Clear_Level_50.Items.Add(item);
                            }
                            Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game3_CruisePowerBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game3_PowerBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
                        Game3_CruisePowerBallOddEvenLevelChange.Text = (int.Parse(Game3_CruisePowerBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game3_CruisePowerBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game3_CruisePowerBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game3_PowerBallUnderOverUseCheck.Checked)
            {
                TextBox Game3_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game3_CruisePowerBallUnderOverBetPickLevel" + Game3_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game3_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game3_Result_2.Contains(Game3_CruisePowerBallUnderOverBetPickLevel.Text))
                    {
                        if (Game3_CruiseBetPowerBallUnderOverSubLevel == 1)
                        {
                            Game3_CruiseBetPowerBallUnderOverSubLevel = 2;
                        }
                        else if (Game3_CruiseBetPowerBallUnderOverSubLevel == 2)
                        {
                            Game3_CruiseBetPowerBallUnderOverSubLevel = 3;
                        }
                        else if (Game3_CruiseBetPowerBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_3 - 1).ToString());
                                item.SubItems.Add(Game3_PowerBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game3_CruisePowerBallUnderOverLevelChange.Text);
                                Game3_Clear_Level_50.Items.Add(item);
                            }

                            Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game3_CruisePowerBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game3_PowerBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
                        Game3_CruisePowerBallUnderOverLevelChange.Text = (int.Parse(Game3_CruisePowerBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game3_CruisePowerBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game3_CruisePowerBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game3_NormalBallOddEvenUseCheck.Checked)
            {
                TextBox Game3_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game3_CruiseNormalBallOddEvenBetPickLevel" + Game3_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game3_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game3_Result_3.Contains(Game3_CruiseNormalBallOddEvenBetPickLevel.Text))
                    {
                        if (Game3_CruiseBetNormalBallOddEvenSubLevel == 1)
                        {
                            Game3_CruiseBetNormalBallOddEvenSubLevel = 2;
                        }
                        else if (Game3_CruiseBetNormalBallOddEvenSubLevel == 2)
                        {
                            Game3_CruiseBetNormalBallOddEvenSubLevel = 3;
                        }
                        else if (Game3_CruiseBetNormalBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_3 - 1).ToString());
                                item.SubItems.Add(Game3_NormalBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game3_CruiseNormalBallOddEvenLevelChange.Text);
                                Game3_Clear_Level_50.Items.Add(item);
                            }

                            Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game3_CruiseNormalBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game3_NormalBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
                        Game3_CruiseNormalBallOddEvenLevelChange.Text = (int.Parse(Game3_CruiseNormalBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game3_CruiseNormalBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game3_CruiseNormalBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game3_NormalBallUnderOverUseCheck.Checked)
            {
                TextBox Game3_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game3_CruiseNormalBallUnderOverBetPickLevel" + Game3_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game3_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game3_Result_4.Contains(Game3_CruiseNormalBallUnderOverBetPickLevel.Text))
                    {
                        if (Game3_CruiseBetNormalBallUnderOverSubLevel == 1)
                        {
                            Game3_CruiseBetNormalBallUnderOverSubLevel = 2;
                        }
                        else if (Game3_CruiseBetNormalBallUnderOverSubLevel == 2)
                        {
                            Game3_CruiseBetNormalBallUnderOverSubLevel = 3;
                        }
                        else if (Game3_CruiseBetNormalBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_3 - 1).ToString());
                                item.SubItems.Add(Game3_NormalBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game3_CruiseNormalBallUnderOverLevelChange.Text);
                                Game3_Clear_Level_50.Items.Add(item);
                            }

                            Game3_CruiseBetNormalBallUnderOverSubLevel = 1;
                            Game3_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game3_NormalBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game3_CruiseBetNormalBallUnderOverSubLevel = 1;
                        Game3_CruiseNormalBallUnderOverLevelChange.Text = (int.Parse(Game3_CruiseNormalBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game3_CruiseNormalBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game3_CruiseNormalBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }
        }

        private void Game4_Betting_Mode_Result_Processing()
        {
            Game4_Betting_Mode_Result_Process = true;
            if (Game4_BetMoney[0] + Game4_BetMoney[1] + Game4_BetMoney[2] + Game4_BetMoney[3] > 0)
            {
                int winCount = 0;
                // 우사홀, 좌사짝, 우삼짝, 좌삼홀
                if (Game4_Bet_Type == 1)
                {
                    if (Game4_Result_1.Contains("우"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_2.Contains("사"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_3.Contains("홀"))
                    {
                        winCount++;
                    }
                }
                else if (Game4_Bet_Type == 2)
                {
                    if (Game4_Result_1.Contains("좌"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_2.Contains("사"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_3.Contains("짝"))
                    {
                        winCount++;
                    }
                }
                else if (Game4_Bet_Type == 3)
                {
                    if (Game4_Result_1.Contains("우"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_2.Contains("삼"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_3.Contains("짝"))
                    {
                        winCount++;
                    }
                }
                else if (Game4_Bet_Type == 4)
                {
                    if (Game4_Result_1.Contains("좌"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_2.Contains("삼"))
                    {
                        winCount++;
                    }
                    else if (Game4_Result_3.Contains("홀"))
                    {
                        winCount++;
                    }
                }
                if (winCount == 0)
                {
                    Game4_BetMoneyLevel++;
                }
                else
                {
                    Game4_BetMoneyLevel = 0;
                }
            }
            else
            {
                Game4_BetMoneyLevel = 0;
            }
        }

        private void Game5_Betting_Mode_Result_Processing()
        {
            // All_Win_Bet_Money
            // EosCruisePowerBallOddEvenBetMoneyLevel1
            Game5_Betting_Mode_Result_Process = true;

            if (Game5_PowerBallOddEvenUseCheck.Checked)
            {
                TextBox Game5_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game5_CruisePowerBallOddEvenBetPickLevel" + Game5_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game5_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game5_Result_1.Contains(Game5_CruisePowerBallOddEvenBetPickLevel.Text))
                    {
                        if (Game5_CruiseBetPowerBallOddEvenSubLevel == 1)
                        {
                            Game5_CruiseBetPowerBallOddEvenSubLevel = 2;
                        }
                        else if (Game5_CruiseBetPowerBallOddEvenSubLevel == 2)
                        {
                            Game5_CruiseBetPowerBallOddEvenSubLevel = 3;
                        }
                        else if (Game5_CruiseBetPowerBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_5 - 1).ToString());
                                item.SubItems.Add(Game5_PowerBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game5_CruisePowerBallOddEvenLevelChange.Text);
                                Game5_Clear_Level_50.Items.Add(item);
                            }
                            Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game5_CruisePowerBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game5_PowerBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
                        Game5_CruisePowerBallOddEvenLevelChange.Text = (int.Parse(Game5_CruisePowerBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game5_CruisePowerBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game5_CruisePowerBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }

            if (Game5_PowerBallUnderOverUseCheck.Checked)
            {
                TextBox Game5_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game5_CruisePowerBallUnderOverBetPickLevel" + Game5_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game5_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game5_Result_2.Contains(Game5_CruisePowerBallUnderOverBetPickLevel.Text))
                    {
                        if (Game5_CruiseBetPowerBallUnderOverSubLevel == 1)
                        {
                            Game5_CruiseBetPowerBallUnderOverSubLevel = 2;
                        }
                        else if (Game5_CruiseBetPowerBallUnderOverSubLevel == 2)
                        {
                            Game5_CruiseBetPowerBallUnderOverSubLevel = 3;
                        }
                        else if (Game5_CruiseBetPowerBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_5 - 1).ToString());
                                item.SubItems.Add(Game5_PowerBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game5_CruisePowerBallUnderOverLevelChange.Text);
                                Game5_Clear_Level_50.Items.Add(item);
                            }

                            Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game5_CruisePowerBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game5_PowerBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
                        Game5_CruisePowerBallUnderOverLevelChange.Text = (int.Parse(Game5_CruisePowerBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game5_CruisePowerBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game5_CruisePowerBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game5_NormalBallOddEvenUseCheck.Checked)
            {
                TextBox Game5_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game5_CruiseNormalBallOddEvenBetPickLevel" + Game5_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game5_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game5_Result_3.Contains(Game5_CruiseNormalBallOddEvenBetPickLevel.Text))
                    {
                        if (Game5_CruiseBetNormalBallOddEvenSubLevel == 1)
                        {
                            Game5_CruiseBetNormalBallOddEvenSubLevel = 2;
                        }
                        else if (Game5_CruiseBetNormalBallOddEvenSubLevel == 2)
                        {
                            Game5_CruiseBetNormalBallOddEvenSubLevel = 3;
                        }
                        else if (Game5_CruiseBetNormalBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_5 - 1).ToString());
                                item.SubItems.Add(Game5_NormalBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game5_CruiseNormalBallOddEvenLevelChange.Text);
                                Game5_Clear_Level_50.Items.Add(item);
                            }

                            Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game5_CruiseNormalBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game5_NormalBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
                        Game5_CruiseNormalBallOddEvenLevelChange.Text = (int.Parse(Game5_CruiseNormalBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game5_CruiseNormalBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game5_CruiseNormalBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game5_NormalBallUnderOverUseCheck.Checked)
            {
                TextBox Game5_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game5_CruiseNormalBallUnderOverBetPickLevel" + Game5_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game5_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game5_Result_4.Contains(Game5_CruiseNormalBallUnderOverBetPickLevel.Text))
                    {
                        if (Game5_CruiseBetNormalBallUnderOverSubLevel == 1)
                        {
                            Game5_CruiseBetNormalBallUnderOverSubLevel = 2;
                        }
                        else if (Game5_CruiseBetNormalBallUnderOverSubLevel == 2)
                        {
                            Game5_CruiseBetNormalBallUnderOverSubLevel = 3;
                        }
                        else if (Game5_CruiseBetNormalBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_5 - 1).ToString());
                                item.SubItems.Add(Game5_NormalBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game5_CruiseNormalBallUnderOverLevelChange.Text);
                                Game5_Clear_Level_50.Items.Add(item);
                            }

                            Game5_CruiseBetNormalBallUnderOverSubLevel = 1;
                            Game5_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game5_NormalBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game5_CruiseBetNormalBallUnderOverSubLevel = 1;
                        Game5_CruiseNormalBallUnderOverLevelChange.Text = (int.Parse(Game5_CruiseNormalBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game5_CruiseNormalBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game5_CruiseNormalBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }
        }

        private void Game6_Betting_Mode_Result_Processing()
        {
            // All_Win_Bet_Money
            // EosCruisePowerBallOddEvenBetMoneyLevel1
            Game6_Betting_Mode_Result_Process = true;

            if (Game6_PowerBallOddEvenUseCheck.Checked)
            {
                TextBox Game6_CruisePowerBallOddEvenBetPickLevel = Controls.Find("Game6_CruisePowerBallOddEvenBetPickLevel" + Game6_CruiseBetPowerBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game6_CruisePowerBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game6_Result_1.Contains(Game6_CruisePowerBallOddEvenBetPickLevel.Text))
                    {
                        if (Game6_CruiseBetPowerBallOddEvenSubLevel == 1)
                        {
                            Game6_CruiseBetPowerBallOddEvenSubLevel = 2;
                        }
                        else if (Game6_CruiseBetPowerBallOddEvenSubLevel == 2)
                        {
                            Game6_CruiseBetPowerBallOddEvenSubLevel = 3;
                        }
                        else if (Game6_CruiseBetPowerBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_6 - 1).ToString());
                                item.SubItems.Add(Game6_PowerBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game6_CruisePowerBallOddEvenLevelChange.Text);
                                Game6_Clear_Level_50.Items.Add(item);
                            }
                            Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
                            Game6_CruisePowerBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game6_PowerBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
                        Game6_CruisePowerBallOddEvenLevelChange.Text = (int.Parse(Game6_CruisePowerBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game6_CruisePowerBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game6_CruisePowerBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }

            if (Game6_PowerBallUnderOverUseCheck.Checked)
            {
                TextBox Game6_CruisePowerBallUnderOverBetPickLevel = Controls.Find("Game6_CruisePowerBallUnderOverBetPickLevel" + Game6_CruiseBetPowerBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game6_CruisePowerBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game6_Result_2.Contains(Game6_CruisePowerBallUnderOverBetPickLevel.Text))
                    {
                        if (Game6_CruiseBetPowerBallUnderOverSubLevel == 1)
                        {
                            Game6_CruiseBetPowerBallUnderOverSubLevel = 2;
                        }
                        else if (Game6_CruiseBetPowerBallUnderOverSubLevel == 2)
                        {
                            Game6_CruiseBetPowerBallUnderOverSubLevel = 3;
                        }
                        else if (Game6_CruiseBetPowerBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_6 - 1).ToString());
                                item.SubItems.Add(Game6_PowerBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game6_CruisePowerBallUnderOverLevelChange.Text);
                                Game6_Clear_Level_50.Items.Add(item);
                            }

                            Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
                            Game6_CruisePowerBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game6_PowerBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
                        Game6_CruisePowerBallUnderOverLevelChange.Text = (int.Parse(Game6_CruisePowerBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game6_CruisePowerBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game6_CruisePowerBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game6_NormalBallOddEvenUseCheck.Checked)
            {
                TextBox Game6_CruiseNormalBallOddEvenBetPickLevel = Controls.Find("Game6_CruiseNormalBallOddEvenBetPickLevel" + Game6_CruiseBetNormalBallOddEvenSubLevel.ToString(), true)[0] as TextBox;
                if (!Game6_CruiseNormalBallOddEvenBetPickLevel.Text.Contains("통"))
                {
                    if (Game6_Result_3.Contains(Game6_CruiseNormalBallOddEvenBetPickLevel.Text))
                    {
                        if (Game6_CruiseBetNormalBallOddEvenSubLevel == 1)
                        {
                            Game6_CruiseBetNormalBallOddEvenSubLevel = 2;
                        }
                        else if (Game6_CruiseBetNormalBallOddEvenSubLevel == 2)
                        {
                            Game6_CruiseBetNormalBallOddEvenSubLevel = 3;
                        }
                        else if (Game6_CruiseBetNormalBallOddEvenSubLevel == 3)
                        {
                            if (int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_6 - 1).ToString());
                                item.SubItems.Add(Game6_NormalBallOddEvenUseCheck.Text);
                                item.SubItems.Add(Game6_CruiseNormalBallOddEvenLevelChange.Text);
                                Game6_Clear_Level_50.Items.Add(item);
                            }

                            Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
                            Game6_CruiseNormalBallOddEvenLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game6_NormalBallOddEvenUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
                        Game6_CruiseNormalBallOddEvenLevelChange.Text = (int.Parse(Game6_CruiseNormalBallOddEvenLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game6_CruiseNormalBallOddEvenBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game6_CruiseNormalBallOddEvenBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }


            if (Game6_NormalBallUnderOverUseCheck.Checked)
            {
                TextBox Game6_CruiseNormalBallUnderOverBetPickLevel = Controls.Find("Game6_CruiseNormalBallUnderOverBetPickLevel" + Game6_CruiseBetNormalBallUnderOverSubLevel.ToString(), true)[0] as TextBox;
                if (!Game6_CruiseNormalBallUnderOverBetPickLevel.Text.Contains("통"))
                {
                    if (Game6_Result_4.Contains(Game6_CruiseNormalBallUnderOverBetPickLevel.Text))
                    {
                        if (Game6_CruiseBetNormalBallUnderOverSubLevel == 1)
                        {
                            Game6_CruiseBetNormalBallUnderOverSubLevel = 2;
                        }
                        else if (Game6_CruiseBetNormalBallUnderOverSubLevel == 2)
                        {
                            Game6_CruiseBetNormalBallUnderOverSubLevel = 3;
                        }
                        else if (Game6_CruiseBetNormalBallUnderOverSubLevel == 3)
                        {
                            if (int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text) >= int.Parse(clearLevelSet.Text))
                            {
                                ListViewItem item;
                                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                item.UseItemStyleForSubItems = false;
                                item.SubItems.Add((dayroundNo_6 - 1).ToString());
                                item.SubItems.Add(Game6_NormalBallUnderOverUseCheck.Text);
                                item.SubItems.Add(Game6_CruiseNormalBallUnderOverLevelChange.Text);
                                Game6_Clear_Level_50.Items.Add(item);
                            }

                            Game6_CruiseBetNormalBallUnderOverSubLevel = 1;
                            Game6_CruiseNormalBallUnderOverLevelChange.Text = "1";

                            if (Game_WinToStopCheckBox.Checked)
                            {
                                Game6_NormalBallUnderOverUseCheck.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        Game6_CruiseBetNormalBallUnderOverSubLevel = 1;
                        Game6_CruiseNormalBallUnderOverLevelChange.Text = (int.Parse(Game6_CruiseNormalBallUnderOverLevelChange.Text) + 1).ToString();
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    TextBox BetPickLevel = Controls.Find("Game6_CruiseNormalBallUnderOverBetPickLevel" + i.ToString(), true)[0] as TextBox;
                    TextBox BetListLevel = Controls.Find("Game6_CruiseNormalBallUnderOverBetListLevel" + i.ToString(), true)[0] as TextBox;

                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    BetPickLevel.Text = "통";

                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;
                }
            }
        }
        private String checkGameCheck(CheckBox GameUseCheck, String BetListString, String BetPickString, String BetMoneyString, int subLevel, ListView RegistListView, String GameCode, int SubItemNum)
        {
            if (!GameUseCheck.Checked)
            {
                return "0";
            }
            TextBox BetListLevel = Controls.Find(BetListString + subLevel, true)[0] as TextBox;
            TextBox BetPickLevel = Controls.Find(BetPickString + subLevel, true)[0] as TextBox;
            TextBox BetMoneyLevel = Controls.Find(BetMoneyString + subLevel, true)[0] as TextBox;
            string[] stringSplit = BetListLevel.Text.Split(new char[] { '|' });

            for (int i = 0; i < stringSplit.Length; i++)
            {
                string[] s = stringSplit[i].Split(new char[] { '-' });
                string s1 = String.Empty;
                int s0Length = s[0].Length;

                if (s0Length <= RegistListView.Items.Count)
                {
                    for (int ii = 0; ii < s0Length; ii++)
                    {
                        ListViewItem item = RegistListView.Items[ii];
                        s1 += item.SubItems[SubItemNum].Text;
                    }
                }

                if (String.IsNullOrEmpty(s1))
                {
                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;

                    BetPickLevel.Text = "통";
                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                    break;
                }
                if (s[0].Equals(str_reverse(s1)))
                {
                    BetListLevel.BackColor = Color.Black;
                    BetListLevel.ForeColor = Color.White;

                    BetPickLevel.Text = s[1];
                    BetPickLevel.BackColor = Color.Black;
                    BetPickLevel.ForeColor = Color.White;
                    return BetMoneyLevel.Text;
                }
                else
                {
                    BetListLevel.BackColor = Color.WhiteSmoke;
                    BetListLevel.ForeColor = Color.Black;

                    BetPickLevel.Text = "통";
                    BetPickLevel.BackColor = Color.WhiteSmoke;
                    BetPickLevel.ForeColor = Color.Black;
                }
            }
            return "0";
        }
        public void setTimeRemaining(Button _button, double _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            _button.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }

        /**********************[클릭 계열만 사용 가능] 사용자의 현재 보유금을 불러온다.*************************/
        private Boolean LoadUserOwnMoney(string ResultUrl)
        {
            try
            {
                string returnMessage;
                var rm = UtilModel.MakeAsyncRequest(ResultUrl, "application/x-www-form-urlencoded; charset=UTF-8");
                returnMessage = rm.Result;
                logger.Info(returnMessage);

                if (returnMessage.Contains("code"))
                {
                    JObject jo = JObject.Parse(returnMessage);
                    int ret_code = int.Parse(jo.SelectToken("code").ToString());
                    if (ret_code == 1)
                    {
                        UtilModel.UserOwnMoney = int.Parse(jo.SelectToken("more_info").SelectToken("wallet").ToString());
                        txtLogAdd(txtLog1, "현재 사용자의 보유금 : " + UtilModel.StringFormatChanged(UtilModel.UserOwnMoney), Color.Black);
                        txtLogAdd(txtLog2, "현재 사용자의 보유금 : " + UtilModel.StringFormatChanged(UtilModel.UserOwnMoney), Color.Black);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
                return false;
            }
        }
        public void txtLogAdd(RichTextBox txtLog, string str, Color _color)
        {
            try
            {
                txtLog.SelectionColor = _color;
                txtLog.AppendText(UtilModel.getTime() + str + "\r\n");
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
                logger.Info(str);
            }
            catch (FormatException formatexception)
            {
                Console.WriteLine(formatexception);
            }
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
                    Game1_RemainingTimer.Stop();
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

        /*private void XMLLoad(String settingNum)
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("settings.xml");

                // 첫노드를 잡아주고 하위 노드를 선택한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode(settingNum);

                XmlNode selectNode;

                for (int i = 1; i <= 15; i++)
                {
                    selectNode = SubNode.SelectSingleNode("levelmoney" + i);
                    if (selectNode != null)
                    {
                        TextBox _msl = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                        _msl.Text = selectNode.InnerText;
                    }
                }

                for (int i = 2; i <= 15; i++)
                {
                    selectNode = SubNode.SelectSingleNode("magnification" + i);
                    if (selectNode != null)
                    {
                        ComboBox _mag = (Controls.Find("magnification" + i.ToString(), true)[0] as ComboBox);
                        _mag.Text = selectNode.InnerText;
                    }
                }

            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        *******************************************************************************************/
        static string str_reverse(string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        #region 행 배경색 설정하기 - SetRowBackgroundColor(listView, oddRowColor, evenRowColor)
        /// <summary>
        /// 행 배경색 설정하기
        /// </summary>
        /// <param name="listView">ListView 객체</param>
        /// <param name="oddRowColor">홀수 행 색상</param>
        /// <param name="evenRowColor">짝수 행 색상</param>
        public void SetRowBackgroundColor(ListView listView, Color oddRowColor, Color evenRowColor)
        {
            foreach (ListViewItem listViewItem in listView.Items)
            {
                if ((listViewItem.Index % 2) == 0)
                {
                    listViewItem.BackColor = evenRowColor;
                }
                else
                {
                    listViewItem.BackColor = oddRowColor;
                }
            }
        }
        #endregion

        System.Timers.Timer Game1_RemainingTimer = new System.Timers.Timer();
        System.Timers.Timer Game2_RemainingTimer = new System.Timers.Timer();
        System.Timers.Timer Game3_RemainingTimer = new System.Timers.Timer();
        System.Timers.Timer Game4_RemainingTimer = new System.Timers.Timer();
        System.Timers.Timer Game5_RemainingTimer = new System.Timers.Timer();
        System.Timers.Timer Game6_RemainingTimer = new System.Timers.Timer();

        /******************************************************************************
         * 코인파워볼 3분 : DSCB3_NM_PB
         * 코인파워볼 5분 : DSCB5_NM_PB
         * 하운슬로 3분 : HSPB3_NM_PB
         * 하운슬로 5분 : HSPB5_NM_PB
         * BEX 파워볼 : BEXB_NM_PB
         * EOS 파워볼 3분 : EOSP3_NM_PB
         * EOS 파워볼 5분 : EOSP5_NM_PB
         * 코인사다리 3분 : DSCL3_NM
         * 코인사다리 5분 : DSCL5_NM
         * 하운슬로 사다리 3분 : HSPL3_NM
         * 하운슬로 사다리 5분 : HSPL5_NM
         * BEX 사다리 : BEXL_NM
         */
        private readonly string[] Royal_GameCodePowerBall = new string[] { "DSCB3_NM_PB", "DSCB5_NM_PB", "EOSP3_NM_PB", "EOSP5_NM_PB", "HSPB3_NM_PB", "HSPB5_NM_PB", "KLAY2", "KLAY5", "BEXB_NM_PB", null, null, null, null, null, null, null, null, null, null, null, null, null, null };
        private readonly string[] Royal_GameCodeLadder = new string[] { "DSCL3_NM", "DSCL5_NM", "HSPL3_NM", "HSPL5_NM", "KLAYSA2", "KALYSA5", "BEXL_NM" };

        private readonly string[] Click_GameCodePowerBall = new string[] { "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", null, null, null, null, null, null, null, null, null, null, null, null, null, null };
        private readonly string[] Click_GameCodeLadder = new string[] { "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };

        private readonly string[] Life_GameCodePowerBall = new string[] { "DSCP3", "DSCP5", "EPB3", "EPB", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
        private readonly string[] Life_GameCodeLadder = new string[] { "CSA3", "CSA5", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };

        private string EPICKGameCode(string gamecode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "CNBALL3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "CNBALL5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "EOSBALL3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "EOSBALL5";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "HSBALL3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "HSBALL5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "CNLADR3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "CNLADR5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "HSLADR3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "HSLADR5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "CNBALL3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "CNBALL5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "EOSBALL3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "EOSBALL5";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "HSBALL3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "HSBALL5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "CNLADR3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "CNLADR5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "HSLADR3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "HSLADR5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCB3_NM_PB"))
                {
                    return "CNBALL3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCB5_NM_PB"))
                {
                    return "CNBALL5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EOSP3_NM_PB"))
                {
                    return "EOSBALL3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EOSP5_NM_PB"))
                {
                    return "EOSBALL5";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "HSBALL3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "HSBALL5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("DSCL3_NM"))
                {
                    return "CNLADR3";
                }
                // 코인사다리5분
                if (gamecode.Equals("DSCL5_NM"))
                {
                    return "CNLADR5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "HSLADR3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "HSLADR5";
                }
            }
            return "ERROR";
        }

        private string BepickGameCode(string gamecode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpower3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpower5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "eosball3m";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "eosball5m";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "hspowerball3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "hspowerball5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "hsladder3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "hsladder5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpower3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpower5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "eosball3m";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "eosball5m";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "hspowerball3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "hspowerball5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "hsladder3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "hsladder5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCB3_NM_PB"))
                {
                    return "coinpower3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCB5_NM_PB"))
                {
                    return "coinpower5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EOSP3_NM_PB"))
                {
                    return "eosball3m";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EOSP5_NM_PB"))
                {
                    return "eosball5m";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "hspowerball3";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "hspowerball5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("DSCL3_NM"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("DSCL5_NM"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "hsladder3";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "hsladder5";
                }
            }
            return "ERROR";
        }
        private string ScoreGameGameCode(string gamecode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coin_3_powerball";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coin_5_powerball";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "ERROR";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "ERROR";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "hounslow-5-powerball";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coin_3_powerball";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coin_5_powerball";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "hounslow-5-powerball";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coin_3_powerball";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coin_5_powerball";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "ERROR";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "ERROR";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSP3"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSP5"))
                {
                    return "hounslow-5-powerball";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coin_3_powerball";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coin_5_powerball";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPSA3"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPSA5"))
                {
                    return "hounslow-5-powerball";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCB3_NM_PB"))
                {
                    return "coin_3_powerball";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCB5_NM_PB"))
                {
                    return "coin_5_powerball";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EOSP3_NM_PB"))
                {
                    return "ERROR";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EOSP5_NM_PB"))
                {
                    return "ERROR";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "hounslow-5-powerball";
                }
                // 코인사다리 3분
                if (gamecode.Equals("DSCL3_NM"))
                {
                    return "coin_3_powerball";
                }
                // 코인사다리5분
                if (gamecode.Equals("DSCL5_NM"))
                {
                    return "coin_5_powerball";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "hounslow-3-powerball";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "hounslow-5-powerball";
                }
            }
            return "ERROR";
        }


        private string DreamScoreGameCode(string gamecode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpowerball5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpowerball5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCB3_NM_PB"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCB5_NM_PB"))
                {
                    return "coinpowerball5";
                }
            }
            return "ERROR";
        }
        private string UpDownGameCode(string gamecode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpowerball5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "eospowerball3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "eospowerball5";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "hspowerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "hspowerbal5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "hsladder";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "hsladde5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {// 코인파워볼3분
                if (gamecode.Equals("DSCP3"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCP5"))
                {
                    return "coinpowerball5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "eospowerball3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "eospowerball5";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "hspowerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "hspowerbal5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("CSA3"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("CSA5"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "hsladder";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "hsladde5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // 코인파워볼3분
                if (gamecode.Equals("DSCB3_NM_PB"))
                {
                    return "coinpowerball3";
                }
                // 코인파워볼5분
                if (gamecode.Equals("DSCB5_NM_PB"))
                {
                    return "coinpowerball5";
                }
                // EOS 파워볼 3분
                if (gamecode.Equals("EOSP3_NM_PB"))
                {
                    return "eospowerball5";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EOSP5_NM_PB"))
                {
                    return "eospowerball3";
                }
                // 하운슬롯 3분
                if (gamecode.Equals("HSPB3_NM_PB"))
                {
                    return "hspowerball";
                }
                // 하운슬롯 5분
                if (gamecode.Equals("HSPB5_NM_PB"))
                {
                    return "hspowerbal5";
                }
                // 코인사다리 3분
                if (gamecode.Equals("DSCL3_NM"))
                {
                    return "coinladder3";
                }
                // 코인사다리5분
                if (gamecode.Equals("DSCL5_NM"))
                {
                    return "coinladder5";
                }
                // 하운슬롯사다리 3분
                if (gamecode.Equals("HSPL3_NM"))
                {
                    return "hsladder";
                }
                // 하운슬롯사다리5분
                if (gamecode.Equals("HSPL5_NM"))
                {
                    return "hsladde5";
                }
            }
            return "ERROR";
        }

        private string NtryGameCode(string gamecode)
        {

            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "3min";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "5min";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                // EOS 파워볼 3분
                if (gamecode.Equals("EPB3"))
                {
                    return "eospowerball3";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EPB"))
                {
                    return "eospowerball5";
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                // EOS 파워볼 3분
                if (gamecode.Equals("EOSP3_NM_PB"))
                {
                    return "eospowerball5";
                }
                // EOS 파워볼 5분
                if (gamecode.Equals("EOSP5_NM_PB"))
                {
                    return "eospowerball3";
                }
            }
            return "ERROR";
        }
        string Game1_Result_1 = string.Empty;
        string Game1_Result_2 = string.Empty;
        string Game1_Result_3 = string.Empty;
        string Game1_Result_4 = string.Empty;

        string Game2_Result_1 = string.Empty;
        string Game2_Result_2 = string.Empty;
        string Game2_Result_3 = string.Empty;

        string Game3_Result_1 = string.Empty;
        string Game3_Result_2 = string.Empty;
        string Game3_Result_3 = string.Empty;
        string Game3_Result_4 = string.Empty;

        string Game4_Result_1 = string.Empty;
        string Game4_Result_2 = string.Empty;
        string Game4_Result_3 = string.Empty;

        string Game5_Result_1 = string.Empty;
        string Game5_Result_2 = string.Empty;
        string Game5_Result_3 = string.Empty;
        string Game5_Result_4 = string.Empty;

        string Game6_Result_1 = string.Empty;
        string Game6_Result_2 = string.Empty;
        string Game6_Result_3 = string.Empty;
        string Game6_Result_4 = string.Empty;

        int Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
        int Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
        int Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
        int Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

        int Game2_FirstBetSubLevel = 1;
        int Game2_SecondBetSubLevel = 1;
        int Game2_ThirdBetSubLevel = 1;

        int Game3_CruiseBetPowerBallOddEvenSubLevel = 1;
        int Game3_CruiseBetPowerBallUnderOverSubLevel = 1;
        int Game3_CruiseBetNormalBallOddEvenSubLevel = 1;
        int Game3_CruiseBetNormalBallUnderOverSubLevel = 1;

        int Game5_CruiseBetPowerBallOddEvenSubLevel = 1;
        int Game5_CruiseBetPowerBallUnderOverSubLevel = 1;
        int Game5_CruiseBetNormalBallOddEvenSubLevel = 1;
        int Game5_CruiseBetNormalBallUnderOverSubLevel = 1;

        int Game6_CruiseBetPowerBallOddEvenSubLevel = 1;
        int Game6_CruiseBetPowerBallUnderOverSubLevel = 1;
        int Game6_CruiseBetNormalBallOddEvenSubLevel = 1;
        int Game6_CruiseBetNormalBallUnderOverSubLevel = 1;


        private int GameNumber_1 = 0;
        private int GameNumber_2 = 0;
        private int GameNumber_3 = 0;
        private int GameNumber_4 = 0;
        private int GameNumber_5 = 0;
        private int GameNumber_6 = 0;

        int BetRemainingTime_1 = 0;
        int BetRemainingTime_2 = 0;
        int BetRemainingTime_3 = 0;
        int BetRemainingTime_4 = 0;
        int BetRemainingTime_5 = 0;
        int BetRemainingTime_6 = 0;

        int BetEndSec_1 = 30;
        int BetEndSec_2 = 30;
        int BetEndSec_3 = 30;
        int BetEndSec_4 = 30;
        int BetEndSec_5 = 30;
        int BetEndSec_6 = 30;

        string GameCode_1 = null;
        string GameCode_2 = null;
        string GameCode_3 = null;
        string GameCode_4 = null;
        string GameCode_5 = null;
        string GameCode_6 = null;

        DateTime StartDateTime_1;
        DateTime StartDateTime_2;
        DateTime StartDateTime_3;
        DateTime StartDateTime_4;
        DateTime StartDateTime_5;
        DateTime StartDateTime_6;

        Boolean StartGame_1 = false;
        Boolean StartGame_2 = false;
        Boolean StartGame_3 = false;
        Boolean StartGame_4 = false;
        Boolean StartGame_5 = false;
        Boolean StartGame_6 = false;

        Boolean Game1_Betting_Mode_Result_Process = false;
        Boolean Game2_Betting_Mode_Result_Process = false;
        Boolean Game3_Betting_Mode_Result_Process = false;
        Boolean Game4_Betting_Mode_Result_Process = false;
        Boolean Game5_Betting_Mode_Result_Process = false;
        Boolean Game6_Betting_Mode_Result_Process = false;


        /************1페이지 게임 배팅 등록 상태**********/
        Boolean Game_1_Betting_Complete_Status = false;
        Boolean Game_2_Betting_Complete_Status = false;
        Boolean Game_3_Betting_Complete_Status = false;
        Boolean Game_4_Betting_Complete_Status = false;
        Boolean Game_5_Betting_Complete_Status = false;
        Boolean Game_6_Betting_Complete_Status = false;

        /************게임 1의 결과 값 로드 여부*********/
        Boolean Game_1_Result_Load_Complete = false;
        Boolean Game_2_Result_Load_Complete = false;
        Boolean Game_3_Result_Load_Complete = false;
        Boolean Game_4_Result_Load_Complete = false;
        Boolean Game_5_Result_Load_Complete = false;
        Boolean Game_6_Result_Load_Complete = false;

        /************게임 1의 배팅 점검 여부*********/
        Boolean Game_1_Check_Complete = false;
        Boolean Game_2_Check_Complete = false;
        Boolean Game_3_Check_Complete = false;
        Boolean Game_4_Check_Complete = false;
        Boolean Game_5_Check_Complete = false;
        Boolean Game_6_Check_Complete = false;

        private int[] Game1_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game2_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game3_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game4_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game5_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game6_BetMoney = new int[] { 0, 0, 0, 0 };

        private string[] Game1_BetPick = new string[] { null, null, null, null };
        private string[] Game2_BetPick = new string[] { null, null, null, null };
        private string[] Game3_BetPick = new string[] { null, null, null, null };
        private string[] Game4_BetPick = new string[] { null, null, null, null };
        private string[] Game5_BetPick = new string[] { null, null, null, null };
        private string[] Game6_BetPick = new string[] { null, null, null, null };

        Double roundNo_1 = 0;
        Double roundNo_2 = 0;
        Double roundNo_3 = 0;
        Double roundNo_4 = 0;
        Double roundNo_5 = 0;
        Double roundNo_6 = 0;

        int dayroundNo_1 = 0;
        int dayroundNo_2 = 0;
        int dayroundNo_3 = 0;
        int dayroundNo_4 = 0;
        int dayroundNo_5 = 0;
        int dayroundNo_6 = 0;

        double[,] Game_1_CruiseAllBetMoney = new double[100, 3];
        double[,] Game_2_CruiseAllBetMoney = new double[100, 3];
        double[,] Game_3_CruiseAllBetMoney = new double[100, 3];
        double[] Game_4_CruiseAllBetMoney = new double[100];
        double[,] Game_5_CruiseAllBetMoney = new double[100, 3];
        double[,] Game_6_CruiseAllBetMoney = new double[100, 3];

        private void virtualMoney_CheckedChanged(object sender, EventArgs e)
        {
            if (virtualMoney.Checked)
            {
                label1.Text = "가상 보유금으로 진행 중입니다.";
                lblTxtNowMoney.Text = "3,000,000";
                UtilModel.UserOwnMoney = 3000000;
            }
            else
            {
                lblTxtNowMoney.Text = "0";
                UtilModel.UserOwnMoney = 0;
                label1.Text = "";
            }
        }

        string[] pattern_1 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-짝|짝홀홀-홀", "홀짝짝짝-홀|짝홀홀홀-짝", "언오-오|오언-언", "언오오-오|오언언-언", "언오오오-언|오언언언-오" };

        string[] pattern_2 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-짝|짝홀홀-홀", "홀짝짝짝-짝|짝홀홀홀-홀", "언오-오|오언-언", "언오오-오|오언언-언", "언오오오-오|오언언언-언" };

        string[] pattern_3 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-홀|짝홀홀-짝", "홀짝짝홀-짝|짝홀홀짝-홀", "언오-오|오언-언", "언오오-언|오언언-오", "언오오언-오|오언언오-언" };

        string[] pattern_4 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-홀|짝홀홀-짝", "홀짝짝홀-홀|짝홀홀짝-짝", "언오-오|오언-언", "언오오-언|오언언-오", "언오오언-언|오언언오-오" };

        string[] pattern_5 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-짝|짝홀짝-홀", "홀짝홀짝-짝|짝홀짝홀-홀", "언오-언|오언-오", "언오언-오|오언오-언", "언오언오-오|오언오언-언" };

        string[] pattern_6 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-짝|짝홀짝-홀", "홀짝홀짝-홀|짝홀짝홀-짝", "언오-언|오언-오", "언오언-오|오언오-언", "언오언오-언|오언오언-오" };

        string[] pattern_7 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-홀|짝홀짝-짝", "홀짝홀홀-짝|짝홀짝짝-홀", "언오-언|오언-오", "언오언-언|오언오-오", "언오언언-오|오언오오-언" };

        string[] pattern_8 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-홀|짝홀짝-짝", "홀짝홀홀-홀|짝홀짝짝-짝", "언오-언|오언-오", "언오언-언|오언오-오", "언오언언-언|오언오오-오" };

        private void game1_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_1[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_1[5];
        }

        private void game1_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_2[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_2[5];
        }


        private void game1_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_3[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_3[5];
        }

        private void game1_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_4[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_4[5];
        }


        private void game1_pattern_5_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_5[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_5[5];
        }

        private void game1_pattern_6_CheckedChanged(object sender, EventArgs e)
        {

            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_6[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_6[5];
        }

        private void game1_pattern_7_CheckedChanged(object sender, EventArgs e)
        {

            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_7[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_7[5];
        }

        private void game1_pattern_8_CheckedChanged(object sender, EventArgs e)
        {

            Game1_CruisePowerBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = pattern_8[5];

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_8[5];
        }

        private void game3_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_1[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_1[5];
        }

        private void game3_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_2[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_2[5];
        }

        private void game3_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_3[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_3[5];
        }
        private void game3_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_4[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_4[5];
        }

        private void game3_pattern_5_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_5[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_5[5];
        }

        private void game3_pattern_6_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_6[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_6[5];
        }

        private void game3_pattern_7_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_7[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_7[5];
        }

        private void game3_pattern_8_CheckedChanged(object sender, EventArgs e)
        {
            Game3_CruisePowerBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game3_CruisePowerBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game3_CruisePowerBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game3_CruisePowerBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game3_CruisePowerBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game3_CruisePowerBallUnderOverBetListLevel3.Text = pattern_8[5];

            Game3_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game3_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game3_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game3_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game3_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game3_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_8[5];
        }
        private void game2_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game2_FirstBetListLevel1.Text = "좌우-우|우좌-좌";
            Game2_FirstBetListLevel2.Text = "좌우우-우|우좌좌-좌";
            Game2_FirstBetListLevel3.Text = "좌우우우-좌|우좌좌좌-우";

            Game2_SecondBetListLevel1.Text = "삼사-사|사삼-삼";
            Game2_SecondBetListLevel2.Text = "삼사사-사|사삼삼-삼";
            Game2_SecondBetListLevel3.Text = "삼사사사-삼|사삼삼삼-사";

            Game2_ThirdBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game2_ThirdBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game2_ThirdBetListLevel3.Text = "홀짝짝짝-홀|짝홀홀홀-짝";
        }

        private void game2_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game2_FirstBetListLevel1.Text = "좌우-우|우좌-좌";
            Game2_FirstBetListLevel2.Text = "좌우우-우|우좌좌-좌";
            Game2_FirstBetListLevel3.Text = "좌우우우-우|우좌좌좌-좌";

            Game2_SecondBetListLevel1.Text = "삼사-사|사삼-삼";
            Game2_SecondBetListLevel2.Text = "삼사사-사|사삼삼-삼";
            Game2_SecondBetListLevel3.Text = "삼사사사-사|사삼삼삼-삼";

            Game2_ThirdBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game2_ThirdBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game2_ThirdBetListLevel3.Text = "홀짝짝짝-짝|짝홀홀홀-홀";
        }

        private void game2_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game2_FirstBetListLevel1.Text = "좌우-우|우좌-좌";
            Game2_FirstBetListLevel2.Text = "좌우우-좌|우좌좌-우";
            Game2_FirstBetListLevel3.Text = "좌우우좌-좌|우좌좌우-우";

            Game2_SecondBetListLevel1.Text = "삼사-사|사삼-삼";
            Game2_SecondBetListLevel2.Text = "삼사사-삼|사삼삼-사";
            Game2_SecondBetListLevel3.Text = "삼사사삼-삼|사삼삼사-사";

            Game2_ThirdBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game2_ThirdBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game2_ThirdBetListLevel3.Text = "홀짝짝홀-홀|짝홀홀짝-짝";
        }

        private void game2_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game2_FirstBetListLevel1.Text = "좌우-우|우좌-좌";
            Game2_FirstBetListLevel2.Text = "좌우우-좌|우좌좌-우";
            Game2_FirstBetListLevel3.Text = "좌우우좌-우|우좌좌우-좌";

            Game2_SecondBetListLevel1.Text = "삼사-사|사삼-삼";
            Game2_SecondBetListLevel2.Text = "삼사사-삼|사삼삼-사";
            Game2_SecondBetListLevel3.Text = "삼사사삼-사|사삼삼사-삼";

            Game2_ThirdBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game2_ThirdBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game2_ThirdBetListLevel3.Text = "홀짝짝홀-짝|짝홀홀짝-홀";
        }

        int Game4_Bet_Type = 1;
        private void Right_Four_Odd_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Game4_Bet_Type = 1;
        }

        private void Left_Four_Even_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Game4_Bet_Type = 2;
        }

        private void Right_Three_Even_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Game4_Bet_Type = 3;
        }

        private void Left_Three_Odd_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Game4_Bet_Type = 4;
        }

        private void game5_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_1[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_1[5];

        }

        private void game5_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_2[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_2[5];
        }

        private void game5_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_3[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_3[5];
        }

        private void game5_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_4[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_4[5];
        }

        private void game5_pattern_5_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_5[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_5[5];
        }

        private void game5_pattern_6_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_6[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_6[5];
        }

        private void game5_pattern_7_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_7[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_7[5];
        }

        private void game5_pattern_8_CheckedChanged(object sender, EventArgs e)
        {
            Game5_CruisePowerBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game5_CruisePowerBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game5_CruisePowerBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game5_CruisePowerBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game5_CruisePowerBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game5_CruisePowerBallUnderOverBetListLevel3.Text = pattern_8[5];

            Game5_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game5_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game5_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game5_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game5_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game5_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_8[5];
        }
        private void game6_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_1[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_1[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_1[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_1[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_1[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_1[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_1[5];
        }

        private void game6_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_2[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_2[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_2[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_2[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_2[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_2[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_2[5];
        }

        private void game6_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_3[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_3[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_3[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_3[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_3[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_3[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_3[5];
        }

        private void game6_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_4[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_4[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_4[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_4[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_4[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_4[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_4[5];
        }

        private void game6_pattern_5_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_5[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_5[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_5[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_5[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_5[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_5[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_5[5];
        }

        private void game6_pattern_6_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_6[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_6[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_6[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_6[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_6[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_6[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_6[5];
        }

        private void game6_pattern_7_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_7[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_7[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_7[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_7[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_7[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_7[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_7[5];
        }

        private void game6_pattern_8_CheckedChanged(object sender, EventArgs e)
        {
            Game6_CruisePowerBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game6_CruisePowerBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game6_CruisePowerBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game6_CruisePowerBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game6_CruisePowerBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game6_CruisePowerBallUnderOverBetListLevel3.Text = pattern_8[5];

            Game6_CruiseNormalBallOddEvenBetListLevel1.Text = pattern_8[0];
            Game6_CruiseNormalBallOddEvenBetListLevel2.Text = pattern_8[1];
            Game6_CruiseNormalBallOddEvenBetListLevel3.Text = pattern_8[2];

            Game6_CruiseNormalBallUnderOverBetListLevel1.Text = pattern_8[3];
            Game6_CruiseNormalBallUnderOverBetListLevel2.Text = pattern_8[4];
            Game6_CruiseNormalBallUnderOverBetListLevel3.Text = pattern_8[5];
        }
        private void profitMoneyInitButton_Click(object sender, EventArgs e)
        {
            Game1_StackMoney.Text = "0";
            Game2_StackMoney.Text = "0";
            Game3_StackMoney.Text = "0";
            Game4_StackMoney.Text = "0";
            Game5_StackMoney.Text = "0";
            Game6_StackMoney.Text = "0";

            Game1_WinMoney.Text = "0";
            Game2_WinMoney.Text = "0";
            Game3_WinMoney.Text = "0";
            Game4_WinMoney.Text = "0";
            Game5_WinMoney.Text = "0";
            Game6_WinMoney.Text = "0";

            Game1_ProfitMoney.Text = "0";
            Game2_ProfitMoney.Text = "0";
            Game3_ProfitMoney.Text = "0";
            Game4_ProfitMoney.Text = "0";
            Game5_ProfitMoney.Text = "0";
            Game6_ProfitMoney.Text = "0";
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
