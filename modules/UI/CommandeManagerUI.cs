using System;
using System.Linq;
using System.Collections.Generic;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class CommandeManagerUI
    {
        private readonly CommandeManager _commandeManager;
        private readonly ClientManager _clientManager;
        private readonly SalarieManager _salarieManager;
        private readonly VehiculeManager _vehiculeManager;
        private readonly GrapheListe _grapheListe;
        private readonly GrapheService _grapheService;
        private bool _donneesVillesChargees;

        public CommandeManagerUI(CommandeManager commandeManager, ClientManager clientManager, 
            SalarieManager salarieManager, VehiculeManager vehiculeManager, GrapheListe grapheListe)
        {
            _commandeManager = commandeManager;
            _clientManager = clientManager;
            _salarieManager = salarieManager;
            _vehiculeManager = vehiculeManager;
            _grapheListe = grapheListe;
            _grapheService = new GrapheService(_grapheListe);
            _donneesVillesChargees = false;
        }

        private void ChargerDonneesVilles()
        {
            if (!_donneesVillesChargees)
            {
                try
                {
                    _grapheService.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    _donneesVillesChargees = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement des villes : {ex.Message}");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

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

                var choix = Console.ReadLine();
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
                        _commandeManager.AfficherToutesCommandes();
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

        private void CreerCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Création d'une commande");

            // Chargement des villes
            ChargerDonneesVilles();
            var villes = _grapheListe.GetToutesLesVilles().OrderBy(v => v.Nom).ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Erreur : Aucune ville n'est disponible dans le système.");
                Console.WriteLine("Impossible de créer une commande sans villes.");
                Console.ReadKey();
                return;
            }

            // Sélection du client
            Console.Write("Numéro de sécurité sociale du client : ");
            string numeroSSClient = Console.ReadLine();
            var client = _clientManager.RechercherClient(numeroSSClient);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            // Sélection du chauffeur
            Console.Write("Numéro de sécurité sociale du chauffeur : ");
            string numeroSSChauffeur = Console.ReadLine();
            var chauffeur = _salarieManager.RechercherParId(numeroSSChauffeur);
            if (chauffeur == null)
            {
                Console.WriteLine("Chauffeur non trouvé.");
                Console.ReadKey();
                return;
            }

            // Sélection du véhicule
            Console.Write("Immatriculation du véhicule : ");
            string immatriculation = Console.ReadLine();
            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
            {
                Console.WriteLine("Véhicule non trouvé.");
                Console.ReadKey();
                return;
            }

            // Affichage de la liste des villes disponibles
            Console.WriteLine("\nVilles disponibles :");
            foreach (var ville in villes)
            {
                Console.WriteLine($"- {ville.Nom}");
            }

            // Sélection des villes
            Console.Write("\nVille de départ : ");
            string villeDepart = Console.ReadLine();
            var villeDepartObj = villes.FirstOrDefault(v => v.Nom.Equals(villeDepart, StringComparison.OrdinalIgnoreCase));
            if (villeDepartObj == null)
            {
                Console.WriteLine("Ville de départ non trouvée. Vérifiez l'orthographe.");
                Console.ReadKey();
                return;
            }

            Console.Write("Ville d'arrivée : ");
            string villeArrivee = Console.ReadLine();
            var villeArriveeObj = villes.FirstOrDefault(v => v.Nom.Equals(villeArrivee, StringComparison.OrdinalIgnoreCase));
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
                var nouvelleCommande = new Commande(client, chauffeur, vehicule, villeDepartObj, villeArriveeObj, dateLivraison);
                _commandeManager.AjouterCommande(nouvelleCommande);
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

            var commande = _commandeManager.TrouverCommandeParId(id);
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
            string nouveauChauffeurSS = Console.ReadLine();
            Salarie nouveauChauffeur = !string.IsNullOrWhiteSpace(nouveauChauffeurSS)
                ? _salarieManager.RechercherParId(nouveauChauffeurSS)
                : commande.Chauffeur;

            // Modification du véhicule
            Console.Write("Nouvelle immatriculation du véhicule : ");
            string nouvelleImmatriculation = Console.ReadLine();
            Vehicule nouveauVehicule = !string.IsNullOrWhiteSpace(nouvelleImmatriculation)
                ? _vehiculeManager.ObtenirVehiculeParImmatriculation(nouvelleImmatriculation)
                : commande.Vehicule;

            // Modification de la date de livraison
            Console.Write("Nouvelle date de livraison (JJ/MM/AAAA) : ");
            string nouvelleDateStr = Console.ReadLine();
            DateTime nouvelleDateLivraison = !string.IsNullOrWhiteSpace(nouvelleDateStr) && DateTime.TryParse(nouvelleDateStr, out DateTime date)
                ? date
                : commande.DateLivraison;

            try
            {
                var commandeModifiee = new Commande(
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

                if (_commandeManager.ModifierCommande(id, commandeModifiee))
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

            var commande = _commandeManager.TrouverCommandeParId(id);
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
                if (_commandeManager.SupprimerCommande(id))
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

            var commande = _commandeManager.TrouverCommandeParId(id);
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

        private void RechercherCommandesParClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche des commandes par client");

            Console.Write("Nom du client : ");
            string nomClient = Console.ReadLine();

            var commandes = _commandeManager.RechercherCommandesParClient(nomClient);
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

        private void RechercherCommandesParVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche des commandes par véhicule");

            Console.Write("Immatriculation du véhicule : ");
            string immatriculation = Console.ReadLine();

            var commandes = _commandeManager.RechercherCommandesParVehicule(immatriculation);
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

            var commandes = _commandeManager.RechercherCommandesParDateLivraison(dateLivraison);
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