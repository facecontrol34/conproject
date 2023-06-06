// MainForm.cs
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WFproject
{
    public partial class MainForm : Form
    {
        private DB db; // Поле для хранения экземпляра класса DB
        private string userId; // Поле для хранения идентификатора пользователя

        public MainForm()
        {
            InitializeComponent();
            db = new DB();
            LoadBalanceData();
        }

        public void SetUserId(string userId)
        {
            this.userId = userId;
        }

        public void LoadBalanceData()
        {
            db.openConnection();

            string query = $"SELECT balanceRUB, balanceUSD, balanceEURO FROM users WHERE userId = '{userId}'";

            MySqlCommand command = new MySqlCommand(query, db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                double balanceRUB = reader.GetDouble("balanceRUB");
                double balanceUSD = reader.GetDouble("balanceUSD");
                double balanceEURO = reader.GetDouble("balanceEURO");

                balanceRubWindow.Text = balanceRUB.ToString();
                balanceUsdWindow.Text = balanceUSD.ToString();
                balanceEuroWindow.Text = balanceEURO.ToString();

                USDtoRUBWindow.Text = "80.88";
                EURtoRUBWindow.Text = "87.1";
            }

            reader.Close();
            db.closeConnection();
        }

        private void mainExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mainExitButton_MouseEnter(object sender, EventArgs e)
        {
            mainExitButton.ForeColor = Color.Black;
        }

        private void mainExitButton_MouseLeave(object sender, EventArgs e)
        {
            mainExitButton.ForeColor = Color.White;
        }

        // Движение окна
        Point lastPoint;

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void convisionButton_Click(object sender, EventArgs e)
        {
            ConversionForm conversionForm = new ConversionForm(userId);
            conversionForm.ShowDialog();
        }

        
    }
}
