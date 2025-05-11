using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    /// <summary>
    /// Implémentation d'un graphe utilisant une matrice d'adjacence.
    /// </summary>
    public class GrapheMatrice : Graphe
    {
        private double[,] matriceAdjacence;
        private readonly List<Ville> villes;
        private readonly Dictionary<Ville, int> villeIndexMap;
        private const int CapaciteInitiale = 10;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GrapheMatrice.
        /// </summary>
        /// <param name="estNonOriente">Indique si le graphe est non orienté.</param>
        public GrapheMatrice(bool estNonOriente = true) : base(estNonOriente)
        {
            villes = new List<Ville>(CapaciteInitiale);
            villeIndexMap = new Dictionary<Ville, int>();
            matriceAdjacence = new double[CapaciteInitiale, CapaciteInitiale];
            InitialiserMatrice();
        }

        /// <summary>
        /// Initialise la matrice d'adjacence avec des valeurs par défaut.
        /// </summary>
        private void InitialiserMatrice()
        {
            int taille = matriceAdjacence.GetLength(0);
            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    matriceAdjacence[i, j] = (i == j) ? 0 : double.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Redimensionne la matrice d'adjacence si nécessaire pour accueillir de nouvelles villes.
        /// </summary>
        private void RedimensionnerMatriceSiNecessaire()
        {
            int capaciteActuelle = matriceAdjacence.GetLength(0);
            if (villes.Count >= capaciteActuelle)
            {
                int nouvelleCapacite = Math.Max(capaciteActuelle * 2, villes.Count + 1);
                double[,] nouvelleMatrice = new double[nouvelleCapacite, nouvelleCapacite];

                for (int i = 0; i < nouvelleCapacite; i++)
                    for (int j = 0; j < nouvelleCapacite; j++)
                        nouvelleMatrice[i, j] = (i == j) ? 0 : double.PositiveInfinity;

                for (int i = 0; i < capaciteActuelle; i++)
                    for (int j = 0; j < capaciteActuelle; j++)
                        nouvelleMatrice[i, j] = matriceAdjacence[i, j];

                matriceAdjacence = nouvelleMatrice;
            }
        }

        /// <summary>
        /// Ajoute une ville au graphe si elle n'existe pas déjà.
        /// </summary>
        /// <param name="v">La ville à ajouter.</param>
        /// <returns>True si la ville a été ajoutée, False si elle existait déjà.</returns>
        public override bool AjouterVille(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (!villeIndexMap.ContainsKey(v))
            {
                RedimensionnerMatriceSiNecessaire();
                int nouvelIndex = villes.Count;
                villes.Add(v);
                villeIndexMap[v] = nouvelIndex;
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
        /// <returns>True si le lien a été ajouté ou modifié, False sinon.</returns>
        public override bool AjouterLien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids ne peut être négatif.");

            if (!ContientVille(origine)) AjouterVille(origine);
            if (!ContientVille(destination)) AjouterVille(destination);

            int indexOrigine = villeIndexMap[origine];
            int indexDestination = villeIndexMap[destination];

            if (matriceAdjacence[indexOrigine, indexDestination] != poids)
            {
                matriceAdjacence[indexOrigine, indexDestination] = poids;
                if (EstNonOriente && indexOrigine != indexDestination)
                {
                    matriceAdjacence[indexDestination, indexOrigine] = poids;
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
            if (villeIndexMap.TryGetValue(v, out int indexOrigine))
            {
                List<(Ville voisin, double poids)> voisins = new List<(Ville, double)>();
                int taille = villes.Count;
                for (int j = 0; j < taille; j++)
                {
                    if (indexOrigine != j && !double.IsPositiveInfinity(matriceAdjacence[indexOrigine, j]))
                    {
                        voisins.Add((villes[j], matriceAdjacence[indexOrigine, j]));
                    }
                }
                return voisins;
            }
            return Enumerable.Empty<(Ville, double)>();
        }

        /// <summary>
        /// Obtient toutes les villes du graphe.
        /// </summary>
        /// <returns>Une collection de toutes les villes du graphe.</returns>
        public override IEnumerable<Ville> GetToutesLesVilles()
        {
            return new List<Ville>(villes);
        }

        /// <summary>
        /// Vérifie si une ville existe dans le graphe.
        /// </summary>
        /// <param name="v">La ville à vérifier.</param>
        /// <returns>True si la ville existe dans le graphe, sinon False.</returns>
        public override bool ContientVille(Ville v)
        {
            if (v == null) return false;
            return villeIndexMap.ContainsKey(v);
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

            if (villeIndexMap.TryGetValue(origine, out int indexOrigine) &&
                villeIndexMap.TryGetValue(destination, out int indexDestination))
            {
                if (indexOrigine < matriceAdjacence.GetLength(0) && indexDestination < matriceAdjacence.GetLength(1))
                {
                    return matriceAdjacence[indexOrigine, indexDestination];
                }
            }
            return double.PositiveInfinity;
        }





        /// <summary>
        /// Obtient une copie de la matrice d'adjacence.
        /// </summary>
        /// <returns>Une copie de la matrice d'adjacence.</returns>
        internal double[,] GetMatriceAdjacence() => (double[,])matriceAdjacence.Clone();

        /// <summary>
        /// Obtient une copie de la liste des villes.
        /// </summary>
        /// <returns>Une copie de la liste des villes.</returns>
        internal List<Ville> GetVilleList() => new List<Ville>(villes);

        /// <summary>
        /// Obtient une copie du dictionnaire de correspondance ville-index.
        /// </summary>
        /// <returns>Une copie du dictionnaire de correspondance ville-index.</returns>
        internal Dictionary<Ville, int> GetVilleIndexMap() => new Dictionary<Ville, int>(villeIndexMap);
    }
}
