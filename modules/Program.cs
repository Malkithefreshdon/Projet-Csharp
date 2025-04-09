using modules.Clients;
using modules.Commandes;
using modules.Graphes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using modules.Salariés;

class Program
{
    // Managers and Services
    static SalarieManager salarieManager;
    static CommandeManager commandeManager;
    static GrapheService grapheServiceListe;
    static GrapheService grapheServiceMatrice;
    static GrapheListe grapheListe;
    static GrapheMatrice grapheMatrice;

    // Placeholder data
    static List<Client> clients; // Simple list for testing

    static void Main(string[] args)
    {
        Console.WriteLine("Démarrage de l'application TransConnect de test...");

        // 1. Initialisation
        InitialiserSysteme();

        // 2. Création de données initiales si nécessaire (si fichiers vides/non existants)
        CreerDonneesInitialesSiVide();

        // 3. Tests Module Salarié / Organigramme
        TesterModuleSalarie();

        // 4. Tests Module Graphe
        TesterModuleGraphe();

        // 5. Tests Module Commande
        TesterModuleCommande();

        // 6. Sauvegarde finale
        SauvegardeFinale();

        Console.WriteLine("\nFin des tests. Appuyez sur Entrée pour quitter.");
        Console.ReadLine();
    }

    static void InitialiserSysteme()
    {
        Console.WriteLine("\n--- Initialisation ---");
        // Initialiser les managers (ils tenteront de charger leurs fichiers)
        salarieManager = new SalarieManager(); // Tente de charger salaries.json
        commandeManager = new CommandeManager(); // Tente de charger commandes.json

        // Initialiser les graphes
        grapheListe = new GrapheListe(estNonOriente: true);
        grapheMatrice = new GrapheMatrice(estNonOriente: true);

        // Initialiser les services de graphe
        grapheServiceListe = new GrapheService(grapheListe);
        grapheServiceMatrice = new GrapheService(grapheMatrice);

        // Charger les données du graphe depuis CSV
        CreerFichierDistancesSiAbsent(); // Crée un CSV de démo si besoin
        grapheServiceListe.ChargerGrapheDepuisCsv("Distances.csv");
        grapheServiceMatrice.ChargerGrapheDepuisCsv("Distances.csv"); // Charger dans les deux pour comparaison

        // Placeholder pour les clients
        clients = new List<Client>
        {
            new Client(1, "Dupont", "Jean"),
            new Client(2, "Martin", "Sophie")
        };
        Console.WriteLine($"{clients.Count} clients de test créés.");
    }

    static void CreerFichierDistancesSiAbsent()
    {
        if (!File.Exists("Distances.csv"))
        {
            Console.WriteLine("Création du fichier Distances.csv de démonstration...");
            string csvContent = "VilleA;VilleB;Distance\n" +
                                "Paris;Lyon;465\n" +
                                "Lyon;Marseille;315\n" +
                                "Paris;Lille;225\n" +
                                "Lille;Lyon;690\n" + // Via Paris? Direct? Mettre une valeur plausible
                                "Paris;Nantes;385\n" +
                                "Nantes;Bordeaux;345\n" +
                                "Lyon;Bordeaux;550\n" +
                                "Paris;Strasbourg;490";
            File.WriteAllText("Distances.csv", csvContent);
        }
    }


