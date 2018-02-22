using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project
{
    public partial class Accountant : Form
    {
        SqlConnection sqlConnection;
        public Accountant()
        {
            InitializeComponent();
            SetStyleDataGridView(); 
        }

        private void SetStyleDataGridView()
        {
            //настройки для невыполненных договоров
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            //настройки для выполненных договоров
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            //настройки для всех договоров
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoResizeColumns();
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
        }

        private async void Accountant_Load(object sender, EventArgs e)
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
            AddButton();
        }

        private void AddButton()
        {
            DataGridViewButtonColumn buttonYes = new DataGridViewButtonColumn() { Name = "buttonSend", HeaderText = @"", Width = 600 };
            buttonYes.Text = @"Отправить квитанцию";
            buttonYes.UseColumnTextForButtonValue = true;
            buttonYes.Visible = true;
            dataGridView1.Columns.Add(buttonYes);

            DataGridViewButtonColumn buttonNo = new DataGridViewButtonColumn() { Name = "buttonPay", HeaderText = @"", Width = 300 };
            buttonNo.Text = @"Оплачен";
            buttonNo.UseColumnTextForButtonValue = true;
            buttonNo.Visible = true;
            dataGridView2.Columns.Add(buttonNo);
        }

        private void GetData()
        {
            try
            {
                string sqlExpression = "sp_SelectData";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@status", @"принят");
                dataAdapter.SelectCommand = command;
                DataTable table1 = new DataTable();
                dataAdapter.Fill(table1);
                dataGridView1.DataSource = table1;

                command = new SqlCommand(sqlExpression, sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@status", @"не оплачен");
                dataAdapter.SelectCommand = command;
                DataTable table2 = new DataTable();
                dataAdapter.Fill(table2);
                dataGridView2.DataSource = table2;


                command = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([sname],' ',[fname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[date] as [Дата],[status] as [Статус] FROM [Contracts];", sqlConnection);
                dataAdapter = new SqlDataAdapter(command);
                DataTable table3 = new DataTable();
                dataAdapter.Fill(table3);
                dataGridView3.DataSource = table3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Accountant_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            Form form = Application.OpenForms[0];
            form.Show();
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "buttonSend")
            {               
                try
                {
                    int id = 0;
                    if (e.ColumnIndex == 7)
                    {
                        id = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value);
                    }
                    else { id = Convert.ToInt32(dataGridView1[1, e.RowIndex].Value); }
                    string sqlExpression = "sp_UpdateStatus";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @"не оплачен");
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                GetData();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "buttonPay")
            {
                try
                {
                    int id = 0;
                    id = Convert.ToInt32(dataGridView2[1, e.RowIndex].Value);
                    string sqlExpression = "sp_UpdateStatus";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @"оплачен");
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                GetData();
            }
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
                            if (dataGridView1.Rows[i].Cells[1].Value != null)
                                if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox8.Text))
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
                            if (dataGridView1.Rows[i].Cells[2].Value != null)
                                if (dataGridView1.Rows[i].Cells[2].Value.ToString().Contains(textBox8.Text))
                                {
                                    dataGridView1.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                dataGridView2.Rows[i].Selected = false;
            }
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        for (int i = 0; i < dataGridView2.RowCount; i++)
                        {
                            if (dataGridView2.Rows[i].Cells[0].Value != null)
                                if (dataGridView2.Rows[i].Cells[0].Value.ToString().Contains(textBox1.Text))
                                {
                                    dataGridView2.Rows[i].Selected = true;
                                }
                            if (dataGridView2.Rows[i].Cells[1].Value != null)
                                if (dataGridView2.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text))
                                {
                                    dataGridView2.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
                case 1:
                    {
                        for (int i = 0; i < dataGridView2.RowCount; i++)
                        {
                            if (dataGridView2.Rows[i].Cells[1].Value != null)
                                if (dataGridView2.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text))
                                {
                                    dataGridView2.Rows[i].Selected = true;
                                }
                            if (dataGridView2.Rows[i].Cells[2].Value != null)
                                if (dataGridView2.Rows[i].Cells[2].Value.ToString().Contains(textBox1.Text))
                                {
                                    dataGridView2.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                dataGridView3.Rows[i].Selected = false;
            }
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    {
                        for (int i = 0; i < dataGridView3.RowCount; i++)
                        {
                            if (dataGridView3.Rows[i].Cells[0].Value != null)
                                if (dataGridView3.Rows[i].Cells[0].Value.ToString().Contains(textBox2.Text))
                                {
                                    dataGridView3.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
                case 1:
                    {
                        for (int i = 0; i < dataGridView3.RowCount; i++)
                        {
                            if (dataGridView3.Rows[i].Cells[1].Value != null)
                                if (dataGridView3.Rows[i].Cells[1].Value.ToString().Contains(textBox2.Text))
                                {
                                    dataGridView3.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i < dataGridView3.RowCount; i++)
                        {
                            if (dataGridView3.Rows[i].Cells[6].Value != null)
                                if (dataGridView3.Rows[i].Cells[6].Value.ToString().Contains(textBox2.Text))
                                {
                                    dataGridView3.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
            }
        }

    }
}
