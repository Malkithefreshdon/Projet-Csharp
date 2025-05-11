using System;

namespace Projet.Modules
{
    /// <summary>
    /// Représente un lien pondéré entre deux villes dans un graphe.
    /// </summary>
    public class Lien
    {
        /// <summary>
        /// Obtient la ville d'origine du lien.
        /// </summary>
        public Ville Origine { get; }

        /// <summary>
        /// Obtient la ville de destination du lien.
        /// </summary>
        public Ville Destination { get; }

        /// <summary>
        /// Obtient ou définit le poids (distance) du lien.
        /// </summary>
        public double Poids { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Lien.
        /// </summary>
        /// <param name="origine">La ville d'origine.</param>
        /// <param name="destination">La ville de destination.</param>
        /// <param name="poids">Le poids (distance) du lien.</param>
        public Lien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids d'un lien ne peut être négatif dans ce contexte.");

            Origine = origine;
            Destination = destination;
            Poids = poids;
        }

        /// <summary>
        /// Retourne une représentation textuelle du lien.
        /// </summary>
        /// <returns>Une chaîne de caractères représentant le lien.</returns>
        public override string ToString()
        {
            return $"{Origine} -> {Destination} ({Poids})";
        }
    }
}
