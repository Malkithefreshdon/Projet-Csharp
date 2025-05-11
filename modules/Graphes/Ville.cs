using System;

namespace Projet.Modules
{
    /// <summary>
    /// Représente une ville dans un graphe.
    /// </summary>
    public class Ville : IEquatable<Ville>
    {
        /// <summary>
        /// Obtient le nom de la ville.
        /// </summary>
        public string Nom { get; private set; }

        // Optionnel: Coordonnées géographiques
        // public double Latitude { get; set; }
        // public double Longitude { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Ville.
        /// </summary>
        /// <param name="nom">Le nom de la ville.</param>
        public Ville(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le nom de la ville ne peut pas être vide.", nameof(nom));
            Nom = nom;
        }

        /// <summary>
        /// Retourne une représentation textuelle de la ville.
        /// </summary>
        /// <returns>Le nom de la ville.</returns>
        public override string ToString()
        {
            return Nom;
        }

        /// <summary>
        /// Détermine si cette ville est égale à une autre ville.
        /// </summary>
        /// <param name="other">La ville à comparer.</param>
        /// <returns>True si les villes sont égales, False sinon.</returns>
        public bool Equals(Ville other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Nom, other.Nom, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Détermine si cette ville est égale à un autre objet.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>True si les objets sont égaux, False sinon.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Ville)obj);
        }

        /// <summary>
        /// Retourne un code de hachage pour cette ville.
        /// </summary>
        /// <returns>Un code de hachage basé sur le nom de la ville.</returns>
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Nom);
        }

        /// <summary>
        /// Compare deux villes pour égalité.
        /// </summary>
        /// <param name="left">La première ville.</param>
        /// <param name="right">La seconde ville.</param>
        /// <returns>True si les villes sont égales, False sinon.</returns>
        public static bool operator ==(Ville left, Ville right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compare deux villes pour inégalité.
        /// </summary>
        /// <param name="left">La première ville.</param>
        /// <param name="right">La seconde ville.</param>
        /// <returns>True si les villes sont différentes, False sinon.</returns>
        public static bool operator !=(Ville left, Ville right)
        {
            return !Equals(left, right);
        }
    }
}
