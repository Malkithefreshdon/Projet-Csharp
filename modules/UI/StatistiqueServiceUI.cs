using System;
using System.Linq;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class StatistiqueServiceUI
    {
        private readonly StatistiqueService _statistiqueService;

        public StatistiqueServiceUI(StatistiqueService statistiqueService)
        {
            _statistiqueService = statistiqueService;
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
    }
} 