using System;
using System.Text.Json.Serialization; 

#nullable enable

namespace Projet.Modules
{
    /// <summary>
    /// Représente un salarié de l'entreprise.
    /// </summary>
    public class Salarie : IEquatable<Salarie?>
    {
        /// <summary>
        /// Obtient ou définit le numéro de sécurité sociale du salarié.
        /// </summary>
        public required string NumeroSecuriteSociale { get; set; }

        /// <summary>
        /// Obtient ou définit le nom du salarié.
        /// </summary>
        public required string Nom { get; set; }

        /// <summary>
        /// Obtient ou définit le prénom du salarié.
        /// </summary>
        public required string Prenom { get; set; }

        /// <summary>
        /// Obtient ou définit le poste occupé par le salarié.
        /// </summary>
        public required string Poste { get; set; }

        /// <summary>
        /// Obtient ou définit la date de naissance du salarié.
        /// </summary>
        public DateTime DateNaissance { get; set; }

        /// <summary>
        /// Obtient ou définit la date d'entrée dans la société.
        /// </summary>
        public DateTime DateEntreeSociete { get; set; }

        /// <summary>
        /// Obtient ou définit l'adresse postale du salarié.
        /// </summary>
        public string AdressePostale { get; set; } = string.Empty;

        /// <summary>
        /// Obtient ou définit l'adresse email du salarié.
        /// </summary>
        public string AdresseMail { get; set; } = string.Empty;

        /// <summary>
        /// Obtient ou définit le numéro de téléphone du salarié.
        /// </summary>
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// Obtient ou définit le salaire du salarié.
        /// </summary>
        public decimal Salaire { get; set; }

        /// <summary>
        /// Obtient ou définit le numéro de sécurité sociale du manager.
        /// Peut être null si le salarié n'a pas de manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        public string? ManagerNumeroSS { get; set; }

        /// <summary>
        /// Constructeur par défaut nécessaire pour la désérialisation JSON.
        /// </summary>
        public Salarie() { }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Salarie avec les informations de base.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale.</param>
        /// <param name="nom">Nom du salarié.</param>
        /// <param name="prenom">Prénom du salarié.</param>
        /// <param name="dateNaissance">Date de naissance.</param>
        /// <param name="dateEntree">Date d'entrée dans la société.</param>
        /// <exception cref="ArgumentNullException">Lancée si un des paramètres requis est null ou vide.</exception>
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
        /// <param name="numeroSecuriteSociale">Numéro de sécurité sociale.</param>
        /// <param name="dateNaissance">Date de naissance.</param>
        /// <param name="dateEntreeSociete">Date d'entrée dans la société.</param>
        /// <param name="nom">Nom du salarié.</param>
        /// <param name="prenom">Prénom du salarié.</param>
        /// <param name="adressePostale">Adresse postale.</param>
        /// <param name="adresseMail">Adresse mail.</param>
        /// <param name="telephone">Numéro de téléphone.</param>
        /// <param name="poste">Poste occupé.</param>
        /// <param name="salaire">Salaire.</param>
        /// <param name="managerNumeroSS">Numéro de sécurité sociale du manager.</param>
        [JsonConstructor]
        public Salarie(
            string numeroSecuriteSociale, 
            DateTime dateNaissance, 
            DateTime dateEntreeSociete, 
            string nom, 
            string prenom, 
            string adressePostale, 
            string adresseMail, 
            string telephone, 
            string poste, 
            decimal salaire, 
            string? managerNumeroSS)
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
        public override string ToString() => $"[{Poste}] {Prenom} {Nom} (ID: {NumeroSecuriteSociale})";

        /// <summary>
        /// Compare deux salariés pour vérifier leur égalité.
        /// </summary>
        /// <param name="other">L'autre salarié à comparer.</param>
        /// <returns>True si les deux salariés sont égaux (même numéro de sécurité sociale), sinon False.</returns>
        public bool Equals(Salarie? other)
        {
            if (other is null) return false;
            return NumeroSecuriteSociale == other.NumeroSecuriteSociale;
        }

        /// <summary>
        /// Compare deux salariés pour vérifier leur égalité.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>True si l'objet est un salarié égal au salarié courant, sinon False.</returns>
        public override bool Equals(object? obj) => Equals(obj as Salarie);

        /// <summary>
        /// Génère un code de hachage pour le salarié.
        /// </summary>
        /// <returns>Un code de hachage basé sur le numéro de sécurité sociale.</returns>
        public override int GetHashCode() => NumeroSecuriteSociale.GetHashCode();

        /// <summary>
        /// Opérateur d'égalité pour comparer deux salariés.
        /// </summary>
        /// <param name="left">Le premier salarié à comparer.</param>
        /// <param name="right">Le second salarié à comparer.</param>
        /// <returns>True si les deux salariés sont égaux ou tous deux null, sinon False.</returns>
        public static bool operator ==(Salarie? left, Salarie? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Opérateur d'inégalité pour comparer deux salariés.
        /// </summary>
        /// <param name="left">Le premier salarié à comparer.</param>
        /// <param name="right">Le second salarié à comparer.</param>
        /// <returns>True si les deux salariés sont différents, sinon False.</returns>
        public static bool operator !=(Salarie? left, Salarie? right) => !(left == right);
    }
}