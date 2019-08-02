using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Vue_Mesures.xaml
    /// </summary>
    public partial class Vue_Mesure : Page
    {
        public Dashboard dashb { get; set; }
        public Mesure ROOT_Mesures { get; set; }
        public Mesure MesureSelectionnee { get; set; }

        readonly object _lockCollection = new object();

        public bool SuprDoc { get; set; }



        public Vue_Mesure()
        {
            InitializeComponent();
        }

        public Vue_Mesure(Dashboard D)
        {
            InitializeComponent();
            dashb = D;
            dashb.Vue_Mesure = this;
            ROOT_Mesures = new Mesure("Menu");
            treeviewFrame.Items.Add(ROOT_Mesures);
        }

        private void TreeviewFrame_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MesureSelectionnee = (Mesure)treeviewFrame.SelectedItem;
        }


        public void AfficherTreeViewMesure()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                ROOT_Mesures.MesuresObservCollec.Clear();
            });

            Mesure[] Li = dashb.mw.database.MesureDatabase.ToArray();
            Mesure[] Lj = Li;
            int[] Ls = new int[Lj.Length];
            int[] lar = new int[Lj.Length];

            for (int i = 0; i < Lj.Length; i++)
            {
                Ls[i] = Lj[i].Id;
            }

            for (int i = 0; i < Li.Length; i++)
            {
                int M = Li[i].Id;
                if ((Li[i].Id == Lj[i].FK_to_Mesures) && (Array.BinarySearch(Ls, M) < 0))
                {
                    lar[i] = M;
                    lock (_lockCollection)
                    {
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            dashb.mw.database.MesureDatabase.ToList()[i].MesuresObservCollec.Add(dashb.mw.database.MesureDatabase.ToList()[i]);
                        });
                        Thread.Sleep(2);
                    }
                }
                else if ((Li[i].FK_to_Mesures == 0) && (dashb.ProjetEnCours.Id == Li[i].FK_to_Projets))
                {
                    int MM = Li[i].Id;
                    if (Array.BinarySearch(lar, MM) < 0)
                    {
                        lar[i] = MM;
                        lock (_lockCollection)
                        {
                            Application.Current.Dispatcher.Invoke(delegate ()
                            {
                                ROOT_Mesures.MesuresObservCollec.Add(dashb.mw.database.MesureDatabase.ToList()[i]);
                            });
                            Thread.Sleep(2);
                        }
                    }
                }
            }

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => AfficherTreeViewMesure());
            if (dashb.Vue != null)
            {
                dashb.Vue.AfficherMesureAssociee();
            }
            else
            {
                dashb.Vue = new Vue_Exigence(dashb);
            }

        }

        private void Btn_supr_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer " + MesureSelectionnee.Name, "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                string CurrentItem = dashb.TableFormaterMesure(dashb.SimpleQuoteFormater(dashb.FormaterToSQLRequest(MesureSelectionnee.Name)));
                Mesure Ntmp = MesureSelectionnee;

                if (MessageBox.Show("Voulez - vous supprimer tous les documents associés à " + MesureSelectionnee.Name + " ? ", "Suppression des documents", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    SuprDoc = true;
                    using (ApplicationDatabase context = new ApplicationDatabase())
                    {
                        //supprime son document associé
                        var docASupr = context.Database.SqlQuery<string>("SELECT DocumentPath from Mesures WHERE Id = " + Ntmp.Id).FirstOrDefault();
                        if (docASupr != null)
                        {
                            File.Delete(docASupr);
                        }
                    }
                }

                //supprime de la BDD
                dashb.mw.database.MesureDatabase.Remove(Ntmp);
                dashb.mw.database.SaveChanges();

                using (ApplicationDatabase context = new ApplicationDatabase())
                {

                    //Quand suppression d'un parent => supprimer la table nominative des enfants
                    dashb.SuppressionTabEntantMesure(CurrentItem);

                    //supprime de la table parent
                    var ParentName = context.Database.SqlQuery<string>("SELECT Name from Exigences WHERE Id= " + Ntmp.FK_to_Mesures).FirstOrDefault();

                    var ListeEnfant = context.Database.SqlQuery<string>("SELECT * FROM " + Ntmp);

                    if (ParentName != "Menu" && ParentName != null)
                    {
                        ParentName = dashb.TableFormater(dashb.FormaterToSQLRequest(ParentName));
                        var zz = context.Database.ExecuteSqlCommand("DELETE FROM " + ParentName + " WHERE Titre = '" + dashb.SimpleQuoteFormater(Ntmp.Name) + "'");
                    }

                    // supprime la table à son nom
                    var x = context.Database.ExecuteSqlCommand("DROP TABLE " + CurrentItem);
                }

                // remove tous ses enfants de la collection Observable
                Ntmp.MesuresObservCollec.Clear();

                // remove de la liste général dans le treeview
                dashb.Vue_Mesure.ROOT_Mesures.MesuresObservCollec.Remove(Ntmp);

                //remove les relations
                dashb.Delete_linkMesure(Ntmp.Id, Ntmp);
            }
        }

        private void Btn_modif_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dashb.FenetreOuverte == false)
            {
                try
                {
                    if (dashb.Vue_Mesure.Name != "Menu")
                    {
                        Modifier_Mesure MM = new Modifier_Mesure(dashb.mw, dashb.Vue_Mesure);
                        MM.Title.Text = dashb.Vue_Mesure.MesureSelectionnee.Name;
                        MM.Content.Text = dashb.Vue_Mesure.MesureSelectionnee.Description;
                        MM.Status.Text = dashb.Vue_Mesure.MesureSelectionnee.Status.ToString();
                        MM.Document.Text = dashb.Vue_Mesure.MesureSelectionnee.DocumentName;
                        MM.Show();
                        dashb.FenetreOuverte = true;
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Selectionnez une mesure à modifier", "error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Documentviewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String fileName = MesureSelectionnee.DocumentPath;
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.Start();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Sélectionnez une mesure");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Aucun document associé");
            }
        }
    }
}
