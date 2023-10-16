using System.Drawing;

namespace JY_PowerBallProgram
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
            this._programClosed = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.userRegist = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this._programVersionLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _programClosed
            // 
            this._programClosed.AutoSize = true;
            this._programClosed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._programClosed.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._programClosed.ForeColor = System.Drawing.Color.Transparent;
            this._programClosed.Location = new System.Drawing.Point(464, 9);
            this._programClosed.Name = "_programClosed";
            this._programClosed.Size = new System.Drawing.Size(36, 36);
            this._programClosed.TabIndex = 10;
            this._programClosed.Text = "X";
            this._programClosed.Click += new System.EventHandler(this._programClosed_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Georgia", 50.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(137, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 77);
            this.label3.TabIndex = 9;
            this.label3.Text = "Login";
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OK.ForeColor = System.Drawing.Color.White;
            this.OK.Location = new System.Drawing.Point(337, 184);
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
            this.PasswordTextBox.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.PasswordTextBox.Location = new System.Drawing.Point(37, 123);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(419, 35);
            this.PasswordTextBox.TabIndex = 1;
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.BackColor = System.Drawing.Color.LightGray;
            this.UsernameTextBox.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UsernameTextBox.Location = new System.Drawing.Point(37, 66);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(419, 35);
            this.UsernameTextBox.TabIndex = 0;
            // 
            // userRegist
            // 
            this.userRegist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.userRegist.ForeColor = System.Drawing.Color.White;
            this.userRegist.Location = new System.Drawing.Point(337, 225);
            this.userRegist.Name = "userRegist";
            this.userRegist.Size = new System.Drawing.Size(119, 35);
            this.userRegist.TabIndex = 5;
            this.userRegist.Text = "회원가입";
            this.userRegist.UseVisualStyleBackColor = true;
            this.userRegist.Click += new System.EventHandler(this.userRegist_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.userRegist);
            this.panel1.Controls.Add(this.rememberMe);
            this.panel1.Controls.Add(this.OK);
            this.panel1.Controls.Add(this.UsernameTextBox);
            this.panel1.Controls.Add(this.PasswordTextBox);
            this.panel1.Location = new System.Drawing.Point(15, 202);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(486, 293);
            this.panel1.TabIndex = 7;
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
            this.rememberMe.Location = new System.Drawing.Point(37, 186);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(113, 19);
            this.rememberMe.TabIndex = 6;
            this.rememberMe.Text = "아이디 기억하기";
            this.rememberMe.UseVisualStyleBackColor = true;
            // 
            // _programVersionLabel
            // 
            this._programVersionLabel.AutoSize = true;
            this._programVersionLabel.Location = new System.Drawing.Point(12, 9);
            this._programVersionLabel.Name = "_programVersionLabel";
            this._programVersionLabel.Size = new System.Drawing.Size(96, 12);
            this._programVersionLabel.TabIndex = 11;
            this._programVersionLabel.Text = "ProgramVersion";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(515, 865);
            this.Controls.Add(this._programVersionLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._programClosed);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "안녕하세요. 파워볼 배팅 프로그램입니다.";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label _programClosed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Button userRegist;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _programVersionLabel;
        private System.Windows.Forms.CheckBox rememberMe;
    }
}