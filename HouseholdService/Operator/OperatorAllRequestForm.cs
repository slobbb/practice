using DatabaseController;
using DatabaseController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HouseholdService.Operator
{
    public partial class OperatorAllRequestForm : Form
    {
        DatabasController controller;
        List<RequestModel> requestModels;
        List<Specialist> masters;
        List<RequestType> requestTypes;
        int selectedRowID;

        Color color = Color.FromArgb(0, 123, 255);

        public OperatorAllRequestForm(DatabasController controller)
        {
            InitializeComponent();
            this.controller = controller;

            BackColor = color;
        }

        private void OperatorAllRequestForm_Load(object sender, EventArgs e)
        {
            requestTypes = controller.GetStatuses();
            masters = controller.GetMasters();
            requestModels = controller.GetAllRequests();

            if (requestModels != null && requestModels.Count > 0)
            {
                var uniqueCustomers = requestModels
                    .Select(r => r.customer.Value)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                comboBox1.Items.Clear();
                comboBox1.Items.Add("Все");
                comboBox1.Items.AddRange(uniqueCustomers.ToArray());
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(requestTypes.Select(rt => rt.name).ToArray());
                comboBox3.SelectedIndex = 0;
                label7.Text = $"Среднее время на обработку: {DatabasController.CalculateAverageCompletionTime(requestModels)}";

                if (dataGridView1.Columns.Count == 0)
                {
                    dataGridView1.Columns.Add("requestID", "ID Заявки");
                    dataGridView1.Columns.Add("startDate", "Дата начала");
                    dataGridView1.Columns.Add("completionDate", "Дата завершения");
                    dataGridView1.Columns.Add("problemDescription", "Описание проблемы");
                    dataGridView1.Columns.Add("requestStatus", "Статус заявки");
                    dataGridView1.Columns.Add("homeTechModel", "Модель техники");
                    dataGridView1.Columns.Add("techType", "Тип техники");
                    dataGridView1.Columns.Add("customer", "Клиент");
                    dataGridView1.Columns.Add("master", "Мастер");
                }

                LoadDataGridView(requestModels);
                comboBox1.SelectedIndex = 0;
                label3.Text = $"{dataGridView1.Rows.Count} из {requestModels.Count}";

                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(masters.Select(m => m.masterName).ToArray());
                comboBox2.SelectedIndex = 0;

                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(requestTypes.Select(m => m.name).ToArray());
                comboBox3.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Нет доступных заявок.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadDataGridView(List<RequestModel> requests)
        {
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
                    request.homeTechModel.Key,
                    request.customer.Value,
                    request.master.HasValue ? request.master.Value.Value : "Нет"
                );
            }
            label3.Text = $"{dataGridView1.Rows.Count} из {requestModels.Count}";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCustomer = comboBox1.SelectedItem.ToString();
            List<RequestModel> filteredRequests;

            if (selectedCustomer == "Все")
            {
                filteredRequests = requestModels;
            }
            else
            {
                filteredRequests = requestModels
                    .Where(r => r.customer.Value != null && r.customer.Value.Trim().Equals(selectedCustomer.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            LoadDataGridView(filteredRequests);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                selectedRowID = Convert.ToInt32(selectedRow.Cells["requestID"].Value);

                if (selectedRow.Cells["completionDate"].Value != null &&
                    DateTime.TryParse(selectedRow.Cells["completionDate"].Value.ToString(), out DateTime completionDate))
                {
                    dateTimePicker1.Value = completionDate;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Now;
                }

                if (selectedRow.Cells["master"].Value != null)
                {
                    var masterName = selectedRow.Cells["master"].Value.ToString();
                    comboBox2.SelectedItem = masterName;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int selectedStatusID = requestTypes.First(rt => rt.name == comboBox3.SelectedItem.ToString()).id;

            int masterID = masters.First(m => m.masterName == comboBox2.SelectedItem.ToString()).masterID;

            DateTime completionDate = dateTimePicker1.Value;

            controller.UpdateOperatorRequest(selectedRowID, completionDate, masterID, selectedStatusID);

            requestModels = controller.GetAllRequests();
            LoadDataGridView(requestModels);

            MessageBox.Show("Изменения сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}