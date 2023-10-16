using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PowerBallAutoMatinUserInformation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            FormClosing += MainForm_FormClosing;
            InitializeComponent();
        }

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


            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록
            timer.Start();
            getPowerballInformation();
        }

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

        public void setTimeRemaining(int _remainTime)
        {
            TimeSpan getTimeSpan = TimeSpan.FromSeconds(_remainTime);

            remainTime.Text = string.Format("{0:00}:{1:00}", getTimeSpan.Minutes, getTimeSpan.Seconds);
        }

        int remainingTime = 0;
        int allinning = 0;
        int todayinning = 0;
        // 남은 시간 타이머
        System.Timers.Timer timer = new System.Timers.Timer();

        delegate void TimerEventFiredDelegate();
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(remainingTimer_Tick));
        }

        // 시간마다 해야할 작업 정리 오류가 나지 않게 더 세세하게 정리해야한다.
        private void remainingTimer_Tick()
        {
            if (remainingTime > 1)
            {
                setTimeRemaining(remainingTime--);
                if (remainingTime == 260)
                {
                    loadingUserInformation();
                }
                
            }
            else
            {
                getPowerballInformation();
            }
        }

        void loadingUserInformation()
        {
            try
            {
                listView1.Items.Clear();
                string url = "http://www.pbserver.cf/pickster/userInformationLoading.php?";                                         // 통신할 URL
                 url += string.Format("loading=&username={0}&password={1}&timetoken={2}", _id, _password, _timetoken); // 전송할 Parameter

                XElement root = XElement.Load(url);

                ListViewItem _listvieitem;
                foreach (XElement item in root.Descendants("item"))
                {
                    _listvieitem = new ListViewItem(item.Attribute("userid").Value);
                    _listvieitem.UseItemStyleForSubItems = false;
                    _listvieitem.SubItems.Add(item.Attribute("nickname").Value);
                    _listvieitem.SubItems.Add(stringFormat(int.Parse(item.Attribute("ownmoney").Value)));
                    _listvieitem.SubItems.Add(item.Attribute("allinning").Value);
                    _listvieitem.SubItems.Add(stringFormat(int.Parse(item.Attribute("startmoney").Value)));
                    _listvieitem.SubItems.Add(stringFormat(int.Parse(item.Attribute("allbettingmoney").Value)));
                    _listvieitem.SubItems.Add(stringFormat(int.Parse(item.Attribute("winmoney").Value)));
                    _listvieitem.SubItems.Add(stringFormat(int.Parse(item.Attribute("gain").Value)));
                    _listvieitem.SubItems.Add(item.Attribute("starttime").Value);
                    _listvieitem.SubItems.Add(item.Attribute("logtime").Value);
                    listView1.Items.Add(_listvieitem);
                };
            } catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        string stringFormat(int _str)
        {
            return string.Format("{0:#,##0}", _str) + " 원";
        }
        public static String GetHttpPOST(string reqstring, string url, string encode, ref int errcode)
        {
            String retValue = "";

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
        void getPowerballInformation()
        {
            try
            {
                String _uri = "http://www.pbserver.cf/pickster/servertime.php";
                XElement root = XElement.Load(_uri);
                foreach (XElement item in root.Descendants("item"))
                {
                    allinning = int.Parse(item.Attribute("allinning").Value);
                    todayinning = int.Parse(item.Attribute("todayinning").Value);
                    remainingTime = int.Parse(item.Attribute("timediffer").Value);
                };

                round.Text = allinning + "회";
                txtBoxTodayInning.Text = (todayinning + 1) + "회";
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        String _timetoken = "";
        String _id = "";
        String _password = "";
        private void LoginSuccess(string id, string password, string name, String _token)
        {
            _id = id;
            _password = password;
            _timetoken = _token;
            MessageBox.Show(name + "님 반갑습니다.");
        }
    }
}
