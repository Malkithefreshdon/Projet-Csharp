using System;
using System.Linq;
using Projet.Modules;
using SkiaSharp;
using System.Diagnostics;
using System.Collections.Generic;

namespace Projet.Modules
{
    public class SousMenus
    {
        private readonly ClientManager _clientManager;
        private readonly CommandeManager _commandeManager;
        private readonly SalarieManager _salarieManager;
        private readonly StatistiqueService _statistiqueService;
        private readonly GrapheService _grapheServiceListe;
        private readonly GrapheService _grapheServiceMatrice;
        private readonly GrapheListe _grapheListe;
        private readonly GrapheMatrice _grapheMatrice;
        private readonly VehiculeManager _vehiculeManager;

        public SousMenus(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager, StatistiqueService statistiqueService)
        {
            _clientManager = clientManager;
            _commandeManager = commandeManager;
            _salarieManager = salarieManager;
            _statistiqueService = statistiqueService;
            _vehiculeManager = new VehiculeManager();

            // Initialisation des graphes
            _grapheListe = new GrapheListe(estNonOriente: true);
            _grapheMatrice = new GrapheMatrice(estNonOriente: true);
            _grapheServiceListe = new GrapheService(_grapheListe);
            _grapheServiceMatrice = new GrapheService(_grapheMatrice);

            // Charger les données du graphe
            _grapheServiceListe.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
            _grapheServiceMatrice.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
        }

