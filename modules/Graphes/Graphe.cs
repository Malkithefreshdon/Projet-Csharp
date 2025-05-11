using System;
using System.Collections.Generic;

namespace Projet.Modules
{
    /// <summary>
    /// Classe abstraite représentant un graphe de villes et leurs connexions.
    /// </summary>
    public abstract class Graphe
    {
        public bool EstNonOriente { get; protected set; }

        protected Graphe(bool estNonOriente = true)
        {
            EstNonOriente = estNonOriente;
        }

        /// <summary>
        /// Ajoute une ville au graphe.
        /// </summary>
        /// <param name="v">La ville à ajouter.</param>
        /// <returns>True si la ville a été ajoutée, False sinon.</returns>
        public abstract bool AjouterVille(Ville v);

        /// <summary>
        /// Ajoute un lien entre deux villes avec un poids spécifié.
        /// </summary>
        public abstract bool AjouterLien(Ville origine, Ville destination, double poids);

        /// <summary>
        /// Obtient les villes voisines d'une ville donnée avec leurs poids.
        /// </summary>
        public abstract IEnumerable<(Ville voisin, double poids)> ObtenirVoisins(Ville v);

        /// <summary>
        /// Obtient toutes les villes du graphe.
        /// </summary>
        public abstract IEnumerable<Ville> GetToutesLesVilles();

        /// <summary>
        /// Vérifie si une ville est présente dans le graphe.
        /// </summary>
        public abstract bool ContientVille(Ville v);

        /// <summary>
        /// Obtient le poids du lien entre deux villes.
        /// </summary>
        public abstract double ObtenirPoidsLien(Ville origine, Ville destination);
    }
}
