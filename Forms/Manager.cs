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
    public partial class Manager : Form
    {
        SqlConnection sqlConection;
        private BindingSource bindingSource = new BindingSource();
        private BindingSource bindingSource1 = new BindingSource();
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();

        public Manager()
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

        private void AddButton()
        {
            //вкладка невыолненные договоры
            DataGridViewButtonColumn buttonRealize = new DataGridViewButtonColumn() { Name = "button", HeaderText = @"Подтверждение", Width = 300 };
            buttonRealize.Text = @"Выполнено";
            buttonRealize.UseColumnTextForButtonValue = true;
            buttonRealize.Visible = true;
            dataGridView1.Columns.Add(buttonRealize);

            //вкладка все договоры
            DataGridViewButtonColumn buttonDelete = new DataGridViewButtonColumn() { Name = "buttonDelete", HeaderText = @"Отменить договор", Width = 300 };
            buttonDelete.Text = @"Удалить";
            buttonDelete.UseColumnTextForButtonValue = true;
            buttonDelete.Visible = true;
            dataGridView3.Columns.Add(buttonDelete);

        }

        private async void Manager_Load(object sender, EventArgs e)
        {
            try
            {
                //создание подключения к БД
                string connectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Database.mdf';Integrated Security=True";
                sqlConection = new SqlConnection(connectionString);
                //открытие ассинхронного подключения к БД, чтобы не тормозил пользовательский интерфейс
                await sqlConection.OpenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GetData();
            AddButton();

        }

        private void Manager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConection != null && sqlConection.State != ConnectionState.Closed)
            {
                sqlConection.Close();
            }
            Form form = Application.OpenForms[0];
            form.Show();
            Close();
        }

        private void GetData()
        {
            try
            {
                //SqlCommand command = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([fname],' ',[sname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[status] as [Статус] FROM [Contracts] WHERE [status]=N'оплачен' AND [id_manager]=(SELECT [employees].[Id_employee] FROM [employees] JOIN [login] ON [employees].[Id_employee]=[login].[id_employee] WHERE [job]=N'Менеджер' AND [login]='" + @Program.login + "');", sqlConection);

                string sqlExpression = "sp_SelectDataUser";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(sqlExpression, sqlConection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@status", @"оплачен");
                command.Parameters.AddWithValue("@job", @"Менеджер");
                command.Parameters.AddWithValue("@login", @Program.login);
                dataAdapter.SelectCommand = command;
                //таблица невыполненные договоры
                DataTable table1 = new DataTable();
                //выполнение команды возвращающей табличное представление
                dataAdapter.Fill(table1);
                bindingSource.DataSource = table1;
                dataGridView1.DataSource = bindingSource;

                //таблица выполненные договоры
                //SqlCommand command1 = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([fname],' ',[sname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[status] as [Статус] FROM [Contracts] WHERE [status]=N'выполнен' AND [id_manager]=(SELECT [employees].[Id_employee] FROM [employees] JOIN [login] ON [employees].[Id_employee]=[login].[id_employee] WHERE [job]=N'Менеджер' AND [login]='" + @Program.login + "');", sqlConection);
                dataAdapter = new SqlDataAdapter();
                SqlCommand command1 = new SqlCommand(sqlExpression, sqlConection);
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                command1.Parameters.AddWithValue("@status", @"выполнен");
                command1.Parameters.AddWithValue("@job", @"Менеджер");
                command1.Parameters.AddWithValue("@login", @Program.login);
                dataAdapter.SelectCommand = command1;
                DataTable table2 = new DataTable();
                dataAdapter.Fill(table2);
                bindingSource1.DataSource = table2;
                dataGridView2.DataSource = bindingSource1;

                command = new SqlCommand("SELECT [Id_contract] as [Номер договора], CONCAT([sname],' ',[fname],' ',[pname]) as [ФИО], [email], [phone_number] as [Телефон], [package] as [Пакет услуг],[date] as [Дата],[status] as [Статус] FROM [Contracts] WHERE [id_manager]=(SELECT [employees].[Id_employee] FROM [employees] JOIN [login] ON [employees].[Id_employee]=[login].[id_employee] WHERE [job]=N'Менеджер' AND [login]='" + @Program.login + "');", sqlConection);
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "button")
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView1[1, e.RowIndex].Value);
                    string sqlExpression = "sp_UpdateStatus";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @"выполнен");
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                GetData();
            }

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "buttonDelete")
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView3[1, e.RowIndex].Value);
                    string status = dataGridView3[7, e.RowIndex].Value.ToString();
                    string sqlExpression = "sp_DeleteContract";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_contract", id);
                    command.Parameters.AddWithValue("@status", @status);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                GetData();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(textBox1.Text);
                string fname, sname, pname, address, phone_number, email, package = "эконом";
                double price;
                DateTime date;
                fname = textBox2.Text.ToString();
                sname = textBox3.Text.ToString();
                pname = textBox4.Text.ToString();
                email = textBox5.Text.ToString();
                address = textBox7.Text.ToString();
                price = Convert.ToDouble(textBox6.Text);
                date = DateTime.ParseExact(maskedTextBox1.Text.ToString(), "dd.M.yyyy", null);
                phone_number = maskedTextBox2.Text.ToString();
                if (((RadioButton)groupBox1.Controls[0]).Checked == true) { package = @"эконом"; }
                if (((RadioButton)groupBox1.Controls[1]).Checked == true) { package = @"стандарт"; }
                if (((RadioButton)groupBox1.Controls[2]).Checked == true) { package = @"VIP"; }

                string sqlExpression = "InsertContract";
                SqlCommand command = new SqlCommand(sqlExpression, sqlConection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@login", @Program.login);
                command.Parameters.AddWithValue("@fname", @fname);
                command.Parameters.AddWithValue("@sname", @sname);
                command.Parameters.AddWithValue("@pname", @pname);
                command.Parameters.AddWithValue("@address", @address);
                command.Parameters.AddWithValue("@phone_number", @phone_number);
                command.Parameters.AddWithValue("@email", @email);
                command.Parameters.AddWithValue("@package", @package);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@id_manager", 1);
                command.ExecuteNonQuery();
                MessageBox.Show("Договор добавлен", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateFilds();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GetData();
        }

        private void UpdateFilds()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";

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

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                dataGridView3.Rows[i].Selected = false;
            }
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        for (int i = 0; i < dataGridView3.RowCount; i++)
                        {
                            if (dataGridView3.Rows[i].Cells[1].Value != null)
                                if (dataGridView3.Rows[i].Cells[1].Value.ToString().Contains(textBox9.Text))
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
                            if (dataGridView3.Rows[i].Cells[2].Value != null)
                                if (dataGridView3.Rows[i].Cells[2].Value.ToString().Contains(textBox9.Text))
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
                            if (dataGridView3.Rows[i].Cells[7].Value != null)
                                if (dataGridView3.Rows[i].Cells[7].Value.ToString().Contains(textBox9.Text))
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
