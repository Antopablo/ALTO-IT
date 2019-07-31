using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alto_IT
{
    public class Mesure : INotifyPropertyChanged
    {
        public Mesure()
        {
            MesuresObservCollec = new ObservableCollection<Mesure>();
            Dico_couleurs = new Dictionary<STATUS, string>();
            Relation_Mesures_to_exigences = new List<string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            if (FK_to_Mesures == 0)
            {
                IsNodeExpanded = true;
            }
        }

        public Mesure(string name)
        {
            Name = name;
            MesuresObservCollec = new ObservableCollection<Mesure>();
            Relation_Mesures_to_exigences = new List<string>();
            Dico_couleurs = new Dictionary<STATUS, string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            if (FK_to_Mesures == 0)
            {
                IsNodeExpanded = true;
            }
        }

        public Mesure(string name, string description, int ForeignKeyM, int foreignKeyP)
        {
            Name = name;
            Description = description;
            FK_to_Mesures = ForeignKeyM;
            FK_to_Projets = foreignKeyP;
            MesuresObservCollec = new ObservableCollection<Mesure>();
            Relation_Mesures_to_exigences = new List<string>();
            Dico_couleurs = new Dictionary<STATUS, string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            if (FK_to_Mesures == 0)
            {
                IsNodeExpanded = true;
            }
        }

        [Key]
        public int Id { get; set; }
        public int FK_to_Projets { get; set; }
        public int FK_to_Mesures { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private string _DocumentPath;
        public string DocumentPath
        {
            get { return _DocumentPath; }
            set
            {
                _DocumentPath = value;
                OnPropertyChanged("DocumentPath");
            }
        }

        private string _DocumentName;
        public string DocumentName

        {
            get { return _DocumentName; }
            set
            {
                _DocumentName = value;
                OnPropertyChanged("DocumentName");
            }
        }

        private STATUS _Status;

        public STATUS Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                GetValueDico();
                OnPropertyChanged("Status");
            }
        }

        private List<string> _Relation_Mesures_to_exigences;
        public List<string> Relation_Mesures_to_exigences
        {
            get { return _Relation_Mesures_to_exigences; }
            set
            {
                _Relation_Mesures_to_exigences = value;
                OnPropertyChanged("Relation_Mesures_to_exigences");
            }
        }

        [NotMapped]
        public Dictionary<STATUS, string> Dico_couleurs { get; set; }

        private string _Couleur;
        [NotMapped]
        public string Couleur
        {
            get { return _Couleur; }
            set
            {
                _Couleur = value;
                OnPropertyChanged("Couleur");
            }
        }

        public bool IsNodeExpanded { get; private set; }

        public ObservableCollection<Mesure> MesuresObservCollec { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void RemplirDicoCouleur()
        {
            Dico_couleurs.Add(STATUS.appliquee, "LimeGreen");
            Dico_couleurs.Add(STATUS.non_applicable, "DarkSlateGray");
            Dico_couleurs.Add(STATUS.non_appliquee, "Crimson");
            Dico_couleurs.Add(STATUS.non_evaluee, "CadetBlue");
            Dico_couleurs.Add(STATUS.programmee, "Orange");
        }

        public void GetValueDico()
        {
            Couleur = Dico_couleurs[Status];
        }
    }
}
