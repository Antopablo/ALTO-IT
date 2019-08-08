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
    /// Logique d'interaction pour AffichageProjets.xaml
    /// </summary>
    public partial class Modif_Projet : Window
    {
        public Projet ProjetSelect { get; set; }
        public MainWindow mw { get; set; }
        public Modif_Projet()
        {
            InitializeComponent();
        }

        public Modif_Projet(MainWindow M, Projet P)
        {
            InitializeComponent();
            ProjetSelect = P;
            mw = M;
            TitreModify.Text = ProjetSelect.Name;
        }

        private void Validation_modification_Click(object sender, RoutedEventArgs e)
        {
            string tmp = ProjetSelect.Name;

            if (TitreModify.Text == "" || TitreModify.Text == null)
            {
                MessageBox.Show("Champ manquant", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                ProjetSelect.Name = TitreModify.Text;

                switch (Combo_Provider.Text)
                {
                    case "Amazon Web Services":
                        ProjetSelect.Provider = PROVIDER.AWS;
                        break;
                    case "Azure":
                        ProjetSelect.Provider = PROVIDER.AZURE;
                        break;
                    case "Google Cloud Services":
                        ProjetSelect.Provider = PROVIDER.GOOGLE;
                        break;
                    default:
                        break;
                }
            }
            mw.WebQueryMySQL("UPDATE Projets SET Name = '"+ ProjetSelect.Name +"', Provider = '"+ (int)ProjetSelect.Provider +"' WHERE Name = '"+ tmp +"' ");
            mw.database.SaveChanges();
            Close();
            MessageBox.Show("Mise à jour terminé", "Mise à jour d'un projet", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
