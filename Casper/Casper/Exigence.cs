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
    public enum STATUS
    {
        non_evaluee,
        non_appliquee,
        programmee,
        appliquee,
        non_applicable
    }
    public class Exigence : INotifyPropertyChanged
    {
        public Exigence(string name, string description, int foreignkey, int foreignkeyTOnorme, int foreignkeyTOProj)
        {
            Name = name;
            Description = description;
            ForeignKey = foreignkey;
            ForeignKey_TO_Norme = foreignkeyTOnorme;
            ForeignKey_TO_Projet = foreignkeyTOProj;
            this.ExigenceObervCollec = new ObservableCollection<Exigence>();
            IDExigence = Id;

            Relation_Exigence_to_Mesures = new List<string>();

            if (ForeignKey == 0)
            {
                IsNodeExpanded = true;
            }

            Dico_couleurs = new Dictionary<STATUS, string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            GetValueDico();
        }

        public Exigence(string name, int foreignkey, int foreignkeyTOnorme, int foreignkeyTOProj)
        {
            Name = name;
            ForeignKey = foreignkey;
            ForeignKey_TO_Norme = foreignkeyTOnorme;
            ForeignKey_TO_Projet = foreignkeyTOProj;
            this.ExigenceObervCollec = new ObservableCollection<Exigence>();
            IDExigence = Id;

            Relation_Exigence_to_Mesures = new List<string>();

            if (ForeignKey == 0)
            {
                IsNodeExpanded = true;
            }

            Dico_couleurs = new Dictionary<STATUS, string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            GetValueDico();
        }

        public Exigence()
        {
            this.ExigenceObervCollec = new ObservableCollection<Exigence>();
            Dico_couleurs = new Dictionary<STATUS, string>();
            RemplirDicoCouleur();
            Status = STATUS.non_evaluee;
            Relation_Exigence_to_Mesures = new List<string>();

            if (ForeignKey == 0)
            {
                IsNodeExpanded = true;
            }
        }

        [Key]
        public int Id { get; set; }

        [InverseProperty("ForeignKey")]
        public int IDExigence { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
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

        private List<string> _Relation_Exigence_to_Mesures;
        public List<string> Relation_Exigence_to_Mesures
        {
            get { return _Relation_Exigence_to_Mesures; }
            set
            {
                _Relation_Exigence_to_Mesures = value;
                OnPropertyChanged("Relation_Exigence_to_Mesures");
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


        private STATUS _Status;

        public STATUS Status
        {
            get { return _Status; }
            set
            {
                _Status = value; GetValueDico();
                OnPropertyChanged("Status");
            }
        }

        public bool IsNodeExpanded { get; private set; }
        public int ForeignKey { get; set; }

        public int ForeignKey_TO_Norme { get; set; }
        public int ForeignKey_TO_Projet { get; set; }


        public ObservableCollection<Exigence> ExigenceObervCollec { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString()
        {
            return Name;
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
