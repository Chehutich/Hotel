using Hotel.Core;
using Hotel.Localization;

namespace Hotel.Forms
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
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.loginBox = new System.Windows.Forms.GroupBox();
            this.commonFont = new System.Drawing.Font("Segoe UI", 12F);

            this.btnLanguage = new System.Windows.Forms.PictureBox();
            this.btnTheme = new System.Windows.Forms.PictureBox();
            this.pbShowPassword = new System.Windows.Forms.PictureBox(); // (НОВЕ)

            ((System.ComponentModel.ISupportInitialize)(this.btnLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTheme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbShowPassword)).BeginInit(); // (НОВЕ)

            // --- Налаштування форми ---
            this.Text = Strings.AppTitle;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(500, 350);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ThemeManager.FormBackground;
            this.FormClosed += (s, e) => { if (this.DialogResult != DialogResult.OK) Application.Exit(); };

            // --- GroupBox ---
            this.loginBox.SuspendLayout();
            this.Controls.Add(this.loginBox);

            this.loginBox.Text = Strings.Login_Title;
            this.loginBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.loginBox.Size = new System.Drawing.Size(400, 250);
            this.loginBox.Location = new System.Drawing.Point(44, 45);
            this.loginBox.ForeColor = ThemeManager.TextColor;

            // --- Елементи з фіксованими позиціями ---

            // lblUsername
            this.lblUsername.Text = Strings.Login_Username;
            this.lblUsername.Font = commonFont;
            this.lblUsername.ForeColor = ThemeManager.TextColor;
            this.lblUsername.Size = new System.Drawing.Size(120, 30);
            this.lblUsername.Location = new System.Drawing.Point(30, 60);
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // txtUsername
            this.txtUsername.Font = commonFont;
            this.txtUsername.Size = new System.Drawing.Size(210, 34);
            this.txtUsername.Location = new System.Drawing.Point(160, 60);
            this.txtUsername.BackColor = ThemeManager.InputBackground;
            this.txtUsername.ForeColor = ThemeManager.InputForeColor;

            // lblPassword
            this.lblPassword.Text = Strings.Login_Password;
            this.lblPassword.Font = commonFont;
            this.lblPassword.ForeColor = ThemeManager.TextColor;
            this.lblPassword.Size = new System.Drawing.Size(120, 30);
            this.lblPassword.Location = new System.Drawing.Point(30, 110);
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // txtPassword
            this.txtPassword.Font = commonFont;
            this.txtPassword.Size = new System.Drawing.Size(210, 34);
            this.txtPassword.Location = new System.Drawing.Point(160, 110);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.BackColor = ThemeManager.InputBackground;
            this.txtPassword.ForeColor = ThemeManager.InputForeColor;

            // (НОВЕ) pbShowPassword (Іконка ока)
            this.pbShowPassword.Location = new System.Drawing.Point(375, 115); // (X, Y) - праворуч від поля пароля
            this.pbShowPassword.Size = new System.Drawing.Size(24, 24);
            this.pbShowPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbShowPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbShowPassword.Click += new System.EventHandler(this.pbShowPassword_Click);

            // btnLogin
            this.btnLogin.Text = Strings.Login_Button;
            this.btnLogin.Font = commonFont;
            this.btnLogin.Size = new System.Drawing.Size(210, 40);
            this.btnLogin.Location = new System.Drawing.Point(160, 170);
            this.btnLogin.BackColor = ThemeManager.ButtonBackground;
            this.btnLogin.ForeColor = ThemeManager.ButtonForeColor;

            // Кнопка мови
            this.btnLanguage.Location = new System.Drawing.Point(450, 6);
            this.btnLanguage.Size = new System.Drawing.Size(32, 32);
            this.btnLanguage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLanguage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);

            // Кнопка теми
            this.btnTheme.Location = new System.Drawing.Point(412, 6);
            this.btnTheme.Size = new System.Drawing.Size(32, 32);
            this.btnTheme.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);

            // Додавання елементів
            this.loginBox.Controls.Add(this.lblUsername);
            this.loginBox.Controls.Add(this.txtUsername);
            this.loginBox.Controls.Add(this.lblPassword);
            this.loginBox.Controls.Add(this.txtPassword);
            this.loginBox.Controls.Add(this.btnLogin);
            this.loginBox.Controls.Add(this.pbShowPassword); // (НОВЕ) Додаємо "око" у GroupBox
            this.Controls.Add(this.btnLanguage);
            this.Controls.Add(this.btnTheme);

            // --- Обробники ---
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            this.AcceptButton = this.btnLogin;

            this.loginBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnLanguage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTheme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbShowPassword)).EndInit(); // (НОВЕ)
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox loginBox;
        private System.Drawing.Font commonFont;
        private System.Windows.Forms.PictureBox btnLanguage;
        private System.Windows.Forms.PictureBox btnTheme;
        private System.Windows.Forms.PictureBox pbShowPassword; // (НОВА ЗМІННА)
    }
}