using DatabaseController;
using DatabaseController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HouseholdService
{
    public partial class CustomerCreateRequest : Form
    {
        DatabasController controller;
        int customerID;
        List<RequestModel> requests = new List<RequestModel>();
        List<TechType> types = new List<TechType>();
        List<Model> models = new List<Model>();
        int selectedModelID; 

        public CustomerCreateRequest(DatabasController controller, int customerID)
        {
            InitializeComponent();

            this.controller = controller;
            this.customerID = customerID;
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;
            
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            
        }

        private void CustomerCreateRequest_Load(object sender, EventArgs e)
        {
            types = controller.GetTypesData();

            if (types != null && types.Count > 0)
            {
                for (int i = 0; i < types.Count; i++)
                {
                    comboBox1.Items.Add(types[i].techType);
                }
            }

            models = controller.GetModelData();

            if (models != null && models.Count > 0)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    comboBox2.Items.Add(models[i].techModel);
                }
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= types.Count)
            {
                return;
            }

            int selectedTechTypeID = types[comboBox1.SelectedIndex].techTypeID;

            comboBox2.Items.Clear();

            var filteredModels = models.Where(model => model.techTypeID == selectedTechTypeID).ToList();

            foreach (var model in filteredModels)
            {
                comboBox2.Items.Add(model.techModel);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем текст выбранного элемента в comboBox2
            string selectedModelName = comboBox2.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedModelName))
            {
                // Находим модель по имени
                var selectedModel = models.FirstOrDefault(model => model.techModel == selectedModelName);
                if (selectedModel != null)
                {
                    selectedModelID = selectedModel.techModelID; // Сохраняем modelID
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (controller.InsertCustomerRequest(selectedModelID, richTextBox1.Text, customerID) == true)
                MessageBox.Show("Заявка успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Возникли проблемы с созданием заявки, попробуйте ещё раз.", "Неудача", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
