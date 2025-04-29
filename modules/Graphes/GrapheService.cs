using System;
using System.Collections.Generic;
using System.Diagnostics; 
using System.IO;
using System.Linq;
using System.Text; 
using OfficeOpenXml;

namespace Projet.Modules
{
    public class GrapheService
    {
        private readonly Graphe _graphe;

        public GrapheService(Graphe graphe)
        {
            _graphe = graphe ?? throw new ArgumentNullException(nameof(graphe));
        }

        public void ChargerGrapheDepuisXlsx(string cheminFichier)
        {

            FileInfo fichierInfo = new FileInfo(cheminFichier);

            if (!fichierInfo.Exists)
            {
                Console.WriteLine($"Erreur: Le fichier Excel '{cheminFichier}' n'a pas été trouvé.");
                return;
            }

            try
            {
                using (var package = new ExcelPackage(fichierInfo))
                {
                    // On suppose que les données sont dans la première feuille de calcul
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        Console.WriteLine($"Erreur: Le fichier Excel '{cheminFichier}' ne contient aucune feuille de calcul.");
                        return;
                    }

                    // Vérifier si la feuille contient des données
                    if (worksheet.Dimension == null)
                    {
                        Console.WriteLine($"Avertissement: La feuille de calcul '{worksheet.Name}' dans '{cheminFichier}' est vide.");
                        return;
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow + 1; row <= endRow; row++)
                    {
                        string nomVilleA = worksheet.Cells[row, 1].Text?.Trim();
                        string nomVilleB = worksheet.Cells[row, 2].Text?.Trim();
                        object valeurDistanceObj = worksheet.Cells[row, 3].Value; 

                        // Ignorer les lignes où les noms de ville sont manquants
                        if (string.IsNullOrWhiteSpace(nomVilleA) && string.IsNullOrWhiteSpace(nomVilleB))
                        {
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(nomVilleA) || string.IsNullOrWhiteSpace(nomVilleB))
                        {
                            Console.WriteLine($"Avertissement: Nom de ville manquant à la ligne {row} dans {cheminFichier}. Ligne ignorée.");
                            continue;
                        }


                        double distance;
                        bool parseOk = false;

                        if (valeurDistanceObj is double d) 
                        {
                            distance = d;
                            parseOk = true;
                        }
                        else if (valeurDistanceObj != null) 
                        {
                            parseOk = double.TryParse(valeurDistanceObj.ToString()?.Trim(),
                                                      System.Globalization.NumberStyles.Any, 
                                                      System.Globalization.CultureInfo.InvariantCulture, 
                                                      out distance);
                        }
                        else
                        {
                            Console.WriteLine($"Avertissement: La cellule de distance est vide à la ligne {row} pour le lien {nomVilleA}-{nomVilleB} dans {cheminFichier}. Ligne ignorée.");
                            continue; 
                        }


                        if (parseOk)
                        {
                            if (distance >= 0)
                            {
                                Ville villeA = new Ville(nomVilleA);
                                Ville villeB = new Ville(nomVilleB);
                                _graphe.AjouterLien(villeA, villeB, distance);
                            }
                            else
                            {
                                Console.WriteLine($"Avertissement: Distance négative ({distance}) ignorée pour le lien {nomVilleA}-{nomVilleB} à la ligne {row} dans {cheminFichier}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Avertissement: Impossible de parser la distance '{valeurDistanceObj}' pour le lien {nomVilleA}-{nomVilleB} à la ligne {row} dans {cheminFichier}.");
                        }
                    }
                } 

                Console.WriteLine($"Graphe chargé depuis {cheminFichier}. Nombre de villes: {_graphe.GetToutesLesVilles().Count()}.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erreur d'entrée/sortie lors de la lecture de {cheminFichier}: {ex.Message}");
            }
            catch (InvalidDataException ex) 
            {
                Console.WriteLine($"Erreur: Le fichier Excel '{cheminFichier}' semble corrompu ou n'est pas un format XLSX valide. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inattendue lors du chargement du graphe depuis {cheminFichier}: {ex.Message}");
               
            }
        }

