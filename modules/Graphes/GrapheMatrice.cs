using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    public class GrapheMatrice : Graphe
    {
        private double[,] _matriceAdjacence;
        private readonly List<Ville> _villes; 
        private readonly Dictionary<Ville, int> _villeIndexMap; 
        private const int CapaciteInitiale = 10;

        public GrapheMatrice(bool estNonOriente = true) : base(estNonOriente)
        {
            _villes = new List<Ville>(CapaciteInitiale);
            _villeIndexMap = new Dictionary<Ville, int>();
            _matriceAdjacence = new double[CapaciteInitiale, CapaciteInitiale];
            InitialiserMatrice();
        }

        private void InitialiserMatrice()
        {
            int taille = _matriceAdjacence.GetLength(0);
            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    _matriceAdjacence[i, j] = (i == j) ? 0 : double.PositiveInfinity;
                }
            }
        }

        private void RedimensionnerMatriceSiNecessaire()
        {
            int capaciteActuelle = _matriceAdjacence.GetLength(0);
            if (_villes.Count >= capaciteActuelle)
            {
                int nouvelleCapacite = Math.Max(capaciteActuelle * 2, _villes.Count + 1);
                double[,] nouvelleMatrice = new double[nouvelleCapacite, nouvelleCapacite];

                for (int i = 0; i < nouvelleCapacite; i++)
                    for (int j = 0; j < nouvelleCapacite; j++)
                        nouvelleMatrice[i, j] = (i == j) ? 0 : double.PositiveInfinity;

                for (int i = 0; i < capaciteActuelle; i++)
                    for (int j = 0; j < capaciteActuelle; j++)
                        nouvelleMatrice[i, j] = _matriceAdjacence[i, j];

                _matriceAdjacence = nouvelleMatrice;
            }
        }

        public override bool AjouterVille(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (!_villeIndexMap.ContainsKey(v))
            {
                RedimensionnerMatriceSiNecessaire();
                int nouvelIndex = _villes.Count;
                _villes.Add(v);
                _villeIndexMap[v] = nouvelIndex;
                return true;
            }
            return false; 
        }

        public override bool AjouterLien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids ne peut être négatif.");

            if (!ContientVille(origine)) AjouterVille(origine);
            if (!ContientVille(destination)) AjouterVille(destination);

            int indexOrigine = _villeIndexMap[origine];
            int indexDestination = _villeIndexMap[destination];

            if (_matriceAdjacence[indexOrigine, indexDestination] != poids)
            {
                _matriceAdjacence[indexOrigine, indexDestination] = poids;
                if (EstNonOriente && indexOrigine != indexDestination)
                {
                    _matriceAdjacence[indexDestination, indexOrigine] = poids;
                }
                return true; 
            }
            return false; 
        }

        public override IEnumerable<(Ville voisin, double poids)> ObtenirVoisins(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (_villeIndexMap.TryGetValue(v, out int indexOrigine))
            {
                List<(Ville voisin, double poids)> voisins = new List<(Ville, double)>();
                int taille = _villes.Count; 
                for (int j = 0; j < taille; j++)
                {
                    if (indexOrigine != j && !double.IsPositiveInfinity(_matriceAdjacence[indexOrigine, j]))
                    {
                        voisins.Add((_villes[j], _matriceAdjacence[indexOrigine, j]));
                    }
                }
                return voisins;
            }
            return Enumerable.Empty<(Ville, double)>();
        }

        public override IEnumerable<Ville> GetToutesLesVilles()
        {
            return new List<Ville>(_villes); 
        }

        public override bool ContientVille(Ville v)
        {
            if (v == null) return false;
            return _villeIndexMap.ContainsKey(v);
        }

        public override double ObtenirPoidsLien(Ville origine, Ville destination)
        {
            if (origine == null || destination == null) return double.PositiveInfinity;

            if (_villeIndexMap.TryGetValue(origine, out int indexOrigine) &&
                _villeIndexMap.TryGetValue(destination, out int indexDestination))
            {
                if (indexOrigine < _matriceAdjacence.GetLength(0) && indexDestination < _matriceAdjacence.GetLength(1))
                {
                    return _matriceAdjacence[indexOrigine, indexDestination];
                }
            }
            return double.PositiveInfinity; 
        }

        internal double[,] GetMatriceAdjacence() => (double[,])_matriceAdjacence.Clone();
        internal List<Ville> GetVilleList() => new List<Ville>(_villes);
        internal Dictionary<Ville, int> GetVilleIndexMap() => new Dictionary<Ville, int>(_villeIndexMap);
    }
}