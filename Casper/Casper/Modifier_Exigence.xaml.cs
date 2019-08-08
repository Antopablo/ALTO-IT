using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Modifier.xaml
    /// </summary>
    public partial class Modifier_Exigence : Window
    {
        public MainWindow mw { get; set; }
        public Vue_Exigence Vue { get; set; }

        List<string> listeMesureCheck { get; set; }
        List<int> listeMesureCheckID { get; set; }

        readonly object locker = new object();

        public Modifier_Exigence()
        {
            InitializeComponent();
            listeMesureCheck = new List<string>();
            listeMesureCheckID = new List<int>();
        }

        public Modifier_Exigence(MainWindow m, Vue_Exigence v)
        {
            InitializeComponent();
            mw = m;
            Vue = v;
            mw.database.MesureDatabase.ToList();
            listeMesureCheck = new List<string>();
            listeMesureCheckID = new List<int>();
        }

        private void ModifierExigence_Click(object sender, RoutedEventArgs e)
        {
            if (Vue.ExigenceSelectionnee != null && Vue.ExigenceSelectionnee.Name != "Menu")
            {
                string CurrentItem = Vue.dash.FormaterToSQLRequest("_" + Vue.dash.NormeSelectionnee.Id + Vue.ExigenceSelectionnee.Name);
                string CurrentDescription = Vue.dash.SimpleQuoteFormater(Content.Text);
                string CurrentTitle = Vue.dash.SimpleQuoteFormater(Title.Text);

                using (ApplicationDatabase context = new ApplicationDatabase())
                {
                    string newTableName = Vue.dash.TableFormater(Vue.dash.SimpleQuoteFormater(Vue.dash.FormaterToSQLRequest(Title.Text)));
                    try
                    {
                        if (CurrentItem != newTableName)
                        {
                            //renomme la table
                            var w = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + CurrentItem + "', '" + newTableName + "'");
                            mw.WebQueryMySQL("RENAME TABLE " + CurrentItem + " TO " + newTableName + "");
                        }

                        //modif dans la table Exigence
                        var yy = context.Database.ExecuteSqlCommand("UPDATE Exigences SET Description = '" + Vue.dash.SimpleQuoteFormater(Content.Text) + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                        mw.WebQueryMySQL("UPDATE Exigences SET Description = '" + Vue.dash.SimpleQuoteFormater(Content.Text) + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                        var y = context.Database.ExecuteSqlCommand("UPDATE Exigences SET Name = '" + Vue.dash.SimpleQuoteFormater(Title.Text) + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                        mw.WebQueryMySQL("UPDATE Exigences SET Name = '" + Vue.dash.SimpleQuoteFormater(Title.Text) + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");


                        try
                        {
                            //modif dans table parents
                            var ParentName = context.Database.SqlQuery<string>("SELECT Name from Exigences WHERE Id= " + Vue.ExigenceSelectionnee.ForeignKey).FirstOrDefault();
                            if (ParentName != "Menu" && ParentName != null)
                            {
                                ParentName = Vue.dash.TableFormater(Vue.dash.SimpleQuoteFormater(Vue.dash.FormaterToSQLRequest(ParentName)));

                                var zz = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Description = '" + Vue.dash.SimpleQuoteFormater(Content.Text) + "' WHERE Titre = '" + Vue.ExigenceSelectionnee.Name + "'");
                                mw.WebQueryMySQL("UPDATE " + ParentName + " SET Description = '" + Vue.dash.SimpleQuoteFormater(Content.Text) + "' WHERE Titre = '" + Vue.ExigenceSelectionnee.Name + "'");
                                var z = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Titre = '" + Vue.dash.SimpleQuoteFormater(Title.Text) + "' WHERE Titre = '" + Vue.ExigenceSelectionnee.Name + "'");
                                mw.WebQueryMySQL("UPDATE " + ParentName + " SET Titre = '" + Vue.dash.SimpleQuoteFormater(Title.Text) + "' WHERE Titre = '" + Vue.ExigenceSelectionnee.Name + "'");
                            }

                            //actualise l'itemSleceted et la Vue grâce INotifyProperty
                            Vue.ExigenceSelectionnee.Name = Title.Text;
                            Vue.ExigenceSelectionnee.Description = Content.Text;

                        }
                        catch (Exception)
                        {
                            var w2 = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + newTableName + "', '" + CurrentItem + "'");
                            mw.WebQueryMySQL("RENAME TABLE " + newTableName + " TO " + CurrentItem + "");
                            var yy2 = context.Database.ExecuteSqlCommand("UPDATE Exigences" + " SET Description = '" + CurrentDescription + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                            mw.WebQueryMySQL("UPDATE Exigences" + " SET Description = '" + CurrentDescription + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                            var y2 = context.Database.ExecuteSqlCommand("UPDATE Exigences" + " SET Name = '" + CurrentTitle + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                            mw.WebQueryMySQL("UPDATE Exigences" + " SET Name = '" + CurrentTitle + "' WHERE Id = " + "'" + Vue.ExigenceSelectionnee.Id + "'");
                            MessageBox.Show("Modification impossible", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible d'actualiser la BDD", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                       mw.database.SaveChanges();
                       Vue.AfficherTreeViewExigences();
                       Close();

                }
            }
            else
            {
                MessageBox.Show("Selectionner une ligne", "error", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Vue.dash.FenetreOuverte = false;
            Vue.AfficherMesureAssociee();
        }

        private void Bouton_AjouterDocument_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            string fileNameWithID = "";
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();

            fileNameWithID = "[" + (Vue.ExigenceSelectionnee.Id) + "]" + open.SafeFileName;
            fileName = open.SafeFileName;
            string sourcePath = open.FileName;
            string targetPath = @"C:\Users\stagiaire\Desktop\ALTO-IT\Casper\Casper\bin\Debug\DocumentClient\" + fileNameWithID;

            try
            {
                File.Copy(sourcePath, targetPath);
                Vue.ExigenceSelectionnee.DocumentPath = targetPath;
                Vue.ExigenceSelectionnee.DocumentName = fileName;
            }
            catch (IOException)
            {
                if (MessageBox.Show("Le document existe déjà, voulez-vous le mettre à jour ?", "Fichier existant", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    File.Delete(targetPath);
                    File.Copy(sourcePath, targetPath);
                    Vue.ExigenceSelectionnee.DocumentPath = targetPath;
                    Vue.ExigenceSelectionnee.DocumentName = fileName;

                }
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            listeMesureCheck.Add(check.Content.ToString());

            IQueryable<int> recupID = (from m in mw.database.MesureDatabase
                                       where m.Name == check.Content.ToString()
                                       select m.Id);

            var M = from m in mw.database.MesureDatabase
                    where m.Name == check.Content.ToString()
                    select m;

            listeMesureCheckID.Add(recupID.FirstOrDefault());

            RelationMesureExigence rme = new RelationMesureExigence(Vue.ExigenceSelectionnee.Id, recupID.FirstOrDefault());
            mw.database.RelationMesureExigenceDatabase.Add(rme);
            mw.WebQueryMySQL("INSERT INTO RelationMesureExigences (IdExigence, IdMesure) VALUES ("+ Vue.ExigenceSelectionnee.Id+","+ recupID.FirstOrDefault()+")");
            mw.database.SaveChanges();

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox uncheck = (CheckBox)sender;
            listeMesureCheck.Remove(uncheck.Content.ToString());

            IQueryable<int> recupID = (from m in mw.database.MesureDatabase
                                       where m.Name == uncheck.Content.ToString()
                                       select m.Id);

            listeMesureCheckID.Remove(recupID.FirstOrDefault());

            var recherheRelation = from n in mw.database.RelationMesureExigenceDatabase
                                   where n.IdExigence == Vue.ExigenceSelectionnee.Id && n.IdMesure == recupID.FirstOrDefault()
                                   select n;

            var M = from m in mw.database.MesureDatabase
                    where m.Name == uncheck.Content.ToString()
                    select m;

            mw.database.RelationMesureExigenceDatabase.Remove(recherheRelation.FirstOrDefault());
            mw.WebQueryMySQL("DELTE FROM RelationMesureExigences WHERE IdExigence = " + Vue.ExigenceSelectionnee.Id + "AND IdMesure = " + recupID.FirstOrDefault());
            mw.database.SaveChanges();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var c = from a in mw.database.RelationMesureExigenceDatabase
                    where a.IdExigence == Vue.ExigenceSelectionnee.Id
                    select a.IdMesure;

            List<int> IDMesure = c.ToList();
            List<CheckBox> listcheck = new List<CheckBox>();

            foreach (Mesure item in mw.database.MesureDatabase)
            {
                if (item.FK_to_Projets == Vue.dash.ProjetEnCours.Id)
                {
                    CheckBox ch = new CheckBox();
                    ch.Content = item.Name;

                    if (IDMesure.Contains(item.Id))
                    {
                        ch.IsChecked = true;
                    }
                    listcheck.Add(ch);
                    mw.database.SaveChanges();
                }
            }
            ListeDesMesures.ItemsSource = listcheck;

            foreach (CheckBox item in listcheck)
            {
                item.Checked += CheckBox_Checked;
                item.Unchecked += CheckBox_Unchecked;

            }
        }
    }
}
