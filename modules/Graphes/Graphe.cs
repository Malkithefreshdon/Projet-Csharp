using System;
using System.Collections.Generic;


namespace Projet.Modules
{
    public abstract class Graphe
    {
        public bool EstNonOriente { get; protected set; }

        protected Graphe(bool estNonOriente = true)
        {
            EstNonOriente = estNonOriente;
        }
        public abstract bool AjouterVille(Ville v);
        public abstract bool AjouterLien(Ville origine, Ville destination, double poids);
        public abstract IEnumerable<(Ville voisin, double poids)> ObtenirVoisins(Ville v);
        public abstract IEnumerable<Ville> GetToutesLesVilles();
        public abstract bool ContientVille(Ville v);
        public abstract double ObtenirPoidsLien(Ville origine, Ville destination);
    }
}