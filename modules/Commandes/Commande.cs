using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Diagnostics;

#nullable enable

namespace Projet.Modules
{
    /// <summary>
    /// Représente une commande de livraison dans le système TransConnect.
    /// Contient les informations sur le client, le chauffeur, l'itinéraire,
    /// la distance, le prix et les dates de la commande.
    /// </summary>
    public class Commande
    {
         const double TAUX_BASE_KILOMETRIQUE = 1.5;
         const double COEFFICIENT_SALAIRE = 0.001;

        /// <summary>
        /// Obtient l'identifiant unique de la commande.
        /// </summary>
        [JsonInclude]
        public int Id { get;  set; }

        /// <summary>
        /// Obtient ou définit le client associé à la commande.
        /// </summary>
        [JsonInclude]
        public Client Client { get; set; }

        /// <summary>
        /// Obtient ou définit le chauffeur assigné à la commande. Peut être null.
        /// </summary>
        [JsonInclude]
        public Salarie? Chauffeur { get; set; }

        /// <summary>
        /// Obtient ou définit le véhicule assigné à la commande. Peut être null.
        /// </summary>
        [JsonInclude]
        public Vehicule? Vehicule { get; set; }

        /// <summary>
        /// Obtient ou définit la ville de départ.
        /// </summary>
        [JsonInclude]
        public Ville VilleDepart { get; set; }

        /// <summary>
        /// Obtient ou définit la ville d'arrivée.
        /// </summary>
        [JsonInclude]
        public Ville VilleArrivee { get; set; }

        /// <summary>
        /// Obtient la date de création de la commande.
        /// </summary>
        [JsonInclude]
        public DateTime DateCommande { get;  set; }

        /// <summary>
        /// Obtient ou définit la date de livraison prévue.
        /// </summary>
        [JsonInclude]
        public DateTime DateLivraison { get; set; }

        /// <summary>
        /// Obtient le prix total de la commande.
        /// </summary>
        [JsonInclude]
        public double Prix { get; set; }

