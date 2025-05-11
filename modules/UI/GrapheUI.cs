using System;
using System.Linq;
using System.Collections.Generic;
using Projet.Modules;
using SkiaSharp;
using System.Diagnostics;

namespace Projet.Modules.UI
{
    /// <summary>
    /// Interface utilisateur pour interagir avec les graphes.
    /// </summary>
    public class GrapheUI
    {
        private readonly GrapheListe grapheListe;
        private readonly GrapheMatrice grapheMatrice;
        private readonly GrapheService grapheServiceListe;
        private readonly GrapheService grapheServiceMatrice;
        private bool donneeesChargees;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GrapheUI.
        /// </summary>
        /// <param name="grapheListe">L'implémentation du graphe avec liste d'adjacence.</param>
        /// <param name="grapheMatrice">L'implémentation du graphe avec matrice d'adjacence.</param>
        public GrapheUI(GrapheListe grapheListe, GrapheMatrice grapheMatrice)
        {
            this.grapheListe = grapheListe;
            this.grapheMatrice = grapheMatrice;
            this.grapheServiceListe = new GrapheService(this.grapheListe);
            this.grapheServiceMatrice = new GrapheService(this.grapheMatrice);
            this.donneeesChargees = false;
        }

        /// <summary>
        /// Charge les données du graphe depuis un fichier Excel.
        /// </summary>
        private void ChargerDonneesGraphe()
        {
            if (!this.donneeesChargees)
            {
                try
                {
                    this.grapheServiceListe.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    this.grapheServiceMatrice.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                    this.donneeesChargees = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement des données : {ex.Message}");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Affiche le menu principal et gère les interactions utilisateur.
        /// </summary>
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

                string choix = Console.ReadLine();
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
                        this.donneeesChargees = false;
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

        /// <summary>
        /// Affiche la liste de toutes les villes du graphe.
        /// </summary>
        private void AfficherVilles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste des Villes");

            ChargerDonneesGraphe();

            List<Ville> villes = this.grapheListe.GetToutesLesVilles().ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Aucune ville n'a été trouvée dans le graphe.");
                Console.WriteLine("Vérifiez que le fichier de données est présent et correctement formaté.");
            }
            else
            {
                Console.WriteLine($"Nombre total de villes : {villes.Count}");
                Console.WriteLine("\nListe des villes :");
                foreach (Ville ville in villes.OrderBy(v => v.Nom))
                {
                    Console.WriteLine($"- {ville.Nom}");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// <summary>
        /// Permet de rechercher et visualiser le plus court chemin entre deux villes.
        /// </summary>
        private void RechercherPlusCourtChemin()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche du Plus Court Chemin");

            ChargerDonneesGraphe();

            List<Ville> villes = this.grapheListe.GetToutesLesVilles().ToList();
            if (!villes.Any())
            {
                Console.WriteLine("Aucune ville n'est disponible dans le graphe.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Villes disponibles :");
            foreach (Ville ville in villes.OrderBy(v => v.Nom))
            {
                Console.WriteLine($"- {ville.Nom}");
            }

            Console.Write("\nVille de départ : ");
            string depart = Console.ReadLine();
            Console.Write("Ville d'arrivée : ");
            string arrivee = Console.ReadLine();

            Ville villeDepart = villes.FirstOrDefault(v => v.Nom.Equals(depart, StringComparison.OrdinalIgnoreCase));
            Ville villeArrivee = villes.FirstOrDefault(v => v.Nom.Equals(arrivee, StringComparison.OrdinalIgnoreCase));

            if (villeDepart == null || villeArrivee == null)
            {
                Console.WriteLine("\nUne ou plusieurs villes n'ont pas été trouvées.");
                Console.WriteLine("Vérifiez l'orthographe et respectez la casse.");
                Console.ReadKey();
                return;
            }

            // Initialisation de la visualisation
            GrapheVisualisation visualisation = new GrapheVisualisation(this.grapheListe);

            // Comparaison des algorithmes
            Console.WriteLine("\n--- Comparaison des algorithmes ---\n");

            // Dijkstra
            Stopwatch chronoDijkstra = Stopwatch.StartNew();
            (List<Ville> cheminDijkstra, double distanceDijkstra) = this.grapheServiceListe.Dijkstra(villeDepart, villeArrivee);
            chronoDijkstra.Stop();
            Console.WriteLine($"Dijkstra: Distance = {distanceDijkstra:F2} km, Temps = {chronoDijkstra.Elapsed.TotalMilliseconds:F4} ms");
            if (cheminDijkstra.Any())
            {
                Console.WriteLine("Chemin trouvé : " + string.Join(" -> ", cheminDijkstra.Select(v => v.Nom)));
                visualisation.DrawPath(cheminDijkstra, "chemin_dijkstra.png", SKColors.Red);
            }

            // Bellman-Ford
            Stopwatch chronoBellmanFord = Stopwatch.StartNew();
            (List<Ville> cheminBellmanFord, double distanceBellmanFord) = this.grapheServiceListe.BellmanFord(villeDepart, villeArrivee);
            chronoBellmanFord.Stop();
            Console.WriteLine($"\nBellman-Ford: Distance = {distanceBellmanFord:F2} km, Temps = {chronoBellmanFord.Elapsed.TotalMilliseconds:F4} ms");
            if (cheminBellmanFord.Any())
            {
                Console.WriteLine("Chemin trouvé : " + string.Join(" -> ", cheminBellmanFord.Select(v => v.Nom)));
                visualisation.DrawPath(cheminBellmanFord, "chemin_bellmanford.png", SKColors.Blue);
            }

            // Floyd-Warshall
            Stopwatch chronoFloydWarshall = Stopwatch.StartNew();
            (double[,] distancesFW, int[,] predecesseursFW)? resultatFloydWarshall = this.grapheServiceListe.FloydWarshall();
            chronoFloydWarshall.Stop();

            if (resultatFloydWarshall.HasValue)
            {
                (double[,] distancesFW, int[,] predecesseursFW) = resultatFloydWarshall.Value;
                int indexDepart = villes.IndexOf(villeDepart);
                int indexArrivee = villes.IndexOf(villeArrivee);

                List<Ville> cheminFloydWarshall = this.grapheServiceListe.ReconstruireCheminFloydWarshall(indexDepart, indexArrivee, distancesFW, predecesseursFW, villes);
                double distanceFloydWarshall = distancesFW[indexDepart, indexArrivee];

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

        /// <summary>
        /// Vérifie et affiche si le graphe est connexe.
        /// </summary>
        private void VerifierConnexite()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification de la Connexité");

            ChargerDonneesGraphe();

            bool connexe = this.grapheServiceListe.EstConnexe();
            Console.WriteLine($"Le graphe est{(connexe ? "" : " non")} connexe.");
            if (!connexe)
            {
                Console.WriteLine("\nCela signifie qu'il existe des villes qui ne sont pas accessibles depuis certaines autres villes.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// <summary>
        /// Vérifie et affiche si le graphe contient des cycles.
        /// </summary>
        private void VerifierCycles()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Vérification des Cycles");

            ChargerDonneesGraphe();

            bool cycle = this.grapheServiceListe.ContientCycle();
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
