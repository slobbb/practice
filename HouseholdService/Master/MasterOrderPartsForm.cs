using Azure.Core;
using DatabaseController;
using DatabaseController.Models;
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
    public partial class MasterOrderPartsForm : Form
    {
        DatabasController controller;
        RequestModel RequestModel;
        public MasterOrderPartsForm(DatabasController controller, RequestModel requestModel)
        {
            InitializeComponent();
            this.controller = controller;
            this.RequestModel = requestModel;
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (controller.OrderParts(RequestModel.requestID, richTextBox1.Text) == true)
            {
                MessageBox.Show("Запчасти добавлены в заказ!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Возникла ошибка, попробуйте снова!", "Неудача", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
    }
}