        /// <summary>
        /// Effectue un parcours en largeur (BFS) à partir d'une ville de départ.
        /// </summary>
        /// <returns>La liste des villes dans l'ordre de visite.</returns>
        public List<Ville> BFS(Ville depart)
        {
            if (depart == null || !_graphe.ContientVille(depart)) return new List<Ville>();

            var visites = new List<Ville>();
            var fileAttente = new Queue<Ville>();
            var visitesSet = new HashSet<Ville>(); 

            fileAttente.Enqueue(depart);
            visitesSet.Add(depart);

            while (fileAttente.Count > 0)
            {
                Ville courant = fileAttente.Dequeue();
                visites.Add(courant);

                foreach (var (voisin, poids) in _graphe.ObtenirVoisins(courant))
                {
                    if (!visitesSet.Contains(voisin))
                    {
                        visitesSet.Add(voisin);
                        fileAttente.Enqueue(voisin);
                    }
                }
            }
            return visites;
        }

        /// <summary>
        /// Effectue un parcours en profondeur (DFS) récursif à partir d'une ville de départ.
        /// </summary>
        /// <returns>La liste des villes dans l'ordre de visite.</returns>
        public List<Ville> DFS(Ville depart)
        {
            if (depart == null || !_graphe.ContientVille(depart)) return new List<Ville>();

            var visites = new List<Ville>();
            var visitesSet = new HashSet<Ville>(); 
            DFSRecursif(depart, visites, visitesSet);
            return visites;
        }

        private void DFSRecursif(Ville courant, List<Ville> visites, HashSet<Ville> visitesSet)
        {
            visites.Add(courant);
            visitesSet.Add(courant);

            foreach (var (voisin, poids) in _graphe.ObtenirVoisins(courant))
            {
                if (!visitesSet.Contains(voisin))
                {
                    DFSRecursif(voisin, visites, visitesSet);
                }
            }
        }

        /// <summary>
        /// Calcule le plus court chemin entre deux villes en utilisant l'algorithme de Dijkstra.
        /// </summary>
        /// <returns>Un tuple contenant la liste des villes formant le chemin et la distance totale.
        /// Retourne une liste vide et distance infinie si aucun chemin n'est trouvé.</returns>
        public (List<Ville> chemin, double distanceTotale) Dijkstra(Ville depart, Ville arrivee)
        {
            if (depart == null || arrivee == null || !_graphe.ContientVille(depart) || !_graphe.ContientVille(arrivee))
                return (new List<Ville>(), double.PositiveInfinity);

            if (depart.Equals(arrivee))
                return (new List<Ville> { depart }, 0);

            Dictionary<Ville, double> distances = new Dictionary<Ville, double>();
            Dictionary<Ville, Ville> predecesseurs = new Dictionary<Ville, Ville>();
            List<Ville> nonVisites = new List<Ville>();

            foreach (Ville ville in _graphe.GetToutesLesVilles())
            {
                distances[ville] = ville.Equals(depart) ? 0 : double.PositiveInfinity;
                nonVisites.Add(ville);
            }

            while (nonVisites.Count > 0)
            {
                Ville villeCourante = TrouverVilleMinimale(nonVisites, distances);

                if (villeCourante == null || double.IsPositiveInfinity(distances[villeCourante]))
                    break;

                if (villeCourante.Equals(arrivee))
                    break;

                nonVisites.Remove(villeCourante);

                foreach ((Ville voisin, double poids) in _graphe.ObtenirVoisins(villeCourante))
                {
                    if (!nonVisites.Contains(voisin))
                        continue;

                    double nouvelleDistance = distances[villeCourante] + poids;
                    if (nouvelleDistance < distances[voisin])
                    {
                        distances[voisin] = nouvelleDistance;
                        predecesseurs[voisin] = villeCourante;
                    }
                }
            }

            if (!predecesseurs.ContainsKey(arrivee) && !depart.Equals(arrivee))
                return (new List<Ville>(), double.PositiveInfinity);

            List<Ville> chemin = new List<Ville>();
            for (Ville ville = arrivee; ville != null; ville = predecesseurs.TryGetValue(ville, out Ville pred) ? pred : null)
            {
                chemin.Add(ville);
                if (ville.Equals(depart))
                    break;
            }

            chemin.Reverse();

            return (chemin, distances[arrivee]);
        }

