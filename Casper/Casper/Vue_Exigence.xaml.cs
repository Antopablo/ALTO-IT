using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Vue_Circulaire.xaml
    /// </summary>
    /// 

    public partial class Vue_Exigence : Page
    {
        public Dashboard dash { get; set; }
        public Exigence ExigenceSelectionnee { get; set; }
        public List<Exigence> ListeExigence { get; set; }
        public Exigence ROOT_Exigences { get; set; }

        readonly object _lockCollection = new object();

        public Vue_Exigence()
        {
            InitializeComponent();
        }

        public Vue_Exigence(Dashboard D)
        {
            InitializeComponent();
            dash = D;
            dash.Vue = this;
            ROOT_Exigences = new Exigence() { Name = "Menu" }; //modifier le nom entraine un changement dans plusieurs classes            
            treeviewFrame.Items.Add(ROOT_Exigences);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(AfficherTreeViewExigences);
            AfficherMesureAssociee();
        }

        public void AfficherTreeViewExigences()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                ROOT_Exigences.ExigenceObervCollec.Clear();
            });

            Exigence[] Li = dash.mw.database.ExigenceDatabase.ToArray();
            Exigence[] Lj = Li;
            int[] Ls = new int[Lj.Length];
            int[] lar = new int[Lj.Length];

            for (int i = 0; i < Lj.Length; i++)
            {
                Ls[i] = Lj[i].Id;
            }
            for (int i = 0; i < Li.Length; i++)
            {
                int M = Li[i].Id;
                if ((Li[i].Id == Lj[i].ForeignKey) && (Array.BinarySearch(Ls, M) < 0))
                {
                    lar[i] = M;
                    lock (_lockCollection)
                    {
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            dash.mw.database.ExigenceDatabase.ToList()[i].ExigenceObervCollec.Add(dash.mw.database.ExigenceDatabase.ToList()[i]);
                        });
                        Thread.Sleep(2);
                    }
                }
                else if ((Li[i].ForeignKey == 0) && (dash.NormeSelectionnee.Id == Li[i].ForeignKey_TO_Norme))
                {
                    int MM = Li[i].Id;
                    if (Array.BinarySearch(lar, MM) < 0)
                    {
                        lar[i] = MM;
                        lock (_lockCollection)
                        {
                            Application.Current.Dispatcher.Invoke(delegate ()
                            {
                                ROOT_Exigences.ExigenceObervCollec.Add(dash.mw.database.ExigenceDatabase.ToList()[i]);
                            });
                            Thread.Sleep(2);
                        }
                    }
                }
            }
        }


        private void TreeviewFrame_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ExigenceSelectionnee = (Exigence)treeviewFrame.SelectedItem;

        }

 

        public void AfficherMesureAssociee()
        {
            if (dash.mw.database.MesureDatabase.Count() > 0)
            {
                List<Exigence> exigencesPresente = new List<Exigence>();

                // récupère toutes les exigences du treeview
                foreach (Exigence item in dash.mw.database.ExigenceDatabase)
                {
                    if (item.ForeignKey_TO_Norme == dash.NormeSelectionnee.Id)
                    {
                        exigencesPresente.Add(item);

                    }
                }

                // récupère tout les ID mesures qui sont associé a chaque exigence
                foreach (Exigence item in exigencesPresente)
                {
                    item.Relation_Exigence_to_Mesures.Clear();
                    List<int> ID = (from a in dash.mw.database.RelationMesureExigenceDatabase
                                    where a.IdExigence == item.Id
                                    select a.IdMesure).ToList();

                    foreach (int mesuresID in ID)
                    {
                        // retrouve l'objet mesure
                        var mesure = (from b in dash.mw.database.MesureDatabase
                                      where b.Id == mesuresID
                                      select b).FirstOrDefault();

                        // ajoute dans la liste Exigence 'item' le nom de la mesure associé
                        item.Relation_Exigence_to_Mesures.Add(mesure.Name);
                        item.Relation_Exigence_to_Mesures.Sort();

                        // ajoute dans la mesure, l'exigence 'item' associé
                        if (!mesure.Relation_Mesures_to_exigences.Contains(item.Name))
                        {
                            mesure.Relation_Mesures_to_exigences.Add(item.Name);
                        }

                    }

                }
            }
        }

        private void Btn_supr_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (dash.Vue.ExigenceSelectionnee != null && dash.Vue.ExigenceSelectionnee.Name != "Menu")
            {
                if (MessageBox.Show("Voulez-vous supprimer " + dash.Vue.ExigenceSelectionnee.Name, "Suppression de l'exigence", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    string CurrentItem = dash.TableFormater(dash.SimpleQuoteFormater(dash.FormaterToSQLRequest(dash.Vue.ExigenceSelectionnee.Name)));

                    Exigence Ntmp = dash.Vue.ExigenceSelectionnee;

                    if (MessageBox.Show("Voulez - vous supprimer tous les documents associés à " + dash.Vue.ExigenceSelectionnee.Name + " ? ", "Suppression des documents", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        dash.SuprDoc = true;
                        using (ApplicationDatabase context = new ApplicationDatabase())
                        {
                            //supprime son document associé
                            var docASupr = context.Database.SqlQuery<string>("SELECT DocumentPath from Exigences WHERE Id = " + Ntmp.Id).FirstOrDefault();
                            if (docASupr != null)
                            {
                                File.Delete(docASupr);
                            }

                            //supprime des documents enfant 
                            // TODO
                        }
                    }

                    // Supprime de la DbSet, à mettre en 1er
                    dash.mw.database.ExigenceDatabase.Remove(Ntmp);
                    dash.mw.database.SaveChanges();


                    using (ApplicationDatabase context = new ApplicationDatabase())
                    {

                        //Quand suppression d'un parent => supprimer la table nominative des enfants
                        dash.SuppressionTabEntant(CurrentItem);

                        //supprime de la table parent
                        var ParentName = context.Database.SqlQuery<string>("SELECT Name from Exigences WHERE Id= " + Ntmp.ForeignKey).FirstOrDefault();

                        var ListeEnfant = context.Database.SqlQuery<string>("SELECT * FROM " + Ntmp);

                        if (ParentName != "Menu" && ParentName != null)
                        {
                            ParentName = dash.TableFormater(dash.FormaterToSQLRequest(ParentName));
                            var zz = context.Database.ExecuteSqlCommand("DELETE FROM " + ParentName + " WHERE Titre = '" + dash.SimpleQuoteFormater(Ntmp.Name) + "'");
                        }

                        // supprime la table à son nom
                        var x = context.Database.ExecuteSqlCommand("DROP TABLE " + CurrentItem);
                    }

                    // remove tous ses enfants de la collection Observable
                    Ntmp.ExigenceObervCollec.Clear();

                    // remove de la liste général dans le treeview
                    dash.Vue.ROOT_Exigences.ExigenceObervCollec.Remove(Ntmp);
                }
            }
            else
            {
                MessageBox.Show("Selectionner une ligne", "error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Btn_modif_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dash.FenetreOuverte == false)
            {
                try
                {
                    if (dash.Vue.ExigenceSelectionnee.Name != "Menu")
                    {
                        Modifier_Exigence M = new Modifier_Exigence(dash.mw, dash.Vue);
                        M.Title.Text = dash.Vue.ExigenceSelectionnee.Name;
                        M.Content.Text = dash.Vue.ExigenceSelectionnee.Description;
                        M.Show();
                        dash.FenetreOuverte = true;
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Selectionnez une exigence à modifier", "error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
