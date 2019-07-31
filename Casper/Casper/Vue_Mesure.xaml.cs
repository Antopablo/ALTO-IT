using System;
using System.Collections.Generic;
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
            await Task.Run(AfficherTreeViewMesure);
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
                String fileName = dashb.NormeSelectionnee.DocumentPath;
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

        private void ExigenceAssocie_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