        private Ville TrouverVilleMinimale(List<Ville> nonVisites, Dictionary<Ville, double> distances)
        {
            double distanceMin = double.PositiveInfinity;
            Ville villeMin = null;

            foreach (Ville ville in nonVisites)
            {
                if (distances[ville] < distanceMin)
                {
                    distanceMin = distances[ville];
                    villeMin = ville;
                }
            }

            return villeMin;
        }

        /// <summary>
        /// Calcule le plus court chemin en utilisant Bellman-Ford. Capable de gérer les poids négatifs.
        /// Utile pour comparaison, bien que les distances réelles soient positives.
        /// </summary>
        /// <returns>Tuple (chemin, distance).</returns>
        public (List<Ville> chemin, double distanceTotale) BellmanFord(Ville depart, Ville arrivee)
        {
            if (depart == null || arrivee == null || !_graphe.ContientVille(depart) || !_graphe.ContientVille(arrivee))
                return (new List<Ville>(), double.PositiveInfinity);

            Dictionary<Ville, double> distances = new Dictionary<Ville, double>();
            Dictionary<Ville, Ville> predecesseurs = new Dictionary<Ville, Ville>();
            List<Ville> toutesLesVilles = _graphe.GetToutesLesVilles().ToList();
            List<Lien> tousLesLiens = new List<Lien>(); 

            foreach (Ville ville in toutesLesVilles)
            {
                distances[ville] = double.PositiveInfinity;
                predecesseurs[ville] = null;
                foreach ((Ville voisin, double poids) in _graphe.ObtenirVoisins(ville))
                {
                    tousLesLiens.Add(new Lien(ville, voisin, poids));
                }
            }
            distances[depart] = 0;

            int nbVilles = toutesLesVilles.Count;

            for (int i = 1; i < nbVilles; i++)
            {
                foreach (Lien lien in tousLesLiens)
                {
                    if (distances[lien.Origine] != double.PositiveInfinity &&
                        distances[lien.Origine] + lien.Poids < distances[lien.Destination])
                    {
                        distances[lien.Destination] = distances[lien.Origine] + lien.Poids;
                        predecesseurs[lien.Destination] = lien.Origine;
                    }
                }
            }

            if (!predecesseurs.ContainsKey(arrivee) && !depart.Equals(arrivee))
            {
                if (depart.Equals(arrivee) && distances[arrivee] == 0) return (new List<Ville> { depart }, 0);
                return (new List<Ville>(), double.PositiveInfinity);
            }

            List<Ville> chemin = new List<Ville>();
            Ville etape = arrivee;
            while (etape != null)
            {
                chemin.Add(etape);
                if (etape.Equals(depart)) break;
                if (!predecesseurs.TryGetValue(etape, out etape))
                {
                    if (!etape.Equals(depart))
                    {
                        return (new List<Ville>(), double.PositiveInfinity); 
                    }
                    etape = null;
                }
            }
            chemin.Reverse();

            if (chemin.Count == 0 || !chemin.First().Equals(depart) || !chemin.Last().Equals(arrivee))
            {
                if (depart.Equals(arrivee) && distances[arrivee] == 0) return (new List<Ville> { depart }, 0);
                return (new List<Ville>(), double.PositiveInfinity);
            }

            return (chemin, distances[arrivee]);
        }


