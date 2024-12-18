using DatabaseController;
using DatabaseController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HouseholdService.Manager
{
    public partial class ManagerRequests : Form
    {
        DatabasController controller;
        List<RequestModel> requests;
        List<Specialist> specialists;
        int selectedRequestID;
        Color color = Color.FromArgb(0, 123, 255);

        public ManagerRequests(DatabasController controller)
        {
            InitializeComponent();
            this.controller = controller;

            this.BackColor = color;
        }

        private void ManagerRequests_Load(object sender, EventArgs e)
        {
            requests = controller.GetAllRequests();
            LoadRequestsToDataGridView(requests);


            specialists = controller.GetMasters();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(specialists.Select(s => s.masterName).ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void LoadRequestsToDataGridView(List<RequestModel> requests)
        {
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("requestID", "ID Заявки");
                dataGridView1.Columns.Add("startDate", "Дата начала");
                dataGridView1.Columns.Add("completionDate", "Дата завершения");
                dataGridView1.Columns.Add("problemDescription", "Описание проблемы");
                dataGridView1.Columns.Add("requestStatus", "Статус заявки");
                dataGridView1.Columns.Add("homeTechModel", "Модель техники");
                dataGridView1.Columns.Add("customer", "Клиент");
                dataGridView1.Columns.Add("maser", "Мастер");
            }
            dataGridView1.Rows.Clear();
            foreach (var request in requests)
            {
                dataGridView1.Rows.Add(
                    request.requestID,
                    request.startDate,
                    request.completionDate.HasValue ? request.completionDate.Value.ToString("g") : "Не завершено",
                    request.problemDescription,
                    request.requestStatus.Value,
                    request.homeTechModel.Value,
                    request.customer.Value,
                    request.master.HasValue ? request.master.Value.Value : "Нет"
                );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите заявку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            selectedRequestID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["requestID"].Value);

            if (controller.ManagerDelete(selectedRequestID))
            {
                MessageBox.Show("Заявка успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                requests = controller.GetAllRequests();
                LoadRequestsToDataGridView(requests);
            }
            else
            {
                MessageBox.Show("Ошибка при удалении.", "Неудача", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string selectedMasterName = comboBox1.SelectedItem.ToString();

            DateTime completionDate = dateTimePicker1.Value;

            int masterID = specialists.First(s => s.masterName == selectedMasterName).masterID;

            selectedRequestID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["requestID"].Value);
            controller.UpdateManagerRequest(selectedRequestID, completionDate, masterID);

            requests = controller.GetAllRequests();
            LoadRequestsToDataGridView(requests);

            MessageBox.Show("Изменения сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}