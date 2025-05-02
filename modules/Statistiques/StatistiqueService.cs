using System;
using System.Collections.Generic;
using System.Linq;


namespace Projet.Modules
{
    public class StatistiqueService
    {
        private readonly ClientManager _clientManager;
        private readonly CommandeManager _commandeManager;
        private readonly SalarieManager _salarieManager;

        public StatistiqueService(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager)
        {
            _clientManager = clientManager;
            _commandeManager = commandeManager;
            _salarieManager = salarieManager;
        }

        public Dictionary<string, int> ObtenirCommandesParVille()
        {
            var commandes = _commandeManager.ObtenirToutesLesCommandes();
            return commandes
                .GroupBy(c => c.VilleArrivee.Nom)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public (double moyenneDistance, double moyennePrix) ObtenirMoyennes()
        {
            var commandes = _commandeManager.ObtenirToutesLesCommandes();
            if (!commandes.Any())
                return (0, 0);

            var moyenneDistance = commandes.Average(c => c.DistanceCalculee);
            var moyennePrix = commandes.Average(c => c.Prix);
            return (moyenneDistance, moyennePrix);
        }

        public Salarie ObtenirChauffeurPlusActif()
        {
            var chauffeurs = _salarieManager.GetTousLesSalaries()
                .Where(s => s.Poste.ToLower().Contains("chauffeur"));

            return chauffeurs
                .OrderByDescending(c => _commandeManager.ObtenirToutesLesCommandes()
                    .Count(cmd => cmd.Chauffeur.NumeroSecuriteSociale == c.NumeroSecuriteSociale))
                .FirstOrDefault();
        }

        public Dictionary<string, int> ObtenirLivraisonsParChauffeur()
        {
            var chauffeurs = _salarieManager.GetTousLesSalaries()
                .Where(s => s.Poste.ToLower().Contains("chauffeur"));
            
            var commandes = _commandeManager.ObtenirToutesLesCommandes();
            
            return chauffeurs.ToDictionary(
                c => $"{c.Nom} {c.Prenom}",
                c => commandes.Count(cmd => cmd.Chauffeur.NumeroSecuriteSociale == c.NumeroSecuriteSociale)
            );
        }

        public double ObtenirMoyenneCompteClients()
        {
            var clients = _clientManager.ObtenirTousLesClients();
            if (!clients.Any()) return 0;

            return clients.Average(c => c.HistoriqueCommandes.Sum(cmd => cmd.Prix));
        }

        public List<Commande> ObtenirCommandesClient(string idClient)
        {
            return _commandeManager.ObtenirToutesLesCommandes()
                .Where(c => c.Client.NumeroSS == idClient)
                .OrderByDescending(c => c.DateCommande)
                .ToList();
        }

        public List<Commande> ObtenirCommandesEntreDates(DateTime dateDebut, DateTime dateFin)
        {
            return _commandeManager.ObtenirToutesLesCommandes()
                .Where(c => c.DateCommande >= dateDebut && c.DateCommande <= dateFin)
                .ToList();
        }
    }
}