        /// <summary>
        /// Calcule les plus courts chemins entre toutes les paires de villes en utilisant Floyd-Warshall.
        /// </summary>
        /// <returns>Un tuple contenant deux matrices:
        ///          1. distances[i, j] = distance la plus courte de la ville i à la ville j.
        ///          2. predecesseurs[i, j] = index du prédécesseur sur le chemin le plus court de i à j.
        ///          Retourne null si le graphe n'est pas compatible ou en cas d'erreur.</returns>
        public (double[,] distances, int[,] predecesseurs)? FloydWarshall()
        {
            List<Ville> villesList;
            double[,] matriceDistances;
            int nbVilles;

            if (_graphe is GrapheMatrice grapheMatrice)
            {
                matriceDistances = grapheMatrice.GetMatriceAdjacence(); 
                villesList = grapheMatrice.GetVilleList();
                nbVilles = villesList.Count;
            }
            else
            {
                Console.WriteLine("Conversion de GrapheListe en matrice pour Floyd-Warshall...");
                villesList = _graphe.GetToutesLesVilles().ToList();
                nbVilles = villesList.Count;
                if (nbVilles == 0) return null;

                matriceDistances = new double[nbVilles, nbVilles];
                Dictionary<Ville, int> villeIndexMap = new Dictionary<Ville, int>();
                for (int i = 0; i < nbVilles; ++i) villeIndexMap[villesList[i]] = i;

                for (int i = 0; i < nbVilles; i++)
                {
                    for (int j = 0; j < nbVilles; j++)
                    {
                        if (i == j) matriceDistances[i, j] = 0;
                        else matriceDistances[i, j] = _graphe.ObtenirPoidsLien(villesList[i], villesList[j]);
                    }
                }
            }

            int[,] matricePredecesseurs = new int[nbVilles, nbVilles];

            for (int i = 0; i < nbVilles; i++)
            {
                for (int j = 0; j < nbVilles; j++)
                {
                    if (i == j || double.IsPositiveInfinity(matriceDistances[i, j]))
                    {
                        matricePredecesseurs[i, j] = -1;
                    }
                    else
                    {
                        matricePredecesseurs[i, j] = i; 
                    }
                }
            }

            for (int k = 0; k < nbVilles; k++) 
            {
                for (int i = 0; i < nbVilles; i++)
                {
                    for (int j = 0; j < nbVilles; j++)
                    {
                        if (matriceDistances[i, k] != double.PositiveInfinity &&
                            matriceDistances[k, j] != double.PositiveInfinity &&
                            matriceDistances[i, k] + matriceDistances[k, j] < matriceDistances[i, j])
                        {
                            matriceDistances[i, j] = matriceDistances[i, k] + matriceDistances[k, j];
                            matricePredecesseurs[i, j] = matricePredecesseurs[k, j]; 
                        }
                    }
                }
            }

            return (matriceDistances, matricePredecesseurs);
        }


        /// <summary>
        /// Reconstruit le chemin le plus court entre deux villes à partir des résultats de Floyd-Warshall.
        /// </summary>
        /// <returns>La liste des villes formant le chemin, ou une liste vide si aucun chemin n'existe.</returns>
        public List<Ville> ReconstruireCheminFloydWarshall(int indexDepart, int indexArrivee, double[,] distances, int[,] predecesseurs, List<Ville> villesList)
        {
            if (distances == null || predecesseurs == null || villesList == null ||
                indexDepart < 0 || indexDepart >= villesList.Count ||
                indexArrivee < 0 || indexArrivee >= villesList.Count)
            {
                return new List<Ville>(); 
            }

            if (double.IsPositiveInfinity(distances[indexDepart, indexArrivee]))
            {
                return new List<Ville>(); 
            }

            List<Ville> chemin = new List<Ville>();
            if (indexDepart == indexArrivee)
            {
                chemin.Add(villesList[indexDepart]);
                return chemin;
            }

            Stack<int> pileIndices = new Stack<int>();
            int courant = indexArrivee;
            while (courant != -1 && courant != indexDepart)
            {
                pileIndices.Push(courant);
                courant = predecesseurs[indexDepart, courant];
                if (pileIndices.Count > villesList.Count)
                {
                    Console.WriteLine("Erreur: Boucle détectée dans la reconstruction du chemin Floyd-Warshall.");
                    return new List<Ville>();
                }
            }

            if (courant == -1 && indexDepart != indexArrivee) 
            {
                Console.WriteLine("Erreur: Impossible de reconstruire le chemin complet (prédécesseur manquant).");
                return new List<Ville>();
            }


            chemin.Add(villesList[indexDepart]);
            while (pileIndices.Count > 0)
            {
                chemin.Add(villesList[pileIndices.Pop()]);
            }

            if (chemin.Count > 0 && (!chemin.First().Equals(villesList[indexDepart]) || !chemin.Last().Equals(villesList[indexArrivee])))
            {
                Console.WriteLine("Erreur: Le chemin reconstruit ne correspond pas aux départ/arrivée attendus.");
                return new List<Ville>();
            }


            return chemin;
        }


