using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
namespace AutoMartinProgram
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainForm()
        {
            FormClosing += MainForm_FormClosing;
            InitializeComponent();
        }
        protected virtual bool DoubleBuffered { get; set; }
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
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
            this.Text += " : " + UtilModel._id;
            this.Text += " : " + UtilModel._programVersion;
            this.Text += " : " + UtilModel._limittime;
            txtSwitchMode.Text = "수동모드";

            if (UtilModel._allBettingEnable == 1)
            {
                this.label9.Text = "총 배팅금 :";
                this.label10.Visible = true;
            }
            Random rand = new Random();
            _r = rand.Next(100000, 999999);
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록
        }

        int startMoney = 0;
        int NowMoney = 0;

        int _r = 0;
        // selenium 의 chrome 정보

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
            MessageBox.Show(nickName + "님 반갑습니다. 해당 프로그램은 고객님의 배팅에 도움을 주기 위해 만들어진 프로그램입니다. 해당 프로그램을 맹신하지 말아주시기 바랍니다.");
        }

        // 종료시 처리 자동으로 금액 저장
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("정말로 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
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


        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        int allinning = 0;
        int todayinning = 0;
        int resultAllInning = 0;
        int resultTodayInning = 0;
        int powerball = 0;
        int normalball = 0;
        string powerballSP = null;
        string powerballUO = null;
        string normalballSP = null;
        string normalballUO = null;

        int pOverMoney; // 파워볼 오버
        int pUnderMoney; // 파워볼 언더
        int pOddMoney; // 파워볼 홀
        int pEvenMoney; // 파워볼 짝
        int nOverMoney;
        int nUnderMoney;
        int nOddMoney;
        int nEvenMoney;

        Double oddPercent = 0;
        Double evenPercent = 0;
        Double overPercent = 0;
        Double underPercent = 0;

        int allbettingMoney;
        Boolean _isStart = false;
        // 결과 뽑아오는 부분
        private void powerballResult()
        {
            try
            {
                String _uri = UtilModel.powerresult;
                _uri += "?a=" + (allinning - 1);
                _uri += "&username=" + UtilModel._id;
                _uri += "&password=" + UtilModel._password;
                _uri += "&timetoken=" + UtilModel._timetoken;
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("result"))
                {
                    resultAllInning = int.Parse(item.Attribute("allinning").Value);
                    resultTodayInning = int.Parse(item.Attribute("todayinning").Value);
                    powerball = int.Parse(item.Attribute("powerball").Value);
                    normalball = int.Parse(item.Attribute("normalball").Value);

                    if (powerball > 4)
                    {
                        powerballUO = "오버";
                    }
                    else
                    {
                        powerballUO = "언더";
                    }
                    if (powerball % 2 == 0)
                    {
                        powerballSP = "짝";
                    }
                    else
                    {
                        powerballSP = "홀";
                    }
                    if (normalball > 72)
                    {
                        normalballUO = "오버";
                    }
                    else
                    {
                        normalballUO = "언더";
                    }
                    if (normalball % 2 == 0)
                    {
                        normalballSP = "짝";
                    }
                    else
                    {
                        normalballSP = "홀";
                    }
                };
                txtLogAdd(resultAllInning + " / " + resultTodayInning
                             + " / 파워볼 : " + powerball + " / " + powerballSP + " / " + powerballUO + " / "
                                + " 일반볼/ " + normalball + " / " + normalballSP + " / " + normalballUO, Color.MediumVioletRed);
                logger.Info(resultAllInning + " / " + resultTodayInning
                             + " / 파워볼 : " + powerball + " / " + powerballSP + " / " + powerballUO + " / "
                                + " 일반볼/ " + normalball + " / " + normalballSP + " / " + normalballUO);
            }
            catch (Exception _ex)
            {
                textBox2.Text += _ex.ToString();
            }
        }

        // 전체, 당일, 남은 시간
        private int getPowerballInformation()
        {
            try
            {
                String _uri = UtilModel.servertime;
                _uri += "?username=" + UtilModel._id;
                _uri += "&password=" + UtilModel._password;
                _uri += "&timetoken=" + UtilModel._timetoken;
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("item"))
                {
                    allinning = int.Parse(item.Attribute("allinning").Value);
                    todayinning = int.Parse(item.Attribute("todayinning").Value);
                    remainingTime = int.Parse(item.Attribute("timediffer").Value);
                };

                round.Text = allinning + "회";
                logger.Info("전체회차 : " + allinning + " / 오늘 회차 : " + todayinning);
            }
            catch (Exception _ex)
            {
                textBox2.Text += _ex.ToString();
            }
            return allinning;
        }

        // 전체 회차 및 당일 회차 정보
        private int getPowerballInning()
        {
            try
            {
                String _uri = UtilModel.servertime;
                _uri += "?username=" + UtilModel._id;
                _uri += "&password=" + UtilModel._password;
                _uri += "&timetoken=" + UtilModel._timetoken;
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("item"))
                {
                    allinning = int.Parse(item.Attribute("allinning").Value);
                    todayinning = int.Parse(item.Attribute("todayinning").Value);
                };

                round.Text = allinning + "회";
            }
            catch (Exception _ex)
            {
                textBox2.Text += _ex.ToString();
            }
            return allinning;
        }

        int _powerballEven = 0; // 짝수
        int _powerballOdd = 0; // 홀수
        int _powerballUnder = 0; // 언더
        int _powerballOver = 0; // 오버

        String loadPicksterType = "ntry";
        // 픽스터 정보를 뽑아와서 배열에 저장 후 listview 에 뿌려주는 부분
        private void pickPickster()
        {
            try
            {
                if (_patternMode)
                {
                    listView1.Items.Clear();
                    btnPicksterChoiceOddNum.Text = "0";
                    btnPicksterChoiceEvenNum.Text = "0";
                    btnPicksterChoiceOverNum.Text = "0";
                    btnPicksterChoiceUnderNum.Text = "0";

                    btnPicksterChoiceOddPercent.Text = "0";
                    btnPicksterChoiceEvenPercent.Text = "0";
                    btnPicksterChoiceOverPercent.Text = "0";
                    btnPicksterChoiceUnderPercent.Text = "0";
                    return;
                }
                _powerballEven = 0; // 짝수
                _powerballOdd = 0; // 홀수
                _powerballUnder = 0; // 언더
                _powerballOver = 0; // 오버

                _picksterInformation = new string[200, 27];
                String page = UtilModel.picksterlist;
                page += "?allinning=" + (todayinning + 1);
                page += "&username=" + UtilModel._id;
                page += "&password=" + UtilModel._password;
                page += "&timetoken=" + UtilModel._timetoken;
                page += "&type=" + loadPicksterType;
                XElement root = XElement.Load(page);
                //하위 노드를 읽는 방법 (XML 원본 참고)
                String pp = "";
                String pb1 = "";
                String pb2 = "";
                String pb3 = "";
                String pb4 = "";
                String pb5 = "";
                String pb6 = "";
                String pb7 = "";
                String pb8 = "";
                String pb9 = "";
                String site = "";
                int k = 0;
                foreach (XElement item in root.Descendants("item"))
                {
                    pp = item.Attribute("pp").Value;
                    pb1 = item.Attribute("pb1").Value;
                    pb2 = item.Attribute("pb2").Value;
                    pb3 = item.Attribute("pb3").Value;
                    pb4 = item.Attribute("pb4").Value;
                    pb5 = item.Attribute("pb5").Value;
                    pb6 = item.Attribute("pb6").Value;
                    pb7 = item.Attribute("pb7").Value;
                    pb8 = item.Attribute("pb8").Value;
                    pb9 = item.Attribute("pb9").Value;
                    site = item.Attribute("site").Value;

                    if (pp.Contains("P") && pb1.Contains("P") && pb2.Contains("P") && pb3.Contains("P"))
                    {
                        continue;
                    }
                    else
                    {
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
                        string[] row = {
                        item.Attribute("pickstername").Value, "파워", item.Attribute("pbrecord").Value, item.Attribute("pbstreak").Value,
                        pp, pb1, pb2, pb3, pb4, pb5, pb6, pb7, pb8, pb9,
                        "일반", item.Attribute("nbrecord").Value, item.Attribute("nbstreak").Value, item.Attribute("np").Value, item.Attribute("nb1").Value, item.Attribute("nb2").Value, item.Attribute("nb3").Value, item.Attribute("nb4").Value,
                        item.Attribute("nb5").Value, item.Attribute("nb6").Value, item.Attribute("nb7").Value, item.Attribute("nb8").Value, item.Attribute("nb9").Value
                        };

                        for (int j = 0; j < row.Length; j++)
                        {
                            _picksterInformation[k, j] = row[j];
                        }
                        k++;
                    }
                };
                btnPicksterChoiceOddNum.Text = _powerballOdd.ToString();
                btnPicksterChoiceEvenNum.Text = _powerballEven.ToString();
                btnPicksterChoiceOverNum.Text = _powerballOver.ToString();
                btnPicksterChoiceUnderNum.Text = _powerballUnder.ToString();

                oddPercent = ((Double)_powerballOdd / ((Double)_powerballOdd + (Double)_powerballEven)) * 100;
                evenPercent = ((Double)_powerballEven / ((Double)_powerballOdd + (Double)_powerballEven)) * 100;
                overPercent = ((Double)_powerballOver / ((Double)_powerballOver + (Double)_powerballUnder)) * 100;
                underPercent = ((Double)_powerballUnder / ((Double)_powerballOver + (Double)_powerballUnder)) * 100;

                btnPicksterChoiceOddPercent.Text = oddPercent.ToString("F") + "%";
                btnPicksterChoiceEvenPercent.Text = evenPercent.ToString("F") + "%";
                btnPicksterChoiceOverPercent.Text = overPercent.ToString("F") + "%";
                btnPicksterChoiceUnderPercent.Text = underPercent.ToString("F") + "%";
                listView1.Items.Clear();
                textLog.Text = "";
                for (int i = 0; i < 200; i++)
                {
                    ListViewItem item;
                    if (_picksterInformation[i, 0] != null)
                    {
                        if (_picksterInformation[i, 0].Contains("모드"))
                        {
                            continue;
                        }
                        // 픽스터 이름 = (_picksterInformation[i, 0]
                        for (int num = 1; num <= 40; num++)
                        {
                            if (num < 10)
                            {
                                TextBox _pickster = (Controls.Find("txtBoxPickster0" + num.ToString(), true)[0] as TextBox);
                                if (_pickster.Text.Equals(_picksterInformation[i, 0]))
                                {
                                    TextBox _ballType = (Controls.Find("txtBoxBallType" + num.ToString(), true)[0] as TextBox);
                                    if (_ballType.Text.Equals("파워"))
                                    {
                                        TextBox PR = (Controls.Find("txtBoxPR0" + num.ToString(), true)[0] as TextBox);
                                        TextBox PS = (Controls.Find("txtBoxPS0" + num.ToString(), true)[0] as TextBox);
                                        TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                                        String strPick = _picksterInformation[i, 4];
                                        if (remainingTime < 160)
                                        {
                                            if (strPick.Contains("P"))
                                            {
                                                pick.ForeColor = Color.DarkGray;
                                                pick.Text = "통과"; // 파볼픽                                              
                                            }
                                            else
                                            {
                                                if (strPick.Contains("홀"))
                                                {
                                                    pick.Text = "홀"; // 파볼픽
                                                } else if (strPick.Contains("짝"))
                                                {
                                                    pick.Text = "짝"; // 파볼픽
                                                } else if (strPick.Contains("언더"))
                                                {
                                                    pick.Text = "언더"; // 파볼픽
                                                } else if (strPick.Contains("오버"))
                                                {
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
                                            String _record = _picksterInformation[i, 2];
                                            String _streak = _picksterInformation[i, 3];
                                                String _newRecord = _record;
                                                int win = 100 - (int.Parse(_record.Replace("승", "")));
                                                _newRecord += win + "패";
                                                PR.Text = _newRecord; // 파워볼 전적
                                                PS.Text = _streak; // 파워볼 연승
                                        }
                                        else
                                        {
                                            PR.Text = "---";
                                            PS.Text = "---"; // 파워볼 연승
                                        }
                                    }
                                    else if (_ballType.Text.Equals("일반"))
                                    {
                                        TextBox PR = (Controls.Find("txtBoxPR0" + num.ToString(), true)[0] as TextBox);
                                        TextBox PS = (Controls.Find("txtBoxPS0" + num.ToString(), true)[0] as TextBox);
                                        TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                                        String strPick = _picksterInformation[i, 17];
                                        if (remainingTime < 160)
                                        {
                                            if (strPick.Contains("P"))
                                            {
                                                pick.Text = "통과"; // 파볼픽
                                                pick.ForeColor = Color.DarkGray;
                                            }
                                            else
                                            {
                                                pick.Text = strPick; // 파볼픽
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
                                            String _record = _picksterInformation[i, 15];
                                            String _streak = _picksterInformation[i, 16];
                                            if (outcomeMarkChechBox.Checked)
                                            {
                                                String _newRecord = _record;
                                                int win = 100 - (int.Parse(_record.Replace("승", "")));
                                                _newRecord += win + "패";
                                                PR.Text = _newRecord; // 파워볼 전적
                                                PS.Text = _streak; // 파워볼 연승
                                            }
                                            else
                                            {
                                                PR.Text = _record; // 파워볼 전적
                                                PS.Text = _streak; // 파워볼 연승
                                            }
                                        }
                                        else
                                        {
                                            PR.Text = "---";
                                            PS.Text = "---"; // 파워볼 연승
                                        }
                                    }
                                }
                            }
                            else
                            {
                                TextBox _pickster = (Controls.Find("txtBoxPickster" + num.ToString(), true)[0] as TextBox);
                                if (_pickster.Text.Equals(_picksterInformation[i, 0]))
                                {
                                    TextBox _ballType = (Controls.Find("txtBoxBallType" + num.ToString(), true)[0] as TextBox);
                                    if (_ballType.Text.Equals("파워"))
                                    {

                                        TextBox PR = (Controls.Find("txtBoxPR" + num.ToString(), true)[0] as TextBox);
                                        TextBox PS = (Controls.Find("txtBoxPS" + num.ToString(), true)[0] as TextBox);
                                        TextBox pick = (Controls.Find("BoxPick" + num.ToString(), true)[0] as TextBox);
                                        String strPick = _picksterInformation[i, 4];
                                        if (remainingTime < 160)
                                        {
                                            if (strPick.Contains("P"))
                                            {
                                                pick.Text = "통과"; // 파볼픽
                                                pick.ForeColor = Color.DarkGray;
                                            }
                                            else
                                            {
                                                if (strPick.Contains("홀"))
                                                {
                                                    pick.Text = "홀"; // 파볼픽
                                                }
                                                else if (strPick.Contains("짝"))
                                                {
                                                    pick.Text = "짝"; // 파볼픽
                                                }
                                                else if (strPick.Contains("언더"))
                                                {
                                                    pick.Text = "언더"; // 파볼픽
                                                }
                                                else if (strPick.Contains("오버"))
                                                {
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
                                            String _record = _picksterInformation[i, 2];
                                            String _streak = _picksterInformation[i, 3];
                                                String _newRecord = _record;
                                                int win = 100 - (int.Parse(_record.Replace("승", "")));
                                                _newRecord += win + "패";
                                                PR.Text = _newRecord; // 파워볼 전적
                                                PS.Text = _streak; // 파워볼 연승
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
                                        String strPick = _picksterInformation[i, 17];
                                        if (remainingTime < 160)
                                        {
                                            if (strPick.Contains("P"))
                                            {
                                                pick.Text = "통과"; // 파볼픽
                                                pick.ForeColor = Color.DarkGray;
                                            }
                                            else
                                            {
                                                pick.Text = strPick; // 파볼픽
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
                                            String _record = _picksterInformation[i, 15];
                                            String _streak = _picksterInformation[i, 16];
                                            if (outcomeMarkChechBox.Checked)
                                            {
                                                String _newRecord = _record;
                                                int win = 100 - (int.Parse(_record.Replace("승", "")));
                                                _newRecord += win + "패";
                                                PR.Text = _newRecord; // 파워볼 전적                                                
                                            }
                                            else
                                            {
                                                PR.Text = _record; // 파워볼 전적                                                
                                            }
                                            PS.Text = _streak; // 파워볼 연승
                                            PS.Text = _streak; // 파워볼 연승
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


                        item = new ListViewItem(_picksterInformation[i, 0]);
                        textLog.Text += "[" + i + "]" + _picksterInformation[i, 0] + Environment.NewLine;
                        item.UseItemStyleForSubItems = false;
                        for (int k2 = 1; k2 < 14; k2++)
                        {
                            if (k2 == 4) // 픽
                            {
                                if (remainingTime < 160)
                                {
                                    if (_picksterInformation[i, k2].Contains("P"))
                                    {
                                        item.SubItems.Add("통");
                                        item.SubItems[k2].ForeColor = Color.LightGray;
                                    }
                                    else
                                    {
                                        if (_picksterInformation[i, 4].Contains("홀")){

                                            item.SubItems.Add("홀");
                                        }
                                        if (_picksterInformation[i, 4].Contains("짝"))
                                        {

                                            item.SubItems.Add("짝");
                                        }
                                        if (_picksterInformation[i, 4].Contains("언더"))
                                        {

                                            item.SubItems.Add("언더");
                                        }
                                        if (_picksterInformation[i, 4].Contains("오버"))
                                        {

                                            item.SubItems.Add("오버");
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
                                        if (_a.Contains("홀"))
                                        {
                                            _a = "홀";
                                        }
                                        if (_a.Contains("짝"))
                                        {
                                            _a = "짝";
                                        }
                                        if (_a.Contains("언더"))
                                        {
                                            _a = "언";
                                        }
                                        else if (_a.Contains("오버"))
                                        {
                                            _a = "오";
                                        }
                                        item.SubItems.Add(_a);
                                        item.SubItems[k2].ForeColor = Color.OrangeRed;
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
                                        String _a = _picksterInformation[i, k2].Replace("X", "");
                                        if (_a.Contains("홀"))
                                        {
                                            _a = "홀";
                                        }
                                        if (_a.Contains("짝"))
                                        {
                                            _a = "짝";
                                        }
                                        if (_a.Contains("언더"))
                                        {
                                            _a = "언";
                                        }
                                        else if (_a.Contains("오버"))
                                        {
                                            _a = "오";
                                        }
                                        item.SubItems.Add(_a);
                                        item.SubItems[k2].ForeColor = Color.BlueViolet;
                                    }
                                }
                                else if (_picksterInformation[i, k2].Contains("P"))
                                {
                                    item.SubItems.Add("통");
                                    item.SubItems[k2].ForeColor = Color.LightGray;
                                }
                            }
                            else if (k2 == 3) // 연승 표기란
                            {
                                if (_picksterInformation[i, 4].Contains("P"))
                                {
                                    item.SubItems.Add("---");
                                }
                                else
                                {
                                    item.SubItems.Add(_picksterInformation[i, k2]);
                                }
                            }
                            else if (k2 == 2 || k2 == 15) // 파워볼 100전 전적란
                            {
                                String _record = _picksterInformation[i, k2];
                                if (outcomeMarkChechBox.Checked)
                                {
                                    String _newRecord = _record;
                                    int win = 100 - (int.Parse(_record.Replace("승", "")));
                                    _newRecord += win + "패";
                                    item.SubItems.Add(_newRecord);
                                }
                                else
                                {
                                    item.SubItems.Add(_picksterInformation[i, k2]);
                                }


                            }
                            else
                            {

                                item.SubItems.Add(_picksterInformation[i, k2]);
                            }
                        }
                        listView1.Items.Add(item);

                        // 엔트리는 파워볼만 하기 때문에 일반볼은 추가 하지 않는다.
                        /*
                        item = new ListViewItem(_picksterInformation[i, 0]);
                        item.UseItemStyleForSubItems = false;
                        for (int k3 = 14; k3 <= 26; k3++)
                        {
                                item.SubItems.Add(_picksterInformation[i, k3]);
                                if (_picksterInformation[i, k3].Equals("당"))
                                {
                                    item.SubItems[k3 - 13].ForeColor = Color.OrangeRed;
                                }
                                else if (_picksterInformation[i, k3].Equals("미"))
                                {
                                    item.SubItems[k3 - 13].ForeColor = Color.BlueViolet;
                                }
                        }
                        listView1.Items.Add(item);
                        */
                    }
                    else
                    {
                        break;
                    }
                }
                SetAlternatingRowColors(listView1, Color.LightGray, Color.White);
                txtLogAdd("총 [" + k + "] 명의 픽스터 정보를 새롭게 불러왔습니다.", Color.MediumVioletRed);
            }
            catch (Exception _ex)
            {
                textBox2.Text += _ex.ToString();
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

        // 픽스터 정보 배열
        string[,] _picksterInformation = new string[200, 27];

        int remainingTime = 0;

        // 남은 시간 타이머
        System.Timers.Timer timer = new System.Timers.Timer();

        void start()
        {
            try
            {
                _isStart = true;
                _bettingClosed = false;
                txtBettingStatus.Text = "자동 배팅이 진행 중입니다.\r\n주의해 주십시요.";
                txtLogAdd("자동 배팅이 시작되었습니다.", Color.MediumVioletRed);
                lblStartTime.Text = DateTime.Now.ToString("HH:mm:ss"); // 시작시간
                getPowerballInformation();
                txtLogAdd("완료", Color.MediumVioletRed);
                txtLogAdd("[" + allinning + "]회차가 진행 중입니다", Color.MediumVioletRed);
                txtNotice.Text = "알림 : " + allinning + "회 파워볼 배팅이 진행 중입니다.";

                if (UtilModel._userSite.Equals("rdw"))
                {
                    JObject jo = JObject.Parse(JGet("http://api.rdwball.com/userinfo/" + UtilModel._apikey));

                    var resultUserInforA = jo.SelectToken("A").ToString();
                    if (resultUserInforA.Equals("0"))
                    {
                        startMoney = int.Parse(jo.SelectToken("B").ToString());
                        lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                    }

                    jo = JObject.Parse(JGet("http://api.rdwball.com/gamestatus/" + UtilModel._apikey + "/4"));

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
                for (int _find = 1; _find <= 40; _find++)
                {
                    if (_find < 10)
                    {
                        TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                        TextBox _betMoney = (Controls.Find("txtBoxPBM0" + _find.ToString(), true)[0] as TextBox);
                        _level.ForeColor = Color.Black;
                        _level.BackColor = Color.White;
                        _level.Text = "1";
                        _betMoney.Text = txtBtMoneySettingL1.Text;
                    }
                    else
                    {
                        TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                        TextBox _betMoney = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                        _level.ForeColor = Color.Black;
                        _level.BackColor = Color.White;
                        _level.Text = "1";
                        _betMoney.Text = txtBtMoneySettingL1.Text;
                    }
                }
                setTimeRemaining(remainingTime);
                if (!_patternMode)
                {
                    pickPickster();
                }
                if (autoRefillCheckBox.Checked)
                {
                    autoRefill();
                    txtLogAdd("자동 채워주기가 활성화 상태입니다.", Color.MediumVioletRed);
                }
                powerballResult();
                timer.Start();

                logger.Info("[" + allinning + "] 배팅이 시작되었습니다.");
            }
            catch (Exception _ex)
            {
                textBox2.Text += _ex.ToString();
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
                textBox2.Text += _ex.ToString();
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

            btnBettingStart.Text = "배팅 시작";
            btnBettingStart.BackColor = Color.White;
            btnBettingStart.ForeColor = Color.Black;

            txtBettingStatus.Text = "자동 배팅이 종료되었습니다.\r\n그만하실 경우 완전 종료하십시요.";
            txtLogAdd("자동 배팅이 종료되었습니다.", Color.MediumVioletRed);
            timer.Stop();
            _isStart = false;
            logger.Info("배팅이 종료되었습니다.");
        }

        // 배팅 금액 설정 부분 한글 표기화
        public string NumToString(Int64 x)
        {
            string HAmt = ""; string EAmt = "";
            Int64 KLen = 0; Int64 ELen = 0;
            int j = 0; int k = 0;
            string W = ""; string Y = "";

            try
            {

                EAmt = x.ToString();
                ELen = EAmt.Length;
                k = 0;
                for (j = 0; j < ELen; j++)
                {
                    KLen = Convert.ToInt64(EAmt.Substring(j, 1));
                    switch (KLen)
                    {
                        case 1:
                            W = "일";
                            break;
                        case 2:
                            W = "이";
                            break;
                        case 3:
                            W = "삼";
                            break;
                        case 4:
                            W = "사";
                            break;
                        case 5:
                            W = "오";
                            break;
                        case 6:
                            W = "육";
                            break;
                        case 7:
                            W = "칠";
                            break;
                        case 8:
                            W = "팔";
                            break;
                        case 9:
                            W = "구";
                            break;
                        case 0:
                            W = "영";
                            break;
                    }
                    switch (ELen)
                    {
                        case 10:
                            Y = "십억천백십만천백십원";
                            break;
                        case 9:
                            Y = "억천백십만천백십원";
                            break;
                        case 8:
                            Y = "천백십만천백십원";
                            break;
                        case 7:
                            Y = "백십만천백십원";
                            break;
                        case 6:
                            Y = "십만천백십원";
                            break;
                        case 5:
                            Y = "만천백십원";
                            break;
                        case 4:
                            Y = "천백십원";
                            break;
                        case 3:
                            Y = "백십원";
                            break;
                        case 2:
                            Y = "십원";
                            break;
                        case 1:
                            Y = "원";
                            break;
                    }
                    if (W != "영")
                    {
                        HAmt = HAmt + (W + Y.Substring(k, 1));
                    }
                    if (Y.Substring(k, 1) == "억")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "억";
                        }
                    }
                    else if (Y.Substring(k, 1) == "만")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "만";
                        }
                    }
                    else if (Y.Substring(k, 1) == "원")
                    {
                        if (W == "영")
                        {
                            HAmt = HAmt + "원";
                        }
                    }

                    k = k + 1;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return HAmt;

        }

        private void txtBtMoneySettingL1_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL1, lblChangeToHangul1);
        }

        private void txtBtMoneySettingL2_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL2, lblChangeToHangul2);
        }

        private void txtBtMoneySettingL3_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL3, lblChangeToHangul3);
        }

        private void txtBtMoneySettingL4_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL4, lblChangeToHangul4);
        }

        private void txtBtMoneySettingL5_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL5, lblChangeToHangul5);
        }

        private void txtBtMoneySettingL6_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL6, lblChangeToHangul6);
        }

        private void txtBtMoneySettingL7_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL7, lblChangeToHangul7);
        }

        private void txtBtMoneySettingL8_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL8, lblChangeToHangul8);
        }

        private void txtBtMoneySettingL9_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL9, lblChangeToHangul9);
        }

        private void txtBtMoneySettingL10_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL10, lblChangeToHangul10);
        }

        private void txtBtMoneySettingL11_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL11, lblChangeToHangul11);
        }

        private void txtBtMoneySettingL12_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL12, lblChangeToHangul12);
        }

        private void txtBtMoneySettingL13_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL13, lblChangeToHangul13);
        }

        private void txtBtMoneySettingL14_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL14, lblChangeToHangul14);
        }

        private void txtBtMoneySettingL15_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL15, lblChangeToHangul15);
        }
        private void txtBtMoneySettingL16_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL16, lblChangeToHangul16);
        }

        private void txtBtMoneySettingL17_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL17, lblChangeToHangul17);
        }

        private void txtBtMoneySettingL18_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL18, lblChangeToHangul18);
        }

        private void txtBtMoneySettingL19_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL19, lblChangeToHangul19);
        }

        private void txtBtMoneySettingL20_TextChanged(object sender, EventArgs e)
        {
            textChangeMoneySetting(txtBtMoneySettingL20, lblChangeToHangul20);
        }

        private void textChangeMoneySetting(TextBox txtBox, Label lbl)
        {
            try
            {

                int numTxtBox = Int32.Parse(txtBox.Text);
                lbl.Text = NumToString(numTxtBox);
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
                txtLog.AppendText(getTime() + str + "\r\n");
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
                logger.Info(str);
            }
            catch (FormatException formatexception)
            {
                Console.WriteLine(formatexception);
            }
        }

        // 시간 표기법
        private String getDateTime()
        {
            return "[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] ";
        }

        private String getTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
        }

        private String getTimeHour()
        {
            return DateTime.Now.ToString("HH");
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

        private String GetHttpPOST(string reqstring, string url, string encode, ref int errcode)
        {
            String retValue = "";
            /*
            if (url.IndexOf("https://") >= 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            }
            */
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.AllowAutoRedirect = true;
                Stream requestStream = null;
                byte[] sendData = null;
                if (reqstring != null)
                {
                    if (String.IsNullOrEmpty(encode) || encode == "UTF-8")
                    {
                        sendData = UTF8Encoding.UTF8.GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    }
                    else if (encode == "EUC-KR")
                    {
                        sendData = Encoding.GetEncoding("EUC-KR").GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=EUC-KR";
                    }
                    else
                    {
                        sendData = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(reqstring);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=ks_c_5601-1987";
                    }
                    httpWebRequest.ContentLength = sendData.Length;
                    requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(sendData, 0, sendData.Length);
                }
                else
                {
                    httpWebRequest.ContentLength = 0;
                    requestStream = httpWebRequest.GetRequestStream();
                }
                requestStream.Flush();
                requestStream.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding(encode));
                retValue = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    errcode = (int)((HttpWebResponse)ex.Response).StatusCode;
                    retValue = ex.Message;
                }
                else
                {
                    retValue = ex.Message;
                    errcode = -1;
                }
            }
            return retValue;
        }

        /*
    url = http://10.0.0.1   웹서버 주소                    
    lblUrl.Text = { "key1":"value1", "key2":"value2", "key3":"value3" }  JSON 문자열
*/
        // JSON POST (정보 전달)
        private String JPOST(String _url, String _param)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var result = "";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(_param);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                Console.WriteLine(result.ToString());
            }
            return result;
        }

        /*
            lblGetUrl.Text = http://10.0.0.1/check?key1=     고정값
            txtGetID.Text = value1                           지정값
        */
        // JSON GET (정보 확인)
        private String JGet(String _url)
        {
            String _stringMessage = "";
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                // 결과값을 Json 문자열로 받아온다.
                _stringMessage = client.DownloadString(_url);
            }
            return _stringMessage;
        }
        private HttpWebRequest _webRequest;
        private HttpWebResponse _webResponse;
        public static CookieContainer _cookieContainer;
        public static CookieCollection _cookieCollection;
        private Stream _responseStream;
        private StreamReader _streamReader;
        //private StreamWriter g;
        private string loadingPageHtml;
        public string loadingPage(string _url, string _referer, string _method, string _params, Encoding _encoding)
        {
            try
            {
                _webRequest = (HttpWebRequest)WebRequest.Create(_url);
                _webRequest.Credentials = CredentialCache.DefaultCredentials;
                _webRequest.Method = _method;
                //_webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                _webRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
                _webRequest.Accept = "text/html, application/xhtml+xml, */*";
                _webRequest.KeepAlive = true;
                _webRequest.ContentType = "application/x-www-form-urlencoded";
                _webRequest.ProtocolVersion = HttpVersion.Version10;

                _webRequest.AllowAutoRedirect = true;

                if (_referer != null)
                {
                    _webRequest.Referer = _referer;
                }
                if (_params != null)
                {
                    byte[] bytes = Encoding.Default.GetBytes(_params);
                    _webRequest.ContentLength = bytes.Length;
                    Stream requestStream = _webRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                _responseStream = _webResponse.GetResponseStream();
                _streamReader = new StreamReader(_responseStream, _encoding, true);
                loadingPageHtml = _streamReader.ReadToEnd();
            }
            catch (WebException exception)
            {
                return exception.Message;
            }
            catch (Exception exception2)
            {
                return exception2.Message;
            }
            finally
            {
                if (_responseStream != null)
                {
                    _responseStream.Close();
                }
                if (_streamReader != null)
                {
                    _streamReader.Close();
                }
            }
            return loadingPageHtml;
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
            _dragDrop(data, txtBoxPickster01, txtBoxBallType1, BoxPick1, txtBoxPR01, txtBoxPS01);
        }

        private void tableLayoutPanel2_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            ListViewItem data = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
            _dragDrop(data, txtBoxPickster02, txtBoxBallType2, BoxPick2, txtBoxPR02, txtBoxPS02);
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
            _dragDrop(data, txtBoxPickster03, txtBoxBallType3, BoxPick3, txtBoxPR03, txtBoxPS03);
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
            _dragDrop(data, txtBoxPickster04, txtBoxBallType4, BoxPick4, txtBoxPR04, txtBoxPS04);
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
            _dragDrop(data, txtBoxPickster05, txtBoxBallType5, BoxPick5, txtBoxPR05, txtBoxPS05);
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
            _dragDrop(data, txtBoxPickster06, txtBoxBallType6, BoxPick6, txtBoxPR06, txtBoxPS06);
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
            _dragDrop(data, txtBoxPickster07, txtBoxBallType7, BoxPick7, txtBoxPR07, txtBoxPS07);
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
            _dragDrop(data, txtBoxPickster08, txtBoxBallType8, BoxPick8, txtBoxPR08, txtBoxPS08);
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
            _dragDrop(data, txtBoxPickster09, txtBoxBallType9, BoxPick9, txtBoxPR09, txtBoxPS09);
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
            _dragDrop(data, txtBoxPickster10, txtBoxBallType10, BoxPick9, txtBoxPR10, txtBoxPS10);
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
            _dragDrop(data, txtBoxPickster11, txtBoxBallType11, BoxPick11, txtBoxPR11, txtBoxPS11);
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
            _dragDrop(data, txtBoxPickster12, txtBoxBallType12, BoxPick12, txtBoxPR12, txtBoxPS12);
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
            _dragDrop(data, txtBoxPickster13, txtBoxBallType13, BoxPick13, txtBoxPR13, txtBoxPS13);
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
            _dragDrop(data, txtBoxPickster14, txtBoxBallType14, BoxPick14, txtBoxPR14, txtBoxPS14);
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
            _dragDrop(data, txtBoxPickster15, txtBoxBallType15, BoxPick15, txtBoxPR15, txtBoxPS15);
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
            _dragDrop(data, txtBoxPickster16, txtBoxBallType16, BoxPick16, txtBoxPR16, txtBoxPS16);
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
            _dragDrop(data, txtBoxPickster17, txtBoxBallType17, BoxPick17, txtBoxPR17, txtBoxPS17);
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
            _dragDrop(data, txtBoxPickster18, txtBoxBallType18, BoxPick18, txtBoxPR18, txtBoxPS18);
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

            _dragDrop(data, txtBoxPickster19, txtBoxBallType19, BoxPick19, txtBoxPR19, txtBoxPS19);
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
            _dragDrop(data, txtBoxPickster20, txtBoxBallType20, BoxPick20, txtBoxPR20, txtBoxPS20);
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
            _dragDrop(data, txtBoxPickster21, txtBoxBallType21, BoxPick21, txtBoxPR21, txtBoxPS21);
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
            _dragDrop(data, txtBoxPickster22, txtBoxBallType22, BoxPick22, txtBoxPR22, txtBoxPS22);
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
            _dragDrop(data, txtBoxPickster23, txtBoxBallType23, BoxPick23, txtBoxPR23, txtBoxPS23);
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
            _dragDrop(data, txtBoxPickster24, txtBoxBallType24, BoxPick24, txtBoxPR24, txtBoxPS24);
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
            _dragDrop(data, txtBoxPickster25, txtBoxBallType25, BoxPick25, txtBoxPR25, txtBoxPS25);
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
            _dragDrop(data, txtBoxPickster26, txtBoxBallType26, BoxPick26, txtBoxPR26, txtBoxPS26);
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
            _dragDrop(data, txtBoxPickster27, txtBoxBallType27, BoxPick27, txtBoxPR27, txtBoxPS27);
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
            _dragDrop(data, txtBoxPickster28, txtBoxBallType28, BoxPick28, txtBoxPR28, txtBoxPS28);
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
            _dragDrop(data, txtBoxPickster29, txtBoxBallType29, BoxPick29, txtBoxPR29, txtBoxPS29);
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
            _dragDrop(data, txtBoxPickster30, txtBoxBallType30, BoxPick30, txtBoxPR30, txtBoxPS30);
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

            _dragDrop(data, txtBoxPickster31, txtBoxBallType31, BoxPick31, txtBoxPR31, txtBoxPS31);
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
            _dragDrop(data, txtBoxPickster32, txtBoxBallType32, BoxPick32, txtBoxPR32, txtBoxPS32);
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

            _dragDrop(data, txtBoxPickster33, txtBoxBallType33, BoxPick33, txtBoxPR33, txtBoxPS33);
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

            _dragDrop(data, txtBoxPickster34, txtBoxBallType34, BoxPick34, txtBoxPR34, txtBoxPS34);
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
            _dragDrop(data, txtBoxPickster35, txtBoxBallType35, BoxPick35, txtBoxPR35, txtBoxPS35);
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

            _dragDrop(data, txtBoxPickster36, txtBoxBallType36, BoxPick36, txtBoxPR36, txtBoxPS36);
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

            _dragDrop(data, txtBoxPickster37, txtBoxBallType37, BoxPick37, txtBoxPR37, txtBoxPS37);
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

            _dragDrop(data, txtBoxPickster38, txtBoxBallType38, BoxPick38, txtBoxPR38, txtBoxPS38);
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
            _dragDrop(data, txtBoxPickster39, txtBoxBallType39, BoxPick39, txtBoxPR39, txtBoxPS39);
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
            _dragDrop(data, txtBoxPickster40, txtBoxBallType40, BoxPick40, txtBoxPR40, txtBoxPS40);
        }
        private void tableLayoutPanel40_dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void _dragDrop(ListViewItem _data, TextBox pickster, TextBox type, TextBox pick, TextBox PR, TextBox PS)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 픽스터를 바꿀 수 없습니다.");
                return;
            }
            if (!samePersonCheckBox.Checked)
            {
                Boolean _bool = false;
                int findNum = 0;
                for (int _find = 1; _find <= 40; _find++)
                {
                    if (_find < 10)
                    {
                        TextBox _pickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                        if (_pickster.Text.Equals(_data.Text))
                        {
                            findNum = _find;
                            _bool = true;
                            break;
                        }
                    }
                    else
                    {
                        TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                        if (_pickster.Text.Equals(_data.Text))
                        {
                            findNum = _find;
                            _bool = true;
                            break;
                        }
                    }
                }
                if (_bool)
                {
                    MessageBox.Show("이미 존재하는 픽스터입니다. [" + findNum + "]번째 칸에 있습니다.");
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
                            type.Text = _data.SubItems[1].Text; // 파볼픽
                            if (type.Text.Equals("파워"))
                            {
                                if (_picksterInformation[k, 4].Contains("짝"))
                                {
                                    pick.Text = "짝";
                                }
                                if (_picksterInformation[k, 4].Contains("홀"))
                                {
                                    pick.Text = "홀";
                                }
                                if (_picksterInformation[k, 4].Contains("언더"))
                                {
                                    pick.Text = "언더";
                                }
                                if (_picksterInformation[k, 4].Contains("오버"))
                                {
                                    pick.Text = "오버";
                                }
                                PR.Text = _picksterInformation[k, 2]; // 파워볼 전적
                                PS.Text = _picksterInformation[k, 3]; // 파워볼 연승

                            }
                            else if (type.Text.Equals("일반"))
                            {
                                if (_picksterInformation[k, 17].Contains("짝"))
                                {
                                    pick.Text = "짝";
                                }
                                if (_picksterInformation[k, 17].Contains("홀"))
                                {
                                    pick.Text = "홀";
                                }
                                if (_picksterInformation[k, 17].Contains("언더"))
                                {
                                    pick.Text = "언더";
                                }
                                if (_picksterInformation[k, 17].Contains("오버"))
                                {
                                    pick.Text = "오버";
                                }
                                PR.Text = _picksterInformation[k, 15]; // 파워볼 전적
                                PS.Text = _picksterInformation[k, 16]; // 파워볼 연승
                            }
                            txtLogAdd(_picksterInformation[k, 0] + "님을 배팅에 참여시켰습니다.", Color.MediumVioletRed);
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
                    textBox2.Text += _ex + Environment.NewLine;
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

        void init280()
        {
            _picksterInformation = new string[200, 27];
            refreshBettingInformation = true;
            _automodeChange = false;
            getPowerballinningAndPowerBallResult = false;
            getPowerBallResultProcessing = false;
            getPicksterAndRemainingtimeReload = false;
            btnPicksterChoiceOddNum.Text = "0";
            btnPicksterChoiceEvenNum.Text = "0";
            btnPicksterChoiceOverNum.Text = "0";
            btnPicksterChoiceUnderNum.Text = "0";

            btnPicksterChoiceOddPercent.Text = "0";
            btnPicksterChoiceEvenPercent.Text = "0";
            btnPicksterChoiceOverPercent.Text = "0";
            btnPicksterChoiceUnderPercent.Text = "0";
        }
        void init275()
        {
            try
            {
                getPowerballinningAndPowerBallResult = true;
                getPowerballInning();
                resultBefore_allbettingmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney;
                powerballResult();
                txtLogAdd("파워볼 결과가 나왔습니다.", Color.MediumVioletRed);
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
                int gain = 0;
                getPowerBallResultProcessing = true;
                if (bettingStatus)
                {
                    resultAfter_allbettingmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney;
                    if (resultBefore_allbettingmoney > 0)
                    {
                        powerBallResultProcessing();
                        txtLogAdd("현재 결과 처리 중입니다.", Color.MediumVioletRed);
                        txtNotice.Text = "알림 : 결과 처리 중입니다.";
                    }
                    else
                    {
                        txtLogAdd("배팅된 금액이 없어 결과 처리를 하지 않습니다.", Color.MediumVioletRed);
                        txtNotice.Text = "알림 : 배팅된 금액이 없어 결과 처리를 하지 않습니다.";
                    }

                    lblPSingle.Text = "0 원";
                    lblPPair.Text = "0 원";
                    lblPUnder.Text = "0 원";
                    lblPOver.Text = "0 원";
                    lblNSingle.Text = "0 원";
                    lblNPair.Text = "0 원";
                    lblNUnder.Text = "0 원";
                    lblNOver.Text = "0 원";

                    NowMoney += pOverMoney;
                    NowMoney += pUnderMoney;
                    NowMoney += pOddMoney;
                    NowMoney += pEvenMoney;
                    NowMoney += nOverMoney;
                    NowMoney += nUnderMoney;
                    NowMoney += nOddMoney;
                    NowMoney += nEvenMoney;

                    int winmoney = pOverMoney + pUnderMoney + pOddMoney + pEvenMoney + nOverMoney + nUnderMoney + nOddMoney + nEvenMoney;
                    lblTxtNowMoney.Text = UtilModel.StringFormatChanged(NowMoney) + " 원";

                    gain = NowMoney - startMoney;
                    if (gain > 0)
                    {
                        lblTxtNowGain.ForeColor = Color.DarkRed;
                        lblTxtNowGain.Text = "▲ " + gain + " 원";
                    }
                    else if (gain < 0)
                    {
                        lblTxtNowGain.ForeColor = Color.DarkBlue;
                        lblTxtNowGain.Text = "▼ " + gain + " 원";
                    }

                    try
                    {
                        if (resultAfter_allbettingmoney > 0)
                        {
                            string url = UtilModel.updateuserstatus;                                      // 통신할 URL
                            string msg = string.Format("update=&userid={0}&nickname={1}&ownmoney={2}&allinning={3}&allbettingmoney={4}&winmoney={5}&gain={6}&token={7}&startmoney={8}&starttime={9}",
                                UtilModel._id, nickName, NowMoney, allinning, resultAfter_allbettingmoney, winmoney, gain, UtilModel._timetoken, startMoney, lblStartTime.Text); // 전송할 Parameter
                            string encodeStr = "UTF-8";                                          // 인코딩 방식
                            int errorcode = 0;                                                     // 에러 전달받을 값
                                                                                                   //string returnVal = "";
                            GetHttpPOST(msg, url, encodeStr, ref errorcode);
                            //MessageBox.Show(returnVal);
                        }
                    }
                    catch (Exception _ex)
                    {
                        textBox2.Text += _ex.ToString();
                    }

                    item = new ListViewItem(allinning + "회");
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(getDateTime());
                    item.SubItems.Add(UtilModel.StringFormatChanged(NowMoney) + " 원");
                    if (gain > 0)
                    {
                        item.SubItems.Add("▲ " + UtilModel.StringFormatChanged(gain) + " 원");
                    }
                    else
                    {
                        item.SubItems.Add("▲ " + UtilModel.StringFormatChanged(gain) + " 원");
                    }
                    item.SubItems.Add(UtilModel.StringFormatChanged(pOddMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(pEvenMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(pOverMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(pUnderMoney) + " 원");

                    item.SubItems.Add(UtilModel.StringFormatChanged(nOddMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(nEvenMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(nOverMoney) + " 원");
                    item.SubItems.Add(UtilModel.StringFormatChanged(nUnderMoney) + " 원");

                    listView2.Items.Add(item);
                }
                pOverMoney = 0;
                pUnderMoney = 0;
                pOddMoney = 0;
                pEvenMoney = 0;
                nOverMoney = 0;
                nUnderMoney = 0;
                nOddMoney = 0;
                nEvenMoney = 0;

                clearBetInfor();

                if (gain > int.Parse(OverProfit.Text))
                {
                    timer.Stop();
                    _isStart = false;
                    txtLogAdd("축하드립니다. 수익 설정 금액을 초과하였습니다.", Color.MediumVioletRed);
                    txtNotice.Text = "알림 : 축하드립니다. 수익 설정 금액을 초과하였습니다.";
                    MessageBox.Show("[" + DateTime.Now.ToString("hh:MM:ss") + "] 수익 설정 금액 초과로 종료되었습니다.");
                    System.Media.SystemSounds.Beep.Play();
                    System.Media.SystemSounds.Asterisk.Play();
                    System.Media.SystemSounds.Exclamation.Play();
                    System.Media.SystemSounds.Question.Play();
                    System.Media.SystemSounds.Hand.Play();
                    return;
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

                txtLogAdd("파워볼 배팅이 진행 중입니다.", Color.MediumVioletRed);
                txtNotice.Text = "알림 : 파워볼 배팅이 진행 중입니다.";

                if (_winstopmode) // 당첨 정지 모드
                {
                    for (int _find = 1; _find <= 40; _find++)
                    {
                        if (_find < 10)
                        {
                            TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                            Button _pr = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                            if (int.Parse(_level.Text) == 1)
                            {
                                _pr.Text = "배팅정지";
                                _pr.ForeColor = Color.White;
                                _pr.BackColor = Color.DarkSlateGray;
                            }
                        }
                        else
                        {
                            TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
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
            }
            catch (Exception _ex)
            {
                logger.Error(_ex.ToString());
            }
        }
        // 시간마다 해야할 작업 정리 오류가 나지 않게 더 세세하게 정리해야한다.
        private void remainingTimer_Tick()
        {
            if (remainingTime > 1)
            {
                setTimeRemaining(remainingTime--);
                if (remainingTime == 280)
                {
                    getPowerballInning();
                    init280();

                }
                if (remainingTime <= 275 && remainingTime >= 273 && !getPowerballinningAndPowerBallResult)
                {
                    init275();
                }

                if (remainingTime == 272 && !getPowerBallResultProcessing)
                {
                    init272();
                }

                if (remainingTime == 270)
                {
                    init270();
                }
                if (remainingTime == 268 || remainingTime == 265 || remainingTime == 240 || remainingTime == 210 || remainingTime == 180 || remainingTime == 150 || remainingTime == 120 || remainingTime == 90 || remainingTime == 60)
                {
                    if (getPicksterAndRemainingtimeReload == false)
                    {
                        getPicksterAndRemainingtimeReload = true;
                        if (!_patternMode)
                        {
                            pickPickster();
                            getPowerballInformation();
                        }
                    }
                }

                if (remainingTime == 260 || remainingTime == 230 || remainingTime == 200 || remainingTime == 170 || remainingTime == 140 || remainingTime == 110 || remainingTime == 80)
                {
                    getPicksterAndRemainingtimeReload = false;
                }

                if (remainingTime == 85 && _automode && !_automodeChange)
                {

                    try
                    {
                        _automodeChange = true;
                        if (!autoRefill())
                        {
                            MessageBox.Show("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + allinning + " 회차는 엔트리 픽스터의 픽이 부족해 통과합니다.");
                        }
                        else
                        {
                            txtLogAdd("자동모드가 진행중입니다. 새롭게 40명을 교체하였습니다.", Color.Black);
                        }
                    }
                    catch (Exception _ex)
                    {
                        txtLogAdd(_ex.ToString(), Color.Black);
                    }
                }
                int picksterNumber = 0;
                int _automodelimit = 0;
                if (remainingTime == 60)
                {
                    txtNotice.Text = "알림 : 잠시 후 배팅이 마감 됩니다.";
                    if (!_automode || !_superioritymode)
                    {
                        String pP = "";
                        String nP = "";

                        for (int i = 0; i < 200; i++)
                        {
                            if (_picksterInformation[i, 0] != null)
                            {
                                // 파워픽 4, 일반픽 17
                                pP = _picksterInformation[i, 4];
                                nP = _picksterInformation[i, 17];
                                if (!pP.Contains("P"))
                                {
                                    picksterNumber++;
                                }
                            }
                        }

                        _automodelimit = int.Parse(automodelimit.Text);
                        if (picksterNumber < _automodelimit)
                        {
                            txtLogAdd("현재 픽스터의 숫자가 매우 부족합니다. 엔트리 문제로 제대로 읽어오지 못하였습니다.", Color.Black);
                        }
                    }
                }
                if (remainingTime == 58 && _superioritymode)
                {
                    try
                    {
                        String pP = "";
                        String nP = "";

                        for (int i = 0; i < 200; i++)
                        {
                            if (_picksterInformation[i, 0] != null)
                            {
                                // 파워픽 4, 일반픽 17
                                pP = _picksterInformation[i, 4];
                                nP = _picksterInformation[i, 17];
                                if (!pP.Contains("P"))
                                {
                                    picksterNumber++;
                                }
                            }
                        }

                        _automodelimit = int.Parse(automodelimit.Text);
                        if (picksterNumber < _automodelimit)
                        {
                            txtLogAdd("현재 픽스터의 숫자가 매우 부족합니다. 엔트리 문제로 제대로 읽어오지 못하였습니다.", Color.Black);
                        }

                        String oddeven = "통과";
                        String overunder = "통과";
                        txtLogAdd(picksterNumber + " 명 픽 완료. " + _automodelimit + " 최소 픽유저", Color.Black);
                        if (picksterNumber >= _automodelimit)
                        {
                            if (oddPercent > int.Parse(superiorModePercent.Text))
                            {
                                if (_powerballOdd > _powerballEven) // 홀이 더 크다면
                                {
                                    oddeven = "홀";
                                    txtLogAdd(oddPercent.ToString("F") + " %로 홀이 선택되었습니다", Color.Black);
                                }
                            }
                            else if (evenPercent > int.Parse(superiorModePercent.Text))
                            {
                                if (_powerballOdd < _powerballEven)
                                {
                                    oddeven = "짝";
                                    txtLogAdd(evenPercent.ToString("F") + " %로 짝이 선택되었습니다", Color.Black);
                                }
                            }

                            if (overPercent > int.Parse(superiorModePercent.Text))
                            {
                                if (_powerballOver > _powerballUnder) // 홀이 더 크다면
                                {
                                    overunder = "오버";
                                    txtLogAdd(overPercent.ToString("F") + " %로 오버가 선택되었습니다", Color.Black);
                                }
                            }
                            else if (underPercent > int.Parse(superiorModePercent.Text))
                            {
                                if (_powerballOver < _powerballUnder)
                                {
                                    overunder = "언더";
                                    txtLogAdd(underPercent.ToString("F") + " %로 언더가 선택되었습니다", Color.Black);
                                }
                            }
                            // _picksterInformation[k, 0]
                            for (int _find = 1; _find <= 40; _find++)
                            {
                                if (_find < 10)
                                {
                                    TextBox _pickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);

                                    TextBox PR = (Controls.Find("txtBoxPR0" + _find.ToString(), true)[0] as TextBox);
                                    TextBox PS = (Controls.Find("txtBoxPS0" + _find.ToString(), true)[0] as TextBox);
                                    _pickster.Text = "우세픽홀짝모드";
                                    _ballType.Text = "파워";
                                    _pick.Text = oddeven; // 홀짝
                                    _pick.ForeColor = Color.Black;
                                    PR.Text = "---";
                                    PR.ForeColor = Color.White;
                                    PS.Text = "---";
                                    PS.ForeColor = Color.White;
                                }
                                else
                                {
                                    if (_find <= 20)
                                    {
                                        TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                                        TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                                        TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                                        TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                                        TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                                        _pickster.Text = "우세픽홀짝모드";
                                        _ballType.Text = "파워";
                                        _pick.Text = oddeven; // 홀짝
                                        _pick.ForeColor = Color.Black;
                                        PR.Text = "---";
                                        PR.ForeColor = Color.White;
                                        PS.Text = "---";
                                        PS.ForeColor = Color.White;
                                    }
                                    else
                                    {
                                        TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                                        TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                                        TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                                        TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                                        TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                                        _pickster.Text = "우세픽언오버모드";
                                        _ballType.Text = "파워";
                                        _pick.Text = overunder; // 오버 언더
                                        _pick.ForeColor = Color.Black;
                                        PR.Text = "---";
                                        PR.ForeColor = Color.White;
                                        PS.Text = "---";
                                        PS.ForeColor = Color.White;
                                    }
                                }
                            }
                        }
                        else
                        {
                            txtLogAdd("우세픽 모드 통과되었습니다.", Color.MediumVioletRed);
                            txtNotice.Text = "알림 : 우세픽 모드 통과되었습니다.";
                        }
                    } catch(Exception _ex)
                    {
                        textBox2.Text += _ex.ToString();
                    }                                      
                }

                if (!_bettingClosed && remainingTime <= 53)
                {
                    _bettingClosed = true;
                    txtLogAdd(allinning + "회 배팅이 마감되었습니다.", Color.MediumVioletRed);
                    txtNotice.Text = "알림 : 배팅이 마감되었습니다. 등록 중입니다.";
                    bet();

                    Double gain = NowMoney - startMoney;
                    if (gain > 0)
                    {
                        lblTxtNowGain.ForeColor = Color.DarkRed;
                        lblTxtNowGain.Text = "▲ " + (NowMoney - startMoney) + " 원";
                    }
                    else if (gain < 0)
                    {
                        lblTxtNowGain.ForeColor = Color.DarkBlue;
                        lblTxtNowGain.Text = "▼ " + (NowMoney - startMoney) + " 원";
                    }
                }

                if (remainingTime == 30)
                {
                    txtLogAdd("잠시 후 파워볼 [" + allinning + "]회 결과가 나올 예정입니다.", Color.MediumVioletRed);
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
                    txtLogAdd("잠시 후 다음 회차가 진행될 예정입니다.", Color.MediumVioletRed);
                    txtNotice.Text = "알림 : 잠시 후 다음 회차가 진행될 예정입니다.";
                    _picksterInformation = new string[200, 27];
                    refreshBettingInformation = true;
                    _automodeChange = false;
                    getPowerballinningAndPowerBallResult = false;
                    getPowerBallResultProcessing = false;
                    btnPicksterChoiceOddNum.Text = "0";
                    btnPicksterChoiceEvenNum.Text = "0";
                    btnPicksterChoiceOverNum.Text = "0";
                    btnPicksterChoiceUnderNum.Text = "0";

                    btnPicksterChoiceOddPercent.Text = "0";
                    btnPicksterChoiceEvenPercent.Text = "0";
                    btnPicksterChoiceOverPercent.Text = "0";
                    btnPicksterChoiceUnderPercent.Text = "0";
                }
            }
        }

        private Boolean autoRefill()
        {
            String pP = "";
            String nP = "";

            int n = 1;
            for (int i = 0; i < 200; i++)
            {
                if (_picksterInformation[i, 0] != null)
                {
                    // 파워픽 4, 일반픽 17
                    pP = _picksterInformation[i, 4];
                    nP = _picksterInformation[i, 17];
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
                for (int i = 0; i < 200; i++)
                {
                    if (_picksterInformation[i, 0] != null)
                    {
                        // 파워픽 4, 일반픽 17
                        pP = _picksterInformation[i, 4];
                        //nP = _picksterInformation[i, 17];
                        if (!pP.Contains("P"))
                        {
                            arrayPickster[num++] = _picksterInformation[i, 0];
                        }
                    }
                }

                Random rand = new Random(); //랜덤선언 _picksterInformation[k, 0]
                int r = 0;
                for (int _find = 1; _find <= 40; _find++)
                {
                    r = rand.Next(0, (arrayPickster.Length - 1));
                    String strpickster = arrayPickster[r];

                    if (_find < 10)
                    {
                        if (strpickster != null)
                        {
                            TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _pickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                            _pickster.Text = strpickster;
                            _ballType.Text = "파워";
                        }
                    }
                    else
                    {
                        if (strpickster != null)
                        {
                            TextBox _ballType = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                            _pickster.Text = strpickster;
                            _ballType.Text = "파워";
                        }
                    }
                }
                return true;
            }
            else
            {
                txtLogAdd("[" + n + "] 픽을 한 픽스터가 " + _automodelimit + "명 이하라서 통과하였습니다.", Color.OrangeRed);
                return false;
            }
        }

        Boolean _soundBeep = false;
        // 결과를 참고하여 단계 및 배팅금 조정, 색깔 조정
        private void powerBallResultProcessing()
        {
            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    Button _btnPR = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    //btnPR02
                    if (_btnPR.Text.Equals("배팅진행"))
                    {
                        TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        if (!_pick.Text.Equals("P"))
                        {
                            // txtBoxPickster01
                            TextBox _pickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                            TextBox _betMoney = (Controls.Find("txtBoxPBM0" + _find.ToString(), true)[0] as TextBox);
                            TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                            Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                            _powerBallResultProcessing(_pickster, _ballType, _pick, _level, _betMoney, _follow);
                        }
                    }
                }
                else
                {
                    Button _btnPR = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    //btnPR02
                    if (_btnPR.Text.Equals("배팅진행"))
                    {
                        TextBox _betMoney = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                        if (int.Parse(_betMoney.Text) > 0)
                        {
                            TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                            TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                            TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                            TextBox _level = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                            Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                            _powerBallResultProcessing(_pickster, _ballType, _pick, _level, _betMoney, _follow);
                        }
                    }
                }
            }

            if (powerballSP.Equals("홀"))
            {
                pOddMoney = (int)(pOddMoney * 1.95);
                pEvenMoney = 0;
            }
            if (powerballSP.Equals("짝"))
            {
                pEvenMoney = (int)(pEvenMoney * 1.95);
                pOddMoney = 0;
            }
            if (powerballUO.Equals("오버"))
            {
                pOverMoney = (int)(pOverMoney * 1.95);
                pUnderMoney = 0;
            }
            if (powerballUO.Equals("언더"))
            {
                pUnderMoney = (int)(pUnderMoney * 1.95);
                pOverMoney = 0;
            }
            if (_soundBeep)
            {
                Beep(512, 300); // 도 0.3초
            }
            _soundBeep = false;
        }

        // 레벨별 글자색과 배경색 설정
        private void _levelColorSetting(TextBox _level, TextBox _betMoney)
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
                _betMoney.Text = _MoneySettingLevel.Text;
                _level.Text = "1";
                _level.ForeColor = Color.Black;
                _level.BackColor = Color.White;
            }
            else
            {
                _level.Text = level.ToString();
                _betMoney.Text = _MoneySettingLevel.Text;
            }
        }

        // 결과값이 나왔을 경우 처리하는 부분.
        private void _powerBallResultProcessing(TextBox _pickster, TextBox _ballType, TextBox _pick, TextBox _level, TextBox _betMoney, Button _follow)
        {
            if (_ballType.Text.Equals("파워"))
            {
                // 따라가기 배팅시 처리
                if (_follow.Text.Equals("따라가기"))
                {
                    if (_pick.Text.Contains("홀") || _pick.Text.Contains("짝"))
                    {
                        if (_pick.Text.Contains(powerballSP)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";

                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballSP);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballSP);
                        }
                    }
                    else if (_pick.Text.Contains("언더") || _pick.Text.Contains("오버"))
                    {
                        if (_pick.Text.Contains(powerballUO)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballUO);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballUO);
                        }
                    }
                    // 반대로 배팅시 처리
                }
                else if (_follow.Text.Equals("반대로"))
                {
                    if (_pick.Text.Contains("홀") || _pick.Text.Contains("짝"))
                    {
                        if (!_pick.Text.Contains(powerballSP)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballSP);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballSP);
                        }
                    }
                    else if (_pick.Text.Contains("언더") || _pick.Text.Contains("오버"))
                    {
                        if (!_pick.Text.Contains(powerballUO)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + powerballUO);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("파워볼 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + powerballUO);
                        }
                    }
                }
            }
            else if (_ballType.Text.Equals("일반"))
            {
                // 따라가기 배팅시 처리
                if (_follow.Text.Equals("따라가기"))
                {
                    if (_pick.Text.Contains("홀") || _pick.Text.Contains("짝"))
                    {
                        if (_pick.Text.Contains(normalballSP)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballSP);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballSP);
                        }
                    }
                    else if (_pick.Text.Contains("언더") || _pick.Text.Contains("오버"))
                    {
                        if (_pick.Text.Contains(normalballUO)) // 파워볼 홀짝
                        {
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballUO);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 따라가기 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballUO);
                        }
                    }
                    // 반대로 배팅시 처리
                }
                else if (_follow.Text.Equals("반대로"))
                {
                    if (_pick.Text.Contains("홀") || _pick.Text.Contains("짝"))
                    {
                        if (!_pick.Text.Contains(normalballSP)) // 파워볼 홀짝
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballSP);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballSP);
                        }
                    }
                    else if (_pick.Text.Contains("언더") || _pick.Text.Contains("오버"))
                    {
                        if (!_pick.Text.Contains(normalballUO)) // 일반볼 언더오버
                        {
                            _level.BackColor = Color.White;
                            _level.ForeColor = Color.Black;
                            _level.Text = "1";
                            _betMoney.Text = txtBtMoneySettingL1.Text;
                            _betMoney.BackColor = Color.White;
                            _betMoney.ForeColor = Color.Black;
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 당첨 : " + normalballUO);
                        }
                        else
                        {
                            semiAutoModeProcessing(_pickster, _level);
                            _levelColorSetting(_level, _betMoney);
                            logger.Info("일반 반대로 : [" + allinning + "회 " + _pickster.Text + " / 픽스터의 선택 : " + _pick.Text + " / 해당차 미당첨 : " + normalballUO);
                        }
                    }
                }
            }
        }

        void semiAutoModeProcessing(TextBox _pickster, TextBox _level)
        {
            try
            {
                if (_semiautomode)
                {
                    if (int.Parse(semiAuto_1.Text) % (int.Parse(_level.Text) + 1) == 0)
                    {
                        if (semiAuto_3.Text.Equals("연승"))
                        {
                            String streak = "";
                            int n = 1;
                            for (int i = 0; i < 200; i++)
                            {
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
                            for (int i = 0; i < 200; i++)
                            {
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
                            for (int _find = 1; _find <= 40; _find++)
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
                            for (int i = 0; i < 200; i++)
                            {
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
                            for (int i = 0; i < 200; i++)
                            {
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
                            for (int _find = 1; _find <= 40; _find++)
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
                textBox2.Text += _ex.ToString();
            }
        }
        // 픽 삭제
        private void clearBetInfor()
        {
            for (int _find = 1; _find <= 40; _find++)
            {
                TextBox _targetTextBox = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                _targetTextBox.Text = "";
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
        }

        void bettingLevel1()
        {
            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    if (_superioritymode)
                    {
                        TextBox _txtBoxPickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                        if (!_txtBoxPickster.Text.Contains("우세픽"))
                        {
                            continue;
                        }
                    }
                    
                    //txtBoxPickster01
                    Button _targetButton = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    if (_targetButton.Text.Equals("배팅진행"))
                    {
                        Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                        TextBox _targetTextBox1 = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                        TextBox _targetTextBox2 = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        TextBox _targetTextBox3 = (Controls.Find("txtBoxPBM0" + _find.ToString(), true)[0] as TextBox);
                        runToBet(_follow.Text, _targetButton.Text, _targetTextBox1.Text, _targetTextBox2.Text, int.Parse(_targetTextBox3.Text));
                    }
                }
                else
                {
                    if (_superioritymode)
                    {
                        TextBox _txtBoxPickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                        if (!_txtBoxPickster.Text.Contains("우세픽"))
                        {
                            continue;
                        }
                    }
                    Button _targetButton = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    if (_targetButton.Text.Equals("배팅진행"))
                    {
                        Button _follow = (Controls.Find("btnFollow" + _find.ToString(), true)[0] as Button);
                        TextBox _targetTextBox1 = (Controls.Find("txtBoxBallType" + _find.ToString(), true)[0] as TextBox);
                        TextBox _targetTextBox2 = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        TextBox _targetTextBox3 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                        runToBet(_follow.Text, _targetButton.Text, _targetTextBox1.Text, _targetTextBox2.Text, int.Parse(_targetTextBox3.Text));
                    }
                }
            }
            bettingStatus = false;
        }
        Boolean bettingStatus = false;
        public void bettingLevel2()
        {
            try
            {
                if (UtilModel._userSite.Equals("wonderful"))
                {
                    string returnMessage = loadingPage(bettingUrl, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                    logger.Info(returnMessage);
                    JObject jo = JObject.Parse(returnMessage);

                    var result = jo.SelectToken("result").ToString();

                    if (result.Contains("OK"))
                    {
                        NowMoney = int.Parse(jo.SelectToken("balance").ToString());
                        if (!_getStartMoney)
                        {
                            _getStartMoney = true;
                            startMoney = NowMoney;
                            lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                        }
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
                        var error = jo.SelectToken("message").ToString();
                        txtLogAdd(error, Color.MediumVioletRed);
                        txtBettingStatus.Text = error;
                        MessageBox.Show(error);
                        logger.Info(allinning + " : 배팅 실패 : " + error);
                        bettingStatus = false;
                    }
                } else if (UtilModel._userSite.Equals("rdw")){
                    logger.Info(_bettingParam);
                    string returnMessage = JPOST(bettingUrl, _bettingParam);
                    logger.Info(returnMessage);
                    JObject jo = JObject.Parse(returnMessage);

                    var result = jo.SelectToken("A").ToString();

                    if (result.Contains("0"))
                    {
                        NowMoney = int.Parse(jo.SelectToken("B").ToString());
                        if (!_getStartMoney)
                        {
                            _getStartMoney = true;
                            startMoney = NowMoney;
                            lblTxtStartMoney.Text = UtilModel.StringFormatChanged(startMoney) + "원";
                        }
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
                        txtBettingStatus.Text = error;
                        MessageBox.Show(error);
                        logger.Info(allinning + " : 배팅 실패 : " + error);
                        bettingStatus = false;
                    }
                }

                //MessageBox.Show(returnMessage);                
            }
            catch (Exception _ex)
            {
                logger.Info(_ex.ToString());                
            }
        }

        String bettingUrl = "";
        String _bettingParam = "";
        private void bet()
        {
            bettingLevel1();
            Double allMoney = pOddMoney + pEvenMoney + nOddMoney + nEvenMoney + pUnderMoney + pOverMoney + nUnderMoney + nOverMoney;
            if (allMoney > 0)
            {
                lblPSingle.Text = pOddMoney + " 원";
                lblPPair.Text = pEvenMoney + " 원";
                lblPUnder.Text = pUnderMoney + " 원";
                lblPOver.Text = pOverMoney + " 원";
                lblNSingle.Text = nOddMoney + " 원";
                lblNPair.Text = nEvenMoney + " 원";
                lblNUnder.Text = nUnderMoney + " 원";
                lblNOver.Text = nOverMoney + " 원";

                if (UtilModel._userSite.Equals("wonderful"))
                {
                    bettingUrl = UtilModel.wonderfulurl;
                    bettingUrl += "?userid=" + UtilModel._id;
                    bettingUrl += "&userpassword=" + UtilModel._password;
                    bettingUrl += "&round=" + allinning;
                    bettingUrl += "&b1_s0=" + pOddMoney;
                    bettingUrl += "&b1_s1=" + pEvenMoney;
                    bettingUrl += "&b2_s0=" + nOddMoney;
                    bettingUrl += "&b2_s1=" + nEvenMoney;
                    bettingUrl += "&b3_s0=" + pUnderMoney;
                    bettingUrl += "&b3_s1=" + pOverMoney;
                    bettingUrl += "&b4_s0=" + nUnderMoney;
                    bettingUrl += "&b4_s1=" + nOverMoney;
                    bettingUrl += "&username=" + UtilModel._id;
                    bettingUrl += "&password=" + UtilModel._password;
                    bettingUrl += "&timetoken=" + UtilModel._timetoken;
                }
                else if (UtilModel._userSite.Equals("rdw"))
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

                    _bettingParam = jsonData.ToString();
                }

                logger.Info(bettingUrl);
                //MessageBox.Show(bettingUrl);
                if (bettingUrl.Length < 10)
                {
                    txtLogAdd("주소가 잘못되었습니다. 확인 부탁드립니다.", Color.MediumVioletRed);
                    txtBettingStatus.Text = "주소가 잘못되었습니다. 확인 부탁드립니다.";
                    MessageBox.Show("주소가 잘못되었습니다. 확인 부탁드립니다.");
                    timer.Stop();
                    _isStart = false;
                    logger.Info("주소가 잘못되었습니다. 확인 부탁드립니다.");
                    return;
                }
                bettingLevel2();
            }
            else
            {
                txtLogAdd("배팅을 한 것이 없어 등록하지 않았습니다.", Color.DarkOrange);
                logger.Info("배팅을 한 것이 없어 등록하지 않았습니다.");
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
            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    TextBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                    TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM0" + _find.ToString(), true)[0] as TextBox);
                    all_Initialization(_targetTextBox1, _targetTextBox2);
                }
                else
                {
                    TextBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                    TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                    all_Initialization(_targetTextBox1, _targetTextBox2);
                }
            }
        }

        // 왼쪽 1~20번 초기화
        private void left_init_Click(object sender, EventArgs e)
        {
            for (int _find = 1; _find <= 20; _find++)
            {
                if (_find < 10)
                {
                    TextBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                    TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM0" + _find.ToString(), true)[0] as TextBox);
                    all_Initialization(_targetTextBox1, _targetTextBox2);
                }
                else
                {
                    TextBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                    TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                    all_Initialization(_targetTextBox1, _targetTextBox2);
                }
            }
        }
        private void right_init_Click(object sender, EventArgs e)
        {
            for (int _find = 21; _find <= 40; _find++)
            {
                TextBox _targetTextBox1 = (Controls.Find("CBL" + _find.ToString(), true)[0] as TextBox);
                TextBox _targetTextBox2 = (Controls.Find("txtBoxPBM" + _find.ToString(), true)[0] as TextBox);
                all_Initialization(_targetTextBox1, _targetTextBox2);
            }
        }
        private void all_Initialization(TextBox L, TextBox PBM)
        {
            L.ForeColor = Color.Black;
            L.BackColor = Color.White;
            L.Text = "1";
            PBM.Text = txtBtMoneySettingL1.Text;

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
            for (int _find = 1; _find <= 20; _find++)
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
            for (int _find = 21; _find <= 40; _find++)
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
            for (int _find = 1; _find <= 40; _find++)
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
            for (int _find = 1; _find <= 20; _find++)
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
            for (int _find = 21; _find <= 40; _find++)
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
            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
            }
        }
        private void left_betStop_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= 20; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
            }
        }
        private void right_betStop_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 21; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStop(_target);
                }
            }
        }


        private void allBettingStart_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
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
            for (int _find = 1; _find <= 20; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
            }
        }

        private void right_betStart_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            for (int _find = 21; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    Button _target = (Controls.Find("btnPR0" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
                else
                {
                    Button _target = (Controls.Find("btnPR" + _find.ToString(), true)[0] as Button);
                    _allBettingStart(_target);
                }
            }
        }
        private void _allBettingStop(Button PR)
        {
            PR.Text = "배팅중지";
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
            powerBallReady(btnPR01);
        }

        private void btnPR02_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR02);
        }

        private void btnPR03_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR03);
        }

        private void btnPR04_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR04);
        }

        private void btnPR05_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR05);
        }

        private void btnPR06_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR06);
        }

        private void btnPR07_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR07);
        }

        private void btnPR08_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR08);
        }

        private void btnPR09_Click(object sender, EventArgs e)
        {
            powerBallReady(btnPR09);
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

        private void powerballFormat(Button _btn, TextBox _L, TextBox _Pbm)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료되어 바꿀 수 없습니다. 진행 중에 변경 부탁드립니다.");
                return;
            }
            _L.ForeColor = Color.Black;
            _L.BackColor = Color.White;
            _L.Text = "1";
            _Pbm.Text = txtBtMoneySettingL1.Text;
        }
        private void btnPF01_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF01, CBL1, txtBoxPBM01);
        }

        private void btnPF02_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF02, CBL2, txtBoxPBM02);
        }

        private void btnPF03_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF03, CBL3, txtBoxPBM03);
        }

        private void btnPF04_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF04, CBL4, txtBoxPBM04);
        }



        private void btnPF05_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF05, CBL5, txtBoxPBM05);
        }


        private void btnPF06_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF06, CBL6, txtBoxPBM06);
        }

        private void btnPF07_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF07, CBL7, txtBoxPBM07);
        }


        private void btnPF08_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF08, CBL8, txtBoxPBM08);
        }

        private void btnPF09_Click(object sender, EventArgs e)
        {
            powerballFormat(btnPF09, CBL9, txtBoxPBM09);
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

        Boolean _automode = false;
        Boolean _automodeChange = false;
        Boolean _manualMode = true;
        Boolean _semiautomode = false;
        Boolean _winstopmode = false;
        Boolean _superioritymode = false;
        Boolean _patternMode = false;

        private void manualModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = false;
            _semiautomode = false;
            _winstopmode = false;
            _superioritymode = false;
            _patternMode = false;
            _manualMode = true;
            picksterPanel.Visible = true;

            txtSwitchMode.Text = "수동 모드";
            txtLogAdd("수동 모드로 선택되었습니다. 해당 모드는 사용자가 모든 픽스터를 선택하는 모드입니다.", Color.MediumVioletRed);
        }
        private void autoModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = true;
            _semiautomode = false;
            _winstopmode = false;
            _superioritymode = false;
            _patternMode = false;
            _manualMode = false;
            picksterPanel.Visible = true;

            txtSwitchMode.Text = "자동 모드";
            txtLogAdd("자동 모드가 시작되었습니다. 해당 모드는 40명의 픽스터를 시스템에서 자동으로 선택하여 배팅을 진행합니다.", Color.MediumVioletRed);
        }

        private void semiAutoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = false;
            _semiautomode = true;
            _winstopmode = false;
            _superioritymode = false;
            _patternMode = false;
            _manualMode = false;
            picksterPanel.Visible = true;

            txtSwitchMode.Text = "반자동 모드";
            txtLogAdd("반자동 모드가 시작되었습니다. 해당 모드는 설정값에 따라 시스템에서 픽스터를 교체 합니다.", Color.MediumVioletRed);
        }

        private void superiorityModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = false;
            _semiautomode = false;
            _winstopmode = false;
            _superioritymode = true;
            _patternMode = false;
            _manualMode = false;
            picksterPanel.Visible = true;

            for (int _find = 1; _find <= 40; _find++)
            {
                if (_find < 10)
                {
                    TextBox _pickster = (Controls.Find("txtBoxPickster0" + _find.ToString(), true)[0] as TextBox);
                    TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                    TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);

                    TextBox PR = (Controls.Find("txtBoxPR0" + _find.ToString(), true)[0] as TextBox);
                    TextBox PS = (Controls.Find("txtBoxPS0" + _find.ToString(), true)[0] as TextBox);
                    _pickster.Text = "우세픽홀짝모드";
                    _ballType.Text = "파워";
                    _pick.Text = "통과"; // 홀짝
                    _pick.ForeColor = Color.Black;
                    PR.Text = "---";
                    PR.ForeColor = Color.White;
                    PS.Text = "---";
                    PS.ForeColor = Color.White;
                }
                else
                {
                    if (_find <= 20)
                    {
                        TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                        TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                        TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                        TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                        _pickster.Text = "우세픽홀짝모드";
                        _ballType.Text = "파워";
                        _pick.Text = "통과"; // 홀짝
                        _pick.ForeColor = Color.Black;
                        PR.Text = "---";
                        PR.ForeColor = Color.White;
                        PS.Text = "---";
                        PS.ForeColor = Color.White;
                    }
                    else
                    {
                        TextBox _pickster = (Controls.Find("txtBoxPickster" + _find.ToString(), true)[0] as TextBox);
                        TextBox _ballType = (Controls.Find("txtboxBallType" + _find.ToString(), true)[0] as TextBox);
                        TextBox _pick = (Controls.Find("BoxPick" + _find.ToString(), true)[0] as TextBox);
                        TextBox PR = (Controls.Find("txtBoxPR" + _find.ToString(), true)[0] as TextBox);
                        TextBox PS = (Controls.Find("txtBoxPS" + _find.ToString(), true)[0] as TextBox);
                        _pickster.Text = "우세픽언오버모드";
                        _ballType.Text = "파워";
                        _pick.Text = "통과"; // 오버 언더
                        _pick.ForeColor = Color.Black;
                        PR.Text = "---";
                        PR.ForeColor = Color.White;
                        PS.Text = "---";
                        PS.ForeColor = Color.White;
                    }
                }
            }

            txtSwitchMode.Text = "픽스터 우세픽 모드";
            txtLogAdd("픽스터 우세픽 모드가 시작되었습니다. 해당 모드는 픽스터의 홀짝,언오버 우세픽에 따라 배팅합니다.", Color.MediumVioletRed);
        }

        private void WinStopRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = false;
            _semiautomode = false;
            _winstopmode = true;
            _superioritymode = false;
            _patternMode = false;
            _manualMode = false;
            picksterPanel.Visible = true;

            txtSwitchMode.Text = "당첨 정지 모드";
            txtLogAdd("당첨정지모드가 시작되었습니다. 해당 모드는 당첨되었을 경우 해당 픽스터는 배팅정지 상태가 됩니다.", Color.MediumVioletRed);
        }

        private void patternModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _automode = false;
            _semiautomode = false;
            _winstopmode = false;
            _superioritymode = false;
            _patternMode = true;
            _manualMode = false;
            picksterPanel.Visible = false;

            txtSwitchMode.Text = "패턴 모드";
            txtLogAdd("패턴 모드로 진행하겠습니다. 해당 모드는 패턴대로 진행됩니다.", Color.MediumVioletRed);
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
                bool isContains = item.SubItems[0].Text.Contains(keyword);
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
            // lblSum1
            int sum = int.Parse((Controls.Find("txtBtMoneySettingL1", true)[0] as TextBox).Text);
            for (int i = 1; i <= 19; i++)
            {
                TextBox txtBtMoneySetting = (Controls.Find("txtBtMoneySettingL" + i.ToString(), true)[0] as TextBox);
                TextBox txtBtMoneySettingUp = (Controls.Find("txtBtMoneySettingL" + (i + 1).ToString(), true)[0] as TextBox);
                TextBox GetWinMoney = (Controls.Find("GetWinMoney" + (i).ToString(), true)[0] as TextBox);
                TextBox GetWinProFit = (Controls.Find("GetWinProFit" + (i).ToString(), true)[0] as TextBox);
                ComboBox magnification = (Controls.Find("magnification" + (i + 1).ToString(), true)[0] as ComboBox);
                TextBox txtBoxSum = (Controls.Find("txtBoxSum" + (i + 1).ToString(), true)[0] as TextBox);
                int _txtBtMoneySetting = int.Parse(txtBtMoneySetting.Text);
                Double _magnification = Double.Parse(magnification.Text);
                txtBtMoneySettingUp.Text = ((int)(_txtBtMoneySetting * _magnification)).ToString();
                int getwinmoney = (int)(_txtBtMoneySetting * 1.95);
                GetWinMoney.Text = UtilModel.StringFormatChanged(getwinmoney) + "원";
                GetWinProFit.Text = UtilModel.StringFormatChanged(getwinmoney - sum) + "원";
                sum += int.Parse(txtBtMoneySettingUp.Text);
                txtBoxSum.Text = UtilModel.StringFormatChanged(sum) + " 원";
            }
        }

        private void samePersonCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (samePersonCheckBox.Checked)
            {
                txtLogAdd("동일 유저를 넣을 수 있습니다.", Color.Black);
            }
            else
            {
                txtLogAdd("동일 유저를 넣을 수 없습니다.", Color.Black);
            }
        }

        private void outcomeMarkChechBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void resultMarkCheckBox_CheckedChanged(object sender, EventArgs e)
        {

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
                    SubNode.AppendChild(CreateNode(XmlDoc, "levelmoney" + i, _msl.Text));
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
                MessageBox.Show(_ex.ToString());
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
                MessageBox.Show(_ex.ToString());
            }
        }

        void LevelChangeSelectedindexChange(TextBox _level, int num)
        {
            TextBox _msl = (Controls.Find("txtBtMoneySettingL" + _level.Text, true)[0] as TextBox);
            TextBox _betMoney;
            if (num < 10)
            {
                _betMoney = (Controls.Find("txtBoxPBM0" + num, true)[0] as TextBox);
            }
            else
            {
                _betMoney = (Controls.Find("txtBoxPBM" + num, true)[0] as TextBox);
            }

            _level.ForeColor = Color.Black;
            _level.BackColor = Color.White;
            if (_betMoney != null)
            {
                _betMoney.Text = _msl.Text;
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

        private void BoxPick1_click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick2_click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick3_click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick4_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick5_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick6_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick7_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick8_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick9_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick10_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick11_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick12_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick13_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick14_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick15_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick16_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick17_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick18_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick19_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick20_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick21_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick22_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick23_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick24_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick25_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick26_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick27_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick28_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick29_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick30_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick31_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick32_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick33_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick34_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick35_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick36_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick37_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick38_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick39_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void BoxPick40_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL1_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL2_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL3_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL4_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL5_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL6_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL7_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL8_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL9_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL10_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL11_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL12_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL13_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL14_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL15_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL16_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL17_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL18_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL19_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL20_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL21_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL22_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL23_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL24_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL25_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL26_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL27_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL28_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL29_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL30_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL31_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL32_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL33_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL34_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL35_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL36_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL37_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL38_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL39_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void CBL40_Click(object sender, EventArgs e)
        {
            if (_bettingClosed)
            {
                MessageBox.Show("지금은 배팅이 종료된 상태이기 때문에 바꿀 수 없습니다.");
                return;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            loadPicksterType = "ntry";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            loadPicksterType = "auto";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            loadPicksterType = "pbg";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            loadPicksterType = "prediction";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            loadPicksterType = "pattern";
        }
    }

    public static class Extensions // 리스트뷰 깜박임 제거
    {
        public static void DoubleBuffered(this Control control, bool enabled)

        {
            var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, enabled, null);
        }
    }
}
