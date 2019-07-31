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
    }
}