        /// <summary>
        /// Vérifie si le graphe est connexe (pour un graphe non orienté).
        /// Utilise un parcours (BFS ou DFS) pour voir si toutes les villes sont atteignables depuis un point de départ.
        /// </summary>
        /// <returns>True si le graphe est connexe, False sinon.</returns>
        public bool EstConnexe()
        {
            List<Ville> toutesLesVilles = _graphe.GetToutesLesVilles().ToList();
            if (!toutesLesVilles.Any()) return true;

            Ville depart = toutesLesVilles.First();
            List<Ville> visites = BFS(depart); // Ou DFS(depart)

            return visites.Count == toutesLesVilles.Count;
        }

        /// <summary>
        /// Détecte la présence de cycles dans le graphe en utilisant DFS.
        /// </summary>
        /// <returns>True si au moins un cycle est détecté, False sinon.</returns>
        public bool ContientCycle()
        {
            IEnumerable<Ville> toutesLesVilles = _graphe.GetToutesLesVilles();
            if (!toutesLesVilles.Any()) return false;

            HashSet<Ville> visites = new HashSet<Ville>();
            HashSet<Ville> enCoursExploration = new HashSet<Ville>(); 

            foreach (Ville ville in toutesLesVilles)
            {
                if (!visites.Contains(ville))
                {
                    if (ContientCycleUtil(ville, visites, enCoursExploration, null))
                    {
                        return true;
                    }
                }
            }
            return false; 
        }

        private bool ContientCycleUtil(Ville courant, HashSet<Ville> visites, HashSet<Ville> enCoursExploration, Ville parent)
        {
            visites.Add(courant);
            enCoursExploration.Add(courant);

            foreach ((Ville voisin, double poids) in _graphe.ObtenirVoisins(courant))
            {
                if (enCoursExploration.Contains(voisin))
                {
                    if (_graphe.EstNonOriente && voisin.Equals(parent))
                    {
                        continue; 
                    }
                    Console.WriteLine($"Cycle détecté impliquant: {courant} -> {voisin}");
                    return true; 
                }

                if (!visites.Contains(voisin))
                {
                    if (ContientCycleUtil(voisin, visites, enCoursExploration, courant))
                    {
                        return true; 
                    }
                }
            }

            enCoursExploration.Remove(courant); 
            return false; 
        }


        /// <summary>
        /// Compare les temps d'exécution de Dijkstra sur deux implémentations de graphe (si possible).
        /// Affiche les résultats dans la console.
        /// </summary>
        public void ComparerImplementationsDijkstra(Ville depart, Ville arrivee, Graphe autreGraphe)
        {
            if (autreGraphe == null)
            {
                Console.WriteLine("Impossible de comparer: le second graphe est null.");
                return;
            }

            Console.WriteLine($"\n--- Comparaison d'exécution Dijkstra ({depart} -> {arrivee}) ---");

            var chrono1 = Stopwatch.StartNew();
            (List<Ville> chemin1, double dist1) = this.Dijkstra(depart, arrivee);
            chrono1.Stop();
            Console.WriteLine($"Graphe actuel ({_graphe.GetType().Name}):");
            AfficherResultatChemin(chemin1, dist1, chrono1.Elapsed);

            GrapheService serviceAutreGraphe = new GrapheService(autreGraphe);
            var chrono2 = Stopwatch.StartNew();
            (List<Ville>chemin2, double dist2) = serviceAutreGraphe.Dijkstra(depart, arrivee);
            chrono2.Stop();
            Console.WriteLine($"\nAutre Graphe ({autreGraphe.GetType().Name}):");
            AfficherResultatChemin(chemin2, dist2, chrono2.Elapsed);

            Console.WriteLine("--- Fin Comparaison ---");
        }

        public static void AfficherResultatChemin(List<Ville> chemin, double distance, TimeSpan? tempsExecution = null)
        {
            if (chemin != null && chemin.Any())
            {
                Console.WriteLine($"  Chemin trouvé ({chemin.Count} villes): {string.Join(" -> ", chemin.Select(v => v.Nom))}");
                Console.WriteLine($"  Distance totale: {distance:F2} km");
            }
            else
            {
                Console.WriteLine("  Aucun chemin trouvé.");
                Console.WriteLine($"  Distance: {distance}"); 
            }
            if (tempsExecution.HasValue)
            {
                Console.WriteLine($"  Temps d'exécution: {tempsExecution.Value.TotalMilliseconds:F4} ms");
            }
        }
    }
}