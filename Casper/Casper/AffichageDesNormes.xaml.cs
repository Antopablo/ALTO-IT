using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour AffichageDesNormes.xaml
    /// </summary>
    public partial class AffichageDesNormes : Window
    {
        MainWindow mw;
        Dashboard dashb;
        Norme NormeSelectionnee { get; set; }
        public AffichageDesNormes(MainWindow m, Dashboard dash)
        {
            InitializeComponent();
            mw = m;
            dashb = dash;
            ListeViewNormes.ItemsSource = mw.database.NormeDatabase.Local;
            mw.database.NormeDatabase.ToList();
        }

        private void ListeViewNormes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NormeSelectionnee = (Norme)ListeViewNormes.SelectedItem;
            if (NormeSelectionnee != null)
            {
                TitreModify.Text = NormeSelectionnee.Nom_Norme;
                TitreModifyBlock.Text = NormeSelectionnee.Nom_Norme;
                Document.Text = NormeSelectionnee.DocumentName;
            }
            AjoutDocument.Visibility = Visibility.Visible;
            label.Visibility = Visibility.Visible;
            scrollV.Visibility = Visibility.Visible;
        }

        private void BoutonValiderModify_Click(object sender, RoutedEventArgs e)
        {

            if (NormeSelectionnee != null)
            {
                NormeSelectionnee.Nom_Norme = TitreModify.Text;
                mw.database.SaveChanges();
                dashb.ROOT_Normes.NormeObervCollec.Clear();
                dashb.AfficherLesTreeView();
                Close();

            }
            else
            {
                MessageBox.Show("Selectionnez une norme à modifier", "error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void BoutonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            Norme tmp = NormeSelectionnee;

            if (NormeSelectionnee != null)
            {
                int IDSelected = NormeSelectionnee.Id;
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer la norme " + NormeSelectionnee.Nom_Norme + " ?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {

                    if (MessageBox.Show("Voulez - vous supprimer tous les documents associés à " + NormeSelectionnee.Nom_Norme + " ? ", "Suppression des documents", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        using (ApplicationDatabase context = new ApplicationDatabase())
                        {
                            // supprime son document
                            var sonDoc = context.Database.SqlQuery<string>("SELECT DocumentPath FROM Normes WHERE Id = " + IDSelected).FirstOrDefault();
                            if (sonDoc != null)
                            {
                                File.Delete(sonDoc);
                            }

                            // supprime documents des exigences enfants
                            var ListeEnfant = context.Database.SqlQuery<string>("SELECT DocumentPath FROM Exigences WHERE ForeignKey_TO_Norme = " + IDSelected).ToList();
                            foreach (string item in ListeEnfant)
                            {
                                if (item != null)
                                {
                                    File.Delete(item);
                                }
                            }
                        }
                    }


                    mw.database.NormeDatabase.Remove(NormeSelectionnee);
                    mw.database.SaveChanges();


                    using (ApplicationDatabase context = new ApplicationDatabase())
                    {
                        var ListeEnfant = context.Database.SqlQuery<string>("SELECT Name FROM Exigences WHERE ForeignKey_TO_Norme = " + IDSelected).ToList();
                        foreach (string item in ListeEnfant)
                        {
                            // supprime la table à son nom
                            var x = context.Database.ExecuteSqlCommand("DROP TABLE " + dashb.SimpleQuoteFormater(dashb.FormaterToSQLRequest("_" + IDSelected + item)));

                            // supprime de Exigences
                            var zz = context.Database.ExecuteSqlCommand("DELETE FROM Exigences WHERE Name = '" + dashb.SimpleQuoteFormater(item) + "'");
                        }
                    }

                    dashb.ROOT_Normes.NormeObervCollec.Clear();
                    dashb.AfficherLesTreeView();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Selectionnez une norme à supprimer", "error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dashb.FenetreOuverte = false;
        }

        private void AjoutDocument_Click(object sender, RoutedEventArgs e)
        {
            {
                string fileName = "";
                string fileNameWithID = "";
                string sourcePath;
                string targetPath;


                if (NormeSelectionnee != null)
                {
                    OpenFileDialog open = new OpenFileDialog();
                    open.ShowDialog();
                    fileNameWithID = "[" + (NormeSelectionnee.Id) + "]" + open.SafeFileName;
                    fileName = open.SafeFileName;
                    sourcePath = open.FileName;
                    targetPath = @"C:\Users\stagiaire\Desktop\ALTO-IT\Casper\Casper\bin\Debug\DocumentClient\" + fileNameWithID;

                    try
                    {
                        File.Copy(sourcePath, targetPath);
                        NormeSelectionnee.DocumentPath = targetPath;
                        NormeSelectionnee.DocumentName = fileName;
                    }
                    catch (IOException)
                    {
                        if (MessageBox.Show("Le document existe déjà, voulez-vous le mettre à jour ?", "Fichier existant", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                        {
                            File.Delete(targetPath);
                            File.Copy(sourcePath, targetPath);
                            NormeSelectionnee.DocumentPath = targetPath;
                            NormeSelectionnee.DocumentName = fileName;
                        }
                    }
                }
            }
        }
    }
}
