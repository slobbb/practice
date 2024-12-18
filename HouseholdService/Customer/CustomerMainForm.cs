using System;
using System.Drawing;
using System.Windows.Forms;
using DatabaseController;
using HouseholdService.Customer;

namespace HouseholdService
{
    public partial class CustomerMainForm : Form
    {
        DatabasController controller;
        int customerID;

        public CustomerMainForm(DatabasController controller, int customerID)
        {
            InitializeComponent();
            this.controller = controller;
            this.customerID = customerID;
            Color color = Color.FromArgb(0, 123, 255);
            BackColor = color;
            User user = controller.getUserData(customerID);
            label1.Text = user.name.ToString();
        }

        private void CustomerMainForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerCreateRequest(controller, customerID))
            {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerEditRequest(controller, customerID))
            {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FormQR qr = new FormQR())
            {
                Hide();
                qr.ShowDialog();
                Show();
            }
        }
    }
}
