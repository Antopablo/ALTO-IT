using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Ajout_Projet.xaml
    /// </summary>
    public partial class Ajout_Projet : Window
    {
        public MainWindow mw { get; set; }

        public Ajout_Projet()
        {
            InitializeComponent();
        }

        public Ajout_Projet(MainWindow m)
        {
            InitializeComponent();
            mw = m;
        }

        private void Bouton_validerProjet_Click(object sender, RoutedEventArgs e)
        {
            Projet P = new Projet();
            switch (Combo_Provider.Text)
            {
                case "Amazon Web Services":
                    P.Name = NomProjet.Text;
                    P.Provider = PROVIDER.AWS;
                    break;
                case "Azure":
                    P.Name = NomProjet.Text;
                    P.Provider = PROVIDER.AZURE;
                    break;
                case "Google Cloud Services":
                    P.Name = NomProjet.Text;
                    P.Provider = PROVIDER.GOOGLE;
                    break;
                default:
                    break;
            }
            if (NomProjet.Text == null || NomProjet.Text == "")
            {
                MessageBox.Show("Vous devez donner un nom au projet", "Nom invalide", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Combo_Provider.Text == null || Combo_Provider.Text == "")
            {
                MessageBox.Show("Vous devez séléctionner un provider", "Provider invalide", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            else
            {
                mw.database.ProjetDatabase.Add(P);
                mw.database.SaveChanges();

                //Dashboard D = new Dashboard(mw, P);
                //D.Show();
                //Close();
            }
        }

        private void Retour_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Voulez-vous revenir à la selection des projets ?", "Retour aux projets", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                Select_projet P = new Select_projet(mw);
                P.Show();
                Close();
            }
        }
    }
}
