using QRCoder; // Не забудьте добавить ссылку на QRCoder
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HouseholdService.Customer
{
    public partial class FormQR : Form
    {
        public FormQR()
        {
            InitializeComponent();
            Color color = Color.FromArgb(0, 123, 255);
            BackColor = color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormQR_Load(object sender, EventArgs e)
        {
            string qrText = "https://www.google.com";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        Bitmap qrCodeImage = qrCode.GetGraphic(8);
                        pictureBox1.Image = qrCodeImage;
                    }
                }
            }
        }
    }
}