    static void CreerDonneesInitialesSiVide()
    {
        Console.WriteLine("\n--- Vérification Données Initiales ---");
        // Si aucun salarié n'a été chargé (fichier inexistant ou vide)
        if (!salarieManager.GetTousLesSalaries().Any())
        {
            Console.WriteLine("Aucun salarié chargé. Création de données initiales...");
            try
            {
                // Créer la hiérarchie basée sur le PDF
                var dg = new Responsable("SS001", "Dupond", "Gérard", new DateTime(1970, 1, 1), new DateTime(2010, 5, 1)) { Salaire = 80000, AdresseMail = "dg@trans.co" };
                salarieManager.AjouterSalarie(dg); // La racine est ajoutée en premier

                var drh = new Responsable("SS002", "Loyeuse", "Mme", new DateTime(1975, 3, 10), new DateTime(2012, 1, 15)) { Salaire = 60000 };
                salarieManager.AjouterSalarie(drh, dg.NumeroSecuriteSociale);

                var dop = new Responsable("SS003", "Fetard", "Mr", new DateTime(1980, 6, 20), new DateTime(2011, 9, 1)) { Salaire = 65000 };
                salarieManager.AjouterSalarie(dop, dg.NumeroSecuriteSociale);

                var chef1 = new Responsable("SS004", "Royal", "Mr", new DateTime(1985, 9, 5), new DateTime(2015, 3, 1)) { Salaire = 45000 };
                salarieManager.AjouterSalarie(chef1, dop.NumeroSecuriteSociale);

                var chef2 = new Responsable("SS005", "Prince", "Mme", new DateTime(1988, 11, 12), new DateTime(2016, 7, 10)) { Salaire = 46000 };
                salarieManager.AjouterSalarie(chef2, dop.NumeroSecuriteSociale);

                var chauf1 = new Chauffeur("SS101", "Romu", "Mr", new DateTime(1990, 2, 15), new DateTime(2018, 1, 20)) { Salaire = 28000 };
                salarieManager.AjouterSalarie(chauf1, chef1.NumeroSecuriteSociale);

                var chauf2 = new Chauffeur("SS102", "Rome", "Mme", new DateTime(1992, 4, 25), new DateTime(2019, 5, 5)) { Salaire = 27500 };
                salarieManager.AjouterSalarie(chauf2, chef1.NumeroSecuriteSociale);

                var chauf3 = new Chauffeur("SS103", "Romi", "Mme", new DateTime(1991, 8, 30), new DateTime(2017, 11, 1)) { Salaire = 29000 };
                salarieManager.AjouterSalarie(chauf3, chef2.NumeroSecuriteSociale);

                // Sauvegarder ces données initiales
                salarieManager.SauvegarderSalariesEtOrganigramme();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création des données initiales salariés : {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"{salarieManager.GetTousLesSalaries().Count} salariés chargés depuis {salarieManager.FichierSauvegarde}.");
        }

        // On pourrait faire pareil pour les commandes si nécessaire
        if (!commandeManager.GetToutesLesCommandes().Any())
        {
            Console.WriteLine("Aucune commande chargée. Le fichier est vide ou inexistant.");
            // Pas de création de commandes initiales par défaut ici, on les ajoutera dans les tests
        }
        else
        {
            Console.WriteLine($"{commandeManager.GetToutesLesCommandes().Count} commandes chargées depuis {commandeManager.FichierSauvegarde}.");
        }
    }


    static void TesterModuleSalarie()
    {
        Console.WriteLine("\n--- Test Module Salarié & Organigramme ---");

        // 1. Afficher l'organigramme chargé ou créé
        salarieManager.AfficherOrganigramme();

        // 2. Rechercher un salarié
        Console.WriteLine("\nRecherche de 'Romu' par nom:");
        var romus = salarieManager.RechercherParNom("Romu");
        if (romus.Any())
        {
            Console.WriteLine($"Trouvé: {romus.First()}");
        }
        else { Console.WriteLine("Non trouvé."); }

        Console.WriteLine("\nRecherche du salarié SS102:");
        var salarie102 = salarieManager.RechercherParId("SS102");
        if (salarie102 != null)
        {
            Console.WriteLine($"Trouvé: {salarie102}");
        }
        else { Console.WriteLine("Non trouvé."); }

        // 3. Ajouter un nouveau salarié (exemple)
        Console.WriteLine("\nAjout d'un nouveau chauffeur 'Test Driver' sous Chef Equipe 'Prince' (SS005)...");
        var nouveauChauffeur = new Chauffeur("SS999", "Driver", "Test", new DateTime(1995, 1, 1), DateTime.Now) { Salaire = 25000 };
        bool ajoutOk = salarieManager.AjouterSalarie(nouveauChauffeur, "SS005");
        if (ajoutOk) salarieManager.AfficherOrganigramme();


        // 4. Supprimer un salarié (le nouveau chauffeur)
        Console.WriteLine("\nSuppression du salarié SS999...");
        bool suppOk = salarieManager.SupprimerSalarie("SS999");
        if (suppOk) salarieManager.AfficherOrganigramme();

        // 5. Tenter de supprimer un manager (exemple: chef1 SS004) pour voir les orphelins
        Console.WriteLine("\nSuppression du manager SS004 (Royal)...");
        salarieManager.SupprimerSalarie("SS004");
        salarieManager.AfficherOrganigramme(); // Devrait montrer SS101 et SS102 sans manager affiché dans l'arbre
                                               // Vérifier l'état des salariés orphelins
        var orphelin1 = salarieManager.RechercherParId("SS101");
        var orphelin2 = salarieManager.RechercherParId("SS102");
        Console.WriteLine($"Statut Manager SS101: {orphelin1?.ManagerNumeroSS ?? "NULL"}");
        Console.WriteLine($"Statut Manager SS102: {orphelin2?.ManagerNumeroSS ?? "NULL"}");

        // Optionnel: Rajouter le manager pour la suite des tests ?
        // var chef1Recree = new Responsable("SS004", "Royal", "Mr", new DateTime(1985, 9, 5), new DateTime(2015, 3, 1)) { Salaire = 45000 };
        // salarieManager.AjouterSalarie(chef1Recree, "SS003"); // Rattacher à Fetard
        // salarieManager.AjouterSalarie(orphelin1, "SS004"); // Rattacher les orphelins - Attention, AjouterSalarie actuel ne modifie pas, il faudrait une fonction ModifierManager ou recréer les liens
        // Il faudrait une logique pour réassigner les managers proprement. Pour ce test, on laisse orphelins.

        Console.WriteLine("--- Fin Test Module Salarié ---");
    }

    static void TesterModuleGraphe()
    {
        Console.WriteLine("\n--- Test Module Graphe ---");

        // Vérifier les villes chargées
        var villes = grapheListe.GetToutesLesVilles().ToList();
        Console.WriteLine($"Nombre de villes chargées (Liste): {villes.Count}");
        // foreach(var v in villes.Take(5)) { Console.WriteLine($"- {v.Nom}"); } // Affiche quelques villes

        // Tester BFS et DFS (sur GrapheListe par exemple)
        Ville departBfsDfs = villes.FirstOrDefault(v => v.Nom.Equals("Paris", StringComparison.OrdinalIgnoreCase));
        if (departBfsDfs != null)
        {
            Console.WriteLine($"\nBFS depuis {departBfsDfs.Nom}:");
            var bfsResult = grapheServiceListe.BFS(departBfsDfs);
            Console.WriteLine(string.Join(", ", bfsResult.Select(v => v.Nom)));

            Console.WriteLine($"\nDFS depuis {departBfsDfs.Nom}:");
            var dfsResult = grapheServiceListe.DFS(departBfsDfs);
            Console.WriteLine(string.Join(", ", dfsResult.Select(v => v.Nom)));

            // Tester la connexité
            bool connexe = grapheServiceListe.EstConnexe();
            Console.WriteLine($"\nLe graphe est connexe: {connexe}");

            // Tester la détection de cycle
            bool cycle = grapheServiceListe.ContientCycle();
            Console.WriteLine($"Le graphe contient un cycle: {cycle}"); // Devrait être true pour un graphe non orienté avec >2 noeuds connectés
        }
        else { Console.WriteLine("Ville 'Paris' non trouvée pour démarrer BFS/DFS."); }


        // Tester Dijkstra
        Ville villeDepart = villes.FirstOrDefault(v => v.Nom.Equals("Paris", StringComparison.OrdinalIgnoreCase));
        Ville villeArrivee = villes.FirstOrDefault(v => v.Nom.Equals("Marseille", StringComparison.OrdinalIgnoreCase));

        if (villeDepart != null && villeArrivee != null)
        {
            Console.WriteLine($"\nCalcul du plus court chemin entre {villeDepart.Nom} et {villeArrivee.Nom} (Dijkstra):");

            Console.WriteLine("\nUtilisation de GrapheListe:");
            var chronoListe = Stopwatch.StartNew();
            var (cheminListe, distListe) = grapheServiceListe.Dijkstra(villeDepart, villeArrivee);
            chronoListe.Stop();
            GrapheService.AfficherResultatChemin(cheminListe, distListe, chronoListe.Elapsed);

            Console.WriteLine("\nUtilisation de GrapheMatrice:");
            var chronoMatrice = Stopwatch.StartNew();
            var (cheminMatrice, distMatrice) = grapheServiceMatrice.Dijkstra(villeDepart, villeArrivee);
            chronoMatrice.Stop();
            GrapheService.AfficherResultatChemin(cheminMatrice, distMatrice, chronoMatrice.Elapsed);

            // Comparaison Bellman-Ford (sur GrapheListe)
            Console.WriteLine("\nUtilisation de Bellman-Ford (GrapheListe):");
            var chronoBF = Stopwatch.StartNew();
            var (cheminBF, distBF, cycleNeg) = grapheServiceListe.BellmanFord(villeDepart, villeArrivee);
            chronoBF.Stop();
            GrapheService.AfficherResultatChemin(cheminBF, distBF, chronoBF.Elapsed);
            if (cycleNeg) Console.WriteLine("  Avertissement: Cycle négatif détecté par Bellman-Ford.");

            // Comparaison Floyd-Warshall (calcule tout) - plus long
            Console.WriteLine("\nCalcul de tous les chemins (Floyd-Warshall sur GrapheMatrice)...");
            var chronoFW = Stopwatch.StartNew();
            var resultatFW = grapheServiceMatrice.FloydWarshall(); // Utilise le service basé sur GrapheMatrice
            chronoFW.Stop();
            if (resultatFW.HasValue)
            {
                Console.WriteLine($"  Calcul Floyd-Warshall terminé en {chronoFW.Elapsed.TotalMilliseconds:F4} ms.");
                // Reconstruire et afficher le chemin Paris -> Marseille depuis FW
                var (distancesFW, predecesseursFW) = resultatFW.Value;
                var villesFW = grapheMatrice.GetVilleList(); // Obtenir la liste de villes de GrapheMatrice
                var mapIndexFW = grapheMatrice.GetVilleIndexMap(); // Obtenir map Ville->Index
                if (mapIndexFW.TryGetValue(villeDepart, out int idxDep) && mapIndexFW.TryGetValue(villeArrivee, out int idxArr))
                {
                    var cheminFW = grapheServiceMatrice.ReconstruireCheminFloydWarshall(idxDep, idxArr, distancesFW, predecesseursFW, villesFW);
                    double distFW = distancesFW[idxDep, idxArr];
                    Console.WriteLine("\nChemin Paris -> Marseille reconstruit depuis Floyd-Warshall:");
                    GrapheService.AfficherResultatChemin(cheminFW, distFW);
                }
                else { Console.WriteLine("Impossible de trouver les index pour Paris/Marseille dans la matrice FW."); }
            }
            else { Console.WriteLine("Erreur lors du calcul Floyd-Warshall."); }

        }
        else { Console.WriteLine("Villes de départ ou d'arrivée non trouvées pour le test Dijkstra."); }

        Console.WriteLine("--- Fin Test Module Graphe ---");
    }

    static void TesterModuleCommande()
    {
        Console.WriteLine("\n--- Test Module Commande ---");

        // Prérequis: Avoir des clients, des chauffeurs et des villes
        Client clientTest = clients.FirstOrDefault(); // Prend le premier client de test
                                                      // Trouver un chauffeur disponible (on prend le premier chauffeur trouvé pour ce test)
        Chauffeur chauffeurTest = salarieManager.GetTousLesSalaries()
                                    .OfType<Chauffeur>() // Filtre pour obtenir seulement les chauffeurs
                                    .FirstOrDefault();
        // Trouver des villes
        Ville villeDepartCmd = grapheListe.GetToutesLesVilles().FirstOrDefault(v => v.Nom.Equals("Paris", StringComparison.OrdinalIgnoreCase));
        Ville villeArriveeCmd = grapheListe.GetToutesLesVilles().FirstOrDefault(v => v.Nom.Equals("Lyon", StringComparison.OrdinalIgnoreCase));

        if (clientTest == null) { Console.WriteLine("Erreur: Aucun client de test trouvé."); return; }
        if (chauffeurTest == null) { Console.WriteLine("Erreur: Aucun chauffeur trouvé dans le SalarieManager."); return; }
        if (villeDepartCmd == null || villeArriveeCmd == null) { Console.WriteLine("Erreur: Villes Paris ou Lyon non trouvées dans le graphe."); return; }

        Console.WriteLine($"Utilisation du client: {clientTest}");
        Console.WriteLine($"Utilisation du chauffeur: {chauffeurTest}");
        Console.WriteLine($"Trajet demandé: {villeDepartCmd} -> {villeArriveeCmd}");

        // 1. Calculer la distance via Dijkstra (sur GrapheListe)
        var (cheminCmd, distanceCmd) = grapheServiceListe.Dijkstra(villeDepartCmd, villeArriveeCmd);

        if (double.IsPositiveInfinity(distanceCmd))
        {
            Console.WriteLine("Impossible de créer la commande: Aucun chemin trouvé entre les villes.");
            return;
        }
        Console.WriteLine($"Distance calculée: {distanceCmd:F2} km");
        Console.WriteLine($"Itinéraire: {string.Join("->", cheminCmd.Select(v => v.Nom))}");


        // 2. Calculer un prix simple (Ex: 1.5 EUR par km + ancienneté chauffeur?)
        decimal prixBaseKm = 1.5m;
        // Ajout basé sur l'ancienneté (ex: +10% si > 5 ans)
        decimal supplementAnciennete = (DateTime.Now.Year - chauffeurTest.DateEntreeSociete.Year) > 5 ? 1.1m : 1.0m;
        decimal prixCalcule = Math.Round((decimal)distanceCmd * prixBaseKm * supplementAnciennete, 2);
        Console.WriteLine($"Prix calculé: {prixCalcule:C}");


        // 3. Créer et ajouter la commande
        try
        {
            Commande nouvelleCommande = new Commande(
                client: clientTest,
                chauffeur: chauffeurTest,
                villeDepart: villeDepartCmd,
                villeArrivee: villeArriveeCmd,
                distance: distanceCmd,
                prix: prixCalcule
            );
            commandeManager.AjouterCommande(nouvelleCommande);
            Console.WriteLine($"Commande #{nouvelleCommande.Id} ajoutée."); // L'ID est assigné par le manager

            // 4. Afficher toutes les commandes
            commandeManager.AfficherToutesCommandes();

            // 5. Rechercher les commandes du client test
            Console.WriteLine($"\nRecherche des commandes pour le client {clientTest.Nom}...");
            var commandesClient = commandeManager.RechercherCommandesParClient(clientTest.Nom);
            if (commandesClient.Any())
            {
                foreach (var cmd in commandesClient) Console.WriteLine(cmd + "\n---");
            }
            else { Console.WriteLine("Aucune commande trouvée."); }

            // 6. Supprimer la commande ajoutée
            Console.WriteLine($"\nSuppression de la commande #{nouvelleCommande.Id}...");
            bool suppCmdOk = commandeManager.SupprimerCommande(nouvelleCommande.Id);
            if (suppCmdOk) Console.WriteLine("Commande supprimée."); else Console.WriteLine("Échec suppression.");

            commandeManager.AfficherToutesCommandes(); // Vérifier la suppression

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la création/gestion de la commande: {ex.Message}");
        }

        Console.WriteLine("--- Fin Test Module Commande ---");
    }

    static void SauvegardeFinale()
    {
        Console.WriteLine("\n--- Sauvegarde Finale ---");
        salarieManager.SauvegarderSalariesEtOrganigramme();
        commandeManager.SauvegarderCommandes();
        Console.WriteLine("Données sauvegardées.");
    }
}