        public void AfficherMenuClients()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Clients");
                Console.WriteLine("1. Ajouter un client");
                Console.WriteLine("2. Modifier un client");
                Console.WriteLine("3. Supprimer un client");
                Console.WriteLine("4. Rechercher un client");
                Console.WriteLine("5. Afficher tous les clients");
                Console.WriteLine("6. Afficher les clients par ordre alphabétique");
                Console.WriteLine("7. Afficher les clients par ville");
                Console.WriteLine("8. Afficher les clients par montant d'achats cumulés");
                Console.WriteLine("9. Importer des clients depuis un fichier CSV");
                Console.WriteLine("10. Sauvegarder les modifications");
                Console.WriteLine("11. Recharger les données");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AjouterClient();
                        break;
                    case "2":
                        ModifierClient();
                        break;
                    case "3":
                        SupprimerClient();
                        break;
                    case "4":
                        RechercherClient();
                        break;
                    case "5":
                        AfficherTousLesClients();
                        break;
                    case "6":
                        AfficherClientsParOrdreAlphabetique();
                        break;
                    case "7":
                        AfficherClientsParVille();
                        break;
                    case "8":
                        AfficherClientsParMontantAchats();
                        break;
                    case "9":
                        ImporterClientsDepuisCSV();
                        break;
                    case "10":
                        try
                        {
                            _clientManager.SauvegarderClients();
                            Console.WriteLine("Les modifications ont été sauvegardées avec succès.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
                        }
                        Console.ReadKey();
                        break;
                    case "11":
                        try
                        {
                            _clientManager.ChargerClients();
                            Console.WriteLine("Les données ont été rechargées avec succès.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur lors du rechargement : {ex.Message}");
                        }
                        Console.ReadKey();
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

        public void AfficherMenuCommandes()
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

        public void AfficherMenuStatistiques()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Statistiques");
                Console.WriteLine("1. Nombre de commandes par ville");
                Console.WriteLine("2. Moyennes (distance/prix)");
                Console.WriteLine("3. Chauffeur le plus actif");
                Console.WriteLine("4. Commandes entre deux dates");
                Console.WriteLine("5. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherCommandesParVille();
                        break;
                    case "2":
                        AfficherMoyennes();
                        break;
                    case "3":
                        AfficherChauffeurPlusActif();
                        break;
                    case "4":
                        AfficherCommandesEntreDates();
                        break;
                    case "5":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void AfficherMenuGraphes()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Graphes");
                Console.WriteLine("1. Afficher toutes les villes");
                Console.WriteLine("2. Rechercher le plus court chemin");
                Console.WriteLine("3. Vérifier la connexité");
                Console.WriteLine("4. Vérifier les cycles");
                Console.WriteLine("5. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherVilles();
                        break;
                    case "2":
                        RechercherPlusCourtChemin();
                        break;
                    case "3":
                        VerifierConnexite();
                        break;
                    case "4":
                        VerifierCycles();
                        break;
                    case "5":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void AfficherMenuSalaries()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Salariés");
                Console.WriteLine("1. Afficher l'organigramme");
                Console.WriteLine("2. Rechercher un salarié");
                Console.WriteLine("3. Ajouter un salarié");
                Console.WriteLine("4. Supprimer un salarié");
                Console.WriteLine("5. Afficher les subordonnés d'un salarié");
                Console.WriteLine("6. Afficher les collègues d'un salarié");
                Console.WriteLine("7. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherOrganigramme();
                        break;
                    case "2":
                        RechercherSalarie();
                        break;
                    case "3":
                        AjouterSalarie();
                        break;
                    case "4":
                        SupprimerSalarie();
                        break;
                    case "5":
                        AfficherSubordonnes();
                        break;
                    case "6":
                        AfficherCollegues();
                        break;
                    case "7":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void AfficherMenuVehicules()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Véhicules");
                Console.WriteLine("1. Afficher tous les véhicules");
                Console.WriteLine("2. Afficher les véhicules par type");
                Console.WriteLine("3. Rechercher un véhicule par immatriculation");
                Console.WriteLine("4. Ajouter un véhicule");
                Console.WriteLine("5. Supprimer un véhicule");
                Console.WriteLine("6. Rechercher des véhicules par critères");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherTousLesVehicules();
                        break;
                    case "2":
                        AfficherVehiculesParType();
                        break;
                    case "3":
                        RechercherVehiculeParImmatriculation();
                        break;
                    case "4":
                        AjouterVehicule();
                        break;
                    case "5":
                        SupprimerVehicule();
                        break;
                    case "6":
                        RechercherVehiculeParCriteres();
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

        private void AjouterClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un client");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(numeroSS))
            {
                Console.WriteLine("Le numéro de sécurité sociale ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            if (_clientManager.RechercherClient(numeroSS) != null)
            {
                Console.WriteLine("Un client avec ce numéro de sécurité sociale existe déjà.");
                Console.ReadKey();
                return;
            }

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine("Le nom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(prenom))
            {
                Console.WriteLine("Le prénom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateNaissance))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(adresse))
            {
                Console.WriteLine("L'adresse ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            try
            {
                _clientManager.AjouterClient(numeroSS, nom, prenom, dateNaissance, adresse);

                Console.Write("Email (optionnel) : ");
                string email = Console.ReadLine();

                Console.Write("Téléphone (optionnel) : ");
                string telephone = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(telephone))
                {
                    _clientManager.MettreAJourClient(numeroSS, nom, adresse, email, telephone);
                }

                Console.WriteLine("\nClient ajouté avec succès !");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du client : {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ModifierClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Modification d'un client");

            Console.Write("Numéro de sécurité sociale du client à modifier : ");
            string numeroSS = Console.ReadLine();

            Client client = _clientManager.RechercherClient(numeroSS);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Modification du client : {client.Nom} {client.Prenom}");

            Console.Write($"Nouveau nom [{client.Nom}] (laisser vide pour ne pas modifier) : ");
            string nouveauNom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nouveauNom))
            {
                nouveauNom = client.Nom;
            }

            Console.Write($"Nouvelle adresse [{client.Adresse}] (laisser vide pour ne pas modifier) : ");
            string nouvelleAdresse = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nouvelleAdresse))
            {
                nouvelleAdresse = client.Adresse;
            }

            Console.Write($"Nouvel email [{client.Email}] (laisser vide pour ne pas modifier) : ");
            string nouvelEmail = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nouvelEmail))
            {
                nouvelEmail = client.Email;
            }

            Console.Write($"Nouveau téléphone [{client.Telephone}] (laisser vide pour ne pas modifier) : ");
            string nouveauTelephone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nouveauTelephone))
            {
                nouveauTelephone = client.Telephone;
            }

            try
            {
                _clientManager.MettreAJourClient(numeroSS, nouveauNom, nouvelleAdresse, nouvelEmail, nouveauTelephone);
                Console.WriteLine("Client modifié avec succès !");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification du client : {ex.Message}");
                Console.ReadKey();
            }
        }

        private void SupprimerClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un client");

            Console.Write("Numéro de sécurité sociale du client à supprimer : ");
            string numeroSS = Console.ReadLine();

            Client client = _clientManager.RechercherClient(numeroSS);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Êtes-vous sûr de vouloir supprimer le client {client.Nom} {client.Prenom} ? (O/N)");
            string confirmation = Console.ReadLine()?.ToUpper();

            if (confirmation == "O")
            {
                if (_clientManager.SupprimerClient(numeroSS))
                {
                    Console.WriteLine("Client supprimé avec succès !");
                }
                else
                {
                    Console.WriteLine("Échec de la suppression du client.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annulée.");
            }
            Console.ReadKey();
        }

        private void RechercherClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche d'un client");

            Console.WriteLine("1. Rechercher par numéro de sécurité sociale");
            Console.WriteLine("2. Rechercher par nom");
            Console.Write("\nVotre choix : ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    RechercherClientParNumeroSS();
                    break;
                case "2":
                    RechercherClientParNom();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    break;
            }
        }

        private void RechercherClientParNumeroSS()
        {
            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            Client client = _clientManager.RechercherClient(numeroSS);
            if (client != null)
            {
                Console.WriteLine("\nClient trouvé :");
                AfficherDetailsClient(client);
            }
            else
            {
                Console.WriteLine("\nClient non trouvé !");
            }
            Console.ReadKey();
        }
        private void RechercherClientParNom()
        {
            Console.Write("Nom du client : ");
            string nom = Console.ReadLine();

            var clients = _clientManager.ObtenirTousLesClients()
                .Where(c => c.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (clients.Any())
            {
                Console.WriteLine($"\n{clients.Count} client(s) trouvé(s) :");
                foreach (var client in clients)
                {
                    AfficherDetailsClient(client);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun client trouvé avec ce nom !");
            }
            Console.ReadKey();
        }

        private void AfficherTousLesClients()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste de tous les clients");

            var clients = _clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            foreach (var client in clients)
            {
                AfficherDetailsClient(client);
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine($"Total : {clients.Count} client(s)");
            Console.ReadKey();
        }

        private void AfficherClientsParOrdreAlphabetique()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par ordre alphabétique");

            var clients = _clientManager.ObtenirTousLesClients()
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToList();

            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            foreach (var client in clients)
            {
                AfficherDetailsClient(client);
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine($"Total : {clients.Count} client(s)");
            Console.ReadKey();
        }

        private void AfficherClientsParVille()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par ville");

            var clients = _clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            // Regrouper les clients par ville (extraite de l'adresse)
            var clientsParVille = clients
                .GroupBy(c => ExtraireVille(c.Adresse))
                .OrderBy(g => g.Key);

            foreach (var groupe in clientsParVille)
            {
                Console.WriteLine($"\n=== VILLE : {groupe.Key} ===");

                foreach (var client in groupe.OrderBy(c => c.Nom).ThenBy(c => c.Prenom))
                {
                    AfficherDetailsClient(client);
                    Console.WriteLine("-----------------------------------");
                }

                Console.WriteLine($"Total pour {groupe.Key} : {groupe.Count()} client(s)");
            }
            Console.ReadKey();
        }

        private void AfficherClientsParMontantAchats()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par montant d'achats cumulés");

            var clients = _clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            // Calculer le montant total des achats pour chaque client
            var clientsAvecMontant = clients
                .Select(c => new
                {
                    Client = c,
                    MontantTotal = c.HistoriqueCommandes.Sum(cmd => cmd.Prix)
                })
                .OrderByDescending(x => x.MontantTotal)
                .ToList();

            foreach (var item in clientsAvecMontant)
            {
                Console.WriteLine($"Montant total d'achats : {item.MontantTotal:C2}");
                AfficherDetailsClient(item.Client);
                Console.WriteLine("-----------------------------------");
            }
            Console.ReadKey();
        }

        private void ImporterClientsDepuisCSV()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Importation de clients depuis un fichier CSV");

            Console.Write("Chemin du fichier CSV : ");
            string cheminFichier = Console.ReadLine();

            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Le fichier spécifié n'existe pas.");
                Console.ReadKey();
                return;
            }

            try
            {
                int compteur = 0;
                string[] lignes = File.ReadAllLines(cheminFichier);

                // Vérifier si le fichier contient une ligne d'en-tête
                bool premiereEstEntete = lignes.Length > 0 &&
                                        (lignes[0].Contains("Nom") ||
                                         lignes[0].Contains("NumeroSS") ||
                                         lignes[0].Contains("Prenom"));

                int debutLecture = premiereEstEntete ? 1 : 0;

                for (int i = debutLecture; i < lignes.Length; i++)
                {
                    string ligne = lignes[i];
                    if (string.IsNullOrWhiteSpace(ligne)) continue;

                    string[] colonnes = ligne.Split(';');
                    if (colonnes.Length < 5)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Format incorrect, ignorée.");
                        continue;
                    }

                    string numeroSS = colonnes[0].Trim();
                    string nom = colonnes[1].Trim();
                    string prenom = colonnes[2].Trim();

                    if (!DateTime.TryParse(colonnes[3].Trim(), out DateTime dateNaissance))
                    {
                        Console.WriteLine($"Ligne {i + 1} : Format de date incorrect, ignorée.");
                        continue;
                    }

                    string adresse = colonnes[4].Trim();

                    // Vérifier si le client existe déjà
                    if (_clientManager.RechercherClient(numeroSS) != null)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Client avec NumeroSS {numeroSS} existe déjà, ignoré.");
                        continue;
                    }

                    try
                    {
                        _clientManager.AjouterClient(numeroSS, nom, prenom, dateNaissance, adresse);

                        // Ajouter email et téléphone s'ils sont présents
                        if (colonnes.Length > 5)
                        {
                            string email = colonnes[5].Trim();
                            string telephone = colonnes.Length > 6 ? colonnes[6].Trim() : "";

                            _clientManager.MettreAJourClient(numeroSS, nom, adresse, email, telephone);
                        }

                        compteur++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Erreur lors de l'ajout du client : {ex.Message}");
                    }
                }

