namespace WaterMelonBettingProgram
{
    partial class UserLogin
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
            this.loginCheck = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.programUserPassword = new System.Windows.Forms.TextBox();
            this.programUserID = new System.Windows.Forms.TextBox();
            this.progarmLoginText = new System.Windows.Forms.Label();
            this.programUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // loginCheck
            // 
            this.loginCheck.Location = new System.Drawing.Point(608, 103);
            this.loginCheck.Name = "loginCheck";
            this.loginCheck.Size = new System.Drawing.Size(145, 126);
            this.loginCheck.TabIndex = 7;
            this.loginCheck.Text = "접속";
            this.loginCheck.UseVisualStyleBackColor = true;
            this.loginCheck.Click += new System.EventHandler(this.loginCheck_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 20F);
            this.label1.Location = new System.Drawing.Point(60, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 27);
            this.label1.TabIndex = 8;
            this.label1.Text = "사용자 아이디";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 20F);
            this.label2.Location = new System.Drawing.Point(60, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 27);
            this.label2.TabIndex = 9;
            this.label2.Text = "사용자 비밀번호";
            // 
            // programUserPassword
            // 
            this.programUserPassword.Font = new System.Drawing.Font("굴림", 20F);
            this.programUserPassword.Location = new System.Drawing.Point(276, 191);
            this.programUserPassword.Name = "programUserPassword";
            this.programUserPassword.Size = new System.Drawing.Size(310, 38);
            this.programUserPassword.TabIndex = 11;
            this.programUserPassword.UseSystemPasswordChar = true;
            // 
            // programUserID
            // 
            this.programUserID.Font = new System.Drawing.Font("굴림", 20F);
            this.programUserID.Location = new System.Drawing.Point(276, 147);
            this.programUserID.Name = "programUserID";
            this.programUserID.Size = new System.Drawing.Size(310, 38);
            this.programUserID.TabIndex = 12;
            // 
            // progarmLoginText
            // 
            this.progarmLoginText.AutoSize = true;
            this.progarmLoginText.ForeColor = System.Drawing.Color.DarkRed;
            this.progarmLoginText.Location = new System.Drawing.Point(346, 258);
            this.progarmLoginText.Name = "progarmLoginText";
            this.progarmLoginText.Size = new System.Drawing.Size(121, 12);
            this.progarmLoginText.TabIndex = 13;
            this.progarmLoginText.Text = "로그인이 필요합니다.";
            // 
            // programUrl
            // 
            this.programUrl.Font = new System.Drawing.Font("굴림", 20F);
            this.programUrl.Location = new System.Drawing.Point(276, 103);
            this.programUrl.Name = "programUrl";
            this.programUrl.Size = new System.Drawing.Size(310, 38);
            this.programUrl.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 20F);
            this.label3.Location = new System.Drawing.Point(60, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 27);
            this.label3.TabIndex = 14;
            this.label3.Text = "사용자 주소";
            // 
            // UserLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 290);
            this.Controls.Add(this.programUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progarmLoginText);
            this.Controls.Add(this.programUserID);
            this.Controls.Add(this.programUserPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loginCheck);
            this.Name = "UserLogin";
            this.Text = "WaterMelon PowerBall Support Program";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button loginCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox programUserPassword;
        private System.Windows.Forms.TextBox programUserID;
        private System.Windows.Forms.Label progarmLoginText;
        private System.Windows.Forms.TextBox programUrl;
        private System.Windows.Forms.Label label3;
    }
}