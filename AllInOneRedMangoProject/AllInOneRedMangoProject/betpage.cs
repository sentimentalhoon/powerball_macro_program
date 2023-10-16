using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AllInOneRedMangoProject
{
    public partial class betpage : UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public betpage()
        {
            InitializeComponent();
        }

        private void Game_Select_Game_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string[] sarray = comboBox.Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Regex.Replace(sarray[0], @"\D", ""), out int outValue);
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                GameCode = UtilModel.Click_GameCodePowerBall[outValue - 1];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                GameCode = UtilModel.Life_GameCodePowerBall[outValue - 1];
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                GameCode = UtilModel.Royal_GameCodePowerBall[outValue - 1];
            }
        }
        private void betpage_Load(object sender, EventArgs e)
        {
            Game_BetResultListView.DoubleBuffered(true);
            Game_Clear_Level_50.DoubleBuffered(true);
            Game_1_CheckBox.Checked = true;
            Game_2_CheckBox.Checked = true;
            Game_3_CheckBox.Checked = true;
            Game_4_CheckBox.Checked = true;
            Game_5_CheckBox.Checked = true;
            Game_6_CheckBox.Checked = true;
            Game_7_CheckBox.Checked = true;
            Game_8_CheckBox.Checked = true;

            Game_RemainingTimer.Interval = 1000;
            Game_RemainingTimer.Elapsed += new System.Timers.ElapsedEventHandler(Game_Timer_Elapsed);
        }

        delegate void TimerEventFiredDelegate();
        void Game_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Game_Timer_Tick));
        }

        private void Game_Timer_Tick()
        {
            // txtLog1. txtLog2 삭제
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 3600 == 0)
            {
                txtLog.Clear();
            }
            if ((int)DateTime.Now.TimeOfDay.TotalSeconds % 60 == 0)
            {
                try
                {
                    JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                    JToken jTokenServers = jObject["Servers"];

                    if (jTokenServers == null || jTokenServers.Type == JTokenType.Null)
                    {
                        txtLogAdd(txtLog, "정보를 읽어오는데 실패", Color.Black);
                    }
                    else
                    {
                        UtilModel.ResultServersNtry = jObject.SelectToken("ResultServers.ntry").Value<bool>();
                        UtilModel.ResultServersUpdown = jObject.SelectToken("ResultServers.updown").Value<bool>();
                        UtilModel.ResultServersBepick = jObject.SelectToken("ResultServers.bepick").Value<bool>();
                        UtilModel.ResultServersApiSite = jObject.SelectToken("ResultServers.apisite").Value<bool>();
                        UtilModel.ResultServersMarukhan = jObject.SelectToken("ResultServers.marukhan").Value<bool>();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            gameTick();
        }
        private void gameTick()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            int totalSeconds = (int)time.TotalSeconds;
            if (StartGame)
            {
                TimeSpan diff = DateTime.Now - StartDateTime;
                Game_PastTimer.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);
                Game_Start_Button.Text = "[" + roundNo + "] 회 배팅이 진행 중입니다.";
                if (Game_WinToStopCheckBox.Checked
                        && !Game_1_CheckBox.Checked
                        && !Game_2_CheckBox.Checked
                        && !Game_3_CheckBox.Checked
                        && !Game_4_CheckBox.Checked
                        && !Game_5_CheckBox.Checked
                        && !Game_6_CheckBox.Checked
                        && !Game_7_CheckBox.Checked
                        && !Game_7_CheckBox.Checked)
                {
                    GameStop();
                }
                if (string.IsNullOrEmpty(GameCode))
                {
                    txtLogAdd(txtLog, "게임이 선택되지 않았습니다.", Color.Red);
                    GameStop();
                    return;
                }
                else
                {
                    if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
                    {
                        // 코인파워볼 3분
                        if (GameCode.Contains("DSCP3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(time.TotalSeconds / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode.Contains("DSCP5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(time.TotalSeconds / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode.Contains("EPB3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(time.TotalSeconds / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode.Contains("EPB"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(time.TotalSeconds / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode.Contains("HSP3"))
                        {
                            BetRemainingTime = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode.Contains("HSP5"))
                        {
                            BetRemainingTime = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode.Contains("KLAY2"))
                        {
                            BetRemainingTime = 120 - totalSeconds % 120 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode.Contains("KLAY5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode.Contains("BEXB"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        } // 마루턴 3분
                        else if (GameCode.Contains("MPB"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else if (GameCode.Contains("SKPB"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else
                        {
                            BetRemainingTime = 10 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                    {
                        // 코인파워볼 3분
                        if (GameCode.Contains("DSCP3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode.Contains("DSCP5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode.Contains("EPB3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode.Contains("EPB"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode.Contains("HSP3"))
                        {
                            BetRemainingTime = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode.Contains("HSP5"))
                        {
                            BetRemainingTime = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode.Contains("KLAY2"))
                        {
                            BetRemainingTime = 120 - totalSeconds % 120 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode.Contains("KLAY5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode.Contains("BEXB"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime = 10 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 10) + 1;
                            return;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        // 코인파워볼 3분
                        if (GameCode.Contains("DSCB3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // 코인파워볼 5분
                        else if (GameCode.Contains("DSCB5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        else // EOS파워볼 3분
                        if (GameCode.Contains("EOSP3"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        // EOS파워볼 5분
                        else if (GameCode.Contains("EOSP5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }
                        // 하운슬롯 파워볼 3분
                        else if (GameCode.Contains("HSPB3"))
                        {
                            BetRemainingTime = 180 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 180) + 1;
                        }
                        // 하운슬롯 파워볼 5분
                        else if (GameCode.Contains("HSPB5"))
                        {
                            BetRemainingTime = 300 - (int)DateTime.UtcNow.TimeOfDay.TotalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor(DateTime.UtcNow.TimeOfDay.TotalSeconds / 300) + 1;
                        }
                        else // 클레이 파워볼 2분
                        if (GameCode.Contains("KLAY2"))
                        {
                            BetRemainingTime = 120 - totalSeconds % 120 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 120) + 1;
                        }
                        // 클레이 파워볼 5분
                        else if (GameCode.Contains("KLAY5"))
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                        }// 벡스 파워볼 3분
                        else if (GameCode.Contains("BEXB"))
                        {
                            BetRemainingTime = 180 - totalSeconds % 180 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 180) + 1;
                        }
                        else
                        {
                            BetRemainingTime = 300 - totalSeconds % 300 + 1;
                            roundNo = dayRoundNo = (int)Math.Floor((time.TotalSeconds) / 300) + 1;
                            return;
                        }
                    }

                    setTimeRemaining(BetRemainTime, BetRemainingTime);

                    if (BetRemainingTime > (betTime(GameCode) - 10))
                    {
                        Game_Betting_Mode_Result_Process = false;
                        Game_Result_Load_Complete = false;
                        Game_Check_Complete = false;
                        Game_Betting_Complete_Status = false;
                    }
                    // 게임 1의 결과값을 불러온다.
                    if (!Game_Result_Load_Complete && (BetRemainingTime % 3 == 0) && (BetRemainingTime < Game_Random_Result_Load_Time_For_PowerBall) && (BetRemainingTime > BetEndSec))
                    {
                        Game_Result[0] = Game_Result[1] = Game_Result[2] = Game_Result[3] = null;
                        Game_Result_Load();
                    }

                    // 배팅을 점검하여 패턴과 맞는지 확인한다.
                    if (!Game_Betting_Complete_Status && Game_Result_Load_Complete && !Game_Check_Complete && (BetRemainingTime > BetEndSec))
                    {
                        Game_PatternCheck();
                    }

                    // 배팅 마감 30초 전 배팅마감전까지 등록한다.
                    if (!Game_Betting_Complete_Status && Game_Result_Load_Complete && (BetRemainingTime % 5 == 0) && (BetRemainingTime < Game_Random_Bet_Regist_Time_For_PowerBall) && (BetRemainingTime > BetEndSec))
                    {
                        Game_BetMoney = new int[8];
                        Game_Bet_Processing_AllSum();

                        if (Game_Bet_Processing_Final())
                        {
                            Game_Bet_Processing_RegistListView();
                            Game_Betting_Complete_Status = true;
                        }
                    }

                    if (!Game_Betting_Complete_Status && BetRemainingTime < BetEndSec)
                    {
                        if (!Game_Result_Load_Complete)
                        {
                            txtLogAdd(txtLog, "[중단] 결과값을 읽어오지 못했습니다.", Color.Red);
                            GameStop();
                        }
                        else
                        {
                            int sum = Game_BetMoney[0] + Game_BetMoney[1] + Game_BetMoney[2] + Game_BetMoney[3] + Game_BetMoney[4] + Game_BetMoney[5] + Game_BetMoney[6] + Game_BetMoney[7];
                            if (sum > 0)
                            {
                                txtLogAdd(txtLog, "[중단] 배팅에 실패하였습니다.", Color.Red);
                                GameStop();
                            }
                            else
                            {
                                Game_BetMoney = new int[8];
                                Game_Bet_Processing_RegistListView();
                                Game_Betting_Complete_Status = true;
                            }
                        }
                    }
                }
            }
        }

        private Boolean Game_Bet_Processing_Final()
        {
            int All_Betting_Money = Game_BetMoney[0] + Game_BetMoney[1] + Game_BetMoney[2] + Game_BetMoney[3] + Game_BetMoney[4] + Game_BetMoney[5] + Game_BetMoney[6] + Game_BetMoney[7];
            if (All_Betting_Money >= 1)
            {
                StringBuilder Game_Url_Param_String = new StringBuilder();
                Game_Url_Param_String.Append(UtilModel.UserSiteUrlAddress);
                if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click || UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
                {
                    Game_Url_Param_String.AppendFormat(":{0}/api/bet?userid={1}&key={2}&gm={3}&tdate={4}&rno={5}", UtilModel.SitePort, UtilModel.UserId, UtilModel.Bet_Api_Key, GameCode, DateTime.Now.ToString("yyyyMMdd"), dayRoundNo.ToString());

                    if (Game_BetMoney[0] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp1={0}", Game_BetMoney[0]);
                    }
                    if (Game_BetMoney[1] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp2={0}", Game_BetMoney[1]);
                    }
                    if (Game_BetMoney[2] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp3={0}", Game_BetMoney[2]);
                    }
                    if (Game_BetMoney[3] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp4={0}", Game_BetMoney[3]);
                    }
                    if (Game_BetMoney[4] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp5={0}", Game_BetMoney[4]);
                    }
                    if (Game_BetMoney[5] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp6={0}", Game_BetMoney[5]);
                    }
                    if (Game_BetMoney[6] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp7={0}", Game_BetMoney[6]);
                    }
                    if (Game_BetMoney[7] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&pp8={0}", Game_BetMoney[7]);
                    }
                    Game_Url_Param_String.AppendFormat("&nonce={0}", Random_Nonce);
                }
                else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                {
                    Game_Url_Param_String.Append("/game/bet.asp");
                    Game_Url_Param_String.AppendFormat("?id={0}", UtilModel.UserId);
                    Game_Url_Param_String.AppendFormat("&key={0}", UtilModel.Bet_Api_Key);
                    Game_Url_Param_String.AppendFormat("&type={0}", GameCode + "_NM_PB");
                    Game_Url_Param_String.AppendFormat("&round={0}", dayRoundNo.ToString());

                    if (Game_BetMoney[0] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p1={0}", Game_BetMoney[0]);
                    }
                    if (Game_BetMoney[1] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p2={0}", Game_BetMoney[1]);
                    }
                    if (Game_BetMoney[2] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p3={0}", Game_BetMoney[2]);
                    }
                    if (Game_BetMoney[3] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p4={0}", Game_BetMoney[3]);
                    }
                    if (Game_BetMoney[4] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p5={0}", Game_BetMoney[4]);
                    }
                    if (Game_BetMoney[5] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p6={0}", Game_BetMoney[5]);
                    }
                    if (Game_BetMoney[6] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p7={0}", Game_BetMoney[6]);
                    }
                    if (Game_BetMoney[7] > 0)
                    {
                        Game_Url_Param_String.AppendFormat("&p8={0}", Game_BetMoney[7]);
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
                                txtLogAdd(txtLog, errorMessage, Color.OrangeRed);
                                if (CountResult > 40)
                                {
                                    break;
                                }
                                if (BetRemainingTime < BetEndSec)
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
                                txtLogAdd(txtLog, errorMessage, Color.OrangeRed);
                                logger.Info(UtilModel.UnicodeToString(returnMessage));
                                if (CountResult > 10)
                                {
                                    break;
                                }
                                if (BetRemainingTime < BetEndSec)
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
                        txtLogAdd(txtLog, errorMessage, Color.OrangeRed);
                        if (CountResult > 10)
                        {
                            break;
                        }
                        if (BetRemainingTime < BetEndSec)
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

                            (Controls.Find("lblTxtNowMoney", true)[0] as Label).Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);

                            txtLogAdd(txtLog, "[" + dayRoundNo + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                            logger.Info("[" + dayRoundNo + "] 정상 배팅 등록 완료.");
                            return true;
                        }
                        else if (ret_code < 0)
                        {
                            if (ret_code == -1009)
                            {
                                return true;
                            }
                            txtLogAdd(txtLog, "배팅 실패 : " + ret_code + " : " + ret_message, Color.OrangeRed);
                            logger.Info(dayRoundNo + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                            UtilModel.Delay(3000);
                            return false;
                        }
                        else
                        {
                            txtLogAdd(txtLog, "배팅 실패 : " + ret_code + " : " + ret_message, Color.OrangeRed);
                            logger.Info(dayRoundNo + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                            UtilModel.Delay(3000);
                            return false;
                        }
                    }
                    else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                    {
                        int ret_code = int.Parse(jo.SelectToken("err").ToString());
                        if (ret_code > 0)
                        {
                            // {"err":95,"msg":"베팅금액이 부족합니다. 먼저 충전하시기 바랍니다. "}
                            // {"err":70,"msg":"마감된 게임이 포함되었거나 선택하신 게임이 존재하지 않습니다."}
                            var ret_message = jo.SelectToken("msg").ToString();
                            txtLogAdd(txtLog, "배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message), Color.OrangeRed);
                            logger.Info(dayRoundNo + " : 배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message));
                            return false;
                        }
                        else
                        {
                            var ret_message = jo.SelectToken("msg").ToString();
                            if (ret_code == 0)
                            {
                                UtilModel.UserOwnMoney = int.Parse(jo.SelectToken("data.GameMoney").ToString());

                                (Controls.Find("lblTxtNowMoney", true)[0] as Label).Text = UtilModel.StringFormatChanged(UtilModel.UserOwnMoney);

                                txtLogAdd(txtLog, "[" + dayRoundNo + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                                logger.Info("[" + dayRoundNo + "] " + UtilModel.UnicodeToString(ret_message));
                                return true;
                            }
                            else
                            {
                                txtLogAdd(txtLog, "배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message), Color.OrangeRed);
                                logger.Info(dayRoundNo + " : 배팅 실패 : " + ret_code + " : " + UtilModel.UnicodeToString(ret_message));
                                UtilModel.Delay(5000);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception _ex)
                {
                    txtLogAdd(txtLog, _ex.ToString(), Color.OrangeRed);
                    logger.Info(_ex.ToString());
                }
            }
            return false;
        }
        private void Game_Result_Load()
        {
            if (GameCode.Equals("MPB") && UtilModel.ResultServersMarukhan)
            {
                if (loadMarukhanSiteResultGame(callResultInning(GameCode)))
                {
                    Game_Result_Processing();
                    Game_Result_Load_Complete = true;
                    txtLogAdd(txtLog, "[성공|마루턴] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersApiSite)
            {
                if (loadAPISiteResultGame(callResultInning(GameCode)))
                {
                    Game_Result_Processing();
                    Game_Result_Load_Complete = true;
                    txtLogAdd(txtLog, "[성공|API] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            if (UtilModel.ResultServersBepick)
            {
                if (loadBepickResultForPowerBallGame(callResultInning(GameCode)))
                {
                    Game_Result_Processing();
                    Game_Result_Load_Complete = true;
                    txtLogAdd(txtLog, "[성공|BP] 결과값을 불러오는데 성공", Color.Red);
                    return;
                }
            }
            /********************************************************************************************

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
            ********************************************************************************************/
            Game_Result_Load_Complete = false;
            txtLogAdd(txtLog, "[" + Game_Select_Game.Text + "]결과값을 불러오는데 실패", Color.Red);
        }

        private Boolean loadMarukhanSiteResultGame(int _dayRoundNo)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://marukhan.net/api/wheel/getResults/10?offset=0");

                string returnMessage;
                using (TimeoutWebClient webClient = new TimeoutWebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    returnMessage = webClient.DownloadString(stringBuilder.ToString());
                }
                logger.Info(stringBuilder.ToString());
                logger.Info(returnMessage);
                JArray jArray = JArray.Parse(returnMessage);

                if (jArray == null || jArray.Type == JTokenType.Null)
                {
                    txtLogAdd(txtLog, "[마루칸]정보를 읽어오는데 실패", Color.Black);
                }
                else
                {
                    foreach (JToken members in jArray.Children())
                    {
                        if (_dayRoundNo.ToString().Equals(members["round"].ToString()))
                        {
                            // 행운번호 합계
                            int.TryParse(members["num1"].ToString(), out int aniNum);
                            // 동물번호 합계
                            int.TryParse(members["num2"].ToString(), out int luckyNum);
                            if (luckyNum % 2 == 1)
                            {
                                Game_Result[0] = "홀";
                            }
                            else
                            {
                                Game_Result[0] = "짝";
                            }
                            if (luckyNum > 4)
                            {
                                Game_Result[1] = "오";
                            }
                            else
                            {
                                Game_Result[1] = "언";
                            }
                            if (aniNum % 2 == 1)
                            {
                                Game_Result[2] = "홀";
                            }
                            else
                            {
                                Game_Result[2] = "짝";
                            }
                            if (aniNum > 6)
                            {
                                Game_Result[3] = "오";
                            }
                            else
                            {
                                Game_Result[3] = "언";
                            }
                            txtLogAdd(txtLog, "[" + (_dayRoundNo) + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
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

        private Boolean loadBepickResultForPowerBallGame(int _dayRoundNo)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("http://bepick.net/live/result/{0}", UtilModel.BepickGameCode(GameCode));
                string returnVal = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8").Result;
                logger.Info(returnVal);
                if (String.IsNullOrEmpty(returnVal))
                {
                    return false;
                }
                if (returnVal.Contains("fd1"))
                {
                    JObject jo = JObject.Parse(returnVal);
                    if (!jo.SelectToken("round").ToString().Equals(_dayRoundNo.ToString()))
                    {
                        logger.Error("회차 정보 다름 loadBepickResultForPowerBallGame data_gameInning : " + jo.SelectToken("round").ToString() + " || " + _dayRoundNo.ToString());
                        return false;
                    }
                    if (jo.SelectToken("fd1").ToString().Equals("1"))
                    {
                        Game_Result[0] = "홀";
                    }
                    else
                    {
                        Game_Result[0] = "짝";
                    }
                    if (jo.SelectToken("fd2").ToString().Equals("1"))
                    {
                        Game_Result[1] = "언";
                    }
                    else
                    {
                        Game_Result[1] = "오";
                    }
                    if (jo.SelectToken("fd3").ToString().Equals("1"))
                    {
                        Game_Result[2] = "홀";
                    }
                    else
                    {
                        Game_Result[2] = "짝";
                    }
                    if (jo.SelectToken("fd4").ToString().Equals("1"))
                    {
                        Game_Result[3] = "언";
                    }
                    else
                    {
                        Game_Result[3] = "오";
                    }
                    txtLogAdd(txtLog, "[" + _dayRoundNo + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
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
        private Boolean loadAPISiteResultGame(int _dayRoundNo)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
                {
                    string dateNow = DateTime.Now.ToString("yyMMdd");
                    if (GameCode.Equals("EOSP3") || GameCode.Equals("DSCB3") || GameCode.Equals("BEXB"))
                    {
                        if (_dayRoundNo == 480)
                        {
                            dateNow = DateTime.Now.AddDays(-1).ToString("yyMMdd");
                        }
                    }
                    else if (GameCode.Equals("HSPB3"))
                    {
                        dateNow = DateTime.UtcNow.ToString("yyMMdd");
                        if (_dayRoundNo == 480)
                        {
                            dateNow = DateTime.UtcNow.AddDays(-1).ToString("yyMMdd");
                        }
                    }
                    else if (GameCode.Equals("HSPB5"))
                    {
                        dateNow = DateTime.UtcNow.ToString("yyMMdd");
                        if (_dayRoundNo == 288)
                        {
                            dateNow = DateTime.UtcNow.AddDays(-1).ToString("yyMMdd");
                        }
                    }
                    else if (GameCode.Equals("DSCB5") || GameCode.Equals("EOSP5"))
                    {
                        if (_dayRoundNo == 288)
                        {
                            dateNow = DateTime.Now.AddDays(-1).ToString("yyMMdd");
                        }
                    }
                    stringBuilder.AppendFormat("{0}/game/result.asp?id={1}&key={2}&type={3}&round={4}", UtilModel.UserSiteUrlAddress, UtilModel.UserId, UtilModel.Bet_Api_Key, GameCode, dateNow.ToString() + string.Format("{0:D3}", _dayRoundNo));
                    logger.Info(stringBuilder.ToString());

                    string returnMessage;
                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        returnMessage = webClient.DownloadString(stringBuilder.ToString());
                    }
                    logger.Info(returnMessage);
                    if (returnMessage.Contains("err"))
                    {
                        JObject jo = JObject.Parse(returnMessage);
                        int ret_code = int.Parse(jo.SelectToken("err").ToString());
                        if (ret_code == 0)
                        {
                            string[] dataResult = jo.SelectToken("data.Result").ToString().Split(new char[] { '|' });
                            Game_Result[0] = dataResult[0];
                            Game_Result[1] = dataResult[1].Replace("더", "").Replace("버", "");
                            Game_Result[2] = dataResult[2];
                            Game_Result[3] = dataResult[3].Replace("더", "").Replace("버", "");
                            txtLogAdd(txtLog, "[" + (_dayRoundNo) + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
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
                else
                {
                    string dateNow = DateTime.Now.ToString("yyyyMMdd");
                    if (GameCode.Equals("EPB3") || GameCode.Equals("DSCP3") || GameCode.Equals("MPB"))
                    {
                        if (_dayRoundNo == 480)
                        {
                            dateNow = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                        }
                    }
                    else if (GameCode.Equals("EPB") || GameCode.Equals("DSCP5") || GameCode.Equals("SKPB"))
                    {
                        if (_dayRoundNo == 288)
                        {
                            dateNow = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                        }
                    }
                    stringBuilder.AppendFormat("{0}:{1}/auto/api/get_pushed_result?gm={2}&d={3}&r={4}&k={5}", UtilModel.UserSiteUrlAddress, UtilModel.SitePort, GameCode, dateNow, _dayRoundNo, UtilModel.Bet_Api_Key);

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
                            if (GameCode.Equals("EPB") || GameCode.Equals("EPB3") || GameCode.Equals("DSCP3") || GameCode.Equals("DSCP5") || GameCode.Equals("SKPB"))
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

                                    if (ballSum[5] % 2 == 1)
                                    {
                                        Game_Result[0] = "홀";
                                    }
                                    else
                                    {
                                        Game_Result[0] = "짝";
                                    }
                                    if (ballSum[5] > 4)
                                    {
                                        Game_Result[1] = "오";
                                    }
                                    else
                                    {
                                        Game_Result[1] = "언";
                                    }

                                    if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] % 2 == 1)
                                    {
                                        Game_Result[2] = "홀";
                                    }
                                    else
                                    {
                                        Game_Result[2] = "짝";
                                    }
                                    if (ballSum[0] + ballSum[1] + ballSum[2] + ballSum[3] + ballSum[4] > 72)
                                    {
                                        Game_Result[3] = "오";
                                    }
                                    else
                                    {
                                        Game_Result[3] = "언";
                                    }
                                    txtLogAdd(txtLog, "[" + (_dayRoundNo) + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
                                    return true;
                                }
                                else
                                {
                                    // 파워볼 합계
                                    int.TryParse(jo.SelectToken("more_info.pball").ToString(), out int pball);

                                    if (pball % 2 == 1)
                                    {
                                        Game_Result[0] = "홀";
                                    }
                                    else
                                    {
                                        Game_Result[0] = "짝";
                                    }
                                    if (pball > 4)
                                    {
                                        Game_Result[1] = "오";
                                    }
                                    else
                                    {
                                        Game_Result[1] = "언";
                                    }
                                    // 일반볼 합계
                                    int.TryParse(jo.SelectToken("more_info.sum").ToString(), out int sum);
                                    if (sum % 2 == 1)
                                    {
                                        Game_Result[2] = "홀";
                                    }
                                    else
                                    {
                                        Game_Result[2] = "짝";
                                    }
                                    if (sum > 72)
                                    {
                                        Game_Result[3] = "오";
                                    }
                                    else
                                    {
                                        Game_Result[3] = "언";
                                    }
                                    txtLogAdd(txtLog, "[" + (_dayRoundNo) + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
                                    return true;
                                }
                            }
                            else if (GameCode.Equals("MPB"))
                            {
                                // 동물번호 합계
                                int.TryParse(jo.SelectToken("more_info.sum").ToString(), out int sum);

                                // 행운번호 합계
                                int.TryParse(jo.SelectToken("more_info.pball").ToString(), out int pball);
                                if (pball % 2 == 1)
                                {
                                    Game_Result[0] = "홀";
                                }
                                else
                                {
                                    Game_Result[0] = "짝";
                                }
                                if (pball > 4)
                                {
                                    Game_Result[1] = "오";
                                }
                                else
                                {
                                    Game_Result[1] = "언";
                                }
                                if (sum % 2 == 1)
                                {
                                    Game_Result[2] = "홀";
                                }
                                else
                                {
                                    Game_Result[2] = "짝";
                                }
                                if (sum > 6)
                                {
                                    Game_Result[3] = "오";
                                }
                                else
                                {
                                    Game_Result[3] = "언";
                                }
                                txtLogAdd(txtLog, "[" + (_dayRoundNo) + "회] " + Game_Result[0] + " | " + Game_Result[1] + " || " + Game_Result[2] + " | " + Game_Result[3], Color.Black);
                                return true;
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
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
                return false;
            }
        }

        private void Game_Betting_Mode_Result_Processing()
        {
            Game_Betting_Mode_Result_Process = true;
            for (int i = 1; i <= 8; i++)
            {
                CheckBox Game_CheckBox = Controls.Find("Game_" + i + "_CheckBox", true)[0] as CheckBox;
                if (Game_CheckBox.Checked)
                {
                    for (int ii = 1; ii <= 4; ii++)
                    {
                        CheckBox Game_Sub_CheckBox = Controls.Find("Game_" + i + "_" + ii + "_CheckBox", true)[0] as CheckBox;
                        Button Game_Betpick = Controls.Find("Game_" + i + "_" + ii + "_Betpick", true)[0] as Button;
                        ComboBox Game_Level = Controls.Find("Game_" + i + "_" + ii + "_Level", true)[0] as ComboBox;
                        betLevelChange(Game_CheckBox, Game_Sub_CheckBox, Game_Betpick, Game_Result[ii - 1], Game_Level);
                    }
                }
            }
        }

        private void betLevelChange(CheckBox checkbox1, CheckBox checkbox2, Button betPick, String gameResult, ComboBox level)
        {
            if (checkbox2.Checked)
            {
                if (!betPick.Text.Contains("통"))
                {
                    string[] levelSplit = level.Text.Split(new char[] { '-' });
                    if (gameResult.Equals(betPick.Text))
                    {
                        if (levelSplit[1].Equals("1"))
                        {
                            level.Text = levelSplit[0] + "-2";
                        }
                        else if (levelSplit[1].Equals("2"))
                        {
                            level.Text = levelSplit[0] + "-3";
                        }
                        else if (levelSplit[1].Equals("3"))
                        {
                            ListViewItem item;
                            item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            item.UseItemStyleForSubItems = false;
                            item.SubItems.Add((dayRoundNo - 1).ToString());
                            item.SubItems.Add(checkbox1.Text + " || " + checkbox2.Text);
                            item.SubItems.Add(levelSplit[0]);
                            Game_Clear_Level_50.Items.Add(item);

                            level.Text = "1-1";
                            if (Game_WinToStopCheckBox.Checked)
                            {
                                checkbox2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        level.Text = (int.Parse(levelSplit[0]) + 1).ToString() + "-1";
                    }
                }
                betPick.BackColor = Color.WhiteSmoke;
                betPick.ForeColor = Color.Black;
                betPick.Text = "통";
            }
        }
        private void Game_Result_Processing()
        {
            if (!Game_Betting_Mode_Result_Process)
            {
                Game_Betting_Mode_Result_Processing();
            }
            ResultProcessListView();
            GameProfitMoneySet();
        }

        private void ResultProcessListView()
        {
            ListViewItem ResultListViewItem = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ResultListViewItem.UseItemStyleForSubItems = false;
            ResultListViewItem.SubItems.Add(callResultInning(GameCode).ToString());
            ResultListViewItem.SubItems.Add(Game_Result[0]);
            ResultListViewItem.SubItems.Add(Game_Result[1]);
            ResultListViewItem.SubItems.Add(Game_Result[2]);
            ResultListViewItem.SubItems.Add(Game_Result[3]);
            Game_BetResultListView.Items.Add(ResultListViewItem);

            for (int i = 0; i < Game_BetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game_BetRegistListView.Items[i];

                bool isContains = item.SubItems[1].Text.Equals(callResultInning(GameCode).ToString());

                if (isContains)
                {
                    /*********** 파워볼 당첨 여부 **************/
                    if (Game_Result[0].Equals("홀"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[2].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[3].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[2].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[2].ForeColor = Color.White;
                        item.SubItems[3].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[3].ForeColor = Color.White;
                    }
                    if (Game_Result[0].Equals("짝"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[4].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[5].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[4].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[4].ForeColor = Color.White;
                        item.SubItems[5].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[5].ForeColor = Color.White;
                    }
                    if (Game_Result[1].Equals("언"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[6].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[7].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[6].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[6].ForeColor = Color.White;
                        item.SubItems[7].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[7].ForeColor = Color.White;
                    }
                    if (Game_Result[1].Equals("오"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[8].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[9].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[8].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[8].ForeColor = Color.White;
                        item.SubItems[9].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[9].ForeColor = Color.White;
                    }


                    if (Game_Result[2].Equals("홀"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[10].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[11].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[10].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[10].ForeColor = Color.White;
                        item.SubItems[11].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[11].ForeColor = Color.White;
                    }
                    if (Game_Result[2].Equals("짝"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[12].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[13].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[12].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[12].ForeColor = Color.White;
                        item.SubItems[13].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[13].ForeColor = Color.White;
                    }
                    if (Game_Result[3].Equals("언"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[14].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[15].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[14].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[14].ForeColor = Color.White;
                        item.SubItems[15].BackColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                        item.SubItems[15].ForeColor = Color.White;
                    }
                    if (Game_Result[3].Equals("오"))
                    {
                        int.TryParse(Regex.Replace(item.SubItems[16].Text, @"\D", ""), out int Out_BetMoney);
                        int winMoney = (int)(Out_BetMoney * 1.95);
                        int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int outValue1);
                        Game_WinMoney.Text = UtilModel.StringFormatChanged(outValue1 + winMoney);

                        //All_Win_Bet_Money += winMoney;
                        item.SubItems[17].Text = UtilModel.StringFormatChanged(winMoney);

                        item.SubItems[16].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[16].ForeColor = Color.White;
                        item.SubItems[17].BackColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
                        item.SubItems[17].ForeColor = Color.White;
                    }
                    break;
                }
            }
        }

        private void GameProfitMoneySet()
        {
            int.TryParse(Regex.Replace(Game_WinMoney.Text, @"\D", ""), out int Game_WinMoney_Out_Value);
            int.TryParse(Regex.Replace(Game_StackMoney.Text, @"\D", ""), out int Game_StackMoney_Out_Value);
            Game_ProfitMoney.Text = UtilModel.StringFormatChanged(Game_WinMoney_Out_Value - Game_StackMoney_Out_Value);

            if (Game_WinMoney_Out_Value - Game_StackMoney_Out_Value < 0)
            {
                Game_ProfitMoney.ForeColor = Color.Red;
            }
            else if (Game_WinMoney_Out_Value - Game_StackMoney_Out_Value > 0)
            {
                Game_ProfitMoney.ForeColor = Color.Blue;
            }
        }

        private void Game_Bet_Processing_RegistListView()
        {
            int.TryParse(Regex.Replace(Game_StackMoney.Text, @"\D", ""), out int outValue);
            bool isContains = false;

            for (int i = 0; i < Game_BetRegistListView.Items.Count; i++)
            {
                ListViewItem items = Game_BetRegistListView.Items[i];

                isContains = items.SubItems[1].Text.Equals(dayRoundNo.ToString());

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
            item.SubItems.Add(dayRoundNo.ToString());

            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[0]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[1]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[2]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[3]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[4]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[5]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[6]));
            item.SubItems.Add("0");
            item.SubItems.Add(UtilModel.StringFormatChanged(Game_BetMoney[7]));
            item.SubItems.Add("0");
            Game_StackMoney.Text = UtilModel.StringFormatChanged(outValue + Game_BetMoney[0] + Game_BetMoney[1] + Game_BetMoney[2] + Game_BetMoney[3] + Game_BetMoney[4] + Game_BetMoney[5] + Game_BetMoney[6] + Game_BetMoney[7]);
            Game_BetRegistListView.Items.Add(item);
            txtLogAdd(txtLog, dayRoundNo + "회 등록이 완료", Color.OrangeRed);
        }

        private void Game_PatternCheck()
        {
            Game_PatternCheckLevel1();
            TimeSpan differentTime;
            for (int i = 0; i < Game_BetRegistListView.Items.Count; i++)
            {
                ListViewItem item = Game_BetRegistListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 120)
                {
                    Game_BetRegistListView.Items.Remove(Game_BetRegistListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }

            for (int i = 0; i < Game_BetResultListView.Items.Count; i++)
            {
                ListViewItem item = Game_BetResultListView.Items[i];
                differentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) - Convert.ToDateTime(item.SubItems[0].Text);

                if (differentTime.TotalMinutes > 30)
                {
                    Game_BetResultListView.Items.Remove(Game_BetResultListView.Items[i]); // 해당  Item 삭제 
                    i = i - 1; //Item 하나가 삭제되면리스트뷰 총 아이템 수가 달라지므로 해당 숫자부터 처음부터 다시 계산 
                }
            }
            Game_Check_Complete = true;
        }
        private void Game_PatternCheckLevel1()
        {
            if (Game_1_CheckBox.Checked)
            {
                Game_PatternCheck_Level2("Game_1_", pattern_1);
                Game_PatternCheck_Level2("Game_2_", pattern_2);
                Game_PatternCheck_Level2("Game_3_", pattern_3);
                Game_PatternCheck_Level2("Game_4_", pattern_4);
                Game_PatternCheck_Level2("Game_5_", pattern_5);
                Game_PatternCheck_Level2("Game_6_", pattern_6);
                Game_PatternCheck_Level2("Game_7_", pattern_7);
                Game_PatternCheck_Level2("Game_8_", pattern_8);
            }
        }
        private void Game_PatternCheck_Level2(string GameNo, string[] pattern)
        {
            CheckBox Game_1_CheckBox = (Controls.Find(GameNo + "1_CheckBox", true)[0] as CheckBox);
            CheckBox Game_2_CheckBox = (Controls.Find(GameNo + "2_CheckBox", true)[0] as CheckBox);
            CheckBox Game_3_CheckBox = (Controls.Find(GameNo + "3_CheckBox", true)[0] as CheckBox);
            CheckBox Game_4_CheckBox = (Controls.Find(GameNo + "4_CheckBox", true)[0] as CheckBox);

            TextBox Game_1_BetMoney = (Controls.Find(GameNo + "1_BetMoney", true)[0] as TextBox);
            TextBox Game_2_BetMoney = (Controls.Find(GameNo + "2_BetMoney", true)[0] as TextBox);
            TextBox Game_3_BetMoney = (Controls.Find(GameNo + "3_BetMoney", true)[0] as TextBox);
            TextBox Game_4_BetMoney = (Controls.Find(GameNo + "4_BetMoney", true)[0] as TextBox);

            Button Game_1_BetPick = (Controls.Find(GameNo + "1_Betpick", true)[0] as Button);
            Button Game_2_BetPick = (Controls.Find(GameNo + "2_Betpick", true)[0] as Button);
            Button Game_3_BetPick = (Controls.Find(GameNo + "3_Betpick", true)[0] as Button);
            Button Game_4_BetPick = (Controls.Find(GameNo + "4_Betpick", true)[0] as Button);

            ComboBox Game_1_Level = (Controls.Find(GameNo + "1_Level", true)[0] as ComboBox);
            ComboBox Game_2_Level = (Controls.Find(GameNo + "2_Level", true)[0] as ComboBox);
            ComboBox Game_3_Level = (Controls.Find(GameNo + "3_Level", true)[0] as ComboBox);
            ComboBox Game_4_Level = (Controls.Find(GameNo + "4_Level", true)[0] as ComboBox);

            Game_1_BetMoney.Text = checkGameCheck(Game_1_CheckBox, pattern, Game_1_BetPick, Game_1_Level, Game_BetResultListView, 2, 0);
            Game_2_BetMoney.Text = checkGameCheck(Game_2_CheckBox, pattern, Game_2_BetPick, Game_2_Level, Game_BetResultListView, 3, 3);
            Game_3_BetMoney.Text = checkGameCheck(Game_3_CheckBox, pattern, Game_3_BetPick, Game_3_Level, Game_BetResultListView, 4, 0);
            Game_4_BetMoney.Text = checkGameCheck(Game_4_CheckBox, pattern, Game_4_BetPick, Game_4_Level, Game_BetResultListView, 5, 3);

            Game_1_BetMoney.ForeColor = ReturnLevelForeColor(int.Parse(Game_1_Level.Text.Split(new char[] { '-' })[0]));
            Game_2_BetMoney.ForeColor = ReturnLevelForeColor(int.Parse(Game_2_Level.Text.Split(new char[] { '-' })[0]));
            Game_3_BetMoney.ForeColor = ReturnLevelForeColor(int.Parse(Game_3_Level.Text.Split(new char[] { '-' })[0]));
            Game_4_BetMoney.ForeColor = ReturnLevelForeColor(int.Parse(Game_4_Level.Text.Split(new char[] { '-' })[0]));

            Game_1_BetMoney.BackColor = ReturnLevelBackColor(int.Parse(Game_1_Level.Text.Split(new char[] { '-' })[0]));
            Game_2_BetMoney.BackColor = ReturnLevelBackColor(int.Parse(Game_2_Level.Text.Split(new char[] { '-' })[0]));
            Game_3_BetMoney.BackColor = ReturnLevelBackColor(int.Parse(Game_3_Level.Text.Split(new char[] { '-' })[0]));
            Game_4_BetMoney.BackColor = ReturnLevelBackColor(int.Parse(Game_4_Level.Text.Split(new char[] { '-' })[0]));
        }
        // string[] pattern_1 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-짝|짝홀홀-홀", "홀짝짝짝-홀|짝홀홀홀-짝", "언오-오|오언-언", "언오오-오|오언언-언", "언오오오-언|오언언언-오" };

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
        private String checkGameCheck(CheckBox GameUseCheck, string[] pattern, Button BetPick, ComboBox gameLevel, ListView resultlistView, int SubItemNum, int plusNum)
        {
            if (!GameUseCheck.Checked)
            {
                return "0";
            }
            string[] gameSubLevel = gameLevel.Text.Split(new char[] { '-' });
            string[] stringSplit = pattern[int.Parse(gameSubLevel[1]) - 1 + plusNum].Split(new char[] { '|' });

            for (int i = 0; i < stringSplit.Length; i++)
            {
                string[] s = stringSplit[i].Split(new char[] { '-' });
                string s1 = String.Empty;

                if (s[0].Length <= resultlistView.Items.Count)
                {
                    for (int ii = 0; ii < s[0].Length; ii++)
                    {
                        s1 += resultlistView.Items[ii].SubItems[SubItemNum].Text;
                    }
                }

                if (String.IsNullOrEmpty(s1))
                {
                    return "0";
                }
                if (s[0].Equals(str_reverse(s1)))
                {
                    BetPick.Text = s[1];
                    return UtilModel.StringFormatChanged(UtilModel.Game_CruiseAllBetMoney[int.Parse(gameSubLevel[0]), int.Parse(gameSubLevel[1])]);
                }
            }
            return "0";
        }

        private void Game_Bet_Processing_AllSum()
        {
            Game_Bet_Process_AllSum_Level1("Game_1_");
            Game_Bet_Process_AllSum_Level1("Game_2_");
            Game_Bet_Process_AllSum_Level1("Game_3_");
            Game_Bet_Process_AllSum_Level1("Game_4_");
            Game_Bet_Process_AllSum_Level1("Game_5_");
            Game_Bet_Process_AllSum_Level1("Game_6_");
            Game_Bet_Process_AllSum_Level1("Game_7_");
            Game_Bet_Process_AllSum_Level1("Game_8_");
        }

        private void Game_Bet_Process_AllSum_Level1(string gameNo)
        {
            Button Game_1_BetPickLevel = Controls.Find(gameNo + "1_BetPick", true)[0] as Button;
            Button Game_2_BetPickLevel = Controls.Find(gameNo + "2_BetPick", true)[0] as Button;
            Button Game_3_BetPickLevel = Controls.Find(gameNo + "3_BetPick", true)[0] as Button;
            Button Game_4_BetPickLevel = Controls.Find(gameNo + "4_BetPick", true)[0] as Button;

            TextBox Game_1_BetMoneyLevel = (Controls.Find(gameNo + "1_BetMoney", true)[0] as TextBox);
            TextBox Game_2_BetMoneyLevel = (Controls.Find(gameNo + "2_BetMoney", true)[0] as TextBox);
            TextBox Game_3_BetMoneyLevel = (Controls.Find(gameNo + "3_BetMoney", true)[0] as TextBox);
            TextBox Game_4_BetMoneyLevel = (Controls.Find(gameNo + "4_BetMoney", true)[0] as TextBox);

            if (string.IsNullOrEmpty(Game_1_BetPickLevel.Text) || Game_1_BetPickLevel.Text.Equals("통"))
            {
                Game_BetMoney[0] += 0;
                Game_BetMoney[1] += 0;
            }
            else if (Game_1_BetPickLevel.Text.Equals("홀"))
            {
                int.TryParse(Regex.Replace(Game_1_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[0] += outValue;
            }
            else if (Game_1_BetPickLevel.Text.Equals("짝"))
            {
                int.TryParse(Regex.Replace(Game_1_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[1] += outValue;
            }
            if (string.IsNullOrEmpty(Game_2_BetPickLevel.Text) || Game_2_BetPickLevel.Text.Equals("통"))
            {
                Game_BetMoney[2] += 0;
                Game_BetMoney[3] += 0;
            }
            else if (Game_2_BetPickLevel.Text.Equals("언"))
            {
                int.TryParse(Regex.Replace(Game_2_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[2] += outValue;
            }
            else if (Game_2_BetPickLevel.Text.Equals("오"))
            {
                int.TryParse(Regex.Replace(Game_2_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[3] += outValue;
            }
            if (string.IsNullOrEmpty(Game_3_BetPickLevel.Text) || Game_3_BetPickLevel.Text.Equals("통"))
            {
                Game_BetMoney[4] += 0;
                Game_BetMoney[5] += 0;
            }
            else if (Game_3_BetPickLevel.Text.Equals("홀"))
            {
                int.TryParse(Regex.Replace(Game_3_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[4] += outValue;
            }
            else if (Game_3_BetPickLevel.Text.Equals("짝"))
            {
                int.TryParse(Regex.Replace(Game_3_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[5] += outValue;
            }
            if (string.IsNullOrEmpty(Game_4_BetPickLevel.Text) || Game_4_BetPickLevel.Text.Equals("통"))
            {
                Game_BetMoney[6] += 0;
                Game_BetMoney[7] += 0;
            }
            else if (Game_4_BetPickLevel.Text.Equals("언"))
            {
                int.TryParse(Regex.Replace(Game_4_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[6] += outValue;
            }
            else if (Game_4_BetPickLevel.Text.Equals("오"))
            {
                int.TryParse(Regex.Replace(Game_4_BetMoneyLevel.Text, @"\D", ""), out int outValue);
                Game_BetMoney[7] += outValue;
            }
        }
        private void GameStop()
        {
            Game_RemainingTimer.Stop();
            Game_Start_Button.ForeColor = Color.FromArgb(255, Color.FromArgb(0xC62828));
            Game_Start_Button.Text = "배팅이 정지되었습니다.";
            StartGame = false;

            Game_Result = new string[4];

            Game_Betting_Mode_Result_Process = false;
            Game_Betting_Complete_Status = false;
            Game_Result_Load_Complete = false;
            Game_Check_Complete = false;
            Game_BetMoney = new int[8];

            Game_1_CheckBox.Checked = false;
            Game_2_CheckBox.Checked = false;
            Game_3_CheckBox.Checked = false;
            Game_4_CheckBox.Checked = false;
            Game_5_CheckBox.Checked = false;
            Game_6_CheckBox.Checked = false;
            Game_7_CheckBox.Checked = false;
            Game_8_CheckBox.Checked = false;

            logger.Info("[프로그램 정지][" + Game_Select_Game.Text + "] 게임이 종료되었습니다.");
        }
        public void setTimeRemaining(Button _button, double _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            _button.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
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

        private int callResultInning(String GCode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                if (dayRoundNo == 1)
                {
                    if (GCode.Equals("EPB") || GCode.Equals("SKPB"))
                    {
                        return 288;
                    }
                    else if (GCode.Equals("MPB"))
                    {
                        return 480;
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
                        return -1;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("EPB") || GCode.Equals("DSCP5") || GCode.Equals("EPB3") || GCode.Equals("DSCP3") || GCode.Equals("HSP3") || GCode.Equals("HSP5") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5") || GCode.Equals("MPB") || GCode.Equals("SKPB") || GCode.Equals("CSA3") || GCode.Equals("CSA5") || GCode.Equals("HSPSA3") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5"))
                    {
                        return dayRoundNo - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Life)
            {
                if (dayRoundNo == 1)
                {
                    if (GCode.Equals("EPB"))
                    {
                        return 288;
                    }
                    else if (GCode.Equals("SKPB"))
                    {
                        return 288;
                    }
                    else if (GCode.Equals("MPB"))
                    {
                        return 480;
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
                        return -1;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("EPB") || GCode.Equals("DSCP5") || GCode.Equals("EPB3") || GCode.Equals("DSCP3") || GCode.Equals("HSP3") || GCode.Equals("HSP5") || GCode.Equals("KLAY2") || GCode.Equals("KLAY5") || GCode.Equals("CSA3") || GCode.Equals("CSA5") || GCode.Equals("HSPSA3") || GCode.Equals("HSPSA5") || GCode.Equals("KLAYSA2") || GCode.Equals("KALYSA5"))
                    {
                        return dayRoundNo - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Royal)
            {
                if (dayRoundNo == 1)
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
                        return -1;
                    }
                }
                else
                {
                    // "DSCP3", "DSCP5", "EPB3", "EPB", "HSP3", "HSP5", "KLAY2", "KLAY5", "CSA3", "CSA5", "HSPSA3", "HSPSA5", "KLAYSA2", "KALYSA5"
                    if (GCode.Equals("EOSP3") || GCode.Equals("EOSP5") || GCode.Equals("HSPB3") || GCode.Equals("HSPB5") || GCode.Equals("DSCB3") || GCode.Equals("DSCB5") || GCode.Equals("BEXB"))
                    {
                        return dayRoundNo - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return -1;
        }
        private int betTime(string GCode)
        {
            if (UtilModel.LoginSiteType == UtilModel.LoginSiteType_Click)
            {
                if (GCode.Equals("DSCP3") || GCode.Equals("EPB3") || GCode.Equals("HSP3") || GCode.Equals("CSA3") || GCode.Equals("HSPSA3") || GCode.Equals("MPB"))
                {
                    return 180;
                }
                else if (GCode.Equals("DSCP5") || GCode.Equals("EPB") || GCode.Equals("SKPB") || GCode.Equals("HSP5") || GCode.Equals("KLAY5")
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
                if (GCode.Equals("DSCP3") || GCode.Equals("EPB3") || GCode.Equals("HSP3") || GCode.Equals("CSA3") || GCode.Equals("HSPSA3") || GCode.Equals("MPB"))
                {
                    return 180;
                }
                else if (GCode.Equals("DSCP5") || GCode.Equals("EPB") || GCode.Equals("SKPB") || GCode.Equals("HSP5") || GCode.Equals("KLAY5")
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
                if (GCode.Equals("EOSP3") || GCode.Equals("HSPB3") || GCode.Equals("DSCB3") || GCode.Equals("BEXB"))
                {
                    return 180;
                }
                else if (GCode.Equals("EOSP5") || GCode.Equals("HSPB5") || GCode.Equals("DSCB5"))
                {
                    return 300;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }
        static string str_reverse(string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        private void Game_Start_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (String.IsNullOrEmpty(GameCode))
            {
                txtLogAdd(txtLog, "게임이 선택되지 않았습니다.", Color.Red);
                return;
            }

            if (!StartGame)
            {
                JObject jObject = JObject.Parse(UtilModel.LoadInformation());
                JToken jTokenCustomers = jObject["Customers"];
                if (jObject.SelectToken("setting.UpdateVersion").ToString().Equals(UtilModel.UpdateVersion) == false)
                {
                    MessageBox.Show("프로그램이 업데이트 되었습니다.\r\n업데이트 후 이용해 주시기 바랍니다.");
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

                Game_RemainingTimer.Start();

                button.Text = "배팅이 진행 중입니다.";
                button.ForeColor = Color.FromArgb(255, Color.FromArgb(0x1565C0));
                StartGame = true;
                StartDateTime = DateTime.Now;

                Game_Random_Result_Load_Time_For_PowerBall = rand.Next(betTime(GameCode) - 50, betTime(GameCode) - 20);
                Game_Random_Bet_Regist_Time_For_PowerBall = rand.Next(betTime(GameCode) - 70, betTime(GameCode) - 40);

                Game_BetResultListView.Items.Clear();
                Random_Nonce = rand.Next(99999);
            }
            else
            {
                if (MessageBox.Show("게임을 정말로 종료하시겠습니까?", "선택하십시요!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Game_BetResultListView.Items.Clear();
                    GameStop();
                }
                else
                {
                    txtLogAdd(txtLog, "취소되었습니다.", Color.Red);
                }
            }
        }
        private void GameCheckBoxCheckedChanged(CheckBox checkbox, string controlName)
        {
            CheckBox Game_1_1_CheckBox = Controls.Find(controlName + "1_CheckBox", true)[0] as CheckBox;
            CheckBox Game_1_2_CheckBox = Controls.Find(controlName + "2_CheckBox", true)[0] as CheckBox;
            CheckBox Game_1_3_CheckBox = Controls.Find(controlName + "3_CheckBox", true)[0] as CheckBox;
            CheckBox Game_1_4_CheckBox = Controls.Find(controlName + "4_CheckBox", true)[0] as CheckBox;

            // Game1_1_1_Level
            ComboBox Game_1_1_Level = Controls.Find(controlName + "1_Level", true)[0] as ComboBox;
            ComboBox Game_1_2_Level = Controls.Find(controlName + "2_Level", true)[0] as ComboBox;
            ComboBox Game_1_3_Level = Controls.Find(controlName + "3_Level", true)[0] as ComboBox;
            ComboBox Game_1_4_Level = Controls.Find(controlName + "4_Level", true)[0] as ComboBox;
            if (checkbox.Checked)
            {
                Game_1_1_CheckBox.Checked = true;
                Game_1_2_CheckBox.Checked = true;
                Game_1_3_CheckBox.Checked = true;
                Game_1_4_CheckBox.Checked = true;
                Game_1_1_Level.Text = "1-1";
                Game_1_2_Level.Text = "1-1";
                Game_1_3_Level.Text = "1-1";
                Game_1_4_Level.Text = "1-1";
            }
            else
            {
                Game_1_1_CheckBox.Checked = false;
                Game_1_2_CheckBox.Checked = false;
                Game_1_3_CheckBox.Checked = false;
                Game_1_4_CheckBox.Checked = false;
                Game_1_1_Level.Text = "0";
                Game_1_2_Level.Text = "0";
                Game_1_3_Level.Text = "0";
                Game_1_4_Level.Text = "0";
            }
        }

        private void Game_1_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_1_");
        }

        private void Game_2_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_2_");
        }

        private void Game_3_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_3_");
        }

        private void Game_4_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_4_");
        }

        private void Game_5_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_5_");
        }

        private void Game_6_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_6_");
        }

        private void Game_7_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_7_");
        }

        private void Game_8_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            GameCheckBoxCheckedChanged(checkBox, "Game_8_");
        }

        Random rand = new Random();
        private int[] Game_BetMoney = new int[8];
        System.Timers.Timer Game_RemainingTimer = new System.Timers.Timer();
        Boolean Game_Betting_Mode_Result_Process = false;
        Boolean Game_Result_Load_Complete = false;
        Boolean Game_Check_Complete = false;
        Boolean Game_Betting_Complete_Status = false;
        Boolean StartGame = false;
        DateTime StartDateTime;
        int roundNo = 0;
        int dayRoundNo = 0;
        string GameCode = null;
        int BetRemainingTime = 0;
        int Game_Random_Result_Load_Time_For_PowerBall = 0;
        int Game_Random_Bet_Regist_Time_For_PowerBall = 0;
        int BetEndSec = 30;
        string[] Game_Result = new string[4];
        int Random_Nonce = 0;
        readonly string[] pattern_1 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-짝|짝홀홀-홀", "홀짝짝짝-홀|짝홀홀홀-짝", "언오-오|오언-언", "언오오-오|오언언-언", "언오오오-언|오언언언-오" };
        readonly string[] pattern_2 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-짝|짝홀홀-홀", "홀짝짝짝-짝|짝홀홀홀-홀", "언오-오|오언-언", "언오오-오|오언언-언", "언오오오-오|오언언언-언" };
        readonly string[] pattern_3 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-홀|짝홀홀-짝", "홀짝짝홀-짝|짝홀홀짝-홀", "언오-오|오언-언", "언오오-언|오언언-오", "언오오언-오|오언언오-언" };
        readonly string[] pattern_4 = new string[] { "홀짝-짝|짝홀-홀", "홀짝짝-홀|짝홀홀-짝", "홀짝짝홀-홀|짝홀홀짝-짝", "언오-오|오언-언", "언오오-언|오언언-오", "언오오언-언|오언언오-오" };
        readonly string[] pattern_5 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-짝|짝홀짝-홀", "홀짝홀짝-짝|짝홀짝홀-홀", "언오-언|오언-오", "언오언-오|오언오-언", "언오언오-오|오언오언-언" };
        readonly string[] pattern_6 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-짝|짝홀짝-홀", "홀짝홀짝-홀|짝홀짝홀-짝", "언오-언|오언-오", "언오언-오|오언오-언", "언오언오-언|오언오언-오" };
        readonly string[] pattern_7 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-홀|짝홀짝-짝", "홀짝홀홀-짝|짝홀짝짝-홀", "언오-언|오언-오", "언오언-언|오언오-오", "언오언언-오|오언오오-언" };
        readonly string[] pattern_8 = new string[] { "홀짝-홀|짝홀-짝", "홀짝홀-홀|짝홀짝-짝", "홀짝홀홀-홀|짝홀짝짝-짝", "언오-언|오언-오", "언오언-언|오언오-오", "언오언언-언|오언오오-오" };

    }
}
