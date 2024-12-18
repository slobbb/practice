using DatabaseController;
using DatabaseController.Models;
using HouseholdService.Manager;
using HouseholdService.Master;
using HouseholdService.Operator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HouseholdService
{
    public partial class Form1 : Form
    {
        public string connectionString = "Server=DESKTOP-3A8HJSP\\SQLEXPRESS;Database=HouseholdService;Trusted_Connection=True;TrustServerCertificate = true;";
        DatabasController controller;
        string captchaText;
        int failedAttempts = 0;
        Timer lockoutTimer;

        public Form1()
        {
            InitializeComponent();
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;

            controller = new DatabasController(connectionString);
            GenerateCaptcha();

            lockoutTimer = new Timer();
            lockoutTimer.Interval = 180000;
            lockoutTimer.Tick += LockoutTimer_Tick;

            txtUser.Visible = false;
            picCaptcha.Visible = false;
            btnGenerate.Visible = false;
            textBoxPassword.UseSystemPasswordChar = true;
            btnShowHidePassword.Text = "Показать";
        }

        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            lockoutTimer.Stop();
            failedAttempts = 0;
            MessageBox.Show("Вы можете попробовать войти снова.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtUser.Visible = false;
            picCaptcha.Visible = false;
            btnGenerate.Visible = false;
        }

        private void GenerateCaptcha()
        {
            captchaText = DatabasController.GenerateRandomString(4);
            picCaptcha.Image = GenerateCaptchaImage(captchaText, picCaptcha.Width, picCaptcha.Height);
            txtUser.Clear();
        }

        private bool VerifyCaptcha()
        {
            return txtUser.Text.Equals(captchaText, StringComparison.OrdinalIgnoreCase);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (failedAttempts >= 2)
            {
                MessageBox.Show("Система заблокирована. Пожалуйста, подождите 3 минуты.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (failedAttempts > 0)
            {
                if (!VerifyCaptcha())
                {
                    MessageBox.Show("Капча не пройдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    GenerateCaptcha();
                    return;
                }
            }

            KeyValuePair<int, int>? authResult = controller.Authorization(textBoxLogin.Text, textBoxPassword.Text);
            if (authResult != null)
            {
                LogLoginAttempt(textBoxLogin.Text, true);
                int userID = authResult.Value.Key;
                int userTypeID = authResult.Value.Value;

                switch (userTypeID)
                {
                    case 1:
                        using ( var form = new ManagerMainForm(controller, userID))
                        {
                            Hide();
                            form.ShowDialog();
                            Show();
                        }
                        break;

                    case 2:
                        using (var form = new MasterMainForm(controller, userID))
                        {
                            Hide();
                            form.ShowDialog();
                            Show();
                        }
                        break;

                    case 3:
                        using (var form = new OperatorMainForm(controller, userID))
                        {
                            Hide();
                            form.ShowDialog();
                            Show();
                        }
                        break;

                    case 4:
                        using (var form = new CustomerMainForm(controller, userID))
                        {
                            Hide();
                            form.ShowDialog();
                            Show();
                        }
                        break;

                    default:
                        MessageBox.Show("Неизвестный тип пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            else
            {
                LogLoginAttempt(textBoxLogin.Text, false);
                failedAttempts++;
                MessageBox.Show("Неправильный логин или пароль!", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (failedAttempts == 1)
                {
                    GenerateCaptcha();
                    txtUser.Visible = true;
                    picCaptcha.Visible = true;
                    btnGenerate.Visible = true;
                }
                else if (failedAttempts == 2)
                {
                    MessageBox.Show("Вы совершили 2 неудачных попытки входа. Система заблокирована на 3 минуты.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lockoutTimer.Start();
                }
            }
        }

        private Bitmap GenerateCaptchaImage(string text, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Font font = new Font("Arial", 24, FontStyle.Bold);
                Brush brush = new SolidBrush(Color.Black);
                g.DrawString(text, font, brush, new PointF(10, 10));

                Random rand = new Random();
                for (int i = 0; i < 8000; i++)
                {
                    g.FillEllipse(Brushes.LightGray, rand.Next(0, width), rand.Next(0, height), 2, 2);
                }
            }
            return bmp;
        }


        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }

        private void btnShowHidePassword_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.UseSystemPasswordChar)
            {
                textBoxPassword.UseSystemPasswordChar = false; 
                btnShowHidePassword.Text = "Скрыть"; 
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = true; 
                btnShowHidePassword.Text = "Показать";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var form = new LoginHistory())
            {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void LogLoginAttempt(string username, bool isSuccess)
        {
            var record = new LoginRecord
            {
                Timestamp = DateTime.Now,
                Username = username,
                IsSuccessful = isSuccess
            };

            string logEntry = record.ToString();
            System.IO.File.AppendAllText("login_history.txt", logEntry + Environment.NewLine);
        }
    }
}