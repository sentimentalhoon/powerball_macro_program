namespace AutoMartinProgram
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
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.textBoxMacAddress = new System.Windows.Forms.TextBox();
            this.textBoxIpAddress = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.noticeBox = new System.Windows.Forms.TextBox();
            this.loginFormPanel2 = new System.Windows.Forms.Panel();
            this._programClosed = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UsernameTextBox.Location = new System.Drawing.Point(196, 43);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(181, 44);
            this.UsernameTextBox.TabIndex = 0;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.PasswordTextBox.Location = new System.Drawing.Point(196, 95);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(181, 44);
            this.PasswordTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(28, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "아이디";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(28, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "비밀번호";
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49)))));
            this.OK.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OK.ForeColor = System.Drawing.Color.Transparent;
            this.OK.Location = new System.Drawing.Point(393, 43);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(119, 96);
            this.OK.TabIndex = 4;
            this.OK.Text = "접속";
            this.OK.UseVisualStyleBackColor = false;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // textBoxMacAddress
            // 
            this.textBoxMacAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49)))));
            this.textBoxMacAddress.ForeColor = System.Drawing.Color.Transparent;
            this.textBoxMacAddress.Location = new System.Drawing.Point(52, 798);
            this.textBoxMacAddress.Name = "textBoxMacAddress";
            this.textBoxMacAddress.ReadOnly = true;
            this.textBoxMacAddress.Size = new System.Drawing.Size(478, 21);
            this.textBoxMacAddress.TabIndex = 5;
            this.textBoxMacAddress.Text = "bbbbbbbbbbbbbbb";
            this.textBoxMacAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxIpAddress
            // 
            this.textBoxIpAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49)))));
            this.textBoxIpAddress.ForeColor = System.Drawing.Color.Transparent;
            this.textBoxIpAddress.Location = new System.Drawing.Point(52, 771);
            this.textBoxIpAddress.Name = "textBoxIpAddress";
            this.textBoxIpAddress.ReadOnly = true;
            this.textBoxIpAddress.Size = new System.Drawing.Size(478, 21);
            this.textBoxIpAddress.TabIndex = 6;
            this.textBoxIpAddress.Text = "aaaaaaaaaaaaaaaa";
            this.textBoxIpAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.UsernameTextBox);
            this.groupBox1.Controls.Add(this.PasswordTextBox);
            this.groupBox1.Controls.Add(this.OK);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(15, 478);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(597, 269);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "로그인 정보";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.noticeBox);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(15, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(597, 411);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "공지사항";
            // 
            // noticeBox
            // 
            this.noticeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49)))));
            this.noticeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.noticeBox.ForeColor = System.Drawing.Color.Transparent;
            this.noticeBox.Location = new System.Drawing.Point(34, 42);
            this.noticeBox.Multiline = true;
            this.noticeBox.Name = "noticeBox";
            this.noticeBox.ReadOnly = true;
            this.noticeBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.noticeBox.Size = new System.Drawing.Size(529, 346);
            this.noticeBox.TabIndex = 10;
            this.noticeBox.Text = "공 지 사 항";
            // 
            // loginFormPanel2
            // 
            this.loginFormPanel2.Location = new System.Drawing.Point(15, 478);
            this.loginFormPanel2.Name = "loginFormPanel2";
            this.loginFormPanel2.Size = new System.Drawing.Size(597, 287);
            this.loginFormPanel2.TabIndex = 0;
            // 
            // _programClosed
            // 
            this._programClosed.AutoSize = true;
            this._programClosed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._programClosed.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._programClosed.ForeColor = System.Drawing.Color.Transparent;
            this._programClosed.Location = new System.Drawing.Point(549, 22);
            this._programClosed.Name = "_programClosed";
            this._programClosed.Size = new System.Drawing.Size(63, 36);
            this._programClosed.TabIndex = 10;
            this._programClosed.Text = "종료";
            this._programClosed.Click += new System.EventHandler(this._programClosed_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49)))));
            this.ClientSize = new System.Drawing.Size(630, 835);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._programClosed);
            this.Controls.Add(this.textBoxMacAddress);
            this.Controls.Add(this.loginFormPanel2);
            this.Controls.Add(this.textBoxIpAddress);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "안녕하세요. 파워볼 배팅 프로그램입니다.";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.TextBox textBoxMacAddress;
        private System.Windows.Forms.TextBox textBoxIpAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel loginFormPanel2;
        private System.Windows.Forms.TextBox noticeBox;
        private System.Windows.Forms.Label _programClosed;
        private System.Windows.Forms.TextBox UsernameTextBox;
    }
}