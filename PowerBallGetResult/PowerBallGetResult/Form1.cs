using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PowerBallGetResult
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            MakeDriver();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록
        }

        public void txtLogAdd(string str)
        {
            try
            {
                textBox1.AppendText(getTime() + str + Environment.NewLine);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
            catch (FormatException formatexception)
            {
                Console.WriteLine(formatexception);
            }
        }

        private String getTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
        }

        int remainingTime = 0;
        private int getPowerballInformation()
        {
            try
            {
                XElement root = XElement.Load("http://pbserver.cafe24.com/pickster/servertime.php");
                foreach (XElement item in root.Descendants("item"))
                {
                    remainingTime = int.Parse(item.Attribute("timediffer").Value);
                };
            }
            catch (Exception adsf)
            {
                Console.WriteLine(adsf.ToString(), Color.MediumVioletRed);
            }
            return remainingTime;
        }

        public static ChromeDriver driver;
        public static ChromeDriver MakeDriver()
        {
            ChromeOptions cOptions = new ChromeOptions();
            //cOptions.AddArguments("disable-infobars"); 
            //cOptions.AddArguments("--js-flags=--expose-gc"); 
            //cOptions.AddArguments("--enable-precise-memory-info"); 
            //cOptions.AddArguments("--disable-popup-blocking"); 
            //cOptions.AddArguments("--disable-default-apps"); 
            //cOptions.AddArguments("--headless"); 
            //options.AddArgument("--window-position=-32000,-32000");
            //윈도우창 위치값을 화면밖으로 조정 
            // 프록시 설정 
            //Proxy proxy = new Proxy();
            //proxy.Kind = ProxyKind.Manual; 
            // proxy.IsAutoDetect = false; 
            //proxy.HttpProxy = proxy.SslProxy = ip; 
            //cOptions.Proxy = proxy; 
            //cOptions.AddArgument("ignore-certificate-errors");

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, cOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return driver;
        }
        string _minute = "";
        string _second = "";
        int nowallInning = 0;
        string allinning = "";
        string todayinning = "";
        string time = "";
        string powerball = "";
        string normalball = "";
        ListViewItem _listvieitem;
        string page = "";
        string page2 = "";
        string page3 = "";
        Boolean _bool1 = false;

        void donghaeng()
        {
            driver.Navigate().GoToUrl("https://www.dhlottery.co.kr/gameInfo.do?method=powerWinNoList");

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _minute = "";
            _second = "";
            allinning = driver.FindElementByCssSelector("#frm > table.tbl_data.tbl_data_col > tbody > tr:nth-child(1) > td:nth-child(2)").Text.ToString(); // 총 회차
            nowallInning = int.Parse(allinning) + 1;
            todayinning = "0";
            time = "";
            powerball = driver.FindElementByXPath("/html/body/div[3]/section/div/div[2]/div/form/table[2]/tbody/tr[1]/td[4]/span").Text.ToString();
            normalball = driver.FindElementByXPath("/html/body/div[3]/section/div/div[2]/div/form/table[2]/tbody/tr[1]/td[5]").Text.ToString();

            txtLogAdd(_minute + "분 " + _second + "초");
            txtLogAdd("현재 진행 라운드 : " + nowallInning);
            txtLogAdd("allinning : " + allinning);
            txtLogAdd("todayinning : " + todayinning);
            txtLogAdd("time : " + time);
            txtLogAdd("powerball : " + powerball);
            txtLogAdd("normalball : " + normalball);
        }
        private Boolean getInfor()
        {
            try
            {
               if (radioButton2.Checked) // 동행복권
                {
                    if (remainingTime > 283)
                    {
                        donghaeng();
                        _listvieitem = new ListViewItem(nowallInning + "회");
                        _listvieitem.UseItemStyleForSubItems = false;
                        _listvieitem.SubItems.Add(allinning);
                        _listvieitem.SubItems.Add(todayinning);
                        _listvieitem.SubItems.Add(time);
                        _listvieitem.SubItems.Add(powerball);
                        _listvieitem.SubItems.Add(normalball);

                        int pbNum = int.Parse(powerball);
                        String POE = "";
                        String PUO = "";
                        if (pbNum > 4)
                        {
                            PUO = "오버";
                        }
                        else
                        {
                            PUO = "언더";
                        }
                        if (pbNum % 2 == 1)
                        {
                            POE = "홀";
                        }
                        else
                        {
                            POE = "짝";
                        }

                        int nbNum = int.Parse(normalball);
                        String NOE = "";
                        String NUO = "";
                        if (nbNum > 4)
                        {
                            NUO = "오버";
                        }
                        else
                        {
                            NUO = "언더";
                        }
                        if (nbNum % 2 == 1)
                        {
                            NOE = "홀";
                        }
                        else
                        {
                            NOE = "짝";
                        }
                        _listvieitem.SubItems.Add(POE);
                        _listvieitem.SubItems.Add(PUO);
                        _listvieitem.SubItems.Add(NOE);
                        _listvieitem.SubItems.Add(NUO);
                        listView1.Items.Add(_listvieitem);
                        //txtLogAdd(page, Color.DarkOrange);

                        if (cafe24.Checked)
                        {
                            page = "http://pbserver.cafe24.com/pickster/registResult.php?";
                            page += "nowallinning=" + nowallInning;
                            page += "&allinning=" + allinning;
                            page += "&todayinning=" + todayinning;
                            //page += "&time='" + time;
                            page += "&powerball=" + powerball;
                            page += "&normalball=" + normalball;

                            string returnMessage = loadingPage(page, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                            if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                            {
                                txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [cafe24]");
                                _bool1 = true;
                            }
                            else if (returnMessage.Contains("already"))
                            {
                                txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[cafe24]");
                                _bool1 = true;
                            }
                            else
                            {
                                txtLogAdd(returnMessage + "오류 [cafe24]");
                                _bool1 = false;
                            }
                        }

                        if (pbserver.Checked)
                        {
                            page2 = "http://104.198.92.168//pickster/registResult.php?";
                            page2 += "nowallinning=" + nowallInning;
                            page2 += "&allinning=" + allinning;
                            page2 += "&todayinning=" + todayinning;
                            //page += "&time='" + time;
                            page2 += "&powerball=" + powerball;
                            page2 += "&normalball=" + normalball;
                            string returnMessage = loadingPage(page2, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                            if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                            {
                                txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [pbserver.ml]");
                                _bool1 = true;
                            }
                            else if (returnMessage.Contains("already"))
                            {
                                txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[pbserver.ml]");
                                _bool1 = true;
                            }
                            else
                            {
                                txtLogAdd(returnMessage + "오류[pbserver.ml]");
                                _bool1 = false;
                            }
                        }

                        if (wonderful.Checked)
                        {
                            page3 = "http://wdfserver.tk/pickster/registResult.php?";
                            page3 += "nowallinning=" + nowallInning;
                            page3 += "&allinning=" + allinning;
                            page3 += "&todayinning=" + todayinning;
                            //page += "&time='" + time;
                            page3 += "&powerball=" + powerball;
                            page3 += "&normalball=" + normalball;
                            string returnMessage = loadingPage(page3, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                            if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                            {
                                txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [pbserver.ml]");
                                _bool1 = true;
                            }
                            else if (returnMessage.Contains("already"))
                            {
                                txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[pbserver.ml]");
                                _bool1 = true;
                            }
                            else
                            {
                                txtLogAdd(returnMessage + "오류[pbserver.ml]");
                                _bool1 = false;
                            }
                        }
                    }
                } else if (radioButton3.Checked)
                {
                    driver.Navigate().GoToUrl("http://www.powerballgame.co.kr/?view=dayLog");

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    _minute = driver.FindElementByXPath("/html/body/div[1]/div[2]/div[1]/span/strong/span[1]").Text.ToString(); // 남은 분
                    _second = driver.FindElementByXPath("/html/body/div[1]/div[2]/div[1]/span/strong/span[2]").Text.ToString(); // 남은 초
                    nowallInning = int.Parse(driver.FindElementByXPath("/html/body/div[1]/div[2]/div[1]/span/span/strong").Text.ToString()); // 현재 총 회차
                    allinning = Regex.Replace(driver.FindElementByXPath("/html/body/table[5]/tbody[2]/tr[1]/td[1]/span[1]").Text.ToString(), @"\D", ""); // 총 회차
                    todayinning = Regex.Replace(driver.FindElementByXPath("/html/body/table[5]/tbody[2]/tr[1]/td[1]/span[2]").Text.ToString(), @"\D", "");
                    time = driver.FindElementByXPath("/html/body/table[5]/tbody[2]/tr[1]/td[2]").Text.ToString();
                    powerball = driver.FindElementByXPath("/html/body/table[5]/tbody[2]/tr[1]/td[3]/div").Text.ToString();
                    normalball = driver.FindElementByXPath("/html/body/table[5]/tbody[2]/tr[1]/td[8]").Text.ToString();

                    txtLogAdd(_minute + "분 " + _second + "초");
                    txtLogAdd("현재 진행 라운드 : " + nowallInning);
                    txtLogAdd("allinning : " + allinning);
                    txtLogAdd("todayinning : " + todayinning);
                    txtLogAdd("time : " + time);
                    txtLogAdd("powerball : " + powerball);
                    txtLogAdd("normalball : " + normalball);

                    _listvieitem = new ListViewItem(nowallInning + "회");
                    _listvieitem.UseItemStyleForSubItems = false;
                    _listvieitem.SubItems.Add(allinning);
                    _listvieitem.SubItems.Add(todayinning);
                    _listvieitem.SubItems.Add(time);
                    _listvieitem.SubItems.Add(powerball);
                    _listvieitem.SubItems.Add(normalball);

                    int pbNum = int.Parse(powerball);
                    String POE = "";
                    String PUO = "";
                    if (pbNum > 4)
                    {
                        PUO = "오버";
                    }
                    else
                    {
                        PUO = "언더";
                    }
                    if (pbNum % 2 == 1)
                    {
                        POE = "홀";
                    }
                    else
                    {
                        POE = "짝";
                    }

                    int nbNum = int.Parse(normalball);
                    String NOE = "";
                    String NUO = "";
                    if (nbNum > 4)
                    {
                        NUO = "오버";
                    }
                    else
                    {
                        NUO = "언더";
                    }
                    if (nbNum % 2 == 1)
                    {
                        NOE = "홀";
                    }
                    else
                    {
                        NOE = "짝";
                    }
                    _listvieitem.SubItems.Add(POE);
                    _listvieitem.SubItems.Add(PUO);
                    _listvieitem.SubItems.Add(NOE);
                    _listvieitem.SubItems.Add(NUO);
                    listView1.Items.Add(_listvieitem);
                    //txtLogAdd(page, Color.DarkOrange);

                    if (cafe24.Checked)
                    {
                        page = "http://pbserver.cafe24.com/pickster/registResult.php?";
                        page += "nowallinning=" + nowallInning;
                        page += "&allinning=" + allinning;
                        page += "&todayinning=" + todayinning;
                        //page += "&time='" + time;
                        page += "&powerball=" + powerball;
                        page += "&normalball=" + normalball;

                        string returnMessage = loadingPage(page, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                        if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                        {
                            txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [cafe24]");
                            _bool1 = true;
                        }
                        else if (returnMessage.Contains("already"))
                        {
                            txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[cafe24]");
                            _bool1 = true;
                        }
                        else
                        {
                            txtLogAdd(returnMessage + "오류 [cafe24]");
                            _bool1 = false;
                        }
                    }

                    if (pbserver.Checked)
                    {
                        page2 = "http://104.198.92.168//pickster/registResult.php?";
                        page2 += "nowallinning=" + nowallInning;
                        page2 += "&allinning=" + allinning;
                        page2 += "&todayinning=" + todayinning;
                        //page += "&time='" + time;
                        page2 += "&powerball=" + powerball;
                        page2 += "&normalball=" + normalball;
                        string returnMessage = loadingPage(page2, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                        if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                        {
                            txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [pbserver.ml]");
                            _bool1 = true;
                        }
                        else if (returnMessage.Contains("already"))
                        {
                            txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[pbserver.ml]");
                            _bool1 = true;
                        }
                        else
                        {
                            txtLogAdd(returnMessage + "오류[pbserver.ml]");
                            _bool1 = false;
                        }
                    }

                    if (wonderful.Checked)
                    {
                        page3 = "http://wdfserver.tk/pickster/registResult.php?";
                        page3 += "nowallinning=" + nowallInning;
                        page3 += "&allinning=" + allinning;
                        page3 += "&todayinning=" + todayinning;
                        //page += "&time='" + time;
                        page3 += "&powerball=" + powerball;
                        page3 += "&normalball=" + normalball;
                        string returnMessage = loadingPage(page3, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                        if (returnMessage.Contains("OK") || returnMessage.Contains("ok"))
                        {
                            txtLogAdd(returnMessage + "정상적으로 등록 되었습니다. [pbserver.ml]");
                            _bool1 = true;
                        }
                        else if (returnMessage.Contains("already"))
                        {
                            txtLogAdd(returnMessage + "이미 다른 컴퓨터에서 등록하였습니다.[pbserver.ml]");
                            _bool1 = true;
                        }
                        else
                        {
                            txtLogAdd(returnMessage + "오류[pbserver.ml]");
                            _bool1 = false;
                        }
                    }
                }

            }
            catch (Exception _info)
            {
                Console.WriteLine(driver.Url + Environment.NewLine + _info.ToString());
            }
            return _bool1;
        }

        private void refreshPicksterInformation()
        {
            try
            {
                int pbNum = int.Parse(powerball);
                String OE = "";
                String UO = "";
                if (pbNum > 4)
                {
                    UO = "오버";
                } else
                {
                    UO = "언더";
                }
                if (pbNum % 2 == 1)
                {
                    OE = "홀";
                } else
                {
                    OE = "짝";
                }
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                string page = "http://pbserver.cafe24.com/pickster/refreshPicksterInformation.php?";
                page += "&OE=" + OE;
                page += "&UO=" + UO;
                //txtLogAdd(page, Color.DarkOrange);

                string returnMessage = loadingPage(page, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                if (returnMessage.Contains("OK"))
                {
                    txtLogAdd(returnMessage + "정상적으로 등록 되었습니다.");
                }
                else
                {
                    txtLogAdd(returnMessage + "오류");
                }
            }
            catch (Exception _info)
            {
                Console.WriteLine(driver.Url + Environment.NewLine + _info.ToString());
            }
        }

        private void refreshPicksterInformationForGoogle()
        {
            try
            {
                int pbNum = int.Parse(powerball);
                int nbNum = int.Parse(normalball);
                String POE = "";
                String PUO = "";
                String NOE = "";
                String NUO = "";
                if (pbNum > 4)
                {
                    PUO = "오";
                }
                else
                {
                    PUO = "언";
                }
                if (nbNum > 72)
                {
                    NUO = "오";
                }
                else
                {
                    NUO = "언";
                }
                if (pbNum % 2 == 1)
                {
                    POE = "홀";
                }
                else
                {
                    POE = "짝";
                }
                if (nbNum % 2 == 1)
                {
                    NOE = "홀";
                }
                else
                {
                    NOE = "짝";
                }
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                string page = "http://104.198.92.168//pickster/refreshPicksterInformation.php?";
                page += "&POE=" + POE;
                page += "&PUO=" + PUO;
                page += "&NOE=" + NOE;
                page += "&NUO=" + NUO;
                //txtLogAdd(page, Color.DarkOrange);

                string returnMessage = loadingPage(page, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                if (returnMessage.Contains("OK"))
                {
                    txtLogAdd("Google Server : " + returnMessage + "정상적으로 등록 되었습니다.");
                }
                else
                {
                    txtLogAdd(returnMessage + "오류");
                }
            }
            catch (Exception _info)
            {
                Console.WriteLine(driver.Url + Environment.NewLine + _info.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            donghaeng();
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
        System.Timers.Timer timer = new System.Timers.Timer();



        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(remainingTimer_Tick));
        }
        public void setTimeRemaining(int _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);
            remainTime.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }
        Boolean _bool = false;
        private void remainingTimer_Tick()
        {
            if (remainingTime > 0)
            {
                setTimeRemaining(remainingTime--);

                if (remainingTime < 270 && remainingTime > 10)
                {
                    if (remainingTime % 30 == 0)
                    {
                        driver.Navigate().GoToUrl("https://www.dhlottery.co.kr/gameInfo.do?method=powerWinNoList");
                    }
                }
                if ((remainingTime == 295 || remainingTime == 290 || remainingTime == 285))
                {
                    if (checkBox2.Checked)
                    {
                        getInfor();
                    }
                }
                if ((remainingTime == 283 || remainingTime == 278 || remainingTime == 273) && !_bool)
                {
                    if (checkBox2.Checked)
                    {
                        _bool = getInfor();
                    }
                }

                if (remainingTime == 270)
                {
                    if (checkBox1.Checked)
                    {
                        refreshPicksterInformation();
                        refreshPicksterInformationForGoogle();
                    }
                }
            }
            else
            {
                _bool = false;
                getPowerballInformation();
                if (remainingTime < 0)
                {
                    remainingTime = 300;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                driver.Close();
            } catch (Exception _ex)
            {

            }
        }
    }
}
