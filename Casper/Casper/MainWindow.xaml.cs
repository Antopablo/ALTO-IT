using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplicationDatabase database;
        public MySqlConnection myServerSQL;
        MySqlCommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            database = new ApplicationDatabase();
            myServerSQL = new MySqlConnection("server=137.74.118.171;userid=sta41;persistsecurityinfo=True;password=w5fc03;database=sta41");
            myServerSQL.Open();
        }

        private void SignIn_bouton_Click(object sender, RoutedEventArgs e)
        {

            //Cherche sur BDD Mysql
            cmd = new MySqlCommand("SELECT Identifiant FROM Users WHERE Identifiant = " + "'" + Champ_identifiant.Text + "' AND Password = '" + Champ_password.Password + "'", myServerSQL);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Select_projet P = new Select_projet(this);
                P.Show();
                Close();
                reader.Close();
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                reader.Close();
            }

            //Select_projet P = new Select_projet(this);
            //P.Show();
            //Close();
        }

        public async void WebQueryMySQL(string commande)
        {
            cmd = new MySqlCommand(commande, myServerSQL);
            await cmd.ExecuteNonQueryAsync();
        }

    }
}
