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
    /// Logique d'interaction pour Ajout_Mesure.xaml
    /// </summary>
    public partial class Ajout_Mesure : Window
    {
        public MainWindow mw { get; set; }
        public Dashboard dashb { get; set; }
        public Ajout_Mesure()
        {
            InitializeComponent();
        }

        public Ajout_Mesure(MainWindow m, Dashboard D)
        {
            InitializeComponent();
            mw = m;
            dashb = D;
        }

        private void ValiderMesure_Click(object sender, RoutedEventArgs e)
        {
            if (TitleMesure.Text == null || TitleMesure.Text == "")
            {
                MessageBox.Show("Vous devez donner un nom à la mesure", "Nom invalide", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (TitleMesure.Text == "Menu")
                {
                    MessageBox.Show("Vous ne pouvez pas appeler une mesure ainsi", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (dashb.Vue_Mesure.MesureSelectionnee == null || dashb.Vue_Mesure.MesureSelectionnee.Name == "Menu")
                    {
                        try
                        {
                            CreateTable(TitleMesure.Text);
                            Mesure MesureParent = new Mesure(TitleMesure.Text, ContentMesure.Text, 0, dashb.ProjetEnCours.Id);
                            dashb.Vue_Mesure.ROOT_Mesures.MesuresObservCollec.Add(MesureParent);
                            mw.database.MesureDatabase.Add(MesureParent);
                            mw.WebQueryMySQL("INSERT INTO Mesures (Description, FK_to_Mesures, FK_to_Projets, Name) VALUES ('"+MesureParent.Description+"',"+MesureParent.FK_to_Mesures+","+MesureParent.FK_to_Projets+",'"+MesureParent.Name+"')");

                            mw.database.SaveChanges();

                            Close();
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            MessageBox.Show("Une Mesure à ce nom existe déjà ou le nom est trop long", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            CreateTable(TitleMesure.Text);
                            try
                            {
                                RemplirTable(dashb.Vue_Mesure.MesureSelectionnee.Name, dashb.Vue_Mesure.MesureSelectionnee.Id);
                                Mesure MesureEnfant = new Mesure(TitleMesure.Text, ContentMesure.Text, dashb.Vue_Mesure.MesureSelectionnee.Id, dashb.ProjetEnCours.Id);
                                dashb.Vue_Mesure.MesureSelectionnee.MesuresObservCollec.Add(MesureEnfant);
                                mw.database.MesureDatabase.Add(MesureEnfant);
                                mw.WebQueryMySQL("INSERT INTO Mesures (Description, FK_to_Mesures, FK_to_Projets, Name) VALUES ('" + MesureEnfant.Description + "'," + MesureEnfant.FK_to_Mesures + "," + MesureEnfant.FK_to_Projets + ",'" + MesureEnfant.Name + "')");

                            }
                            catch (Exception)
                            {
                                SupprimerTable(TitleMesure.Text);
                                MessageBox.Show("Impossible de remplir dans table parent", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            Close();
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {

                            MessageBox.Show("Une Mesure à ce nom existe déjà ou le nom est trop long", "error", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                        mw.database.SaveChanges();
                    }
                }
            }
        }

        public void CreateTable(string TableName)
        {
            TableName = dashb.TableFormaterMesure(dashb.SimpleQuoteFormater(dashb.FormaterToSQLRequest(TableName)));
            using (ApplicationDatabase context = new ApplicationDatabase())
            {
                var x = context.Database.ExecuteSqlCommand("CREATE TABLE " + TableName + " (ID INTEGER IDENTITY(1,1) PRIMARY KEY, ForeignKey INT, Titre VARCHAR(MAX), Description VARCHAR(MAX))");
            }
            mw.WebQueryMySQL("CREATE TABLE " + TableName + " (ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT, ForeignKey INT, Titre VARCHAR(10000), Description VARCHAR(10000))");
        }

        public void RemplirTable(string TableName, int ForeignKey)
        {
            TableName = dashb.TableFormaterMesure(dashb.SimpleQuoteFormater(dashb.FormaterToSQLRequest(TableName)));

            if (TableName != "_Menu")
            {
                try
                {
                    using (ApplicationDatabase context = new ApplicationDatabase())
                    {
                        var x = context.Database.ExecuteSqlCommand("INSERT INTO " + TableName + " (ForeignKey, Titre, Description) VALUES (" + "'" + ForeignKey + "'" + "," + "'" + dashb.SimpleQuoteFormater(TitleMesure.Text) + "'" + "," + "'" + dashb.SimpleQuoteFormater(ContentMesure.Text) + "'" + ")");
                        mw.WebQueryMySQL("INSERT INTO " + TableName + " (ForeignKey, Titre, Description) VALUES (" + "'" + ForeignKey + "'" + "," + "'" + dashb.SimpleQuoteFormater(TitleMesure.Text) + "'" + "," + "'" + dashb.SimpleQuoteFormater(ContentMesure.Text) + "'" + ")");
                        Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible d'ajouter à la table parent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void SupprimerTable(string TableName)
        {
            TableName = dashb.TableFormaterMesure(dashb.SimpleQuoteFormater(dashb.FormaterToSQLRequest(TableName)));
            using (ApplicationDatabase context = new ApplicationDatabase())
            {
                var x = context.Database.ExecuteSqlCommand("DROP TABLE " + TableName);
                mw.WebQueryMySQL("DROP TABLE " + TableName);
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dashb.FenetreOuverte = false;
        }
    }
}
