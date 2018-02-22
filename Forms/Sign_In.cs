using project.Forms;
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
    public partial class sign_in : Form
    {
        private SqlConnection sqlConnection;

        public sign_in()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            SelectEmployee();



        }

        private void SelectEmployee()
        {
            try
            {
                //создание подключения к БД
                string connectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Database.mdf';Integrated Security=True";
                sqlConnection = new SqlConnection(connectionString);
                //открытие ассинхронного подключения к БД, чтобы не тормозил пользовательский интерфейс
                sqlConnection.Open();
                //позволяет получать таблицу в табличном представлении
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Program.login = textBox1.Text.ToString();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT [login],[password] FROM [login] WHERE [login]='" + Program.login + "' AND [password]='" + textBox2.Text + "'", sqlConnection);
            SqlCommand commandJob = new SqlCommand("SELECT [job] FROM [login] JOIN [employees] ON [login].[id_employee]=[employees].[Id_employee] WHERE [login] = @login;", sqlConnection);
            commandJob.Parameters.AddWithValue("@login", @Program.login);
            try
            {
                //выполнение команды возвращающей табличное представление
                string job = commandJob.ExecuteScalar().ToString();
                sqlReader = command.ExecuteReader();
                if (sqlReader.HasRows)
                {

                    textBox1.Text = "";
                    textBox2.Text = "";
                    if (job == "Менеджер")
                    {
                        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                        {
                            sqlConnection.Close();
                        }
                        Manager form = new Manager();

                        Hide();
                        form.ShowDialog();


                    }
                    if (job == "Юрист")
                    {
                        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                        {
                            sqlConnection.Close();
                        }
                        Lawyer form = new Lawyer();
                        Hide();
                        form.ShowDialog();

                    }
                    if (job == "Бухгалтер")
                    {
                        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                        {
                            sqlConnection.Close();
                        }
                        Accountant form = new Accountant();
                        Hide();
                        form.ShowDialog();
                    }
                    if (job == "Директор")
                    {
                        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                        {
                            sqlConnection.Close();
                        }

                        Leader form = new Leader();
                        Hide();
                        form.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Логин или пароль не верен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }

            }
        }



        private void sign_in_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }
    }
}
