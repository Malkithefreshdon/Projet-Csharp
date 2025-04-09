using modules.Graphes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace modules.Graphes 
{
    public class GrapheListe : Graphe
    {
        private readonly Dictionary<Ville, List<(Ville voisin, double poids)>> _adjacence;

        public GrapheListe(bool estNonOriente = true) : base(estNonOriente)
        {
            _adjacence = new Dictionary<Ville, List<(Ville, double)>>();
        }

        public override bool AjouterVille(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (!_adjacence.ContainsKey(v))
            {
                _adjacence[v] = new List<(Ville, double)>();
                return true;
            }
            return false; 
        }

        public override bool AjouterLien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids ne peut être négatif.");

            AjouterVille(origine);
            AjouterVille(destination);

            if (!_adjacence[origine].Any(lien => lien.voisin.Equals(destination)))
            {
                _adjacence[origine].Add((destination, poids));
                if (EstNonOriente && !origine.Equals(destination))
                {
                    if (!_adjacence[destination].Any(lien => lien.voisin.Equals(origine)))
                    {
                        _adjacence[destination].Add((origine, poids));
                    }
                    // Optionnel : mettre à jour le poids si le lien inverse existe avec un poids différent?
                    // else { /* gérer conflit de poids si nécessaire */ }
                }
                return true; // Lien ajouté
            }
            // Optionnel : Mettre à jour le poids si le lien existe déjà ?
            // else { /* trouver le lien et mettre à jour poids */ }
            return false; 
        }

        public override IEnumerable<(Ville voisin, double poids)> ObtenirVoisins(Ville v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (_adjacence.TryGetValue(v, out var voisins))
            {
                return voisins;
            }
            return Enumerable.Empty<(Ville, double)>(); 
        }

        public override IEnumerable<Ville> GetToutesLesVilles()
        {
            return _adjacence.Keys.ToList(); 
        }

        public override bool ContientVille(Ville v)
        {
            if (v == null) return false;
            return _adjacence.ContainsKey(v);
        }

        public override double ObtenirPoidsLien(Ville origine, Ville destination)
        {
            if (origine == null || destination == null) return double.PositiveInfinity;

            if (_adjacence.TryGetValue(origine, out var voisins))
            {
                var lien = voisins.FirstOrDefault(l => l.voisin.Equals(destination));
                if (lien.voisin != null)
                {
                    return lien.poids;
                }
            }
            return double.PositiveInfinity; 
        }
    }
}