using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PowerBallGameGetPickster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public ChromeDriver driver;
        public ChromeDriver MakeDriver()
        {
            ChromeOptions cOptions = new ChromeOptions();
            //cOptions.AddArguments("disable-infobars"); 
            //cOptions.AddArguments("--js-flags=--expose-gc"); 
            //cOptions.AddArguments("--enable-precise-memory-info"); 
            cOptions.AddArguments("--disable-popup-blocking");
            cOptions.AddArguments("--blink-settings=imagesEnabled=false");
            cOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
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
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            return driver;
        }

        /*
        private void gotoUrlLevel1()
        {
            var idField = driver.FindElementById("login_email"); //id를 통해 tag 셀렉트 
            var pwField = driver.FindElementById("login_password");
            var loginButton = driver.FindElementByXPath("/html/body/div[1]/div[2]/div/form/fieldset/button");
            idField.SendKeys(id.Text);
            pwField.SendKeys(password.Text);
            loginButton.Click();
        }
        */
        public bool verifyXpath(string elementName)
        {
            try
            {
                bool isElementDisplayed = driver.FindElement(By.XPath(elementName)).Displayed;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool verifySelector(string elementName)
        {
            try
            {
                bool isElementDisplayed = driver.FindElement(By.CssSelector(elementName)).Displayed;
                return true;
            }
            catch
            {
                return false;
            }
        }
        int second = 0;
        private void gotoUrlLevel2(String _url)
        {
            if (remainingTime > 0 && remainingTime < 55 || remainingTime > 270)
            {
                return;
            }
            driver.Navigate().GoToUrl(_url);
            String _powerPick = "";
            String _record = "";
            String _id = "";
            String _countdown = "";
            for (int i = 1; i <= 22; i++)
            {
                try
                {
                    bool _bool_countdown = driver.FindElement(By.CssSelector("#game_draw_time > strong")).Displayed;
                    if (!_bool_countdown)
                    {
                        textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " [" + i + "] : nullException" + Environment.NewLine;
                        break;
                    }
                    _countdown = (driver.FindElementByCssSelector("#game_draw_time > strong").Text.ToString());

                    _countdown = _countdown.Replace("분", "");
                    _countdown = _countdown.Replace("초", "");
                    string[] ctdSplit = _countdown.Split(new char[] { ' ' });

                    second = (int.Parse(ctdSplit[0]) * 60) + (int.Parse(ctdSplit[1]));
                    if (second < 55 || second > 270)
                    {
                        remainingTime = second;
                        textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " : " + second + " 초. 안 읽어온다." + Environment.NewLine;
                        break;
                    }
                }
                catch (Exception _ex)
                {
                    textBox1.Text += _ex.ToString() + Environment.NewLine;
                }

                try
                {
                    bool test = verifySelector("#content > div.game_pick_list > table > tbody > tr:nth-child(" + i + ") > td.pick_cell.main_pick_cell.ready_pick > span");
                    if (!test)
                    {
                        textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " [" + i + "] : nullException" + Environment.NewLine;
                        break;
                    }

                    // #content > div.game_pick_list > table > tbody > tr:nth-child(1) > td.pick_cell.main_pick_cell.ready_pick > span    방채팅
                    // #content > div.game_pick_list > table > tbody > tr:nth-child(2) > td.pick_cell.main_pick_cell.ready_pick > span
                    // #content > div.game_pick_list > table > tbody > tr:nth-child(3) > td.pick_cell.main_pick_cell.ready_pick > span
                    // #content > div.pagination > div > div > div > a:nth-child(6)
                    // #content > div.pagination > div > div > div > a:nth-child(7)
                    _powerPick = (driver.FindElementByCssSelector("#content > div.game_pick_list > table > tbody > tr:nth-child(" + i + ") > td.pick_cell.main_pick_cell.ready_pick > span").Text.ToString());
                    textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " : " + _powerPick + Environment.NewLine;
                    if (_powerPick.Contains("방채팅"))
                    {
                        continue;
                    }
                    if (_powerPick.Contains("구독"))
                    {
                        try
                        {
                            driver.FindElementByXPath("/html/body/div[3]/div[2]/div[2]/div[7]/table/tbody/tr[" + i + "]/td[1]/span").Click();
                            Thread.Sleep(500);
                        } catch (Exception _ex)
                        {
                            textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " : " + _ex.ToString() + Environment.NewLine;
                        }
                    } else
                    {
                        if (_powerPick.Equals("언"))
                        {
                            _powerPick = "파언더";
                        }
                        else if (_powerPick.Equals("오"))
                        {
                            _powerPick = "파오버";
                        }
                        else if (_powerPick.Equals("홀"))
                        {
                            _powerPick = "파홀";
                        }
                        else if (_powerPick.Equals("짝"))
                        {
                            _powerPick = "파짝";
                        }

                        _record = (driver.FindElementByXPath("/html/body/div[3]/div[2]/div[2]/div[7]/table/tbody/tr[" + i + "]/td[3]/a").Text.ToString());
                        _id = (driver.FindElementByCssSelector("#content > div.game_pick_list > table > tbody > tr:nth-child(" + i + ") > td._user_info_ > div > a.unick").Text.ToString());

                        if (_powerPick.Contains("홀") || _powerPick.Contains("짝") || _powerPick.Contains("언더") || _powerPick.Contains("오버"))
                        {
                            _record = _record.Replace("[", " ") + Environment.NewLine;
                            _record = _record.Replace("]", " ") + Environment.NewLine;
                            _record = _record.Replace("(", " ") + Environment.NewLine;
                            _record = _record.Replace(")", " ") + Environment.NewLine;
                            _record = _record.Replace("파워", "") + Environment.NewLine;
                            _record = _record.Replace("일반", "") + Environment.NewLine;

                            string[] strSplit = _record.Split(new char[] { ' ' });
                            string _inning = strSplit[1]; //회차
                            string pbrecord = strSplit[3]; // 파워볼 승
                            string pbstreak = strSplit[4]; // 연승

                            string page = "http://pbserver.cafe24.com/pickster/registPickster.php?";
                            page += "picksterName=" + _id;
                            page += "&site=ntry";
                            page += "&inning=" + _inning;
                            page += "&pp=" + _powerPick;
                            page += "&pbrecord=" + pbrecord;
                            page += "&pbstreak=" + pbstreak;
                            loadingPage(page, null, "GET", null, Encoding.GetEncoding("UTF-8"));

                            string page2 = "http://104.198.92.168//pickster/registPickster.php?";
                            page2 += "picksterName=" + _id;
                            page2 += "&site=ntry";
                            page2 += "&inning=" + _inning;
                            page2 += "&pp=" + _powerPick;
                            page2 += "&pbrecord=" + pbrecord;
                            page2 += "&pbstreak=" + pbstreak;
                            loadingPage(page2, null, "GET", null, Encoding.GetEncoding("UTF-8"));
                        }
                    }
                    
                }
                catch (Exception _ex)
                {
                    textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " : " + _ex.ToString() + Environment.NewLine;
                    break;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer.Start();
            //gotoUrlLevel1();
            second = remainingTime;
        }

        private  HttpWebRequest _webRequest;
        private  HttpWebResponse _webResponse;
        public  CookieContainer _cookieContainer;
        public  CookieCollection _cookieCollection;
        private  Stream _responseStream;
        private  StreamReader _streamReader;
        //private StreamWriter g;
        private string loadingPageHtml;
        public String loadingPage(string _url, string _referer, string _method, string _params, Encoding _encoding)
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
        int remainingTime = 0;
        System.Timers.Timer timer = new System.Timers.Timer();

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록

            MakeDriver();
            driver.Navigate().GoToUrl("http://ntry.com");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public void setTimeRemaining(int _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            remaintime.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }
        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(remainingTimer_Tick));
        }
        void FuncA()
        {
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
            }
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
            }
        }
        void FuncB()
        {
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
            }
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
            }
        }
        void FuncC()
        {
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
            }
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
            }
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=3");
            }
            if (second > 55 || second < 271)
            {
                gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
            }
        }
        void FuncD()
        {
            if (radioButton3.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=3");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
            }
            else if (radioButton2.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
            }
            else if (radioButton1.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=3");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
            }
            else if (radioButton4.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
            }
        }
        void FuncE()
        {
            if (radioButton3.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=3");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
            } else if (radioButton2.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=2");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
            } else if (radioButton1.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=3");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
            } else if (radioButton4.Checked)
            {
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=7");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=6");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=5");
                }
                if (second > 55 || second < 271)
                {
                    gotoUrlLevel2("http://ntry.com/tipster/pick.php?qgame=POWER&page=4");
                }
            }

        }

        private void remainingTimer_Tick()
        {
            if (remainingTime > 0)
            {
                remaintime.Text = (remainingTime--) + "초";
                if (remainingTime == 272)
                {
                    second = 0;
                }
                if (remainingTime == 250)
                {
                    second = 0;
                    FuncA();
                }
                else if (remainingTime == 209)
                {
                    second = 0;
                    if (radioButton4.Checked)
                    {
                        FuncE();
                    } else
                    {
                        FuncE();
                    }
                }
                else if (remainingTime == 150)
                {
                    second = 0;
                    FuncE();
                }
                else if (remainingTime == 120 || remainingTime == 90 || remainingTime == 70)
                {
                    second = 0;
                    FuncE();
                }
            }
            else
            {
                if (remainingTime < 0)
                {
                    remainingTime = 300;
                }
                else
                {
                    getPowerballRemainingTime();
                }
            }
        }
        private int getPowerballRemainingTime()
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
                textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + " [getPowerballRemainingTime error]" + adsf + Environment.NewLine;
            }
            return remainingTime;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            driver.Close();
        }
    }
}
