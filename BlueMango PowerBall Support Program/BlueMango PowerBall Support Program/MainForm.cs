using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace BlueMango_PowerBall_Support_Program
{
    /*
     * // GET /auto/api/get_pushed_result?gm=DSCP3&d=20220921&r=20220921436&k=SKGD6CX2PYVONWZFLAGX8JY5M7QZFMB9&_=1663763684460 HTTP/1.1 
    게임 정보
     :8085/auto/api/round_timing?gm={gamecode}&u={userid}&_={unixtime}
    {"code":1,"comment":"","more_info":{"dateIdx":20220914,"roundNo":284,"dayRoundNo":284,"leftTime":144,"end":1663166400,"end_sec":30}}
    게임 결과
     :8085/auto/api/get_pushed_result?gm={gamecode}&d={todaydate}&r=96&k={apikey}&_={unixtime}
     {"code":1,"comment":"","more_info":{"sum":67,"pball":7}}
    현재 보유금 :8085/auto/api/user_bal?u={userid}&k={apikey}
     코인파워볼3분 DSCP3
     코인파워볼5분 DSCP5
     EOS파워볼 3분 EPB3 
     EOS파워볼 5분 EPB
     하운슬로파워볼3분 HSP3
     하운슬로파워볼5분 HSP5
    클레이파워볼2분 KLAY2
    클레이파워볼5분 KLAY5


    코인사다리3분 CSA3
    코인사다리5분 CSA5
    하운슬로파사3분 HSPSA3
    하운슬로파사5분 HSPSA5
    클레이사다리2분 KLAYSA2
    클레이사다리5분 KLAYSA2
    */

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

            Game1_CruiseBetMoneyPercentSettingComboBox.Text = "100";
            BetMoneySetting(Game1_CruiseBetListView, Game1_CruiseBetMoneySettingTextBox, Game1_CruiseBetMoneyPercentSettingComboBox, Game_1_CruiseAllBetMoney);
            Game1_CruiseBetListView.DoubleBuffered(true);
            Game1_CruiseBetRegistListView.DoubleBuffered(true);
            Game1_Clear_Level_50.DoubleBuffered(true);

            Game2_CruiseBetMoneyPercentSettingComboBox.Text = "100";
            BetMoneySetting(Game2_CruiseBetListView, Game2_CruiseBetMoneySettingTextBox, Game2_CruiseBetMoneyPercentSettingComboBox, Game_2_CruiseAllBetMoney);
            Game2_CruiseBetListView.DoubleBuffered(true);
            Game2_CruiseBetRegistListView.DoubleBuffered(true);
            Game2_Clear_Level_50.DoubleBuffered(true);

            this.Text = "BlueMango PowerBall Support Program || " + UtilModel.UserSiteUrlAddress + " || " + UtilModel.UserId + " || " + UtilModel._ip;

            lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
            _RemainingTimer.Interval = 1000;
            _RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            _RemainingTimer.Start();
            virtualMoney.Checked = true;
            if (virtualMoney.Checked)
            {
                UtilModel.UserOwnMoney = 3000000;
                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);
            }
        }
        private void BetMoneySetting(ListView BetListView, TextBox BetMoneySetting, ComboBox BetMoneyPercentSetting, double[,] CruiseAllBetMoney)
        {
            BetListView.Items.Clear();
            ListViewItem Game1_CruiseBetListViewSubItem;
            double ValueSum = 0;
            string[] sarray = BetMoneySetting.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int numberI = 1; numberI <= BetMoneySetting.Lines.Length; numberI++)
            {
                if (ValueSum < 100000000)
                {
                    int.TryParse(Regex.Replace(sarray[numberI - 1], @"\D", ""), out int outValue);
                    outValue = (int)(outValue * int.Parse(Regex.Replace(BetMoneyPercentSetting.Text, @"\D", "")) * 0.01);
                    ValueSum += outValue;
                    Game1_CruiseBetListViewSubItem = new ListViewItem(numberI.ToString());
                    Game1_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged(outValue)); // 1차 배팅금
                    Game1_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95))); // 2차 배팅금
                    Game1_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)(outValue * 1.95 * 1.95))); // 3차 배팅금
                    Game1_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)ValueSum)); // 총 배팅금
                    Game1_CruiseBetListViewSubItem.SubItems.Add(UtilModel.StringFormatChanged((int)((outValue * 1.95 * 1.95 * 1.95) - ValueSum))); // 당첨 이익금
                    BetListView.Items.Add(Game1_CruiseBetListViewSubItem);

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

        private void Game1_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BetMoneySetting(Game1_CruiseBetListView, Game1_CruiseBetMoneySettingTextBox, Game1_CruiseBetMoneyPercentSettingComboBox, Game_1_CruiseAllBetMoney);
        }

        private void Game2_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BetMoneySetting(Game2_CruiseBetListView, Game2_CruiseBetMoneySettingTextBox, Game2_CruiseBetMoneyPercentSettingComboBox, Game_2_CruiseAllBetMoney);
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] sarray = comboBox1.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            groupBox1.Text = sarray[1] + " 배팅 금액 정보";
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_1 = outValue - 1;
            GameCode_1 = GameCode[GameNumber_1];
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] sarray = comboBox7.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            BetInformation2.Text = sarray[1] + " 배팅 금액 정보";
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            GameNumber_2 = outValue - 1;
            GameCode_2 = GameCode[GameNumber_2];
        }
        /********************************첫페이지 게임 시작 및 정지 버튼*************************************/
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(GameCode_1))
            {
                txtLogAdd(txtLog1, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_1)
            {
                button1.Text = "배팅이 진행 중입니다.";
                button1.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_1 = true;
                StartDateTime_1 = DateTime.Now;
                loadRoungInformation1(GameCode_1);
            }
            else
            {
                button1.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                button1.Text = "배팅이 정지되었습니다.";
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
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(GameCode_2))
            {
                txtLogAdd(txtLog2, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }
            if (!StartGame_2)
            {
                button3.Text = "배팅이 진행 중입니다.";
                button3.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame_2 = true;
                StartDateTime_2 = DateTime.Now;
                loadRoungInformation2(GameCode_2);
            }
            else
            {
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
            }
        }

        private void Game1_WinToStopCheckBox_CheckedChanged(object sender, EventArgs e)
        {

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
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game1_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game1_StackMoney.Text, @"\D", ""), out outValue);

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
            txtLogAdd(txtLog1, dayroundNo_1 + "회차 등록이 완료되었습니다.", Color.OrangeRed);
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        private void Game2_Bet_Processing_RegistListView()
        {
            int outValue;
            int.TryParse(Regex.Replace(Game2_StackMoney.Text, @"\D", ""), out outValue);

            ListViewItem item;
            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add((dayroundNo_2.ToString()));
            if (!String.IsNullOrEmpty(Game2_BetPick[0]))
            {
                if (Game2_BetPick[0].Contains("통"))
                {
                    Game2_BetPick[0] = "";
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
                Game2_BetPick[0] = "";
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
                    Game2_BetPick[1] = "";
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
                Game2_BetPick[1] = "";
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
                    Game2_BetPick[2] = "";
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
                Game2_BetPick[2] = "";
                Game2_BetMoney[2] = 0;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
            }
            Game2_StackMoney.Text = UtilModel.StringFormatChanged(outValue);

            Game2_CruiseBetRegistListView.Items.Add(item);
            txtLogAdd(txtLog2, dayroundNo_2 + "회차 등록이 완료되었습니다.", Color.OrangeRed);
            //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);
        }

        StringBuilder Game_Url_Param_String;
        private Boolean Game_Bet_Processing_Final(String[] pick, int[] money, Double gameInning, int remainTime, RichTextBox logBox, int endSec, string GameCode)
        {
            Boolean Betting_Status = false;
            int All_Betting_Money = money[0] + money[1] + money[2] + money[3];
            if (All_Betting_Money >= 100)
            {
                Game_Url_Param_String = new StringBuilder();
                Game_Url_Param_String.Append(UtilModel.UserSiteUrlAddress);
                Game_Url_Param_String.Append(":8082/api/bet");
                Game_Url_Param_String.AppendFormat("?userid={0}", UtilModel.UserId);
                Game_Url_Param_String.AppendFormat("&key={0}", UtilModel.Bet_Api_Key);

                Game_Url_Param_String.AppendFormat("&gm={0}", GameCode);
                Game_Url_Param_String.AppendFormat("&tdate={0}", todayDate);
                Game_Url_Param_String.AppendFormat("&rno={0}", gameInning.ToString());
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

                Game_Url_Param_String.AppendFormat("&nonce={0}", gameInning);

                String Message = null;
                int CountResult = 0;
                while (true)
                {
                    try
                    {
                        //Uri myUri = new Uri(sb);
                        var returnMessage = UtilModel.MakeAsyncRequest(Game_Url_Param_String.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                        Message = returnMessage.Result;
                        logger.Info(Message);
                        if (Message.Contains("ret_code"))
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
                            UtilModel.Delay(1500);
                            continue;
                        }
                    }
                    catch (Exception _ex)
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
                        logger.Error(_ex.ToString());
                        UtilModel.Delay(1500);
                        continue;
                    }
                }
                if (!Message.Contains("ret_code"))
                {
                    string errorMessage = "[" + CountResult + "] 배팅 시도를 여러 차례 시도하였지만 실패하여 배팅이 등록되지 않았습니다.";
                    txtLogAdd(logBox, errorMessage, Color.White);
                    logger.Info(errorMessage);
                }
                else
                {
                    JObject jo = JObject.Parse(Message);
                    int ret_code = int.Parse(jo.SelectToken("ret_code").ToString());
                    var ret_message = jo.SelectToken("comment").ToString();
                    if (ret_code == 1)
                    {
                        UtilModel.UserOwnMoney = int.Parse(jo.SelectToken("more_info.balance").ToString());

                        lblTxtNowMoney.Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);

                        txtLogAdd(logBox, "[" + gameInning + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                        logger.Info("[" + gameInning + "] 정상 배팅 등록 완료.");
                        Betting_Status = true;
                    }
                    else if (ret_code < 0)
                    {
                        txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + ret_message, Color.White);
                        MessageBox.Show("배팅 실패 : " + ret_message);
                        logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                    }
                    else
                    {
                        txtLogAdd(logBox, "배팅 실패 : " + ret_code + " : " + ret_message, Color.White);
                        MessageBox.Show("배팅 실패 : " + ret_message);
                        logger.Info(gameInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                    }
                }
            }
            return Betting_Status;
        }

        // GET /auto/api/get_pushed_result?gm=DSCP3&d=20220921&r=20220921436&k=SKGD6CX2PYVONWZFLAGX8JY5M7QZFMB9&_=1663763684460 HTTP/1.1 
        private Boolean loadAPISiteResultGame(string gamecode, int dayroundno)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}:8085/auto/api/get_pushed_result?gm={1}&d={2}&r={3}&k={4}", UtilModel.UserSiteUrlAddress, gamecode, todayDate, dayroundno, UtilModel.Bet_Api_Key);
                string returnMessage;

                var rm = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                returnMessage = rm.Result;
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
                                    Game1_Result_2 = "오";
                                }
                                else
                                {
                                    Game1_Result_2 = "언";
                                }
                                if (ballSum[5] % 2 == 1)
                                {
                                    Game1_Result_1 = "홀";
                                }
                                else
                                {
                                    Game1_Result_1 = "짝";
                                }

                                if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] > 72)
                                {
                                    Game1_Result_4 = "오";
                                }
                                else
                                {
                                    Game1_Result_4 = "언";
                                }
                                if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] % 2 == 1)
                                {
                                    Game1_Result_3 = "홀";
                                }
                                else
                                {
                                    Game1_Result_3 = "짝";
                                }
                                txtLogAdd(txtLog1, "[" + (dayroundno) + "회] 파워볼 : " + Game1_Result_1 + " | " + Game1_Result_2 + " || 일반볼 : " + Game1_Result_3 + " | " + Game1_Result_4, Color.Black);
                                return true;
                            }
                            else
                            {
                                // 파워볼 합계
                                int.TryParse(jo.SelectToken("more_info.pball").ToString(), out int pball);
                                if (pball > 4)
                                {
                                    Game1_Result_2 = "오";
                                }
                                else
                                {
                                    Game1_Result_2 = "언";
                                }
                                if (pball % 2 == 1)
                                {
                                    Game1_Result_1 = "홀";
                                }
                                else
                                {
                                    Game1_Result_1 = "짝";
                                }
                                // 일반볼 합계
                                int.TryParse(jo.SelectToken("more_info.sum").ToString(), out int sum);
                                if (sum > 72)
                                {
                                    Game1_Result_4 = "오";
                                }
                                else
                                {
                                    Game1_Result_4 = "언";
                                }
                                if (sum % 2 == 1)
                                {
                                    Game1_Result_3 = "홀";
                                }
                                else
                                {
                                    Game1_Result_3 = "짝";
                                }
                                txtLogAdd(txtLog1, "[" + (dayroundno) + "회] 파워볼 : " + Game1_Result_1 + " | " + Game1_Result_2 + " || 일반볼 : " + Game1_Result_3 + " | " + Game1_Result_4, Color.Black);
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

        private Boolean loadBepickResultGame(string gamecode, int gameInning)
        {
            try
            {
                // https://updown2.com/api/last?g_type=coinladder3&_=1663507276707
                // {"error":false,"msg":"성공","g_date":"2022-09-18","date_round":448,"lr":"좌","line":"3","oe":"짝"}
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://bepick.net/live/result/{0}", gamecode == "CSA3" ? "coinladder3" : "coinladder5");
                Uri contoso = new Uri(stringBuilder.ToString());
                string returnVal = UtilModel.GetHttp(contoso, "GET", null);
                logger.Info(stringBuilder.ToString());
                logger.Info(returnVal);

                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("fd1"))
                {
                    JObject jo = JObject.Parse(returnVal);
                    if (jo.SelectToken("fd1").ToString().Equals("1"))
                    {
                        Game2_Result_1 = "좌";
                    }
                    else
                    {
                        Game2_Result_1 = "우";
                    }
                    if (jo.SelectToken("fd2").ToString().Equals("1"))
                    {
                        Game2_Result_2 = "삼";
                    }
                    else
                    {
                        Game2_Result_2 = "사";
                    }
                    if (jo.SelectToken("fd3").ToString().Equals("1"))
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
                else
                {
                    return false;
                }
            }
            catch (Exception _ex)
            {
                txtLogAdd(txtLog2, _ex.ToString(), Color.Black);
                logger.Error(_ex.ToString());
                return false;
            }
        }



        private Boolean loadUpDownResultGame(string gamecode, int gameInning)
        {
            try
            {
                // https://updown2.com/api/last?g_type=coinladder3&_=1663507276707
                // {"error":false,"msg":"성공","g_date":"2022-09-18","date_round":448,"lr":"좌","line":"3","oe":"짝"}
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("https://updown2.com/api/last?g_type={0}", gamecode == "CSA3" ? "coinladder3" : "coinladder5");
                Uri contoso = new Uri(stringBuilder.ToString());
                string returnVal = UtilModel.GetHttp(contoso, "GET", null);
                logger.Info(stringBuilder.ToString());
                logger.Info(returnVal);

                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
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
                logger.Error(_ex.ToString());
                return false;
            }
        }
        private Boolean loadRoungInformation1(string gamecode)
        {
            try
            {
                /****************************
                 * 
                 * ] {"code":1,"comment":"","more_info":{"dateIdx":20220916,"roundNo_1":325,"dayroundNo_1":325,"leftTime":158,"end":1663312500,"end_sec":17}}
                 * 
                 */
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}:8085/auto/api/round_timing?gm={1}&u={2}&_={3}", UtilModel.UserSiteUrlAddress, gamecode, UtilModel.UserId, DateTime.UtcNow.Ticks.ToString());
                string returnMessage;
                var rm = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                returnMessage = rm.Result;
                logger.Info(stringBuilder.ToString());
                logger.Info(returnMessage);

                if (returnMessage.Contains("code"))
                {
                    //txtLogAdd(txtLog1, returnMessage, Color.Black);
                    JObject jo = JObject.Parse(returnMessage);
                    int ret_code = int.Parse(jo.SelectToken("code").ToString());
                    if (ret_code == 1)
                    {
                        if (gamecode.Equals("EPB") || gamecode.Equals("EPB3") || gamecode.Equals("DSCP3") || gamecode.Equals("DSCP5") || gamecode.Equals("CSA3") || gamecode.Equals("CSA5"))
                        {
                            todayDate = jo.SelectToken("more_info").SelectToken("dateIdx").ToString();
                            BetEndSec_1 = int.Parse(jo.SelectToken("more_info").SelectToken("end_sec").ToString());
                            roundNo_1 = double.Parse(jo.SelectToken("more_info").SelectToken("roundNo").ToString());
                            dayroundNo_1 = int.Parse(jo.SelectToken("more_info").SelectToken("dayRoundNo").ToString());
                            BetRemainingTime_1 = int.Parse(jo.SelectToken("more_info").SelectToken("leftTime").ToString());
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
                return false;
            }
        }
        private Boolean loadRoungInformation2(string gamecode)
        {
            try
            {
                /****************************
                 * 
                 * ] {"code":1,"comment":"","more_info":{"dateIdx":20220916,"roundNo":325,"dayRoundNo":325,"leftTime":158,"end":1663312500,"end_sec":17}}
                 * 
                 */
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}:8085/auto/api/round_timing?gm={1}&u={2}&_={3}", UtilModel.UserSiteUrlAddress, gamecode, UtilModel.UserId, DateTime.UtcNow.Ticks.ToString());
                string returnMessage;
                var rm = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                returnMessage = rm.Result;
                logger.Info(stringBuilder.ToString());
                logger.Info(returnMessage);

                if (returnMessage.Contains("code"))
                {
                    //txtLogAdd(txtLog1, returnMessage, Color.Black);
                    JObject jo = JObject.Parse(returnMessage);
                    int ret_code = int.Parse(jo.SelectToken("code").ToString());
                    if (ret_code == 1)
                    {
                        if (gamecode.Equals("EPB") || gamecode.Equals("EPB3") || gamecode.Equals("DSCP3") || gamecode.Equals("DSCP5") || gamecode.Equals("CSA3") || gamecode.Equals("CSA5"))
                        {
                            todayDate = jo.SelectToken("more_info").SelectToken("dateIdx").ToString();
                            BetEndSec_2 = int.Parse(jo.SelectToken("more_info").SelectToken("end_sec").ToString());
                            roundNo_2 = double.Parse(jo.SelectToken("more_info").SelectToken("roundNo").ToString());
                            dayroundNo_2 = int.Parse(jo.SelectToken("more_info").SelectToken("dayRoundNo").ToString());
                            BetRemainingTime_2 = int.Parse(jo.SelectToken("more_info").SelectToken("leftTime").ToString());
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
                return false;
            }
        }

        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(remainingTimer_Tick));
        }

        private int callResultInning(String GCode)
        {
            if (dayroundNo_1 == 1 || dayroundNo_2 == 1)
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
                } else
                {
                    return 288;
                }
            }
            else
            {
                // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                if (GCode.Equals("EPB") || GCode.Equals("DSCP5") || GCode.Equals("EPB3") || GCode.Equals("DSCP3") || GCode.Equals("HSP3") || GCode.Equals("HSP5") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5"))
                {
                    return (dayroundNo_1 - 1);
                }
                else if (GCode.Equals("CSA3") || GCode.Equals("CSA5") || GCode.Equals("HSPSA3") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5"))
                {
                    return (dayroundNo_2 - 1);
                }
                else
                {
                    return 288;
                }
            }
        }
        /* 1초마다 체크할 사항
        **************************************
        가장 중요한 부분이다.
        **************************************
        */
        private void remainingTimer_Tick()
        {
            /***************************************
            첫번째 페이지 게임 진행 : 파워볼 게임 종류
            ***************************************/
            if (StartGame_1)
            {
                TimeSpan time = DateTime.Now.TimeOfDay;
                TimeSpan diff = DateTime.Now - StartDateTime_1;
                label6.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                if (BetRemainingTime_1 > 0)
                {
                    setTimeRemaining(BetRemainTime_1, BetRemainingTime_1--);
                    if ((BetRemainingTime_1 % 4 == 0) && (BetRemainingTime_1 > (GameTime[GameNumber_1] - 10)))
                    {
                        /*********************게임 회차 설정*********************************/
                        if (GameCode_1.Equals("EPB"))
                        {
                            dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else if (GameCode_1.Contains("2"))
                        {
                            dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        else if (GameCode_1.Contains("3"))
                        {
                            dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else if (GameCode_1.Contains("5"))
                        {
                            dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else
                        {
                            dayroundNo_1 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }

                        Game1_CruiseBetGroupBox.Text = "[ " + dayroundNo_1 + "회 ] 크루즈 배팅 ]";
                    }
                    // 게임 1의 결과값을 불러온다.
                    if (!Game_1_Result_Load_Complete && (BetRemainingTime_1 % 7 == 0) && (BetRemainingTime_1 < (GameTime[GameNumber_1] - 20)) && (BetRemainingTime_1 > BetEndSec_1))
                    {
                        Game_1_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_1_Betting_Complete_Status && Game_1_Result_Load_Complete && !Game_1_Check_Complete && (BetRemainingTime_1 % 15 == 0) && (BetRemainingTime_1 > BetEndSec_1))
                    {
                        Game_1_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_1_Betting_Complete_Status && (BetRemainingTime_1 % 8 == 0) && (BetRemainingTime_1 < BetEndSec_1 + 60) && (BetRemainingTime_1 > BetEndSec_1))
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
                            if (Game_Bet_Processing_Final(Game1_BetPick, Game1_BetMoney, roundNo_1, BetRemainingTime_1, txtLog1, BetEndSec_1, GameCode_1))
                            {
                                Game1_Bet_Processing_RegistListView();
                                Game_1_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_1_Betting_Complete_Status && BetRemainingTime_1 < BetEndSec_1)
                    {
                        Game1_Bet_Processing_RegistListView();
                        Game_1_Betting_Complete_Status = true;
                    }
                }
                else
                {
                    Game1_Betting_Mode_Result_Process = false;
                    Game_1_Result_Load_Complete = false;
                    Game_1_Check_Complete = false;
                    Game_1_Betting_Complete_Status = false;
                    if (String.IsNullOrEmpty(GameCode_1))
                    {
                        txtLogAdd(txtLog1, "게임이 선택되지 않았습니다.", Color.Red);
                        return;
                    }

                    /*********************2,3,5분 게임 시간 설정*********************************/
                    if (GameCode_1.Equals("EPB"))
                    {
                        BetRemainingTime_1 = 300;
                    }
                    else if (GameCode_1.Contains("2"))
                    {
                        BetRemainingTime_1 = 120;
                    }
                    else if (GameCode_1.Contains("3"))
                    {
                        BetRemainingTime_1 = 180;
                    }
                    else if (GameCode_1.Contains("5"))
                    {
                        BetRemainingTime_1 = 300;
                    }
                    else
                    {
                        BetRemainingTime_1 = 300;
                    }
                }
            }

            /***************************************
            두번째 페이지 게임 진행 : 사다리 게임 종류
            ***************************************/
            if (StartGame_2)
            {
                TimeSpan time = DateTime.Now.TimeOfDay;
                TimeSpan diff = DateTime.Now - StartDateTime_2;
                label10.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);

                if (BetRemainingTime_2 > 0)
                {
                    setTimeRemaining(BetRemainTime_2, BetRemainingTime_2--);
                    if ((BetRemainingTime_2 % 4 == 0) && (BetRemainingTime_2 > (GameTime[GameNumber_2] - 10)))
                    {
                        /*********************게임 회차 설정*********************************/
                        if (GameCode_2.Equals("EPB"))
                        {
                            dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else if (GameCode_2.Contains("2"))
                        {
                            dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        else if (GameCode_2.Contains("3"))
                        {
                            dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else if (GameCode_2.Contains("5"))
                        {
                            dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else
                        {
                            dayroundNo_2 = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }

                        Game2_CruiseBetGroupBox.Text = "[ " + dayroundNo_2 + "회 ] 크루즈 배팅 ]";
                    }
                    // 게임 2의 결과값을 불러온다.
                    if (!Game_2_Result_Load_Complete && (BetRemainingTime_2 % 4 == 0) && (BetRemainingTime_2 < (GameTime[GameNumber_2] - 20)) && (BetRemainingTime_2 > BetEndSec_2))
                    {
                        Game_2_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_2_Betting_Complete_Status && Game_2_Result_Load_Complete && !Game_2_Check_Complete && (BetRemainingTime_2 % 5 == 0) && (BetRemainingTime_2 > BetEndSec_2))
                    {
                        Game_2_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_2_Betting_Complete_Status && (BetRemainingTime_2 % 6 == 0) && (BetRemainingTime_2 < BetEndSec_2 + 60) && (BetRemainingTime_2 > BetEndSec_2))
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
                            if (Game_Bet_Processing_Final(Game2_BetPick, Game2_BetMoney, roundNo_2, BetRemainingTime_2, txtLog2, BetEndSec_2, GameCode_2))
                            {
                                Game2_Bet_Processing_RegistListView();
                                Game_2_Betting_Complete_Status = true;
                            }
                        }
                    }

                    if (!Game_2_Betting_Complete_Status && BetRemainingTime_2 < BetEndSec_2)
                    {
                        Game2_Bet_Processing_RegistListView();
                        Game_2_Betting_Complete_Status = true;
                    }
                }
                else
                {
                    Game2_Betting_Mode_Result_Process = false;
                    Game_2_Result_Load_Complete = false;
                    Game_2_Check_Complete = false;
                    Game_2_Betting_Complete_Status = false;
                    if (String.IsNullOrEmpty(GameCode_2))
                    {
                        txtLogAdd(txtLog2, "게임이 선택되지 않았습니다.", Color.Red);
                        return;
                    }

                    /*********************2,3,5분 게임 시간 설정*********************************/
                    if (GameCode_1.Equals("EPB"))
                    {
                        BetRemainingTime_2 = 300;
                    }
                    else if (GameCode_1.Contains("2"))
                    {
                        BetRemainingTime_2 = 120;
                    }
                    else if (GameCode_1.Contains("3"))
                    {
                        BetRemainingTime_2 = 180;
                    }
                    else if (GameCode_1.Contains("5"))
                    {
                        BetRemainingTime_2 = 300;
                    }
                    else
                    {
                        BetRemainingTime_2 = 300;
                    }
                }
            }
        }

        private void Game_1_PatternCheck()
        {
            loadRoungInformation1(GameCode_1);
            checkGameCheck(Game1_PowerBallOddEvenUseCheck, "Game1_CruisePowerBallOddEvenBetListLevel", "Game1_CruisePowerBallOddEvenBetPickLevel", Game1_CruiseBetPowerBallOddEvenSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 3);
            checkGameCheck(Game1_PowerBallUnderOverUseCheck, "Game1_CruisePowerBallUnderOverBetListLevel", "Game1_CruisePowerBallUnderOverBetPickLevel", Game1_CruiseBetPowerBallUnderOverSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 7);
            checkGameCheck(Game1_NormalBallOddEvenUseCheck, "Game1_CruiseNormalBallOddEvenBetListLevel", "Game1_CruiseNormalBallOddEvenBetPickLevel", Game1_CruiseBetNormalBallOddEvenSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 11);
            checkGameCheck(Game1_NormalBallUnderOverUseCheck, "Game1_CruiseNormalBallUnderOverBetListLevel", "Game1_CruiseNormalBallUnderOverBetPickLevel", Game1_CruiseBetNormalBallUnderOverSubLevel, Game1_CruiseBetRegistListView, GameCode_1, 15);

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
            loadRoungInformation2(GameCode_2);
            checkGameCheck(Game2_FirstUseCheck, "Game2_FirstBetListLevel", "Game2_FirstBetPickLevel", Game2_FirstBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 3);
            checkGameCheck(Game2_SecondUseCheck, "Game2_SecondBetListLevel", "Game2_SecondBetPickLevel", Game2_SecondBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 7);
            checkGameCheck(Game2_ThirdUseCheck, "Game2_ThirdBetListLevel", "Game2_ThirdBetPickLevel", Game2_ThirdBetSubLevel, Game2_CruiseBetRegistListView, GameCode_2, 11);

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

        private void Game_1_Result_Load()
        {
            if (loadAPISiteResultGame(GameCode_1, callResultInning(GameCode_1)))
            {
                Game_1_Result_Load_Complete = true;
                txtLogAdd(txtLog1, "결과값을 불러오는데 성공하였습니다.", Color.Red);
                if (!Game1_Betting_Mode_Result_Process)
                {
                    Game1_Betting_Mode_Result_Processing();
                }
                for (int i = 0; i < Game1_CruiseBetRegistListView.Items.Count; i++)
                {
                    ListViewItem item = Game1_CruiseBetRegistListView.Items[i];

                    bool isContains = item.SubItems[1].Text.Equals((callResultInning(GameCode_1)).ToString());

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

                        item.SubItems[3].Text = Game1_Result_1;
                        item.SubItems[7].Text = Game1_Result_2;
                        item.SubItems[11].Text = Game1_Result_3;
                        item.SubItems[15].Text = Game1_Result_4;

                        int.TryParse(Regex.Replace(BetMoney_1, @"\D", ""), out int Out_BetMoney_1);
                        int.TryParse(Regex.Replace(BetMoney_2, @"\D", ""), out int Out_BetMoney_2);
                        int.TryParse(Regex.Replace(BetMoney_3, @"\D", ""), out int Out_BetMoney_3);
                        int.TryParse(Regex.Replace(BetMoney_4, @"\D", ""), out int Out_BetMoney_4);

                        //Game1_CruiseBetRegistListView.EnsureVisible(Game1_CruiseBetRegistListView.Items.Count - 1);

                        /*********** 파워볼 홀짝 당첨 여부 **************/
                        if (Game1_Result_1.Equals(Pick_1))
                        {
                            if (Game1_Result_1.Equals("홀"))
                            {
                                item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                                item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (Game1_Result_1.Equals("짝"))
                            {
                                item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
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
                                item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[2].Text.Equals("짝"))
                            {
                                item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            }
                            if (item.SubItems[3].Text.Equals("홀"))
                            {
                                item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[3].Text.Equals("짝"))
                            {
                                item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            }
                            item.SubItems[5].Text = "0";
                            item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                        }
                        /*********** 파워볼 언오버 당첨 여부 **************/
                        if (Game1_Result_2.Equals(Pick_2))
                        {
                            if (Game1_Result_2.Equals("언"))
                            {
                                item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                                item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (Game1_Result_2.Equals("오"))
                            {
                                item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
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
                                item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[6].Text.Equals("오"))
                            {
                                item.SubItems[6].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            }
                            if (item.SubItems[7].Text.Equals("언"))
                            {
                                item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[7].Text.Equals("오"))
                            {
                                item.SubItems[7].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            }
                            item.SubItems[9].Text = "0";
                            item.SubItems[9].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                        }
                        /*********** 일반볼 홀짝 당첨 여부 **************/
                        if (Game1_Result_3.Equals(Pick_3))
                        {
                            if (Game1_Result_3.Equals("홀"))
                            {
                                item.SubItems[10].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                                item.SubItems[11].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (Game1_Result_3.Equals("짝"))
                            {
                                item.SubItems[10].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                item.SubItems[11].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
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
                            item.SubItems[13].Text = "0";
                            item.SubItems[13].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                        }
                        /*********** 일반볼 언오버 당첨 여부 **************/
                        if (Game1_Result_4.Equals(Pick_4))
                        {
                            if (Game1_Result_4.Equals("언"))
                            {
                                item.SubItems[14].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                                item.SubItems[15].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (Game1_Result_4.Equals("오"))
                            {
                                item.SubItems[14].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                                item.SubItems[15].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
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
                                item.SubItems[14].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[14].Text.Equals("오"))
                            {
                                item.SubItems[14].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                            }
                            if (item.SubItems[15].Text.Equals("언"))
                            {
                                item.SubItems[15].ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                            }
                            else if (item.SubItems[15].Text.Equals("오"))
                            {
                                item.SubItems[15].ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
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

                SetRowBackgroundColor(Game1_CruiseBetRegistListView, Color.LightGray, Color.White);

                int Game1_WinMoney_Out_Value;
                int.TryParse(Regex.Replace(Game1_WinMoney.Text, @"\D", ""), out Game1_WinMoney_Out_Value);

                int Game1_StackMoney_Out_Value;
                int.TryParse(Regex.Replace(Game1_StackMoney.Text, @"\D", ""), out Game1_StackMoney_Out_Value);

                Game1_ProfitMoney.Text = UtilModel.StringFormatChanged(Game1_WinMoney_Out_Value - Game1_StackMoney_Out_Value);
            }
            else
            {
                txtLogAdd(txtLog1, "결과값을 불러오는데 실패하였습니다.", Color.Red);
            }
        }

        private void Game_2_Result_Load()
        {
            /*
            if (loadAPISiteResultGame(GameCode_2, callResultInning(GameCode_2)))
            {
                Game_2_Result_Processiong();
                Game_2_Result_Load_Complete = true;
                txtLogAdd(txtLog2, "결과값을 불러오는데 성공하였습니다.", Color.Red);
            }
            else
            {
            */

            if (loadUpDownResultGame(GameCode_2, callResultInning(GameCode_2)))
            {
                //https://updown2.com/api/last?g_type=coinladder5
                Game_2_Result_Processiong();
                Game_2_Result_Load_Complete = true;
                txtLogAdd(txtLog2, "[업다운] 결과값을 불러오는데 성공하였습니다.", Color.Red);
                return;
            }
            if (loadBepickResultGame(GameCode_2, callResultInning(GameCode_2)))
            {
                // https://bepick.net/live/result/coinladder3?_=325233
                Game_2_Result_Processiong();
                Game_2_Result_Load_Complete = true;
                txtLogAdd(txtLog2, "[베픽] 결과값을 불러오는데 성공하였습니다.", Color.Red);
                return;
            }
            txtLogAdd(txtLog2, "[Game2] 결과값을 불러오는데 실패하였습니다.", Color.Red);
        }

        private void Game_2_Result_Processiong()
        {

            if (!Game2_Betting_Mode_Result_Process)
            {
                Game2_Betting_Mode_Result_Processing();
            }
            for (int i = 0; i < Game2_CruiseBetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game2_CruiseBetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals((callResultInning(GameCode_2)).ToString());

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

            SetRowBackgroundColor(Game2_CruiseBetRegistListView, Color.LightGray, Color.White);

            int Game2_WinMoney_Out_Value;
            int.TryParse(Regex.Replace(Game2_WinMoney.Text, @"\D", ""), out Game2_WinMoney_Out_Value);

            int Game2_StackMoney_Out_Value;
            int.TryParse(Regex.Replace(Game2_StackMoney.Text, @"\D", ""), out Game2_StackMoney_Out_Value);

            Game2_ProfitMoney.Text = UtilModel.StringFormatChanged(Game2_WinMoney_Out_Value - Game2_StackMoney_Out_Value);
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
                            if (int.Parse(Game1_CruisePowerBallOddEvenLevelChange.Text) >= 45)
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

                            if (Game1_WinToStopCheckBox.Checked)
                            {
                                Game1_PowerBallOddEvenUseCheck.Checked = false;
                                txtLogAdd(txtLog1, "당첨 후 정지 기능으로 [" + groupBox9.Text + "] 게임이 종료되었습니다.", Color.Black);
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
                            if (int.Parse(Game1_CruisePowerBallUnderOverLevelChange.Text) >= 45)
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

                            if (Game1_WinToStopCheckBox.Checked)
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
                            if (int.Parse(Game1_CruiseNormalBallOddEvenLevelChange.Text) >= 45)
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

                            if (Game1_WinToStopCheckBox.Checked)
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
                            if (int.Parse(Game1_CruiseNormalBallUnderOverLevelChange.Text) >= 45)
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

                            if (Game1_WinToStopCheckBox.Checked)
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
                            if (int.Parse(Game2_FirstLevelChange.Text) >= 45)
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
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox8.Text + "] 게임이 종료되었습니다.", Color.Black);
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
                            if (int.Parse(Game2_SecondLevelChange.Text) >= 45)
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
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox7.Text + "] 게임이 종료되었습니다.", Color.Black);
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
                            if (int.Parse(Game2_ThirdLevelChange.Text) >= 45)
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
                                txtLogAdd(txtLog1, "클리어 후 정지 기능으로 [" + groupBox6.Text + "] 게임이 종료되었습니다.", Color.Black);
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

        //checkGameCheck(Game1_PowerBallOddEvenUseCheck, "Game1_CruisePowerBallOddEvenBetListLevel", "Game1_CruisePowerBallOddEvenBetPickLevel", Game1_CruiseBetPowerBallOddEvenSubLevel, Game1_CruiseBetRegistListView, GameCode_1);
        private void checkGameCheck(CheckBox GameUseCheck, String BetListString, String BetPickString, int subLevel, ListView RegistListView, String GameCode, int SubItemNum)
        {
            if (!GameUseCheck.Checked)
            {
                return;
            }
            TextBox BetListLevel = Controls.Find(BetListString + subLevel, true)[0] as TextBox;
            TextBox BetPickLevel = Controls.Find(BetPickString + subLevel, true)[0] as TextBox;
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
                    break;
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
        }
        public void setTimeRemaining(Button _button, double _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            _button.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }

        /**********************사용자의 현재 보유금을 불러온다.*************************/
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
                    _RemainingTimer.Stop();
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

        // <summary>
        // 수정 삭제하기
        // </summary>
        /***************************************************************
        private void XMLModifier(String settingNum)
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("settings.xml");

                // 첫노드를 잡아주고 하위 노드를 서냍ㄱ한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode(settingNum);

                // 하위 노드 특성에 날짜를 입력하기를 원할때 (추가를 원할때)
                //SubNode.SetAttribute("DATA", DateTime.Today.ToString());

                // 하위 노드를 추가, 삭제, 수정하고 싶을때 (Book(보다 하위)
                // 아래 두줄은 삭제할때나 수정할때 사용하면 된다.
                XmlNode DeleteNode;

                for (int i = 1; i <= 15; i++)
                {
                    DeleteNode = SubNode.SelectSingleNode("levelmoney" + i);
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);

                        TextBox _msl = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                        int outValue = 0;
                        bool _b = int.TryParse(Regex.Replace(_msl.Text, @"\D", ""), out outValue);
                        if (_b)
                        {
                            SubNode.AppendChild(CreateNode(XmlDoc, "levelmoney" + i, _msl.Text));
                        }
                    }
                }

                for (int i = 2; i <= 15; i++)
                {
                    DeleteNode = SubNode.SelectSingleNode("magnification" + i);
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);

                        ComboBox _mag = (Controls.Find("magnification" + i.ToString(), true)[0] as ComboBox);
                        SubNode.AppendChild(CreateNode(XmlDoc, "magnification" + i, _mag.Text));
                    }
                }
                //magnification2
                // 아래 한 줄은 추가, 수정할때 사용한다.


                // 위에 했던 행위들을 바꿔준다.
                // ReplaceChild(SubNode, SubNode); 에서 ()안에 앞에 노드는 변경할 노드 뒤에 노드는 변경당할 노드를 적어준다.
                //Firstnode.ReplaceChild(SubNode, SubNode);

                XmlDoc.Save("settings.xml");
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        private void XMLLoad(String settingNum)
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

        System.Timers.Timer _RemainingTimer = new System.Timers.Timer();

        string todayDate = null;
        private readonly string[] GameCode = new string[] { "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5" };
        private readonly int[] GameTime = new int[] { 180, 300, 180, 300, 180, 300, 180, 300, 180, 300, 180, 300, 180, 300, 120, 300 };

        string Game1_Result_1 = string.Empty;
        string Game1_Result_2 = string.Empty;
        string Game1_Result_3 = string.Empty;
        string Game1_Result_4 = string.Empty;

        string Game2_Result_1 = string.Empty;
        string Game2_Result_2 = string.Empty;
        string Game2_Result_3 = string.Empty;

        int Game1_CruiseBetPowerBallOddEvenSubLevel = 1;
        int Game1_CruiseBetPowerBallUnderOverSubLevel = 1;
        int Game1_CruiseBetNormalBallOddEvenSubLevel = 1;
        int Game1_CruiseBetNormalBallUnderOverSubLevel = 1;

        int Game2_FirstBetSubLevel = 1;
        int Game2_SecondBetSubLevel = 1;
        int Game2_ThirdBetSubLevel = 1;

        private int GameNumber_1 = 0;
        private int GameNumber_2 = 0;

        int BetRemainingTime_1 = 0;
        int BetRemainingTime_2 = 0;

        int BetEndSec_1 = 0;
        int BetEndSec_2 = 0;

        string GameCode_1 = null;
        string GameCode_2 = null;

        DateTime StartDateTime_1;
        DateTime StartDateTime_2;

        Boolean StartGame_1 = false;
        Boolean StartGame_2 = false;

        Boolean Game1_Betting_Mode_Result_Process = false;
        Boolean Game2_Betting_Mode_Result_Process = false;


        /************1페이지 게임 배팅 등록 상태**********/
        Boolean Game_1_Betting_Complete_Status = false;
        /************2페이지 게임 배팅 등록 상태**********/
        Boolean Game_2_Betting_Complete_Status = false;


        /************게임 1의 결과 값 로드 여부*********/
        Boolean Game_1_Result_Load_Complete = false;
        /************게임 2의 결과 값 로드 여부*********/
        Boolean Game_2_Result_Load_Complete = false;

        /************게임 1의 배팅 점검 여부*********/
        Boolean Game_1_Check_Complete = false;
        /************게임 2의 배팅 점검 여부*********/
        Boolean Game_2_Check_Complete = false;

        private int[] Game1_BetMoney = new int[] { 0, 0, 0, 0 };
        private int[] Game2_BetMoney = new int[] { 0, 0, 0, 0 };

        private string[] Game1_BetPick = new string[] { null, null, null, null };
        private string[] Game2_BetPick = new string[] { null, null, null, null };

        Double roundNo_1 = 0;
        Double roundNo_2 = 0;

        int dayroundNo_1 = 0;
        int dayroundNo_2 = 0;

        double[,] Game_1_CruiseAllBetMoney = new double[100, 3];
        double[,] Game_2_CruiseAllBetMoney = new double[100, 3];

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

        private void game1_Pattern_1222_1_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = "홀짝짝짝-홀|짝홀홀홀-짝";

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = "언오오-오|오언언-언";
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = "언오오오-언|오언언언-오";

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = "홀짝짝짝-홀|짝홀홀홀-짝";

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = "언오오-오|오언언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = "언오오오-언|오언언언-오";
        }

        private void game1_Pattern_1222_2_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = "홀짝짝짝-짝|짝홀홀홀-홀";

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = "언오오-오|오언언-언";
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = "언오오오-오|오언언언-언";

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = "홀짝짝-짝|짝홀홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = "홀짝짝짝-짝|짝홀홀홀-홀";

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = "언오오-오|오언언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = "언오오오-오|오언언언-언";
        }

        private void game1_Pattern_1221_1_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = "홀짝짝홀-홀|짝홀홀짝-짝";

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = "언오오-언|오언언-오";
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = "언오오언-언|오언언오-오";

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = "홀짝짝홀-홀|짝홀홀짝-짝";

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = "언오오-언|오언언-오";
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = "언오오언-언|오언언오-오";
        }

        private void game1_Pattern_1221_2_CheckedChanged(object sender, EventArgs e)
        {
            Game1_CruisePowerBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruisePowerBallOddEvenBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game1_CruisePowerBallOddEvenBetListLevel3.Text = "홀짝짝홀-짝|짝홀홀짝-홀";

            Game1_CruisePowerBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruisePowerBallUnderOverBetListLevel2.Text = "언오오-언|오언언-오";
            Game1_CruisePowerBallUnderOverBetListLevel3.Text = "언오오언-오|오언언오-언";

            Game1_CruiseNormalBallOddEvenBetListLevel1.Text = "홀짝-짝|짝홀-홀";
            Game1_CruiseNormalBallOddEvenBetListLevel2.Text = "홀짝짝-홀|짝홀홀-짝";
            Game1_CruiseNormalBallOddEvenBetListLevel3.Text = "홀짝짝홀-짝|짝홀홀짝-홀";

            Game1_CruiseNormalBallUnderOverBetListLevel1.Text = "언오-오|오언-언";
            Game1_CruiseNormalBallUnderOverBetListLevel2.Text = "언오오-언|오언언-오";
            Game1_CruiseNormalBallUnderOverBetListLevel3.Text = "언오오언-오|오언언오-언";
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
