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
    public class Norme
    {
        [Key]
        public int Id { get; set; }


        private string _Nom_Norme;

        public Norme(string nom_Norme, int IDProj)
        {
            Nom_Norme = nom_Norme;
            FK_to_Projet = IDProj;
            NormeObervCollec = new ObservableCollection<Norme>();
            IDNorme = Id;
        }

        public Norme() { }

        public string Nom_Norme
        {
            get { return _Nom_Norme; }
            set
            {
                _Nom_Norme = value;
                OnPropertyChanged("Nom_Norme");
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

        [InverseProperty("ForeignKey_TO_Norme")]
        public int IDNorme { get; set; }

        public int FK_to_Projet { get; set; }

        public ObservableCollection<Norme> NormeObervCollec { get; set; }

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
            return Nom_Norme;
        }
    }
}
