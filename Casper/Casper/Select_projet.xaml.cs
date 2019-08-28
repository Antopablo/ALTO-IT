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
    /// Logique d'interaction pour Select_projet.xaml
    /// </summary>
    public partial class Select_projet : Window
    {
        public MainWindow mw { get; set; }

        public Select_projet()
        {
            InitializeComponent();
        }

        public Select_projet(MainWindow m)
        {
            InitializeComponent();
            mw = m;
            AfficherProjet();
        }

        private void Add_Projet_Click(object sender, RoutedEventArgs e)
        {
            Ajout_Projet CP = new Ajout_Projet(mw);
            CP.Show();
            Close();
        }

        public void AfficherProjet()
        {
            foreach (Projet item in mw.database.ProjetDatabase.ToList())
            {
                Combo_Provider.Items.Add(item);
            }
        }

        private void ValiderChoixProjet_Click(object sender, RoutedEventArgs e)
        {
            if (Combo_Provider.Text == null || Combo_Provider.Text == "")
            {
                MessageBox.Show("Vous devez choisir un projet ou en créer un nouveau", "Choix du projet", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Dashboard D = new Dashboard(mw, (Projet)Combo_Provider.SelectedValue);
                D.Show();
                Close();
            }

        }

        private void Bouton_modifier_projet_Click(object sender, RoutedEventArgs e)
        {
            Projet P = (Projet)Combo_Provider.SelectedValue;
            Modif_Projet AP = new Modif_Projet(mw, (Projet)Combo_Provider.SelectedValue);
            AP.Title = "Modification de : '" + P.Name + "'";
            AP.Show();
        }

        private void Combo_Provider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bouton_modifier_projet.Visibility = Visibility.Visible;
            Bouton_supprimer_projet.Visibility = Visibility.Visible;
        }

        private void Bouton_supprimer_projet_Click(object sender, RoutedEventArgs e)
        {
            Projet tmp = (Projet)Combo_Provider.SelectedValue;
            if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer le projet " + tmp.Name, "Suppression du projet", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                mw.database.ProjetDatabase.Remove(tmp);
                mw.WebQueryMySQL("DELETE FROM Projets WHERE Name = '"+ tmp.Name +"' ");
                mw.database.SaveChanges();
                Combo_Provider.Items.Clear();
                AfficherProjet();
                MessageBox.Show("Projet supprimé", "Projet Supprimé", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

    }
}
