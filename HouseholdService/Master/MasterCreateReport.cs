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
    public partial class MasterCreateReport : Form
    {
        RequestModel requestModel;
        DatabasController controller;

        public MasterCreateReport(DatabasController controller, RequestModel request)
        {
            InitializeComponent();

            this.controller = controller;
            this.requestModel = request;
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = $"Отчет_заявка_{requestModel.requestID}.txt";
            string filePath = System.IO.Path.Combine(desktopPath, fileName);

            string reportContent = $"Номер заявки: {requestModel.requestID}\n" +
                                   $"Проделанная работа: {richTextBox1.Text}";

            try
            {
                System.IO.File.WriteAllText(filePath, reportContent);

                MessageBox.Show($"Отчет успешно создан: {filePath}", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
