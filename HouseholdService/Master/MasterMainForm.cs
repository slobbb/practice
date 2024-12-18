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

namespace HouseholdService.Master
{
    public partial class MasterMainForm : Form
    {
        int masterID;
        DatabasController controller;
        Color color = Color.FromArgb(0, 123, 255);

        public MasterMainForm(DatabasController controller, int masterID)
        {
            InitializeComponent();

            this.controller = controller;
            this.masterID = masterID;

            BackColor = color;
        }

        private void MasterMainForm_Load(object sender, EventArgs e)
        {
            User user = controller.getUserData(masterID);
            label1.Text = user.name.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var form = new MasterRequestsForm(controller, masterID))
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
