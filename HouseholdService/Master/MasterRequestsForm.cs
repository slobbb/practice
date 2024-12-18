using Azure.Core;
using DatabaseController;
using DatabaseController.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HouseholdService.Master
{
    public partial class MasterRequestsForm : Form
    {
        int masterID;
        DatabasController controller;
        List<RequestModel> allRequests; 
        public MasterRequestsForm(DatabasController controller, int masterID)
        {
            InitializeComponent();

            this.controller = controller;
            this.masterID = masterID;
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;
            LoadRequests();
        }

        private void LoadRequests()
        {
            allRequests = controller.GetRequestsMaster(masterID);
            label3.Text = $"{dataGridView1.Rows.Count} из {allRequests.Count}";
            SetupDataGridView();

            LoadDataGridView(allRequests);

            FillCustomerComboBox();
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("requestID", "ID Заявки");
            dataGridView1.Columns.Add("startDate", "Дата начала");
            dataGridView1.Columns.Add("completionDate", "Дата завершения");
            dataGridView1.Columns.Add("problemDescription", "Описание проблемы");
            dataGridView1.Columns.Add("requestStatus", "Статус заявки");
            dataGridView1.Columns.Add("homeTechModel", "Модель техники");
            dataGridView1.Columns.Add("customer", "Клиент");
            dataGridView1.Columns.Add("master", "Мастер");
        }

        private void LoadDataGridView(List<RequestModel> requests)
        {
            dataGridView1.Rows.Clear();
            foreach (var request in requests)
            {
                dataGridView1.Rows.Add(
                    request.requestID,
                    request.startDate.ToString("g"),
                    request.completionDate.HasValue ? request.completionDate.Value.ToString("g") : "Не завершено",
                    request.problemDescription,
                    request.requestStatus.Value,
                    request.homeTechModel.Value,
                    request.customer.Value,
                    request.master.HasValue ? request.master.Value.Value : "Нет"
                );
            }
            label3.Text = $"{dataGridView1.Rows.Count} из {allRequests.Count}";
        }

        private void FillCustomerComboBox()
        {
            var customers = allRequests
                .Select(r => r.customer.Value)
                .Distinct()
                .ToList();

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Все"); 
            comboBox1.Items.AddRange(customers.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCustomer = comboBox1.SelectedItem.ToString();

            var filteredRequests = selectedCustomer == "Все"
                ? allRequests
                : allRequests.Where(r => r.customer.Value == selectedCustomer).ToList();

            LoadDataGridView(filteredRequests);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                int requestID = (int)selectedRow.Cells["requestID"].Value;

                var selectedRequest = allRequests.FirstOrDefault(r => r.requestID == requestID);

                if (selectedRequest != null)
                {
                    MasterAddCommentForm addCommentForm = new MasterAddCommentForm(controller, selectedRequest);
                    addCommentForm.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("Не удалось найти выбранную заявку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для добавления комментария.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                int requestID = (int)selectedRow.Cells["requestID"].Value;

                var selectedRequest = allRequests.FirstOrDefault(r => r.requestID == requestID);

                if (selectedRequest != null)
                {
                    MasterOrderPartsForm orderParts = new MasterOrderPartsForm(controller, selectedRequest);
                    orderParts.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("Не удалось найти выбранную заявку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для добавления комментария.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                int requestID = (int)selectedRow.Cells["requestID"].Value;

                var selectedRequest = allRequests.FirstOrDefault(r => r.requestID == requestID);

                if (selectedRequest != null)
                {
                    MasterCreateReport createReport = new MasterCreateReport(controller, selectedRequest);
                    createReport.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Не удалось найти выбранную заявку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для добавления комментария.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}