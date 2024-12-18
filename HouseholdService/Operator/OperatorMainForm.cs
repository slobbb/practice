using DatabaseController;
using DatabaseController.Models;
using HouseholdService.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseholdService.Operator
{
    public partial class OperatorMainForm : Form
    {
        int operatorID;
        DatabasController controller;
        Color color = Color.FromArgb(0, 123, 255);
        public OperatorMainForm(DatabasController controller, int operatorID)
        {
            InitializeComponent();
            this.controller = controller;
            this.operatorID = operatorID;

            BackColor = color;
        }

        private void OperatorMainForm_Load(object sender, EventArgs e)
        {
            User user = controller.getUserData(operatorID);
            label1.Text = user.name.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var form = new OperatorAllRequestForm(controller))
            {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
