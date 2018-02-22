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
    public partial class Lawyer : Form
    {
        SqlConnection sqlConnection;
        private BindingSource bindingSource = new BindingSource();

        public Lawyer()
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

        private async void Lawyer_Load(object sender, EventArgs e)
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
            DataGridViewButtonColumn buttonYes = new DataGridViewButtonColumn() { Name = "buttonY", HeaderText = @"", Width = 300 };
            buttonYes.Text = @"Принят";
            buttonYes.UseColumnTextForButtonValue = true;
            buttonYes.Visible = true;
            dataGridView1.Columns.Add(buttonYes);

            DataGridViewButtonColumn buttonNo = new DataGridViewButtonColumn() { Name = "buttonN", HeaderText = @"", Width = 300 };
            buttonNo.Text = @"Не принят";
            buttonNo.UseColumnTextForButtonValue = true;
            buttonNo.Visible = true;
            dataGridView1.Columns.Add(buttonNo);

        }

        private void GetData()
        {
            try
            {
                string sqlExpression = "sp_SelectData";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@status", @"не рассмотрен");
                dataAdapter.SelectCommand = command;
                DataTable table1 = new DataTable();
                dataAdapter.Fill(table1);
                bindingSource.DataSource = table1;
                dataGridView1.DataSource = bindingSource;

                command = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([sname],' ',[fname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[date] as [Дата],[status] as [Статус] FROM [Contracts];", sqlConnection);
                dataAdapter = new SqlDataAdapter(command);
                DataTable table3 = new DataTable();
                dataAdapter.Fill(table3);
                dataGridView2.DataSource = table3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Lawyer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            Form form = Application.OpenForms[0];
            form.Show();
            Close();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "buttonY")
            {
                int id = 0;
                try
                {
                    if (e.ColumnIndex == 7) { id = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value); }
                    else
                    {
                        id = Convert.ToInt32(dataGridView1[2, e.RowIndex].Value);
                    }
                    string sqlExpression = "sp_UpdateStatus";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @"принят");
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                GetData();
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "buttonN")
            {
                try
                {
                    int id = 0;
                    if (e.ColumnIndex == 8) { id = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value); }
                    else
                    {
                        id = Convert.ToInt32(dataGridView1[2, e.RowIndex].Value);
                    }
                    string sqlExpression = "sp_UpdateStatus";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @"не принят");
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
                            if (dataGridView1.Rows[i].Cells[2].Value != null)
                                if (dataGridView1.Rows[i].Cells[2].Value.ToString().Contains(textBox8.Text))
                                {
                                    dataGridView1.Rows[i].Selected = true;
                                }
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
                            if (dataGridView1.Rows[i].Cells[3].Value != null)
                                if (dataGridView1.Rows[i].Cells[3].Value.ToString().Contains(textBox8.Text))
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
                        }
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i < dataGridView2.RowCount; i++)
                        {
                            if (dataGridView2.Rows[i].Cells[6].Value != null)
                                if (dataGridView2.Rows[i].Cells[6].Value.ToString().Contains(textBox1.Text))
                                {
                                    dataGridView2.Rows[i].Selected = true;
                                }
                        }
                    }
                    break;
            }
        }
    }
}
