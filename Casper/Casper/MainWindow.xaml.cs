using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplicationDatabase database;
        public MainWindow()
        {
            InitializeComponent();
            database = new ApplicationDatabase();

        }

        private void SignIn_bouton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    User us = (from u in database.UsersDatabase
            //             where u.Identifiant == Champ_identifiant.Text && u.Password == Champ_password.Password
            //               select u).First();

            //    Dashboard D = new Dashboard(this);
            //    D.Show();
            //    Close();
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Identifiant ou Mot de Passe invalide");
            //}



            //Dashboard D = new Dashboard(this);
            // D.Show();

            //Projet P = new Projet(this);
            //P.Show();
            //Close();

        }


    }
}
