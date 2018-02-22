using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace project.Forms
{
    public partial class Leader : Form
    {
        SqlConnection sqlConnection;
        SqlDataAdapter dataAdapter;
        BindingSource bindingSource1 = new BindingSource();

        public Leader()
        {
            InitializeComponent();
            SetStyleDataGridView();
        }

        private void SetStyleDataGridView()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;


        }

        private async void Leader_Load(object sender, EventArgs e)
        {
            try
            {
                //создание подключения к БД
                string connectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Database.mdf';Integrated Security=True";
                sqlConnection = new SqlConnection(connectionString);
                //открытие ассинхронного подключения к БД, чтобы не тормозил пользовательский интерфейс
                await sqlConnection.OpenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GetData();
        }

        private void GetData()
        {
            try
            {
                
                SqlCommand command = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([sname],' ',[fname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[date] as [Дата],[status] as [Статус] FROM [Contracts];", sqlConnection);
                dataAdapter = new SqlDataAdapter(command);
                DataTable table3 = new DataTable();
                dataAdapter.Fill(table3);
                dataGridView1.DataSource = table3;


                DateTime date1 = new DateTime();
                date1 = DateTime.Now;
                dataAdapter = new SqlDataAdapter();
                string sqlExpression = "sp_Manager";
                SqlCommand command1 = new SqlCommand(sqlExpression, sqlConnection);
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                command1.Parameters.AddWithValue("@date", date1.Month);
                dataAdapter.SelectCommand = command1;
                DataTable table2 = new DataTable();
                dataAdapter.Fill(table2);
                bindingSource1.DataSource = table2;
                dataGridView2.DataSource = bindingSource1;

                command = new SqlCommand("SELECT COUNT([Id_contract]) FROM [Contracts] WHERE [package]=N'VIP';", sqlConnection);
                textBox1.Text = command.ExecuteScalar().ToString();
                command = new SqlCommand("SELECT COUNT([Id_contract]) FROM [Contracts] WHERE [package]=N'стандарт';", sqlConnection);
                textBox2.Text = command.ExecuteScalar().ToString();
                command = new SqlCommand("SELECT COUNT([Id_contract]) FROM [Contracts] WHERE [package]=N'эконом';", sqlConnection);
                textBox3.Text = command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Leader_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            Form form = Application.OpenForms[0];
            form.Show();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Selected = false;
            }
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value != null)
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains(textBox8.Text))
                                {
                                    dataGridView1.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
                case 1:
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[1].Value != null)
                                if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox8.Text))
                                {
                                    dataGridView1.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[6].Value != null)
                                if (dataGridView1.Rows[i].Cells[6].Value.ToString().Contains(textBox8.Text))
                                {
                                    dataGridView1.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
            }
        }
    }
}
