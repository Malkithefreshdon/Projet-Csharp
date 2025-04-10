using System;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    public class Responsable : Salarie
    {

        public override string Poste { get; set; } = "Responsable"; 

        public Responsable(string numeroSS, string nom, string prenom, DateTime dateNaissance, DateTime dateEntree)
            : base(numeroSS, nom, prenom, dateNaissance, dateEntree)
        {
        }

        [JsonConstructor]
        public Responsable(string numeroSecuriteSociale, DateTime dateNaissance, DateTime dateEntreeSociete, string nom, string prenom, string adressePostale, string adresseMail, string telephone, string poste, double salaire, string managerNumeroSS)
         : base(numeroSecuriteSociale, dateNaissance, dateEntreeSociete, nom, prenom, adressePostale, adresseMail, telephone, poste, salaire, managerNumeroSS)
        {
            Poste = "Responsable";
        }
    }
}