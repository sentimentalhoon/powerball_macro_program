using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace JY_PowerBallProgram
{
    public partial class MainForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainForm()
        {
            InitializeComponent();
            FormClosing += MainForm_FormClosing;
        }
        LoginForm loginForm;
        protected virtual bool DoubleBuffered { get; set; }
        private void MainForm_Load(object sender, EventArgs e)
        {
            loginForm = new LoginForm();
            loginForm.loginEventHandler += new EventHandler(LoginSuccess);

            switch (loginForm.ShowDialog())
            {
                case DialogResult.OK:
                    loginForm.Close();
                    break;
                case DialogResult.Cancel:
                    Dispose();
                    break;
            }
            listView1.DoubleBuffered(true);

            this.Text += " : " + UtilModel._userSite;
            this.Text += " : " + UtilModel.betid;
            this.Text += " : " + UtilModel._userprofile;
            this.Text += " : " + UtilModel._programVersion;
            this.Text += " : " + UtilModel._limittime;
            txtSwitchMode.Text = "수동모드";

            if (UtilModel._allBettingEnable == 1)
            {
                this.label9.Text = "총 배팅금 :";
                this.label10.Visible = true;
            }

            String folderPath = @"./screenshot";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            folderPath = @"./Log";
            di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }

            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록

            randNonce = rand.Next(1000000, 9999999);
            int _patternRandomNumber = rand.Next(0, 5);
            if (_patternRandomNumber == 0)
            {
                patternNumber.Text = "패턴 1번";
            }
            else if (_patternRandomNumber == 1)
            {
                patternNumber.Text = "패턴 2번";
            }
            else if (_patternRandomNumber == 2)
            {
                patternNumber.Text = "패턴 3번";
            }
            else if (_patternRandomNumber == 3)
            {
                patternNumber.Text = "패턴 4번";
            }
            else if (_patternRandomNumber == 4)
            {
                patternNumber.Text = "패턴 5번";
            }
            else if (_patternRandomNumber == 5)
            {
                patternNumber.Text = "패턴 6번";
            }
        }

        int startMoney = 0;
        int NowMoney = 0;

        // int _r = 0;
        int randNonce = 0;
        String nickName;
        Boolean _getStartMoney = false;
        /// <summary> 
        /// 비프음을 내는 시스템 함수
        /// </summary>
        /// <param name="freq">주파수</param>
        /// <param name="dur">비프음 길이(시간, 단위 : 1000 = 1초)</param>
        [DllImport("KERNEL32.DLL")]
        extern public static void Beep(int freq, int dur);

        private void LoginSuccess(string name)
        {
            nickName = name;
            MessageBox.Show(nickName + "님 반갑습니다.\r\n\r\n해당 프로그램은 고객님의 배팅에 \r\n\r\n도움을 주기 위해 만들어진 프로그램입니다. \r\n\r\n해당 프로그램을 맹신하지 말아주시기 바랍니다." +
                "\r\n\r\n프로그램 만료일 : " + UtilModel._limittime,
                            nickName + "님 반갑습니다.",
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
                    Properties.Settings.Default.Save();
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

        int accountNumber = 40;
        int allinning = 0;
        int todayinning = 0;
        int resultAllInning = 0;
        //int resultTodayInning = 0;
        int powerball = 0;
        int normalball = 0;
        string powerballOddEven = null;
        string powerballUnderOver = null;
        string normalballOddEven = null;
        string normalballUnderOver = null;
        string powerBallLadderLeftRight = null;
        string powerBallLadder34 = null;
        string powerBallLadderOddEven = null;
        int pOverMoney = 0; // 파워볼 오버
        int pUnderMoney = 0; // 파워볼 언더
        int pOddMoney = 0; // 파워볼 홀
        int pEvenMoney = 0; // 파워볼 짝
        int nOverMoney = 0;
        int nUnderMoney = 0;
        int nOddMoney = 0;
        int nEvenMoney = 0;
        int ladderOddMoney = 0;
        int ladderEvenMoney = 0;
        int ladder3Money = 0;
        int ladder4Money = 0;
        int ladderLeftMoney = 0;
        int ladderRightMoney = 0;

        Double poweroddPercent = 0;
        Double powerevenPercent = 0;
        Double poweroverPercent = 0;
        Double powerunderPercent = 0;

        Double normaloddPercent = 0;
        Double normalevenPercent = 0;
        Double normaloverPercent = 0;
        Double normalunderPercent = 0;

        int allbettingMoney;
        Boolean _isStart = false;

        int loadPickster_min = 10001;
        int loadPickster_max = 10100;

        int startInning = 0;
        Random rand = new Random();
        // 결과 뽑아오는 부분
        private void powerballResult()
        {
            try
            {
                string url = UtilModel.powerresult;                                    // 통신할 URL
                string msg = string.Format("inning={0}&username={1}&password={2}&timetoken={3}", (allinning - 1), UtilModel.betid, UtilModel._password, UtilModel._timetoken); // 전송할 Parameter
                string encodeStr = "UTF-8";                                          // 인코딩 방식
                int errorcode = 0;                                                     // 에러 전달받을 값
                String _result = "";
                Boolean _BoolResult = false;
                int CountResult = 0;
                while (!_BoolResult)
                {
                    String returnMessage = UtilModel.GetHttpPOST(msg, url, "POST", encodeStr, ref errorcode);
                    logger.Info(returnMessage);
                    if (returnMessage.Contains("result"))
                    {
                        JObject jo = JObject.Parse(returnMessage);
                        _result = jo.SelectToken("result").ToString();
                        if (_result.Equals("OK"))
                        {
                            resultAllInning = int.Parse(jo.SelectToken("inning").ToString());
                            powerball = int.Parse(jo.SelectToken("powerball").ToString());
                            normalball = int.Parse(jo.SelectToken("normalball").ToString());
                            powerballUnderOver = jo.SelectToken("powerball_underover").ToString();
                            powerballOddEven = jo.SelectToken("powerball_oddeven").ToString();
                            normalballUnderOver = jo.SelectToken("normalball_underover").ToString();
                            normalballOddEven = jo.SelectToken("normalball_oddeven").ToString();
                            powerBallLadderLeftRight = jo.SelectToken("firstball_1").ToString();
                            powerBallLadder34 = jo.SelectToken("firstball_2").ToString();
                            powerBallLadderOddEven = jo.SelectToken("firstball_3").ToString();

                            resultRound.Text = resultAllInning + "회";
                            resultPowerBallNumber.Text = powerball.ToString();
                            resultPowerBallOddEven.Text = powerballOddEven;
                            resultPowerballUnderOver.Text = powerballUnderOver;
                            resultNormalBallNumber.Text = normalball.ToString();
                            resultNormalBallOddEven.Text = normalballOddEven;
                            resultNormalBallUnderOver.Text = normalballUnderOver;
                            resultPowerLadder34.Text = powerBallLadder34;
                            resultPowerLadderLeftRight.Text = powerBallLadderLeftRight;
                            resultPowerLadderOddEven.Text = powerBallLadderOddEven;

                            if (remainingTime < 280)
                            {
                                txtLogAdd("파워볼 결과 : [" + resultAllInning + "회][" + powerball + "][" + powerballOddEven + "][" + powerballUnderOver + "]", Color.FromArgb(100, 181, 246));
                                txtLogAdd("일반볼 결과 : [" + resultAllInning + "회][" + normalball + "][" + normalballOddEven + "][" + normalballUnderOver + "]", Color.FromArgb(100, 181, 246));
                                txtLogAdd("파워사다리 결과 : [" + resultAllInning + "회][" + powerBallLadderLeftRight + "][" + powerBallLadder34 + "][" + powerBallLadderOddEven + "]", Color.FromArgb(100, 181, 246));
                                logger.Info(resultAllInning + " / "
                                             + " / 파워볼 : " + powerball + " / " + powerballOddEven + " / " + powerballUnderOver + " / "
                                                + " 일반볼/ " + normalball + " / " + normalballOddEven + " / " + normalballUnderOver);
                            }
                        }
                        _BoolResult = true;
                    }
                    else
                    {
                        CountResult++;
                        if (CountResult > 5)
                        {
                            _BoolResult = true;
                        }
                    }
                }

                if (CountResult > 5)
                {
                    string errorMessage = "회차값 오류로 인해 결과값 불러오는 데 실패하였습니다. 프로그램을 정지합니다.";
                    txtLogAdd(errorMessage, Color.White);
                    MessageBox.Show(errorMessage);
                    logger.Info(errorMessage);
                    _isStart = false;
                    timer.Stop();
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        // 전체, 당일, 남은 시간
        private void getPowerballInformation()
        {
            try
            {
                String _uri = UtilModel.servertime;
                _uri += "?username=" + UtilModel.betid;
                _uri += "&password=" + UtilModel._password;
                _uri += "&timetoken=" + UtilModel._timetoken;
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("item"))
                {
                    remainingTime = int.Parse(item.Attribute("timediffer").Value);
                    allinning = int.Parse(item.Attribute("allinning").Value);
                    todayinning = int.Parse(item.Attribute("todayinning").Value);
                }
                round.Text = allinning + "회";
                logger.Info("[전체회차 : " + allinning + " ][일일 회차 " + todayinning + "]");
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        int _powerballEven = 0; // 짝수
        int _powerballOdd = 0; // 홀수
        int _powerballUnder = 0; // 언더
        int _powerballOver = 0; // 오버

        int _normalballEven = 0; // 짝수
        int _normalballOdd = 0; // 홀수
        int _normalballUnder = 0; // 언더
        int _normalballOver = 0; // 오버
                                 // 픽스터 정보를 뽑아와서 배열에 저장 후 listview 에 뿌려주는 부분

        int picksternumber = 0;
        String loadPicksterType = null;
        Boolean _picksterListLoad = true;
        List<string> winPicksterName;
        List<string> losePicksterName;

        void arrayListView()
        {
            try
            {
                winPicksterName = new List<string>();
                losePicksterName = new List<string>();
                picksternumber = 0;
                _picksterInformation = new string[arrayNum, array2Num];

                if (systemPickUseCheck.Checked)
                {
                    loadPicksterType = "로봇";
                }
                else if (userPickNtryUseCheck.Checked)
                {
                    loadPicksterType = "ntry";
                }
                else if (userPickPowerBallGameUseCheck.Checked)
                {
                    loadPicksterType = "pbg";
                }
                else if (GodPickUseCheck.Checked)
                {
                    loadPicksterType = "갓픽";
                }
                else if (PatternPickUse.Checked)
                {
                    loadPicksterType = "pattern1";
                    if (patternNumber.Text.Contains("1"))
                    {
                        loadPicksterType = "pattern1";
                    }
                    else if (patternNumber.Text.Contains("2"))
                    {
                        loadPicksterType = "pattern2";
                    }
                    else if (patternNumber.Text.Contains("3"))
                    {
                        loadPicksterType = "pattern3";
                    }
                    else if (patternNumber.Text.Contains("4"))
                    {
                        loadPicksterType = "pattern4";
                    }
                    else if (patternNumber.Text.Contains("5"))
                    {
                        loadPicksterType = "pattern5";
                    }
                    else if (patternNumber.Text.Contains("6"))
                    {
                        loadPicksterType = "pattern6";
                    }
                    else
                    {
                        loadPicksterType = "pattern1";
                    }
                }
                string page = string.Format("username={0}&password={1}&timetoken={2}&type={3}&min={4}&max={5}", UtilModel.betid, UtilModel._password, UtilModel._timetoken, loadPicksterType, loadPickster_min, loadPickster_max);// 전송할 Parameter

                string encodeStr = "UTF-8";                                          // 인코딩 방식
                int errorcode = 0;                                                     // 에러 전달받을 값
                string returnVal = "";
                returnVal = UtilModel.GetHttpPOST(page, UtilModel.picksterlist, "POST", encodeStr, ref errorcode);
                JObject jo = JObject.Parse(returnVal);
                String pp = "P";
                String pickstername = "";
                String pbrecord = "";
                String pbstreak = "";
                String site = "";
                String ballType = "";
                String record = "";

                var a = jo.SelectToken("result");
                foreach (var item in a)
                {
                    pbstreak = "";
                    pbrecord = "";
                    ballType = "";
                    record = "";

                    pickstername = item.SelectToken("pickstername").ToString();

                    site = item.SelectToken("site").ToString();

                    string[] powerballPick = new string[30];
                    for (int i = 0; i < 30; i++)
                    {
                        powerballPick[i] = item.SelectToken("pb" + (i + 1)).ToString();
                    }
                    pp = item.SelectToken("pp").ToString();
                    if (site.Contains("로봇") || site.Contains("pattern") || site.Contains("갓픽"))
                    {
                        int winstreak = 0;
                        int losestreak = 0;
                        int winnum = 0;
                        int losenum = 0;
                        if (site.Contains("pattern"))
                        {
                            site = "패턴";
                        }
                        for (int i = 0; i < 30; i++)
                        {
                            if (powerballPick[i].Contains("O"))
                            {
                                winnum++;
                            }
                            else if (powerballPick[i].Contains("X"))
                            {
                                losenum++;
                            }
                        }

                        record = winnum + "승" + losenum + "패";

                        if (powerballPick[0].Contains("O"))
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                if (string.IsNullOrEmpty(powerballPick[i]))
                                {
                                    break;
                                }
                                if (powerballPick[i].Contains("O"))
                                {
                                    winstreak++;
                                }
                                else if (powerballPick[i].Contains("P"))
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            pbstreak = winstreak + "연승";

                            int pCR = int.Parse(picksterChangeRound.Text);
                            if (pCR > 0)
                            {
                                int _streakNumber = int.Parse(streakNumber.Text);
                                if (winstreak >= _streakNumber)
                                {
                                    winPicksterName.Add(pickstername);
                                }
                                else
                                {
                                    if (checkBoxPicksterMark.Checked)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        else if (powerballPick[0].Contains("X"))
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                if (string.IsNullOrEmpty(powerballPick[i]))
                                {
                                    break;
                                }
                                if (powerballPick[i].Contains("X"))
                                {
                                    losestreak++;
                                }
                                else if (powerballPick[i].Contains("P"))
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            pbstreak = losestreak + "연패";

                            int pCR = int.Parse(picksterChangeRound.Text);
                            if (pCR > 0)
                            {
                                int _streakNumber = int.Parse(streakNumber.Text);
                                if (losestreak >= _streakNumber)
                                {
                                    losePicksterName.Add(pickstername);
                                }
                                else
                                {
                                    if (checkBoxPicksterMark.Checked)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (powerballPick[1].Contains("O"))
                            {
                                for (int i = 1; i < 30; i++)
                                {
                                    if (string.IsNullOrEmpty(powerballPick[i]))
                                    {
                                        break;
                                    }
                                    if (powerballPick[i].Contains("O"))
                                    {
                                        winstreak++;
                                    }
                                    else if (powerballPick[i].Contains("P"))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                pbstreak = winstreak + "연승";

                                int pCR = int.Parse(picksterChangeRound.Text);
                                if (pCR > 0)
                                {
                                    int _streakNumber = int.Parse(streakNumber.Text);
                                    if (winstreak >= _streakNumber)
                                    {
                                        winPicksterName.Add(pickstername);
                                    }
                                    else
                                    {
                                        if (checkBoxPicksterMark.Checked)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            else if (powerballPick[1].Contains("X"))
                            {
                                for (int i = 1; i < 30; i++)
                                {
                                    if (string.IsNullOrEmpty(powerballPick[i]))
                                    {
                                        break;
                                    }
                                    if (powerballPick[i].Contains("X"))
                                    {
                                        losestreak++;
                                    }
                                    else if (powerballPick[i].Contains("P"))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                pbstreak = losestreak + "연패";

                                int pCR = int.Parse(picksterChangeRound.Text);
                                if (pCR > 0)
                                {
                                    int _streakNumber = int.Parse(streakNumber.Text);
                                    if (losestreak >= _streakNumber)
                                    {
                                        losePicksterName.Add(pickstername);
                                    }
                                    else
                                    {
                                        if (checkBoxPicksterMark.Checked)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (!includeMemberId.Checked) // 회원 이라는 글자가 포함된 픽스터는 제외한다.
                        {
                            if (pickstername.Contains("회원"))
                            {
                                continue;
                            }
                        }
                        pbrecord = item.SelectToken("pbrecord").ToString();
                        if (pbrecord.Contains("패"))
                        {
                            record = pbrecord;
                        }
                        else
                        {
                            if (pbrecord.Contains("승"))
                            {
                                int lose = 100 - (int.Parse(pbrecord.Replace("승", "")));
                                record = pbrecord + lose + "패";
                            }
                            else
                            {
                                record = "0승0패";
                            }
                        }
                        pbstreak = item.SelectToken("pbstreak").ToString();
                        int pCR = int.Parse(picksterChangeRound.Text);

                        if (pCR > 0)
                        {
                            if (pp.Contains("P"))
                            {
                                continue;
                            }
                            int winstreak = 0;
                            int losestreak = 0;
                            if (pbstreak.Contains("패"))
                            {
                                int _streakNumber = int.Parse(streakNumber.Text);
                                losestreak = int.Parse(Regex.Replace(pbstreak, @"\D", ""));
                                if (losestreak >= _streakNumber)
                                {
                                    losePicksterName.Add(pickstername);
                                }
                                else
                                {
                                    if (checkBoxPicksterMark.Checked)
                                    {
                                        continue;
                                    }
                                }
                            }
                            else if (pbstreak.Contains("승"))
                            {
                                int _streakNumber = int.Parse(streakNumber.Text);
                                winstreak = int.Parse(Regex.Replace(pbstreak, @"\D", ""));
                                if (winstreak >= _streakNumber)
                                {
                                    winPicksterName.Add(pickstername);
                                }
                                else
                                {
                                    if (checkBoxPicksterMark.Checked)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    if (!pp.Contains("P"))
                    {
                        picksternumber++;
                    }

                    if (pp.Contains("파"))
                    {
                        ballType = "파워";
                        if (pp.Contains("홀"))
                        {
                            _powerballOdd++;
                        }
                        else if (pp.Contains("짝"))
                        {
                            _powerballEven++;
                        }
                        else if (pp.Contains("오버"))
                        {
                            _powerballOver++;
                        }
                        else if (pp.Contains("언더"))
                        {
                            _powerballUnder++;
                        }
                    }
                    else if (pp.Contains("일"))
                    {
                        ballType = "일반";
                        if (pp.Contains("홀"))
                        {
                            _normalballOdd++;
                        }
                        else if (pp.Contains("짝"))
                        {
                            _normalballEven++;
                        }
                        else if (pp.Contains("오버"))
                        {
                            _normalballOver++;
                        }
                        else if (pp.Contains("언더"))
                        {
                            _normalballUnder++;
                        }
                    }
                    else if (pp.Contains("사"))
                    {
                        ballType = "파사";
                    }
                    string[] row = {
                        pickstername, ballType, record, pbstreak, pp, powerballPick[0], powerballPick[1], powerballPick[2], powerballPick[3], powerballPick[4], powerballPick[5], powerballPick[6], powerballPick[7], powerballPick[8], powerballPick[9], site
                        };
                    for (int j = 0; j < row.Length; j++)
                    {
                        _picksterInformation[allPicksterNumber, j] = row[j];
                    }
                    allPicksterNumber++;
                }
            }
            catch (Exception _ex)
            {
                _picksterListLoad = false;
                txtLogAdd("픽스터 정보를 가져오는 것에 실패하였습니다. 곧 재시도 하겠습니다.", Color.Red);
                logger.Error(_ex.ToString());
            }
        }

        void updateBettingUserInformation()
        {
            // 픽스터 이름 = (_picksterInformation[i, 0]
            if (!superiorityModeRadioButton.Checked)
            {
                for (int num = 1; num <= accountNumber; num++)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + num.ToString(), true)[0] as Button);
                    if (String.IsNullOrEmpty(_pickster.Text))
                    {
                        continue;
                    }
                    if (_pickster.Text.Contains("--"))
                    {
                        continue;
                    }
                    int index = -1;

                    for (int i = 0; i < _picksterInformation.GetLength(0); i++)
                    {
                        if (String.IsNullOrEmpty(_picksterInformation[i, 0]))
                        {
                            break;
                        }
                        if (_picksterInformation[i, 0].Equals(_pickster.Text))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index < 0)
                    {
                        continue;
                    }
                    if (!String.IsNullOrEmpty(_picksterInformation[index, 0]))
                    {
                        TextBox _ballType = (Controls.Find("txtBoxBallType" + num.ToString(), true)[0] as TextBox);
                        _ballType.Text = _picksterInformation[index, 1].ToString();
                        if (_ballType.Text.Equals("파워"))
                        {
                            TextBox PR = (Controls.Find("txtBoxPR" + num.ToString(), true)[0] as TextBox);
                            TextBox PS = (Controls.Find("txtBoxPS" + num.ToString(), true)[0] as TextBox);
                            TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                            String strPick = _picksterInformation[index, 4];
                            if (remainingTime < 190)
                            {
                                if (strPick.Contains("P") || strPick.Contains("통과"))
                                {
                                    pick.Text = "통과"; // 파볼픽
                                    pick.ForeColor = Color.DarkGray;
                                }
                                else
                                {
                                    if (strPick.Contains("홀"))
                                    {

                                        pick.ForeColor = Color.RoyalBlue;
                                        pick.Text = "홀"; // 파볼픽
                                    }
                                    else if (strPick.Contains("언더"))
                                    {

                                        pick.ForeColor = Color.RoyalBlue;
                                        pick.Text = "언더"; // 파볼픽
                                    }
                                    else if (strPick.Contains("짝"))
                                    {

                                        pick.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25))))); // 230, 74, 25
                                        pick.Text = "짝"; // 파볼픽
                                    }
                                    else if (strPick.Contains("오버"))
                                    {
                                        pick.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                        pick.Text = "오버"; // 파볼픽
                                    }
                                }
                            }
                            else
                            {
                                if (strPick.Contains("P") || strPick.Contains("통과"))
                                {
                                    pick.Text = "◇";
                                }
                                else
                                {
                                    pick.Text = "◆";
                                }
                            }

                            if (strPick.Contains("홀") || strPick.Contains("짝") || strPick.Contains("언더") || strPick.Contains("오버"))
                            {
                                PR.Text = _picksterInformation[index, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[index, 3]; // 파워볼 연승
                            }
                            else
                            {
                                PR.Text = "---";
                                PS.Text = "---"; // 파워볼 연승
                            }
                        }
                        else if (_ballType.Text.Equals("일반"))
                        {
                            TextBox PR = (Controls.Find("txtBoxPR" + num.ToString(), true)[0] as TextBox);
                            TextBox PS = (Controls.Find("txtBoxPS" + num.ToString(), true)[0] as TextBox);
                            TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                            String strPick = _picksterInformation[index, 4];
                            if (remainingTime < 190)
                            {
                                if (strPick.Contains("P") || strPick.Contains("통과"))
                                {
                                    pick.Text = "통과"; // 파볼픽
                                    pick.ForeColor = Color.DarkGray;
                                }
                                else
                                {
                                    if (strPick.Contains("홀"))
                                    {

                                        pick.ForeColor = Color.RoyalBlue;
                                        pick.Text = "홀"; // 파볼픽
                                    }
                                    else if (strPick.Contains("언더"))
                                    {

                                        pick.ForeColor = Color.RoyalBlue;
                                        pick.Text = "언더"; // 파볼픽
                                    }
                                    else if (strPick.Contains("짝"))
                                    {

                                        pick.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                        pick.Text = "짝"; // 파볼픽
                                    }
                                    else if (strPick.Contains("오버"))
                                    {
                                        pick.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                        pick.Text = "오버"; // 파볼픽
                                    }
                                }
                            }
                            else
                            {
                                if (strPick.Contains("P"))
                                {
                                    pick.Text = "◇";
                                }
                                else
                                {
                                    pick.Text = "◆";
                                }
                            }

                            if (strPick.Contains("홀") || strPick.Contains("짝") || strPick.Contains("언더") || strPick.Contains("오버"))
                            {
                                PR.Text = _picksterInformation[index, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[index, 3]; // 파워볼 연승
                            }
                            else
                            {
                                PR.Text = "---";
                                PS.Text = "---"; // 파워볼 연승
                            }
                        }
                        else if (_ballType.Text.Equals("파사"))
                        {
                            TextBox PR = (Controls.Find("txtBoxPR" + num.ToString(), true)[0] as TextBox);
                            TextBox PS = (Controls.Find("txtBoxPS" + num.ToString(), true)[0] as TextBox);
                            TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                            String strPick = _picksterInformation[index, 4];
                            if (remainingTime < 190)
                            {
                                if (strPick.Contains("P") || strPick.Contains("통과"))
                                {
                                    pick.Text = "통과"; // 파볼픽
                                    pick.ForeColor = Color.DarkGray;
                                }
                                else
                                {
                                    if (strPick.Contains("홀") || strPick.Contains("3") || strPick.Contains("좌"))
                                    {

                                        pick.ForeColor = Color.RoyalBlue;
                                        pick.Text = strPick.Replace("사", ""); // 파볼픽
                                    }
                                    else if (strPick.Contains("짝") || strPick.Contains("4") || strPick.Contains("우"))
                                    {

                                        pick.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                        pick.Text = strPick.Replace("사", ""); // 파볼픽
                                    }
                                }
                            }
                            else
                            {
                                if (strPick.Contains("P"))
                                {
                                    pick.Text = "◇";
                                }
                                else
                                {
                                    pick.Text = "◆";
                                }
                            }

                            if (strPick.Contains("홀") || strPick.Contains("짝") || strPick.Contains("3") || strPick.Contains("4") || strPick.Contains("좌") || strPick.Contains("우"))
                            {
                                PR.Text = _picksterInformation[index, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[index, 3]; // 파워볼 연승
                            }
                            else
                            {
                                PR.Text = "---";
                                PS.Text = "---"; // 파워볼 연승
                            }
                        }
                    }
                }
            }
        }


        int allPicksterNumber = 0;
        private void pickPickster()
        {
            try
            {
                _picksterListLoad = true;
                _powerballEven = 0; // 짝수
                _powerballOdd = 0; // 홀수
                _powerballUnder = 0; // 언더
                _powerballOver = 0; // 오버

                _normalballEven = 0; // 짝수
                _normalballOdd = 0; // 홀수
                _normalballUnder = 0; // 언더
                _normalballOver = 0; // 오버
                allPicksterNumber = 0;

                arrayListView();

                if (_picksterListLoad)
                {
                    btnPicksterChoicePowerOddNum.Text = _powerballOdd.ToString();
                    btnPicksterChoicePowerEvenNum.Text = _powerballEven.ToString();
                    btnPicksterChoicePowerOverNum.Text = _powerballOver.ToString();
                    btnPicksterChoicePowerUnderNum.Text = _powerballUnder.ToString();

                    btnPicksterChoiceNormalOddNum.Text = _normalballOdd.ToString();
                    btnPicksterChoiceNormalEvenNum.Text = _normalballEven.ToString();
                    btnPicksterChoiceNormalOverNum.Text = _normalballOver.ToString();
                    btnPicksterChoiceNormalUnderNum.Text = _normalballUnder.ToString();

                    poweroddPercent = ((Double)_powerballOdd / ((Double)_powerballOdd + (Double)_powerballEven)) * 100;
                    powerevenPercent = ((Double)_powerballEven / ((Double)_powerballOdd + (Double)_powerballEven)) * 100;
                    poweroverPercent = ((Double)_powerballOver / ((Double)_powerballOver + (Double)_powerballUnder)) * 100;
                    powerunderPercent = ((Double)_powerballUnder / ((Double)_powerballOver + (Double)_powerballUnder)) * 100;

                    btnPicksterChoicePowerOddPercent.Text = poweroddPercent.ToString("F") + "%";
                    btnPicksterChoicePowerEvenPercent.Text = powerevenPercent.ToString("F") + "%";
                    btnPicksterChoicePowerOverPercent.Text = poweroverPercent.ToString("F") + "%";
                    btnPicksterChoicePowerUnderPercent.Text = powerunderPercent.ToString("F") + "%";

                    normaloddPercent = ((Double)_normalballOdd / ((Double)_normalballOdd + (Double)_normalballEven)) * 100;
                    normalevenPercent = ((Double)_normalballEven / ((Double)_normalballOdd + (Double)_normalballEven)) * 100;
                    normaloverPercent = ((Double)_normalballOver / ((Double)_normalballOver + (Double)_normalballUnder)) * 100;
                    normalunderPercent = ((Double)_normalballUnder / ((Double)_normalballOver + (Double)_normalballUnder)) * 100;

                    btnPicksterChoiceNormalOddPercent.Text = normaloddPercent.ToString("F") + "%";
                    btnPicksterChoiceNormalEvenPercent.Text = normalevenPercent.ToString("F") + "%";
                    btnPicksterChoiceNormalOverPercent.Text = normaloverPercent.ToString("F") + "%";
                    btnPicksterChoiceNormalUnderPercent.Text = normalunderPercent.ToString("F") + "%";

                    updateBettingUserInformation();

                    if (!listViewUnVisible.Checked)
                    {
                        try
                        {
                            listView1.BeginUpdate();
                            listView1.Items.Clear();
                            for (int i = 0; i < arrayNum; i++)
                            {
                                if (_picksterInformation[i, 0] == null)
                                {
                                    break;
                                }
                                ListViewItem item;
                                if (_picksterInformation[i, 0] != null)
                                {
                                    item = new ListViewItem(_picksterInformation[i, 0]); // 픽스터 이름
                                    item.UseItemStyleForSubItems = false;
                                    for (int k2 = 1; k2 <= 13; k2++)
                                    {
                                        if (k2 == 1) // 파워볼 / 엔트리 여부
                                        {
                                            if (_picksterInformation[i, 15].Contains("pbg"))
                                            {
                                                item.SubItems.Add("파워");
                                                item.SubItems[0].BackColor = Color.LightGray;
                                                item.SubItems[1].BackColor = Color.LightGray;
                                            }
                                            else if (_picksterInformation[i, 15].Contains("ntry"))
                                            {
                                                item.SubItems.Add("엔트");
                                            }
                                            else
                                            {
                                                item.SubItems.Add(_picksterInformation[i, 15].ToString());
                                            }
                                        }
                                        else if (k2 == 2) // 전적
                                        {
                                            if (_picksterInformation[i, 4].Contains("P"))
                                            {
                                                item.SubItems.Add(_picksterInformation[i, 2]);
                                                item.SubItems[2].ForeColor = Color.Gainsboro;
                                            }
                                            else
                                            {
                                                item.SubItems.Add(_picksterInformation[i, 2]);
                                                item.SubItems[2].ForeColor = Color.Black;
                                            }
                                        }
                                        else if (k2 == 3) // 연승 표기란
                                        {
                                            if (_picksterInformation[i, 15].Contains("ntry"))
                                            {
                                                if (_picksterInformation[i, 4].Contains("P"))
                                                {
                                                    item.SubItems.Add("0연승");
                                                    item.SubItems[3].ForeColor = Color.Gainsboro;
                                                }
                                                else
                                                {
                                                    item.SubItems.Add(_picksterInformation[i, 3]);  // 연승
                                                }
                                            }
                                            else
                                            {
                                                item.SubItems.Add(_picksterInformation[i, 3]);  // 연승
                                                if (_picksterInformation[i, 3].Contains("연승"))
                                                {
                                                    item.SubItems[3].ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                                }
                                                else
                                                {
                                                    item.SubItems[3].ForeColor = Color.RoyalBlue;
                                                }
                                            }
                                        }
                                        else if (k2 == 4) // 픽
                                        {
                                            if (remainingTime < 190)
                                            {
                                                if (_picksterInformation[i, k2].Contains("P"))
                                                {
                                                    item.SubItems.Add("통");
                                                    item.SubItems[k2].ForeColor = Color.LightGray;
                                                }
                                                else
                                                {
                                                    if (_picksterInformation[i, k2].Contains("파"))
                                                    {
                                                        String _pick = "";
                                                        if (_picksterInformation[i, 4].Contains("홀"))
                                                        {
                                                            _pick = "홀";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("짝"))
                                                        {
                                                            _pick = "짝";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("오버"))
                                                        {
                                                            _pick = "오";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("언더"))
                                                        {
                                                            _pick = "언";
                                                        }

                                                        item.SubItems.Add(_pick);
                                                        item.SubItems[k2].BackColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(74)))), ((int)(((byte)(25)))));
                                                        item.SubItems[k2].ForeColor = Color.White;
                                                    }
                                                    else if (_picksterInformation[i, k2].Contains("일"))
                                                    {
                                                        String _pick = "";
                                                        if (_picksterInformation[i, 4].Contains("홀"))
                                                        {
                                                            _pick = "홀";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("짝"))
                                                        {
                                                            _pick = "짝";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("오버"))
                                                        {
                                                            _pick = "오";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("언더"))
                                                        {
                                                            _pick = "언";
                                                        }

                                                        item.SubItems.Add(_pick);
                                                        item.SubItems[k2].BackColor = Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(136)))), ((int)(((byte)(229))))); // 30, 136, 229
                                                        item.SubItems[k2].ForeColor = Color.White;
                                                    }
                                                    else if (_picksterInformation[i, k2].Contains("사"))
                                                    {
                                                        String _pick = "";
                                                        if (_picksterInformation[i, 4].Contains("홀"))
                                                        {
                                                            _pick = "홀";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("짝"))
                                                        {
                                                            _pick = "짝";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("좌"))
                                                        {
                                                            _pick = "좌";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("우"))
                                                        {
                                                            _pick = "우";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("3"))
                                                        {
                                                            _pick = "3";
                                                        }
                                                        else if (_picksterInformation[i, 4].Contains("4"))
                                                        {
                                                            _pick = "4";
                                                        }

                                                        item.SubItems.Add(_pick);
                                                        item.SubItems[k2].BackColor = Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(136)))), ((int)(((byte)(229))))); // 30, 136, 229
                                                        item.SubItems[k2].ForeColor = Color.White;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (_picksterInformation[i, k2].Contains("P"))
                                                {
                                                    item.SubItems.Add("◇");
                                                }
                                                else
                                                {
                                                    item.SubItems.Add("◆");
                                                }
                                            }
                                        }
                                        else if (k2 >= 5) // 당/미/패 표기
                                        {
                                            if (_picksterInformation[i, k2].Contains("O"))
                                            {
                                                if (resultMarkCheckBox.Checked)
                                                {
                                                    item.SubItems.Add("당");
                                                    item.SubItems[k2].ForeColor = Color.White;
                                                    item.SubItems[k2].BackColor = Color.DarkRed;
                                                }
                                                else
                                                {
                                                    String _a = _picksterInformation[i, k2].Replace("O", "");
                                                    if (_a.Contains("파"))
                                                    {
                                                        if (_a.Contains("언"))
                                                        {
                                                            _a = "언";
                                                        }
                                                        else if (_a.Contains("오"))
                                                        {
                                                            _a = "오";
                                                        }
                                                        else if (_a.Contains("홀"))
                                                        {
                                                            _a = "홀";
                                                        }
                                                        else if (_a.Contains("짝"))
                                                        {
                                                            _a = "짝";
                                                        }
                                                        item.SubItems.Add(_a);
                                                        item.SubItems[k2].ForeColor = Color.OrangeRed;
                                                    }
                                                    else if (_a.Contains("일"))
                                                    {

                                                        if (_a.Contains("언"))
                                                        {
                                                            _a = "언";
                                                        }
                                                        else if (_a.Contains("오"))
                                                        {
                                                            _a = "오";
                                                        }
                                                        else if (_a.Contains("홀"))
                                                        {
                                                            _a = "홀";
                                                        }
                                                        else if (_a.Contains("짝"))
                                                        {
                                                            _a = "짝";
                                                        }
                                                        item.SubItems.Add(_a);
                                                        item.SubItems[k2].ForeColor = Color.MediumVioletRed;
                                                    }
                                                    else if (_a.Contains("사"))
                                                    {

                                                        if (_a.Contains("좌"))
                                                        {
                                                            _a = "좌";
                                                        }
                                                        else if (_a.Contains("우"))
                                                        {
                                                            _a = "우";
                                                        }
                                                        else if (_a.Contains("홀"))
                                                        {
                                                            _a = "홀";
                                                        }
                                                        else if (_a.Contains("짝"))
                                                        {
                                                            _a = "짝";
                                                        }
                                                        else if (_a.Contains("3"))
                                                        {
                                                            _a = "3";
                                                        }
                                                        else if (_a.Contains("4"))
                                                        {
                                                            _a = "4";
                                                        }
                                                        item.SubItems.Add(_a);
                                                        item.SubItems[k2].ForeColor = Color.MediumVioletRed;
                                                    }
                                                }
                                            }
                                            else if (_picksterInformation[i, k2].Contains("X"))
                                            {
                                                if (resultMarkCheckBox.Checked)
                                                {
                                                    item.SubItems.Add("미");
                                                    item.SubItems[k2].ForeColor = Color.White;
                                                    item.SubItems[k2].BackColor = Color.DarkGray;
                                                }
                                                else
                                                {
                                                    String _b = _picksterInformation[i, k2].Replace("X", "");
                                                    if (_b.Contains("파"))
                                                    {
                                                        if (_b.Contains("언"))
                                                        {
                                                            _b = "언";
                                                        }
                                                        else if (_b.Contains("오"))
                                                        {
                                                            _b = "오";
                                                        }
                                                        else if (_b.Contains("홀"))
                                                        {
                                                            _b = "홀";
                                                        }
                                                        else if (_b.Contains("짝"))
                                                        {
                                                            _b = "짝";
                                                        }
                                                        item.SubItems.Add(_b);
                                                        item.SubItems[k2].ForeColor = Color.BlueViolet;
                                                    }
                                                    else if (_b.Contains("일"))
                                                    {

                                                        if (_b.Contains("언"))
                                                        {
                                                            _b = "언";
                                                        }
                                                        else if (_b.Contains("오"))
                                                        {
                                                            _b = "오";
                                                        }
                                                        else if (_b.Contains("홀"))
                                                        {
                                                            _b = "홀";
                                                        }
                                                        else if (_b.Contains("짝"))
                                                        {
                                                            _b = "짝";
                                                        }
                                                        item.SubItems.Add(_b);
                                                        item.SubItems[k2].ForeColor = Color.DarkBlue;
                                                    }
                                                    else if (_b.Contains("사"))
                                                    {

                                                        if (_b.Contains("좌"))
                                                        {
                                                            _b = "좌";
                                                        }
                                                        else if (_b.Contains("우"))
                                                        {
                                                            _b = "우";
                                                        }
                                                        else if (_b.Contains("홀"))
                                                        {
                                                            _b = "홀";
                                                        }
                                                        else if (_b.Contains("짝"))
                                                        {
                                                            _b = "짝";
                                                        }
                                                        else if (_b.Contains("3"))
                                                        {
                                                            _b = "3";
                                                        }
                                                        else if (_b.Contains("4"))
                                                        {
                                                            _b = "4";
                                                        }
                                                        item.SubItems.Add(_b);
                                                        item.SubItems[k2].ForeColor = Color.DarkBlue;
                                                    }
                                                }
                                            }
                                            else if (_picksterInformation[i, k2].Contains("P"))
                                            {
                                                item.SubItems.Add("통");
                                                item.SubItems[k2].ForeColor = Color.LightGray;
                                            }
                                        }
                                    }
                                    listView1.Items.Add(item);
                                }
                            }
                        }
                        finally
                        {
                            listView1.EndUpdate();
                        }
                    }
                    SetAlternatingRowColors(listView1, Color.LightGray, Color.White);
                    if (remainingTime < 160)
                    {
                        txtLogAdd("총 [" + allPicksterNumber + "] 명 [픽완료 : " + picksternumber + "]의 픽스터 정보를 새롭게 불러왔습니다.", Color.White);
                    }
                    else
                    {

                        txtLogAdd("총 [" + allPicksterNumber + "] 명의 픽스터 정보를 새롭게 불러왔습니다.", Color.White);
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        public void SetAlternatingRowColors(ListView lst, Color color1, Color color2)
        {
            //loop through each ListViewItem in the ListView control
            foreach (ListViewItem item in lst.Items)
            {
                if ((item.Index % 2) == 0)
                    item.BackColor = color1;
                else
                    item.BackColor = color2;
            }
        }
        // 남은 시간 표기
        public void setTimeRemaining(int _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            remainTime.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }
        static int arrayNum = 2000;
        static int array2Num = 16;
        // 픽스터 정보 배열
        string[,] _picksterInformation = new string[arrayNum, array2Num];

        int[] _picksterRoundArray = new int[40];
        int remainingTime = 0;
        // 남은 시간 타이머
        System.Timers.Timer timer = new System.Timers.Timer();

        void start()
        {
            try
            {
                _isStart = true;
                _bettingClosed = false;
                txtLogAdd("자동 배팅이 시작되었습니다.", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                lblStartTime.Text = DateTime.Now.ToString("HH:mm:ss"); // 시작시간
                getPowerballInformation();
                txtLogAdd("완료", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                txtLogAdd("[" + allinning + "]회 "
                    + "차가 진행 중입니다", Color.White);
                txtNotice.Text = "알림 : " + allinning + "회 파워볼 배팅이 진행 중입니다.";

                if (UtilModel._userSite.Equals("rdw"))
                {
                    JObject jo = JObject.Parse(UtilModel.GetHttpGet("http://api.rdwball.com/userinfo/" + UtilModel._apikey));

                    var resultUserInforA = jo.SelectToken("A").ToString();
                    if (resultUserInforA.Equals("0"))
                    {
                        startMoney = int.Parse(jo.SelectToken("B").ToString());
                        lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                        lblTxtNowMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                    }

                    jo = JObject.Parse(UtilModel.GetHttpGet("http://api.rdwball.com/gamestatus/" + UtilModel._apikey + "/4"));

                    var resultGameStatusA = jo.SelectToken("A").ToString();
                    if (resultGameStatusA.Equals("0"))
                    {
                        textBox2.Text += jo.SelectToken("B").SelectToken("A").ToString() + Environment.NewLine; // 게임 서버 점검 유무
                        textBox2.Text += jo.SelectToken("B").SelectToken("B").ToString() + Environment.NewLine; // 전체 게임 시간
                        textBox2.Text += jo.SelectToken("B").SelectToken("C").ToString() + Environment.NewLine; // 사이트 배팅 가능한 초
                        textBox2.Text += jo.SelectToken("B").SelectToken("D").ToString() + Environment.NewLine; // 게임 진행 상태
                        textBox2.Text += jo.SelectToken("B").SelectToken("E").ToString() + Environment.NewLine; // 게임 서버 현재 시간
                        textBox2.Text += jo.SelectToken("B").SelectToken("F").ToString() + Environment.NewLine; // 현재 게임 진행 시간(0~300)
                        textBox2.Text += jo.SelectToken("B").SelectToken("G").ToString() + Environment.NewLine; // 오늘 회차
                        textBox2.Text += jo.SelectToken("B").SelectToken("H").ToString() + Environment.NewLine; // 전체 회차
                    }
                }

                startInning = allinning;

                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    ComboBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                    TextBox _betMoney = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                    _level.ForeColor = Color.Black;
                    _level.BackColor = Color.White;
                    _level.Text = "1";
                    int outValue = 0;
                    bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                    if (_b)
                    {
                        _betMoney.Text = outValue.ToString();
                    }
                }
                setTimeRemaining(remainingTime);

                pickPickster();
                if (autoRefillCheckBox.Checked)
                {
                    if (!superiorityModeRadioButton.Checked)
                    {
                        autoRefill();
                        txtLogAdd("자동 채워주기가 활성화 상태입니다.", Color.White);
                    }
                }
                powerballResult();
                timer.Start();
                beepSound();
                logger.Info("[" + allinning + "] 배팅이 시작되었습니다.");
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }

        }
        // 시작 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isStart)
                {
                    return;
                }

                btnBettingStart.Text = "- 진행 중 -";
                btnBettingStart.BackColor = Color.DarkGray;
                btnBettingStart.ForeColor = Color.Teal;
                start();
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        // 배팅 종료 눌러서 완전 종료할 때
        private void btnBettingStop_Click(object sender, EventArgs e)
        {
            if (!_isStart)
            {
                MessageBox.Show("진행 중이 아닙니다.");
                return;
            }

            for (int _find = 1; _find <= accountNumber; _find++)
            {
                TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                TextBox _boxPick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                _ballType.Text = "";
                _boxPick.Text = "";
                _boxPick.ForeColor = Color.Black;
            }

            btnBettingStart.Text = "배팅 시작";
            btnBettingStart.BackColor = Color.White;
            btnBettingStart.ForeColor = Color.Black;

            txtLogAdd("자동 배팅이 종료되었습니다.", Color.White);
            timer.Stop();
            _isStart = false;
            logger.Info("배팅이 종료되었습니다.");
        }

        /*
        private void textChangeMoneySetting(TextBox txtBox, Label lbl)
        {
            try
            {

                int numTxtBox = Int32.Parse(txtBox.Text);
                lbl.Text = UtilModel.NumToString(numTxtBox);
            }
            catch (FormatException a)
            {
                Console.WriteLine(a.Message);
            }
            catch (OverflowException o)
            {
                Console.WriteLine(o.Message);
            }
        }
        */
        private void txtBtMoneySettingL1_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL2_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL3_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL4_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL5_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL6_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL7_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL8_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL9_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL10_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL11_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL12_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL13_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL14_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL15_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }
        private void txtBtMoneySettingL16_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL17_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL18_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }

        private void txtBtMoneySettingL19_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }
        private void txtBtMoneySettingL20_KeyPress(object sender, KeyPressEventArgs e)
        {
            UtilModel.TypingOnlyNumber(sender, e, false, true);
        }
        // 각종 알림 사항
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
        // txtlog 창 100 줄 넘으면 초기화
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();

            if (txtLog.Lines.Length > 100)
            {
                txtLog.Clear();
            }
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        private void tableLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster1, txtBoxBallType1, BoxPick1, txtBoxPR1, txtBoxPS1);
        }

        private void tableLayoutPanel2_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster2, txtBoxBallType2, BoxPick2, txtBoxPR2, txtBoxPS2);
        }

        private void tableLayoutPanel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel3_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster3, txtBoxBallType3, BoxPick3, txtBoxPR3, txtBoxPS3);
        }

        private void tableLayoutPanel3_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel4_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster4, txtBoxBallType4, BoxPick4, txtBoxPR4, txtBoxPS4);
        }

        private void tableLayoutPanel4_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel5_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster5, txtBoxBallType5, BoxPick5, txtBoxPR5, txtBoxPS5);
        }

        private void tableLayoutPanel5_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel6_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster6, txtBoxBallType6, BoxPick6, txtBoxPR6, txtBoxPS6);
        }

        private void tableLayoutPanel6_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel7_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster7, txtBoxBallType7, BoxPick7, txtBoxPR7, txtBoxPS7);
        }

        private void tableLayoutPanel7_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel8_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster8, txtBoxBallType8, BoxPick8, txtBoxPR8, txtBoxPS8);
        }

        private void tableLayoutPanel8_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel9_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster9, txtBoxBallType9, BoxPick9, txtBoxPR9, txtBoxPS9);
        }

        private void tableLayoutPanel9_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel10_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster10, txtBoxBallType10, BoxPick9, txtBoxPR10, txtBoxPS10);
        }

        private void tableLayoutPanel10_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel11_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster11, txtBoxBallType11, BoxPick11, txtBoxPR11, txtBoxPS11);
        }

        private void tableLayoutPanel11_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel12_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster12, txtBoxBallType12, BoxPick12, txtBoxPR12, txtBoxPS12);
        }

        private void tableLayoutPanel12_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel13_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster13, txtBoxBallType13, BoxPick13, txtBoxPR13, txtBoxPS13);
        }

        private void tableLayoutPanel13_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel14_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster14, txtBoxBallType14, BoxPick14, txtBoxPR14, txtBoxPS14);
        }

        private void tableLayoutPanel14_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel15_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster15, txtBoxBallType15, BoxPick15, txtBoxPR15, txtBoxPS15);
        }

        private void tableLayoutPanel15_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel16_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster16, txtBoxBallType16, BoxPick16, txtBoxPR16, txtBoxPS16);
        }

        private void tableLayoutPanel16_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel17_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster17, txtBoxBallType17, BoxPick17, txtBoxPR17, txtBoxPS17);
        }

        private void tableLayoutPanel17_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel18_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster18, txtBoxBallType18, BoxPick18, txtBoxPR18, txtBoxPS18);
        }

        private void tableLayoutPanel18_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel19_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster19, txtBoxBallType19, BoxPick19, txtBoxPR19, txtBoxPS19);
        }

        private void tableLayoutPanel19_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel20_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster20, txtBoxBallType20, BoxPick20, txtBoxPR20, txtBoxPS20);
        }

        private void tableLayoutPanel20_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }


        private void tableLayoutPanel21_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster21, txtBoxBallType21, BoxPick21, txtBoxPR21, txtBoxPS21);
        }

        private void tableLayoutPanel21_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel22_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster22, txtBoxBallType22, BoxPick22, txtBoxPR22, txtBoxPS22);
        }

        private void tableLayoutPanel22_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel23_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster23, txtBoxBallType23, BoxPick23, txtBoxPR23, txtBoxPS23);
        }

        private void tableLayoutPanel23_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel24_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster24, txtBoxBallType24, BoxPick24, txtBoxPR24, txtBoxPS24);
        }

        private void tableLayoutPanel24_dragenter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel25_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster25, txtBoxBallType25, BoxPick25, txtBoxPR25, txtBoxPS25);
        }

        private void tableLayoutPanel25_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel26_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster26, txtBoxBallType26, BoxPick26, txtBoxPR26, txtBoxPS26);
        }

        private void tableLayoutPanel26_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel27_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster27, txtBoxBallType27, BoxPick27, txtBoxPR27, txtBoxPS27);
        }

        private void tableLayoutPanel27_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel28_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster28, txtBoxBallType28, BoxPick28, txtBoxPR28, txtBoxPS28);
        }

        private void tableLayoutPanel28_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel29_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster29, txtBoxBallType29, BoxPick29, txtBoxPR29, txtBoxPS29);
        }

        private void tableLayoutPanel29_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel30_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster30, txtBoxBallType30, BoxPick30, txtBoxPR30, txtBoxPS30);
        }

        private void tableLayoutPanel30_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void tableLayoutPanel31_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster31, txtBoxBallType31, BoxPick31, txtBoxPR31, txtBoxPS31);
        }

        private void tableLayoutPanel31_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel32_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster32, txtBoxBallType32, BoxPick32, txtBoxPR32, txtBoxPS32);
        }

        private void tableLayoutPanel32_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel33_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster33, txtBoxBallType33, BoxPick33, txtBoxPR33, txtBoxPS33);
        }

        private void tableLayoutPanel33_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel34_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster34, txtBoxBallType34, BoxPick34, txtBoxPR34, txtBoxPS34);
        }

        private void tableLayoutPanel34_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel35_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster35, txtBoxBallType35, BoxPick35, txtBoxPR35, txtBoxPS35);
        }

        private void tableLayoutPanel35_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel36_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster36, txtBoxBallType36, BoxPick36, txtBoxPR36, txtBoxPS36);
        }

        private void tableLayoutPanel36_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel37_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster37, txtBoxBallType37, BoxPick37, txtBoxPR37, txtBoxPS37);
        }

        private void tableLayoutPanel37_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel38_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            _dragDrop(data, BoxPickster38, txtBoxBallType38, BoxPick38, txtBoxPR38, txtBoxPS38);
        }

        private void tableLayoutPanel38_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel39_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster39, txtBoxBallType39, BoxPick39, txtBoxPR39, txtBoxPS39);
        }

        private void tableLayoutPanel39_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tableLayoutPanel40_dragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, BoxPickster40, txtBoxBallType40, BoxPick40, txtBoxPR40, txtBoxPS40);
        }
        private void tableLayoutPanel40_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void _dragDrop(ListViewItem _data, Button pickster, TextBox type, TextBox pick, TextBox PR, TextBox PS)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 픽스터를 바꿀 수 없습니다.");
                return;
            }
            if (superiorityModeRadioButton.Checked)
            {
                return;
            }
            if (!samePersonCheckBox.Checked)
            {
                Boolean _bool = false;
                int findNum = 0;
                for (int _find = 1; _find <= 40; _find++)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    if (_pickster.Text.Equals(_data.Text))
                    {
                        findNum = _find;
                        _bool = true;
                        break;
                    }
                }
                if (_bool)
                {
                    txtLogAdd("이미 존재하는 픽스터입니다. [" + findNum + "]번째 칸에 있습니다.", Color.OrangeRed);
                    return;
                }
            }

            for (int k = 0; k < _picksterInformation.Length; k++)
            {
                try
                {
                    if (_picksterInformation[k, 0] != null)
                    {
                        if (_picksterInformation[k, 0].Equals(_data.Text))
                        {
                            pickster.Text = _picksterInformation[k, 0]; // 픽스터 이름
                            if (_picksterInformation[k, 4].Contains("파"))
                            {
                                type.Text = "파워";
                                if (_picksterInformation[k, 4].Contains("홀"))
                                {
                                    pick.Text = "홀"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("짝"))
                                {
                                    pick.Text = "짝"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("언"))
                                {
                                    pick.Text = "언더"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("오"))
                                {
                                    pick.Text = "오버"; // 파볼픽
                                }

                                PR.Text = _picksterInformation[k, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[k, 3]; // 파워볼 연승
                            }
                            else if (_picksterInformation[k, 4].Contains("일"))
                            {
                                type.Text = "일반";
                                if (_picksterInformation[k, 4].Contains("홀"))
                                {
                                    pick.Text = "홀"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("짝"))
                                {
                                    pick.Text = "짝"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("언"))
                                {
                                    pick.Text = "언더"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("오"))
                                {
                                    pick.Text = "오버"; // 파볼픽
                                }

                                PR.Text = _picksterInformation[k, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[k, 3]; // 파워볼 연승
                            }
                            else if (_picksterInformation[k, 4].Contains("사"))
                            {
                                type.Text = "파사";
                                if (_picksterInformation[k, 4].Contains("홀"))
                                {
                                    pick.Text = "홀"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("짝"))
                                {
                                    pick.Text = "짝"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("좌"))
                                {
                                    pick.Text = "좌"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("우"))
                                {
                                    pick.Text = "우"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("3"))
                                {
                                    pick.Text = "3"; // 파볼픽
                                }
                                else if (_picksterInformation[k, 4].Contains("4"))
                                {
                                    pick.Text = "4"; // 파볼픽
                                }

                                PR.Text = _picksterInformation[k, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[k, 3]; // 파워볼 연승
                            }
                            txtLogAdd(_picksterInformation[k, 0] + "님을 배팅에 참여시켰습니다.", Color.White);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception _ex)
                {
                    logger.Error(_ex.ToString());
                }
            }
            logger.Info("[" + _data.Text + " ] 를 배팅 목록에 추가");
        }
        Boolean _bettingClosed = false;
        Boolean getPicksterAndRemainingtimeReload = false;
        Boolean getPowerballinningAndPowerBallResult = false;
        Boolean getPowerBallResultProcessing = false;
        Boolean refreshBettingInformation = false;

        ListViewItem item;
        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(remainingTimer_Tick));
        }
        int resultAfter_allbettingmoney = 0;
        int resultBefore_allbettingmoney = 0;

        int resultBeforePOverMoney = 0;
        int resultBeforePUnderMoney = 0;
        int resultBeforePOddMoney = 0;
        int resultBeforePEvenMoney = 0;
        int resultBeforeNOverMoney = 0;
        int resultBeforeNUnderMoney = 0;
        int resultBeforeNOddMoney = 0;
        int resultBeforeNEvenMoney = 0;
        bool _automodeChange = false;
        void init285()
        {
            _picksterInformation = new string[arrayNum, array2Num];
            refreshBettingInformation = true;
            _automodeChange = false;
            getPowerballinningAndPowerBallResult = false;
            getPowerBallResultProcessing = false;
            getPicksterAndRemainingtimeReload = false;
            powerballResult();
        }
        void init275()
        {
            try
            {
                lblPSingle.Text = "0 원";
                lblPPair.Text = "0 원";
                lblPUnder.Text = "0 원";
                lblPOver.Text = "0 원";
                lblNSingle.Text = "0 원";
                lblNPair.Text = "0 원";
                lblNUnder.Text = "0 원";
                lblNOver.Text = "0 원";
                _powerLadder34.Text = "0/0";
                _powerLadderLeftRight.Text = "0/0";
                _powerLadderOddEven.Text = "0/0";

                resultBeforePOverMoney = pOverMoney;
                resultBeforePUnderMoney = pUnderMoney;
                resultBeforePOddMoney = pOddMoney;
                resultBeforePEvenMoney = pEvenMoney;

                resultBeforeNOverMoney = nOverMoney;
                resultBeforeNUnderMoney = nUnderMoney;
                resultBeforeNOddMoney = nOddMoney;
                resultBeforeNEvenMoney = nEvenMoney;

                listView3.Items.Clear();

                getPowerballinningAndPowerBallResult = true;
                getPowerballInformation();
                resultBefore_allbettingmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney + ladder3Money + ladder4Money + ladderOddMoney + ladderEvenMoney + ladderLeftMoney + ladderRightMoney;
                powerballResult();
                txtLogAdd("파워볼 결과가 나왔습니다.", Color.White);
                txtNotice.Text = "알림 : 파워볼 결과가 나왔습니다.";
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        void init272()
        {
            try
            {
                getPowerBallResultProcessing = true;
                int gain = 0;
                int winmoney = 0;
                if (bettingStatus)
                {
                    resultAfter_allbettingmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney + ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
                    if (resultBefore_allbettingmoney > 0)
                    {
                        powerBallResultProcessing();
                        txtLogAdd("현재 결과 처리 중입니다.", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                        txtNotice.Text = "알림 : 결과 처리 중입니다.";
                    }
                    else
                    {
                        txtLogAdd("배팅된 금액이 없어 결과 처리를 하지 않습니다.", Color.White);
                        txtNotice.Text = "알림 : 배팅된 금액이 없어 결과 처리를 하지 않습니다.";
                    }
                    winmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney + ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
                    NowMoney += winmoney;

                    groupBoxBettingStatus.Text = "[" + (allinning - 1) + "회][" + (todayinning - 1) + "회][배팅 금액 : " + UtilModel.StringFormatChanged(resultBefore_allbettingmoney) + " 원][당첨금액 : " + UtilModel.StringFormatChanged(winmoney) + " 원]";
                    lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + " 원";

                    gain = NowMoney - startMoney;
                    if (gain > 0)
                    {
                        lblTxtNowGain.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(171)))), ((int)(((byte)(145)))));
                        lblTxtNowGain.Text = "▲ " + UtilModel.StringFormatChanged(gain) + " 원";
                    }
                    else if (gain < 0)
                    {
                        lblTxtNowGain.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
                        lblTxtNowGain.Text = "▼ " + UtilModel.StringFormatChanged(gain) + " 원";
                    }


                    if (allinning % 100 == 1)
                    {
                        listView2.Items.Clear();
                    }
                    item = new ListViewItem(allinning + "회");
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(UtilModel.getDateTime2());
                    item.SubItems.Add(UtilModel.StringFormatChanged(NowMoney));
                    if (gain > 0)
                    {
                        item.SubItems.Add("▲ " + UtilModel.StringFormatChanged(gain));
                    }
                    else
                    {
                        item.SubItems.Add("▼ " + UtilModel.StringFormatChanged(gain));
                    }

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBefore_allbettingmoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(winmoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(winmoney - resultBefore_allbettingmoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforePOddMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(pOddMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforePEvenMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(pEvenMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforePOverMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(pOverMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforePUnderMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(pUnderMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforeNOddMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(nOddMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforeNEvenMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(nEvenMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforeNOverMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(nOverMoney));

                    item.SubItems.Add(UtilModel.StringFormatChanged(resultBeforeNUnderMoney));
                    item.SubItems.Add(UtilModel.StringFormatChanged(nUnderMoney));

                    listView2.Items.Add(item);

                    if (UtilModel.telegramChatId > 0)
                    {
                        string _message = "[결과처리완료][" + allinning + "회][보유금 : " + UtilModel.StringFormatChanged(NowMoney) + " 원][수익 : " + UtilModel.StringFormatChanged(gain) + " 원]";
                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(_message));
                        }
                    }
                }

                updateUserStatus(winmoney, gain, termInning);

                clearBetInfor();

                if (gain > int.Parse(OverProfit.Text))
                {
                    beepSound();
                    txtLogAdd("축하드립니다. 수익 설정 금액을 초과하였습니다.", Color.White);
                    txtNotice.Text = "알림 : 축하드립니다. 수익 설정 금액을 초과하였습니다.";
                    MessageBox.Show("[" + DateTime.Now.ToString("hh:MM:ss") + "] 수익 설정 금액 초과로 전체 배팅정지되었습니다.");
                    for (int _find = 1; _find <= accountNumber; _find++)
                    {
                        Button _pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                        _pr.Text = "배팅정지";
                        _pr.ForeColor = Color.White;
                        _pr.BackColor = Color.Black;
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        void updateUserStatus(int winmoney, int gain, int termInning)
        {
            try
            {
                if (resultAfter_allbettingmoney > 0 || termInning > 0)
                {
                    string url = UtilModel.updateuserstatus;
                    string msg = string.Format("update=&userid={0}&nickname={1}&ownmoney={2}&allinning={3}&allbettingmoney={4}&winmoney={5}&gain={6}&token={7}&startmoney={8}&starttime={9}&usersite={10}&userip={11}&distributor={12}&version={13}&termInning={14}",
                        UtilModel.betid, nickName, NowMoney, allinning, resultAfter_allbettingmoney, winmoney, gain, UtilModel._timetoken, startMoney, lblStartTime.Text, UtilModel._userSite, UtilModel._ip, UtilModel.distributor, UtilModel._programVersion, termInning); // 전송할 Parameter
                    string encodeStr = "UTF-8";
                    int errorcode = 0;

                    String returnMessage = UtilModel.GetHttpPOST(msg, url, "POST", encodeStr, ref errorcode);
                    JObject jo = JObject.Parse(returnMessage);
                    String _result = jo.SelectToken("result").ToString();
                    String _reason = jo.SelectToken("reason").ToString();

                    logger.Info("UserStatusUpdate : " + _result + " / " + _reason + "  / 배팅 안한 이닝 : " + termInning);

                    if (termInning > 10)
                    {
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        void init270()
        {
            try
            {
                _bettingClosed = false;

                txtLogAdd("파워볼 배팅이 진행 중입니다.", Color.White);
                txtNotice.Text = "알림 : 파워볼 배팅이 진행 중입니다.";

                if (WinStop.Checked) // 당첨 정지 모드
                {
                    for (int _find = 1; _find <= accountNumber; _find++)
                    {
                        ComboBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                        Button _pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                        if (int.Parse(_level.Text) == 1)
                        {
                            _pr.Text = "배팅정지";
                            _pr.ForeColor = Color.White;
                            _pr.BackColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        void beepSound()
        {
            System.Media.SystemSounds.Hand.Play();

            // 도 = 256Hz
            // 레 = 도 * 9/8 = 288Hz
            // 미 = 레 * 10/9 = 320Hz
            // 파 = 미 * 16/15 = 341.3Hz
            // 솔 = 파 * 9/8 = 384Hz
            // 라 = 솔 * 10/9 = 426.6Hz
            // 시 = 라 * 9/8 = 480Hz
            // 도 = 시 * 16/15 = 512Hz (= 처음 도의 2배)
            // 2배 = 높은음, 1/2배 = 낮은음

            Beep(512, 300); // 도 0.3초
            Beep(640, 300); // 미 0.3초
            Beep(768, 300); // 솔 0.3초
        }
        int noticeNum = 0;
        Boolean _picksterRoundChange = false;
        // 시간마다 해야할 작업 정리 오류가 나지 않게 더 세세하게 정리해야한다.
        private void remainingTimer_Tick()
        {
            if (remainingTime > 1)
            {
                if (remainingTime % 10 == 0)
                {
                    if (UtilModel.notice.Length > 0)
                    {
                        if (UtilModel.notice.Length <= noticeNum)
                        {
                            noticeNum = 0;
                        }
                        textBoxNotice.Text = UtilModel.notice[noticeNum];
                        noticeNum++;
                    }
                    /*
                    if (remainingTime >= 50 && remainingTime <= 240)
                    {
                        int _nextLevelAllBettingMony = 0;
                        for (int _find = 1; _find <= accountNumber; _find++)
                        {
                            // btnPR1
                            Button _boxPickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                            if (_boxPickster.Text.Contains("--"))
                            {
                                continue;
                            }
                            Button pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                            TextBox pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                            Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                            TextBox _targetTextBox1 = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _targetTextBox3 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                            int money = int.Parse(_targetTextBox3.Text);
                            string type = _targetTextBox1.Text;
                            if (!pr.Text.Contains("진행"))
                            {
                                continue;
                            }
                            if (type.Contains("파워"))
                            {

                                if (pick.Text.Contains("홀"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        pOddMoney += money;
                                    }
                                    else
                                    {
                                        pEvenMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("짝"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        pEvenMoney += money;
                                    }
                                    else
                                    {
                                        pOddMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("언더"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        pUnderMoney += money;
                                    }
                                    else
                                    {
                                        pOverMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("오버"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        pOverMoney += money;
                                    }
                                    else
                                    {
                                        pUnderMoney += money;
                                    }

                                }
                            }
                            else if (type.Contains("일반"))
                            {
                                if (pick.Text.Contains("홀"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        nOddMoney += money;
                                    }
                                    else
                                    {
                                        nEvenMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("짝"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        nEvenMoney += money;
                                    }
                                    else
                                    {
                                        nOddMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("언더"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        nUnderMoney += money;
                                    }
                                    else
                                    {
                                        nOverMoney += money;
                                    }
                                }
                                else if (pick.Text.Contains("오버"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        nOverMoney += money;
                                    }
                                    else
                                    {
                                        nUnderMoney += money;
                                    }
                                }
                            }
                            else if (type.Contains("파사"))
                            {
                                if (pick.Text.Contains("홀"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladderOddMoney += money;
                                    }
                                    else
                                    {
                                        ladderEvenMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("짝"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladderEvenMoney += money;
                                    }
                                    else
                                    {
                                        ladderOddMoney += money;
                                    }

                                }
                                else if (pick.Text.Contains("3"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladder3Money += money;
                                    }
                                    else
                                    {
                                        ladder4Money += money;
                                    }
                                }
                                else if (pick.Text.Contains("4"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladder4Money += money;
                                    }
                                    else
                                    {
                                        ladder3Money += money;
                                    }
                                }
                                else if (pick.Text.Contains("좌"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladderLeftMoney += money;
                                    }
                                    else
                                    {
                                        ladderRightMoney += money;
                                    }
                                }
                                else if (pick.Text.Contains("우"))
                                {
                                    if (_follow.Equals("따라가기"))
                                    {
                                        ladderRightMoney += money;
                                    }
                                    else
                                    {
                                        ladderLeftMoney += money;
                                    }
                                }
                            }
                            _nextLevelAllBettingMony += money;
                        }
                        int allMoney = pOddMoney + pEvenMoney + nOddMoney + nEvenMoney + pUnderMoney + pOverMoney + nUnderMoney + nOverMoney + ladder3Money + ladder4Money + ladderOddMoney + ladderEvenMoney + ladderLeftMoney + ladderRightMoney;
                        lblPSingle.Text = UtilModel.StringFormatChanged(pOddMoney) + " 원";
                        lblPPair.Text = UtilModel.StringFormatChanged(pEvenMoney) + " 원";
                        lblPUnder.Text = UtilModel.StringFormatChanged(pUnderMoney) + " 원";
                        lblPOver.Text = UtilModel.StringFormatChanged(pOverMoney) + " 원";
                        lblNSingle.Text = UtilModel.StringFormatChanged(nOddMoney) + " 원";
                        lblNPair.Text = UtilModel.StringFormatChanged(nEvenMoney) + " 원";
                        lblNUnder.Text = UtilModel.StringFormatChanged(nUnderMoney) + " 원";
                        lblNOver.Text = UtilModel.StringFormatChanged(nOverMoney) + " 원";
                       // _powerLadderLeftRight.Text = UtilModel.StringFormatChanged(ladderLeftMoney) + "/" + UtilModel.StringFormatChanged(ladderRightMoney);
                        //_powerLadder34.Text = UtilModel.StringFormatChanged(ladder3Money) + "/" + UtilModel.StringFormatChanged(ladder4Money);
                        //_powerLadderOddEven.Text = UtilModel.StringFormatChanged(ladderOddMoney) + "/" + UtilModel.StringFormatChanged(ladderEvenMoney);
                        groupBoxBettingStatus.Text = "[" + (allinning - 1) + "회][" + (todayinning - 1) + "] 총 배팅 예상 금액 : " + UtilModel.StringFormatChanged(allMoney) + " 원";
                        pOddMoney = 0;
                        pEvenMoney = 0;
                        nOddMoney = 0;
                        nEvenMoney = 0;
                        pUnderMoney = 0;
                        pOverMoney = 0;
                        nUnderMoney = 0;
                        nOverMoney = 0;
                        ladder3Money = 0;
                        ladder4Money = 0;
                        ladderEvenMoney = 0;
                        ladderOddMoney = 0;
                        ladderLeftMoney = 0;
                        ladderRightMoney = 0;
                    }
                    */
                }
                setTimeRemaining(remainingTime--);
                if (remainingTime == 290)
                {
                    try
                    {
                        powerballResult();
                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            UtilModel.notice = webClient.DownloadString(UtilModel.noticeUrl).Split(new char[] { '|' });
                        }
                    }
                    catch (Exception _ex)
                    {
                        logger.Error(_ex.ToString() + "공지사항을 읽어오는데 실패하였습니다.");
                    }
                }
                if (remainingTime == 285)
                {
                    getPowerballInformation();
                    _picksterRoundChange = false;
                    init285();
                }
                if (remainingTime <= 273 && remainingTime >= 271 && !getPowerballinningAndPowerBallResult)
                {
                    init275();
                }

                if (remainingTime == 270 && !getPowerBallResultProcessing)
                {
                    init272();
                }

                if (remainingTime == 265)
                {
                    init270();
                    picksterNameUnLock();

                    remainTime.ForeColor = Color.Black;
                    remainTime.BackColor = Color.White;
                    getPicksterAndRemainingtimeReload = false;
                }
                if (remainingTime == 260
                    || remainingTime == 240
                    || remainingTime == 180 && !updateOneMinute.Checked
                    || remainingTime == 150
                    || remainingTime == 120 && !updateOneMinute.Checked
                    || remainingTime == 90
                    || remainingTime >= 55 && remainingTime <= 60)
                {
                    if (getPicksterAndRemainingtimeReload == false)
                    {
                        getPicksterAndRemainingtimeReload = true;
                        pickPickster();
                        getPowerballInformation();
                    }
                }

                if (remainingTime == 250)
                {
                    // 보유금액과 배팅금액을 체크하여 배팅금액이 보유금액보다 클 시에는 경고 창을 보내자;
                    int _nextLevelAllBettingMony = 0;
                    for (int _find = 1; _find <= accountNumber; _find++)
                    {
                        // btnPR1
                        Button _boxPickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                        if (_boxPickster.Text.Contains("--"))
                        {
                            continue;
                        }
                        Button _pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                        if (_pr.Text.Contains("진행"))
                        {
                            TextBox _bettingMoney = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                            _nextLevelAllBettingMony += int.Parse(_bettingMoney.Text);
                        }
                    }
                    if (NowMoney > 0)
                    {
                        int _gab = _nextLevelAllBettingMony - NowMoney;
                        if (_gab > 0)
                        {
                            beepSound();
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 다음 단계의 금액이 [" + _gab + "원] 부족합니다." +
                                "\r\n\r\n충전이 필요합니다." +
                                "\r\n\r\n단 충전을 하였을 경우 다음 배팅 후" +
                                "\r\n\r\n금액이 적용되오니 신경 쓰지 말아주시기 바랍니다.");
                        }
                    }
                }

                if (remainingTime == 255 || remainingTime == 230 || remainingTime == 200 || remainingTime == 170 || remainingTime == 140 || remainingTime == 110 || remainingTime == 83 || remainingTime == 68 || remainingTime == 10)
                {
                    getPicksterAndRemainingtimeReload = false;
                }

                if (remainingTime == 85 && autoModeRadioButton.Checked && !_automodeChange)
                {
                    try
                    {
                        _automodeChange = true;
                        if (!autoRefill())
                        {
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + allinning + " 회차는 픽스터의 픽이 부족해 통과합니다.");
                        }
                        else
                        {
                            txtLogAdd("자동모드가 진행중입니다. 새롭게 40명을 교체하였습니다.", Color.White);
                        }
                    }
                    catch (Exception _ex)
                    {
                        txtLogAdd(_ex.ToString(), Color.White);
                    }
                }

                // GodPickUseCheck
                // systemPickRandomUseCheck
                // userPickPowerBallGameUseCheck
                // userPickNtryUseCheck
                if (systemPickUseCheck.Checked)
                {
                    if (remainingTime < 110 && remainingTime > 65 && !_picksterRoundChange)
                    {
                        int picksterRound = int.Parse(picksterChangeRound.Text);
                        if (picksterRound > 0)
                        { // 로봇일 경우 해당 라운드 이상일 경우 교체한다.
                            picksterRoundChange(picksterRound);
                            _picksterRoundChange = true;
                        }
                    }
                }
                else if (userPickPowerBallGameUseCheck.Checked || userPickNtryUseCheck.Checked)
                {
                    if (remainingTime < 55 && remainingTime > 51 && !_picksterRoundChange)
                    {
                        int picksterRound = int.Parse(picksterChangeRound.Text);
                        if (picksterRound > 0)
                        { // 로봇일 경우 해당 라운드 이상일 경우 교체한다.
                            picksterRoundChange(picksterRound);
                            updateBettingUserInformation();
                            _picksterRoundChange = true;
                        }
                    }
                }
                else if (PatternPickUse.Checked)
                {
                    if (remainingTime < 55 && remainingTime > 51 && !_picksterRoundChange)
                    {
                        patternBet();
                    }
                }

                int _picksterNumber = 0;
                int _automodelimit = 0;
                if (remainingTime == 60)
                {
                    txtNotice.Text = "알림 : 잠시 후 배팅이 마감 됩니다.";
                    if (!autoModeRadioButton.Checked || !superiorityModeRadioButton.Checked)
                    {
                        String pP = "";

                        for (int i = 0; i < arrayNum; i++)
                        {
                            if (_picksterInformation[i, 0] == null)
                            {
                                break;
                            }
                            if (_picksterInformation[i, 0] != null)
                            {
                                // 파워픽 4, 일반픽 17
                                pP = _picksterInformation[i, 4];
                                if (!pP.Contains("P"))
                                {
                                    _picksterNumber++;
                                }
                            }
                        }

                        _automodelimit = int.Parse(automodelimit.Text);
                        if (_picksterNumber < _automodelimit)
                        {
                            txtLogAdd("현재 픽스터의 숫자가 매우 부족합니다.", Color.White);
                        }
                    }
                }
                if (remainingTime == 52 && superiorityModeRadioButton.Checked)
                {
                    superiorityModeProcessing(_picksterNumber, _automodelimit);
                }

                if (!_bettingClosed && remainingTime <= 45 && remainingTime >= 35)
                {
                    _bettingClosed = true;
                    txtLogAdd(allinning + "회 배팅이 마감되었습니다.", Color.White);
                    txtNotice.Text = "알림 : 배팅이 마감되었습니다. 등록 중입니다.";
                    bet();
                    picksterNameLock();
                    remainTime.ForeColor = Color.White;
                    remainTime.BackColor = Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                    Double gain = NowMoney - startMoney;
                    if (gain > 0)
                    {
                        lblTxtNowGain.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(112)))), ((int)(((byte)(67)))));
                        lblTxtNowGain.Text = "▲ " + UtilModel.StringFormatChanged((NowMoney - startMoney)) + " 원";
                    }
                    else if (gain < 0)
                    {
                        lblTxtNowGain.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(112)))), ((int)(((byte)(67)))));
                        lblTxtNowGain.Text = "▼ " + UtilModel.StringFormatChanged((NowMoney - startMoney)) + " 원";
                    }
                    ScreenCapture(this.Width, this.Height, this.Location);
                    getPowerballInformation();
                }

                if (remainingTime == 10)
                {
                    txtLogAdd("잠시 후 파워볼 [" + allinning + "]회 결과가 나올 예정입니다.", Color.White);
                    txtNotice.Text = "알림 : 잠시 후 파워볼 [" + allinning + "]회 결과가 나올 예정입니다.";
                    refreshBettingInformation = false;
                }
            }
            else
            {
                remainingTime = 300;
                if (!refreshBettingInformation)
                {
                    getPowerballInformation();
                    txtLogAdd("잠시 후 다음 회차가 진행될 예정입니다.", Color.White);
                    txtNotice.Text = "알림 : 잠시 후 다음 회차가 진행될 예정입니다.";
                    _picksterInformation = new string[arrayNum, array2Num];
                    refreshBettingInformation = true;
                    _automodeChange = false;
                    getPowerballinningAndPowerBallResult = false;
                    getPowerBallResultProcessing = false;
                    btnPicksterChoicePowerOddNum.Text = "-";
                    btnPicksterChoicePowerEvenNum.Text = "-";
                    btnPicksterChoicePowerOverNum.Text = "-";
                    btnPicksterChoicePowerUnderNum.Text = "-";

                    btnPicksterChoicePowerOddPercent.Text = "-";
                    btnPicksterChoicePowerEvenPercent.Text = "-";
                    btnPicksterChoicePowerOverPercent.Text = "-";
                    btnPicksterChoicePowerUnderPercent.Text = "-";

                    btnPicksterChoiceNormalOddNum.Text = "-";
                    btnPicksterChoiceNormalEvenNum.Text = "-";
                    btnPicksterChoiceNormalOverNum.Text = "-";
                    btnPicksterChoiceNormalUnderNum.Text = "-";

                    btnPicksterChoiceNormalOddPercent.Text = "-";
                    btnPicksterChoiceNormalEvenPercent.Text = "-";
                    btnPicksterChoiceNormalOverPercent.Text = "-";
                    btnPicksterChoiceNormalUnderPercent.Text = "-";
                }
            }
        }

        void patternBet()
        {
            int patternbetnumber = 0;
            bool result = int.TryParse(patternBetNumber.Text, out patternbetnumber);
            if (result && patternbetnumber > 0)
            {
                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    TextBox _ps = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                    if (string.IsNullOrEmpty(_ps.Text))
                    {
                        continue;
                    }
                    int streak = 0;
                    bool streakBool = int.TryParse(Regex.Replace(_ps.Text, @"\D", ""), out streak);
                    if (streakBool && streak < patternbetnumber)
                    {
                        TextBox _BoxPick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        _BoxPick.Text = "통과";
                        _BoxPick.ForeColor = Color.Black;
                    }
                    else
                    {
                        if (patternAutoChange.Checked)
                        {
                            Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                            if (_ps.Text.Contains("연승"))
                            {
                                _follow.ForeColor = Color.White;
                                _follow.BackColor = Color.Black;
                                _follow.Text = "반대로";
                            }
                            else if (_ps.Text.Contains("연패"))
                            {
                                _follow.ForeColor = Color.Black;
                                _follow.BackColor = Color.White;
                                _follow.Text = "따라가기";
                            }
                        }
                    }
                }
            }
            _picksterRoundChange = true;
        }
        void superiorityModeProcessing(int _picksterNumber, int _automodelimit)
        {
            try
            {
                String pP = "";

                for (int i = 0; i < arrayNum; i++)
                {
                    if (_picksterInformation[i, 0] == null)
                    {
                        break;
                    }
                    if (_picksterInformation[i, 0] != null)
                    {
                        // 파워픽 4, 일반픽 17
                        pP = _picksterInformation[i, 4];
                        if (!pP.Contains("P"))
                        {
                            _picksterNumber++;
                        }
                    }
                }

                _automodelimit = int.Parse(automodelimit.Text);
                if (_picksterNumber < _automodelimit)
                {
                    txtLogAdd("현재 픽스터의 숫자가 매우 부족합니다. 엔트리 문제로 제대로 읽어오지 못하였습니다.", Color.White);
                }

                String poweroddeven = "통과";
                String poweroverunder = "통과";
                String normaloddeven = "통과";
                String normaloverunder = "통과";
                txtLogAdd(_picksterNumber + " 명 픽 완료. " + _automodelimit + " 최소 픽유저", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                if (poweroddPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_powerballOdd >= _automodelimit) // 홀이 더 크다면
                    {
                        poweroddeven = "홀";
                        txtLogAdd(poweroddPercent.ToString("F") + " %로 파워볼 홀이 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[파워홀 선택 유저 : " + _powerballOdd + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }
                else if (powerevenPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_automodelimit <= _powerballEven)
                    {
                        poweroddeven = "짝";
                        txtLogAdd(powerevenPercent.ToString("F") + " %로 파워볼 짝이 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[파워짝 선택 유저 : " + _powerballEven + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }

                if (poweroverPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_powerballOver >= _automodelimit) // 홀이 더 크다면
                    {
                        poweroverunder = "오버";
                        txtLogAdd(poweroverPercent.ToString("F") + " %로 파워볼 오버가 선택되었습니다", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                    else
                    {
                        txtLogAdd("[파워오버 선택 유저 : " + _powerballOver + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }
                else if (powerunderPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_automodelimit <= _powerballUnder)
                    {
                        poweroverunder = "언더";
                        txtLogAdd(powerunderPercent.ToString("F") + " %로 파워볼 언더가 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[파워언더 선택 유저 : " + _powerballUnder + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }

                if (normaloddPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_normalballOdd >= _automodelimit) // 홀이 더 크다면
                    {
                        normaloddeven = "홀";
                        txtLogAdd(normaloddPercent.ToString("F") + " %로 일반볼 홀이 선택되었습니다", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                    else
                    {
                        txtLogAdd("[일반홀 선택 유저 : " + _normalballOdd + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }
                else if (normalevenPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_automodelimit <= _normalballEven)
                    {
                        normaloddeven = "짝";
                        txtLogAdd(normalevenPercent.ToString("F") + " %로 일반볼 짝이 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[일반짝 선택 유저 : " + _normalballEven + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }

                if (normaloverPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_normalballOver >= _automodelimit) // 홀이 더 크다면
                    {
                        normaloverunder = "오버";
                        txtLogAdd(normaloverPercent.ToString("F") + " %로 일반볼 오버가 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[일반오버 선택 유저 : " + _normalballOver + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }
                else if (normalunderPercent >= int.Parse(superiorModePercent.Text))
                {
                    if (_automodelimit <= _normalballUnder)
                    {
                        normaloverunder = "언더";
                        txtLogAdd(normalunderPercent.ToString("F") + " %로 일반볼 언더가 선택되었습니다", Color.White);
                    }
                    else
                    {
                        txtLogAdd("[일반언더 선택 유저 : " + _normalballUnder + "] [" + _automodelimit + " 최소 픽유저] 최소 픽유저 미만 통과", Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246))))));
                    }
                }

                // _picksterInformation[k, 0]
                _superioritymodeSetting(poweroddeven, poweroverunder, normaloddeven, normaloverunder);
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        void picksterRoundChange(int picksterRound)
        {
            try
            {
                //samePersonCheckBox
                int _winNumber = 0;
                int _loseNumber = 0;

                if (!samePersonCheckBox.Checked)
                {
                    Boolean containPicksterName = false;
                    List<String> CopyWinPicksterName = new List<String>(winPicksterName);
                    winPicksterName.Clear();
                    for (int _i = 0; _i < CopyWinPicksterName.Count; _i++)
                    {
                        containPicksterName = false;
                        if (!string.IsNullOrEmpty(CopyWinPicksterName[_i]))
                        {
                            for (int _find = 1; _find <= accountNumber; _find++)
                            {
                                Button _boxPickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);

                                if (_boxPickster.Text.Equals(CopyWinPicksterName[_i]))
                                {
                                    containPicksterName = true;
                                    break;
                                }
                            }
                            if (!containPicksterName)
                            {
                                winPicksterName.Add(CopyWinPicksterName[_i]);
                            }
                        }
                    }
                    List<String> CopyLosePicksterName = new List<String>(losePicksterName);
                    losePicksterName.Clear();
                    for (int _i = 0; _i < CopyLosePicksterName.Count; _i++)
                    {
                        containPicksterName = false;
                        if (!string.IsNullOrEmpty(CopyLosePicksterName[_i]))
                        {
                            for (int _find = 1; _find <= accountNumber; _find++)
                            {
                                Button _boxPickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                                if (_boxPickster.Text.Equals(CopyLosePicksterName[_i]))
                                {
                                    containPicksterName = true;
                                    break;
                                }
                            }
                            if (!containPicksterName)
                            {
                                losePicksterName.Add(CopyLosePicksterName[_i]);
                            }
                        }
                    }
                    losePicksterName.Shuffle();
                    winPicksterName.Shuffle();
                }
                else
                {
                    winPicksterName.Shuffle();
                    losePicksterName.Shuffle();
                }

                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    // btnPR1
                    int _picksterRound = _picksterRoundArray[(_find - 1)];

                    Button _boxPickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);

                    if (_picksterRound >= picksterRound || _boxPickster.Text.Contains("--"))
                    {
                        // CBL1
                        ComboBox _cbl = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                        int cbl = int.Parse(_cbl.Text);
                        if (cbl <= 1)
                        {
                            Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                            TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                            TextBox _ps = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                            TextBox _pr = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                            Button _bpr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);

                            if (_bpr.Text.Contains("정지"))
                            {
                                continue;
                            }
                            _boxPickster.Text = "------";
                            _pick.Text = "";
                            _ps.Text = "0연승";
                            _pr.Text = "0승0패";
                            if (_follow.Text.Contains("반대")) // 반대로일 경우엔 연승자를
                            {
                                if (winPicksterName.Count > 0)
                                {
                                    if (_winNumber < winPicksterName.Count)
                                    {
                                        //Boolean _bool = false;
                                        if (winPicksterName[_winNumber] != null)
                                        {
                                            String rbtName = winPicksterName[_winNumber];
                                            txtLogAdd("[" + _boxPickster.Text + "] => [" + rbtName + " ]교체", Color.White);
                                            _picksterRoundArray[(_find - 1)] = 0;
                                            _boxPickster.Text = rbtName;
                                            _pick.Text = "";
                                            _ps.Text = "0연승";
                                            _pr.Text = "0승0패";
                                            _winNumber++;
                                        }
                                    }
                                }
                            }
                            else if (_follow.Text.Contains("따라가기"))// 따라가기일 경우 연패자를 넣는다.
                            {
                                if (losePicksterName.Count > 0)
                                {
                                    if (_loseNumber < losePicksterName.Count)
                                    {
                                        if (losePicksterName[_loseNumber] != null)
                                        {
                                            String rbtName = losePicksterName[_loseNumber];
                                            txtLogAdd("[" + _boxPickster.Text + "] => [" + rbtName + " ]교체", Color.White);
                                            _picksterRoundArray[(_find - 1)] = 0;
                                            _boxPickster.Text = rbtName;
                                            _pick.Text = "";
                                            _ps.Text = "0연승";
                                            _pr.Text = "0승0패";
                                            _loseNumber++;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                txtLogAdd("자동 픽스터 교체 완료!", Color.White);
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        private void ScreenCapture(int intBitmapWidth, int intBitmapHeight, Point ptSource)
        {
            try
            {
                Bitmap bitmap = new Bitmap(intBitmapWidth, intBitmapHeight);
                Graphics g = Graphics.FromImage(bitmap);

                g.CopyFromScreen(ptSource, new Point(0, 0), new Size(intBitmapWidth, intBitmapHeight));

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HHmmss.fff", CultureInfo.InvariantCulture);

                bitmap.Save("./screenshot/" + UtilModel.betid + "." + timestamp + ".png", ImageFormat.Png);
                logger.Info(timestamp + ".png 파일로 저장 완료.");
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        void picksterNameLock()
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                _pickster.ForeColor = Color.White;
                _pickster.BackColor = Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            }
        }
        void picksterNameUnLock()
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                _pickster.ForeColor = Color.Black;
                _pickster.BackColor = Color.White;
            }
        }
        void _superioritymodeSetting(String poweroddeven, String poweroverunder, String normaloddeven, String normaloverunder)
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                if (_find <= 10)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                    TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                    TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                    _pickster.Text = "[파워]홀짝";
                    _ballType.Text = "파워";
                    _pick.Text = poweroddeven; // 홀짝
                    _pick.ForeColor = Color.Black;
                    PR.Text = "---";
                    PR.ForeColor = Color.White;
                    PS.Text = "---";
                    PS.ForeColor = Color.White;
                }
                else if (_find <= 20)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                    TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                    TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                    _pickster.Text = "[일반]홀짝";
                    _ballType.Text = "일반";
                    _pick.Text = normaloddeven; // 오버 언더
                    _pick.ForeColor = Color.Black;
                    PR.Text = "---";
                    PR.ForeColor = Color.White;
                    PS.Text = "---";
                    PS.ForeColor = Color.White;
                }
                else if (_find <= 30)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                    TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                    TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                    _pickster.Text = "[파워]언오버";
                    _ballType.Text = "파워";
                    _pick.Text = poweroverunder; // 홀짝
                    _pick.ForeColor = Color.Black;
                    PR.Text = "---";
                    PR.ForeColor = Color.White;
                    PS.Text = "---";
                    PS.ForeColor = Color.White;
                }
                else if (_find <= 40)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                    TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                    TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                    _pickster.Text = "[일반]언오버";
                    _ballType.Text = "일반";
                    _pick.Text = normaloverunder; // 오버 언더
                    _pick.ForeColor = Color.Black;
                    PR.Text = "---";
                    PR.ForeColor = Color.White;
                    PS.Text = "---";
                    PS.ForeColor = Color.White;
                }
            }
        }
        private Boolean autoRefill()
        {
            String pP = "";

            int n = 1;
            for (int i = 0; i < arrayNum; i++)
            {
                if (_picksterInformation[i, 0] == null)
                {
                    break;
                }
                if (_picksterInformation[i, 0] != null)
                {
                    // 파워픽 4, 일반픽 17
                    pP = _picksterInformation[i, 4];
                    if (!pP.Contains("P"))
                    {
                        n++;
                    }
                }
            }
            int _automodelimit = int.Parse(automodelimit.Text);
            if (n >= _automodelimit)
            {
                String[] arrayPickster = new String[n];
                int num = 0;
                for (int i = 0; i < arrayNum; i++)
                {
                    if (_picksterInformation[i, 0] == null)
                    {
                        break;
                    }
                    if (_picksterInformation[i, 0] != null)
                    {
                        // 파워픽 4, 일반픽 17
                        pP = _picksterInformation[i, 4];
                        if (!pP.Contains("P"))
                        {
                            arrayPickster[num++] = _picksterInformation[i, 0];
                        }
                    }
                }

                Random rand = new Random(); //랜덤선언 _picksterInformation[k, 0]
                int r = 0;

                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    r = rand.Next(0, (arrayPickster.Length - 1));
                    String strpickster = arrayPickster[r];
                    if (strpickster != null)
                    {
                        for (int _find2 = 1; _find2 <= accountNumber; _find2++)
                        {
                            Button _pickster = (Controls.Find("BoxPickster" + _find2.ToString(), true)[0] as Button);
                            if (_pickster.Text.Equals(strpickster))
                            {
                                r = rand.Next(0, (arrayPickster.Length - 1));
                                strpickster = arrayPickster[r];
                                break;
                            }
                        }

                        for (int i = 0; i < arrayNum; i++)
                        {
                            if (_picksterInformation[i, 0] == null)
                            {
                                break;
                            }
                            if (_picksterInformation[i, 0].Equals(strpickster))
                            {
                                Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                                TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                                TextBox _boxPr = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                                TextBox _boxPs = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                                _pickster.Text = _picksterInformation[i, 0].ToString(); // 픽스터 이름
                                _ballType.Text = _picksterInformation[i, 1].ToString(); // 픽스터 이름
                                _boxPr.Text = _picksterInformation[i, 2].ToString(); // 픽스터 이름
                                _boxPs.Text = _picksterInformation[i, 3].ToString(); // 픽스터 이름
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                txtLogAdd("[" + n + "] 픽을 한 픽스터가 " + _automodelimit + "명 이하라서 통과하였습니다.", Color.White);
                return false;
            }
        }

        Boolean _soundBeep = false;
        // 결과를 참고하여 단계 및 배팅금 조정, 색깔 조정
        private void powerBallResultProcessing()
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _btnPR = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                //btnPR02
                if (_btnPR.Text.Equals("배팅진행"))
                {
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                    if (!_pick.Text.Contains("통"))
                    {
                        Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                        TextBox _betMoney = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                        TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                        ComboBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                        Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                        _powerBallResultProcessing(_pickster, _ballType, _pick, _level, _betMoney, _follow);
                    }
                }
            }

            if (powerballOddEven.Equals("홀"))
            {
                pOddMoney = (int)(pOddMoney * 1.95);
                pEvenMoney = 0;
            }
            if (powerballOddEven.Equals("짝"))
            {
                pEvenMoney = (int)(pEvenMoney * 1.95);
                pOddMoney = 0;
            }
            if (powerballUnderOver.Equals("오버"))
            {
                pOverMoney = (int)(pOverMoney * 1.95);
                pUnderMoney = 0;
            }
            if (powerballUnderOver.Equals("언더"))
            {
                pUnderMoney = (int)(pUnderMoney * 1.95);
                pOverMoney = 0;
            }

            if (normalballOddEven.Equals("홀"))
            {
                nOddMoney = (int)(nOddMoney * 1.95);
                nEvenMoney = 0;
            }
            if (normalballOddEven.Equals("짝"))
            {
                nEvenMoney = (int)(nEvenMoney * 1.95);
                nOddMoney = 0;
            }
            if (normalballUnderOver.Equals("오버"))
            {
                nOverMoney = (int)(nOverMoney * 1.95);
                nUnderMoney = 0;
            }
            if (normalballUnderOver.Equals("언더"))
            {
                nUnderMoney = (int)(nUnderMoney * 1.95);
                nOverMoney = 0;
            }
            if (powerBallLadder34.Equals("3"))
            {
                ladder3Money = (int)(ladder3Money * 1.95);
                ladder4Money = 0;
            }
            if (powerBallLadder34.Equals("4"))
            {
                ladder4Money = (int)(ladder4Money * 1.95);
                ladder3Money = 0;
            }
            if (powerBallLadderLeftRight.Equals("좌"))
            {
                ladderLeftMoney = (int)(ladderLeftMoney * 1.95);
                ladderRightMoney = 0;
            }
            if (powerBallLadderLeftRight.Equals("우"))
            {
                ladderRightMoney = (int)(ladderRightMoney * 1.95);
                ladderLeftMoney = 0;
            }
            if (powerBallLadderOddEven.Equals("홀"))
            {
                ladderOddMoney = (int)(ladderOddMoney * 1.95);
                ladderEvenMoney = 0;
            }
            if (powerBallLadderOddEven.Equals("짝"))
            {
                ladderEvenMoney = (int)(ladderEvenMoney * 1.95);
                ladderOddMoney = 0;
            }
            if (_soundBeep)
            {
                Beep(512, 300); // 도 0.3초
            }
            _soundBeep = false;
        }

        // 레벨별 글자색과 배경색 설정
        private void _levelColorSetting(ComboBox _level, TextBox _betMoney)
        {
            // Color.FromArgb(r, g, b);
            int level = int.Parse(_level.Text) + 1;
            if (level == 1)
            {
                if (alarmCheckBox1.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.Black;
                _betMoney.BackColor = Color.White;
            }
            else if (level == 2)
            {
                if (alarmCheckBox2.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.Black;
                _betMoney.BackColor = Color.FromArgb(255, 205, 210);
            }
            else if (level == 3)
            {
                if (alarmCheckBox3.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.Black;
                _betMoney.BackColor = Color.FromArgb(239, 154, 154);
            }
            else if (level == 4)
            {
                if (alarmCheckBox4.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(229, 115, 115);
            }
            else if (level == 5)
            {
                if (alarmCheckBox5.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(239, 83, 80);
            }
            else if (level == 6)
            {
                if (alarmCheckBox6.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(244, 67, 54);
            }
            else if (level == 7)
            {
                if (alarmCheckBox7.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(229, 57, 53);
            }
            else if (level == 8)
            {
                if (alarmCheckBox8.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(211, 47, 47);
            }
            else if (level == 9)
            {
                if (alarmCheckBox9.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(198, 40, 40);
            }
            else if (level == 10)
            {
                if (alarmCheckBox10.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(183, 28, 28);
            }
            else if (level == 11)
            {
                if (alarmCheckBox11.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(74, 20, 140);
            }
            else if (level == 12)
            {
                if (alarmCheckBox12.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(74, 20, 140);
            }
            else if (level == 13)
            {
                if (alarmCheckBox13.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(74, 20, 140);
            }
            else if (level == 14)
            {
                if (alarmCheckBox14.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(74, 20, 140);
            }
            else if (level >= 15)
            {
                if (alarmCheckBox15.Checked)
                {
                    _soundBeep = true;
                }
                _betMoney.ForeColor = Color.White;
                _betMoney.BackColor = Color.FromArgb(74, 20, 140);
            }

            TextBox _MoneySettingLevel = (Controls.Find("txtBtMoneySettingL" + level.ToString(), true)[0] as TextBox);
            if (_MoneySettingLevel.Text.Equals("-1"))
            {
                _MoneySettingLevel = (Controls.Find("txtBtMoneySettingL1", true)[0] as TextBox);

                int outValue = 0;
                bool _b = int.TryParse(Regex.Replace(_MoneySettingLevel.Text, @"\D", ""), out outValue);
                if (_b)
                {
                    _betMoney.Text = outValue.ToString();
                    _level.Text = "1";
                    _level.ForeColor = Color.Black;
                    _level.BackColor = Color.White;
                }
            }
            else
            {
                int outValue = 0;
                bool _b = int.TryParse(Regex.Replace(_MoneySettingLevel.Text, @"\D", ""), out outValue);
                if (_b)
                {
                    _level.Text = level.ToString();
                    _betMoney.Text = outValue.ToString();
                }
            }
        }

        // 결과값이 나왔을 경우 처리하는 부분.
        private void _powerBallResultProcessing(Button _pickster, TextBox _ballType, TextBox _pick, ComboBox _level, TextBox _betMoney, Button _follow)
        {

            if (_ballType.Text.Equals("파워"))
            {
                // 따라가기 배팅시 처리
                if (_follow.Text.Equals("따라가기"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (_pick.Text.Equals(powerballOddEven)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";

                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("언더") || _pick.Text.Equals("오버"))
                    {
                        if (_pick.Text.Equals(powerballUnderOver)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballUnderOver);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballUnderOver);
                        }
                    }
                    // 반대로 배팅시 처리
                }
                else if (_follow.Text.Equals("반대로"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (!_pick.Text.Equals(powerballOddEven)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("언더") || _pick.Text.Equals("오버"))
                    {
                        if (!_pick.Text.Equals(powerballUnderOver)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballUnderOver);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballUnderOver);
                        }
                    }
                }
            }
            else if (_ballType.Text.Equals("일반"))
            {
                // 따라가기 배팅시 처리
                if (_follow.Text.Equals("따라가기"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (_pick.Text.Equals(normalballOddEven)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("언더") || _pick.Text.Equals("오버"))
                    {
                        if (_pick.Text.Equals(normalballUnderOver)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballUnderOver);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballUnderOver);
                        }
                    }
                    // 반대로 배팅시 처리
                }
                else if (_follow.Text.Equals("반대로"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (!_pick.Text.Equals(normalballOddEven)) // 파워볼 홀짝
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("언더") || _pick.Text.Equals("오버"))
                    {
                        if (!_pick.Text.Equals(normalballUnderOver)) // 일반볼 언더오버
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballUnderOver);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballUnderOver);
                        }
                    }
                }
            }
            else if (_ballType.Text.Equals("파사")) // powerladder 파워사다리
            {
                // 따라가기 배팅시 처리
                if (_follow.Text.Equals("따라가기"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (_pick.Text.Equals(powerBallLadderOddEven)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadderOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadderOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("3") || _pick.Text.Equals("4"))
                    {
                        if (_pick.Text.Equals(powerBallLadder34)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadder34);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadder34);
                        }
                    }
                    else if (_pick.Text.Equals("좌") || _pick.Text.Equals("우"))
                    {
                        if (_pick.Text.Equals(powerBallLadderLeftRight)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadderLeftRight);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadderLeftRight);
                        }
                    }
                    // 반대로 배팅시 처리
                }
                else if (_follow.Text.Equals("반대로"))
                {
                    if (_pick.Text.Equals("홀") || _pick.Text.Equals("짝"))
                    {
                        if (!_pick.Text.Equals(powerBallLadderOddEven)) // 파워볼 홀짝
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadderOddEven);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadderOddEven);
                        }
                    }
                    else if (_pick.Text.Equals("3") || _pick.Text.Equals("4"))
                    {
                        if (!_pick.Text.Equals(powerBallLadder34)) // 일반볼 언더오버
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadder34);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadder34);
                        }
                    }
                    else if (_pick.Text.Equals("좌") || _pick.Text.Equals("우"))
                    {
                        if (!_pick.Text.Equals(powerBallLadderLeftRight)) // 일반볼 언더오버
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            int outValue = 0;
                            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
                            if (_b)
                            {
                                _betMoney.Text = outValue.ToString();
                            }
                            else
                            {
                                _betMoney.Text = "0";
                            }
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerBallLadderLeftRight);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워사다리 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerBallLadderLeftRight);
                        }
                    }
                }
            }
        }

        void semiAutoModeProcessing(Button _pickster, ComboBox _level)
        {
            try
            {
                if (semiAutoRadioButton.Checked)
                {
                    if (int.Parse(semiAuto_1.Text) % (int.Parse(_level.Text) + 1) == 0)
                    {
                        if (semiAuto_3.Text.Equals("연승"))
                        {
                            String streak = "";
                            int n = 1;
                            for (int i = 0; i < arrayNum; i++)
                            {
                                if (_picksterInformation[i, 0] == null)
                                {
                                    break;
                                }
                                if (_picksterInformation[i, 0] != null)
                                {
                                    streak = _picksterInformation[i, 3];
                                    if (streak.Contains("연승"))
                                    {
                                        streak = streak.Replace("연승", "");
                                        if (int.Parse(streak) >= int.Parse(semiAuto_2.Text))
                                        {
                                            n++;
                                        }
                                    }
                                }
                            }
                            String[] arrayPickster = new String[n];
                            int num = 0;
                            streak = "";
                            for (int i = 0; i < arrayNum; i++)
                            {
                                if (_picksterInformation[i, 0] == null)
                                {
                                    break;
                                }
                                if (_picksterInformation[i, 0] != null)
                                {
                                    // 파워픽 4, 일반픽 17
                                    streak = _picksterInformation[i, 3]; // 연승 연패 표시
                                                                         //nP = _picksterInformation[i, 17];
                                    if (streak.Contains("연승"))
                                    {
                                        streak = streak.Replace("연승", "");
                                        if (int.Parse(streak) >= int.Parse(semiAuto_2.Text))
                                        {
                                            arrayPickster[num++] = _picksterInformation[i, 0];
                                        }
                                    }
                                }
                            }

                            Random rand = new Random(); //랜덤선언 _picksterInformation[k, 0]
                            int r = 0;
                            for (int _find = 1; _find <= accountNumber; _find++)
                            {
                                r = rand.Next(0, (arrayPickster.Length - 1));
                                String strpickster = arrayPickster[r];

                                if (_find < 10)
                                {
                                    if (strpickster != null)
                                    {
                                        _pickster.Text = strpickster;
                                    }
                                }
                                else
                                {
                                    if (strpickster != null)
                                    {
                                        _pickster.Text = strpickster;
                                    }
                                }
                            }
                        }
                        else if (semiAuto_3.Text.Equals("연패"))
                        {
                            String streak = "";
                            int n = 1;
                            for (int i = 0; i < arrayNum; i++)
                            {
                                if (_picksterInformation[i, 0] == null)
                                {
                                    break;
                                }
                                if (_picksterInformation[i, 0] != null)
                                {
                                    streak = _picksterInformation[i, 3];
                                    if (streak.Contains("연패"))
                                    {
                                        streak = streak.Replace("연패", "");
                                        if (int.Parse(streak) >= int.Parse(semiAuto_2.Text))
                                        {
                                            n++;
                                        }
                                    }
                                }
                            }
                            String[] arrayPickster = new String[n];
                            int num = 0;
                            streak = "";
                            for (int i = 0; i < arrayNum; i++)
                            {
                                if (_picksterInformation[i, 0] == null)
                                {
                                    break;
                                }
                                if (_picksterInformation[i, 0] != null)
                                {
                                    // 파워픽 4, 일반픽 17
                                    streak = _picksterInformation[i, 3]; // 연승 연패 표시
                                                                         //nP = _picksterInformation[i, 17];
                                    if (streak.Contains("연패"))
                                    {
                                        streak = streak.Replace("연패", "");
                                        if (int.Parse(streak) >= int.Parse(semiAuto_2.Text))
                                        {
                                            arrayPickster[num++] = _picksterInformation[i, 0];
                                        }
                                    }
                                }
                            }

                            Random rand = new Random(); //랜덤선언 _picksterInformation[k, 0]
                            int r = 0;
                            for (int _find = 1; _find <= accountNumber; _find++)
                            {
                                r = rand.Next(0, (arrayPickster.Length - 1));
                                String strpickster = arrayPickster[r];

                                if (_find < 10)
                                {
                                    if (strpickster != null)
                                    {
                                        _pickster.Text = strpickster;
                                    }
                                }
                                else
                                {
                                    if (strpickster != null)
                                    {
                                        _pickster.Text = strpickster;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        // 픽 삭제
        private void clearBetInfor()
        {
            pOverMoney = 0;
            pUnderMoney = 0;
            pOddMoney = 0;
            pEvenMoney = 0;
            nOverMoney = 0;
            nUnderMoney = 0;
            nOddMoney = 0;
            nEvenMoney = 0;
            ladder3Money = 0;
            ladder4Money = 0;
            ladderEvenMoney = 0;
            ladderOddMoney = 0;
            ladderLeftMoney = 0;
            ladderRightMoney = 0;
            resultBeforePOddMoney = 0;
            resultBeforePEvenMoney = 0;
            resultBeforePOverMoney = 0;
            resultBeforePUnderMoney = 0;
            resultBeforeNOddMoney = 0;
            resultBeforeNEvenMoney = 0;
            resultBeforeNOverMoney = 0;
            resultBeforeNUnderMoney = 0;

            for (int _find = 1; _find <= accountNumber; _find++)
            {
                var _targetTextBox = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                _targetTextBox.Text = "----";
                _targetTextBox.ForeColor = Color.DarkGray;

                if (!superiorityModeRadioButton.Checked)
                {
                    var _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                    _ballType.Text = "";
                    _ballType.ForeColor = Color.Black;
                }

                var _ps = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                _ps.Text = "----";
                _ps.ForeColor = Color.Black;
            }
        }

        // 배팅 진행. 각각의 배팅금액을 더한다.
        private void runToBet(String _follow, String pr, String type, String pick, int money)
        {
            if (!pr.Contains("진행"))
            {
                return;
            }
            if (type.Contains("파워"))
            {
                if (pick.Contains("홀"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        pOddMoney += money;
                    }
                    else
                    {
                        pEvenMoney += money;
                    }

                }
                else if (pick.Contains("짝"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        pEvenMoney += money;
                    }
                    else
                    {
                        pOddMoney += money;
                    }

                }
                else if (pick.Contains("언더"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        pUnderMoney += money;
                    }
                    else
                    {
                        pOverMoney += money;
                    }

                }
                else if (pick.Contains("오버"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        pOverMoney += money;
                    }
                    else
                    {
                        pUnderMoney += money;
                    }

                }
            }
            else if (type.Contains("일반"))
            {
                if (pick.Contains("홀"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        nOddMoney += money;
                    }
                    else
                    {
                        nEvenMoney += money;
                    }

                }
                else if (pick.Contains("짝"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        nEvenMoney += money;
                    }
                    else
                    {
                        nOddMoney += money;
                    }

                }
                else if (pick.Contains("언더"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        nUnderMoney += money;
                    }
                    else
                    {
                        nOverMoney += money;
                    }
                }
                else if (pick.Contains("오버"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        nOverMoney += money;
                    }
                    else
                    {
                        nUnderMoney += money;
                    }
                }
            }
            else if (type.Contains("파사"))
            {
                if (pick.Contains("홀"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladderOddMoney += money;
                    }
                    else
                    {
                        ladderEvenMoney += money;
                    }
                }
                else if (pick.Contains("짝"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladderEvenMoney += money;
                    }
                    else
                    {
                        ladderOddMoney += money;
                    }

                }
                else if (pick.Contains("3"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladder3Money += money;
                    }
                    else
                    {
                        ladder4Money += money;
                    }
                }
                else if (pick.Contains("4"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladder4Money += money;
                    }
                    else
                    {
                        ladder3Money += money;
                    }
                }
                else if (pick.Contains("좌"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladderLeftMoney += money;
                    }
                    else
                    {
                        ladderRightMoney += money;
                    }
                }
                else if (pick.Contains("우"))
                {
                    if (_follow.Equals("따라가기"))
                    {
                        ladderRightMoney += money;
                    }
                    else
                    {
                        ladderLeftMoney += money;
                    }
                }
            }
        }
        void bettingLevel1()
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                TextBox _BoxPick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                if (_pr.Text.Equals("배팅진행"))
                {
                    Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                    TextBox _targetTextBox1 = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _targetTextBox3 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                    runToBet(_follow.Text, _pr.Text, _targetTextBox1.Text, _BoxPick.Text, int.Parse(_targetTextBox3.Text));
                }
                else
                {
                    _BoxPick.Text = "통과";
                    _BoxPick.ForeColor = Color.Black;
                }
            }
            bettingStatus = false;
        }

        Boolean bettingStatus = false;
        public void bettingLevel2(String _url, String _param, string type)
        {
            try
            {
                if (realOrVirtualMode == 1)
                {
                    if (type.Equals("PSA"))
                    {
                        allbettingMoney += ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
                        if (UtilModel._allBettingEnable == 1)
                        {
                            label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                        }
                        NowMoney = NowMoney - (ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney);

                        lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                        txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                        logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                        logger.Info("[배팅완료][3줄 : " + ladder3Money + "][4줄 : " + ladder4Money + "][홀 : " + ladderOddMoney + "][짝 : " + ladderEvenMoney + "][좌 : " + ladderLeftMoney + "][우 : " + ladderLeftMoney + "]");
                        bettingStatus = true;
                    }
                    else
                    {
                        allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                        if (UtilModel._allBettingEnable == 1)
                        {
                            label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                        }
                        NowMoney = NowMoney - (pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney);
                        lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                        txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                        logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                        logger.Info("[파홀 : " + pOddMoney + "][파짝 : " + pEvenMoney + "][파언 : " + pUnderMoney + "][파오 : " + pOverMoney + "][일홀 : " + nOddMoney + "][일짝 : " + nEvenMoney + "][일언 : " + nUnderMoney + "][일오 : " + nOverMoney + "]");
                        bettingStatus = true;
                    }
                }
                else if (realOrVirtualMode == 0)
                {
                    if (UtilModel._userSite.Equals("rdw"))
                    {
                        if (type.Equals("PWB"))
                        {
                            Boolean _BoolResult = false;

                            int CountResult = 0;
                            while (!_BoolResult)
                            {
                                try
                                {
                                    string returnMessage = UtilModel.JPOST(_url, _param);
                                    logger.Info(returnMessage);


                                    JObject jo = JObject.Parse(returnMessage);
                                    var result = jo.SelectToken("A").ToString();

                                    if (result.Contains("0"))
                                    {
                                        NowMoney = int.Parse(jo.SelectToken("B").ToString());
                                        lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";
                                        if (UtilModel._allBettingEnable == 1)
                                        {
                                            allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                            label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                        }
                                        txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.DarkViolet);
                                        logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                        logger.Info(pOddMoney + " / " + pEvenMoney + " / " + pUnderMoney + " / " + pOverMoney + " / " + nOddMoney + " / " + nEvenMoney + " / " + nUnderMoney + " / " + nOverMoney);
                                        bettingStatus = true;
                                    }
                                    else
                                    {
                                        var error = jo.SelectToken("B").ToString();
                                        txtLogAdd(error, Color.MediumVioletRed);
                                        MessageBox.Show(error);
                                        logger.Info(allinning + " : 배팅 실패 : " + error);
                                        bettingStatus = false;
                                    }
                                }
                                catch (Exception _ex)
                                {
                                    CountResult++;
                                    if (CountResult > 3)
                                    {
                                        _BoolResult = true;
                                    }
                                    logger.Error(_ex.ToString());
                                    continue;
                                }
                                _BoolResult = true;
                            }

                            if (CountResult > 3)
                            {
                                string errorMessage = "배팅 시도를 3회를 하였지만 배팅이 등록지 않았습니다.";
                                txtLogAdd(errorMessage, Color.White);
                                MessageBox.Show(errorMessage);
                                logger.Info(errorMessage);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (UtilModel._userSite.Contains("vega"))
                    {
                        String returnMessage = "";

                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            returnMessage = webClient.DownloadString(_url + "?" + _param);
                        }
                        logger.Info(returnMessage);
                        JObject jo = JObject.Parse(returnMessage);
                        var ret_code = jo.SelectToken("ret_code").ToString();
                        var ret_message = jo.SelectToken("comment").ToString();
                        if (ret_code.Equals("1"))
                        {
                            if (type.Equals("PSA"))
                            {
                                allbettingMoney += ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                logger.Info("[배팅완료][3줄 : " + ladder3Money + "][4줄 : " + ladder4Money + "][홀 : " + ladderOddMoney + "][짝 : " + ladderEvenMoney + "][좌 : " + ladderLeftMoney + "][우 : " + ladderLeftMoney + "]");
                                bettingStatus = true;
                            }
                            else
                            {
                                allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                logger.Info("[파홀 : " + pOddMoney + "][파짝 : " + pEvenMoney + "][파언 : " + pUnderMoney + "][파오 : " + pOverMoney + "][일홀 : " + nOddMoney + "][일짝 : " + nEvenMoney + "][일언 : " + nUnderMoney + "][일오 : " + nOverMoney + "]");
                                bettingStatus = true;
                            }
                        }
                        else
                        {
                            beepSound();
                            txtLogAdd("오류가 발생하여 배팅이 진행되지 않았습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ret_message + " : 오류가 발생하여 배팅이 진행되지 않았습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                    }
                    else if (UtilModel._userSite.Contains("gtm") || UtilModel._userSite.Contains("fact"))
                    {
                        String returnMessage = "";

                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            returnMessage = webClient.DownloadString(_url + "?" + _param);
                        }
                        logger.Info(returnMessage);
                        JObject jo = JObject.Parse(returnMessage);
                        // {"ret_code":1,"comment":"ok","more_info":{"balance":98100}}
                        var ret_code = jo.SelectToken("ret_code").ToString();
                        var ret_message = jo.SelectToken("comment").ToString();
                        if (ret_code.Equals("1"))
                        {
                            if (type.Equals("PSA"))
                            {
                                allbettingMoney += ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                logger.Info("[배팅완료][3줄 : " + ladder3Money + "][4줄 : " + ladder4Money + "][홀 : " + ladderOddMoney + "][짝 : " + ladderEvenMoney + "][좌 : " + ladderLeftMoney + "][우 : " + ladderLeftMoney + "]");
                                bettingStatus = true;
                            }
                            else
                            {
                                allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                logger.Info("[파홀 : " + pOddMoney + "][파짝 : " + pEvenMoney + "][파언 : " + pUnderMoney + "][파오 : " + pOverMoney + "][일홀 : " + nOddMoney + "][일짝 : " + nEvenMoney + "][일언 : " + nUnderMoney + "][일오 : " + nOverMoney + "]");
                                bettingStatus = true;
                            }
                        }
                        else
                        {
                            beepSound();
                            txtLogAdd("오류가 발생하여 배팅이 진행되지 않았습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ret_message + " : 오류가 발생하여 배팅이 진행되지 않았습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                    }
                    else if (UtilModel._userSite.Contains("gong"))
                    {
                        String returnMessage = "";

                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            returnMessage = webClient.DownloadString(_url + "?" + _param);
                        }
                        logger.Info(returnMessage);
                        JObject jo = JObject.Parse(returnMessage);
                        var ret_code = jo.SelectToken("ret_code").ToString();
                        var ret_message = jo.SelectToken("ret_msg").ToString();
                        if (ret_code.Equals("1"))
                        {
                            NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                            String allot_rate = jo.SelectToken("more_info").SelectToken("allot_rate").ToString();

                            if (type.Equals("PSA"))
                            {

                                allbettingMoney += ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;

                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                String bettingMoneyString = "[배팅완료][" + allinning + " 회][3줄 : " + ladder3Money + "][4줄 : " + ladder4Money + "][홀 : " + ladderOddMoney + "][짝 : " + ladderEvenMoney + "][좌 : " + ladderLeftMoney + "][우 : " + ladderLeftMoney + "]";
                                logger.Info(bettingMoneyString);
                                if (UtilModel.telegramChatId > 0)
                                {
                                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                                    {
                                        webClient.Encoding = Encoding.UTF8;
                                        webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(bettingMoneyString));
                                    }
                                }
                                bettingStatus = true;
                            }
                            else
                            {
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                if (UtilModel._allBettingEnable == 1)
                                {
                                    allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                String bettingMoneyString = "[배팅완료][" + allinning + " 회][파홀:" + pOddMoney + "][파짝:" + pEvenMoney + "][파언더:" + pUnderMoney + "][파오버:" + pOverMoney + "][일홀:" + nOddMoney + "][일짝:" + nEvenMoney + "][일언더:" + nUnderMoney + "][일오버:" + nOverMoney + "]";
                                logger.Info(bettingMoneyString);
                                if (UtilModel.telegramChatId > 0)
                                {
                                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                                    {
                                        webClient.Encoding = Encoding.UTF8;
                                        webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(bettingMoneyString));
                                    }
                                }
                                bettingStatus = true;
                            }
                        }
                        else if (ret_code.Equals("-99"))
                        {
                            beepSound();
                            txtLogAdd(ret_code + " : " + ret_message, Color.White);
                            MessageBox.Show(ret_code + " : " + ret_message);
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                        else if (ret_code.Equals("-100"))
                        {
                            beepSound();
                            txtLogAdd("페이지를 찾을 수 없습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 페이지를 찾을 수 없습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                        else if (ret_code.Equals("-102"))
                        {
                            beepSound();
                            txtLogAdd("API 배팅 서비스가 정지 되었습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] API 배팅 서비스가 정지 되었습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-103"))
                        {
                            beepSound();
                            txtLogAdd("운영중인 게임이 아닙니다..", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 운영중인 게임이 아닙니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-104"))
                        {
                            beepSound();
                            txtLogAdd("배팅 금액이 너무 작아 오류가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 배팅 금액이 너무 작아 오류가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-105"))
                        {

                            beepSound();
                            txtLogAdd("최대 배팅 금액 오류. 배팅 금액이 너무 큽니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 최대 배팅 금액 오류. 배팅 금액이 너무 큽니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-106"))
                        {
                            beepSound();
                            txtLogAdd("배팅 금액이 없습니다. (최소금액 제한)", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 배팅 금액이 없습니다. (최소금액 제한)");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-107"))
                        {
                            beepSound();
                            txtLogAdd("회차 정보가 달라 배팅에 실패하였습니다.", Color.White);
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-108"))
                        {
                            beepSound();
                            txtLogAdd("회원 인증에 실패하였습니다. 아이디나 KEY 값이 다릅니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 회원 인증에 실패하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-109"))
                        {
                            beepSound();
                            txtLogAdd("회원 인증에 실패하였습니다. 회원님의 IP가 차단되었습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 회원 인증에 실패하였습니다. 회원님의 IP가 차단되었습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-110"))
                        {
                            beepSound();
                            txtLogAdd("[-110] 네트워크 에러가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] [0] 서버측 네트워크 에러가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-1001"))
                        {
                            beepSound();
                            txtLogAdd("보유 금액이 부족합니다. 충전이 필요합니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 보유 금액이 부족합니다. 충전이 필요합니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-1003"))
                        {
                            beepSound();
                            txtLogAdd("[-1003] 네트워크 에러가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] [3] 서버측 네트워크 에러가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);


                        }
                        else if (ret_code.Equals("-1004"))
                        {
                            beepSound();
                            txtLogAdd("해당 회차에 이미 배팅을 하셨습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 해당 회차에 이미 배팅을 하셨습니다. 회차당 1회의 배팅만 허용됩니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else
                        {
                            beepSound();
                            txtLogAdd("오류가 발생하여 배팅이 진행되지 않았습니다.", Color.White);
                            txtLogAdd(returnMessage, Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ret_code + " : " + ret_message + " : 오류가 발생하여 배팅이 진행되지 않았습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                    }
                    // {"ret_code":1,"comment":"ok","more_info":{"balance":2995000}}
                    else if (UtilModel._userSite.Contains("ari") || UtilModel._userSite.Contains("dcb") || UtilModel._userSite.Contains("ace"))
                    {
                        String returnMessage = "";

                        using (TimeoutWebClient webClient = new TimeoutWebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            returnMessage = webClient.DownloadString(_url + "?" + _param);
                        }
                        logger.Info(returnMessage);
                        JObject jo = JObject.Parse(returnMessage);
                        var ret_code = jo.SelectToken("ret_code").ToString();
                        var ret_message = jo.SelectToken("comment").ToString();
                        if (ret_code.Equals("1"))
                        {
                            NowMoney = int.Parse(jo.SelectToken("more_info").SelectToken("balance").ToString());
                            //String allot_rate = jo.SelectToken("more_info").SelectToken("allot_rate").ToString();
                            if (type.Equals("PSA"))
                            {

                                allbettingMoney += ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;

                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + allbettingMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                if (UtilModel._allBettingEnable == 1)
                                {
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                String bettingMoneyString = "[배팅완료][" + allinning + " 회][3줄 : " + ladder3Money + "][4줄 : " + ladder4Money + "][홀 : " + ladderOddMoney + "][짝 : " + ladderEvenMoney + "][좌 : " + ladderLeftMoney + "][우 : " + ladderLeftMoney + "]";
                                logger.Info(bettingMoneyString);
                                if (UtilModel.telegramChatId > 0)
                                {
                                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                                    {
                                        webClient.Encoding = Encoding.UTF8;
                                        webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(bettingMoneyString));
                                    }
                                }
                                bettingStatus = true;
                            }
                            else
                            {
                                if (!_getStartMoney)
                                {
                                    _getStartMoney = true;
                                    startMoney = NowMoney + pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                    lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                                }
                                lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + "원";

                                if (UtilModel._allBettingEnable == 1)
                                {
                                    allbettingMoney += pOddMoney + pEvenMoney + pUnderMoney + pOverMoney + nOddMoney + nEvenMoney + nUnderMoney + nOverMoney;
                                    label10.Text = UtilModel.StringFormatChanged(allbettingMoney) + " 원";
                                }
                                txtLogAdd("[" + allinning + "] 정상 배팅 등록 완료.", Color.White);
                                logger.Info("[" + allinning + "] 정상 배팅 등록 완료.");
                                String bettingMoneyString = "[배팅완료][" + allinning + " 회][파홀:" + pOddMoney + "][파짝:" + pEvenMoney + "][파언더:" + pUnderMoney + "][파오버:" + pOverMoney + "][일홀:" + nOddMoney + "][일짝:" + nEvenMoney + "][일언더:" + nUnderMoney + "][일오버:" + nOverMoney + "]";
                                logger.Info(bettingMoneyString);
                                if (UtilModel.telegramChatId > 0)
                                {
                                    using (TimeoutWebClient webClient = new TimeoutWebClient())
                                    {
                                        webClient.Encoding = Encoding.UTF8;
                                        webClient.DownloadString(UtilModel.telegramChatUrl + "?chatid=" + UtilModel.telegramChatId + "&Message=" + HttpUtility.UrlEncode(bettingMoneyString));
                                    }
                                }
                                bettingStatus = true;
                            }

                        }
                        else if (ret_code.Equals("-99"))
                        {
                            beepSound();
                            txtLogAdd(ret_code + " : " + ret_message, Color.White);
                            MessageBox.Show(ret_code + " : " + ret_message);
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                        else if (ret_code.Equals("-100"))
                        {
                            beepSound();
                            txtLogAdd("페이지를 찾을 수 없습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 페이지를 찾을 수 없습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                        else if (ret_code.Equals("-102"))
                        {
                            beepSound();
                            txtLogAdd("API 배팅 서비스가 정지 되었습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] API 배팅 서비스가 정지 되었습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-103"))
                        {
                            beepSound();
                            txtLogAdd("운영중인 게임이 아닙니다..", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 운영중인 게임이 아닙니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-104"))
                        {
                            beepSound();
                            txtLogAdd("배팅 금액이 너무 작아 오류가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 배팅 금액이 너무 작아 오류가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-105"))
                        {

                            beepSound();
                            txtLogAdd("최대 배팅 금액 오류. 배팅 금액이 너무 큽니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 최대 배팅 금액 오류. 배팅 금액이 너무 큽니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-106"))
                        {
                            beepSound();
                            txtLogAdd("배팅 금액이 없습니다. (최소금액 제한)", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 배팅 금액이 없습니다. (최소금액 제한)");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-107"))
                        {
                            beepSound();
                            txtLogAdd("회차 정보가 달라 배팅에 실패하였습니다.", Color.White);
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-108"))
                        {
                            beepSound();
                            txtLogAdd("회원 인증에 실패하였습니다. 아이디나 KEY 값이 다릅니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 회원 인증에 실패하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-109"))
                        {
                            beepSound();
                            txtLogAdd("회원 인증에 실패하였습니다. 회원님의 IP가 차단되었습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 회원 인증에 실패하였습니다. 회원님의 IP가 차단되었습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-110"))
                        {
                            beepSound();
                            txtLogAdd("[-110] 네트워크 에러가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] [0] 서버측 네트워크 에러가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-1001"))
                        {
                            beepSound();
                            txtLogAdd("보유 금액이 부족합니다. 충전이 필요합니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 보유 금액이 부족합니다. 충전이 필요합니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-1003"))
                        {
                            beepSound();
                            txtLogAdd("[-1003] 네트워크 에러가 발생하였습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] [3] 서버측 네트워크 에러가 발생하였습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else if (ret_code.Equals("-1004"))
                        {
                            beepSound();
                            txtLogAdd("해당 회차에 이미 배팅을 하셨습니다.", Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] 해당 회차에 이미 배팅을 하셨습니다. 회차당 1회의 배팅만 허용됩니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);

                        }
                        else
                        {
                            beepSound();
                            txtLogAdd("오류가 발생하여 배팅이 진행되지 않았습니다.", Color.White);
                            txtLogAdd(returnMessage, Color.White);
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ret_code + " : " + ret_message + " : 오류가 발생하여 배팅이 진행되지 않았습니다.");
                            logger.Info(allinning + " : 배팅 실패 : " + ret_code + " : " + ret_message);
                        }
                    }
                }

            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        private void bet()
        {
            pOddMoney = 0;
            pEvenMoney = 0;
            pUnderMoney = 0;
            pOverMoney = 0;
            nOddMoney = 0;
            nEvenMoney = 0;
            nUnderMoney = 0;
            nOverMoney = 0;
            ladder3Money = 0;
            ladder4Money = 0;
            ladderOddMoney = 0;
            ladderEvenMoney = 0;
            ladderLeftMoney = 0;
            ladderRightMoney = 0;

            bettingLevel1();
            if (pOddMoney < 100 && pOddMoney > 0)
            {
                pOddMoney = 100;
            }
            if (pEvenMoney < 100 && pEvenMoney > 0)
            {
                pEvenMoney = 100;
            }
            if (nOddMoney < 100 && nOddMoney > 0)
            {
                nOddMoney = 100;
            }
            if (nEvenMoney < 100 && nEvenMoney > 0)
            {
                nEvenMoney = 100;
            }
            if (pUnderMoney < 100 && pUnderMoney > 0)
            {
                pUnderMoney = 100;
            }
            if (pOverMoney < 100 && pOverMoney > 0)
            {
                pOverMoney = 100;
            }
            if (nUnderMoney < 100 && nUnderMoney > 0)
            {
                nUnderMoney = 100;
            }
            if (nOverMoney < 100 && nOverMoney > 0)
            {
                nOverMoney = 100;
            }

            int powerballMoney = pOddMoney + pEvenMoney + nOddMoney + nEvenMoney + pUnderMoney + pOverMoney + nUnderMoney + nOverMoney;
            int powerLadderMoney = ladder3Money + ladder4Money + ladderEvenMoney + ladderOddMoney + ladderLeftMoney + ladderRightMoney;
            groupBoxBettingStatus.Text = "[" + (allinning) + "회][" + (todayinning) + "회] 총 배팅 금액 : " + UtilModel.StringFormatChanged((powerballMoney + powerLadderMoney)) + " 원";

            if (powerballMoney > 0)
            {
                powerballBetting(0, "PWB");
                startInning = allinning;
            }
            if (powerLadderMoney > 0)
            {
                powerLadderBetting("PSA");
                startInning = allinning;
            }
            if (powerballMoney == 0 && powerLadderMoney == 0)
            {
                txtLogAdd("배팅을 한 것이 없어 등록하지 않았습니다.", Color.White);
                logger.Info("배팅을 한 것이 없어 등록하지 않았습니다.");
            }
            termInning = allinning - startInning;
        }
        int termInning = 0;
        void powerLadderBetting(string gm)
        {
            String bettingUrl = "";
            String param = "";
            String _date = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "");
            if (UtilModel._userSite.Contains("vega"))
            {
                bettingUrl = UtilModel.vegaurl;
            }
            else if (UtilModel._userSite.Contains("gtm"))
            {
                bettingUrl = UtilModel.gtmUrl;
            }
            else if (UtilModel._userSite.Contains("fact"))
            {
                bettingUrl = UtilModel.factUrl;
            }
            else if (UtilModel._userSite.Contains("ari"))
            {
                bettingUrl = UtilModel.ariUrl;
            }
            else if (UtilModel._userSite.Contains("dcb"))
            {
                bettingUrl = UtilModel.dcbUrl;
            }
            else if (UtilModel._userSite.Contains("ace"))
            {
                bettingUrl = UtilModel.aceUrl;
            }
            else if (UtilModel._userSite.Contains("gong"))
            {
                bettingUrl = UtilModel.gongurl;
            }
            param = string.Format("gm={0}&userid={1}&key={2}&tdate={3}&rno={4}&pp1={5}&pp2={6}&pp3={7}&pp4={8}&pp5={9}&pp6={10}&pp7={11}&pp8={12}&nonce={13}", gm, UtilModel.betid, UtilModel._apikey, _date, todayinning, pOddMoney, pEvenMoney, pUnderMoney, pOverMoney, nOddMoney, nEvenMoney, nUnderMoney, nOverMoney, randNonce);
            //MessageBox.Show(bettingUrl + "?" + param);
            if (bettingUrl.Length < 10)
            {
                beepSound();
                txtLogAdd("주소가 잘못되었습니다. 확인 부탁드립니다.", Color.MediumVioletRed);
                MessageBox.Show("주소가 잘못되었습니다. 확인 부탁드립니다.");

                logger.Info("[" + bettingUrl + "]주소가 잘못되었습니다. 확인 부탁드립니다.");
                return;
            }

            bettingLevel2(bettingUrl, param, "powerladder");

            if (bettingStatus)
            {
                String str = "";
                if (ladderLeftMoney > 0)
                {
                    str += "[좌 : " + UtilModel.StringFormatChanged(ladderLeftMoney) + "원]";
                }
                if (ladderRightMoney > 0)
                {
                    str += "[우 : " + UtilModel.StringFormatChanged(ladderRightMoney) + "원]";
                }
                if (ladder3Money > 0)
                {
                    str += "[3줄 : " + UtilModel.StringFormatChanged(ladder3Money) + "원]";
                }
                if (ladder4Money > 0)
                {
                    str += "[4줄 : " + UtilModel.StringFormatChanged(ladder4Money) + "원]";
                }
                if (ladderOddMoney > 0)
                {
                    str += "[홀 : " + UtilModel.StringFormatChanged(ladderOddMoney) + "원]";
                }
                if (ladderEvenMoney > 0)
                {
                    str += "[짝 : " + UtilModel.StringFormatChanged(ladderEvenMoney) + "원]";
                }
                if (str.Length > 1)
                {
                    txtLogAdd(str, Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(199)))), ((int)(((byte)(132)))))); // 129, 199, 132
                }
                _powerLadder34.Text = UtilModel.StringFormatChanged(ladder3Money) + "/" + UtilModel.StringFormatChanged(ladder4Money);
                _powerLadderLeftRight.Text = UtilModel.StringFormatChanged(ladderLeftMoney) + "/" + UtilModel.StringFormatChanged(ladderRightMoney);
                _powerLadderOddEven.Text = UtilModel.StringFormatChanged(ladderOddMoney) + "/" + UtilModel.StringFormatChanged(ladderEvenMoney);
                _picksterRoundUpdate();
            }
        }
        void powerballBetting(int type, string gm)
        {
            String bettingUrl = "";
            String param = "";
            String _date = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "");

            if (type == 0)
            {
                if (UtilModel._userSite.Equals("rdw"))
                {
                    bettingUrl = UtilModel.rdwUrl; //"http://api.rdwball.com/bet";

                    var jsonData = new JObject();
                    jsonData.Add("A", UtilModel._apikey);
                    jsonData.Add("B", 4);
                    jsonData.Add("C", allinning);

                    var BettingInformation = new JObject();

                    var jarray = new JArray();
                    jarray.Add(nOddMoney); // 일반볼 홀
                    jarray.Add(nEvenMoney); // 일반볼 짝
                    jarray.Add(nUnderMoney); // 일반볼 언더
                    jarray.Add(nOverMoney); // 일반볼 오버
                    jarray.Add(pOddMoney); // 파워볼 홀
                    jarray.Add(pEvenMoney); // 파워볼 짝
                    jarray.Add(pUnderMoney); // 파워볼 언더
                    jarray.Add(pOverMoney); // 파워볼 오버

                    jarray.Add(0); // 일반볼 대
                    jarray.Add(0); // 일반볼 중
                    jarray.Add(0); // 일반볼 소

                    jarray.Add(0); // 파워볼 숫자 0
                    jarray.Add(0); // 파워볼 숫자 1
                    jarray.Add(0); // 파워볼 숫자 2
                    jarray.Add(0); // 파워볼 숫자 3
                    jarray.Add(0); // 파워볼 숫자 4
                    jarray.Add(0); // 파워볼 숫자 5
                    jarray.Add(0); // 파워볼 숫자 6
                    jarray.Add(0); // 파워볼 숫자 7
                    jarray.Add(0); // 피워볼 숫자 8
                    jarray.Add(0); // 파워볼 숫자 9

                    jarray.Add(0); // 일반볼 P/B 플레이어
                    jarray.Add(0); // 일반볼 P/B 뱅커
                    jarray.Add(0); // 일반볼 P/B 타이

                    jarray.Add(0); // 일반볼 D/T 드래곤
                    jarray.Add(0); // 일반볼 D/T 타이커
                    jarray.Add(0); // 일반볼 D/T 타이

                    jarray.Add(0); // 일반볼 홀&언더
                    jarray.Add(0); // 일반볼 홀&오버
                    jarray.Add(0); // 일반볼 짝&언더
                    jarray.Add(0); // 일반볼 짝&오버

                    jarray.Add(0); // 일반볼 홀&대
                    jarray.Add(0); // 일반볼 홀&중
                    jarray.Add(0); // 일반볼 홀&소

                    jarray.Add(0); // 일반볼 짝&대
                    jarray.Add(0); // 일반볼 짝&중
                    jarray.Add(0); // 일반볼 짝&소

                    jarray.Add(0); // 파워볼 홀&언더
                    jarray.Add(0); // 파워볼 홀&오버
                    jarray.Add(0); // 파워볼 짝&언더
                    jarray.Add(0); // 파워볼 짝&오버

                    BettingInformation.Add("A", jarray);

                    jsonData.Add("D", JsonConvert.SerializeObject(BettingInformation));

                    param = jsonData.ToString();
                }
                else
                {
                    if (UtilModel._userSite.Contains("gtm"))
                    {
                        bettingUrl = UtilModel.gtmUrl;
                    }
                    else if (UtilModel._userSite.Contains("fact"))
                    {
                        bettingUrl = UtilModel.factUrl;
                    }
                    else if (UtilModel._userSite.Contains("vega"))
                    {
                        bettingUrl = UtilModel.vegaurl;
                    }
                    else if (UtilModel._userSite.Contains("ari"))
                    {
                        bettingUrl = UtilModel.ariUrl;
                    }
                    else if (UtilModel._userSite.Contains("dcb"))
                    {
                        bettingUrl = UtilModel.dcbUrl;
                    }
                    else if (UtilModel._userSite.Contains("ace"))
                    {
                        bettingUrl = UtilModel.aceUrl;
                    }
                    else if (UtilModel._userSite.Contains("gong"))
                    {
                        bettingUrl = UtilModel.gongurl;
                    }
                    param = string.Format("gm={0}&userid={1}&key={2}&tdate={3}&rno={4}&pp1={5}&pp2={6}&pp3={7}&pp4={8}&pp5={9}&pp6={10}&pp7={11}&pp8={12}&nonce={13}", gm, UtilModel.betid, UtilModel._apikey, _date, allinning, pOddMoney, pEvenMoney, pUnderMoney, pOverMoney, nOddMoney, nEvenMoney, nUnderMoney, nOverMoney, randNonce);
                }
            }
            else if (type > 0) // 직접 배팅 시 처리
            {
                if (UtilModel._userSite.Equals("rdw"))
                {
                    bettingUrl = UtilModel.rdwUrl; //"http://api.rdwball.com/bet";

                    var jsonData = new JObject();
                    jsonData.Add("A", UtilModel._apikey);
                    jsonData.Add("B", 4);
                    jsonData.Add("C", allinning);

                    var BettingInformation = new JObject();

                    var jarray = new JArray();
                    jarray.Add(nOddMoney); // 일반볼 홀
                    jarray.Add(nEvenMoney); // 일반볼 짝
                    jarray.Add(nUnderMoney); // 일반볼 언더
                    jarray.Add(nOverMoney); // 일반볼 오버
                    jarray.Add(pOddMoney); // 파워볼 홀
                    jarray.Add(pEvenMoney); // 파워볼 짝
                    jarray.Add(pUnderMoney); // 파워볼 언더
                    jarray.Add(pOverMoney); // 파워볼 오버

                    jarray.Add(0); // 일반볼 대
                    jarray.Add(0); // 일반볼 중
                    jarray.Add(0); // 일반볼 소

                    jarray.Add(0); // 파워볼 숫자 0
                    jarray.Add(0); // 파워볼 숫자 1
                    jarray.Add(0); // 파워볼 숫자 2
                    jarray.Add(0); // 파워볼 숫자 3
                    jarray.Add(0); // 파워볼 숫자 4
                    jarray.Add(0); // 파워볼 숫자 5
                    jarray.Add(0); // 파워볼 숫자 6
                    jarray.Add(0); // 파워볼 숫자 7
                    jarray.Add(0); // 피워볼 숫자 8
                    jarray.Add(0); // 파워볼 숫자 9

                    jarray.Add(0); // 일반볼 P/B 플레이어
                    jarray.Add(0); // 일반볼 P/B 뱅커
                    jarray.Add(0); // 일반볼 P/B 타이

                    jarray.Add(0); // 일반볼 D/T 드래곤
                    jarray.Add(0); // 일반볼 D/T 타이커
                    jarray.Add(0); // 일반볼 D/T 타이

                    jarray.Add(0); // 일반볼 홀&언더
                    jarray.Add(0); // 일반볼 홀&오버
                    jarray.Add(0); // 일반볼 짝&언더
                    jarray.Add(0); // 일반볼 짝&오버

                    jarray.Add(0); // 일반볼 홀&대
                    jarray.Add(0); // 일반볼 홀&중
                    jarray.Add(0); // 일반볼 홀&소

                    jarray.Add(0); // 일반볼 짝&대
                    jarray.Add(0); // 일반볼 짝&중
                    jarray.Add(0); // 일반볼 짝&소

                    jarray.Add(0); // 파워볼 홀&언더
                    jarray.Add(0); // 파워볼 홀&오버
                    jarray.Add(0); // 파워볼 짝&언더
                    jarray.Add(0); // 파워볼 짝&오버

                    BettingInformation.Add("A", jarray);

                    jsonData.Add("D", JsonConvert.SerializeObject(BettingInformation));

                    param = jsonData.ToString();
                }
                else
                {
                    int _Nonce = rand.Next(10000000, 99999999);
                    int betInning = 0;
                    if (type == 1) // 파워볼
                    {
                        betInning = allinning;
                    }
                    else if (type == 2) // 파워볼 조합
                    {
                        betInning = allinning;
                    }
                    else if (type == 3) // 
                    {
                        betInning = allinning;
                    }
                    else if (type == 4) // 파워사다리
                    {
                        betInning = todayinning;
                    }
                    else
                    {
                        betInning = allinning;
                    }
                    if (UtilModel._userSite.Contains("gtm"))
                    {
                        bettingUrl = UtilModel.gtmUrl;
                    }
                    else if (UtilModel._userSite.Contains("fact"))
                    {
                        bettingUrl = UtilModel.factUrl;
                    }
                    else if (UtilModel._userSite.Contains("vega"))
                    {
                        bettingUrl = UtilModel.vegaurl;
                    }
                    else if (UtilModel._userSite.Contains("ari"))
                    {
                        bettingUrl = UtilModel.ariUrl;
                    }
                    else if (UtilModel._userSite.Contains("dcb"))
                    {
                        bettingUrl = UtilModel.dcbUrl;
                    }
                    else if (UtilModel._userSite.Contains("ace"))
                    {
                        bettingUrl = UtilModel.aceUrl;
                    }
                    else if (UtilModel._userSite.Contains("gong"))
                    {
                        bettingUrl = UtilModel.gongurl;
                    }
                    param = string.Format("gm={0}&userid={1}&key={2}&tdate={3}&rno={4}&pp1={5}&pp2={6}&pp3={7}&pp4={8}&pp5={9}&pp6={10}&pp7={11}&pp8={12}&nonce={13}", gm, UtilModel.betid, UtilModel._apikey, _date, betInning, pOddMoney, pEvenMoney, pUnderMoney, pOverMoney, nOddMoney, nEvenMoney, nUnderMoney, nOverMoney, _Nonce);

                }
            }

            if (bettingUrl.Length < 10)
            {
                beepSound();
                txtLogAdd("주소가 잘못되었습니다. 확인 부탁드립니다.", Color.MediumVioletRed);
                MessageBox.Show("주소가 잘못되었습니다. 확인 부탁드립니다.");

                logger.Info("[" + bettingUrl + "]주소가 잘못되었습니다. 확인 부탁드립니다.");
                return;
            }

            bettingLevel2(bettingUrl, param, gm);

            if (type == 0)
            {
                if (bettingStatus)
                {
                    String str = "";
                    String str2 = "";
                    if (pOddMoney > 0)
                    {
                        str += "[파홀 : " + UtilModel.StringFormatChanged(pOddMoney) + "원]";
                    }
                    if (pEvenMoney > 0)
                    {
                        str += "[파짝 : " + UtilModel.StringFormatChanged(pEvenMoney) + "원]";
                    }
                    if (pUnderMoney > 0)
                    {
                        str += "[파언 : " + UtilModel.StringFormatChanged(pUnderMoney) + "원]";
                    }
                    if (pOverMoney > 0)
                    {
                        str += "[파오 : " + UtilModel.StringFormatChanged(pOverMoney) + "원]";
                    }
                    if (nOddMoney > 0)
                    {
                        str2 += "[일홀 : " + UtilModel.StringFormatChanged(nOddMoney) + "원]";
                    }
                    if (nEvenMoney > 0)
                    {
                        str2 += "[일짝 : " + UtilModel.StringFormatChanged(nEvenMoney) + "원]";
                    }
                    if (nUnderMoney > 0)
                    {
                        str2 += "[일언 : " + UtilModel.StringFormatChanged(nUnderMoney) + "원]";
                    }
                    if (nOverMoney > 0)
                    {
                        str2 += "[일오 : " + UtilModel.StringFormatChanged(nOverMoney) + "원]";

                    }
                    if (str.Length > 1)
                    {
                        txtLogAdd(str, Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(199)))), ((int)(((byte)(132)))))); // 129, 199, 132
                    }
                    if (str2.Length > 1)
                    {
                        txtLogAdd(str2, Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(199)))), ((int)(((byte)(132)))))); // 129, 199, 132
                    }
                    lblPSingle.Text = UtilModel.StringFormatChanged(pOddMoney) + " 원";
                    lblPPair.Text = UtilModel.StringFormatChanged(pEvenMoney) + " 원";
                    lblPUnder.Text = UtilModel.StringFormatChanged(pUnderMoney) + " 원";
                    lblPOver.Text = UtilModel.StringFormatChanged(pOverMoney) + " 원";
                    lblNSingle.Text = UtilModel.StringFormatChanged(nOddMoney) + " 원";
                    lblNPair.Text = UtilModel.StringFormatChanged(nEvenMoney) + " 원";
                    lblNUnder.Text = UtilModel.StringFormatChanged(nUnderMoney) + " 원";
                    lblNOver.Text = UtilModel.StringFormatChanged(nOverMoney) + " 원";
                    _picksterRoundUpdate();
                }
            }
            else if (type > 0)
            {
                if (bettingStatus)
                {
                    String str = "";
                    String str2 = "";
                    if (type == 1)
                    {
                        if (pOddMoney > 0)
                        {
                            str += "[파홀 : " + UtilModel.StringFormatChanged(pOddMoney) + "원]";
                        }
                        if (pEvenMoney > 0)
                        {
                            str += "[파짝 : " + UtilModel.StringFormatChanged(pEvenMoney) + "원]";
                        }
                        if (pUnderMoney > 0)
                        {
                            str += "[파언 : " + UtilModel.StringFormatChanged(pUnderMoney) + "원]";
                        }
                        if (pOverMoney > 0)
                        {
                            str += "[파오 : " + UtilModel.StringFormatChanged(pOverMoney) + "원]";
                        }
                        if (nOddMoney > 0)
                        {
                            str2 += "[일홀 : " + UtilModel.StringFormatChanged(nOddMoney) + "원]";
                        }
                        if (nEvenMoney > 0)
                        {
                            str2 += "[일짝 : " + UtilModel.StringFormatChanged(nEvenMoney) + "원]";
                        }
                        if (nUnderMoney > 0)
                        {
                            str2 += "[일언 : " + UtilModel.StringFormatChanged(nUnderMoney) + "원]";
                        }
                        if (nOverMoney > 0)
                        {
                            str2 += "[일오 : " + UtilModel.StringFormatChanged(nOverMoney) + "원]";

                        }
                        if (str.Length > 1)
                        {
                            txtLogAdd(str, Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(199)))), ((int)(((byte)(132)))))); // 129, 199, 132

                            ListViewItem item;

                            item = new ListViewItem("파워"); // 픽스터 이름
                            if (pOddMoney > 0)
                            {
                                item.SubItems.Add("홀");
                                item.SubItems.Add(UtilModel.StringFormatChanged(pOddMoney));
                            }
                            else if (pEvenMoney > 0)
                            {
                                item.SubItems.Add("짝");
                                item.SubItems.Add(UtilModel.StringFormatChanged(pEvenMoney));
                            }
                            else if (pUnderMoney > 0)
                            {
                                item.SubItems.Add("언더");
                                item.SubItems.Add(UtilModel.StringFormatChanged(pUnderMoney));
                            }
                            else if (pOverMoney > 0)
                            {
                                item.SubItems.Add("오버");
                                item.SubItems.Add(UtilModel.StringFormatChanged(pOverMoney));
                            }
                            listView3.Items.Add(item);
                        }
                        if (str2.Length > 1)
                        {
                            txtLogAdd(str2, Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(199)))), ((int)(((byte)(132)))))); // 129, 199, 132

                            ListViewItem item;

                            item = new ListViewItem("일반"); // 픽스터 이름
                            if (nOddMoney > 0)
                            {
                                item.SubItems.Add("홀");
                                item.SubItems.Add(UtilModel.StringFormatChanged(nOddMoney));
                            }
                            else if (nEvenMoney > 0)
                            {
                                item.SubItems.Add("짝");
                                item.SubItems.Add(UtilModel.StringFormatChanged(nEvenMoney));
                            }
                            else if (nUnderMoney > 0)
                            {
                                item.SubItems.Add("언더");
                                item.SubItems.Add(UtilModel.StringFormatChanged(nUnderMoney));
                            }
                            else if (nOverMoney > 0)
                            {
                                item.SubItems.Add("오버");
                                item.SubItems.Add(UtilModel.StringFormatChanged(nOverMoney));
                            }
                            listView3.Items.Add(item);
                        }
                    }
                }
            }
        }
        void _picksterRoundUpdate()
        {
            try
            {
                textLog.Text = "";
                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    Button _pr = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    if (_pr.Text.Contains("진행"))
                    {
                        _picksterRoundArray[(_find - 1)] += 1;
                        textLog.Text += _picksterRoundArray[(_find - 1)] + Environment.NewLine;
                    }
                }
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        // listview 정렬
        private void listView_columnClick(object sender, ColumnClickEventArgs e)
        {
            columnsorter.currentColumn = e.Column;

            if (columnsorter.currentColumn > 4)
            {
                return;
            }
            // 전에 선택했던 컬럼과 다르면 오름 차순 정렬
            if (columnsorter.previousColumn != columnsorter.currentColumn)
            {
                columnsorter.sort = Sorting.Ascending;
            }
            else    // 전에 선택했던 컬럼과 같을때
            {
                switch (columnsorter.sort)
                {
                    case Sorting.Ascending:// 오름차순이였다면 내림 차순으로 바꾼다.
                        columnsorter.sort = Sorting.Descending;
                        break;
                    case Sorting.Descending:
                        columnsorter.sort = Sorting.Ascending;
                        break;
                }
            }

            if (flag == 0)
            {
                listView1.ListViewItemSorter = columnsorter; // 자동으로 listvHeader.Sort()함수가 수행된다.
                flag = 1;
            }
            else
            {
                listView1.Sort();
            }

            // 현재 선택했던 컬럼을 기억해 둔다.
            columnsorter.previousColumn = columnsorter.currentColumn;
            return;
        }
        ColumnSorter columnsorter = new ColumnSorter();

        private int flag = 0;

        public enum Sorting { Ascending, Descending };

        //ListView에서 컬럼을 누르면 정렬이 되게 하기 위해...
        public class ColumnSorter : IComparer
        {
            public int currentColumn = -1;  // 현재 선택한 컬럼
            public int previousColumn = -1;  // 전에 선책한 컬럼     
            public Sorting sort = Sorting.Ascending;

            public int Compare(object x, object y)
            {
                int result = 0;
                ListViewItem rowA = (ListViewItem)x;
                ListViewItem rowB = (ListViewItem)y;

                if (rowA.ListView.Columns[currentColumn].Tag == null) // 리스트뷰 Tag 속성이 Null 이면 기본적으로 Text 정렬을 사용하겠다는 의미
                {
                    rowA.ListView.Columns[currentColumn].Tag = "Text";
                }

                if (rowA.ListView.Columns[currentColumn].Tag.ToString() == "Numeric") // 0승 연패
                {
                    string str1 = rowA.SubItems[currentColumn].Text;
                    string str2 = rowB.SubItems[currentColumn].Text;
                    float fl1;
                    float fl2;

                    if (str1.IndexOf("승") < 1)
                    {
                        str1 = "99승";
                    }
                    if (str2.IndexOf("승") < 1)
                    {
                        str2 = "99승";
                    }

                    fl1 = float.Parse(str1.Substring(0, str1.IndexOf("승")));
                    fl2 = float.Parse(str2.Substring(0, str2.IndexOf("승")));

                    switch (sort)
                    {
                        case Sorting.Ascending:    // 오름차 정렬을 원할때
                            result = fl1.CompareTo(fl2);
                            break;
                        case Sorting.Descending:    // 내림차순 정렬을 원할때
                            result = fl2.CompareTo(fl1);
                            break;
                    }
                }
                else if (rowA.ListView.Columns[currentColumn].Tag.ToString() == "Numeric2")
                {
                    string str1 = rowA.SubItems[currentColumn].Text;
                    string str2 = rowB.SubItems[currentColumn].Text;
                    float fl1 = 0;
                    float fl2 = 0;

                    if (str1.IndexOf("연") < 1)
                    {
                        fl1 = -9999;
                    }
                    else
                    {
                        if (str1.Contains("연승"))
                        {
                            fl1 = float.Parse(str1.Substring(0, str1.IndexOf("연")));
                        }
                        else
                        {
                            fl1 = float.Parse(str1.Substring(0, str1.IndexOf("연"))) * -1;
                        }
                    }
                    if (str2.IndexOf("연") < 1)
                    {
                        fl2 = -9999;
                    }
                    else
                    {
                        if (str2.Contains("연승"))
                        {
                            fl2 = float.Parse(str2.Substring(0, str2.IndexOf("연")));
                        }
                        else
                        {
                            fl2 = float.Parse(str2.Substring(0, str2.IndexOf("연"))) * -1;
                        }
                    }

                    switch (sort)
                    {
                        case Sorting.Ascending:    // 오름차 정렬을 원할때
                            result = fl1.CompareTo(fl2);
                            break;
                        case Sorting.Descending:    // 내림차순 정렬을 원할때
                            result = fl2.CompareTo(fl1);
                            break;
                    }
                }
                else
                {
                    switch (sort)
                    {
                        case Sorting.Ascending:    // 오름차 정렬을 원할때
                            result = String.Compare(rowA.SubItems[currentColumn].Text, rowB.SubItems[currentColumn].Text);
                            break;
                        case Sorting.Descending:    // 내림차순 정렬을 원할때
                            result = String.Compare(rowB.SubItems[currentColumn].Text, rowA.SubItems[currentColumn].Text);
                            break;
                    }
                }
                return result;
            }

            public ColumnSorter() { }
        }
        // 전체 초기화
        private void all_Initialization_Button_Click(object sender, EventArgs e)
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                ComboBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                all_Initialization(_targetTextBox1, _targetTextBox2);
            }
        }

        // 왼쪽 1~20번 초기화
        private void left_init_Click(object sender, EventArgs e)
        {
            for (int _find = 1; _find <= (accountNumber / 2); _find++)
            {
                ComboBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                all_Initialization(_targetTextBox1, _targetTextBox2);
            }
        }
        private void right_init_Click(object sender, EventArgs e)
        {
            for (int _find = ((accountNumber / 2) + 1); _find <= accountNumber; _find++)
            {
                ComboBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as ComboBox);
                TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                all_Initialization(_targetTextBox1, _targetTextBox2);
            }
        }
        private void all_Initialization(ComboBox L, TextBox PBM)
        {
            L.ForeColor = Color.Black;
            L.BackColor = Color.White;
            L.Text = "1";
            int outValue = 0;
            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
            if (_b)
            {
                PBM.Text = outValue.ToString();
            }
        }

        private void btnAllFollow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= 40; _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allFollow(_targetButton);
            }
        }
        private void left_follow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= (accountNumber / 2); _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allFollow(_targetButton);
            }
        }

        private void right_follow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = ((accountNumber / 2) + 1); _find <= accountNumber; _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allFollow(_targetButton);
            }
        }
        private void btnAllUnFollow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allUnFollow(_targetButton);
            }
        }
        private void left_unfollow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= (accountNumber / 2); _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allUnFollow(_targetButton);
            }
        }

        private void right_unfollow_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = ((accountNumber / 2) + 1); _find <= accountNumber; _find++)
            {
                Button _targetButton = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                _allUnFollow(_targetButton);
            }
        }

        private void _allUnFollow(Button _follow)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            _follow.ForeColor = Color.White;
            _follow.BackColor = Color.Black;
            _follow.Text = "반대로";

        }

        private void _allFollow(Button _unFollow)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            _unFollow.ForeColor = Color.Black;
            _unFollow.BackColor = Color.White;
            _unFollow.Text = "따라가기";

        }
        // allBetStop
        private void button93_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= accountNumber; _find++)
            {

                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStop(_target);
            }
        }
        private void left_betStop_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= (accountNumber / 2); _find++)
            {

                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStop(_target);
            }
        }
        private void right_betStop_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = ((accountNumber / 2) + 1); _find <= accountNumber; _find++)
            {
                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStop(_target);
            }
        }


        private void allBettingStart_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStart(_target);
            }
            //배팅 진행
        }
        private void left_betStart_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= (accountNumber / 2); _find++)
            {
                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStart(_target);
            }
        }

        private void right_betStart_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = ((accountNumber / 2) + 1); _find <= accountNumber; _find++)
            {
                Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                _allBettingStart(_target);
            }
        }
        private void _allBettingStop(Button PR)
        {
            PR.Text = "배팅정지";
            PR.ForeColor = Color.White;
            PR.BackColor = Color.DarkSlateGray;
        }

        private void _allBettingStart(Button PR)
        {
            PR.ForeColor = Color.Black;
            PR.BackColor = Color.White;
            PR.Text = "배팅진행";
        }

        private void followUnfollowClick(Button _btn)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            if (_btn.Text.Equals("따라가기"))
            {
                _btn.ForeColor = Color.White;
                _btn.BackColor = Color.Black;
                _btn.Text = "반대로";
            }
            else
            {
                _btn.ForeColor = Color.Black;
                _btn.BackColor = Color.White;
                _btn.Text = "따라가기";
            }
        }

        private void btnFollow1_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow1);
        }

        private void btnFollow2_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow2);
        }

        private void btnFollow3_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow3);
        }

        private void btnFollow4_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow4);
        }

        private void btnFollow5_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow5);
        }


        private void btnFollow6_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow6);
        }

        private void btnFollow7_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow7);
        }

        private void btnFollow8_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow8);
        }

        private void btnFollow9_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow9);
        }

        private void btnFollow10_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow10);
        }

        private void btnFollow11_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow11);
        }

        private void btnFollow12_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow12);
        }
        private void btnFollow13_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow13);
        }


        private void btnFollow14_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow14);
        }


        private void btnFollow15_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow15);
        }

        private void btnFollow16_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow16);
        }
        private void btnFollow17_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow17);
        }

        private void btnFollow18_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow18);
        }

        private void btnFollow19_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow19);
        }

        private void btnFollow20_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow20);
        }

        private void btnFollow21_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow21);
        }

        private void btnFollow22_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow22);
        }

        private void btnFollow23_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow23);
        }

        private void btnFollow24_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow24);
        }

        private void btnFollow25_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow25);
        }

        private void btnFollow26_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow26);
        }

        private void btnFollow27_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow27);
        }

        private void btnFollow28_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow28);
        }

        private void btnFollow29_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow29);
        }

        private void btnFollow30_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow30);
        }

        private void btnFollow31_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow31);
        }

        private void btnFollow32_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow32);
        }

        private void btnFollow33_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow33);
        }

        private void btnFollow34_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow34);
        }

        private void btnFollow35_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow35);
        }

        private void btnFollow36_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow36);
        }

        private void btnFollow37_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow37);
        }

        private void btnFollow38_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow38);
        }

        private void btnFollow39_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow39);
        }

        private void btnFollow40_Click(object sender, EventArgs e)
        {
            followUnfollowClick(btnFollow40);
        }

        private void powerBallReady(Button _btn)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            if (_btn.Text.Equals("배팅진행"))
            {
                _btn.ForeColor = Color.White;
                _btn.BackColor = Color.DarkSlateGray;
                _btn.Text = "배팅정지";
            }
            else
            {
                _btn.ForeColor = Color.Black;
                _btn.BackColor = Color.White;
                _btn.Text = "배팅진행";
            }
        }
        private void btnPR01_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR1);
        }

        private void btnPR02_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR2);
        }

        private void btnPR03_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR3);
        }

        private void btnPR04_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR4);
        }

        private void btnPR05_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR5);
        }

        private void btnPR06_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR6);
        }

        private void btnPR07_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR7);
        }

        private void btnPR08_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR8);
        }

        private void btnPR09_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR9);
        }

        private void btnPR10_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR10);
        }

        private void btnPR11_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR11);
        }

        private void btnPR12_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR12);
        }

        private void btnPR13_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR13);
        }

        private void btnPR14_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR14);
        }


        private void btnPR15_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR15);
        }

        private void btnPR16_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR16);
        }

        private void btnPR17_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR17);
        }


        private void btnPR18_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR18);
        }

        private void btnPR19_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR19);
        }

        private void btnPR20_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR20);
        }


        private void btnPR21_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR21);
        }

        private void btnPR22_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR22);
        }

        private void btnPR23_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR23);
        }

        private void btnPR24_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR24);
        }

        private void btnPR25_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR25);
        }

        private void btnPR26_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR26);
        }

        private void btnPR27_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR27);
        }

        private void btnPR28_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR28);
        }

        private void btnPR29_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR29);
        }

        private void btnPR30_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR30);
        }

        private void btnPR31_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR31);
        }

        private void btnPR32_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR32);
        }

        private void btnPR33_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR33);
        }

        private void btnPR34_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR34);
        }

        private void btnPR35_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR35);
        }

        private void btnPR36_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR36);
        }

        private void btnPR37_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR37);
        }

        private void btnPR38_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR38);
        }

        private void btnPR39_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR39);
        }

        private void btnPR40_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR40);
        }

        private void powerballFormat(Button _btn, ComboBox _L, TextBox _Pbm)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            _L.ForeColor = Color.Black;
            _L.BackColor = Color.White;
            _L.Text = "1";
            int outValue = 0;
            bool _b = int.TryParse(Regex.Replace(txtBtMoneySettingL1.Text, @"\D", ""), out outValue);
            if (_b)
            {
                _Pbm.Text = outValue.ToString();
            }
        }
        private void btnPF01_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF1, CBL1, txtBoxPBM1);
        }

        private void btnPF02_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF2, CBL2, txtBoxPBM2);
        }

        private void btnPF03_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF3, CBL3, txtBoxPBM3);
        }

        private void btnPF04_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF4, CBL4, txtBoxPBM4);
        }



        private void btnPF05_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF5, CBL5, txtBoxPBM5);
        }


        private void btnPF06_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF6, CBL6, txtBoxPBM6);
        }

        private void btnPF07_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF7, CBL7, txtBoxPBM7);
        }


        private void btnPF08_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF8, CBL8, txtBoxPBM8);
        }

        private void btnPF09_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF9, CBL9, txtBoxPBM9);
        }


        private void btnPF10_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF10, CBL10, txtBoxPBM10);
        }

        private void btnPF11_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF11, CBL11, txtBoxPBM11);
        }



        private void btnPF12_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF12, CBL12, txtBoxPBM12);
        }



        private void btnPF13_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF13, CBL13, txtBoxPBM13);
        }


        private void btnPF14_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF14, CBL14, txtBoxPBM14);
        }
        private void btnPF15_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF15, CBL15, txtBoxPBM15);
        }


        private void btnPF16_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF16, CBL16, txtBoxPBM16);
        }



        private void btnPF17_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF17, CBL17, txtBoxPBM17);
        }


        private void btnPF18_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF18, CBL18, txtBoxPBM18);
        }

        private void btnPF19_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF19, CBL19, txtBoxPBM19);
        }



        private void btnPF20_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF20, CBL20, txtBoxPBM20);
        }


        private void btnPF21_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF21, CBL21, txtBoxPBM21);
        }

        private void btnPF22_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF22, CBL22, txtBoxPBM22);
        }

        private void btnPF23_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF23, CBL23, txtBoxPBM23);
        }

        private void btnPF24_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF24, CBL24, txtBoxPBM24);
        }

        private void btnPF25_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF25, CBL25, txtBoxPBM25);
        }

        private void btnPF26_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF26, CBL26, txtBoxPBM26);
        }

        private void btnPF27_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF27, CBL27, txtBoxPBM27);
        }

        private void btnPF28_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF28, CBL28, txtBoxPBM28);
        }

        private void btnPF29_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF29, CBL29, txtBoxPBM29);
        }

        private void btnPF30_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF30, CBL30, txtBoxPBM30);
        }

        private void btnPF31_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF31, CBL31, txtBoxPBM31);
        }

        private void btnPF32_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF32, CBL32, txtBoxPBM32);
        }

        private void btnPF33_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF33, CBL33, txtBoxPBM33);
        }

        private void btnPF34_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF34, CBL34, txtBoxPBM34);
        }

        private void btnPF35_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF35, CBL35, txtBoxPBM35);
        }

        private void btnPF36_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF36, CBL36, txtBoxPBM36);
        }

        private void btnPF37_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF37, CBL37, txtBoxPBM37);
        }

        private void btnPF38_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF38, CBL38, txtBoxPBM38);
        }

        private void btnPF39_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF39, CBL39, txtBoxPBM39);
        }

        private void btnPF40_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF40, CBL40, txtBoxPBM40);
        }

        private void manualModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            txtSwitchMode.Text = "수동 모드";
            txtLogAdd("수동 모드로 선택되었습니다. 해당 모드는 사용자가 모든 픽스터를 선택하는 모드입니다.", Color.White);
            logger.Info("수동 모드를 선택하였습니다.");
        }
        private void autoModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            txtSwitchMode.Text = "자동 모드";
            txtLogAdd("자동 모드가 시작되었습니다. 해당 모드는 40명의 픽스터를 시스템에서 자동으로 선택하여 배팅을 진행합니다.", Color.White);
            logger.Info("자동 모드를 선택하였습니다.");
        }

        private void semiAutoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            txtSwitchMode.Text = "반자동 모드";
            txtLogAdd("반자동 모드가 시작되었습니다. 해당 모드는 설정값에 따라 시스템에서 픽스터를 교체 합니다.", Color.White);
            logger.Info("반자동 모드를 선택하였습니다.");
        }

        private void superiorityModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            txtSwitchMode.Text = "픽스터 우세픽 모드";
            txtLogAdd("픽스터 우세픽 모드가 시작되었습니다. 해당 모드는 픽스터의 홀짝,언오버 우세픽에 따라 배팅합니다.", Color.White);
            logger.Info("우세픽 모드를 선택하였습니다.");

            _superioritymodeSetting("통과", "통과", "통과", "통과");
        }
        private static int CurrentIndex
        {
            get;
            set;
        }
        private ListViewItem FindItem(string keyword, int startIndex)
        {
            for (int i = startIndex; i < listView1.Items.Count; i++)
            {
                ListViewItem item = listView1.Items[i];
                bool isContains = item.SubItems[0].Text.Equals(keyword);
                if (isContains)
                {
                    listView1.EnsureVisible(listView1.Items.Count - (listView1.Items.Count - i));
                    return item;
                }
            }
            return null;
        }

        private void SelectItem(ListViewItem item)
        {
            CurrentIndex = item.Index;

            listView1.MultiSelect = false;
            item.Selected = true;
            listView1.Select();
            listView1.MultiSelect = true;
        }
        private void btnFindPickster_Click(object sender, EventArgs e)
        {
            ListViewItem item = FindItem(textBoxFindPickster.Text, 0);
            //textBoxFindPickster
            if (item == null)
                MessageBox.Show("일치하는 데이터가 없습니다.");
            else
                SelectItem(item);
        }

        private void textBoxFindPickster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ListViewItem item = FindItem(textBoxFindPickster.Text, 0);
                //textBoxFindPickster
                if (item == null)
                    MessageBox.Show("일치하는 데이터가 없습니다.");
                else
                    SelectItem(item);
            }
            else
            {
                return;
            }
        }

        private void btnMagnificationProcessing_Click(object sender, EventArgs e)
        {
            int outSum = 0;
            bool _sum = int.TryParse((Controls.Find("txtBtMoneySettingL1", true)[0] as TextBox).Text, out outSum);
            if (_sum)
            {
                for (int i = 1; i <= 19; i++)
                {
                    TextBox txtBtMoneySetting = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                    textChangeMoneySetting(txtBtMoneySetting);
                    TextBox txtBtMoneySettingUp = (Controls.Find("txtBtMoneySettingL" + (i + 1).ToString(), true)[0] as TextBox);
                    TextBox GetWinMoney = (Controls.Find("GetWinMoney" + (i).ToString(), true)[0] as TextBox);
                    TextBox GetWinProFit = (Controls.Find("GetWinProFit" + (i).ToString(), true)[0] as TextBox);
                    ComboBox magnification = (Controls.Find("magnification" + (i + 1).ToString(), true)[0] as ComboBox);
                    TextBox txtBoxSum = (Controls.Find("txtBoxSum" + (i + 1).ToString(), true)[0] as TextBox);

                    int outValue = 0;
                    bool _b = int.TryParse(Regex.Replace(txtBtMoneySetting.Text, @"\D", ""), out outValue);
                    if (_b)
                    {
                        Double _magnification = Double.Parse(magnification.Text);
                        if (_magnification == 0)
                        {
                            txtBtMoneySettingUp.Text = "-1";
                        }
                        else
                        {
                            if (outValue > 50000000)
                            {
                                txtBtMoneySettingUp.Text = "100";
                            }
                            else
                            {

                                txtBtMoneySettingUp.Text = ((int)(outValue * _magnification)).ToString();
                            }
                        }
                        int getwinmoney = (int)(outValue * 1.95);
                        GetWinMoney.Text = UtilModel.StringFormatChanged(getwinmoney);
                        int winProfit = getwinmoney - outSum;
                        if (winProfit < 0)
                        {
                            winProfit = 0;
                        }
                        GetWinProFit.Text = UtilModel.StringFormatChanged(winProfit);

                        _b = int.TryParse(Regex.Replace(txtBtMoneySettingUp.Text, @"\D", ""), out outValue);
                        if (_b)
                        {
                            outSum += outValue;
                            txtBoxSum.Text = UtilModel.StringFormatChanged(outSum);
                        }
                        else
                        {
                            txtBoxSum.Text = "100";
                        }
                    }
                }
            }
        }
        private void textChangeMoneySetting(TextBox txtBox)
        {
            try
            {
                int outValue = 0;
                bool _b = int.TryParse(Regex.Replace(txtBox.Text, @"\D", ""), out outValue);
                if (_b)
                {
                    txtBox.Text = UtilModel.StringFormatChanged(outValue);
                }
            }
            catch (FormatException a)
            {
                Console.WriteLine(a.Message);
            }
            catch (OverflowException o)
            {
                Console.WriteLine(o.Message);
            }
        }
        private void samePersonCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (samePersonCheckBox.Checked)
            {
                txtLogAdd("동일 유저를 넣을 수 있습니다.", Color.White);
            }
            else
            {
                txtLogAdd("동일 유저를 넣을 수 없습니다.", Color.White);
            }
        }

        private void btn1ConfigLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("1번 설정을 불러오시겠습니까?", "설정 불러오기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLLoad("setting1");
            }
        }

        private void btn2ConfigLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("2번 설정을 불러오시겠습니까?", "설정 불러오기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLLoad("setting2");
            }
        }

        private void btn3ConfigLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("3번 설정을 불러오시겠습니까?", "설정 불러오기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLLoad("setting3");
            }
        }

        private void btn4ConfigLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("4번 설정을 불러오시겠습니까?", "설정 불러오기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLLoad("setting4");
            }
        }

        private void btn5ConfigLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("5번 설정을 불러오시겠습니까?", "설정 불러오기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLLoad("setting5");
            }
        }

        private void btn1ConfigSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("1번 설정을 저장하시겠습니까?", "설정 저장하기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLModifier("setting1");
                MessageBox.Show("1번 설정을 저장 완료하였습니다");
            }
        }

        private void btn2ConfigSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("2번 설정을 저장하시겠습니까?", "설정 저장하기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLModifier("setting2");
                MessageBox.Show("2번 설정을 저장 완료하였습니다");
            }
        }

        private void btn3ConfigSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("3번 설정을 저장하시겠습니까?", "설정 저장하기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLModifier("setting3");
                MessageBox.Show("3번 설정을 저장 완료하였습니다");
            }
        }

        private void btn4ConfigSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("4번 설정을 저장하시겠습니까?", "설정 저장하기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLModifier("setting4");
                MessageBox.Show("4번 설정을 저장 완료하였습니다");
            }
        }

        private void btn5ConfigSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("5번 설정을 저장하시겠습니까?", "설정 저장하기", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                XMLModifier("setting5");
                MessageBox.Show("5번 설정을 저장 완료하였습니다");
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

                for (int i = 1; i <= 20; i++)
                {
                    DeleteNode = SubNode.SelectSingleNode("levelmoney" + i);
                    SubNode.RemoveChild(DeleteNode);

                    TextBox _msl = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                    int outValue = 0;
                    bool _b = int.TryParse(Regex.Replace(_msl.Text, @"\D", ""), out outValue);
                    if (_b)
                    {
                        SubNode.AppendChild(CreateNode(XmlDoc, "levelmoney" + i, _msl.Text));
                    }
                }

                for (int i = 2; i <= 20; i++)
                {
                    DeleteNode = SubNode.SelectSingleNode("magnification" + i);
                    SubNode.RemoveChild(DeleteNode);

                    ComboBox _mag = (Controls.Find("magnification" + i.ToString(), true)[0] as ComboBox);
                    SubNode.AppendChild(CreateNode(XmlDoc, "magnification" + i, _mag.Text));
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

                for (int i = 1; i <= 20; i++)
                {
                    selectNode = SubNode.SelectSingleNode("levelmoney" + i);

                    TextBox _msl = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                    _msl.Text = selectNode.InnerText;
                }

                for (int i = 2; i <= 20; i++)
                {
                    selectNode = SubNode.SelectSingleNode("magnification" + i);

                    ComboBox _mag = (Controls.Find("magnification" + i.ToString(), true)[0] as ComboBox);
                    _mag.Text = selectNode.InnerText;
                }

            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }

        void LevelChangeSelectedindexChange(ComboBox _level, int num)
        {
            TextBox _msl = (Controls.Find("txtBtMoneySettingL" + _level.Text, true)[0] as TextBox);
            TextBox _betMoney = (Controls.Find("txtBoxPBM" + num, true)[0] as TextBox);

            _level.ForeColor = Color.Black;
            _level.BackColor = Color.White;
            if (_betMoney != null)
            {
                int outValue = 0;
                bool _b = int.TryParse(Regex.Replace(_msl.Text, @"\D", ""), out outValue);
                if (_b)
                {

                }
                _betMoney.Text = outValue.ToString();
            }

            logger.Info("사용자가 " + num + "번째 픽스터의 레벨을 " + _level.Text + " 레벨로 변경하였습니다");
        }

        private void CBL1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL1, 1);
        }

        private void CBL2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL2, 2);
        }

        private void CBL3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL3, 3);
        }

        private void CBL4_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL4, 4);
        }

        private void CBL5_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL5, 5);
        }

        private void CBL6_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL6, 6);
        }

        private void CBL7_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL7, 7);
        }

        private void CBL8_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL8, 8);
        }

        private void CBL9_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL9, 9);
        }

        private void CBL10_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL10, 10);
        }

        private void CBL11_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL11, 11);
        }

        private void CBL12_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL12, 12);
        }

        private void CBL13_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL13, 13);
        }

        private void CBL14_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL14, 14);
        }

        private void CBL15_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL15, 15);
        }

        private void CBL16_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL16, 16);
        }

        private void CBL17_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL17, 17);
        }

        private void CBL18_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL18, 18);
        }

        private void CBL19_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL19, 19);
        }

        private void CBL20_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL20, 20);
        }

        private void CBL21_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL21, 21);
        }

        private void CBL22_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL22, 22);
        }

        private void CBL23_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL23, 23);
        }

        private void CBL24_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL24, 24);
        }

        private void CBL25_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL25, 25);
        }

        private void CBL26_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL26, 26);
        }

        private void CBL27_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL27, 27);
        }

        private void CBL28_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL28, 28);
        }

        private void CBL29_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL29, 29);
        }

        private void CBL30_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL30, 30);
        }

        private void CBL31_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL31, 31);
        }

        private void CBL32_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL32, 32);
        }

        private void CBL33_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL33, 33);
        }

        private void CBL34_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL34, 34);
        }

        private void CBL35_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL35, 35);
        }

        private void CBL36_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL36, 36);
        }

        private void CBL37_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL37, 37);
        }

        private void CBL38_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL38, 38);
        }

        private void CBL39_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL39, 39);
        }

        private void CBL40_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelChangeSelectedindexChange(CBL40, 40);
        }

        private void BoxPick1_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick1.Text + "] 로 변경하였습니다");
        }

        private void BoxPick2_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick2.Text + "] 로 변경하였습니다");
        }

        private void BoxPick3_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick3.Text + "] 로 변경하였습니다");
        }

        private void BoxPick4_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick4.Text + "] 로 변경하였습니다");
        }

        private void BoxPick5_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick5.Text + "] 로 변경하였습니다");
        }

        private void BoxPick6_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick6.Text + "] 로 변경하였습니다");
        }

        private void BoxPick7_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick7.Text + "] 로 변경하였습니다");
        }

        private void BoxPick8_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick8.Text + "] 로 변경하였습니다");
        }

        private void BoxPick9_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick9.Text + "] 로 변경하였습니다");
        }

        private void BoxPick10_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick10.Text + "] 로 변경하였습니다");
        }

        private void BoxPick11_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick11.Text + "] 로 변경하였습니다");
        }

        private void BoxPick12_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick12.Text + "] 로 변경하였습니다");
        }

        private void BoxPick13_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick13.Text + "] 로 변경하였습니다");
        }

        private void BoxPick14_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick14.Text + "] 로 변경하였습니다");
        }

        private void BoxPick15_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick15.Text + "] 로 변경하였습니다");
        }

        private void BoxPick16_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick16.Text + "] 로 변경하였습니다");
        }

        private void BoxPick17_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick17.Text + "] 로 변경하였습니다");
        }

        private void BoxPick18_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick18.Text + "] 로 변경하였습니다");
        }

        private void BoxPick19_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick19.Text + "] 로 변경하였습니다");
        }

        private void BoxPick20_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick20.Text + "] 로 변경하였습니다");
        }

        private void BoxPick21_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick21.Text + "] 로 변경하였습니다");
        }

        private void BoxPick22_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick22.Text + "] 로 변경하였습니다");
        }

        private void BoxPick23_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick23.Text + "] 로 변경하였습니다");
        }

        private void BoxPick24_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick24.Text + "] 로 변경하였습니다");
        }

        private void BoxPick25_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick25.Text + "] 로 변경하였습니다");
        }

        private void BoxPick26_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick26.Text + "] 로 변경하였습니다");
        }

        private void BoxPick27_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick27.Text + "] 로 변경하였습니다");
        }

        private void BoxPick28_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick28.Text + "] 로 변경하였습니다");
        }

        private void BoxPick29_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick29.Text + "] 로 변경하였습니다");
        }

        private void BoxPick30_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick30.Text + "] 로 변경하였습니다");
        }

        private void BoxPick31_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick31.Text + "] 로 변경하였습니다");
        }

        private void BoxPick32_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick32.Text + "] 로 변경하였습니다");
        }

        private void BoxPick33_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick33.Text + "] 로 변경하였습니다");
        }

        private void BoxPick34_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick34.Text + "] 로 변경하였습니다");
        }

        private void BoxPick35_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick35.Text + "] 로 변경하였습니다");
        }

        private void BoxPick36_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick36.Text + "] 로 변경하였습니다");
        }

        private void BoxPick37_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick37.Text + "] 로 변경하였습니다");
        }

        private void BoxPick38_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick38.Text + "] 로 변경하였습니다");
        }

        private void BoxPick39_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick39.Text + "] 로 변경하였습니다");
        }

        private void BoxPick40_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Info("사용자가 픽을 [" + BoxPick40.Text + "] 로 변경하였습니다");
        }

        void findUser(Button _pickster)
        {
            ListViewItem item = FindItem(_pickster.Text, 0);
            //textBoxFindPickster
            if (item == null)
                MessageBox.Show("일치하는 데이터가 없습니다.");
            else
                SelectItem(item);
        }
        private void BoxPickster1_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster1);
        }

        private void BoxPickster2_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster2);
        }

        private void BoxPickster3_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster3);
        }

        private void BoxPickster4_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster4);
        }

        private void BoxPickster5_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster5);
        }

        private void BoxPickster6_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster6);
        }

        private void BoxPickster7_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster7);
        }

        private void BoxPickster8_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster8);
        }

        private void BoxPickster9_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster9);
        }

        private void BoxPickster10_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster10);
        }

        private void BoxPickster11_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster11);
        }

        private void BoxPickster12_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster12);
        }

        private void BoxPickster13_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster13);
        }

        private void BoxPickster14_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster14);
        }

        private void BoxPickster15_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster15);
        }

        private void BoxPickster16_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster16);
        }

        private void BoxPickster17_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster17);
        }

        private void BoxPickster18_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster18);
        }

        private void BoxPickster19_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster19);
        }

        private void BoxPickster20_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster20);
        }

        private void BoxPickster21_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster21);
        }

        private void BoxPickster22_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster22);
        }

        private void BoxPickster23_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster23);
        }

        private void BoxPickster24_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster24);
        }

        private void BoxPickster25_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster25);
        }

        private void BoxPickster26_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster26);
        }

        private void BoxPickster27_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster27);
        }

        private void BoxPickster28_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster28);
        }

        private void BoxPickster29_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster29);
        }

        private void BoxPickster30_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster30);
        }

        private void BoxPickster31_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster31);
        }

        private void BoxPickster32_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster32);
        }

        private void BoxPickster33_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster33);
        }

        private void BoxPickster34_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster34);
        }

        private void BoxPickster35_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster35);
        }

        private void BoxPickster36_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster36);
        }

        private void BoxPickster37_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster37);
        }

        private void BoxPickster38_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster38);
        }

        private void BoxPickster39_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster39);
        }

        private void BoxPickster40_Click(object sender, EventArgs e)
        {
            findUser(BoxPickster40);
        }

        private void leftDeletePickster_Click(object sender, EventArgs e)
        {
            for (int _find = 1; _find <= accountNumber; _find++)
            {
                Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                TextBox _boxPr = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                TextBox _boxPs = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                _pickster.Text = "------";
                _ballType.Text = "";
                _boxPr.Text = "";
                _boxPs.Text = "";
            }
        }

        private void autoRefillButton_Click(object sender, EventArgs e)
        {
            if (!_bettingClosed)
            {
                systemPicksterAutoRefill();
            }
        }

        private Boolean systemPicksterAutoRefill()
        {
            String pP = "";

            int n = 1;
            for (int i = 0; i < arrayNum; i++)
            {
                if (_picksterInformation[i, 0] == null)
                {
                    break;
                }

                if (_picksterInformation[i, 0].Contains("*"))
                {
                    // 파워픽 4, 일반픽 17
                    pP = _picksterInformation[i, 4];
                    if (!pP.Contains("P"))
                    {
                        n++;
                    }
                }
            }
            int _automodelimit = int.Parse(automodelimit.Text);
            if (n > _automodelimit)
            {
                String[] arrayPickster = new String[n];
                int num = 0;
                for (int i = 0; i < arrayNum; i++)
                {
                    if (_picksterInformation[i, 0] == null)
                    {
                        break;
                    }
                    if (_picksterInformation[i, 0].Contains("*"))
                    {
                        // 파워픽 4, 일반픽 17
                        pP = _picksterInformation[i, 4];
                        if (!pP.Contains("P"))
                        {
                            arrayPickster[num++] = _picksterInformation[i, 0];
                        }
                    }
                }
                var rng = new Random();
                rng.Shuffle(arrayPickster);
                Random rand = new Random(); //랜덤선언 _picksterInformation[k, 0]
                int r = 0;

                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    String strpickster = arrayPickster[_find];
                    if (strpickster != null)
                    {
                        for (int _find2 = 1; _find2 <= accountNumber; _find2++)
                        {
                            Button _pickster = (Controls.Find("BoxPickster" + _find2.ToString(), true)[0] as Button);
                            if (_pickster.Text.Equals(strpickster))
                            {
                                r = rand.Next(0, (arrayPickster.Length - 1));
                                strpickster = arrayPickster[r];
                                break;
                            }
                        }

                        int index = Array.IndexOf(_picksterInformation, strpickster);
                        if (!String.IsNullOrEmpty(_picksterInformation[index, 0]))
                        {
                            Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                            TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _boxPr = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                            TextBox _boxPs = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                            _pickster.Text = _picksterInformation[index, 0].ToString(); // 픽스터 이름
                            _ballType.Text = _picksterInformation[index, 1].ToString(); // 픽스터 이름
                            _boxPr.Text = _picksterInformation[index, 2].ToString(); // 픽스터 이름
                            _boxPs.Text = _picksterInformation[index, 3].ToString(); // 픽스터 이름
                        }
                    }
                }
                return true;
            }
            else
            {
                txtLogAdd("[" + n + "] 픽을 한 픽스터가 " + _automodelimit + "명 이하라서 통과하였습니다.", Color.White);
                return false;
            }
        }

        private void PatternPickUse_CheckedChanged(object sender, EventArgs e)
        {
            if (PatternPickUse.Checked)
            {
                resultMarkCheckBox.Checked = false;
                samePersonCheckBox.Checked = false;
                picksterChangeRound.Text = "0";
            }
        }

        private void systemPickUseCheck_CheckedChanged(object sender, EventArgs e)
        {
            int _r = 0;
            if (systemPickUseCheck.Checked)
            {
                int loadNum = int.Parse(loadPicksterNumber.Text);
                if (loadNum == 100)
                {
                    _r = rand.Next(10001, 10901);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 99;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 50)
                {
                    _r = rand.Next(10001, 10951);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 49;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 150)
                {
                    _r = rand.Next(10001, 10851);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 149;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 200)
                {
                    _r = rand.Next(10001, 10801);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 199;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 250)
                {
                    _r = rand.Next(10001, 10751);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 249;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 300)
                {
                    _r = rand.Next(10001, 10701);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 299;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 350)
                {
                    _r = rand.Next(10001, 10651);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 349;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 400)
                {
                    _r = rand.Next(10001, 10601);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 399;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 450)
                {
                    _r = rand.Next(10001, 10551);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 449;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 500)
                {
                    _r = rand.Next(10001, 10501);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 499;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else
                {
                    _r = rand.Next(10001, 10901);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 99;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
            }
            else
            {
                loadPickster_min = 0;
                loadPickster_max = 0;
            }
        }

        private void loadPicksterNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            int _r = 0;
            if (systemPickUseCheck.Checked)
            {
                int loadNum = int.Parse(loadPicksterNumber.Text);
                if (loadNum == 100)
                {
                    _r = rand.Next(10001, 10901);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 99;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 50)
                {
                    _r = rand.Next(10001, 10951);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 49;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 150)
                {
                    _r = rand.Next(10001, 10851);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 149;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 200)
                {
                    _r = rand.Next(10001, 10801);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 199;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 250)
                {
                    _r = rand.Next(10001, 10751);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 249;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 300)
                {
                    _r = rand.Next(10001, 10701);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 299;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 350)
                {
                    _r = rand.Next(10001, 10651);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 349;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 400)
                {
                    _r = rand.Next(10001, 10601);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 399;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 450)
                {
                    _r = rand.Next(10001, 10551);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 449;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else if (loadNum == 500)
                {
                    _r = rand.Next(10001, 10501);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 499;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
                else
                {
                    _r = rand.Next(10001, 10901);

                    loadPickster_min = _r;
                    loadPickster_max = _r + 99;
                    txtLogAdd("[" + loadPickster_min + "] ~ [" + loadPickster_max + "] 의 픽스터를 불러옵니다..", Color.White);
                }
            }
            else
            {
                loadPickster_min = 0;
                loadPickster_max = 0;
            }
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                string picksterName = lvItem.SubItems[0].Text;

                if (!samePersonCheckBox.Checked)
                {
                    Boolean _bool = false;
                    int findNum = 0;
                    for (int _find = 1; _find <= accountNumber; _find++)
                    {
                        Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                        if (_pickster.Text.Equals(picksterName))
                        {
                            findNum = _find;
                            _bool = true;
                            break;
                        }
                    }
                    if (_bool)
                    {
                        txtLogAdd("이미 존재하는 픽스터입니다. [" + findNum + "]번째 칸에 있습니다.", Color.FromArgb(230, 38, 38));
                        return;
                    }
                }
                for (int _find = 1; _find <= accountNumber; _find++)
                {
                    Button _pickster = (Controls.Find("BoxPickster" + _find.ToString(), true)[0] as Button);
                    if (_pickster.Text.Contains("--"))
                    {
                        _pickster.Text = picksterName; // 픽스터 이름
                        TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                        TextBox _boxPr = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                        TextBox _boxPs = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                        _ballType.Text = "--"; // 픽스터 이름
                        _boxPr.Text = "--"; // 픽스터 이름
                        _boxPs.Text = "--"; // 픽스터 이름
                        break;
                    }
                }
            }
        }

        private void moneyUp1000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 1000)).ToString();
            }
        }

        private void moneyUp5000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 5000)).ToString();
            }
        }

        private void moneyUp10000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 10000)).ToString();
            }
        }
        private void moneyUp30000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 30000)).ToString();
            }
        }
        private void money50000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 50000)).ToString();
            }
        }

        private void moneyUp100000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 100000)).ToString();
            }
        }
        private void moneyUp200000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 200000)).ToString();
            }
        }
        private void moneyUp300000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 300000)).ToString();
            }
        }

        private void moneyUp500000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 500000)).ToString();
            }
        }

        private void moneyUp1000000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 1000000)).ToString();
            }
        }


        private void moneyUp2000000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 2000000)).ToString();
            }
        }

        private void moneyUp5000000_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 5000000)).ToString();
            }
        }

        private void moneyUp100_Click(object sender, EventArgs e)
        {
            int money = 0;
            bool result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (result)
            {
                moneyUp.Text = (UtilModel.StringFormatChanged(money + 100)).ToString();
            }
        }
        private void moneyUpInit_Click(object sender, EventArgs e)
        {
            moneyUp.Text = "0";
        }

        private void directBetButton_Click(object sender, EventArgs e)
        {
            if (remainingTime < 48 || remainingTime > 270)
            {
                MessageBox.Show("직접 배팅 시간이 종료되었습니다. 남은 시간 50초 이상, 4분 이하일 때만 가능합니다.");
                return;
            }
            if (!_isStart)
            {
                MessageBox.Show("배팅 시작 버튼을 눌러주세요!");
                return;
            }
            pOddMoney = 0;
            pEvenMoney = 0;
            pUnderMoney = 0;
            pOverMoney = 0;
            nOddMoney = 0;
            nEvenMoney = 0;
            nUnderMoney = 0;
            nOverMoney = 0;
            int money = 0;
            bool _result = int.TryParse(Regex.Replace(moneyUp.Text, @"\D", ""), out money);
            if (_result)
            {
                if (money <= 0)
                {
                    MessageBox.Show("배팅금액이 0원입니다.");
                    return;
                }
            }

            if (money > 0)
            {
                String _pick = "";
                int betType = 0;
                string gm = null;
                if (pOddCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "파워볼 [홀]";
                    pOddMoney = money;
                    pOddCheck.Checked = false;
                }
                else if (pEvenCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "파워볼 [짝]";
                    pEvenMoney = money;
                    pEvenCheck.Checked = false;
                }
                else if (pUnderCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "파워볼 [언더]";
                    pUnderMoney = money;
                    pUnderCheck.Checked = false;
                }
                else if (pOverCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "파워볼 [오버]";
                    pOverMoney = money;
                    pOverCheck.Checked = false;
                }
                else if (nOddCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "일반볼 [홀]";
                    nOddMoney = money;
                    nOddCheck.Checked = false;
                }
                else if (nEvenCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "일반볼 [짝]";
                    nEvenMoney = money;
                    nEvenCheck.Checked = false;
                }
                else if (nUnderCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "일반볼 [언더]";
                    nUnderMoney = money;
                    nUnderCheck.Checked = false;
                }
                else if (nOverCheck.Checked)
                {
                    gm = "PWB";
                    betType = 1;
                    _pick = "일반볼 [오버]";
                    nOverMoney = money;
                    nOverCheck.Checked = false;

                    // 파워볼 조합
                }
                else if (pOddUnderCheck.Checked)
                {
                    gm = "PWB_PB_MIX";
                    betType = 2;
                    _pick = "파워볼 조합 [홀][언더]";
                    pOddMoney = money;
                    pOddUnderCheck.Checked = false;
                }
                else if (pOddOverCheck.Checked)
                {
                    gm = "PWB_PB_MIX";
                    betType = 2;
                    _pick = "파워볼 조합 [홀][오버]";
                    pEvenMoney = money;
                    pOddOverCheck.Checked = false;
                }
                else if (pEvenUnderCheck.Checked)
                {
                    gm = "PWB_PB_MIX";
                    betType = 2;
                    _pick = "파워볼 조합 [짝][언더]";
                    pUnderMoney = money;
                    pEvenUnderCheck.Checked = false;
                }
                else if (pEvenOverCheck.Checked)
                {
                    gm = "PWB_PB_MIX";
                    betType = 2;
                    _pick = "파워볼 조합 [짝][오버]";
                    pOverMoney = money;
                    pEvenOverCheck.Checked = false;
                    // 일반볼 조합
                }
                else if (nOddUnderCheck.Checked)
                {
                    gm = "PWB_NB_MIX";
                    betType = 3;
                    _pick = "일반볼 조합 [홀][언더]";
                    pOddMoney = money;
                    nOddUnderCheck.Checked = false;
                }
                else if (nOddOverCheck.Checked)
                {
                    gm = "PWB_NB_MIX";
                    betType = 3;
                    _pick = "일반볼 조합 [홀][오버]";
                    pEvenMoney = money;
                    nOddOverCheck.Checked = false;
                }
                else if (nEvenUnderCheck.Checked)
                {
                    gm = "PWB_NB_MIX";
                    betType = 3;
                    _pick = "일반볼 조합 [짝][언더]";
                    pUnderMoney = money;
                    nEvenUnderCheck.Checked = false;
                }
                else if (nEvenOverCheck.Checked)
                {
                    gm = "PWB_NB_MIX";
                    betType = 3;
                    _pick = "일반볼 조합 [짝][오버]";
                    pOverMoney = money;
                    nEvenOverCheck.Checked = false;

                    // 파워사다리
                }
                else if (ladderLeftCheck.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워 사다리 [좌]";
                    pOddMoney = money;
                    ladderLeftCheck.Checked = false;
                }
                else if (ladderRightCheck.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워 사다리 [우]";
                    pEvenMoney = money;
                    ladderRightCheck.Checked = false;
                }
                else if (ladder3Check.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워 사다리 [3]";
                    pUnderMoney = money;
                    ladder3Check.Checked = false;
                }
                else if (ladder4Check.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워사다리 [4]";
                    pOverMoney = money;
                    ladder4Check.Checked = false;
                }
                else if (ladderOddCheck.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워사다리 [홀]";
                    nOddMoney = money;
                    ladderOddCheck.Checked = false;
                }
                else if (ladderEvenCheck.Checked)
                {
                    gm = "PSA";
                    betType = 4;
                    _pick = "파워사다리 [짝]";
                    nEvenMoney = money;
                    ladderEvenCheck.Checked = false;
                }
                if (betType > 0)
                {
                    DialogResult result = MessageBox.Show("선택된 " + _pick + " 픽에 " + UtilModel.StringFormatChanged(money) + " 를 배팅하시겠습니까?", "직접 배팅 진행", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        if (betType > 0)
                        {
                            powerballBetting(betType, gm);
                        }
                        moneyUp.Text = "0";
                    }
                    else
                    {
                        txtLogAdd("직접 배팅이 취소 되었습니다.", Color.White);
                    }
                }
            }
            else
            {
                txtLogAdd("선택된 픽이 없거나 선택을 할 수 없는 픽입니다.", Color.White);
                return;
            }
        }

        private Boolean IsFolded { get; set; }

        private void extendSize_Click(object sender, EventArgs e)
        {
            if (IsFolded == false)
            {
                // 접힌 상태의 사이즈 설정
                this.Size = new Size(2542, 1400);
            }
            else
            {
                // 펼쳐진 상태의 사이즈 설정
                this.Size = new Size(1938, 1047);
            }
            IsFolded = !(IsFolded);
        }

        private void userPickRegist_Click(object sender, EventArgs e)
        {
            MessageBox.Show("아직 지원되지 않는 기능입니다.");
        }
        // 0 = 실제 배팅 
        // 1 = 가상 배팅
        int realOrVirtualMode = 0;
        private void RealOrVirtualBettingMode_Click(object sender, EventArgs e)
        {
            if (_isStart)
            {
                MessageBox.Show("게임이 진행 중입니다. 배팅 정지 후 변경 하십시요!");
                return;
            }
            if (realOrVirtualMode == 0)
            {
                panel3.BackColor = Color.FromArgb(78, 52, 46);
                realOrVirtualMode = 1;
                RealOrVirtualBettingMode.ForeColor = Color.FromArgb(244, 143, 177);
                RealOrVirtualBettingMode.Text = "가상 배팅 진행 중 !";
                lblTxtStartMoney.Text = "10,000,000원";
                lblTxtNowMoney.Text = "10,000,000원";
                lblTxtNowGain.Text = "0";
                startMoney = 10000000;
                NowMoney = 10000000;
            }
            else if (realOrVirtualMode == 1)
            {
                panel3.BackColor = Color.FromArgb(66, 66, 66);
                realOrVirtualMode = 0;
                RealOrVirtualBettingMode.ForeColor = Color.FromArgb(100, 181, 246);
                RealOrVirtualBettingMode.Text = "실제 배팅 진행 중 !";
                lblTxtStartMoney.Text = "0";
                lblTxtNowMoney.Text = "0";
                lblTxtNowGain.Text = "0";
                startMoney = 0;
                NowMoney = 0;
            }
        }
    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
    static class Extensions // 리스트뷰 깜박임 제거
    {
        public static void DoubleBuffered(this Control control, bool enabled)

        {
            var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, enabled, null);
        }
    }

    static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
