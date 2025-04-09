using modules.Graphes;
using System;
using System.Collections.Generic;
using System.Diagnostics; 
using System.IO;
using System.Linq;
using System.Text; 

namespace modules.Graphes 
{
    public class GrapheService
    {
        private readonly Graphe _graphe;

        public GrapheService(Graphe graphe)
        {
            _graphe = graphe ?? throw new ArgumentNullException(nameof(graphe));
        }

        public void ChargerGrapheDepuisCsv(string cheminFichier)
        {
            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine($"Erreur: Le fichier CSV '{cheminFichier}' n'a pas été trouvé.");
                return;
            }

            try
            {
                var lignes = File.ReadAllLines(cheminFichier);
                foreach (var ligne in lignes.Skip(1)) 
                {
                    if (string.IsNullOrWhiteSpace(ligne)) continue;

                    var elements = ligne.Split(';');
                    if (elements.Length >= 3)
                    {
                        string nomVilleA = elements[0].Trim();
                        string nomVilleB = elements[1].Trim();
                        if (double.TryParse(elements[2].Trim(), out double distance))
                        {
                            if (distance >= 0)
                            {
                                Ville villeA = new Ville(nomVilleA);
                                Ville villeB = new Ville(nomVilleB);
                                _graphe.AjouterLien(villeA, villeB, distance);
                            }
                            else
                            {
                                Console.WriteLine($"Avertissement: Distance négative ignorée pour le lien {nomVilleA}-{nomVilleB} dans {cheminFichier}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Avertissement: Impossible de parser la distance '{elements[2]}' pour le lien {nomVilleA}-{nomVilleB} dans {cheminFichier}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Avertissement: Ligne mal formatée ignorée dans {cheminFichier}: {ligne}");
                    }
                }
                Console.WriteLine($"Graphe chargé depuis {cheminFichier}. Nombre de villes: {_graphe.GetToutesLesVilles().Count()}.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erreur d'entrée/sortie lors de la lecture de {cheminFichier}: {ex.Message}");
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

            var distances = new Dictionary<Ville, double>();
            var predecesseurs = new Dictionary<Ville, Ville>();
            var filePriorite = new SortedSet<(double distance, Ville ville)>(Comparer<(double distance, Ville ville)>.Create(
                (a, b) => {
                    int distCompare = a.distance.CompareTo(b.distance);
                    if (distCompare == 0)
                        return Comparer<Ville>.Default.Compare(a.ville, b.ville); 
                    return distCompare;
                }
            ));


            foreach (var ville in _graphe.GetToutesLesVilles())
            {
                distances[ville] = double.PositiveInfinity;
            }
            distances[depart] = 0;
            filePriorite.Add((0, depart));

            while (filePriorite.Count > 0)
            {
                var (distCourante, villeCourante) = filePriorite.Min; 
                filePriorite.Remove(filePriorite.Min);

                if (villeCourante.Equals(arrivee)) break; 
                if (double.IsPositiveInfinity(distCourante)) break; 

                foreach (var (voisin, poids) in _graphe.ObtenirVoisins(villeCourante))
                {
                    double nouvelleDistance = distCourante + poids;
                    if (nouvelleDistance < distances[voisin])
                    {
                        filePriorite.Remove((distances[voisin], voisin));

                        distances[voisin] = nouvelleDistance;
                        predecesseurs[voisin] = villeCourante;

                        filePriorite.Add((nouvelleDistance, voisin));
                    }
                }
            }

            if (!predecesseurs.ContainsKey(arrivee) && !depart.Equals(arrivee))
            {
                return (new List<Ville>(), double.PositiveInfinity); 
            }

            var chemin = new List<Ville>();
            Ville etape = arrivee;
            while (etape != null)
            {
                chemin.Add(etape);
                if (etape.Equals(depart)) break; 
                if (!predecesseurs.TryGetValue(etape, out etape))
                {
                    if (!etape.Equals(depart))
                    {
                        Console.WriteLine($"Erreur de reconstruction de chemin à l'étape {chemin.LastOrDefault()}");
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
        /// Calcule le plus court chemin en utilisant Bellman-Ford. Gère les poids négatifs (mais pas les cycles négatifs).
        /// Utile pour comparaison, bien que les distances réelles soient positives.
        /// </summary>
        /// <returns>Tuple (chemin, distance). Détecte et signale les cycles négatifs.</returns>
        public (List<Ville> chemin, double distanceTotale, bool cycleNegatifDetecte) BellmanFord(Ville depart, Ville arrivee)
        {
            if (depart == null || arrivee == null || !_graphe.ContientVille(depart) || !_graphe.ContientVille(arrivee))
                return (new List<Ville>(), double.PositiveInfinity, false);

            var distances = new Dictionary<Ville, double>();
            var predecesseurs = new Dictionary<Ville, Ville>();
            var toutesLesVilles = _graphe.GetToutesLesVilles().ToList();
            var tousLesLiens = new List<Lien>(); 

            foreach (var ville in toutesLesVilles)
            {
                distances[ville] = double.PositiveInfinity;
                predecesseurs[ville] = null;
                // Collecter tous les liens
                foreach (var (voisin, poids) in _graphe.ObtenirVoisins(ville))
                {
                    tousLesLiens.Add(new Lien(ville, voisin, poids));
                }
            }
            distances[depart] = 0;

            int nbVilles = toutesLesVilles.Count;

            for (int i = 1; i < nbVilles; i++)
            {
                foreach (var lien in tousLesLiens)
                {
                    if (distances[lien.Origine] != double.PositiveInfinity &&
                        distances[lien.Origine] + lien.Poids < distances[lien.Destination])
                    {
                        distances[lien.Destination] = distances[lien.Origine] + lien.Poids;
                        predecesseurs[lien.Destination] = lien.Origine;
                    }
                }
            }

            bool cycleNegatifDetecte = false;
            foreach (var lien in tousLesLiens)
            {
                if (distances[lien.Origine] != double.PositiveInfinity &&
                    distances[lien.Origine] + lien.Poids < distances[lien.Destination])
                {
                    cycleNegatifDetecte = true;
                    Console.WriteLine($"Cycle négatif détecté affectant le lien {lien}");
                    break; 
                }
            }

            if (cycleNegatifDetecte)
            {
                // Si un cycle négatif est détecté, le concept de "plus court chemin" peut être invalide
                // On retourne quand même le chemin trouvé, mais avec un avertissement.
                // Ou on pourrait retourner un chemin vide pour indiquer l'impossibilité.
                Console.WriteLine("Avertissement: Cycle négatif détecté, le chemin le plus court peut ne pas être fiable.");
                return (new List<Ville>(), double.NegativeInfinity, true); // Option alternative
            }

            if (!predecesseurs.ContainsKey(arrivee) && !depart.Equals(arrivee))
            {
                if (depart.Equals(arrivee) && distances[arrivee] == 0) return (new List<Ville> { depart }, 0, cycleNegatifDetecte);
                return (new List<Ville>(), double.PositiveInfinity, cycleNegatifDetecte);
            }

            var chemin = new List<Ville>();
            Ville etape = arrivee;
            while (etape != null)
            {
                chemin.Add(etape);
                if (etape.Equals(depart)) break;
                if (!predecesseurs.TryGetValue(etape, out etape))
                {
                    if (!etape.Equals(depart))
                    {
                        return (new List<Ville>(), double.PositiveInfinity, cycleNegatifDetecte); 
                    }
                    etape = null;
                }
            }
            chemin.Reverse();

            if (chemin.Count == 0 || !chemin.First().Equals(depart) || !chemin.Last().Equals(arrivee))
            {
                if (depart.Equals(arrivee) && distances[arrivee] == 0) return (new List<Ville> { depart }, 0, cycleNegatifDetecte);
                return (new List<Ville>(), double.PositiveInfinity, cycleNegatifDetecte);
            }

            return (chemin, distances[arrivee], cycleNegatifDetecte);
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
                var villeIndexMap = new Dictionary<Ville, int>();
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

            // Optionnel: Détection de cycles négatifs (si diagonale devient < 0)
            for (int i = 0; i < nbVilles; i++)
            {
                if (matriceDistances[i, i] < 0)
                {
                    Console.WriteLine("Avertissement: Cycle négatif détecté par Floyd-Warshall.");
                    // Gérer comme nécessaire (retourner null, lancer une exception, etc.)
                    // return null;
                    break;
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
            var toutesLesVilles = _graphe.GetToutesLesVilles().ToList();
            if (!toutesLesVilles.Any()) return true;

            Ville depart = toutesLesVilles.First();
            var visites = BFS(depart); // Ou DFS(depart)

            return visites.Count == toutesLesVilles.Count;
        }

        /// <summary>
        /// Détecte la présence de cycles dans le graphe en utilisant DFS.
        /// </summary>
        /// <returns>True si au moins un cycle est détecté, False sinon.</returns>
        public bool ContientCycle()
        {
            var toutesLesVilles = _graphe.GetToutesLesVilles();
            if (!toutesLesVilles.Any()) return false;

            var visites = new HashSet<Ville>();     
            var enCoursExploration = new HashSet<Ville>(); 

            foreach (var ville in toutesLesVilles)
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

            foreach (var (voisin, poids) in _graphe.ObtenirVoisins(courant))
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
            var (chemin1, dist1) = this.Dijkstra(depart, arrivee);
            chrono1.Stop();
            Console.WriteLine($"Graphe actuel ({_graphe.GetType().Name}):");
            AfficherResultatChemin(chemin1, dist1, chrono1.Elapsed);

            var serviceAutreGraphe = new GrapheService(autreGraphe);
            var chrono2 = Stopwatch.StartNew();
            var (chemin2, dist2) = serviceAutreGraphe.Dijkstra(depart, arrivee);
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