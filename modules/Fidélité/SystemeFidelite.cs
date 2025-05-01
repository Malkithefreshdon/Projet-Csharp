using System;
using System.Linq;

namespace Projet.Modules
{
    /// <summary>
    /// Système de fidélité simplifié pour les clients TransConnect
    /// </summary>
    public class SystemeFidelite
    {
        // Niveaux de fidélité
        public enum StatutFidelite { Standard, Bronze, Argent, Or, Platine }

        // Seuils de points et remises
        private static readonly (int seuil, double remise)[] NIVEAUX = {
            (0, 0),         // Standard: 0% de remise
            (1000, 0.05),   // Bronze: 5% de remise
            (5000, 0.07),   // Argent: 7% de remise
            (15000, 0.10),  // Or: 10% de remise
            (30000, 0.15)   // Platine: 15% de remise
        };

        /// <summary>
        /// Calcule les points de fidélité d'un client
        /// </summary>
        public int CalculerPoints(Client client)
        {
            if (client == null) return 0;

            // Points basés sur le montant des commandes (1 point par euro)
            int points = (int)client.HistoriqueCommandes.Sum(c => c.Prix);

            // Bonus pour le nombre de commandes
            points += client.HistoriqueCommandes.Count * 50;

            return points;
        }

        /// <summary>
        /// Détermine le statut de fidélité d'un client
        /// </summary>
        public StatutFidelite DeterminerStatut(Client client)
        {
            int points = CalculerPoints(client);

            for (int i = NIVEAUX.Length - 1; i >= 0; i--)
            {
                if (points >= NIVEAUX[i].seuil)
                    return (StatutFidelite)i;
            }

            return StatutFidelite.Standard;
        }

        /// <summary>
        /// Calcule le prix après remise fidélité
        /// </summary>
        public double AppliquerRemise(double prixBase, Client client)
        {
            if (client == null) return prixBase;

            int statutIndex = (int)DeterminerStatut(client);
            double remise = NIVEAUX[statutIndex].remise;

            return Math.Round(prixBase * (1 - remise), 2);
        }

        /// <summary>
        /// Affiche les informations de fidélité d'un client
        /// </summary>
        public void AfficherInfos(Client client)
        {
            if (client == null) return;

            Console.Clear();
            Console.WriteLine("=== PROGRAMME DE FIDÉLITÉ ===");
            Console.WriteLine($"Client: {client.Nom} {client.Prenom}");

            int points = CalculerPoints(client);
            StatutFidelite statut = DeterminerStatut(client);
            int statutIndex = (int)statut;
            double remise = NIVEAUX[statutIndex].remise * 100;

            Console.WriteLine($"Points: {points}");
            Console.WriteLine($"Statut: {statut}");
            Console.WriteLine($"Remise: {remise}%");

            // Progression vers le niveau suivant
            if (statut != StatutFidelite.Platine)
            {
                int prochainNiveau = NIVEAUX[statutIndex + 1].seuil;
                int pointsManquants = prochainNiveau - points;
                Console.WriteLine($"Points pour le niveau suivant: {pointsManquants}");
            }
        }

        /// <summary>
        /// Affiche un récapitulatif de remise pour une commande
        /// </summary>
        public void AfficherRecapitulatif(Client client, double prixBase)
        {
            if (client == null) return;

            StatutFidelite statut = DeterminerStatut(client);
            int statutIndex = (int)statut;
            double remise = NIVEAUX[statutIndex].remise * 100;
            double prixRemise = AppliquerRemise(prixBase, client);

            Console.WriteLine("\n=== REMISE FIDÉLITÉ ===");
            Console.WriteLine($"Statut: {statut}");
            Console.WriteLine($"Remise: {remise}%");
            Console.WriteLine($"Prix original: {prixBase:C2}");
            Console.WriteLine($"Prix après remise: {prixRemise:C2}");
            Console.WriteLine($"Économie: {prixBase - prixRemise:C2}");
        }
    }
}