                Console.WriteLine($"{compteur} client(s) importé(s) avec succès.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation du fichier : {ex.Message}");
                Console.ReadKey();
            }
        }
        private void AfficherDetailsClient(Client client)
        {
            if (client == null) return;

            double montantTotal = client.HistoriqueCommandes.Sum(c => c.Prix);

            Console.WriteLine($"Client: {client.Nom} {client.Prenom}");
            Console.WriteLine($"N°SS: {client.NumeroSS}");
            Console.WriteLine($"Date de naissance: {client.DateNaissance.ToShortDateString()}");
            Console.WriteLine($"Adresse: {client.Adresse}");
            Console.WriteLine($"Email: {client.Email ?? "Non renseigné"}");
            Console.WriteLine($"Téléphone: {client.Telephone ?? "Non renseigné"}");
            Console.WriteLine($"Nombre de commandes: {client.HistoriqueCommandes.Count}");
            Console.WriteLine($"Montant total des achats: {montantTotal:C2}");
        }

        private string ExtraireVille(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return "Inconnue";

            // Méthode simple : on suppose que la ville est après le dernier chiffre du code postal
            // Cette méthode est basique et pourrait être améliorée
            string[] parties = adresse.Split(new[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parties.Length - 1; i++)
            {
                if (parties[i].All(char.IsDigit) && parties[i].Length == 5)
                {
                    // On a trouvé ce qui ressemble à un code postal français
                    return string.Join(" ", parties.Skip(i + 1));
                }
            }

            // Si on ne trouve pas de code postal, on retourne la dernière partie
            return parties.Length > 0 ? parties[parties.Length - 1] : "Inconnue";
        }

        private void CreerCommande()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Création d'une commande");

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

            // Sélection des villes
            Console.Write("Ville de départ : ");
            string villeDepart = Console.ReadLine();
            var villeDepartObj = _grapheListe.GetToutesLesVilles()
                .FirstOrDefault(v => v.Nom.Equals(villeDepart, StringComparison.OrdinalIgnoreCase));
            if (villeDepartObj == null)
            {
                Console.WriteLine("Ville de départ non trouvée.");
                Console.ReadKey();
                return;
            }

            Console.Write("Ville d'arrivée : ");
            string villeArrivee = Console.ReadLine();
            var villeArriveeObj = _grapheListe.GetToutesLesVilles()
                .FirstOrDefault(v => v.Nom.Equals(villeArrivee, StringComparison.OrdinalIgnoreCase));
            if (villeArriveeObj == null)
            {
                Console.WriteLine("Ville d'arrivée non trouvée.");
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
                Console.WriteLine("Commande créée avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la commande : {ex.Message}");
            }
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

        private void AfficherCommandesParVille()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Commandes par ville");

            var stats = _statistiqueService.ObtenirCommandesParVille();
            foreach (var stat in stats)
            {
                Console.WriteLine($"{stat.Key}: {stat.Value} commandes");
            }
            Console.ReadKey();
        }

        private void AfficherMoyennes()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Moyennes");

            var (moyenneDistance, moyennePrix) = _statistiqueService.ObtenirMoyennes();
            Console.WriteLine($"Moyenne des distances: {moyenneDistance:F2} km");
            Console.WriteLine($"Moyenne des prix: {moyennePrix:F2} €");
            Console.ReadKey();
        }

        private void AfficherChauffeurPlusActif()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Chauffeur le plus actif");

            var chauffeur = _statistiqueService.ObtenirChauffeurPlusActif();
            if (chauffeur != null)
            {
                Console.WriteLine($"Nom: {chauffeur.Nom}");
                Console.WriteLine($"Nombre de livraisons: Non disponible");
            }
            else
            {
                Console.WriteLine("Aucun chauffeur trouvé");
            }
            Console.ReadKey();
        }

        private void AfficherCommandesEntreDates()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Commandes entre deux dates");

            Console.Write("Date de début (JJ/MM/AAAA) : ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dateDebut))
            {
                Console.Write("Date de fin (JJ/MM/AAAA) : ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dateFin))
                {
                    var commandes = _statistiqueService.ObtenirCommandesEntreDates(dateDebut, dateFin);
                    Console.WriteLine($"\nNombre de commandes trouvées: {commandes.Count}");
                    foreach (var commande in commandes)
                    {
                        Console.WriteLine(commande);
                    }
                }
            }
            Console.ReadKey();
        }

        private void AfficherVilles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste des Villes");
            var villes = _grapheListe.GetToutesLesVilles().ToList();
            foreach (var ville in villes)
            {
                Console.WriteLine($"- {ville.Nom}");
            }
            Console.ReadKey();
        }

        private void RechercherPlusCourtChemin()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche du Plus Court Chemin");

            Console.Write("Ville de départ : ");
            string depart = Console.ReadLine();
            Console.Write("Ville d'arrivée : ");
            string arrivee = Console.ReadLine();

            var villeDepart = _grapheListe.GetToutesLesVilles().FirstOrDefault(v => v.Nom.Equals(depart, StringComparison.OrdinalIgnoreCase));
            var villeArrivee = _grapheListe.GetToutesLesVilles().FirstOrDefault(v => v.Nom.Equals(arrivee, StringComparison.OrdinalIgnoreCase));

            if (villeDepart == null || villeArrivee == null)
            {
                Console.WriteLine("\nUne ou plusieurs villes n'ont pas été trouvées.");
                Console.ReadKey();
                return;
            }

            // Initialisation de la visualisation
            var visualisation = new GrapheVisualisation(_grapheListe);

            // Comparaison des algorithmes
            Console.WriteLine("\n--- Comparaison des algorithmes ---\n");

            // Dijkstra
            var chronoDijkstra = Stopwatch.StartNew();
            var (cheminDijkstra, distanceDijkstra) = _grapheServiceListe.Dijkstra(villeDepart, villeArrivee);
            chronoDijkstra.Stop();
            Console.WriteLine($"Dijkstra: Distance = {distanceDijkstra:F2} km, Temps = {chronoDijkstra.Elapsed.TotalMilliseconds:F4} ms");
            visualisation.DrawPath(cheminDijkstra, "chemin_dijkstra.png", SKColors.Red);

            // Bellman-Ford
            var chronoBellmanFord = Stopwatch.StartNew();
            var (cheminBellmanFord, distanceBellmanFord) = _grapheServiceListe.BellmanFord(villeDepart, villeArrivee);
            chronoBellmanFord.Stop();
            Console.WriteLine($"Bellman-Ford: Distance = {distanceBellmanFord:F2} km, Temps = {chronoBellmanFord.Elapsed.TotalMilliseconds:F4} ms");
            visualisation.DrawPath(cheminBellmanFord, "chemin_bellmanford.png", SKColors.Blue);

            // Floyd-Warshall
            var chronoFloydWarshall = Stopwatch.StartNew();
            var resultatFloydWarshall = _grapheServiceListe.FloydWarshall();
            chronoFloydWarshall.Stop();

            if (resultatFloydWarshall.HasValue)
            {
                var (distancesFW, predecesseursFW) = resultatFloydWarshall.Value;
                var villes = _grapheListe.GetToutesLesVilles().ToList();
                var indexDepart = villes.IndexOf(villeDepart);
                var indexArrivee = villes.IndexOf(villeArrivee);

                var cheminFloydWarshall = _grapheServiceListe.ReconstruireCheminFloydWarshall(indexDepart, indexArrivee, distancesFW, predecesseursFW, villes);
                var distanceFloydWarshall = distancesFW[indexDepart, indexArrivee];

                Console.WriteLine($"Floyd-Warshall: Distance = {distanceFloydWarshall:F2} km, Temps = {chronoFloydWarshall.Elapsed.TotalMilliseconds:F4} ms");
                visualisation.DrawPath(cheminFloydWarshall, "chemin_floydwarshall.png", SKColors.Green);
            }
            else
            {
                Console.WriteLine("Floyd-Warshall: Erreur lors du calcul.");
            }
            Console.WriteLine("Visualisations créées.");
            Console.WriteLine("\n--- Fin de la comparaison ---");
            Console.ReadKey();
        }

        private void VerifierConnexite()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification de la Connexité");
            bool connexe = _grapheServiceListe.EstConnexe();
            Console.WriteLine($"Le graphe est connexe: {connexe}");
            Console.ReadKey();
        }

        private void VerifierCycles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification des Cycles");
            bool cycle = _grapheServiceListe.ContientCycle();
            Console.WriteLine($"Le graphe contient un cycle: {cycle}");
            Console.ReadKey();
        }

        private void AfficherOrganigramme()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Organigramme");
            _salarieManager.AfficherOrganigramme();
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche de Salarié");
            Console.WriteLine("1. Par nom");
            Console.WriteLine("2. Par numéro de sécurité sociale");
            Console.Write("\nVotre choix : ");

            var choix = Console.ReadLine();
            if (choix == "1")
            {
                Console.Write("\nNom du salarié : ");
                string nom = Console.ReadLine();
                var resultats = _salarieManager.RechercherParNom(nom);
                if (resultats.Any())
                {
                    Console.WriteLine("\nSalariés trouvés :");
                    foreach (var salarie in resultats)
                    {
                        AfficherDetailsSalarie(salarie);
                        Console.WriteLine("-----------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\nAucun salarié trouvé.");
                }
            }
            else if (choix == "2")
            {
                Console.Write("\nNuméro de sécurité sociale : ");
                string numeroSS = Console.ReadLine();
                var salarie = _salarieManager.RechercherParId(numeroSS);
                if (salarie != null)
                {
                    Console.WriteLine("\nSalarié trouvé :");
                    AfficherDetailsSalarie(salarie);
                }
                else
                {
                    Console.WriteLine("\nAucun salarié trouvé.");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un Salarié");

            Console.WriteLine("1. Ajout complet d'un salarié");
            Console.WriteLine("2. Ajout rapide d'un salarié de test");
            Console.Write("\nVotre choix : ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AjouterSalarieComplet();
                    break;
                case "2":
                    AjouterSalarieTest();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    break;
            }
        }

        private void AjouterSalarieTest()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout Rapide d'un Salarié de Test");

            // Saisie des informations de base
            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine("Le nom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prenom))
            {
                Console.WriteLine("Le prénom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            // Sélection du poste
            Console.WriteLine("\nChoisissez le poste :");
            Console.WriteLine("1. Directeur");
            Console.WriteLine("2. Chef d'Équipe");
            Console.WriteLine("3. Chauffeur");
            Console.WriteLine("4. Autre (à saisir)");
            Console.Write("\nVotre choix : ");

            string poste;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    poste = "Directeur";
                    break;
                case "2":
                    poste = "Chef d'Équipe";
                    break;
                case "3":
                    poste = "Chauffeur";
                    break;
                case "4":
                    Console.Write("Saisissez le poste : ");
                    poste = Console.ReadLine()?.Trim();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    return;
            }

            if (string.IsNullOrWhiteSpace(poste))
            {
                Console.WriteLine("Le poste ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            // Sélection du manager
            string managerNumeroSS = null;
            Console.WriteLine("\nChoisissez un manager :");
            Console.WriteLine("1. Ajouter sous un manager existant");
            Console.WriteLine("2. Sans manager (racine)");
            Console.Write("\nVotre choix : ");

            bool isRoot = false;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    // Afficher la liste des managers potentiels
                    var managers = _salarieManager.GetTousLesSalaries()
                        .Where(s => s.Poste.Contains("Directeur", StringComparison.OrdinalIgnoreCase) ||
                                  s.Poste.Contains("Chef", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (!managers.Any())
                    {
                        Console.WriteLine("Aucun manager disponible.");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("\nManagers disponibles :");
                    for (int i = 0; i < managers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {managers[i].Nom} ({managers[i].Poste}) - SS: {managers[i].NumeroSecuriteSociale}");
                    }

                    Console.Write("\nChoisissez le numéro du manager (0 pour annuler) : ");
                    if (int.TryParse(Console.ReadLine(), out int choixManager) && choixManager > 0 && choixManager <= managers.Count)
                    {
                        managerNumeroSS = managers[choixManager - 1].NumeroSecuriteSociale;
                    }
                    else if (choixManager != 0)
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.ReadKey();
                        return;
                    }
                    break;

                case "2":
                    isRoot = true;
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    return;
            }

            // Générer un numéro de sécurité sociale unique avec préfixe TEST
            string numeroSS = $"TEST{DateTime.Now.ToString("yyMMddHHmmss")}";

            // Création du salarié
            var nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = numeroSS,
                Nom = nom,
                Prenom = prenom,
                Poste = poste,
                DateNaissance = new DateTime(1990, 1, 1),
                DateEntreeSociete = DateTime.Now,
                AdressePostale = "1 rue de Test, 75000 Paris",
                AdresseMail = $"{prenom.ToLower()}.{nom.ToLower()}@test.com",
                Telephone = "0123456789",
                Salaire = 30000
            };

            // Si c'est une racine, il faut d'abord vérifier s'il y a déjà une racine
            if (isRoot)
            {
                var racineExistante = _salarieManager.GetTousLesSalaries()
                    .FirstOrDefault(s => string.IsNullOrEmpty(s.ManagerNumeroSS));

                if (racineExistante != null)
                {
                    Console.WriteLine($"\nIl existe déjà une racine : {racineExistante.Nom} ({racineExistante.Poste})");
                    Console.WriteLine("Voulez-vous :");
                    Console.WriteLine("1. Définir ce salarié comme nouvelle racine");
                    Console.WriteLine("2. Ajouter sous la racine existante");
                    Console.Write("\nVotre choix : ");

                    switch (Console.ReadLine()?.Trim())
                    {
                        case "1":
                            // La racine existante devient subordonnée de la nouvelle racine
                            racineExistante.ManagerNumeroSS = numeroSS;
                            break;
                        case "2":
                            managerNumeroSS = racineExistante.NumeroSecuriteSociale;
                            break;
                        default:
                            Console.WriteLine("Choix invalide. Opération annulée.");
                            Console.ReadKey();
                            return;
                    }
                }
            }

            // Ajout du salarié
            if (_salarieManager.AjouterSalarie(nouveauSalarie, managerNumeroSS))
            {
                Console.WriteLine("\nSalarié de test ajouté avec succès !");
                Console.WriteLine("\nDétails du salarié créé :");
                AfficherDetailsSalarie(nouveauSalarie);
            }
            else
            {
                Console.WriteLine("\nErreur lors de l'ajout du salarié de test.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterSalarieComplet()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout Complet d'un Salarié");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(numeroSS))
            {
                Console.WriteLine("Le numéro de sécurité sociale ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            if (_salarieManager.RechercherParId(numeroSS) != null)
            {
                Console.WriteLine("Un salarié avec ce numéro existe déjà.");
                Console.ReadKey();
                return;
            }

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();

            Console.Write("Poste : ");
            string poste = Console.ReadLine();

            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateNaissance))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Date d'entrée dans la société (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateEntree))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Adresse postale : ");
            string adressePostale = Console.ReadLine();

            Console.Write("Email : ");
            string email = Console.ReadLine();

            Console.Write("Téléphone : ");
            string telephone = Console.ReadLine();

            Console.Write("Salaire : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salaire))
            {
                Console.WriteLine("Format de salaire invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Numéro de sécurité sociale du manager (laisser vide si aucun) : ");
            string managerNumeroSS = Console.ReadLine();

            var nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = numeroSS,
                Nom = nom,
                Prenom = prenom,
                Poste = poste,
                DateNaissance = dateNaissance,
                DateEntreeSociete = dateEntree,
                AdressePostale = adressePostale,
                AdresseMail = email,
                Telephone = telephone,
                Salaire = salaire,
                ManagerNumeroSS = string.IsNullOrWhiteSpace(managerNumeroSS) ? null : managerNumeroSS
            };

            bool ajoutOk = _salarieManager.AjouterSalarie(nouveauSalarie, managerNumeroSS);
            if (ajoutOk)
            {
                Console.WriteLine("Salarié ajouté avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'ajout du salarié.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void SupprimerSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un Salarié");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            var salarie = _salarieManager.RechercherParId(numeroSS);
            if (salarie == null)
            {
                Console.WriteLine("Salarié non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nVous allez supprimer le salarié suivant :");
            AfficherDetailsSalarie(salarie);

            Console.Write("\nÊtes-vous sûr de vouloir supprimer ce salarié ? (O/N) : ");
            if (Console.ReadLine()?.ToUpper() != "O")
            {
                Console.WriteLine("Suppression annulée.");
                Console.ReadKey();
                return;
            }

            bool suppOk = _salarieManager.SupprimerSalarie(numeroSS);
            if (suppOk)
            {
                Console.WriteLine("Salarié supprimé avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de la suppression du salarié.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherSubordonnes()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Affichage des Subordonnés");

            Console.Write("Numéro de sécurité sociale du salarié : ");
            string numeroSS = Console.ReadLine();

            var subordonnes = _salarieManager.ObtenirSubordonnesDirects(numeroSS);
            if (subordonnes.Any())
            {
                Console.WriteLine("\nSubordonnés directs :");
                foreach (var subordonne in subordonnes)
                {
                    AfficherDetailsSalarie(subordonne);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun subordonné trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherCollegues()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Affichage des Collègues");

            Console.Write("Numéro de sécurité sociale du salarié : ");
            string numeroSS = Console.ReadLine();

            var collegues = _salarieManager.ObtenirCollegues(numeroSS);
            if (collegues.Any())
            {
                Console.WriteLine("\nCollègues :");
                foreach (var collegue in collegues)
                {
                    AfficherDetailsSalarie(collegue);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun collègue trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherDetailsSalarie(Salarie salarie)
        {
            if (salarie == null) return;

            Console.WriteLine($"Nom : {salarie.Nom} {salarie.Prenom}");
            Console.WriteLine($"Poste : {salarie.Poste}");
            Console.WriteLine($"N° SS : {salarie.NumeroSecuriteSociale}");
            Console.WriteLine($"Date de naissance : {salarie.DateNaissance:dd/MM/yyyy}");
            Console.WriteLine($"Date d'entrée : {salarie.DateEntreeSociete:dd/MM/yyyy}");
            Console.WriteLine($"Adresse : {salarie.AdressePostale}");
            Console.WriteLine($"Email : {salarie.AdresseMail ?? "Non renseigné"}");
            Console.WriteLine($"Téléphone : {salarie.Telephone ?? "Non renseigné"}");
            Console.WriteLine($"Salaire : {salarie.Salaire:C2}");
            Console.WriteLine($"Manager : {salarie.ManagerNumeroSS ?? "Aucun"}");
        }

        private void AfficherTousLesVehicules()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste de tous les véhicules");

            var vehicules = _vehiculeManager.ObtenirTousLesVehicules();
            if (!vehicules.Any())
            {
                Console.WriteLine("Aucun véhicule enregistré.");
            }
            else
            {
                foreach (var vehicule in vehicules)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
                Console.WriteLine($"\nTotal : {vehicules.Count} véhicule(s)");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherVehiculesParType()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Véhicules par type");
            Console.WriteLine("1. Voitures");
            Console.WriteLine("2. Camionnettes");
            Console.WriteLine("3. Camions Citernes");
            Console.WriteLine("4. Camions Bennes");
            Console.WriteLine("5. Camions Frigorifiques");
            Console.WriteLine("6. Poids Lourds");
            Console.Write("\nChoisissez un type : ");

            var choix = Console.ReadLine();
            Console.Clear();

            List<Vehicule> vehicules = choix switch
            {
                "1" => _vehiculeManager.ObtenirVehiculesParType<Voiture>().Cast<Vehicule>().ToList(),
                "2" => _vehiculeManager.ObtenirVehiculesParType<Camionnette>().Cast<Vehicule>().ToList(),
                "3" => _vehiculeManager.ObtenirVehiculesParType<CamionCiterne>().Cast<Vehicule>().ToList(),
                "4" => _vehiculeManager.ObtenirVehiculesParType<CamionBenne>().Cast<Vehicule>().ToList(),
                "5" => _vehiculeManager.ObtenirVehiculesParType<CamionFrigorifique>().Cast<Vehicule>().ToList(),
                "6" => _vehiculeManager.ObtenirVehiculesParType<PoidsLourd>().Cast<Vehicule>().ToList(),
                _ => new List<Vehicule>()
            };

            if (!vehicules.Any())
            {
                Console.WriteLine("Aucun véhicule de ce type trouvé.");
            }
            else
            {
                foreach (var vehicule in vehicules)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
                Console.WriteLine($"\nTotal : {vehicules.Count} véhicule(s)");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherVehiculeParImmatriculation()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche par immatriculation");

            Console.Write("Entrez l'immatriculation : ");
            string immatriculation = Console.ReadLine();

            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule != null)
            {
                Console.WriteLine("\nVéhicule trouvé :");
                Console.WriteLine(vehicule.GetDescription());
            }
            else
            {
                Console.WriteLine("\nAucun véhicule trouvé avec cette immatriculation.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un véhicule");

            Console.WriteLine("Type de véhicule :");
            Console.WriteLine("1. Voiture");
            Console.WriteLine("2. Camionnette");
            Console.WriteLine("3. Camion Citerne");
            Console.WriteLine("4. Camion Benne");
            Console.WriteLine("5. Camion Frigorifique");
            Console.WriteLine("6. Poids Lourd");
            Console.Write("\nChoisissez un type : ");

            var choix = Console.ReadLine();

            // Informations communes
            Console.Write("\nImmatriculation : ");
            string immatriculation = Console.ReadLine();
            Console.Write("Poids maximal (tonnes) : ");
            double.TryParse(Console.ReadLine(), out double poidsMaximal);
            Console.Write("Marque : ");
            string marque = Console.ReadLine();
            Console.Write("Modèle : ");
            string modele = Console.ReadLine();

            try
            {
                Vehicule nouveauVehicule = null;

                switch (choix)
                {
                    case "1": // Voiture
                        Console.Write("Nombre de passagers : ");
                        int.TryParse(Console.ReadLine(), out int nombrePassagers);
                        nouveauVehicule = new Voiture(immatriculation, poidsMaximal, marque, modele, nombrePassagers);
                        break;

                    case "2": // Camionnette
                        Console.Write("Usage : ");
                        string usage = Console.ReadLine();
                        Console.Write("Transport de verre (O/N) : ");
                        bool transportVerre = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new Camionnette(immatriculation, poidsMaximal, marque, modele, usage, transportVerre);
                        break;

                    case "3": // Camion Citerne
                        Console.Write("Capacité de la cuve (litres) : ");
                        double.TryParse(Console.ReadLine(), out double capaciteCuve);
                        Console.Write("Type de produit : ");
                        string typeProduit = Console.ReadLine();
                        nouveauVehicule = new CamionCiterne(immatriculation, poidsMaximal, marque, modele, capaciteCuve, typeProduit);
                        break;

                    case "4": // Camion Benne
                        Console.Write("Nombre de bennes : ");
                        int.TryParse(Console.ReadLine(), out int nombreBennes);
                        Console.Write("Équipé d'une grue (O/N) : ");
                        bool hasGrue = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new CamionBenne(immatriculation, poidsMaximal, marque, modele, nombreBennes, hasGrue);
                        break;

                    case "5": // Camion Frigorifique
                        Console.Write("Température minimale : ");
                        double.TryParse(Console.ReadLine(), out double temperatureMin);
                        Console.Write("Équipé d'un groupe électrogène (O/N) : ");
                        bool hasGroupeElectrogene = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new CamionFrigorifique(immatriculation, poidsMaximal, marque, modele, temperatureMin, hasGroupeElectrogene);
                        break;

                    case "6": // Poids Lourd
                        Console.Write("Volume de la remorque (m³) : ");
                        double.TryParse(Console.ReadLine(), out double volumeRemorque);
                        Console.Write("Type de remorque : ");
                        string typeRemorque = Console.ReadLine();
                        Console.Write("Type de marchandise : ");
                        string typeMarchandise = Console.ReadLine();
                        Console.Write("Équipé d'un hayon (O/N) : ");
                        bool hasHayon = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new PoidsLourd(immatriculation, poidsMaximal, marque, modele, volumeRemorque, typeRemorque, typeMarchandise, hasHayon);
                        break;
                }

                if (nouveauVehicule != null)
                {
                    _vehiculeManager.AjouterVehicule(nouveauVehicule);
                    Console.WriteLine("\nVéhicule ajouté avec succès !");
                }
                else
                {
                    Console.WriteLine("\nType de véhicule non reconnu.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErreur lors de l'ajout du véhicule : {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void SupprimerVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un véhicule");

            Console.Write("Entrez l'immatriculation du véhicule à supprimer : ");
            string immatriculation = Console.ReadLine();

            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
            {
                Console.WriteLine("\nAucun véhicule trouvé avec cette immatriculation.");
            }
            else
            {
                Console.WriteLine("\nVéhicule trouvé :");
                Console.WriteLine(vehicule.GetDescription());
                Console.Write("\nÊtes-vous sûr de vouloir supprimer ce véhicule ? (O/N) : ");

                if (Console.ReadLine()?.ToUpper() == "O")
                {
                    if (_vehiculeManager.SupprimerVehicule(immatriculation))
                    {
                        Console.WriteLine("\nVéhicule supprimé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("\nErreur lors de la suppression du véhicule.");
                    }
                }
                else
                {
                    Console.WriteLine("\nSuppression annulée.");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherVehiculeParCriteres()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche par critères");

            Console.WriteLine("1. Rechercher par marque");
            Console.WriteLine("2. Rechercher par poids maximal");
            Console.WriteLine("3. Retour");
            Console.Write("\nVotre choix : ");

            var choix = Console.ReadLine();
            List<Vehicule> resultats = new List<Vehicule>();

            switch (choix)
            {
                case "1":
                    Console.Write("\nEntrez la marque : ");
                    string marque = Console.ReadLine();
                    resultats = _vehiculeManager.RechercherVehicules(v =>
                        v.Marque.Contains(marque, StringComparison.OrdinalIgnoreCase));
                    break;

                case "2":
                    Console.Write("\nPoids maximal minimum (tonnes) : ");
                    if (double.TryParse(Console.ReadLine(), out double poidsMin))
                    {
                        resultats = _vehiculeManager.RechercherVehicules(v => v.PoidsMaximal >= poidsMin);
                    }
                    break;
            }

            if (resultats.Any())
            {
                Console.WriteLine($"\n{resultats.Count} véhicule(s) trouvé(s) :");
                foreach (var vehicule in resultats)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun véhicule trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        public void AfficherMenuFidelite()
        {
            bool continuer = true;
            SystemeFidelite fidelite = new SystemeFidelite();

            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Programme de Fidélité");
                Console.WriteLine("1. Consulter le statut d'un client");
                Console.WriteLine("2. Simuler une remise");
                Console.WriteLine("0. Retour");
                Console.Write("\nVotre choix : ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Numéro de sécurité sociale du client : ");
                        string numeroSS = Console.ReadLine();
                        Client client = _clientManager.RechercherClient(numeroSS);

                        if (client != null)
                            fidelite.AfficherInfos(client);
                        else
                            Console.WriteLine("Client non trouvé.");

                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.Write("Numéro de sécurité sociale du client : ");
                        string num = Console.ReadLine();
                        Client c = _clientManager.RechercherClient(num);

                        if (c == null)
                        {
                            Console.WriteLine("Client non trouvé.");
                            Console.ReadKey();
                            break;
                        }

                        Console.Write("Prix de la commande : ");
                        if (double.TryParse(Console.ReadLine(), out double prix))
                            fidelite.AfficherRecapitulatif(c, prix);
                        else
                            Console.WriteLine("Prix invalide.");

                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "0":
                        continuer = false;
                        break;

                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        public void AfficherMenuFinance()
        {
            // Créer une instance avec les managers existants pour accéder aux données
            FinanceSimple finance = new FinanceSimple(null, _commandeManager, _salarieManager);
            bool continuer = true;

            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion Financière");
                Console.WriteLine("1. Tableau de bord financier");
                Console.WriteLine("2. Historique des transactions");
                Console.WriteLine("3. Ajouter une entrée d'argent");
                Console.WriteLine("4. Ajouter une sortie d'argent");
                Console.WriteLine("5. Synchroniser avec les commandes");
                Console.WriteLine("6. Générer transactions de salaires");
                Console.WriteLine("7. Générer rapports financiers");
                Console.WriteLine("0. Retour");
                Console.Write("\nVotre choix : ");

                switch (Console.ReadLine())
                {
                    case "1":
                        finance.AfficherTableauDeBord();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "2":
                        finance.AfficherHistorique();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "3":
                        AjouterEntreeArgent(finance);
                        break;

                    case "4":
                        AjouterSortieArgent(finance);
                        break;

                    case "5":
                        finance.SynchroniserAvecCommandes();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "6":
                        finance.GenererTransactionsSalaires();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;

                    case "7":
                        finance.GenererRapports();
                        break;

                    case "0":
                        continuer = false;
                        break;

                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AjouterEntreeArgent(FinanceSimple finance)
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajouter une Entrée d'Argent");

            Console.Write("Montant (€) : ");
            if (!double.TryParse(Console.ReadLine(), out double montant) || montant <= 0)
            {
                Console.WriteLine("Montant invalide. Opération annulée.");
                Console.ReadKey();
                return;
            }

            // Choix de la catégorie
            Console.WriteLine("\nCatégorie :");
            Console.WriteLine("1. Transport");
            Console.WriteLine("2. Vente de véhicule");
            Console.WriteLine("3. Subvention");
            Console.WriteLine("4. Autre");
            Console.Write("\nVotre choix : ");

            string categorie;
            switch (Console.ReadLine())
            {
                case "1": categorie = "Transport"; break;
                case "2": categorie = "Vente de véhicule"; break;
                case "3": categorie = "Subvention"; break;
                case "4":
                    Console.Write("Précisez la catégorie : ");
                    categorie = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(categorie))
                        categorie = "Divers";
                    break;
                default: categorie = "Divers"; break;
            }

            Console.Write("Description : ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = $"Entrée d'argent - {categorie}";
            }

            // Ajouter la transaction
            finance.AjouterTransaction(montant, "Crédit", description, categorie);

            Console.WriteLine($"\nTransaction de {montant:N2} € ajoutée avec succès!");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterSortieArgent(FinanceSimple finance)
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajouter une Sortie d'Argent");

            Console.Write("Montant (€) : ");
            if (!double.TryParse(Console.ReadLine(), out double montant) || montant <= 0)
            {
                Console.WriteLine("Montant invalide. Opération annulée.");
                Console.ReadKey();
                return;
            }

            // Choix de la catégorie
            Console.WriteLine("\nCatégorie :");
            Console.WriteLine("1. Salaires");
            Console.WriteLine("2. Carburant");
            Console.WriteLine("3. Maintenance");
            Console.WriteLine("4. Achat de véhicule");
            Console.WriteLine("5. Assurance");
            Console.WriteLine("6. Taxes");
            Console.WriteLine("7. Autre");
            Console.Write("\nVotre choix : ");

            string categorie;
            switch (Console.ReadLine())
            {
                case "1": categorie = "Salaires"; break;
                case "2": categorie = "Carburant"; break;
                case "3": categorie = "Maintenance"; break;
                case "4": categorie = "Achat de véhicule"; break;
                case "5": categorie = "Assurance"; break;
                case "6": categorie = "Taxes"; break;
                case "7":
                    Console.Write("Précisez la catégorie : ");
                    categorie = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(categorie))
                        categorie = "Divers";
                    break;
                default: categorie = "Divers"; break;
            }

            Console.Write("Description : ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = $"Sortie d'argent - {categorie}";
            }

            // Ajouter la transaction
            finance.AjouterTransaction(montant, "Débit", description, categorie);

            Console.WriteLine($"\nTransaction de {montant:N2} € ajoutée avec succès!");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
}
