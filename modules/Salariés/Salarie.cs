using modules.Salariés;
using System;
using System.Text.Json.Serialization; 

namespace modules.Salariés
{
    [JsonDerivedType(typeof(Responsable), typeDiscriminator: "Responsable")]
    [JsonDerivedType(typeof(Chauffeur), typeDiscriminator: "Chauffeur")]
    public class Salarie : IEquatable<Salarie>
    {
        public string NumeroSecuriteSociale { get; init; }
        public DateTime DateNaissance { get; init; }
        public DateTime DateEntreeSociete { get; init; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string AdressePostale { get; set; }
        public string AdresseMail { get; set; }
        public string Telephone { get; set; }
        public virtual string Poste { get; set; }
        public decimal Salaire { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        public string ManagerNumeroSS { get; set; }

        public Salarie(string numeroSS, string nom, string prenom, DateTime dateNaissance, DateTime dateEntree)
        {
            if (string.IsNullOrWhiteSpace(numeroSS)) throw new ArgumentNullException(nameof(numeroSS));
            if (string.IsNullOrWhiteSpace(nom)) throw new ArgumentNullException(nameof(nom));
            if (string.IsNullOrWhiteSpace(prenom)) throw new ArgumentNullException(nameof(prenom));

            NumeroSecuriteSociale = numeroSS;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            DateEntreeSociete = dateEntree;
            Poste = "Salarié"; 
            Salaire = 0; 
            AdressePostale = string.Empty;
            AdresseMail = string.Empty;
            Telephone = string.Empty;
            ManagerNumeroSS = null; 
        }

        [JsonConstructor]
        public Salarie(string numeroSecuriteSociale, DateTime dateNaissance, DateTime dateEntreeSociete, string nom, string prenom, string adressePostale, string adresseMail, string telephone, string poste, decimal salaire, string managerNumeroSS)
        {
            NumeroSecuriteSociale = numeroSecuriteSociale;
            DateNaissance = dateNaissance;
            DateEntreeSociete = dateEntreeSociete;
            Nom = nom;
            Prenom = prenom;
            AdressePostale = adressePostale;
            AdresseMail = adresseMail;
            Telephone = telephone;
            Poste = poste;
            Salaire = salaire;
            ManagerNumeroSS = managerNumeroSS;
        }


        public override string ToString()
        {
            return $"[{Poste}] {Prenom} {Nom} (ID: {NumeroSecuriteSociale})";
        }

        public bool Equals(Salarie other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return NumeroSecuriteSociale == other.NumeroSecuriteSociale;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Salarie);
        }

        public override int GetHashCode()
        {
            return NumeroSecuriteSociale?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Salarie left, Salarie right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Salarie left, Salarie right)
        {
            return !Equals(left, right);
        }
    }
}