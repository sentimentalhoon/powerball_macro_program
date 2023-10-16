namespace AllInOneRedMangoProject
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.Game_CruiseBetMoneySettingTextBox = new System.Windows.Forms.TextBox();
            this.Game_CruiseBetListView = new System.Windows.Forms.ListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader146 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader145 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.virtualMoney = new System.Windows.Forms.CheckBox();
            this.lblNowMoney = new System.Windows.Forms.Label();
            this.lblTxtNowMoney = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Game_CruiseBetMoneyPercentSettingComboBox = new System.Windows.Forms.ComboBox();
            this.serverCheckTimeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.betpage3 = new AllInOneRedMangoProject.betpage();
            this.betpage4 = new AllInOneRedMangoProject.betpage();
            this.betpage2 = new AllInOneRedMangoProject.betpage();
            this.betpage1 = new AllInOneRedMangoProject.betpage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.clearLevelSet = new System.Windows.Forms.ComboBox();
            this.betpage5 = new AllInOneRedMangoProject.betpage();
            this.betpage6 = new AllInOneRedMangoProject.betpage();
            this.betpage7 = new AllInOneRedMangoProject.betpage();
            this.betpage8 = new AllInOneRedMangoProject.betpage();
            this.groupBox25.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.Game_CruiseBetMoneySettingTextBox);
            this.groupBox25.Controls.Add(this.Game_CruiseBetListView);
            this.groupBox25.ForeColor = System.Drawing.Color.Black;
            this.groupBox25.Location = new System.Drawing.Point(342, 25);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(623, 258);
            this.groupBox25.TabIndex = 135;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "파워볼 크루즈 설정";
            this.groupBox25.Visible = false;
            // 
            // Game_CruiseBetMoneySettingTextBox
            // 
            this.Game_CruiseBetMoneySettingTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Game_CruiseBetMoneySettingTextBox.ForeColor = System.Drawing.Color.Black;
            this.Game_CruiseBetMoneySettingTextBox.Location = new System.Drawing.Point(6, 20);
            this.Game_CruiseBetMoneySettingTextBox.Multiline = true;
            this.Game_CruiseBetMoneySettingTextBox.Name = "Game_CruiseBetMoneySettingTextBox";
            this.Game_CruiseBetMoneySettingTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Game_CruiseBetMoneySettingTextBox.Size = new System.Drawing.Size(104, 216);
            this.Game_CruiseBetMoneySettingTextBox.TabIndex = 84;
            this.Game_CruiseBetMoneySettingTextBox.Text = resources.GetString("Game_CruiseBetMoneySettingTextBox.Text");
            this.Game_CruiseBetMoneySettingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Game_CruiseBetListView
            // 
            this.Game_CruiseBetListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader146,
            this.columnHeader145,
            this.columnHeader14});
            this.Game_CruiseBetListView.FullRowSelect = true;
            this.Game_CruiseBetListView.GridLines = true;
            this.Game_CruiseBetListView.HideSelection = false;
            this.Game_CruiseBetListView.Location = new System.Drawing.Point(116, 20);
            this.Game_CruiseBetListView.Name = "Game_CruiseBetListView";
            this.Game_CruiseBetListView.Size = new System.Drawing.Size(496, 216);
            this.Game_CruiseBetListView.TabIndex = 0;
            this.Game_CruiseBetListView.UseCompatibleStateImageBehavior = false;
            this.Game_CruiseBetListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "단계";
            this.columnHeader11.Width = 59;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "1차 배팅";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader12.Width = 70;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "2차 배팅";
            this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader13.Width = 70;
            // 
            // columnHeader146
            // 
            this.columnHeader146.Text = "3단 배팅";
            this.columnHeader146.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader146.Width = 70;
            // 
            // columnHeader145
            // 
            this.columnHeader145.Text = "총배팅액";
            this.columnHeader145.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader145.Width = 80;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "이익금";
            this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader14.Width = 80;
            // 
            // virtualMoney
            // 
            this.virtualMoney.AutoSize = true;
            this.virtualMoney.Location = new System.Drawing.Point(18, 245);
            this.virtualMoney.Name = "virtualMoney";
            this.virtualMoney.Size = new System.Drawing.Size(88, 16);
            this.virtualMoney.TabIndex = 107;
            this.virtualMoney.Text = "가상 보유금";
            this.virtualMoney.UseVisualStyleBackColor = true;
            this.virtualMoney.Visible = false;
            // 
            // lblNowMoney
            // 
            this.lblNowMoney.AutoSize = true;
            this.lblNowMoney.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNowMoney.ForeColor = System.Drawing.Color.Black;
            this.lblNowMoney.Location = new System.Drawing.Point(5, 5);
            this.lblNowMoney.Margin = new System.Windows.Forms.Padding(5);
            this.lblNowMoney.Name = "lblNowMoney";
            this.lblNowMoney.Size = new System.Drawing.Size(65, 12);
            this.lblNowMoney.TabIndex = 11;
            this.lblNowMoney.Text = "보유 금액 :";
            // 
            // lblTxtNowMoney
            // 
            this.lblTxtNowMoney.AutoSize = true;
            this.lblTxtNowMoney.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTxtNowMoney.ForeColor = System.Drawing.Color.Black;
            this.lblTxtNowMoney.Location = new System.Drawing.Point(80, 5);
            this.lblTxtNowMoney.Margin = new System.Windows.Forms.Padding(5);
            this.lblTxtNowMoney.Name = "lblTxtNowMoney";
            this.lblTxtNowMoney.Size = new System.Drawing.Size(11, 12);
            this.lblTxtNowMoney.TabIndex = 12;
            this.lblTxtNowMoney.Text = "0";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Game_CruiseBetMoneyPercentSettingComboBox);
            this.panel1.Controls.Add(this.serverCheckTimeLabel);
            this.panel1.Controls.Add(this.lblNowMoney);
            this.panel1.Controls.Add(this.lblTxtNowMoney);
            this.panel1.Location = new System.Drawing.Point(1, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(937, 28);
            this.panel1.TabIndex = 13;
            // 
            // Game_CruiseBetMoneyPercentSettingComboBox
            // 
            this.Game_CruiseBetMoneyPercentSettingComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.Game_CruiseBetMoneyPercentSettingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Game_CruiseBetMoneyPercentSettingComboBox.FormattingEnabled = true;
            this.Game_CruiseBetMoneyPercentSettingComboBox.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "110",
            "120",
            "130",
            "140",
            "150",
            "160",
            "170",
            "180",
            "190",
            "200",
            "300",
            "400",
            "500",
            "600",
            "700",
            "800",
            "900",
            "1000"});
            this.Game_CruiseBetMoneyPercentSettingComboBox.Location = new System.Drawing.Point(872, 3);
            this.Game_CruiseBetMoneyPercentSettingComboBox.Name = "Game_CruiseBetMoneyPercentSettingComboBox";
            this.Game_CruiseBetMoneyPercentSettingComboBox.Size = new System.Drawing.Size(59, 20);
            this.Game_CruiseBetMoneyPercentSettingComboBox.TabIndex = 85;
            this.Game_CruiseBetMoneyPercentSettingComboBox.SelectedIndexChanged += new System.EventHandler(this.Game_CruiseBetMoneyPercentSettingComboBox_SelectedIndexChanged);
            // 
            // serverCheckTimeLabel
            // 
            this.serverCheckTimeLabel.AutoSize = true;
            this.serverCheckTimeLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.serverCheckTimeLabel.Location = new System.Drawing.Point(163, 5);
            this.serverCheckTimeLabel.Name = "serverCheckTimeLabel";
            this.serverCheckTimeLabel.Size = new System.Drawing.Size(0, 12);
            this.serverCheckTimeLabel.TabIndex = 111;
            this.serverCheckTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Brown;
            this.label1.Location = new System.Drawing.Point(112, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "||||||||||||||||||||";
            this.label1.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(1, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1895, 949);
            this.tabControl1.TabIndex = 110;
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Controls.Add(this.betpage5);
            this.tabPage4.Controls.Add(this.betpage6);
            this.tabPage4.Controls.Add(this.betpage7);
            this.tabPage4.Controls.Add(this.betpage8);
            this.tabPage4.Controls.Add(this.betpage3);
            this.tabPage4.Controls.Add(this.betpage4);
            this.tabPage4.Controls.Add(this.betpage2);
            this.tabPage4.Controls.Add(this.betpage1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1887, 923);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "배팅 페이지";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // betpage3
            // 
            this.betpage3.AutoScroll = true;
            this.betpage3.Location = new System.Drawing.Point(939, 417);
            this.betpage3.Name = "betpage3";
            this.betpage3.Size = new System.Drawing.Size(926, 405);
            this.betpage3.TabIndex = 125;
            // 
            // betpage4
            // 
            this.betpage4.AutoScroll = true;
            this.betpage4.Location = new System.Drawing.Point(7, 417);
            this.betpage4.Name = "betpage4";
            this.betpage4.Size = new System.Drawing.Size(926, 405);
            this.betpage4.TabIndex = 124;
            // 
            // betpage2
            // 
            this.betpage2.AutoScroll = true;
            this.betpage2.Location = new System.Drawing.Point(939, 6);
            this.betpage2.Name = "betpage2";
            this.betpage2.Size = new System.Drawing.Size(926, 405);
            this.betpage2.TabIndex = 123;
            // 
            // betpage1
            // 
            this.betpage1.AutoScroll = true;
            this.betpage1.Location = new System.Drawing.Point(7, 6);
            this.betpage1.Name = "betpage1";
            this.betpage1.Size = new System.Drawing.Size(926, 405);
            this.betpage1.TabIndex = 122;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox25);
            this.tabPage3.Controls.Add(this.clearLevelSet);
            this.tabPage3.Controls.Add(this.virtualMoney);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1873, 923);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "::::::::::::::";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // clearLevelSet
            // 
            this.clearLevelSet.BackColor = System.Drawing.SystemColors.Window;
            this.clearLevelSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clearLevelSet.FormattingEnabled = true;
            this.clearLevelSet.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60",
            "61",
            "62",
            "63",
            "64",
            "65",
            "66",
            "67",
            "68",
            "69",
            "70",
            "71",
            "72",
            "73",
            "74",
            "75",
            "76",
            "77",
            "78",
            "79",
            "80",
            "81",
            "82",
            "83",
            "84",
            "85",
            "86",
            "87",
            "88",
            "89",
            "90",
            "91",
            "92",
            "93",
            "94",
            "95",
            "96",
            "97",
            "98",
            "99",
            "100"});
            this.clearLevelSet.Location = new System.Drawing.Point(7, 6);
            this.clearLevelSet.Name = "clearLevelSet";
            this.clearLevelSet.Size = new System.Drawing.Size(42, 20);
            this.clearLevelSet.TabIndex = 120;
            this.clearLevelSet.Visible = false;
            // 
            // betpage5
            // 
            this.betpage5.AutoScroll = true;
            this.betpage5.Location = new System.Drawing.Point(939, 1239);
            this.betpage5.Name = "betpage5";
            this.betpage5.Size = new System.Drawing.Size(926, 405);
            this.betpage5.TabIndex = 129;
            // 
            // betpage6
            // 
            this.betpage6.AutoScroll = true;
            this.betpage6.Location = new System.Drawing.Point(7, 1239);
            this.betpage6.Name = "betpage6";
            this.betpage6.Size = new System.Drawing.Size(926, 405);
            this.betpage6.TabIndex = 128;
            // 
            // betpage7
            // 
            this.betpage7.AutoScroll = true;
            this.betpage7.Location = new System.Drawing.Point(939, 828);
            this.betpage7.Name = "betpage7";
            this.betpage7.Size = new System.Drawing.Size(926, 405);
            this.betpage7.TabIndex = 127;
            // 
            // betpage8
            // 
            this.betpage8.AutoScroll = true;
            this.betpage8.Location = new System.Drawing.Point(7, 828);
            this.betpage8.Name = "betpage8";
            this.betpage8.Size = new System.Drawing.Size(926, 405);
            this.betpage8.TabIndex = 126;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1899, 991);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RedMango For EveryOne | Userid | UserIP | Version";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox25;
        private System.Windows.Forms.TextBox Game_CruiseBetMoneySettingTextBox;
        private System.Windows.Forms.ListView Game_CruiseBetListView;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader146;
        private System.Windows.Forms.ColumnHeader columnHeader145;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.Label lblNowMoney;
        private System.Windows.Forms.Label lblTxtNowMoney;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox virtualMoney;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label serverCheckTimeLabel;
        private System.Windows.Forms.ComboBox clearLevelSet;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox Game_CruiseBetMoneyPercentSettingComboBox;
        private betpage betpage1;
        private betpage betpage2;
        private betpage betpage3;
        private betpage betpage4;
        private betpage betpage5;
        private betpage betpage6;
        private betpage betpage7;
        private betpage betpage8;
    }
}

