using System;
using System.Linq;
using Projet.Modules;

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

        public SousMenus(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager, StatistiqueService statistiqueService)
        {
            _clientManager = clientManager;
            _commandeManager = commandeManager;
            _salarieManager = salarieManager;
            _statistiqueService = statistiqueService;

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
                Console.WriteLine("2. Associer une commande à un client");
                Console.WriteLine("3. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        // TODO: Implémenter la création de commande
                        break;
                    case "2":
                        AssocierCommandeClient();
                        break;
                    case "3":
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
                Console.WriteLine("5. Retour");
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


        private void AssocierCommandeClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Association commande-client");
            
            Console.Write("Nom du client : ");
            string nomClient = Console.ReadLine();

            // TODO: Implémenter la sélection de la commande
            Console.WriteLine("Fonctionnalité à implémenter");
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

            if (villeDepart != null && villeArrivee != null)
            {
                var chemin = _grapheServiceListe.Dijkstra(villeDepart, villeArrivee);
                if (chemin.Item1 != null)
                {
                    Console.WriteLine("\nChemin trouvé :");
                    foreach (var ville in chemin.Item1)
                    {
                        Console.WriteLine($"- {ville.Nom}");
                    }
                }
                else
                {
                    Console.WriteLine("\nAucun chemin trouvé.");
                }
            }
            else
            {
                Console.WriteLine("\nUne ou plusieurs villes n'ont pas été trouvées.");
            }
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
                Console.Write("Nom du salarié : ");
                string nom = Console.ReadLine();
                var resultats = _salarieManager.RechercherParNom(nom);
                if (resultats.Any())
                {
                    foreach (var salarie in resultats)
                    {
                        Console.WriteLine(salarie);
                    }
                }
                else
                {
                    Console.WriteLine("Aucun salarié trouvé.");
                }
            }
            else if (choix == "2")
            {
                Console.Write("Numéro de sécurité sociale : ");
                string numeroSS = Console.ReadLine();
                var salarie = _salarieManager.RechercherParId(numeroSS);
                if (salarie != null)
                {
                    Console.WriteLine(salarie);
                }
                else
                {
                    Console.WriteLine("Aucun salarié trouvé.");
                }
            }
            Console.ReadKey();
        }

        private void AjouterSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un Salarié");
            
            Console.Write("Type (1: Responsable, 2: Livreur) : ");
            string type = Console.ReadLine();
            
            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();
            
            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            
            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            
            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            DateTime dateNaissance = DateTime.Parse(Console.ReadLine());
            
            Console.Write("Date d'embauche (JJ/MM/AAAA) : ");
            DateTime dateEmbauche = DateTime.Parse(Console.ReadLine());
            
            Console.Write("Salaire : ");
            double salaire = double.Parse(Console.ReadLine());
            
            Console.Write("Numéro de sécurité sociale du manager : ");
            string managerNumeroSS = Console.ReadLine();

            Salarie nouveauSalarie;
            if (type == "1")
            {
                nouveauSalarie = new Responsable(numeroSS, nom, prenom, dateNaissance, dateEmbauche) { Salaire = salaire };
            }
            else
            {
                nouveauSalarie = new Chauffeur(numeroSS, nom, prenom, dateNaissance, dateEmbauche) { Salaire = salaire };
            }

            bool ajoutOk = _salarieManager.AjouterSalarie(nouveauSalarie, managerNumeroSS);
            if (ajoutOk)
            {
                Console.WriteLine("Salarié ajouté avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'ajout du salarié.");
            }
            Console.ReadKey();
        }

        private void SupprimerSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un Salarié");
            
            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            bool suppOk = _salarieManager.SupprimerSalarie(numeroSS);
            if (suppOk)
            {
                Console.WriteLine("Salarié supprimé avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de la suppression du salarié.");
            }
            Console.ReadKey();
        }
    }
}