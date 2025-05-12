using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    /// <summary>
    /// Représente un graphe de villes avec des liens pondérés.
    /// </summary>
    public class GrapheListe : Graphe
    {
        private readonly Dictionary<Ville, List<(Ville voisin, double poids)>> adjacence;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GrapheListe.
        /// </summary>
        /// <param name="estNonOriente">Indique si le graphe est non orienté.</param>
        public GrapheListe(bool estNonOriente = true) : base(estNonOriente)
        {
            adjacence = new Dictionary<Ville, List<(Ville voisin, double poids)>>();
        }

        /// <summary>
        /// Ajoute une ville au graphe si elle n'existe pas déjà.
        /// </summary>
        /// <param name="v">La ville à ajouter.</param>
        /// <returns>True si la ville a été ajoutée, False si elle existait déjà.</returns>
        public override bool AjouterVille(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (!adjacence.ContainsKey(v))
            {
                adjacence[v] = new List<(Ville voisin, double poids)>();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ajoute un lien pondéré entre deux villes.
        /// </summary>
        /// <param name="origine">La ville d'origine.</param>
        /// <param name="destination">La ville de destination.</param>
        /// <param name="poids">Le poids (distance) du lien.</param>
        /// <returns>True si le lien a été ajouté, False s'il existait déjà.</returns>
        public override bool AjouterLien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids ne peut être négatif.");

            AjouterVille(origine);
            AjouterVille(destination);

            if (!adjacence[origine].Any(lien => lien.voisin.Equals(destination)))
            {
                adjacence[origine].Add((voisin: destination, poids: poids));
                if (EstNonOriente && !origine.Equals(destination))
                {
                    if (!adjacence[destination].Any(lien => lien.voisin.Equals(origine)))
                    {
                        adjacence[destination].Add((voisin: origine, poids: poids));
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient la liste des villes voisines et leurs distances depuis une ville donnée.
        /// </summary>
        /// <param name="v">La ville dont on veut obtenir les voisins.</param>
        /// <returns>Une collection de tuples (ville voisine, poids du lien).</returns>
        public override IEnumerable<(Ville voisin, double poids)> ObtenirVoisins(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (adjacence.TryGetValue(v, out List<(Ville voisin, double poids)> voisins))
            {
                return voisins;
            }
            return Enumerable.Empty<(Ville voisin, double poids)>();
        }

        /// <summary>
        /// Obtient toutes les villes du graphe.
        /// </summary>
        /// <returns>Une collection de toutes les villes du graphe.</returns>
        public override IEnumerable<Ville> GetToutesLesVilles()
        {
            return adjacence.Keys.ToList();
        }

        /// <summary>
        /// Vérifie si une ville existe dans le graphe.
        /// </summary>
        /// <param name="v">La ville à vérifier.</param>
        /// <returns>True si la ville existe dans le graphe, sinon False.</returns>
        public override bool ContientVille(Ville v)
        {
            if (v == null) return false;
            return adjacence.ContainsKey(v);
        }

        /// <summary>
        /// Obtient le poids du lien entre deux villes.
        /// </summary>
        /// <param name="origine">La ville d'origine.</param>
        /// <param name="destination">La ville de destination.</param>
        /// <returns>Le poids du lien ou double.PositiveInfinity si le lien n'existe pas.</returns>
        public override double ObtenirPoidsLien(Ville origine, Ville destination)
        {
            if (origine == null || destination == null) return double.PositiveInfinity;

            if (adjacence.TryGetValue(origine, out List<(Ville voisin, double poids)> voisins))
            {
                (Ville voisin, double poids) lien = voisins.FirstOrDefault(l => l.voisin.Equals(destination));
                if (lien.voisin != null)
                {
                    return lien.poids;
                }
            }
            return double.PositiveInfinity;
        }
    }
}
