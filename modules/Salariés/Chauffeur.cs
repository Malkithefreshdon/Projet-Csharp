using modules.Graphes;
using System;
using System.Text.Json.Serialization;

namespace modules.Salariés
{
    public class Chauffeur : Salarie
    {
        public override string Poste { get; set; } = "Chauffeur";

        // Optionnel : Propriétés spécifiques aux chauffeurs
        // public string TypePermis { get; set; }
        // public bool Disponible { get; set; } = true;

        public Chauffeur(string numeroSS, string nom, string prenom, DateTime dateNaissance, DateTime dateEntree)
            : base(numeroSS, nom, prenom, dateNaissance, dateEntree)
        {
        }

        [JsonConstructor]
        public Chauffeur(string numeroSecuriteSociale, DateTime dateNaissance, DateTime dateEntreeSociete, string nom, string prenom, string adressePostale, string adresseMail, string telephone, string poste, decimal salaire, string managerNumeroSS)
         : base(numeroSecuriteSociale, dateNaissance, dateEntreeSociete, nom, prenom, adressePostale, adresseMail, telephone, poste, salaire, managerNumeroSS)
        {
            Poste = "Chauffeur";
        }
    }
}