using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public MainWindow mw { get; set; }
        public Vue_Exigence Vue { get; set; }
        public Vue_Mesure Vue_Mesure { get; set; }
        public Norme ROOT_Normes { get; set; }
        public Norme NormeSelectionnee { get; set; }
        public Mesure ROOT_Mesures { get; set; }
        public Projet ProjetEnCours { get; set; }
        public bool FenetreOuverte { get; set; }
        public bool SuprDoc { get; set; }


        public Dashboard(MainWindow m, Projet p)
        {
            InitializeComponent();
            mw = m;
            ProjetEnCours = p;
            //Vue = new Vue_Circulaire(this);
            ROOT_Normes = new Norme("Normes", ProjetEnCours.Id);
            TreeViewNORME.Items.Add(ROOT_Normes);

            ROOT_Mesures = new Mesure("Mesures");
            TreeViewMesures.Items.Add(ROOT_Mesures);

            AfficherLesTreeView();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // FullScreen
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;
            this.Height = SystemParameters.WorkArea.Height;
            this.Width = SystemParameters.WorkArea.Width;

            // set du titre du dashboard
            Title = "Dashboard - Projet : " + ProjetEnCours.Name;
        }

        private void Ajout_exigence_Click(object sender, RoutedEventArgs e)
        {
            if (FenetreOuverte == false)
            {
                Ajout_Exigence A = new Ajout_Exigence(mw, this, (Vue_Exigence)Frame_Vue_Circulaire.Content);
                A.Show();
                FenetreOuverte = true;
            }
        }

        public void SuppressionTabEnfantExigence(string CurrentItem)
        {
            List<string> ListeGenerale = new List<string>();
            List<string> ListeEnfant = new List<string>();
            using (ApplicationDatabase context = new ApplicationDatabase())
            {
                var RequestListEnfant = context.Database.SqlQuery<string>("Select Titre from " + CurrentItem).ToList();
                ListeEnfant = RequestListEnfant;
                foreach (string item in ListeEnfant)
                {
                    string tmp = "";
                    ListeGenerale.Add(item);
                    tmp = item;
                    SuppressionTabEnfantExigence(TableFormater(FormaterToSQLRequest(tmp)));
                }
                foreach (string item2 in ListeGenerale)
                {
                    if (SuprDoc == true)
                    {
                        var docASupr = context.Database.SqlQuery<string>("SELECT DocumentPath from Exigences WHERE Name = '" + SimpleQuoteFormater(item2) + "'").FirstOrDefault();
                        if (docASupr != null)
                        {
                            File.Delete(docASupr);
                        }
                        SuprDoc = false;
                    }

                    var suppenfantTableExigence = context.Database.ExecuteSqlCommand("DELETE FROM Exigences WHERE Name = '" + SimpleQuoteFormater(item2) + "'");

                    string tmp2 = "";
                    tmp2 = item2;
                    var suppenfant = context.Database.ExecuteSqlCommand("DROP TABLE " + TableFormater(SimpleQuoteFormater(FormaterToSQLRequest(tmp2))));
                }
                RequestListEnfant.Clear();
            }
            ListeGenerale.Clear();
            ListeEnfant.Clear();
        }

        public void SuppressionTabEntantMesure(string CurrentItem)
        {
            List<string> ListeGenerale = new List<string>();
            List<string> ListeEnfant = new List<string>();
            using (ApplicationDatabase context = new ApplicationDatabase())
            {
                var RequestListEnfant = context.Database.SqlQuery<string>("Select Titre from " + CurrentItem).ToList();
                ListeEnfant = RequestListEnfant;
                foreach (string item in ListeEnfant)
                {
                    string tmp = "";
                    ListeGenerale.Add(item);
                    tmp = item;
                    SuppressionTabEntantMesure(TableFormaterMesure(FormaterToSQLRequest(tmp)));
                }
                foreach (string item2 in ListeGenerale)
                {
                    if (SuprDoc == true)
                    {
                        var docASupr = context.Database.SqlQuery<string>("SELECT DocumentPath from Mesures WHERE Name = '" + SimpleQuoteFormater(item2) + "'").FirstOrDefault();
                        if (docASupr != null)
                        {
                            File.Delete(docASupr);
                        }
                        SuprDoc = false;
                    }

                    var suppenfantTableExigence = context.Database.ExecuteSqlCommand("DELETE FROM Mesures WHERE Name = '" + SimpleQuoteFormater(item2) + "'");

                    string tmp2 = "";
                    tmp2 = item2;
                    var suppenfant = context.Database.ExecuteSqlCommand("DROP TABLE " + TableFormaterMesure(SimpleQuoteFormater(FormaterToSQLRequest(tmp2))));
                }
                RequestListEnfant.Clear();
            }
            ListeGenerale.Clear();
            ListeEnfant.Clear();
        }


        private void Ajout_Norme_Click(object sender, RoutedEventArgs e)
        {
            if (FenetreOuverte == false)
            {
                Ajout_Norme AJ = new Ajout_Norme(mw, this, Vue);
                AJ.Show();
                FenetreOuverte = true;
            }

        }

        private void Modif_Norme_Click(object sender, RoutedEventArgs e)
        {
            if (FenetreOuverte == false)
            {
                AffichageDesNormes AF = new AffichageDesNormes(mw, this);
                AF.scrollV.Visibility = Visibility.Collapsed;
                AF.label.Visibility = Visibility.Collapsed;
                AF.AjoutDocument.Visibility = Visibility.Collapsed;
                AF.BoutonValiderModify.Visibility = Visibility.Visible;
                AF.BoutonSupprimer.Visibility = Visibility.Collapsed;
                AF.AjoutDocument.Visibility = Visibility.Visible;
                AF.TitreModify.Visibility = Visibility.Visible;
                AF.TitreModifyBlock.Visibility = Visibility.Hidden;
                AF.Show();
                FenetreOuverte = true;
            }
        }

        private void Supr_Norme_Click(object sender, RoutedEventArgs e)
        {
            if (FenetreOuverte == false)
            {
                AffichageDesNormes AF = new AffichageDesNormes(mw, this);
                AF.BoutonValiderModify.Visibility = Visibility.Hidden;
                AF.AjoutDocument.Visibility = Visibility.Collapsed;
                AF.BoutonSupprimer.Visibility = Visibility.Visible;
                AF.TitreModify.Visibility = Visibility.Hidden;
                AF.TitreModifyBlock.Visibility = Visibility.Visible;
                AF.Show();
                FenetreOuverte = true;
            }
        }

        private void TreeViewNORME_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NormeSelectionnee = (Norme)TreeViewNORME.SelectedItem;

                if (NormeSelectionnee.Nom_Norme == "Normes")
                {
                    TreeViewMesures.Visibility = Visibility.Collapsed;

                    GridControle_Norme.Visibility = Visibility.Visible;
                    GridControle_exigence.Visibility = Visibility.Collapsed;
                    GridControle_Mesure.Visibility = Visibility.Collapsed;

                    Frame_Vue_Circulaire.Visibility = Visibility.Collapsed;
                    Frame_Vue_Documentation.Visibility = Visibility.Collapsed;
                    Frame_Vue_Mesures.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TreeViewMesures.Visibility = Visibility.Visible;

                    GridControle_Norme.Visibility = Visibility.Collapsed;
                    GridControle_exigence.Visibility = Visibility.Visible;
                    GridControle_Mesure.Visibility = Visibility.Collapsed;

                    Frame_Vue_Circulaire.Visibility = Visibility.Visible;
                    Frame_Vue_Documentation.Visibility = Visibility.Collapsed;
                    Frame_Vue_Mesures.Visibility = Visibility.Collapsed;
                    Frame_Vue_Circulaire.Content = new Vue_Exigence(this);
                }
            }
            catch (Exception)
            {
            }

        }

        private void TreeViewMesures_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                GridControle_Norme.Visibility = Visibility.Collapsed;
                GridControle_exigence.Visibility = Visibility.Collapsed;
                GridControle_Mesure.Visibility = Visibility.Visible;

                Frame_Vue_Circulaire.Visibility = Visibility.Collapsed;
                Frame_Vue_Documentation.Visibility = Visibility.Collapsed;

                Frame_Vue_Mesures.Visibility = Visibility.Visible;
                Frame_Vue_Mesures.Content = new Vue_Mesure(this);
            }
            catch (Exception)
            {
            }

        }

        private void TreeViewDocumentation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GridControle_Norme.Visibility = Visibility.Collapsed;
            GridControle_exigence.Visibility = Visibility.Collapsed;
            GridControle_Mesure.Visibility = Visibility.Collapsed;

            Frame_Vue_Circulaire.Visibility = Visibility.Collapsed;
            Frame_Vue_Documentation.Visibility = Visibility.Visible;
            Frame_Vue_Mesures.Visibility = Visibility.Collapsed;

            Frame_Vue_Documentation.Content = new Vue_Document(this);
        }

        public void AfficherLesTreeView()
        {
            foreach (Norme item in mw.database.NormeDatabase)
            {
                if (item.FK_to_Projet == ProjetEnCours.Id)
                {
                    ROOT_Normes.NormeObervCollec.Add(item);
                }
            }
            foreach (Mesure item in mw.database.MesureDatabase)
            {
                if (item.Name == "Mesures")
                {
                    ROOT_Mesures.MesuresObservCollec.Add(item);
                }
            }
        }

        public void AfficherTreeViewNorme()
        {

        }

        public string FormaterToSQLRequest(string Text)
        {
            Text = Text.Replace(' ', '_');
            Text = Text.Replace("'", "");
            Text = Text.Replace('/', '$');
            Text = Text.Replace('\\', '_');
            Text = Text.Replace('*', '_');
            Text = Text.Replace(';', '_');
            Text = Text.Replace(':', '$');
            Text = Text.Replace('{', '$');
            Text = Text.Replace('}', '_');
            Text = Text.Replace('^', '_');
            Text = Text.Replace('+', '_');
            Text = Text.Replace('-', '_');
            Text = Text.Replace('=', '$');
            Text = Text.Replace('£', '_');
            Text = Text.Replace('?', '$');
            Text = Text.Replace('!', '$');
            Text = Text.Replace(',', '_');
            Text = Text.Replace('<', '$');
            Text = Text.Replace('>', '_');
            Text = Text.Replace('§', '_');
            Text = Text.Replace('%', '_');
            Text = Text.Replace("\"", "");
            Text = Text.Replace("[", "$");
            Text = Text.Replace("]", "_");
            Text = Text.Replace(".", "");
            Text = Text.Replace("-", "_");
            Text = Text.Replace("|", "_");
            Text = Text.Replace("&", "_");
            Text = Text.Replace("`", "_");
            Text = Text.Replace("#", "_");
            Text = Text.Replace("°", "_");
            Text = Text.Replace("(", "$");
            Text = Text.Replace(")", "_");
            Text.Trim();
            return Text;
        }

        public string AccentFormater(string Text)
        {
            Text = Text.Replace("à", "a");
            Text = Text.Replace("á", "a");
            Text = Text.Replace("â", "a");
            Text = Text.Replace("ä", "a");

            Text = Text.Replace("ç", "c");

            Text = Text.Replace("é", "e");
            Text = Text.Replace("è", "e");
            Text = Text.Replace("ê", "e");
            Text = Text.Replace("ë", "e");

            Text = Text.Replace("î", "i");
            Text = Text.Replace("ï", "i");

            Text = Text.Replace("ô", "o");

            Text = Text.Replace("û", "u");
            Text = Text.Replace("ù", "u");

            return Text;
        }

        public string TableFormater(string Text)
        {
            StringBuilder builder = new StringBuilder(Text);
            return Text = builder.Insert(0, "_" + NormeSelectionnee.Id).ToString();
        }

        public string TableFormaterMesure(string Text)
        {
            StringBuilder builder = new StringBuilder(Text);
            return Text = builder.Insert(0, "_" + ProjetEnCours.Id).ToString();
        }

        public string SimpleQuoteFormater(string text)
        {
            return text.Replace("'", "''");
        }

        private void Documentviewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String fileName = NormeSelectionnee.DocumentPath;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = fileName;
                process.Start();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Sélectionnez une exigence");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Aucun document associé");
            }

        }

        private void Ajout_Mesure_Click(object sender, RoutedEventArgs e)
        {
            Ajout_Mesure AM = new Ajout_Mesure(mw, this);
            AM.Show();
            FenetreOuverte = true;
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

        private void Ajout_exigence_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Ajout_Mesure_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

    }
}