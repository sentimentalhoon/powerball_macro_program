using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;
using System.Web;
using System.Xml;
using System.Net;
using log4net.Util;

namespace WaterMelonBettingProgram
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainForm()
        {
            InitializeComponent();
        }

        System.Timers.Timer bettingTimer = new System.Timers.Timer();
        DateTime _startDateTime;
        readonly int programVersion = 2020070401;
        Boolean BetStartStatus { get; set; }
        Boolean BetProcessing { get; set; }
        Boolean BetResultProcessing { get; set; }
        Boolean BetClosed { get; set; }
        Boolean bettingStatus { get; set; }
        int bettingTime { get; set; }
        Boolean useVirtureMoney { get; set; }
        int betStartMoney { get; set; }
        int betOwnMoney { get; set; }
        int nowAllInning { get; set; }
        int todayRound { get; set; }
        Boolean loadPowerBallAllResult { get; set; }
        Boolean loadPowerBallResult { get; set; }
        String Result_Powerball_Oddeven { get; set; }
        String Result_Powerball_Underover { get; set; }
        String Result_Normalball_Oddeven { get; set; }
        String Result_Normalball_Underover { get; set; }
        int betPOddMoney { get; set; }
        int betPEvenMoney { get; set; }
        int betPOverMoney { get; set; }
        int betPUnderMoney { get; set; }
        int betNOddMoney { get; set; }
        int betNEvenMoney { get; set; }
        int betNOverMoney { get; set; }
        int betNUnderMoney { get; set; }
        Boolean timeInit { get; set; }
        StringBuilder oddPowerBallOddEven;
        StringBuilder evenPowerBallOddEven;

        StringBuilder oddPowerBallUnderOver;
        StringBuilder evenPowerBallUnderOver;

        StringBuilder oddNormalBallOddEven;
        StringBuilder evenNormalBallOddEven;

        StringBuilder oddNormalBallUnderOver;
        StringBuilder evenNormalBallUnderOver;
        private void bettingTiemr_Tick()
        {
            TimeSpan diff = DateTime.Now - _startDateTime;
            lblElapsedTime.Text = string.Format("{0:dd\\ hh\\:mm\\:ss}", diff);
            bettingTime--;
            remainingTimeText.Text = bettingTime.ToString();

            if (bettingTime > 64 && bettingTime % 60 == 5)
            {
                getRemainingTime();
            }
            if (bettingTime <= 0)
            {
                bettingTime = 300;
                txtLog.Text = "";
                getRemainingTime();
                loadPowerBallAllResult = false;
                loadPowerBallResult = false;
                BetClosed = false;
                BetProcessing = false;
                BetResultProcessing = false;
            }
            if (bettingTime <= 299 && bettingTime >= 295 && !timeInit)
            {
                timeInit = true;
                nowAllInning++;
                todayRound++;
            }
            if (bettingTime <= 275 && bettingTime <= 265 && !loadPowerBallResult)
            {
                string returnMessage = string.Empty;
                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("http://www.powerballgame.co.kr/?view=action&action=ajaxPowerballLog&actionType=refreshLog");
                    stringBuilder.AppendFormat("&round=1000000&date={0}", DateTime.Now.ToString("yyyy-MM-dd"));
                    var rm = UtilModel.MakeAsyncRequest(stringBuilder.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                    returnMessage = rm.Result;
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse)
                    {
                        logger.Info(((int)((HttpWebResponse)ex.Response).StatusCode));
                        logger.Info(ex.Message);
                    }
                    else
                    {
                        logger.Info(ex.Message);
                    }
                    returnMessage = string.Empty;
                }
                if (string.IsNullOrEmpty(returnMessage))
                {
                    loadPowerBallResult = false;
                }
                else
                {
                    JObject jObject = JObject.Parse(returnMessage);

                    var a = jObject.SelectToken("state").ToString();
                    if (a.Equals("success"))
                    {
                        var content = jObject.SelectToken("content");

                        foreach (var contents in content)
                        {
                            nowAllInning = int.Parse(contents.SelectToken("round").ToString());
                            todayRound = int.Parse(contents.SelectToken("todayRound").ToString());
                            if (contents.SelectToken("powerballOddEven").ToString().Equals("odd"))
                            {
                                Result_Powerball_Oddeven = "홀";
                            }
                            else if (contents.SelectToken("powerballOddEven").ToString().Equals("even"))
                            {
                                Result_Powerball_Oddeven = "짝";
                            }

                            if (contents.SelectToken("powerballUnderOver").ToString().Equals("under"))
                            {
                                Result_Powerball_Underover = "언더";
                            }
                            else if (contents.SelectToken("powerballUnderOver").ToString().Equals("over"))
                            {
                                Result_Powerball_Underover = "오버";
                            }

                            if (contents.SelectToken("numberOddEven").ToString().Equals("odd"))
                            {
                                Result_Normalball_Oddeven = "홀";
                            }
                            else if (contents.SelectToken("numberOddEven").ToString().Equals("even"))
                            {
                                Result_Normalball_Oddeven = "짝";
                            }

                            if (contents.SelectToken("numberUnderOver").ToString().Equals("under"))
                            {
                                Result_Normalball_Underover = "언더";
                            }
                            else if (contents.SelectToken("numberUnderOver").ToString().Equals("over"))
                            {
                                Result_Normalball_Underover = "오버";
                            }
                        }

                        nowAllInning += 1;

                        txtLogAdd("파워볼 결과가 나왔습니다.", Color.Black);
                        txtLogAdd("파워볼 홀짝 : " + Result_Powerball_Oddeven, Color.Black);
                        txtLogAdd("파워볼 언오버 : " + Result_Powerball_Underover, Color.Black);
                        txtLogAdd("일반볼 홀짝 : " + Result_Normalball_Oddeven, Color.Black);
                        txtLogAdd("일반볼 언오버 : " + Result_Normalball_Underover, Color.Black);

                        nowInningText.Text = nowAllInning.ToString();

                        loadPowerBallResult = true;
                    }
                }
            }

            if (bettingTime <= 265 && bettingTime >= 260 && !BetResultProcessing)
            {
                BetResultProcessing = true;
                CheckBox checkBoxStatus;
                RichTextBox textBoxPattern;
                TextBox textBoxPick;
                TextBox textBoxEqual;
                ComboBox comboBoxLevel;
                TextBox oddEvenBet;
                for (int _find = 1; _find <= 40; _find++)
                {
                    oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);
                    if ((nowAllInning - 1) % 2 == 1)
                    {
                        if (oddEvenBet.Text.Equals("0"))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (oddEvenBet.Text.Equals("1"))
                        {
                            continue;
                        }
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }

                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        continue;
                    }
                    int _ComboBoxLevel = int.Parse(comboBoxLevel.Text);
                    if (_ComboBoxLevel == 0)
                    {
                        continue;
                    }
                    textBoxPick = (Controls.Find("textBoxPick" + _find.ToString(), true)[0] as TextBox);
                    if (textBoxPick.Text.Equals("통과"))
                    {
                        continue;
                    }
                    textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                    textBoxEqual.ForeColor = Color.DarkGray;

                    if (textBoxEqual.Text.Contains("불"))
                    {
                        continue;
                    }
                    textBoxEqual.Text = "불일치";

                    textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);

                    if (_find > 30)
                    {
                        if (Result_Normalball_Underover.Contains(textBoxPick.Text))
                        {
                            textBoxPattern.ForeColor = Color.Black;
                            textBoxPattern.BackColor = Color.White;
                            comboBoxLevel.Text = "0";
                        }
                        else
                        {
                            textBoxPattern.ForeColor = Color.White;
                            textBoxPattern.BackColor = Color.Black;
                            comboBoxLevel.Text = (_ComboBoxLevel + 1).ToString(); ;
                        }
                    }
                    else if (_find > 20)
                    {
                        if (textBoxPick.Text.Equals(Result_Normalball_Oddeven))
                        {
                            textBoxPattern.ForeColor = Color.Black;
                            textBoxPattern.BackColor = Color.White;
                            comboBoxLevel.Text = "0";
                        }
                        else
                        {
                            textBoxPattern.ForeColor = Color.White;
                            textBoxPattern.BackColor = Color.Black;
                            comboBoxLevel.Text = (_ComboBoxLevel + 1).ToString(); ;
                        }
                    }
                    else if (_find > 10)
                    {
                        if (Result_Powerball_Underover.Contains(textBoxPick.Text))
                        {
                            textBoxPattern.ForeColor = Color.Black;
                            textBoxPattern.BackColor = Color.White;
                            comboBoxLevel.Text = "0";
                        }
                        else
                        {
                            textBoxPattern.ForeColor = Color.White;
                            textBoxPattern.BackColor = Color.Black;
                            comboBoxLevel.Text = (_ComboBoxLevel + 1).ToString(); ;
                        }
                    }
                    else
                    {
                        if (Result_Powerball_Oddeven.Contains(textBoxPick.Text))
                        {
                            textBoxPattern.ForeColor = Color.Black;
                            textBoxPattern.BackColor = Color.White;
                            comboBoxLevel.Text = "0";
                        }
                        else
                        {
                            textBoxPattern.ForeColor = Color.White;
                            textBoxPattern.BackColor = Color.Black;
                            comboBoxLevel.Text = (_ComboBoxLevel + 1).ToString(); ;
                        }
                    }
                }
                directBetPowerBallResultProcessing();
            }
            // 결과값 불러와 패턴과 일치하는지 확인하는 과정
            if (bettingTime <= 240 && bettingTime >= 60)
            {
                if (bettingTime % 60 == 0)
                {
                    if (!loadPowerBallAllResult)
                    {
                        oddPowerBallOddEven = new StringBuilder();
                        evenPowerBallOddEven = new StringBuilder();

                        oddPowerBallUnderOver = new StringBuilder();
                        evenPowerBallUnderOver = new StringBuilder();

                        oddNormalBallOddEven = new StringBuilder();
                        evenNormalBallOddEven = new StringBuilder();

                        oddNormalBallUnderOver = new StringBuilder();
                        evenNormalBallUnderOver = new StringBuilder();
                        string returnMessage1 = string.Empty;
                        string returnMessage2 = string.Empty;

                        StringBuilder stringBuilder1 = new StringBuilder();
                        StringBuilder stringBuilder2 = new StringBuilder();

                        stringBuilder1.Append("http://www.powerballgame.co.kr/?view=action&action=ajaxPowerballLog&actionType=dayLog");
                        stringBuilder1.AppendFormat("&date={0}", DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                        stringBuilder1.Append("&page=1");

                        if (todayRound < 25)
                        {
                            stringBuilder2.Append("http://www.powerballgame.co.kr/?view=action&action=ajaxPowerballLog&actionType=dayLog");
                            stringBuilder2.AppendFormat("&date={0}", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", ""));
                            stringBuilder2.Append("&page=1");
                        }
                        try
                        {
                            var rm = UtilModel.MakeAsyncRequest(stringBuilder1.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                            returnMessage1 = rm.Result;
                            if (todayRound < 25)
                            {
                                rm = UtilModel.MakeAsyncRequest(stringBuilder2.ToString(), "application/x-www-form-urlencoded; charset=UTF-8");
                                returnMessage2 = rm.Result;
                            }
                        }
                        catch (WebException ex)
                        {
                            if (ex.Response is HttpWebResponse)
                            {
                                logger.Info(((int)((HttpWebResponse)ex.Response).StatusCode));
                                logger.Info(ex.Message);
                            }
                            else
                            {
                                logger.Info(ex.Message);
                            }
                            loadPowerBallAllResult = false;
                            returnMessage1 = string.Empty;
                            returnMessage2 = string.Empty;
                        }

                        if (string.IsNullOrEmpty(returnMessage1))
                        {
                            loadPowerBallAllResult = false;
                            txtLogAdd("결과값을 불러오는데 실패하였습니다. 재시도 중입니다", Color.Black);
                        }
                        else
                        {
                            if (todayRound < 25)
                            {
                                LoadPowerAllReultProcess(returnMessage2);
                            }
                            LoadPowerAllReultProcess(returnMessage1);

                            loadPowerBallAllResult = true;

                            PowerOddEvenResultListView.Items.Clear();
                            PowerUnderOverResultListView.Items.Clear();
                            NormalOddEvenResultListView.Items.Clear();
                            NormalUnderOverResultListView.Items.Clear();

                            ListViewItem Even_PowerBallOddEvenItem = new ListViewItem("짝수"); // 픽스터 이름
                            ListViewItem Odd_PowerBallOddEvenItem = new ListViewItem("홀수"); // 픽스터 이름

                            ListViewItem Even_PowerBallUnderOverEvenItem = new ListViewItem("짝수"); // 픽스터 이름
                            ListViewItem Odd_PowerBallUnderOverItem = new ListViewItem("홀수"); // 픽스터 이름

                            ListViewItem Even_NormalBallOddEvenItem = new ListViewItem("짝수"); // 픽스터 이름
                            ListViewItem Odd_NormalBallOddEvenItem = new ListViewItem("홀수"); // 픽스터 이름

                            ListViewItem Even_NormalBallUnderOverItem = new ListViewItem("짝수"); // 픽스터 이름
                            ListViewItem Odd_NormalBallUnderOverItem = new ListViewItem("홀수"); // 픽스터 이름

                            string odd_powerball_oddeven = string.Empty;
                            string odd_powerball_underover = string.Empty;
                            string odd_normalball_oddeven = string.Empty;
                            string odd_normalball_underover = string.Empty;

                            string even_powerball_oddeven = string.Empty;
                            string even_powerball_underover = string.Empty;
                            string even_normalball_oddeven = string.Empty;
                            string even_normalball_underover = string.Empty;

                            int num = -10;
                            for (int i = 11; i >= 0; i--)
                            {
                                Odd_PowerBallOddEvenItem.UseItemStyleForSubItems = false;
                                odd_powerball_oddeven = Reverse_Odd_PowerBallOddEven.Split(new char[] { '|' })[i];
                                Odd_PowerBallOddEvenItem.SubItems.Add(odd_powerball_oddeven);
                                if (odd_powerball_oddeven.Equals("홀"))
                                {
                                    Odd_PowerBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (odd_powerball_oddeven.Equals("짝"))
                                {
                                    Odd_PowerBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Odd_PowerBallOddEvenItem.SubItems[i + num].ForeColor = Color.White;

                                Odd_PowerBallUnderOverItem.UseItemStyleForSubItems = false;
                                odd_powerball_underover = Reverse_Odd_PowerBallUnderOver.Split(new char[] { '|' })[i];
                                Odd_PowerBallUnderOverItem.SubItems.Add(odd_powerball_underover);
                                if (odd_powerball_underover.Equals("언"))
                                {
                                    Odd_PowerBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (odd_powerball_underover.Equals("오"))
                                {
                                    Odd_PowerBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Odd_PowerBallUnderOverItem.SubItems[i + num].ForeColor = Color.White;

                                Odd_NormalBallOddEvenItem.UseItemStyleForSubItems = false;
                                odd_normalball_oddeven = Reverse_Odd_NormalBallOddEven.Split(new char[] { '|' })[i];
                                Odd_NormalBallOddEvenItem.SubItems.Add(odd_normalball_oddeven);
                                if (odd_normalball_oddeven.Equals("홀"))
                                {
                                    Odd_NormalBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (odd_normalball_oddeven.Equals("짝"))
                                {
                                    Odd_NormalBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Odd_NormalBallOddEvenItem.SubItems[i + num].ForeColor = Color.White;

                                Odd_NormalBallUnderOverItem.UseItemStyleForSubItems = false;
                                odd_normalball_underover = Reverse_Odd_NormalBallUnderOver.Split(new char[] { '|' })[i];
                                Odd_NormalBallUnderOverItem.SubItems.Add(odd_normalball_underover);
                                if (odd_normalball_underover.Equals("언"))
                                {
                                    Odd_NormalBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (odd_normalball_underover.Equals("오"))
                                {
                                    Odd_NormalBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Odd_NormalBallUnderOverItem.SubItems[i + num].ForeColor = Color.White;

                                Even_PowerBallOddEvenItem.UseItemStyleForSubItems = false;
                                even_powerball_oddeven = Reverse_Even_PowerBallOddEven.Split(new char[] { '|' })[i];
                                Even_PowerBallOddEvenItem.SubItems.Add(even_powerball_oddeven);
                                if (even_powerball_oddeven.Equals("홀"))
                                {
                                    Even_PowerBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (even_powerball_oddeven.Equals("짝"))
                                {
                                    Even_PowerBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Even_PowerBallOddEvenItem.SubItems[i + num].ForeColor = Color.White;

                                Even_PowerBallUnderOverEvenItem.UseItemStyleForSubItems = false;
                                even_powerball_underover = Reverse_Even_PowerBallUnderOver.Split(new char[] { '|' })[i];
                                Even_PowerBallUnderOverEvenItem.SubItems.Add(even_powerball_underover);
                                if (even_powerball_underover.Equals("언"))
                                {
                                    Even_PowerBallUnderOverEvenItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (even_powerball_underover.Equals("오"))
                                {
                                    Even_PowerBallUnderOverEvenItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Even_PowerBallUnderOverEvenItem.SubItems[i + num].ForeColor = Color.White;

                                Even_NormalBallOddEvenItem.UseItemStyleForSubItems = false;
                                even_normalball_oddeven = Reverse_Even_NormalBallOddEven.Split(new char[] { '|' })[i];
                                Even_NormalBallOddEvenItem.SubItems.Add(even_normalball_oddeven);
                                if (even_normalball_oddeven.Equals("홀"))
                                {
                                    Even_NormalBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (even_normalball_oddeven.Equals("짝"))
                                {
                                    Even_NormalBallOddEvenItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Even_NormalBallOddEvenItem.SubItems[i + num].ForeColor = Color.White;

                                Even_NormalBallUnderOverItem.UseItemStyleForSubItems = false;
                                even_normalball_underover = Reverse_Even_NormalBallUnderOver.Split(new char[] { '|' })[i];
                                Even_NormalBallUnderOverItem.SubItems.Add(even_normalball_underover);
                                if (even_normalball_underover.Equals("언"))
                                {
                                    Even_NormalBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkBlue;
                                }
                                else if (even_normalball_underover.Equals("오"))
                                {
                                    Even_NormalBallUnderOverItem.SubItems[i + num].BackColor = Color.DarkRed;
                                }
                                Even_NormalBallUnderOverItem.SubItems[i + num].ForeColor = Color.White;

                                num += 2;
                            }

                            PowerOddEvenResultListView.Items.Add(Odd_PowerBallOddEvenItem);
                            PowerOddEvenResultListView.Items.Add(Even_PowerBallOddEvenItem);

                            PowerUnderOverResultListView.Items.Add(Odd_PowerBallUnderOverItem);
                            PowerUnderOverResultListView.Items.Add(Even_PowerBallUnderOverEvenItem);

                            NormalOddEvenResultListView.Items.Add(Odd_NormalBallOddEvenItem);
                            NormalOddEvenResultListView.Items.Add(Even_NormalBallOddEvenItem);

                            NormalUnderOverResultListView.Items.Add(Odd_NormalBallUnderOverItem);
                            NormalUnderOverResultListView.Items.Add(Even_NormalBallUnderOverItem);
                        }
                    }

                    if (!BetClosed && loadPowerBallAllResult)
                    {
                        checkPatternString();
                    }
                }
            }

            if (bettingTime <= 55 && bettingTime >= 30 && !BetClosed)
            {
                BetClosed = true;
                timeInit = false;
                if (nowAllInning % 10 == 0)
                {
                    bettingStatusListView.Items.Clear();
                }

                betPOddMoney = 0;
                betPEvenMoney = 0;
                betPUnderMoney = 0;
                betPOverMoney = 0;
                betNOddMoney = 0;
                betNEvenMoney = 0;
                betNUnderMoney = 0;
                betNOverMoney = 0;
                CheckBox checkBoxStatus;
                ComboBox comboBoxLevel;
                TextBox textBoxPick;
                TextBox textBoxBetMoney;
                TextBox oddEvenBet;
                TextBox textBoxEqual;
                for (int _find = 1; _find <= 40; _find++)
                {
                    if (!checkPowerBallOddEven.Checked)
                    {
                        if (_find <= 10)
                        {
                            continue;
                        }
                    }
                    if (!checkPowerBallUnderOver.Checked)
                    {
                        if (_find >= 11 && _find <= 20)
                        {
                            continue;
                        }
                    }
                    if (!checkNormalBallOddEven.Checked)
                    {
                        if (_find >= 21 && _find <= 30)
                        {
                            continue;
                        }
                    }
                    if (!checkNormalBallUnderOver.Checked)
                    {
                        if (_find >= 31 && _find <= 40)
                        {
                            continue;
                        }
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }
                    oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);

                    if (nowAllInning % 2 == 0)
                    {
                        if (oddEvenBet.Text.Equals("1"))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (oddEvenBet.Text.Equals("0"))
                        {
                            continue;
                        }
                    }
                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        continue;
                    }
                    if (comboBoxLevel.Text.Equals("0"))
                    {
                        continue;
                    }
                    textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                    if (textBoxEqual.Text.Contains("불"))
                    {
                        continue;
                    }


                    textBoxPick = (Controls.Find("textBoxPick" + _find.ToString(), true)[0] as TextBox);
                    textBoxBetMoney = (Controls.Find("textBoxBetMoney" + _find.ToString(), true)[0] as TextBox);
                    int boxBetMoney = int.Parse(Regex.Replace(textBoxBetMoney.Text, @"\D", ""));
                    if (_find > 30)
                    {
                        if (textBoxPick.Text.Equals("홀"))
                        {
                            betNOddMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("짝"))
                        {
                            betNEvenMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("언"))
                        {
                            betNUnderMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("오"))
                        {
                            betNOverMoney += boxBetMoney;
                        }
                    }
                    else if (_find > 20)
                    {
                        if (textBoxPick.Text.Equals("홀"))
                        {
                            betNOddMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("짝"))
                        {
                            betNEvenMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("언"))
                        {
                            betNUnderMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("오"))
                        {
                            betNOverMoney += boxBetMoney;
                        }
                    }
                    else if (_find > 10)
                    {
                        if (textBoxPick.Text.Equals("홀"))
                        {
                            betPOddMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("짝"))
                        {
                            betPEvenMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("언"))
                        {
                            betPUnderMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("오"))
                        {
                            betPOverMoney += boxBetMoney;
                        }
                    }
                    else
                    {
                        if (textBoxPick.Text.Equals("홀"))
                        {
                            betPOddMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("짝"))
                        {
                            betPEvenMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("언"))
                        {
                            betPUnderMoney += boxBetMoney;
                        }
                        if (textBoxPick.Text.Equals("오"))
                        {
                            betPOverMoney += boxBetMoney;
                        }
                    }
                }
                powerballBetting();
            }
        }

        string trClass = string.Empty;
        string _powerball_oddeven = string.Empty;
        string _powerball_underover = string.Empty;
        string _normalball_oddeven = string.Empty;
        string _normalball_underover = string.Empty;

        string Reverse_Odd_PowerBallOddEven = string.Empty;
        string Reverse_Even_PowerBallOddEven = string.Empty;
        string Reverse_Odd_PowerBallUnderOver = string.Empty;
        string Reverse_Even_PowerBallUnderOver = string.Empty;

        string Reverse_Odd_NormalBallOddEven = string.Empty;
        string Reverse_Even_NormalBallOddEven = string.Empty;
        string Reverse_Odd_NormalBallUnderOver = string.Empty;
        string Reverse_Even_NormalBallUnderOver = string.Empty;

        int oddNumber = 1;
        int EvenNumber = 1;

        private void LoadPowerAllReultProcess(string returnMessage)
        {
            JObject jo = JObject.Parse(returnMessage);

            var content = jo.SelectToken("content");

            foreach (var contents in content)
            {
                trClass = contents.SelectToken("trClass").ToString();
                if (contents.SelectToken("powerballOddEven").ToString().Equals("odd"))
                {
                    _powerball_oddeven = "홀";
                }
                else if (contents.SelectToken("powerballOddEven").ToString().Equals("even"))
                {
                    _powerball_oddeven = "짝";
                }

                if (contents.SelectToken("powerballUnderOver").ToString().Equals("under"))
                {
                    _powerball_underover = "언";
                }
                else if (contents.SelectToken("powerballUnderOver").ToString().Equals("over"))
                {
                    _powerball_underover = "오";
                }

                if (contents.SelectToken("numberOddEven").ToString().Equals("odd"))
                {
                    _normalball_oddeven = "홀";
                }
                else if (contents.SelectToken("numberOddEven").ToString().Equals("even"))
                {
                    _normalball_oddeven = "짝";
                }

                if (contents.SelectToken("numberUnderOver").ToString().Equals("under"))
                {
                    _normalball_underover = "언";
                }
                else if (contents.SelectToken("numberUnderOver").ToString().Equals("over"))
                {
                    _normalball_underover = "오";
                }
                if (trClass.Equals("trOdd"))
                {
                    if (EvenNumber <= 12)
                    {
                        Reverse_Odd_PowerBallOddEven += _powerball_oddeven + "|";
                        Reverse_Odd_PowerBallUnderOver += _powerball_underover + "|";
                        Reverse_Odd_NormalBallOddEven += _normalball_oddeven + "|";
                        Reverse_Odd_NormalBallUnderOver += _normalball_underover + "|";

                        oddPowerBallOddEven.Append(_powerball_oddeven);
                        oddPowerBallUnderOver.Append(_powerball_underover);
                        oddNormalBallOddEven.Append(_normalball_oddeven);
                        oddNormalBallUnderOver.Append(_normalball_underover);
                    }
                    EvenNumber++;
                }
                else if (trClass.Equals("trEven"))
                {
                    if (oddNumber <= 12)
                    {
                        Reverse_Even_PowerBallOddEven += _powerball_oddeven + "|";
                        Reverse_Even_PowerBallUnderOver += _powerball_underover + "|";
                        Reverse_Even_NormalBallOddEven += _normalball_oddeven + "|";
                        Reverse_Even_NormalBallUnderOver += _normalball_underover + "|";

                        evenPowerBallOddEven.Append(_powerball_oddeven);
                        evenPowerBallUnderOver.Append(_powerball_underover);
                        evenNormalBallOddEven.Append(_normalball_oddeven);
                        evenNormalBallUnderOver.Append(_normalball_underover);
                    }
                    oddNumber++;
                }
            }
        }
        private void checkPatternString()
        {
            String PatternString = string.Empty;
            if (checkPowerBallOddEven.Checked)
            {
                CheckBox checkBoxStatus;
                RichTextBox textBoxPattern;
                TextBox textBoxEqual;
                TextBox oddEvenBet;
                ComboBox comboBoxLevel;

                for (int _find = 1; _find <= 10; _find++)
                {
                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        comboBoxLevel.Text = "0";
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }
                    if (checkBoxStatus.Checked)
                    {
                        oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);
                        textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                        PatternString = Reverse(textBoxPattern.Text);
                        textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                        if (nowAllInning % 2 == 0)
                        {
                            if (oddEvenBet.Text.Equals("1"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, evenPowerBallOddEven.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                        else
                        {
                            if (oddEvenBet.Text.Equals("0"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, oddPowerBallOddEven.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                    }
                }
            }
            if (checkPowerBallUnderOver.Checked)
            {
                CheckBox checkBoxStatus;
                RichTextBox textBoxPattern;
                TextBox textBoxEqual;
                TextBox oddEvenBet;
                ComboBox comboBoxLevel;
                for (int _find = 11; _find <= 20; _find++)
                {
                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        comboBoxLevel.Text = "0";
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }
                    textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                    PatternString = Reverse(textBoxPattern.Text);
                    textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                    if (checkBoxStatus.Checked)
                    {
                        oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);
                        if (nowAllInning % 2 == 0)
                        {
                            if (oddEvenBet.Text.Equals("1"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, evenPowerBallUnderOver.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                        else
                        {
                            if (oddEvenBet.Text.Equals("0"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, oddPowerBallUnderOver.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                    }
                }
            }
            if (checkNormalBallOddEven.Checked)
            {
                CheckBox checkBoxStatus;
                RichTextBox textBoxPattern;
                TextBox textBoxEqual;
                ComboBox comboBoxLevel;
                TextBox oddEvenBet;
                for (int _find = 21; _find <= 30; _find++)
                {
                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        comboBoxLevel.Text = "0";
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }
                    textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                    PatternString = Reverse(textBoxPattern.Text);
                    textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                    if (checkBoxStatus.Checked)
                    {
                        oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);
                        if (nowAllInning % 2 == 0)
                        {
                            if (oddEvenBet.Text.Equals("1"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, evenNormalBallOddEven.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                        else
                        {
                            if (oddEvenBet.Text.Equals("0"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, oddNormalBallOddEven.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                    }
                }
            }
            if (checkNormalBallUnderOver.Checked)
            {
                CheckBox checkBoxStatus;
                RichTextBox textBoxPattern;
                TextBox textBoxEqual;
                ComboBox comboBoxLevel;
                TextBox oddEvenBet;
                for (int _find = 31; _find <= 40; _find++)
                {
                    comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                    if (string.IsNullOrEmpty(comboBoxLevel.Text))
                    {
                        comboBoxLevel.Text = "0";
                    }
                    checkBoxStatus = (Controls.Find("checkBoxStatus" + _find.ToString(), true)[0] as CheckBox);
                    if (!checkBoxStatus.Checked)
                    {
                        continue;
                    }
                    textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                    PatternString = Reverse(textBoxPattern.Text);
                    textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                    if (checkBoxStatus.Checked)
                    {
                        oddEvenBet = (Controls.Find("oddEvenBet" + _find.ToString(), true)[0] as TextBox);
                        if (nowAllInning % 2 == 0)
                        {
                            if (oddEvenBet.Text.Equals("1"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, evenNormalBallUnderOver.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                        else
                        {
                            if (oddEvenBet.Text.Equals("0"))
                            {
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                continue;
                            }

                            if (wordCheck(PatternString, oddNormalBallUnderOver.ToString()))
                            {
                                textBoxEqual.ForeColor = Color.Black;
                                textBoxPattern.ForeColor = Color.White;
                                textBoxPattern.BackColor = Color.Black;
                                textBoxEqual.Text = "일치";
                                if (comboBoxLevel.Text.Equals("0"))
                                {
                                    comboBoxLevel.Text = "1";
                                }
                            }
                            else
                            {
                                textBoxEqual.ForeColor = Color.DarkGray;
                                textBoxPattern.ForeColor = Color.Black;
                                textBoxPattern.BackColor = Color.White;
                                textBoxEqual.Text = "불일치";
                            }
                        }
                    }
                }
            }
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        Random rand = new Random();
        int nonce = 0;
        private void powerballBetting()
        {
            if (BetProcessing)
            {
                return;
            }
            int a = betPOddMoney + betPEvenMoney + betPUnderMoney + betPOverMoney + betNOddMoney + betNEvenMoney + betNUnderMoney + betNOverMoney;

            if (a <= 0)
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();

            if (!ckbVirtureMoney.Checked)
            {
                stringBuilder.Append(UtilModel.BettingUrlAddress);
                stringBuilder.Append(":8082/api/bet");
                stringBuilder.AppendFormat("?userid={0}", UtilModel.UserId);
                stringBuilder.AppendFormat("&key={0}", UtilModel.ApiKey);
                stringBuilder.AppendFormat("&gm={0}", "PWB");
                stringBuilder.AppendFormat("&tdate={0}", DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                stringBuilder.AppendFormat("&rno={0}", nowAllInning);
                if (betPOddMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp1={0}", betPOddMoney);
                }
                if (betPEvenMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp2={0}", betPEvenMoney);
                }
                if (betPUnderMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp3={0}", betPUnderMoney);
                }
                if (betPOverMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp4={0}", betPOverMoney);
                }
                if (betNOddMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp5={0}", betNOddMoney);
                }
                if (betNEvenMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp6={0}", betNEvenMoney);
                }
                if (betNUnderMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp7={0}", betNUnderMoney);
                }
                if (betNOverMoney > 0)
                {
                    stringBuilder.AppendFormat("&pp8={0}", betNOverMoney);
                }
                stringBuilder.AppendFormat("&nonce={0}", nonce);
            }

            bettingLevel2(stringBuilder.ToString());

            if (bettingStatus)
            {
                String str = "";
                String str2 = "";
                if (betPOddMoney > 0)
                {
                    str += "[파홀 : " + UtilModel.StringFormatChanged(betPOddMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("파워");
                    item.SubItems.Add("홀");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betPOddMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betPEvenMoney > 0)
                {
                    str += "[파짝 : " + UtilModel.StringFormatChanged(betPEvenMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("파워");
                    item.SubItems.Add("짝");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betPEvenMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betPUnderMoney > 0)
                {
                    str += "[파언 : " + UtilModel.StringFormatChanged(betPUnderMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("파워");
                    item.SubItems.Add("언더");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betPUnderMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betPOverMoney > 0)
                {
                    str += "[파오 : " + UtilModel.StringFormatChanged(betPOverMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("파워");
                    item.SubItems.Add("오버");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betPOverMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betNOddMoney > 0)
                {
                    str2 += "[일홀 : " + UtilModel.StringFormatChanged(betNOddMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("일반");
                    item.SubItems.Add("홀");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betNOddMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betNEvenMoney > 0)
                {
                    str2 += "[일짝 : " + UtilModel.StringFormatChanged(betNEvenMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("일반");
                    item.SubItems.Add("짝");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betNEvenMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betNUnderMoney > 0)
                {
                    str2 += "[일언 : " + UtilModel.StringFormatChanged(betNUnderMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("일반");
                    item.SubItems.Add("언더");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betNUnderMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }
                if (betNOverMoney > 0)
                {
                    str2 += "[일오 : " + UtilModel.StringFormatChanged(betNOverMoney) + "원]";
                    ListViewItem item = new ListViewItem(nowAllInning.ToString()); // 픽스터 이름
                    item.SubItems.Add("일반");
                    item.SubItems.Add("오버");
                    item.SubItems.Add(UtilModel.StringFormatChanged(betNOverMoney));
                    item.SubItems.Add("--");
                    item.SubItems.Add("--");
                    bettingStatusListView.Items.Add(item);
                }

                if (str.Length > 1)
                {
                    txtLogAdd(str, Color.Black);
                }
                if (str2.Length > 1)
                {
                    txtLogAdd(str2, Color.Black);
                }
            }
        }

        public void bettingLevel2(String sb)
        {
            try
            {
                BetProcessing = true;
                bettingStatus = false;
                if (ckbVirtureMoney.Checked)
                {
                    betOwnMoney = betOwnMoney - (betPOddMoney + betPEvenMoney + betPUnderMoney + betPOverMoney + betNOddMoney + betNEvenMoney + betNUnderMoney + betNOverMoney);
                    lblOwnMoney.Text = UtilModel.StringFormatChanged(betOwnMoney);
                    lblNowProfit.Text = UtilModel.StringFormatChanged(betOwnMoney - betStartMoney);
                    txtLogAdd("[" + nowAllInning + "] 정상 배팅 등록 완료.", Color.Black);
                    logger.Info("[" + nowAllInning + "] 정상 배팅 등록 완료.");
                    logger.Info("[파홀 : " + betPOddMoney + "][파짝 : " + betPEvenMoney + "][파언 : " + betPUnderMoney + "][파오 : " + betPOverMoney + "][일홀 : " + betNOddMoney + "][일짝 : " + betNEvenMoney + "][일언 : " + betNUnderMoney + "][일오 : " + betNOverMoney + "]");
                    bettingStatus = true;
                }
                else
                {
                    String Message = null;
                    Boolean _BoolResult = false;

                    int CountResult = 0;
                    while (!_BoolResult)
                    {
                        try
                        {
                            //Uri myUri = new Uri(sb);
                            var returnMessage = UtilModel.MakeAsyncRequest(sb, "application/x-www-form-urlencoded; charset=UTF-8");
                            Message = returnMessage.Result;
                            logger.Info(Message);
                            if (Message.Contains("ret_code"))
                            {
                                _BoolResult = true;
                                continue;
                            }
                            else
                            {
                                CountResult++;
                                string errorMessage = "재배팅 시도 횟수 : [" + CountResult + "] 회";

                                txtLogAdd(errorMessage, Color.OrangeRed);
                                if (CountResult > 40)
                                {
                                    _BoolResult = true;
                                }
                                if (bettingTime < 25)
                                {
                                    _BoolResult = true;
                                }
                                UtilModel.Delay(1500);
                                continue;
                            }
                        }
                        catch (Exception _ex)
                        {
                            CountResult++;
                            string errorMessage = "재배팅 시도 횟수 : [" + CountResult + "] 회";

                            txtLogAdd(errorMessage, Color.OrangeRed);
                            if (CountResult > 40)
                            {
                                _BoolResult = true;
                            }
                            if (bettingTime < 25)
                            {
                                _BoolResult = true;
                            }
                            logger.Error(_ex.ToString());
                            UtilModel.Delay(1500);
                            continue;
                        }
                    }
                    bettingStatus = false;
                    if (!Message.Contains("ret_code"))
                    {
                        betPOddMoney = 0;
                        betPOddMoney = 0;
                        betPUnderMoney = 0;
                        betPOverMoney = 0;
                        betNOddMoney = 0;
                        betNEvenMoney = 0;
                        betNUnderMoney = 0;
                        betNOverMoney = 0;
                        string errorMessage = "[" + CountResult + "] 배팅 시도를 여러 차례 시도하였지만 실패하여 배팅이 등록되지 않았습니다.";
                        txtLogAdd(errorMessage, Color.White);
                        logger.Info(errorMessage);
                    }
                    else
                    {
                        JObject jo = JObject.Parse(Message);
                        int ret_code = int.Parse(jo.SelectToken("ret_code").ToString());
                        var ret_message = jo.SelectToken("comment").ToString();
                        if (ret_code == 1)
                        {
                            betOwnMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                            if (betStartMoney <= 0)
                            {
                                betStartMoney = betOwnMoney + betPOddMoney + betPEvenMoney + betPUnderMoney + betPOverMoney + betNOddMoney + betNEvenMoney + betNUnderMoney + betNOverMoney;
                            }
                            lblStartMoney.Text = UtilModel.StringFormatChanged(betStartMoney);
                            lblOwnMoney.Text = UtilModel.StringFormatChanged(betOwnMoney);
                            lblNowProfit.Text = UtilModel.StringFormatChanged(betOwnMoney - betStartMoney);
                            txtLogAdd("[" + nowAllInning + "] 정상 배팅 등록 완료.", Color.FromArgb(255, Color.FromArgb(0x42A5F5)));
                            logger.Info("[" + nowAllInning + "] 정상 배팅 등록 완료.");
                            String bettingMoneyString = "[배팅완료][" + nowAllInning + " 회][파홀:" + betPOddMoney + "][파짝:" + betPOddMoney + "][파언더:" + betPUnderMoney + "][파오버:" + betPOverMoney + "][일홀:" + betNOddMoney + "][일짝:" + betNEvenMoney + "][일언더:" + betNUnderMoney + "][일오버:" + betNOverMoney + "]";
                            logger.Info(bettingMoneyString);
                            bettingStatus = true;
                        }
                        else if (ret_code < 0)
                        {
                            txtLogAdd("배팅 실패 : " + ret_code + " : " + ret_message, Color.White);
                            MessageBox.Show("배팅 실패 : " + ret_message);
                            logger.Info(nowAllInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                        else
                        {
                            txtLogAdd("배팅 실패 : " + ret_code + " : " + ret_message, Color.White);
                            MessageBox.Show("배팅 실패 : " + ret_message);
                            logger.Info(nowAllInning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        private void getRemainingTime()
        {
            try
            {
                var returnMessage = UtilModel.MakeAsyncRequest("http://www.powerballgame.co.kr/?view=action&action=ajaxPowerballLog&actionType=remainTime&type=powerball", "application/x-www-form-urlencoded; charset=UTF-8");
                string Message = returnMessage.Result;
                logger.Info(Message);
                if (Message.Contains("remainTime"))
                {
                    JObject jo = JObject.Parse(Message);
                    bettingTime = int.Parse(jo.SelectToken("remainTime").ToString());
                }
                else
                {
                    returnMessage = UtilModel.MakeAsyncRequest("http://www.powerballgame.co.kr/?view=action&action=ajaxPowerballLog&actionType=remainTime&type=powerball", "application/x-www-form-urlencoded; charset=UTF-8");
                    Message = returnMessage.Result;
                    logger.Info(Message);
                    if (Message.Contains("remainTime"))
                    {
                        JObject jo = JObject.Parse(Message);
                        bettingTime = int.Parse(jo.SelectToken("remainTime").ToString());
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    logger.Info(((int)((HttpWebResponse)ex.Response).StatusCode));
                    logger.Info(ex.Message);
                }
                else
                {
                    logger.Info(ex.Message);
                }
            }

        }

        UserLogin userLogin;
        private void MainForm_Load(object sender, EventArgs e)
        {
            userLogin = new UserLogin();
            userLogin.loginEventHandler += new EventHandler(LoginSuccess);

            switch (userLogin.ShowDialog())
            {
                case DialogResult.OK:
                    userLogin.Close();
                    break;
                case DialogResult.Cancel:
                    Dispose();
                    break;
            }
            XMLLoadPropertiesSettings();

            UrlAddress.Text = UtilModel.BettingUrlAddress;
            ApiKeyValue.Text = UtilModel.ApiKey;
            UsernameTextBox.Text = UtilModel.UserId;

            this.Text += " | " + programVersion;
            this.Text += " | " + UtilModel.BettingUrlAddress;
            this.Text += " | " + UtilModel.UserId;

            nonce = rand.Next(1000, 9999);
            bettingTimer.Interval = 1000;
            bettingTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("정말로 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    bettingTimer.Stop();
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
        private void LoginSuccess(string name)
        {

        }
        private void btnBetOnOff_Click(object sender, EventArgs e)
        {
            if (!BetStartStatus)
            {
                groupBox2.Text = "현재 배팅 진행 중입니다.";
                BetStartStatus = true;
                BetClosed = false;
                _startDateTime = DateTime.Now;
                bettingTimer.Start();
                getRemainingTime();
                txtLogAdd("배팅이 시작되었습니다.", Color.Black);
            }
            else
            {
                groupBox2.Text = "현재 배팅 정지 중입니다.";
                BetStartStatus = false;
                bettingTimer.Stop();
                remainingTimeText.Text = "-1";
                lblElapsedTime.Text = "00 00:00:00";
                txtLogAdd("배팅이 종료되었습니다.", Color.Black);
            }

            RichTextBox textBoxPattern;
            TextBox textBoxEqual;
            ComboBox comboBoxLevel;

            for (int _find = 1; _find <= 40; _find++)
            {
                comboBoxLevel = (Controls.Find("comboBoxLevel" + _find.ToString(), true)[0] as ComboBox);
                comboBoxLevel.Text = "0";

                textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                textBoxPattern.ForeColor = Color.Black;
                textBoxPattern.BackColor = Color.White;

                textBoxEqual = (Controls.Find("textBoxEqual" + _find.ToString(), true)[0] as TextBox);
                textBoxEqual.ForeColor = Color.DarkGray;
            }
        }
        private void ckbVirtureMoney_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbVirtureMoney.Checked)
            {
                useVirtureMoney = true;
                betStartMoney = 10000000;
                betOwnMoney = 10000000;
                lblStartMoney.Text = "10,000,000";
                lblOwnMoney.Text = "10,000,000";
                lblNowProfit.Text = "0";
                txtLogAdd("배팅을 가상머니로 진행합니다.", Color.DarkBlue);
            }
            else
            {
                useVirtureMoney = false;
                betStartMoney = 0;
                betOwnMoney = 0;
                lblStartMoney.Text = "0";
                lblOwnMoney.Text = "0";
                lblNowProfit.Text = "0";
            }
        }
        private void directBetPowerBallResultProcessing()
        {
            for (int i = 0; i < bettingStatusListView.Items.Count; i++)
            {
                ListViewItem item = bettingStatusListView.Items[i];

                int betInning = int.Parse(item.SubItems[0].Text);

                if (betInning == (nowAllInning - 1))
                {
                    string pick = item.SubItems[2].Text;
                    string betMoney = item.SubItems[3].Text;
                    double outMoney = 0;
                    bool _bool = double.TryParse(Regex.Replace(betMoney, @"\D", ""), out outMoney);
                    if (item.SubItems[1].Text.Equals("일반"))
                    {
                        if (Result_Normalball_Oddeven.Equals(pick) || Result_Normalball_Underover.Equals(pick))
                        {
                            if (_bool)
                            {
                                int winMoney = (int)(outMoney * 1.95);
                                betOwnMoney += winMoney;
                                item.SubItems[4].Text = UtilModel.StringFormatChanged(winMoney);
                                lblOwnMoney.Text = UtilModel.StringFormatChanged(betOwnMoney);
                            }
                            item.SubItems[5].Text = "당첨";
                            item.SubItems[0].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[1].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[4].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                        }
                        else
                        {
                            item.SubItems[4].Text = "0";
                            item.SubItems[5].Text = "미당첨";
                            item.SubItems[0].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[1].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[4].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                        }
                    }
                    else if (item.SubItems[1].Text.Equals("파워"))
                    {
                        if (Result_Powerball_Oddeven.Equals(pick) || Result_Powerball_Underover.Equals(pick))
                        {
                            item.SubItems[5].Text = "당첨";
                            if (_bool)
                            {
                                int winMoney = (int)(outMoney * 1.95);
                                betOwnMoney += winMoney;
                                item.SubItems[4].Text = UtilModel.StringFormatChanged(winMoney);
                                lblOwnMoney.Text = UtilModel.StringFormatChanged(betOwnMoney);
                            }
                            item.SubItems[0].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[1].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[4].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                            item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xA1887F));
                        }
                        else
                        {
                            item.SubItems[5].Text = "미당첨";
                            item.SubItems[4].Text = "0";
                            item.SubItems[0].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[1].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[2].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[3].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[4].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                            item.SubItems[5].ForeColor = Color.FromArgb(255, Color.FromArgb(0xBDBDBD));
                        }
                    }
                }
            }
            lblNowProfit.Text = UtilModel.StringFormatChanged(betOwnMoney - betStartMoney);
        }

        private Boolean wordCheck(string PatternString, string checkString)
        {
            string[] Word = PatternString.Split(new char[] { '|' });
            for (int Wordnum = 0; Wordnum < Word.Length; Wordnum++)
            {
                if (checkString.IndexOf(Word[Wordnum]) == 0)
                {
                    return true;
                }
            }
            return false;
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

                    selectNode = SubNode.SelectSingleNode("pattern");
                    if (selectNode != null)
                    {
                        RichTextBox textBoxPattern;
                        string[] pattern = selectNode.InnerText.Split(new char[] { ',' });
                        for (int _find = 1; _find <= 40; _find++)
                        {
                            textBoxPattern = (Controls.Find("textBoxPattern" + _find.ToString(), true)[0] as RichTextBox);
                            textBoxPattern.Text = pattern[_find - 1];
                        }
                    }
                    selectNode = SubNode.SelectSingleNode("allpick");
                    if (selectNode != null)
                    {
                        TextBox textBoxAllPick;
                        string[] pattern = selectNode.InnerText.Split(new char[] { ',' });
                        for (int _find = 1; _find <= 40; _find++)
                        {
                            textBoxAllPick = (Controls.Find("textBoxAllPick" + _find.ToString(), true)[0] as TextBox);
                            textBoxAllPick.Text = pattern[_find - 1];
                        }
                    }
                    selectNode = SubNode.SelectSingleNode("moneySettings");
                    if (selectNode != null)
                    {
                        TextBox betMoneyLevel;
                        string[] pattern = selectNode.InnerText.Split(new char[] { ',' });
                        for (int _find = 1; _find <= 15; _find++)
                        {
                            betMoneyLevel = (Controls.Find("betMoneyLevel" + _find.ToString(), true)[0] as TextBox);
                            betMoneyLevel.Text = pattern[_find - 1];
                        }
                    }

                    selectNode = SubNode.SelectSingleNode("site");
                    if (selectNode != null)
                    {
                        UrlAddress.Text = selectNode.InnerText;
                    }
                    selectNode = SubNode.SelectSingleNode("userid");
                    if (selectNode != null)
                    {
                        UsernameTextBox.Text = selectNode.InnerText;
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        private void XMLModifierPropertiesSettings()
        {
            try
            {
                // xml 문서를 불러온다.
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("config.xml");

                // 첫노드를 잡아주고 하위 노드를 서냍ㄱ한다.
                XmlNode Firstnode = XmlDoc.DocumentElement;
                XmlElement SubNode = (XmlElement)Firstnode.SelectSingleNode("propertiesSettings");

                if (SubNode != null)
                {
                    XmlNode DeleteNode;

                    DeleteNode = SubNode.SelectSingleNode("pattern");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(
                        CreateNode(
                        XmlDoc,
                        "pattern",
                        string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39}",
                        textBoxPattern1.Text, textBoxPattern2.Text, textBoxPattern3.Text, textBoxPattern4.Text, textBoxPattern5.Text, textBoxPattern6.Text, textBoxPattern7.Text, textBoxPattern8.Text, textBoxPattern9.Text, textBoxPattern10.Text,
                         textBoxPattern11.Text, textBoxPattern12.Text, textBoxPattern13.Text, textBoxPattern14.Text, textBoxPattern15.Text, textBoxPattern16.Text, textBoxPattern17.Text, textBoxPattern18.Text, textBoxPattern19.Text, textBoxPattern20.Text,
                          textBoxPattern21.Text, textBoxPattern22.Text, textBoxPattern23.Text, textBoxPattern24.Text, textBoxPattern25.Text, textBoxPattern26.Text, textBoxPattern27.Text, textBoxPattern28.Text, textBoxPattern29.Text, textBoxPattern30.Text,
                           textBoxPattern31.Text, textBoxPattern32.Text, textBoxPattern33.Text, textBoxPattern34.Text, textBoxPattern35.Text, textBoxPattern36.Text, textBoxPattern37.Text, textBoxPattern38.Text, textBoxPattern39.Text, textBoxPattern40.Text
                                )
                            )
                        );

                    DeleteNode = SubNode.SelectSingleNode("allpick");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(
                        CreateNode(
                        XmlDoc,
                        "allpick",
                        string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39}",
                        textBoxAllPick1.Text, textBoxAllPick2.Text, textBoxAllPick3.Text, textBoxAllPick4.Text, textBoxAllPick5.Text, textBoxAllPick6.Text, textBoxAllPick7.Text, textBoxAllPick8.Text, textBoxAllPick9.Text, textBoxAllPick10.Text,
                         textBoxAllPick11.Text, textBoxAllPick12.Text, textBoxAllPick13.Text, textBoxAllPick14.Text, textBoxAllPick15.Text, textBoxAllPick16.Text, textBoxAllPick17.Text, textBoxAllPick18.Text, textBoxAllPick19.Text, textBoxAllPick20.Text,
                          textBoxAllPick21.Text, textBoxAllPick22.Text, textBoxAllPick23.Text, textBoxAllPick24.Text, textBoxAllPick25.Text, textBoxAllPick26.Text, textBoxAllPick27.Text, textBoxAllPick28.Text, textBoxAllPick29.Text, textBoxAllPick30.Text,
                           textBoxAllPick31.Text, textBoxAllPick32.Text, textBoxAllPick33.Text, textBoxAllPick34.Text, textBoxAllPick35.Text, textBoxAllPick36.Text, textBoxAllPick37.Text, textBoxAllPick38.Text, textBoxAllPick39.Text, textBoxAllPick40.Text
                                )
                            )
                        );

                    DeleteNode = SubNode.SelectSingleNode("moneySettings");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "moneySettings", string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                        UtilModel.RegexOnlyNumber(betMoneyLevel1.Text), UtilModel.RegexOnlyNumber(betMoneyLevel2.Text), UtilModel.RegexOnlyNumber(betMoneyLevel3.Text),
                        UtilModel.RegexOnlyNumber(betMoneyLevel4.Text), UtilModel.RegexOnlyNumber(betMoneyLevel5.Text), UtilModel.RegexOnlyNumber(betMoneyLevel6.Text),
                        UtilModel.RegexOnlyNumber(betMoneyLevel7.Text), UtilModel.RegexOnlyNumber(betMoneyLevel8.Text), UtilModel.RegexOnlyNumber(betMoneyLevel9.Text),
                        UtilModel.RegexOnlyNumber(betMoneyLevel10.Text), UtilModel.RegexOnlyNumber(betMoneyLevel11.Text), UtilModel.RegexOnlyNumber(betMoneyLevel12.Text)
                        , UtilModel.RegexOnlyNumber(betMoneyLevel13.Text)
                        , UtilModel.RegexOnlyNumber(betMoneyLevel14.Text)
                        , UtilModel.RegexOnlyNumber(betMoneyLevel15.Text)
                        )
                        )
                        );

                    DeleteNode = SubNode.SelectSingleNode("site");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "site", UrlAddress.Text));

                    DeleteNode = SubNode.SelectSingleNode("userid");
                    if (DeleteNode != null)
                    {
                        SubNode.RemoveChild(DeleteNode);
                    }
                    SubNode.AppendChild(CreateNode(XmlDoc, "userid", UsernameTextBox.Text));

                    XmlDoc.Save("config.xml");
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

        public void txtLogAdd(string str, Color _color)
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

        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(bettingTiemr_Tick));
        }

        private void betMoneyLevel1_TextChanged(object sender, EventArgs e)
        {
            TextBox moneyUpText = sender as TextBox;
            string lgsText;

            lgsText = Regex.Replace(moneyUpText.Text, @"\D", "");

            if (!string.IsNullOrEmpty(lgsText))
            {
                int money = Convert.ToInt32(lgsText);

                moneyUpText.Text = String.Format("{0:#,##0}", money);

                moneyUpText.SelectionStart = moneyUpText.TextLength;

                moneyUpText.SelectionLength = 0;
            }
        }
        private void levelIndexChange(object sender, TextBox textBox, TextBox allPick, TextBox pick)
        {
            ComboBox comboBox = sender as ComboBox;
            int num = int.Parse(comboBox.Text);

            if (num == 0)
            {
                pick.Text = "통과";
                pick.ForeColor = Color.DarkGray;
            }
            else
            {
                string[] allPickSplit = allPick.Text.Split(new char[] { '|' });
                if (allPickSplit.Length >= num)
                {
                    string sub = allPickSplit[num - 1];
                    if (sub.Equals("홀") || sub.Equals("짝") || sub.Equals("언") || sub.Equals("오"))
                    {
                        pick.Text = sub;
                        pick.ForeColor = Color.Black;
                    }
                    else
                    {
                        pick.Text = "통과";
                        pick.ForeColor = Color.DarkGray;
                    }
                }
                else
                {
                    pick.Text = "통과";
                    pick.ForeColor = Color.DarkGray;
                }
            }
            switch (num)
            {
                case 0:
                    textBox.Text = "0";
                    textBox.ForeColor = Color.DarkGray;
                    break;
                case 1:
                    textBox.Text = betMoneyLevel1.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 2:
                    textBox.Text = betMoneyLevel2.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 3:
                    textBox.Text = betMoneyLevel3.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 4:
                    textBox.Text = betMoneyLevel4.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 5:
                    textBox.Text = betMoneyLevel5.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 6:
                    textBox.Text = betMoneyLevel6.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 7:
                    textBox.Text = betMoneyLevel7.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 8:
                    textBox.Text = betMoneyLevel8.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 9:
                    textBox.Text = betMoneyLevel9.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 10:
                    textBox.Text = betMoneyLevel10.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 11:
                    textBox.Text = betMoneyLevel11.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 12:
                    textBox.Text = betMoneyLevel12.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 13:
                    textBox.Text = betMoneyLevel13.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 14:
                    textBox.Text = betMoneyLevel14.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                case 15:
                    textBox.Text = betMoneyLevel15.Text;
                    textBox.ForeColor = Color.Black;
                    break;
                default:
                    textBox.Text = "0";
                    textBox.ForeColor = Color.DarkGray;
                    pick.Text = "통과";
                    pick.ForeColor = Color.DarkGray;
                    break;
            }

            if (num > 1)
            {
                if (textBox.Text.Equals("0"))
                {
                    comboBox.Text = "1";
                }
            }
        }
        private void comboBoxLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney1, textBoxAllPick1, textBoxPick1);
        }

        private void comboBoxLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney2, textBoxAllPick2, textBoxPick2);
        }

        private void comboBoxLevel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney3, textBoxAllPick3, textBoxPick3);
        }

        private void comboBoxLevel4_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney4, textBoxAllPick4, textBoxPick4);
        }

        private void comboBoxLevel5_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney5, textBoxAllPick5, textBoxPick5);
        }

        private void comboBoxLevel6_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney6, textBoxAllPick6, textBoxPick6);
        }

        private void comboBoxLevel7_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney7, textBoxAllPick7, textBoxPick7);
        }

        private void comboBoxLevel8_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney8, textBoxAllPick8, textBoxPick8);
        }

        private void comboBoxLevel9_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney9, textBoxAllPick9, textBoxPick9);
        }

        private void comboBoxLevel10_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney10, textBoxAllPick10, textBoxPick10);
        }

        private void comboBoxLevel11_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney11, textBoxAllPick11, textBoxPick11);
        }

        private void comboBoxLevel12_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney12, textBoxAllPick12, textBoxPick12);
        }

        private void comboBoxLevel13_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney13, textBoxAllPick13, textBoxPick13);
        }

        private void comboBoxLevel14_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney14, textBoxAllPick14, textBoxPick14);
        }

        private void comboBoxLevel15_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney15, textBoxAllPick15, textBoxPick15);
        }

        private void comboBoxLevel16_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney16, textBoxAllPick16, textBoxPick16);
        }

        private void comboBoxLevel17_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney17, textBoxAllPick17, textBoxPick17);
        }

        private void comboBoxLevel18_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney18, textBoxAllPick18, textBoxPick18);
        }

        private void comboBoxLevel19_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney19, textBoxAllPick19, textBoxPick19);
        }

        private void comboBoxLevel20_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney20, textBoxAllPick20, textBoxPick20);
        }

        private void comboBoxLevel21_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney21, textBoxAllPick21, textBoxPick21);
        }

        private void comboBoxLevel22_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney22, textBoxAllPick22, textBoxPick22);
        }

        private void comboBoxLevel23_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney23, textBoxAllPick23, textBoxPick23);
        }

        private void comboBoxLevel24_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney24, textBoxAllPick24, textBoxPick24);
        }

        private void comboBoxLevel25_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney25, textBoxAllPick25, textBoxPick25);
        }

        private void comboBoxLevel26_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney26, textBoxAllPick26, textBoxPick26);
        }

        private void comboBoxLevel27_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney27, textBoxAllPick27, textBoxPick27);
        }

        private void comboBoxLevel28_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney28, textBoxAllPick28, textBoxPick28);
        }

        private void comboBoxLevel29_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney29, textBoxAllPick29, textBoxPick29);
        }

        private void comboBoxLevel30_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney30, textBoxAllPick30, textBoxPick30);
        }

        private void comboBoxLevel31_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney31, textBoxAllPick31, textBoxPick31);
        }

        private void comboBoxLevel32_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney32, textBoxAllPick32, textBoxPick32);
        }

        private void comboBoxLevel33_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney33, textBoxAllPick33, textBoxPick33);
        }

        private void comboBoxLevel34_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney34, textBoxAllPick34, textBoxPick34);
        }

        private void comboBoxLevel35_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney35, textBoxAllPick35, textBoxPick35);
        }

        private void comboBoxLevel36_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney36, textBoxAllPick36, textBoxPick36);
        }

        private void comboBoxLevel37_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney37, textBoxAllPick37, textBoxPick37);
        }

        private void comboBoxLevel38_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney38, textBoxAllPick38, textBoxPick38);
        }

        private void comboBoxLevel39_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney39, textBoxAllPick39, textBoxPick39);
        }

        private void comboBoxLevel40_SelectedIndexChanged(object sender, EventArgs e)
        {
            levelIndexChange(sender, textBoxBetMoney40, textBoxAllPick40, textBoxPick40);
        }

        private void checkBoxStatusChanged(object sender, RichTextBox pattern, TextBox Allpick)
        {
            CheckBox status = sender as CheckBox;
            if (status.Checked)
            {
                pattern.ReadOnly = true;
                Allpick.ReadOnly = true;
            }
            else
            {
                pattern.ReadOnly = false;
                Allpick.ReadOnly = false;
            }
        }
        private void checkBoxStatus1_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern1, textBoxAllPick1);
        }

        private void checkBoxStatus2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern2, textBoxAllPick2);
        }

        private void checkBoxStatus3_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern3, textBoxAllPick3);
        }

        private void checkBoxStatus4_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern4, textBoxAllPick4);
        }

        private void checkBoxStatus5_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern5, textBoxAllPick5);
        }

        private void checkBoxStatus6_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern6, textBoxAllPick6);
        }

        private void checkBoxStatus7_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern7, textBoxAllPick7);
        }

        private void checkBoxStatus8_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern8, textBoxAllPick8);
        }

        private void checkBoxStatus9_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern9, textBoxAllPick9);
        }

        private void checkBoxStatus10_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern10, textBoxAllPick10);
        }

        private void checkBoxStatus11_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern11, textBoxAllPick11);
        }

        private void checkBoxStatus12_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern12, textBoxAllPick12);
        }

        private void checkBoxStatus13_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern13, textBoxAllPick13);
        }

        private void checkBoxStatus14_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern14, textBoxAllPick14);
        }

        private void checkBoxStatus15_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern15, textBoxAllPick15);
        }

        private void checkBoxStatus16_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern16, textBoxAllPick16);
        }

        private void checkBoxStatus17_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern17, textBoxAllPick17);
        }

        private void checkBoxStatus18_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern18, textBoxAllPick18);
        }

        private void checkBoxStatus19_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern19, textBoxAllPick19);
        }

        private void checkBoxStatus20_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern20, textBoxAllPick20);
        }

        private void checkBoxStatus21_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern21, textBoxAllPick21);
        }

        private void checkBoxStatus22_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern22, textBoxAllPick22);
        }

        private void checkBoxStatus23_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern23, textBoxAllPick23);
        }

        private void checkBoxStatus24_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern24, textBoxAllPick24);
        }

        private void checkBoxStatus25_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern25, textBoxAllPick25);
        }

        private void checkBoxStatus26_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern26, textBoxAllPick26);
        }

        private void checkBoxStatus27_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern27, textBoxAllPick27);
        }

        private void checkBoxStatus28_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern28, textBoxAllPick28);
        }

        private void checkBoxStatus29_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern29, textBoxAllPick29);
        }

        private void checkBoxStatus30_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern30, textBoxAllPick30);
        }

        private void checkBoxStatus31_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern31, textBoxAllPick31);
        }

        private void checkBoxStatus32_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern32, textBoxAllPick32);
        }

        private void checkBoxStatus33_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern33, textBoxAllPick33);
        }

        private void checkBoxStatus34_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern34, textBoxAllPick34);
        }

        private void checkBoxStatus35_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern35, textBoxAllPick35);
        }

        private void checkBoxStatus36_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern36, textBoxAllPick36);
        }

        private void checkBoxStatus37_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern37, textBoxAllPick37);
        }

        private void checkBoxStatus38_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern38, textBoxAllPick38);
        }

        private void checkBoxStatus39_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern39, textBoxAllPick39);
        }

        private void checkBoxStatus40_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatusChanged(sender, textBoxPattern40, textBoxAllPick40);
        }
    }
}
