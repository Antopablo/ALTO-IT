using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alto_IT
{
    public class User
    {
        public User(string identifiant, string password)
        {
            Identifiant = identifiant;
            Password = password;
        }

        public User() // indispensable pour les requêtes LINQ
        {
        }

        [Key]
        public int Id { get; set; }

        public string Identifiant { get; set; }

        public string Password { get; set; }
    }
}
