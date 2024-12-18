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
    public partial class MasterAddCommentForm : Form
    {
        DatabasController controller;
        RequestModel request;
        public MasterAddCommentForm(DatabasController controller, RequestModel request)
        {
            InitializeComponent();
            this.controller = controller;
            this.request = request;
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (controller.AddComment(request.requestID, richTextBox1.Text) == true)
                MessageBox.Show("Комментарий успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Возникла ошибка, попробуйте снова!", "Неудача", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();    
        }
    }
}
