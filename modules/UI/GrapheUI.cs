using System;
using System.Linq;
using System.Collections.Generic;
using Projet.Modules;
using SkiaSharp;
using System.Diagnostics;

namespace Projet.Modules.UI
{
    public class GrapheUI
    {
        private readonly GrapheListe _grapheListe;
        private readonly GrapheMatrice _grapheMatrice;
        private readonly GrapheService _grapheServiceListe;
        private readonly GrapheService _grapheServiceMatrice;
        private bool _donneeesChargees;

        public GrapheUI(GrapheListe grapheListe, GrapheMatrice grapheMatrice)
        {
            _grapheListe = grapheListe;
            _grapheMatrice = grapheMatrice;
            _grapheServiceListe = new GrapheService(_grapheListe);
            _grapheServiceMatrice = new GrapheService(_grapheMatrice);
            _donneeesChargees = false;
        }

        private void ChargerDonneesGraphe()
        {
            if (!_donneeesChargees)
            {
                try
                {
                    _grapheServiceListe.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    _grapheServiceMatrice.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    _donneeesChargees = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement des données : {ex.Message}");
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
                ConsoleHelper.AfficherTitre("Gestion des Graphes");
                Console.WriteLine("1. Afficher toutes les villes");
                Console.WriteLine("2. Rechercher le plus court chemin");
                Console.WriteLine("3. Vérifier la connexité");
                Console.WriteLine("4. Vérifier les cycles");
                Console.WriteLine("5. Recharger les données");
                Console.WriteLine("0. Retour");
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
                        _donneeesChargees = false;
                        ChargerDonneesGraphe();
                        Console.WriteLine("Données rechargées.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
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

        private void AfficherVilles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste des Villes");

            ChargerDonneesGraphe();

            var villes = _grapheListe.GetToutesLesVilles().ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Aucune ville n'a été trouvée dans le graphe.");
                Console.WriteLine("Vérifiez que le fichier de données est présent et correctement formaté.");
            }
            else
            {
                Console.WriteLine($"Nombre total de villes : {villes.Count}");
                Console.WriteLine("\nListe des villes :");
                foreach (var ville in villes.OrderBy(v => v.Nom))
                {
                    Console.WriteLine($"- {ville.Nom}");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherPlusCourtChemin()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche du Plus Court Chemin");

            ChargerDonneesGraphe();

            var villes = _grapheListe.GetToutesLesVilles().ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Aucune ville n'est disponible dans le graphe.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Villes disponibles :");
            foreach (var ville in villes.OrderBy(v => v.Nom))
            {
                Console.WriteLine($"- {ville.Nom}");
            }

            Console.Write("\nVille de départ : ");
            string depart = Console.ReadLine();
            Console.Write("Ville d'arrivée : ");
            string arrivee = Console.ReadLine();

            var villeDepart = villes.FirstOrDefault(v => v.Nom.Equals(depart, StringComparison.OrdinalIgnoreCase));
            var villeArrivee = villes.FirstOrDefault(v => v.Nom.Equals(arrivee, StringComparison.OrdinalIgnoreCase));

            if (villeDepart == null || villeArrivee == null)
            {
                Console.WriteLine("\nUne ou plusieurs villes n'ont pas été trouvées.");
                Console.WriteLine("Vérifiez l'orthographe et respectez la casse.");
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
            if (cheminDijkstra.Any())
            {
                Console.WriteLine("Chemin trouvé : " + string.Join(" -> ", cheminDijkstra.Select(v => v.Nom)));
                visualisation.DrawPath(cheminDijkstra, "chemin_dijkstra.png", SKColors.Red);
            }

            // Bellman-Ford
            var chronoBellmanFord = Stopwatch.StartNew();
            var (cheminBellmanFord, distanceBellmanFord) = _grapheServiceListe.BellmanFord(villeDepart, villeArrivee);
            chronoBellmanFord.Stop();
            Console.WriteLine($"\nBellman-Ford: Distance = {distanceBellmanFord:F2} km, Temps = {chronoBellmanFord.Elapsed.TotalMilliseconds:F4} ms");
            if (cheminBellmanFord.Any())
            {
                Console.WriteLine("Chemin trouvé : " + string.Join(" -> ", cheminBellmanFord.Select(v => v.Nom)));
                visualisation.DrawPath(cheminBellmanFord, "chemin_bellmanford.png", SKColors.Blue);
            }

            // Floyd-Warshall
            var chronoFloydWarshall = Stopwatch.StartNew();
            var resultatFloydWarshall = _grapheServiceListe.FloydWarshall();
            chronoFloydWarshall.Stop();

            if (resultatFloydWarshall.HasValue)
            {
                var (distancesFW, predecesseursFW) = resultatFloydWarshall.Value;
                var indexDepart = villes.IndexOf(villeDepart);
                var indexArrivee = villes.IndexOf(villeArrivee);

                var cheminFloydWarshall = _grapheServiceListe.ReconstruireCheminFloydWarshall(indexDepart, indexArrivee, distancesFW, predecesseursFW, villes);
                var distanceFloydWarshall = distancesFW[indexDepart, indexArrivee];

                Console.WriteLine($"\nFloyd-Warshall: Distance = {distanceFloydWarshall:F2} km, Temps = {chronoFloydWarshall.Elapsed.TotalMilliseconds:F4} ms");
                if (cheminFloydWarshall.Any())
                {
                    Console.WriteLine("Chemin trouvé : " + string.Join(" -> ", cheminFloydWarshall.Select(v => v.Nom)));
                    visualisation.DrawPath(cheminFloydWarshall, "chemin_floydwarshall.png", SKColors.Green);
                }
            }
            else
            {
                Console.WriteLine("\nFloyd-Warshall: Erreur lors du calcul.");
            }

            Console.WriteLine("\nVisualisations créées.");
            Console.WriteLine("\n--- Fin de la comparaison ---");
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void VerifierConnexite()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification de la Connexité");

            ChargerDonneesGraphe();

            bool connexe = _grapheServiceListe.EstConnexe();
            Console.WriteLine($"Le graphe est{(connexe ? "" : " non")} connexe.");
            if (!connexe)
            {
                Console.WriteLine("\nCela signifie qu'il existe des villes qui ne sont pas accessibles depuis certaines autres villes.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void VerifierCycles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification des Cycles");

            ChargerDonneesGraphe();

            bool cycle = _grapheServiceListe.ContientCycle();
            Console.WriteLine($"Le graphe{(cycle ? "" : " ne")} contient{(cycle ? "" : " pas")} de cycle.");
            if (cycle)
            {
                Console.WriteLine("\nCela signifie qu'il existe au moins un chemin qui permet de revenir à une ville de départ en passant par d'autres villes.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
} 