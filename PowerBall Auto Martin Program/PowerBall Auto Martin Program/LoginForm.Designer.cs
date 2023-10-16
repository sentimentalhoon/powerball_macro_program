using System.Drawing;

namespace PowerBallAutoMartinProgram
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this._programClosed = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.userRegist = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.programUpdatePanel = new System.Windows.Forms.Panel();
            this.programUpdateButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.fileDownLoadProgressBar = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.UserUrlAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this._programVersionLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.agreePanel = new System.Windows.Forms.Panel();
            this.agreeButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.programUpdatePanel.SuspendLayout();
            this.agreePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _programClosed
            // 
            this._programClosed.AutoSize = true;
            this._programClosed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._programClosed.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._programClosed.ForeColor = System.Drawing.Color.Transparent;
            this._programClosed.Location = new System.Drawing.Point(519, 9);
            this._programClosed.Name = "_programClosed";
            this._programClosed.Size = new System.Drawing.Size(36, 36);
            this._programClosed.TabIndex = 10;
            this._programClosed.Text = "X";
            this._programClosed.Click += new System.EventHandler(this._programClosed_Click);
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OK.ForeColor = System.Drawing.Color.White;
            this.OK.Location = new System.Drawing.Point(337, 248);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(119, 35);
            this.OK.TabIndex = 2;
            this.OK.Text = "접속하기";
            this.OK.UseVisualStyleBackColor = false;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.BackColor = System.Drawing.Color.LightGray;
            this.PasswordTextBox.Font = new System.Drawing.Font("굴림", 16F);
            this.PasswordTextBox.Location = new System.Drawing.Point(149, 176);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(307, 32);
            this.PasswordTextBox.TabIndex = 1;
            this.PasswordTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasswordTextBox_KeyDown);
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.BackColor = System.Drawing.Color.LightGray;
            this.UsernameTextBox.Font = new System.Drawing.Font("굴림", 16F);
            this.UsernameTextBox.Location = new System.Drawing.Point(149, 135);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(307, 32);
            this.UsernameTextBox.TabIndex = 0;
            this.UsernameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UsernameTextBox_KeyDown);
            // 
            // userRegist
            // 
            this.userRegist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(105)))), ((int)(((byte)(30)))));
            this.userRegist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.userRegist.ForeColor = System.Drawing.Color.White;
            this.userRegist.Location = new System.Drawing.Point(201, 248);
            this.userRegist.Name = "userRegist";
            this.userRegist.Size = new System.Drawing.Size(119, 35);
            this.userRegist.TabIndex = 5;
            this.userRegist.Text = "회원가입";
            this.userRegist.UseVisualStyleBackColor = false;
            this.userRegist.Visible = false;
            this.userRegist.Click += new System.EventHandler(this.userRegist_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.panel1.Controls.Add(this.programUpdatePanel);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.UserUrlAddress);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.userRegist);
            this.panel1.Controls.Add(this.rememberMe);
            this.panel1.Controls.Add(this.OK);
            this.panel1.Controls.Add(this.UsernameTextBox);
            this.panel1.Controls.Add(this.PasswordTextBox);
            this.panel1.Location = new System.Drawing.Point(45, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(486, 331);
            this.panel1.TabIndex = 7;
            // 
            // programUpdatePanel
            // 
            this.programUpdatePanel.Controls.Add(this.programUpdateButton);
            this.programUpdatePanel.Controls.Add(this.textBox1);
            this.programUpdatePanel.Controls.Add(this.fileDownLoadProgressBar);
            this.programUpdatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.programUpdatePanel.Location = new System.Drawing.Point(0, 0);
            this.programUpdatePanel.Name = "programUpdatePanel";
            this.programUpdatePanel.Size = new System.Drawing.Size(486, 331);
            this.programUpdatePanel.TabIndex = 12;
            this.programUpdatePanel.Visible = false;
            // 
            // programUpdateButton
            // 
            this.programUpdateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(170)))), ((int)(((byte)(51)))));
            this.programUpdateButton.Font = new System.Drawing.Font("굴림", 9F);
            this.programUpdateButton.ForeColor = System.Drawing.Color.Black;
            this.programUpdateButton.Location = new System.Drawing.Point(26, 176);
            this.programUpdateButton.Name = "programUpdateButton";
            this.programUpdateButton.Size = new System.Drawing.Size(430, 69);
            this.programUpdateButton.TabIndex = 2;
            this.programUpdateButton.Text = "업데이트 시작";
            this.programUpdateButton.UseVisualStyleBackColor = false;
            this.programUpdateButton.Click += new System.EventHandler(this.programUpdateButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(4)))), ((int)(((byte)(37)))));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(26, 49);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(430, 109);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "\r\n\r\n\r\n프로그램이 업데이트 되었습니다.\r\n\r\n버튼을 눌러 업데이트를 진행하여 주시기 바랍니다.";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fileDownLoadProgressBar
            // 
            this.fileDownLoadProgressBar.Location = new System.Drawing.Point(26, 263);
            this.fileDownLoadProgressBar.Name = "fileDownLoadProgressBar";
            this.fileDownLoadProgressBar.Size = new System.Drawing.Size(430, 48);
            this.fileDownLoadProgressBar.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "사용자 비밀번호";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "사용자 아이디";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "사용자 주소";
            // 
            // UserUrlAddress
            // 
            this.UserUrlAddress.BackColor = System.Drawing.Color.LightGray;
            this.UserUrlAddress.Font = new System.Drawing.Font("굴림", 16F);
            this.UserUrlAddress.Location = new System.Drawing.Point(149, 94);
            this.UserUrlAddress.Name = "UserUrlAddress";
            this.UserUrlAddress.Size = new System.Drawing.Size(307, 32);
            this.UserUrlAddress.TabIndex = 8;
            this.UserUrlAddress.Text = "http://www.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(33, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 18);
            this.label1.TabIndex = 7;
            this.label1.Text = "Sign in with your ID";
            // 
            // rememberMe
            // 
            this.rememberMe.AutoSize = true;
            this.rememberMe.Checked = true;
            this.rememberMe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rememberMe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rememberMe.ForeColor = System.Drawing.Color.White;
            this.rememberMe.Location = new System.Drawing.Point(37, 250);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(113, 19);
            this.rememberMe.TabIndex = 6;
            this.rememberMe.Text = "아이디 기억하기";
            this.rememberMe.UseVisualStyleBackColor = true;
            this.rememberMe.Visible = false;
            // 
            // _programVersionLabel
            // 
            this._programVersionLabel.AutoSize = true;
            this._programVersionLabel.Location = new System.Drawing.Point(12, 18);
            this._programVersionLabel.Name = "_programVersionLabel";
            this._programVersionLabel.Size = new System.Drawing.Size(96, 12);
            this._programVersionLabel.TabIndex = 11;
            this._programVersionLabel.Text = "ProgramVersion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(83, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(324, 180);
            this.label2.TabIndex = 12;
            this.label2.Text = "해당 프로그램은 파워볼 이용에 도움을 주기 위해 \r\n\r\n만들어진 무료 프로그램입니다.\r\n\r\n프로그램 사용 도중 발생하는\r\n\r\n컴퓨터 에러 및 다운" +
    " 현상 그외의 문제에 대해서는\r\n\r\n책임지지 않습니다.\r\n\r\n반드시 모니터링하여 주십시요!\r\n\r\n해당 프로그램은 Windows 10 에서 정상적" +
    "으로 작동합니다.\r\n\r\n 미만의 작동은 보장하지 않습니다.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // agreePanel
            // 
            this.agreePanel.Controls.Add(this.agreeButton);
            this.agreePanel.Controls.Add(this.label2);
            this.agreePanel.Location = new System.Drawing.Point(45, 448);
            this.agreePanel.Name = "agreePanel";
            this.agreePanel.Size = new System.Drawing.Size(486, 271);
            this.agreePanel.TabIndex = 8;
            // 
            // agreeButton
            // 
            this.agreeButton.ForeColor = System.Drawing.Color.Black;
            this.agreeButton.Location = new System.Drawing.Point(357, 238);
            this.agreeButton.Name = "agreeButton";
            this.agreeButton.Size = new System.Drawing.Size(99, 22);
            this.agreeButton.TabIndex = 13;
            this.agreeButton.Text = "확인";
            this.agreeButton.UseVisualStyleBackColor = true;
            this.agreeButton.Visible = false;
            this.agreeButton.Click += new System.EventHandler(this.agreeButton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(567, 767);
            this.Controls.Add(this.agreePanel);
            this.Controls.Add(this._programVersionLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._programClosed);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "안녕하세요. 파워볼 배팅 프로그램입니다.";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.programUpdatePanel.ResumeLayout(false);
            this.programUpdatePanel.PerformLayout();
            this.agreePanel.ResumeLayout(false);
            this.agreePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label _programClosed;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Button userRegist;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _programVersionLabel;
        private System.Windows.Forms.CheckBox rememberMe;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel agreePanel;
        private System.Windows.Forms.Button agreeButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UserUrlAddress;
        private System.Windows.Forms.Panel programUpdatePanel;
        private System.Windows.Forms.Button programUpdateButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ProgressBar fileDownLoadProgressBar;
    }
}