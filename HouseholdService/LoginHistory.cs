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

namespace HouseholdService
{
    public partial class LoginHistory : Form
    {
        private List<LoginRecord> loginRecords;

        public LoginHistory()
        {
            InitializeComponent();
            dataGridView1.Columns.Clear(); 
            dataGridView1.Columns.Add("Timestamp", "Время");
            dataGridView1.Columns.Add("Username", "Логин");
            dataGridView1.Columns.Add("Status", "Статус");
            LoadLoginHistory();
            Color color = Color.FromArgb(0, 123, 255);
            this.BackColor = color;
            PopulateDataGridView();

        }

        private void LoadLoginHistory()
        {
            loginRecords = new List<LoginRecord>();
            if (System.IO.File.Exists("login_history.txt"))
            {
                var lines = System.IO.File.ReadAllLines("login_history.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        loginRecords.Add(new LoginRecord
                        {
                            Timestamp = DateTime.Parse(parts[0].Trim()),
                            Username = parts[1].Trim(),
                            IsSuccessful = parts[2].Trim() == "Успешно"
                        });
                    }
                }
            }
        }

        private void PopulateDataGridView()
        {
            dataGridView1.Rows.Clear();
            foreach (var record in loginRecords)
            {
                dataGridView1.Rows.Add(record.Timestamp, record.Username, record.IsSuccessful ? "Успешно" : "Ошибка");
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string filterUsername = textBox1.Text.Trim();
            var filteredRecords = loginRecords
                .Where(r => r.Username.IndexOf(filterUsername, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(r => r.Timestamp)
                .ToList();

            dataGridView1.Rows.Clear();
            foreach (var record in filteredRecords)
            {
                dataGridView1.Rows.Add(record.Timestamp, record.Username, record.IsSuccessful ? "Успешно" : "Ошибка");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
