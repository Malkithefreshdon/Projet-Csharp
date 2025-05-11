using System;
using System.Collections.Generic;
using System.Linq;


namespace Projet.Modules
{
    /// <summary>
    /// Service fournissant des statistiques sur les clients, commandes et salariés.
    /// </summary>
    public class StatistiqueService
    {
        private readonly ClientManager ClientManager;
        private readonly CommandeManager CommandeManager;
        private readonly SalarieManager SalarieManager;

        /// <summary>
        /// Initialise le service de statistiques avec les gestionnaires nécessaires.
        /// </summary>
        /// <param name="clientManager">Gestionnaire des clients.</param>
        /// <param name="commandeManager">Gestionnaire des commandes.</param>
        /// <param name="salarieManager">Gestionnaire des salariés.</param>
        public StatistiqueService(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager)
        {
            ClientManager = clientManager;
            CommandeManager = commandeManager;
            SalarieManager = salarieManager;
        }

        /// <summary>
        /// Retourne le nombre de commandes par ville d'arrivée.
        /// </summary>
        /// <returns>Dictionnaire ville => nombre de commandes.</returns>
        public Dictionary<string, int> ObtenirCommandesParVille()
        {
            List<Commande> commandes = CommandeManager.GetToutesLesCommandes();
            return commandes
                .GroupBy(c => c.VilleArrivee.Nom)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// Calcule la moyenne des distances et des prix des commandes.
        /// </summary>
        /// <returns>Tuple (moyenneDistance, moyennePrix).</returns>
        public (double moyenneDistance, double moyennePrix) ObtenirMoyennes()
        {
            List<Commande> commandes = CommandeManager.GetToutesLesCommandes();
            if (!commandes.Any())
                return (0, 0);

            double moyenneDistance = commandes.Average(c => c.DistanceCalculee);
            double moyennePrix = commandes.Average(c => c.Prix);
            return (moyenneDistance, moyennePrix);
        }

        /// <summary>
        /// Retourne le chauffeur ayant effectué le plus de livraisons.
        /// </summary>
        /// <returns>Le salarié chauffeur le plus actif, ou null si aucun.</returns>
        public Salarie ObtenirChauffeurPlusActif()
        {
            IEnumerable<Salarie> chauffeurs = SalarieManager.GetTousLesSalaries()
                .Where(s => s.Poste.ToLower().Contains("chauffeur"));

            return chauffeurs
                .OrderByDescending(c => CommandeManager.GetToutesLesCommandes()
                    .Count(cmd => cmd.Chauffeur.NumeroSecuriteSociale == c.NumeroSecuriteSociale))
                .FirstOrDefault();
        }

        /// <summary>
        /// Retourne le nombre de livraisons effectuées par chaque chauffeur.
        /// </summary>
        /// <returns>Dictionnaire nom complet du chauffeur => nombre de livraisons.</returns>
        public Dictionary<string, int> ObtenirLivraisonsParChauffeur()
        {
            IEnumerable<Salarie> chauffeurs = SalarieManager.GetTousLesSalaries()
                .Where(s => s.Poste.ToLower().Contains("chauffeur"));
            
            List<Commande> commandes = CommandeManager.GetToutesLesCommandes();
            
            return chauffeurs.ToDictionary(
                c => $"{c.Nom} {c.Prenom}",
                c => commandes.Count(cmd => cmd.Chauffeur.NumeroSecuriteSociale == c.NumeroSecuriteSociale)
            );
        }

        /// <summary>
        /// Calcule la moyenne du chiffre d'affaires par client.
        /// </summary>
        /// <returns>Moyenne du chiffre d'affaires par client.</returns>
        public double ObtenirMoyenneCompteClients()
        {
            List<Client> clients = ClientManager.ObtenirTousLesClients();
            if (!clients.Any()) return 0;

            return clients.Average(c => c.HistoriqueCommandes.Sum(cmd => cmd.Prix));
        }

        /// <summary>
        /// Retourne la liste des commandes d'un client donné, triées par date décroissante.
        /// </summary>
        /// <param name="idClient">Numéro de sécurité sociale du client.</param>
        /// <returns>Liste des commandes du client.</returns>
        public List<Commande> ObtenirCommandesClient(string idClient)
        {
            return CommandeManager.GetToutesLesCommandes()
                .Where(c => c.Client.NumeroSS == idClient)
                .OrderByDescending(c => c.DateCommande)
                .ToList();
        }

        /// <summary>
        /// Retourne la liste des commandes passées entre deux dates.
        /// </summary>
        /// <param name="dateDebut">Date de début (incluse).</param>
        /// <param name="dateFin">Date de fin (incluse).</param>
        /// <returns>Liste des commandes dans l'intervalle.</returns>
        public List<Commande> ObtenirCommandesEntreDates(DateTime dateDebut, DateTime dateFin)
        {
            return CommandeManager.GetToutesLesCommandes()
                .Where(c => c.DateCommande >= dateDebut && c.DateCommande <= dateFin)
                .ToList();
        }
    }
}