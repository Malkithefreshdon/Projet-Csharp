using System;
using System.Linq;
using Projet.Modules;
using System.Collections.Generic;

namespace Projet.Modules.UI
{
    public class StatistiqueServiceUI
    {
        private readonly StatistiqueService statistiqueService;

        public StatistiqueServiceUI(StatistiqueService statistiqueService)
        {
            this.statistiqueService = statistiqueService;
        }

        public void AfficherMenu()
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
                Console.WriteLine("5. Livraisons par chauffeur");
                Console.WriteLine("6. Moyenne des comptes clients");
                Console.WriteLine("7. Commandes d'un client");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                string choix = Console.ReadLine();
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
                        AfficherLivraisonsParChauffeur();
                        break;
                    case "6":
                        AfficherMoyenneCompteClients();
                        break;
                    case "7":
                        AfficherCommandesClient();
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

        private void AfficherCommandesParVille()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Commandes par ville");

            Dictionary<string, int> stats = statistiqueService.ObtenirCommandesParVille();
            foreach (KeyValuePair<string, int> stat in stats)
            {
                Console.WriteLine($"{stat.Key}: {stat.Value} commandes");
            }
            Console.ReadKey();
        }

        private void AfficherMoyennes()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Moyennes");

            (double moyenneDistance, double moyennePrix) = statistiqueService.ObtenirMoyennes();
            Console.WriteLine($"Moyenne des distances: {moyenneDistance:F2} km");
            Console.WriteLine($"Moyenne des prix: {moyennePrix:F2} €");
            Console.ReadKey();
        }

        private void AfficherChauffeurPlusActif()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Chauffeur le plus actif");

            Salarie chauffeur = statistiqueService.ObtenirChauffeurPlusActif();
            if (chauffeur != null)
            {
                Dictionary<string, int> livraisonsParChauffeur = statistiqueService.ObtenirLivraisonsParChauffeur();
                int nombreLivraisons = livraisonsParChauffeur[$"{chauffeur.Nom} {chauffeur.Prenom}"];
                Console.WriteLine($"Nom: {chauffeur.Nom} {chauffeur.Prenom}");
                Console.WriteLine($"Nombre de livraisons: {nombreLivraisons}");
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
                    List<Commande> commandes = statistiqueService.ObtenirCommandesEntreDates(dateDebut, dateFin);
                    Console.WriteLine($"\nNombre de commandes trouvées: {commandes.Count}");
                    foreach (Commande commande in commandes)
                    {
                        Console.WriteLine(commande);
                    }
                }
            }
            Console.ReadKey();
        }

        private void AfficherLivraisonsParChauffeur()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Livraisons par chauffeur");

            Dictionary<string, int> stats = statistiqueService.ObtenirLivraisonsParChauffeur();
            foreach (KeyValuePair<string, int> stat in stats.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{stat.Key}: {stat.Value} livraisons");
            }
            Console.ReadKey();
        }

        private void AfficherMoyenneCompteClients()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Moyenne des comptes clients");

            double moyenne = statistiqueService.ObtenirMoyenneCompteClients();
            Console.WriteLine($"Moyenne des dépenses par client: {moyenne:C2}");
            Console.ReadKey();
        }

        private void AfficherCommandesClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Commandes d'un client");

            Console.Write("Numéro de sécurité sociale du client: ");
            string idClient = Console.ReadLine();

            List<Commande> commandes = statistiqueService.ObtenirCommandesClient(idClient);
            Console.WriteLine($"\nNombre de commandes trouvées: {commandes.Count}");
            foreach (Commande commande in commandes)
            {
                Console.WriteLine($"\nDate: {commande.DateCommande:dd/MM/yyyy}");
                Console.WriteLine($"De: {commande.VilleDepart.Nom} → À: {commande.VilleArrivee.Nom}");
                Console.WriteLine($"Prix: {commande.Prix:C2}");
                Console.WriteLine($"Distance: {commande.DistanceCalculee:F2} km");
                Console.WriteLine("----------------------------------------");
            }
            Console.ReadKey();
        }
    }
}