using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProektFirstVariant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string lineConnection;

        public MainWindow()
        {
            InitializeComponent();
            Connect("", ""); //указать БД и подключение 
        }

        // Метод для установки строки подключения к базе данных
        public void Connect(string servername, string dbname)
            => lineConnection = "Data Source=" + servername + ";Initial Catalog=" + dbname + ";Integrated Security=True";

        // Метод для добавления нового пользователя
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Role", role);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Метод для добавления нового картриджа
        private void AddCartridgeButton_Click(object sender, RoutedEventArgs e)
        {
            string type = TypeTextBox.Text;
            string model = ModelTextBox.Text;
            string serialNumber = SerialNumberTextBox.Text;
            DateTime installationDate = InstallationDatePicker.SelectedDate ?? DateTime.Now;
            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string comments = CommentsTextBox.Text;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "INSERT INTO Cartridges (Type, Model, SerialNumber, InstallationDate, Status, Comments) VALUES (@Type, @Model, @SerialNumber, @InstallationDate, @Status, @Comments)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@Model", model);
                command.Parameters.AddWithValue("@SerialNumber", serialNumber);
                command.Parameters.AddWithValue("@InstallationDate", installationDate);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@Comments", comments);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Метод для удаления картриджа
        private void DeleteCartridgeButton_Click(object sender, RoutedEventArgs e)
        {
            int cartridgeID = int.Parse(TypeTextBox.Text); // Предполагается, что ID картриджа вводится в поле TypeTextBox

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "DELETE FROM Cartridges WHERE CartridgeID = @CartridgeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartridgeID", cartridgeID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Метод для обновления статуса картриджа
        private void UpdateCartridgeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            int cartridgeID = int.Parse(TypeTextBox.Text); // Предполагается, что ID картриджа вводится в поле TypeTextBox
            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "UPDATE Cartridges SET Status = @Status WHERE CartridgeID = @CartridgeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@CartridgeID", cartridgeID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Метод для поиска картриджей
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "SELECT * FROM Cartridges WHERE Type LIKE @SearchTerm OR Model LIKE @SearchTerm OR SerialNumber LIKE @SearchTerm";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                SearchResultsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        // Метод для просмотра истории картриджей
        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            int cartridgeID = int.Parse(HistoryCartridgeIDTextBox.Text);

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "SELECT * FROM Repairs WHERE CartridgeID = @CartridgeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartridgeID", cartridgeID);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                HistoryDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        // Метод для просмотра аналитики
        private void ViewAnalyticsButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now;
            DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                string query = "SELECT SUM(Cost) AS TotalCost FROM Repairs WHERE RepairDate BETWEEN @StartDate AND @EndDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                AnalyticsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }
    }
}

