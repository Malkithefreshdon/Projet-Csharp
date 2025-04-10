using System;
using System.Linq;
using Projet.Modules;

namespace Projet.UI
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
            CreerFichierDistancesSiAbsent();
            _grapheServiceListe.ChargerGrapheDepuisCsv("Ressources/Distances.csv");
            _grapheServiceMatrice.ChargerGrapheDepuisCsv("Ressources/Distances.csv");
        }

        private void CreerFichierDistancesSiAbsent()
        {
            if (!System.IO.File.Exists("Ressources/Distances.csv"))
            {
                Console.WriteLine("Création du fichier Distances.csv de démonstration...");
                string csvContent = "VilleA;VilleB;Distance\n" +
                                    "Paris;Lyon;465\n" +
                                    "Lyon;Marseille;315\n" +
                                    "Paris;Lille;225\n" +
                                    "Lille;Lyon;690\n" +
                                    "Paris;Nantes;385\n" +
                                    "Nantes;Bordeaux;345\n" +
                                    "Lyon;Bordeaux;550\n" +
                                    "Paris;Strasbourg;490";
                System.IO.File.WriteAllText("Ressources/Distances.csv", csvContent);
            }
        }

        public void AfficherMenuClients()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Clients");
                Console.WriteLine("1. Ajouter un client");
                Console.WriteLine("2. Supprimer un client");
                Console.WriteLine("3. Rechercher un client");
                Console.WriteLine("4. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AjouterClient();
                        break;
                    case "2":
                        SupprimerClient();
                        break;
                    case "3":
                        RechercherClient();
                        break;
                    case "4":
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

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            
            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();

            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dateNaissance))
            {
                _clientManager.AjouterClient(numeroSS, nom, prenom, dateNaissance, adresse);
                
                Console.WriteLine("\nClient ajouté avec succès !");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Date de naissance invalide. Veuillez réessayer.");
                Console.ReadKey();
            }
        }

        private void SupprimerClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un client");
            
            Console.Write("Nom du client à supprimer : ");
            string nom = Console.ReadLine();

            if (_clientManager.SupprimerClient(nom))
            {
                Console.WriteLine("\nClient supprimé avec succès !");
            }
            else
            {
                Console.WriteLine("\nClient non trouvé !");
            }
            Console.ReadKey();
        }

        private void RechercherClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche d'un client");
            
            Console.Write("Nom du client à rechercher : ");
            string nom = Console.ReadLine();

            var client = _clientManager.RechercherClient(nom);
            if (client != null)
            {
                Console.WriteLine("\nClient trouvé :");
                Console.WriteLine(client);
            }
            else
            {
                Console.WriteLine("\nClient non trouvé !");
            }
            Console.ReadKey();
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