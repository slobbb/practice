using DatabaseController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseholdService.Manager
{
    public partial class ManagerMainForm : Form
    {
        DatabasController controller;
        Color color = Color.FromArgb(0, 123, 255);
        int managerID;

        public ManagerMainForm(DatabasController controller, int managerID)
        {
            InitializeComponent();
            this.controller = controller;

            this.managerID = managerID;

            BackColor = color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (ManagerRequests form = new ManagerRequests(controller)) {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void ManagerMainForm_Load(object sender, EventArgs e)
        {
            User user = controller.getUserData(managerID);
            label1.Text = user.name.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
