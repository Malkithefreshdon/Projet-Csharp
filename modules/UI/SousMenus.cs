using System;
using System.Linq;
using modules.Commandes;
using modules.UI;
using Projet.Modules.Clients;
using modules.Commandes;
using Projet.Modules.Statistiques;

namespace modules.UI
{
    public class SousMenus
    {
        private readonly ClientManager _clientManager;
        private readonly CommandeManager _commandeManager;
        private readonly StatistiqueService _statistiqueService;

        public SousMenus(ClientManager clientManager, CommandeManager commandeManager, StatistiqueService statistiqueService)
        {
            _clientManager = clientManager;
            _commandeManager = commandeManager;
            _statistiqueService = statistiqueService;
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

        private void AjouterClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un client");

            Console.Write("Nom du client : ");
            string nom = Console.ReadLine();

            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();

            var client = new Client(nom, adr);
            _clientManager.AjouterClient(client);

            Console.WriteLine("\nClient ajouté avec succès !");
            Console.ReadKey();
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
                Console.WriteLine($"Nombre de livraisons: {chauffeur.NombreLivraisons}");
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
    }
}