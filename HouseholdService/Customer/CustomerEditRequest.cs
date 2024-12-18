using DatabaseController;
using DatabaseController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HouseholdService.Customer
{
    public partial class CustomerEditRequest : Form
    {
        DatabasController controller;
        int customerID;
        List<Model> models = new List<Model>();
        List<RequestModel> requests;
        Color color = Color.FromArgb(0, 123, 255);
        int selectedRequestID;
        public CustomerEditRequest(DatabasController controller, int customerID)
        {
            InitializeComponent();

            this.controller = controller;
            this.customerID = customerID;

            this.BackColor = color;
        }

        private void CustomerEditRequest_Load(object sender, EventArgs e)
        {
            models = controller.GetModelData();

            if (models != null && models.Count > 0)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    comboBox2.Items.Add(models[i].techModel);
                }
            }

            requests = controller.GetRequestsCustomer(customerID);
            comboBox1.Items.Clear();
            for (int i = 0; i < requests.Count; i++)
                comboBox1.Items.Add(requests[i].requestID);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= requests.Count)
            {
                return;
            }

            selectedRequestID = (int)comboBox1.SelectedItem;

            RequestModel selectedRequest = requests.Find(request => request.requestID == selectedRequestID);

            if (selectedRequest != null)
            {
                label5.Text = $"Дата подачи: {selectedRequest.startDate}";

                label6.Text = $"Модель: {selectedRequest.homeTechModel.Value}";
                label7.Text = $"Описание проблемы: {selectedRequest.problemDescription}";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= requests.Count)
            {
                MessageBox.Show("Пожалуйста, выберите заявку для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            selectedRequestID = (int)comboBox1.SelectedItem;

            if (comboBox2.SelectedIndex < 0 || comboBox2.SelectedIndex >= models.Count)
            {
                MessageBox.Show("Пожалуйста, выберите модель.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int selectedModelID = models[comboBox2.SelectedIndex].techModelID;

            string problemDescription = requests.Find(request => request.requestID == selectedRequestID)?.problemDescription;

            bool success = controller.UpdateCustomerRequest(selectedRequestID, selectedModelID, richTextBox1.Text, problemDescription);

            if (success)
            {
                MessageBox.Show("Заявка успешно обновлена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Не удалось обновить заявку. Пожалуйста, попробуйте еще раз.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}