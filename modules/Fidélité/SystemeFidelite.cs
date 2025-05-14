using System;
using System.Linq;

#nullable enable

namespace Projet.Modules
{
    /// <summary>
    /// Représente le système de fidélité de TransConnect.
    /// Gère les niveaux de fidélité, le calcul des commandes et l'application des remises.
    /// </summary>
    public class SystemeFidelite
    {
        private readonly CommandeManager commandeManager;

        public SystemeFidelite()
        {
            commandeManager = new CommandeManager();
        }

        /// <summary>
        /// Définit les différents niveaux de fidélité disponibles pour les clients.
        /// </summary>
        public enum StatutFidelite 
        { 
            /// <summary>Niveau de base, sans avantages</summary>
            Standard, 
            /// <summary>Premier niveau, 5% de remise</summary>
            Bronze, 
            /// <summary>Deuxième niveau, 7% de remise</summary>
            Argent, 
            /// <summary>Troisième niveau, 10% de remise</summary>
            Or, 
            /// <summary>Niveau le plus élevé, 15% de remise</summary>
            Platine 
        }

        /// <summary>
        /// Définition des seuils de commandes et des remises associées pour chaque niveau de fidélité.
        /// Le tableau est indexé par l'énumération StatutFidelite.
        /// </summary>
        private static readonly (int nbCommandes, double remise)[] niveaux = {
            (0, 0.00),    // Standard
            (5, 0.05),    // Bronze: après 5 commandes
            (10, 0.07),   // Argent: après 10 commandes
            (20, 0.10),   // Or: après 20 commandes
        };

        /// <summary>
        /// Compte le nombre de commandes effectuées par un client.
        /// </summary>
        /// <param name="client">Le client dont on veut compter les commandes.</param>
        /// <returns>Le nombre total de commandes. Retourne 0 si le client est null.</returns>
        public int CompterCommandes(Client? client)
        {
            if (client == null) 
                return 0;

            return commandeManager.GetToutesLesCommandes()
                .Count(c => c.Client.NumeroSS == client.NumeroSS);
        }

        /// <summary>
        /// Détermine le statut de fidélité d'un client en fonction de son nombre de commandes.
        /// Le statut est déterminé en comparant le nombre de commandes du client avec les seuils définis.
        /// </summary>
        /// <param name="client">Le client dont on veut déterminer le statut.</param>
        /// <returns>Le statut de fidélité correspondant au nombre de commandes du client.
        /// Retourne StatutFidelite.Standard si le client est null.</returns>
        public StatutFidelite DeterminerStatut(Client? client)
        {
            int nbCommandes = CompterCommandes(client);

            // On parcourt les niveaux du plus élevé au plus bas
            for (int i = niveaux.Length - 1; i >= 0; i--)
            {
                if (nbCommandes >= niveaux[i].nbCommandes)
                    return (StatutFidelite)i;
            }

            return StatutFidelite.Standard;
        }

        /// <summary>
        /// Calcule le prix après application de la remise fidélité.
        /// </summary>
        /// <param name="prixBase">Le prix initial avant remise.</param>
        /// <param name="client">Le client pour lequel calculer la remise.</param>
        /// <returns>Le prix après application de la remise. 
        /// Retourne le prix de base si le client est null.</returns>
        public double AppliquerRemise(double prixBase, Client? client)
        {
            if (client == null) 
                return prixBase;

            int statutIndex = (int)DeterminerStatut(client);
            double remise = niveaux[statutIndex].remise;

            return Math.Round(prixBase * (1 - remise), 2);
        }

        /// <summary>
        /// Affiche dans la console les informations détaillées de fidélité d'un client.
        /// Inclut le nombre de commandes, le statut, la remise et la progression vers le niveau suivant.
        /// </summary>
        /// <param name="client">Le client dont on veut afficher les informations.</param>
        public void AfficherInfos(Client? client)
        {
            if (client == null)
            {
                Console.WriteLine("Impossible d'afficher les informations de fidélité : client non spécifié.");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== PROGRAMME DE FIDÉLITÉ ===");
            Console.WriteLine($"Client: {client.Nom} {client.Prenom}");

            int nbCommandes = CompterCommandes(client);
            StatutFidelite statut = DeterminerStatut(client);
            int statutIndex = (int)statut;
            double remise = niveaux[statutIndex].remise * 100;

            Console.WriteLine($"Nombre de commandes: {nbCommandes}");
            Console.WriteLine($"Statut: {statut}");
            Console.WriteLine($"Remise: {remise:F1}%");

            if (statut != StatutFidelite.Platine)
            {
                int commandesManquantes = niveaux[statutIndex + 1].nbCommandes - nbCommandes;
                Console.WriteLine($"Commandes restantes pour le niveau suivant: {commandesManquantes}");
            }
            else
            {
                Console.WriteLine("Niveau maximum atteint !");
            }
        }

        /// <summary>
        /// Affiche dans la console un récapitulatif des remises appliquées à une commande.
        /// Inclut le statut du client, le pourcentage de remise, les prix avant et après remise,
        /// ainsi que le montant économisé.
        /// </summary>
        /// <param name="client">Le client concerné par la commande.</param>
        /// <param name="prixBase">Le prix de base de la commande avant remise.</param>
        public void AfficherRecapitulatif(Client? client, double prixBase)
        {
            if (client == null)
            {
                Console.WriteLine("Impossible d'afficher le récapitulatif : client non spécifié.");
                return;
            }

            StatutFidelite statut = DeterminerStatut(client);
            int statutIndex = (int)statut;
            double remise = niveaux[statutIndex].remise * 100;
            double prixRemise = AppliquerRemise(prixBase, client);

            Console.WriteLine("\n=== REMISE FIDÉLITÉ ===");
            Console.WriteLine($"Statut: {statut}");
            Console.WriteLine($"Remise: {remise:F1}%");
            Console.WriteLine($"Prix original: {prixBase:C2}");
            Console.WriteLine($"Prix après remise: {prixRemise:C2}");
            Console.WriteLine($"Économie: {prixBase - prixRemise:C2}");
        }
    }
}
