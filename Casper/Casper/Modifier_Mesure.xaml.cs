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
    /// Logique d'interaction pour Modifier_Mesure.xaml
    /// </summary>
    public partial class Modifier_Mesure : Window
    {
        public Vue_Mesure VueMesure { get; set; }
        public MainWindow mw { get; set; }

        List<string> listeMesureCheck { get; set; }
        List<int> listeMesureCheckID { get; set; }
        public Modifier_Mesure()
        {
            InitializeComponent();
            listeMesureCheck = new List<string>();
            listeMesureCheckID = new List<int>();
        }

        public Modifier_Mesure(MainWindow m, Vue_Mesure vue)
        {
            InitializeComponent();
            VueMesure = vue;
            mw = m;
            listeMesureCheck = new List<string>();
            listeMesureCheckID = new List<int>();
        }

        private void Bouton_AjouterDocument_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            string fileNameWithID = "";
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();

            fileNameWithID = "[" + (VueMesure.MesureSelectionnee.Id) + "]" + open.SafeFileName;
            fileName = open.SafeFileName;
            string sourcePath = open.FileName;
            string targetPath = @"C:\Users\stagiaire\Desktop\ALTO-IT\Casper\Casper\bin\Debug\DocumentClient\" + fileNameWithID;

            try
            {
                File.Copy(sourcePath, targetPath);
                VueMesure.MesureSelectionnee.DocumentPath = targetPath;
                VueMesure.MesureSelectionnee.DocumentName = fileName;
            }
            catch (IOException)
            {
                if (MessageBox.Show("Le document existe déjà, voulez-vous le mettre à jour ?", "Fichier existant", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    File.Delete(targetPath);
                    File.Copy(sourcePath, targetPath);
                    VueMesure.MesureSelectionnee.DocumentPath = targetPath;
                    VueMesure.MesureSelectionnee.DocumentName = fileName;

                }
            }
            mw.database.SaveChanges();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VueMesure.dashb.FenetreOuverte = false;
            VueMesure.dashb.Vue.AfficherMesureAssociee();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            listeMesureCheck.Add(check.Content.ToString());

            IQueryable<int> recupID = (from m in mw.database.ExigenceDatabase
                                       where m.Name == check.Content.ToString()
                                       select m.Id);

            var M = from m in mw.database.ExigenceDatabase
                    where m.Name == check.Content.ToString()
                    select m;

            listeMesureCheckID.Add(recupID.FirstOrDefault());

            RelationMesureExigence rme = new RelationMesureExigence(recupID.FirstOrDefault(), VueMesure.MesureSelectionnee.Id);
            mw.WebQueryMySQL("INSERT INTO RelationMesureExigence (IdExigence, IdMesure) VALUES (" + recupID.FirstOrDefault() +  "," + VueMesure.MesureSelectionnee.Id+")");

            mw.database.RelationMesureExigenceDatabase.Add(rme);

            mw.database.SaveChanges();

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox uncheck = (CheckBox)sender;
            listeMesureCheck.Remove(uncheck.Content.ToString());

            IQueryable<int> recupID = (from m in mw.database.ExigenceDatabase
                                       where m.Name == uncheck.Content.ToString()
                                       select m.Id);

            listeMesureCheckID.Remove(recupID.FirstOrDefault());

            var recherheRelation = from n in mw.database.RelationMesureExigenceDatabase
                                   where n.IdExigence == VueMesure.MesureSelectionnee.Id && n.IdMesure == recupID.FirstOrDefault()
                                   select n;

            var M = from m in mw.database.ExigenceDatabase
                    where m.Name == uncheck.Content.ToString()
                    select m;

            mw.database.RelationMesureExigenceDatabase.Remove(recherheRelation.FirstOrDefault());
            mw.WebQueryMySQL("DELTE FROM RelationMesureExigence WHERE IdExigence = " + recupID.FirstOrDefault() + "AND IdMesure = " + VueMesure.MesureSelectionnee.Id);
            mw.database.SaveChanges();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var cc = from aa in mw.database.RelationMesureExigenceDatabase
                     where aa.IdMesure == VueMesure.MesureSelectionnee.Id
                     select aa.IdExigence;


            List<int> IDexigence = cc.ToList();
            List<CheckBox> listcheck = new List<CheckBox>();

            foreach (Exigence item in mw.database.ExigenceDatabase)
            {
                if (item.ForeignKey_TO_Projet == VueMesure.dashb.ProjetEnCours.Id)
                {
                    CheckBox ch = new CheckBox();
                    ch.Content = item.Name;

                    if (IDexigence.Contains(item.Id))
                    {
                        ch.IsChecked = true;
                    }
                    listcheck.Add(ch);
                    mw.database.SaveChanges();
                }
            }
            ListesDesExigences.ItemsSource = listcheck;

            foreach (CheckBox item in listcheck)
            {
                item.Checked += CheckBox_Checked;
                item.Unchecked += CheckBox_Unchecked;

            }
        }

        private void ModifierMesure_Click(object sender, RoutedEventArgs e)
        {
            if (VueMesure.MesureSelectionnee != null && VueMesure.MesureSelectionnee.Name != "Menu")
            {
                string CurrentItem = VueMesure.dashb.FormaterToSQLRequest("_" + VueMesure.dashb.ProjetEnCours.Id + VueMesure.MesureSelectionnee.Name);
                string CurrentDescription = VueMesure.dashb.SimpleQuoteFormater(Content.Text);
                string CurrentTitle = VueMesure.dashb.SimpleQuoteFormater(Title.Text);

                using (ApplicationDatabase context = new ApplicationDatabase())
                {
                    string newTableName = VueMesure.dashb.TableFormaterMesure(VueMesure.dashb.SimpleQuoteFormater(VueMesure.dashb.FormaterToSQLRequest(Title.Text)));
                    try
                    {
                        if (CurrentItem != newTableName)
                        {   //renomme la table
                            var w = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + CurrentItem + "', '" + newTableName + "'");
                            mw.WebQueryMySQL("RENAME TABLE " + CurrentItem + " TO " + newTableName + "");
                        }

                        //modif dans la table Mesure
                        var yy = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Description = '" + VueMesure.dashb.SimpleQuoteFormater(Content.Text) + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                        mw.WebQueryMySQL("UPDATE Mesures" + " SET Description = '" + VueMesure.dashb.SimpleQuoteFormater(Content.Text) + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                        var y = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Name = '" + VueMesure.dashb.SimpleQuoteFormater(Title.Text) + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                        mw.WebQueryMySQL("UPDATE Mesures" + " SET Name = '" + VueMesure.dashb.SimpleQuoteFormater(Title.Text) + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");

                        try
                        {
                            //modif dans table parents
                            var ParentName = context.Database.SqlQuery<string>("SELECT Name from Mesures WHERE Id= " + VueMesure.MesureSelectionnee.FK_to_Mesures).FirstOrDefault();
                            if (ParentName != "Menu" && ParentName != null)
                            {
                                ParentName = VueMesure.dashb.TableFormaterMesure(VueMesure.dashb.SimpleQuoteFormater(VueMesure.dashb.FormaterToSQLRequest(ParentName)));

                                var zz = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Description = '" + VueMesure.dashb.SimpleQuoteFormater(Content.Text) + "' WHERE Titre = '" + VueMesure.MesureSelectionnee.Name + "'");
                                mw.WebQueryMySQL("UPDATE " + ParentName + " SET Description = '" + VueMesure.dashb.SimpleQuoteFormater(Content.Text) + "' WHERE Titre = '" + VueMesure.MesureSelectionnee.Name + "'");

                                var z = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Titre = '" + VueMesure.dashb.SimpleQuoteFormater(Title.Text) + "' WHERE Titre = '" + VueMesure.MesureSelectionnee.Name + "'");
                                mw.WebQueryMySQL("UPDATE " + ParentName + " SET Titre = '" + VueMesure.dashb.SimpleQuoteFormater(Title.Text) + "' WHERE Titre = '" + VueMesure.MesureSelectionnee.Name + "'");

                            }

                            //actualise l'itemSleceted et la Vue grâce INotifyProperty
                            VueMesure.MesureSelectionnee.Name = Title.Text;
                            VueMesure.MesureSelectionnee.Description = Content.Text;

                        }
                        catch (Exception)
                        {
                            var w2 = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + newTableName + "', '" + CurrentItem + "'");
                            mw.WebQueryMySQL("RENAME TABLE " + newTableName + " TO " + CurrentItem + "");
                            var yy2 = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Description = '" + CurrentDescription + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                            mw.WebQueryMySQL("UPDATE Mesures" + " SET Description = '" + CurrentDescription + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                            var y2 = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Name = '" + CurrentTitle + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                            mw.WebQueryMySQL("UPDATE Mesures" + " SET Name = '" + CurrentTitle + "' WHERE Id = " + "'" + VueMesure.MesureSelectionnee.Id + "'");
                            MessageBox.Show("Modification impossible", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible d'actualiser la BDD", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    switch (ComboBoxStatus.Text)
                    {
                        case "Non Évaluée":
                            VueMesure.MesureSelectionnee.Status = STATUS.non_evaluee;
                            break;
                        case "Non Appliquée":
                            VueMesure.MesureSelectionnee.Status = STATUS.non_appliquee;
                            break;
                        case "Programmée":
                            VueMesure.MesureSelectionnee.Status = STATUS.programmee;
                            break;
                        case "Appliquée":
                            VueMesure.MesureSelectionnee.Status = STATUS.appliquee;
                            break;
                        case "Non Applicable":
                            VueMesure.MesureSelectionnee.Status = STATUS.non_applicable;
                            break;
                        default:
                            break;
                    }

                    mw.database.SaveChanges();
                    VueMesure.AfficherTreeViewMesure();
                    Close();
                }
            }
        }
    }
}