        /// <summary>
        /// Obtient la distance calculée pour le trajet.
        /// </summary>
        [JsonInclude]
        public double DistanceCalculee { get; set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public Commande() { }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Commande.
        /// </summary>
        /// <param name="client">Le client qui passe la commande.</param>
        /// <param name="chauffeur">Le chauffeur assigné à la livraison.</param>
        /// <param name="vehicule">Le véhicule utilisé pour la livraison.</param>
        /// <param name="villeDepart">La ville de départ.</param>
        /// <param name="villeArrivee">La ville d'arrivée.</param>
        /// <param name="dateLivraison">La date de livraison prévue.</param>
        /// <exception cref="ArgumentNullException">Levée si un paramètre requis est null.</exception>
        /// <exception cref="ArgumentException">Levée si les villes sont identiques ou si la date est invalide.</exception>
        public Commande(Client client, Salarie? chauffeur, Vehicule? vehicule, Ville villeDepart, Ville villeArrivee, DateTime dateLivraison)
        {
            if (villeDepart.Equals(villeArrivee))
                throw new ArgumentException("La ville de départ et la ville d'arrivée ne peuvent pas être identiques.");
            if (dateLivraison < DateTime.Now.Date)
                throw new ArgumentException("La date de livraison ne peut pas être dans le passé.");

            Id = 0;
            Client = client;
            Chauffeur = chauffeur;
            Vehicule = vehicule;
            VilleDepart = villeDepart;
            VilleArrivee = villeArrivee;
            DateCommande = DateTime.Now;
            DateLivraison = dateLivraison.Date;

            // Calcul de la distance avec l'algorithme de Bellman-Ford
            CalculerDistance();
            CalculerPrix();
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Commande pour la désérialisation.
        /// </summary>
        public Commande(int id, Client client, Salarie? chauffeur, Vehicule? vehicule, Ville villeDepart, 
                       Ville villeArrivee, DateTime dateCommande, DateTime dateLivraison, double distance, double prix)
        {
            Id = id;
            Client = client;
            Chauffeur = chauffeur;
            Vehicule = vehicule;
            VilleDepart = villeDepart;
            VilleArrivee = villeArrivee;
            DateCommande = dateCommande;
            DateLivraison = dateLivraison;
            DistanceCalculee = distance;
            Prix = prix;
            
            // Suppression de la création de FinanceSimple pour éviter la récursion infinie
        }

        /// <summary>
        /// Calcule la distance entre la ville de départ et la ville d'arrivée.
        /// </summary>
        private void CalculerDistance()
        {
            GrapheListe grapheListe = new GrapheListe(true);
            GrapheService grapheService = new GrapheService(grapheListe);
            
            try 
            {
                Console.WriteLine($"Calcul de la distance entre {VilleDepart.Nom} et {VilleArrivee.Nom}");
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string cheminFichier = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "distances_villes_france.xlsx");
                grapheService.ChargerGrapheDepuisXlsx(cheminFichier);
                
                List<Ville> toutesLesVilles = grapheListe.GetToutesLesVilles().ToList();
                if (!toutesLesVilles.Any(v => v.Nom.Equals(VilleDepart.Nom)) || 
                    !toutesLesVilles.Any(v => v.Nom.Equals(VilleArrivee.Nom)))
                {
                    throw new InvalidOperationException(
                        $"Une des villes n'existe pas dans le graphe. Villes disponibles : {string.Join(", ", toutesLesVilles.Select(v => v.Nom))}");
                }

                // Calculer toutes les distances
                var dijkstra = grapheService.Dijkstra(VilleDepart, VilleArrivee);
                var bellmanFord = grapheService.BellmanFord(VilleDepart, VilleArrivee);
                
                var fw = grapheService.FloydWarshall();
                int departIndex = toutesLesVilles.FindIndex(v => v.Nom.Equals(VilleDepart.Nom));
                int arriveeIndex = toutesLesVilles.FindIndex(v => v.Nom.Equals(VilleArrivee.Nom));
                double distanceFw = double.PositiveInfinity;
                List<Ville> cheminFw = new List<Ville>();
                if (fw.HasValue)
                {
                    cheminFw = grapheService.ReconstruireCheminFloydWarshall(departIndex, arriveeIndex, fw.Value.distances, fw.Value.predecesseurs, toutesLesVilles);
                    distanceFw = fw.Value.distances[departIndex, arriveeIndex];
                }

                // Choisir le plus court chemin
                (List<Ville> chemin, double distance) resultat;
                if (dijkstra.distanceTotale <= bellmanFord.distanceTotale && dijkstra.distanceTotale <= distanceFw)
                {
                    resultat = dijkstra;
                    Console.WriteLine("Dijkstra trouve le plus court chemin");
                }
                else if (bellmanFord.distanceTotale <= distanceFw)
                {
                    resultat = bellmanFord;
                    Console.WriteLine("Bellman-Ford trouve le plus court chemin");
                }
                else
                {
                    resultat = (cheminFw, distanceFw);
                    Console.WriteLine("Floyd-Warshall trouve le plus court chemin");
                }

                if (double.IsInfinity(resultat.distance) || resultat.distance <= 0)
                {
                    Console.WriteLine($"Chemin trouvé : {(resultat.chemin != null ? string.Join(" -> ", resultat.chemin.Select(v => v.Nom)) : "aucun")}");
                    Console.WriteLine($"Distance calculée : {resultat.distance}");
                    throw new InvalidOperationException(
                        $"Impossible de calculer un itinéraire valide entre {VilleDepart.Nom} et {VilleArrivee.Nom}. " +
                        "Vérifiez que les villes sont bien connectées dans le graphe.");
                }

                DistanceCalculee = resultat.distance;
                Console.WriteLine($"Distance calculée avec succès : {resultat.distance} km");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du calcul de la distance : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Calcule le prix de la commande en fonction du véhicule et du chauffeur.
        /// </summary>
        private void CalculerPrix()
        {
            double tauxKilometrique = TAUX_BASE_KILOMETRIQUE;

            // Ajustement en fonction du salaire du chauffeur
            if (Chauffeur is Salarie chauffeur)
            {
                tauxKilometrique += (double)(chauffeur.Salaire * (decimal)COEFFICIENT_SALAIRE);
            }

            // Ajustement en fonction du véhicule
            if (Vehicule is Vehicule vehicule)
            {
                // Majoration en fonction du poids maximal
                tauxKilometrique += vehicule.PoidsMaximal * 0.1;

                // Majorations spécifiques selon le type
                switch (vehicule)
                {
                    case CamionFrigorifique _:
                        tauxKilometrique *= 1.3; // +30% pour les camions frigorifiques
                        break;
                    case CamionCiterne _:
                        tauxKilometrique *= 1.25; // +25% pour les camions citernes
                        break;
                    case PoidsLourd _:
                        tauxKilometrique *= 1.2; // +20% pour les poids lourds
                        break;
                }
            }

            Prix = DistanceCalculee * tauxKilometrique;
        }

        /// <summary>
        /// Vérifie si un chauffeur est disponible pour une date de livraison donnée.
        /// </summary>
        /// <param name="manager">Le gestionnaire de commandes.</param>
        /// <param name="chauffeur">Le chauffeur à vérifier.</param>
        /// <param name="dateLivraison">La date de livraison à vérifier.</param>
        /// <returns>True si le chauffeur est disponible, False sinon.</returns>
        public static bool EstChauffeurDisponible(CommandeManager manager, Salarie? chauffeur, DateTime dateLivraison)
        {
            if (chauffeur is not Salarie) return false;

            List<Commande> commandes = manager.GetToutesLesCommandes();
            return !commandes.Any(c => 
                c.Chauffeur?.NumeroSecuriteSociale == chauffeur.NumeroSecuriteSociale && 
                c.DateLivraison.Date == dateLivraison.Date);
        }

        /// <summary>
        /// Assigne un nouvel ID à la commande.
        /// </summary>
        /// <param name="id">Le nouvel ID à assigner.</param>
        /// <exception cref="ArgumentOutOfRangeException">Levée si l'ID est négatif ou nul.</exception>
        /// <exception cref="InvalidOperationException">Levée si la commande a déjà un ID.</exception>
        internal void AssignerId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "L'ID assigné doit être positif.");
            }

            if (Id == 0)
            {
                Id = id;
            }
            else if (Id != id)
            {
                throw new InvalidOperationException($"Impossible de changer l'ID de la commande de {Id} à {id}. L'ID est déjà assigné.");
            }
        }

        /// <summary>
        /// Retourne une représentation textuelle de la commande.
        /// </summary>
        /// <returns>Une chaîne décrivant les détails de la commande.</returns>
        public override string ToString()
        {
            string clientInfo = Client.ToString();
            string chauffeurInfo = Chauffeur?.ToString() ?? "Chauffeur non assigné";
            string vehiculeInfo = Vehicule?.GetDescription() ?? "Véhicule non assigné";
            string departInfo = VilleDepart.Nom;
            string arriveeInfo = VilleArrivee.Nom;

            return $"Commande #{Id}\n" +
                   $"  Date commande: {DateCommande:dd/MM/yyyy HH:mm}\n" +
                   $"  Date livraison: {DateLivraison:dd/MM/yyyy}\n" +
                   $"  Client:    {clientInfo}\n" +
                   $"  Chauffeur: {chauffeurInfo}\n" +
                   $"  Véhicule:  {vehiculeInfo}\n" +
                   $"  Trajet:    {departInfo} -> {arriveeInfo}\n" +
                   $"  Distance:  {DistanceCalculee:F2} km\n" +
                   $"  Prix:      {Prix:C2}";
        }
    }
}