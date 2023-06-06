using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WFproject
{
    public partial class ConversionForm : Form
    {
        private DB db;
        private string userId;

        public ConversionForm(string userId)
        {
            InitializeComponent();
            db = new DB();
            this.userId = userId;
        }

        public void SetUserId(string userId)
        {
            this.userId = userId;
        }

        private void mainExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            string selectedBalance = "";
            string selectedCurrency = "";
            decimal amount = 0;

            if (balanceRubCheckBox.Checked)
                selectedBalance = "balanceRUB";
            else if (balanceUSDCheckBox.Checked)
                selectedBalance = "balanceUSD";
            else if (balanceEUROCheckBox.Checked)
                selectedBalance = "balanceEURO";
            else
            {
                MessageBox.Show("Выберите баланс.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rubCheckBox.Checked)
                selectedCurrency = "balanceRUB";
            else if (usdCheckBox.Checked)
                selectedCurrency = "balanceUSD";
            else if (euroCheckBox.Checked)
                selectedCurrency = "balanceEURO";
            else
            {
                MessageBox.Show("Выберите валюту для конвертации.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(amountTextBox.Text, out amount))
            {
                MessageBox.Show("Введите корректную сумму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal convertedAmount = 0;

            // Логика конвертации валюты
            if (selectedBalance == "balanceRUB" && selectedCurrency == "balanceUSD")
            {
                // Пример конвертации из balanceRUB в balanceUSD
                convertedAmount = ConvertRUBtoUSD(amount);
            }
            else if (selectedBalance == "balanceRUB" && selectedCurrency == "balanceEURO")
            {
                // Пример конвертации из balanceRUB в balanceEURO
                convertedAmount = ConvertRUBtoEURO(amount);
            }
            else if (selectedBalance == "balanceUSD" && selectedCurrency == "balanceRUB")
            {
                // Пример конвертации из balanceUSD в balanceRUB
                convertedAmount = ConvertUSDtoRUB(amount);
            }
            else if (selectedBalance == "balanceUSD" && selectedCurrency == "balanceEURO")
            {
                // Пример конвертации из balanceUSD в balanceEURO
                convertedAmount = ConvertUSDtoEURO(amount);
            }
            else if (selectedBalance == "balanceEURO" && selectedCurrency == "balanceRUB")
            {
                // Пример конвертации из balanceEURO в balanceRUB
                convertedAmount = ConvertEUROtoRUB(amount);
            }
            else if (selectedBalance == "balanceEURO" && selectedCurrency == "balanceUSD")
            {
                // Пример конвертации из balanceEURO в balanceUSD
                convertedAmount = ConvertEUROtoUSD(amount);
            }
            else
            {
                MessageBox.Show("Ошибка при конвертации валюты.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Проверка на отрицательный баланс или нулевой баланс
            double currentBalance = GetBalance(selectedBalance);
            if (currentBalance <= 0)
            {
                MessageBox.Show("Недостаточно средств для конвертации.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                db.openConnection();

                string debitUpdateQuery = $"UPDATE users SET {selectedBalance} = {selectedBalance} - '{amount.ToString().Replace(",", ".")}' WHERE userId = '{userId}';";
                MySqlCommand debitUpdateCommand = new MySqlCommand(debitUpdateQuery, db.getConnection());
                debitUpdateCommand.ExecuteNonQuery();

                string creditUpdateQuery = $"UPDATE users SET {selectedCurrency} = {selectedCurrency} + '{convertedAmount.ToString().Replace(",", ".")}' WHERE userId = '{userId}';";
                MySqlCommand creditUpdateCommand = new MySqlCommand(creditUpdateQuery, db.getConnection());
                creditUpdateCommand.ExecuteNonQuery();

                db.closeConnection();

                MessageBox.Show("Конвертация успешно выполнена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            decimal convertedAmountToShow = 0;
            if (selectedCurrency == "balanceRUB")
                convertedAmountToShow = convertedAmount;
            else if (selectedCurrency == "balanceUSD")
                convertedAmountToShow = ConvertEUROtoUSD(convertedAmount);
            else if (selectedCurrency == "balanceEURO")
                convertedAmountToShow = ConvertEUROtoRUB(convertedAmount);

            // Отображение конвертированной суммы во втором текстовом поле
            convertedAmountTextBox.Text = convertedAmountToShow.ToString();
        }
        // Получение текущего баланса
        private double GetBalance(string balanceType)
        {
            try
            {
                db.openConnection();

                string query = $"SELECT {balanceType} FROM users WHERE userId = '{userId}';";
                MySqlCommand command = new MySqlCommand(query, db.getConnection());
                object result = command.ExecuteScalar();

                db.closeConnection();

                if (result != null && double.TryParse(result.ToString(), out double balance))
                    return balance;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении баланса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }


        // Методы для конвертации валюты
        private decimal ConvertRUBtoUSD(decimal amount)
        {
            // Ваш код для конвертации balanceRUB в balanceUSD
            decimal exchangeRate = 0.012m; // Пример курса обмена
            return amount * exchangeRate;
        }

        private decimal ConvertRUBtoEURO(decimal amount)
        {
            // Ваш код для конвертации balanceRUB в balanceEURO
            decimal exchangeRate = 0.0105m; // Пример курса обмена
            return amount * exchangeRate;
        }

        private decimal ConvertUSDtoRUB(decimal amount)
        {
            // Ваш код для конвертации balanceUSD в balanceRUB
            decimal exchangeRate = 80.0m; // Пример курса обмена
            return amount * exchangeRate;
        }

        private decimal ConvertUSDtoEURO(decimal amount)
        {
            // Ваш код для конвертации balanceUSD в balanceEURO
            decimal exchangeRate = 0.92m; // Пример курса обмена
            return amount * exchangeRate;
        }

        private decimal ConvertEUROtoRUB(decimal amount)
        {
            // Ваш код для конвертации balanceEURO в balanceRUB
            decimal exchangeRate = 95.0m; // Пример курса обмена
            return amount * exchangeRate;
        }

        private decimal ConvertEUROtoUSD(decimal amount)
        {
            // Ваш код для конвертации balanceEURO в balanceUSD
            decimal exchangeRate = 1.09m; // Пример курса обмена
            return amount * exchangeRate;
        }
    }
}
