using System;
using System.Linq;
using System.Collections.Generic;
using Projet.Modules;

namespace Projet.Modules.UI
{
    /// <summary>
    /// Interface utilisateur pour la gestion des commandes
    /// </summary>
    public class CommandeManagerUI
    {
        private readonly CommandeManager commandeManager;
        private readonly ClientManager clientManager;
        private readonly SalarieManager salarieManager;
        private readonly VehiculeManager vehiculeManager;
        private readonly GrapheListe grapheListe;
        private readonly GrapheService grapheService;
        private bool donneesVillesChargees;

        /// <summary>
        /// Initialise une nouvelle instance de CommandeManagerUI
        /// </summary>
        public CommandeManagerUI(CommandeManager commandeManager, ClientManager clientManager, 
            SalarieManager salarieManager, VehiculeManager vehiculeManager, GrapheListe grapheListe)
        {
            this.commandeManager = commandeManager;
            this.clientManager = clientManager;
            this.salarieManager = salarieManager;
            this.vehiculeManager = vehiculeManager;
            this.grapheListe = grapheListe;
            this.grapheService = new GrapheService(grapheListe);
            this.donneesVillesChargees = false;
        }

        /// <summary>
        /// Charge les données des villes depuis le fichier Excel si ce n'est pas déjà fait
        /// </summary>
        private void ChargerDonneesVilles()
        {
            if (!donneesVillesChargees)
            {
                try
                {
                    grapheService.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    donneesVillesChargees = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement des villes : {ex.Message}");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Affiche le menu principal de gestion des commandes
        /// </summary>
        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Commandes");
                Console.WriteLine("1. Créer une commande");
                Console.WriteLine("2. Modifier une commande");
                Console.WriteLine("3. Supprimer une commande");
                Console.WriteLine("4. Rechercher une commande");
                Console.WriteLine("5. Afficher toutes les commandes");
                Console.WriteLine("6. Rechercher les commandes par client");
                Console.WriteLine("7. Rechercher les commandes par véhicule");
                Console.WriteLine("8. Rechercher les commandes par date de livraison");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                string choix = Console.ReadLine() ?? "";
                switch (choix)
                {
                    case "1":
                        CreerCommande();
                        break;
                    case "2":
                        ModifierCommande();
                        break;
                    case "3":
                        SupprimerCommande();
                        break;
                    case "4":
                        RechercherCommande();
                        break;
                    case "5":
                        commandeManager.AfficherToutesCommandes();
                        Console.ReadKey();
                        break;
                    case "6":
                        RechercherCommandesParClient();
                        break;
                    case "7":
                        RechercherCommandesParVehicule();
                        break;
                    case "8":
                        RechercherCommandesParDate();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Crée une nouvelle commande
        /// </summary>
        private void CreerCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Création d'une commande");

            // Chargement des villes
            ChargerDonneesVilles();
            List<Ville> villes = grapheListe.GetToutesLesVilles().OrderBy(v => v.Nom).ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Erreur : Aucune ville n'est disponible dans le système.");
                Console.WriteLine("Impossible de créer une commande sans villes.");
                Console.ReadKey();
                return;
            }

            // Sélection du client
            Console.Write("Numéro de sécurité sociale du client : ");
            string numeroSSClient = Console.ReadLine() ?? "";
            Client? client = clientManager.RechercherClient(numeroSSClient);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            // Sélection du chauffeur
            Console.Write("Numéro de sécurité sociale du chauffeur : ");
            string numeroSSChauffeur = Console.ReadLine() ?? "";
            Salarie? chauffeur = salarieManager.RechercherParId(numeroSSChauffeur);
            if (chauffeur == null)
            {
                Console.WriteLine("Chauffeur non trouvé.");
                Console.ReadKey();
                return;
            }

            // Sélection du véhicule
            Console.Write("Immatriculation du véhicule : ");
            string immatriculation = Console.ReadLine() ?? "";
            Vehicule? vehicule = vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
            {
                Console.WriteLine("Véhicule non trouvé.");
                Console.ReadKey();
                return;
            }

            // Affichage de la liste des villes disponibles
            Console.WriteLine("\nVilles disponibles :");
            foreach (Ville ville in villes)
            {
                Console.WriteLine($"- {ville.Nom}");
            }

            // Sélection des villes
            Console.Write("\nVille de départ : ");
            string villeDepartNom = Console.ReadLine() ?? "";
            Ville? villeDepartObj = villes.FirstOrDefault(v => v.Nom.Equals(villeDepartNom, StringComparison.OrdinalIgnoreCase));
            if (villeDepartObj == null)
            {
                Console.WriteLine("Ville de départ non trouvée. Vérifiez l'orthographe.");
                Console.ReadKey();
                return;
            }

            Console.Write("Ville d'arrivée : ");
            string villeArriveeNom = Console.ReadLine() ?? "";
            Ville? villeArriveeObj = villes.FirstOrDefault(v => v.Nom.Equals(villeArriveeNom, StringComparison.OrdinalIgnoreCase));
            if (villeArriveeObj == null)
            {
                Console.WriteLine("Ville d'arrivée non trouvée. Vérifiez l'orthographe.");
                Console.ReadKey();
                return;
            }

            // Date de livraison
            Console.Write("Date de livraison (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateLivraison))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            try
            {
                Commande nouvelleCommande = new Commande(client, chauffeur, vehicule, villeDepartObj, villeArriveeObj, dateLivraison);
                commandeManager.AjouterCommande(nouvelleCommande);
                Console.WriteLine("\nCommande créée avec succès !");
                Console.WriteLine("\nDétails de la commande :");
                Console.WriteLine($"Client : {client.Nom} {client.Prenom}");
                Console.WriteLine($"Chauffeur : {chauffeur.Nom} {chauffeur.Prenom}");
                Console.WriteLine($"Véhicule : {vehicule.Immatriculation} ({vehicule.Marque} {vehicule.Modele})");
                Console.WriteLine($"Trajet : {villeDepartObj.Nom} -> {villeArriveeObj.Nom}");
                Console.WriteLine($"Date de livraison : {dateLivraison:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErreur lors de la création de la commande : {ex.Message}");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// <summary>
        /// Modifie une commande existante
        /// </summary>
        private void ModifierCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Modification d'une commande");

            Console.Write("ID de la commande à modifier : ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID invalide.");
                Console.ReadKey();
                return;
            }

            var commande = commandeManager.TrouverCommandeParId(id);
            if (commande == null)
            {
                Console.WriteLine("Commande non trouvée.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nCommande actuelle :");
            Console.WriteLine(commande);

            // Modification des informations
            Console.WriteLine("\nLaisser vide pour conserver la valeur actuelle");

            // Modification du chauffeur
            Console.Write("\nNouveau numéro de sécurité sociale du chauffeur : ");
            string nouveauChauffeurSS = Console.ReadLine() ?? "";
            Salarie? nouveauChauffeur = !string.IsNullOrWhiteSpace(nouveauChauffeurSS)
                ? salarieManager.RechercherParId(nouveauChauffeurSS)
                : commande.Chauffeur;

            // Modification du véhicule
            Console.Write("Nouvelle immatriculation du véhicule : ");
            string nouvelleImmatriculation = Console.ReadLine() ?? "";
            Vehicule? nouveauVehicule = !string.IsNullOrWhiteSpace(nouvelleImmatriculation)
                ? vehiculeManager.ObtenirVehiculeParImmatriculation(nouvelleImmatriculation)
                : commande.Vehicule;

            // Modification de la date de livraison
            Console.Write("Nouvelle date de livraison (JJ/MM/AAAA) : ");
            string nouvelleDateStr = Console.ReadLine() ?? "";
            DateTime nouvelleDateLivraison = !string.IsNullOrWhiteSpace(nouvelleDateStr) && DateTime.TryParse(nouvelleDateStr, out DateTime date)
                ? date
                : commande.DateLivraison;

            try
            {
                Commande commandeModifiee = new Commande(
                    commande.Id,
                    commande.Client,
                    nouveauChauffeur,
                    nouveauVehicule,
                    commande.VilleDepart,
                    commande.VilleArrivee,
                    commande.DateCommande,
                    nouvelleDateLivraison,
                    commande.DistanceCalculee,
                    commande.Prix
                );

                if (commandeManager.ModifierCommande(id, commandeModifiee))
                {
                    Console.WriteLine("Commande modifiée avec succès !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Supprime une commande existante
        /// </summary>
        private void SupprimerCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'une commande");

            Console.Write("ID de la commande à supprimer : ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID invalide.");
                Console.ReadKey();
                return;
            }

            var commande = commandeManager.TrouverCommandeParId(id);
            if (commande == null)
            {
                Console.WriteLine("Commande non trouvée.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nCommande à supprimer :");
            Console.WriteLine(commande);
            Console.Write("\nÊtes-vous sûr de vouloir supprimer cette commande ? (O/N) : ");

            if (Console.ReadLine()?.ToUpper() == "O")
            {
                if (commandeManager.SupprimerCommande(id))
                {
                    Console.WriteLine("Commande supprimée avec succès !");
                }
            }
            else
            {
                Console.WriteLine("Suppression annulée.");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche une commande par son ID
        /// </summary>
        private void RechercherCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche d'une commande");

            Console.Write("ID de la commande : ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID invalide.");
                Console.ReadKey();
                return;
            }

            var commande = commandeManager.TrouverCommandeParId(id);
            if (commande != null)
            {
                Console.WriteLine("\nCommande trouvée :");
                Console.WriteLine(commande);
            }
            else
            {
                Console.WriteLine("Aucune commande trouvée avec cet ID.");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche les commandes par client
        /// </summary>
        private void RechercherCommandesParClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche des commandes par client");

            Console.Write("Nom du client : ");
            string nomClient = Console.ReadLine() ?? "";

            var commandes = commandeManager.RechercherCommandesParClient(nomClient);
            if (commandes.Any())
            {
                Console.WriteLine($"\n{commandes.Count} commande(s) trouvée(s) :");
                foreach (var commande in commandes)
                {
                    Console.WriteLine(commande);
                    Console.WriteLine("-----------------------------------");
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche les commandes par véhicule
        /// </summary>
        private void RechercherCommandesParVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche des commandes par véhicule");

            Console.Write("Immatriculation du véhicule : ");
            string immatriculation = Console.ReadLine() ?? "";

            var commandes = commandeManager.RechercherCommandesParVehicule(immatriculation);
            if (commandes.Any())
            {
                Console.WriteLine($"\n{commandes.Count} commande(s) trouvée(s) :");
                foreach (var commande in commandes)
                {
                    Console.WriteLine(commande);
                    Console.WriteLine("-----------------------------------");
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche les commandes par date de livraison
        /// </summary>
        private void RechercherCommandesParDate()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche des commandes par date de livraison");

            Console.Write("Date de livraison (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateLivraison))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            var commandes = commandeManager.RechercherCommandesParDateLivraison(dateLivraison);
            if (commandes.Any())
            {
                Console.WriteLine($"\n{commandes.Count} commande(s) trouvée(s) :");
                foreach (var commande in commandes)
                {
                    Console.WriteLine(commande);
                    Console.WriteLine("-----------------------------------");
                }
            }
            Console.ReadKey();
        }
    }
}