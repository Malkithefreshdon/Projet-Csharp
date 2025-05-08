using System;
using System.Text.Json.Serialization; 

namespace Projet.Modules
{
    /// <summary>
    /// Représente un salarié de l'entreprise.
    /// </summary>
    public class Salarie : IEquatable<Salarie>
    {
        public string NumeroSecuriteSociale { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Poste { get; set; }
        public DateTime DateNaissance { get; set; }
        public DateTime DateEntreeSociete { get; set; }
        public string AdressePostale { get; set; }
        public string AdresseMail { get; set; }
        public string Telephone { get; set; }
        public decimal Salaire { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        public string ManagerNumeroSS { get; set; }

        /// <summary>
        /// Constructeur par défaut nécessaire pour la désérialisation JSON.
        /// </summary>
        public Salarie() { }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Salarie avec les informations de base.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale</param>
        /// <param name="nom">Nom du salarié</param>
        /// <param name="prenom">Prénom du salarié</param>
        /// <param name="dateNaissance">Date de naissance</param>
        /// <param name="dateEntree">Date d'entrée dans la société</param>
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

        /// <summary>
        /// Constructeur utilisé pour la désérialisation JSON avec tous les champs.
        /// </summary>
        /// <param name="numeroSecuriteSociale">Numéro de sécurité sociale</param>
        /// <param name="dateNaissance">Date de naissance</param>
        /// <param name="dateEntreeSociete">Date d'entrée dans la société</param>
        /// <param name="nom">Nom du salarié</param>
        /// <param name="prenom">Prénom du salarié</param>
        /// <param name="adressePostale">Adresse postale</param>
        /// <param name="adresseMail">Adresse mail</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="poste">Poste occupé</param>
        /// <param name="salaire">Salaire</param>
        /// <param name="managerNumeroSS">Numéro de sécurité sociale du manager</param>
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

        /// <summary>
        /// Retourne une représentation textuelle du salarié.
        /// </summary>
        /// <returns>Chaîne décrivant le salarié.</returns>
        public override string ToString()
        {
            return $"[{Poste}] {Prenom} {Nom} (ID: {NumeroSecuriteSociale})";
        }

        /// <summary>
        /// Détermine si l'objet spécifié est égal à l'objet courant (comparaison sur le numéro de sécurité sociale).
        /// </summary>
        /// <param name="other">L'autre salarié à comparer.</param>
        /// <returns>True si les objets sont égaux, sinon False.</returns>
        public bool Equals(Salarie other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return NumeroSecuriteSociale == other.NumeroSecuriteSociale;
        }

        /// <summary>
        /// Détermine si l'objet spécifié est égal à l'objet courant.
        /// </summary>
        /// <param name="obj">L'objet à comparer avec l'objet courant.</param>
        /// <returns>True si les objets sont égaux, sinon False.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Salarie);
        }

        /// <summary>
        /// Retourne le code de hachage pour cette instance.
        /// </summary>
        /// <returns>Code de hachage basé sur le numéro de sécurité sociale.</returns>
        public override int GetHashCode()
        {
            return NumeroSecuriteSociale?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Détermine si deux instances de Salarie sont égales.
        /// </summary>
        /// <param name="left">Premier salarié.</param>
        /// <param name="right">Deuxième salarié.</param>
        /// <returns>True si les deux salariés sont égaux, sinon False.</returns>
        public static bool operator ==(Salarie left, Salarie right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Détermine si deux instances de Salarie sont différentes.
        /// </summary>
        /// <param name="left">Premier salarié.</param>
        /// <param name="right">Deuxième salarié.</param>
        /// <returns>True si les deux salariés sont différents, sinon False.</returns>
        public static bool operator !=(Salarie left, Salarie right)
        {
            return !Equals(left, right);
        }
    }
}