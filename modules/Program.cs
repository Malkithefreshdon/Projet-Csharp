using System;
using System.Diagnostics;
using OfficeOpenXml;
using Projet.Modules;
using SkiaSharp;

namespace modules
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Initialisation des managers
            var clientManager = new ClientManager();
            var commandeManager = new CommandeManager();
            var salarieManager = new SalarieManager();

            // Initialisation du service de statistiques
            var statistiqueService = new StatistiqueService(clientManager, commandeManager, salarieManager);

            // Initialisation de l'interface utilisateur
            var menuPrincipal = new MenuPrincipal(clientManager, commandeManager, salarieManager, statistiqueService);

            // Démarrage de l'application
            Console.WriteLine("Bienvenue dans le système de gestion TransConnect");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();

            // Affichage du menu principal
            //menuPrincipal.AfficherMenu();

            VisuGrapheTest();

            Console.WriteLine("\nMerci d'avoir utilisé notre système. Au revoir !");
        }

        static void VisuGrapheTest()
        {
            // Initialisation du graphe et des services
            var graphe = new GrapheListe(estNonOriente: true);
            var grapheService = new GrapheService(graphe);
            grapheService.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");

            // Initialisation de la visualisation
            var visualisation = new GrapheVisualisation(graphe);

            // Dessiner le graphe
            visualisation.DrawGraphe("graphe.png");

            // Définir les villes de départ et d'arrivée
            var villeDepart = graphe.GetToutesLesVilles().FirstOrDefault(v => v.Nom == "Lille");
            var villeArrivee = graphe.GetToutesLesVilles().FirstOrDefault(v => v.Nom == "Nantes");

            if (villeDepart == null || villeArrivee == null)
            {
                Console.WriteLine("Les villes de départ ou d'arrivée n'ont pas été trouvées.");
                return;
            }

            // Comparaison des algorithmes
            Console.WriteLine("\n--- Comparaison des algorithmes ---");

            // Dijkstra
            var chronoDijkstra = Stopwatch.StartNew();
            var (cheminDijkstra, distanceDijkstra) = grapheService.Dijkstra(villeDepart, villeArrivee);
            chronoDijkstra.Stop();
            Console.WriteLine($"Dijkstra: Distance = {distanceDijkstra:F2} km, Temps = {chronoDijkstra.Elapsed.TotalMilliseconds:F4} ms");
            visualisation.DrawPath(cheminDijkstra, "chemin_dijkstra.png", SKColors.Red);

            // Bellman-Ford
            var chronoBellmanFord = Stopwatch.StartNew();
            var (cheminBellmanFord, distanceBellmanFord) = grapheService.BellmanFord(villeDepart, villeArrivee);
            chronoBellmanFord.Stop();
            Console.WriteLine($"Bellman-Ford: Distance = {distanceBellmanFord:F2} km, Temps = {chronoBellmanFord.Elapsed.TotalMilliseconds:F4} ms");
            visualisation.DrawPath(cheminBellmanFord, "chemin_bellmanford.png", SKColors.Blue);

            // Floyd-Warshall
            var chronoFloydWarshall = Stopwatch.StartNew();
            var resultatFloydWarshall = grapheService.FloydWarshall();
            chronoFloydWarshall.Stop();

            if (resultatFloydWarshall.HasValue)
            {
                var (distancesFW, predecesseursFW) = resultatFloydWarshall.Value;
                var villes = graphe.GetToutesLesVilles().ToList();
                var indexDepart = villes.IndexOf(villeDepart);
                var indexArrivee = villes.IndexOf(villeArrivee);

                var cheminFloydWarshall = grapheService.ReconstruireCheminFloydWarshall(indexDepart, indexArrivee, distancesFW, predecesseursFW, villes);
                var distanceFloydWarshall = distancesFW[indexDepart, indexArrivee];

                Console.WriteLine($"Floyd-Warshall: Distance = {distanceFloydWarshall:F2} km, Temps = {chronoFloydWarshall.Elapsed.TotalMilliseconds:F4} ms");
                visualisation.DrawPath(cheminFloydWarshall, "chemin_floydwarshall.png", SKColors.Green);
            }
            else
            {
                Console.WriteLine("Floyd-Warshall: Erreur lors du calcul.");
            }

            Console.WriteLine("\n--- Fin de la comparaison ---");
        }
    }
}
