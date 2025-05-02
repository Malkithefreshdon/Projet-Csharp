using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    /// <summary>
    /// Représente une commande de livraison dans le système TransConnect.
    /// Contient les informations sur le client, le chauffeur, l'itinéraire,
    /// la distance, le prix et les dates de la commande.
    /// </summary>
    public class Commande
    {
        [JsonInclude]
        public int Id { get; private set; }
        [JsonInclude]
        public Client Client { get; set; }
        [JsonInclude]
        public Salarie Chauffeur { get; set; }
        [JsonInclude]
        public Vehicule Vehicule { get; set; }
        [JsonInclude]
        public Ville VilleDepart { get; set; }
        [JsonInclude]
        public Ville VilleArrivee { get; set; }
        [JsonInclude]
        public DateTime DateCommande { get; private set; }
        [JsonInclude]
        public DateTime DateLivraison { get; set; }
        [JsonInclude]
        public double Prix { get; private set; }
        [JsonInclude]
        public double DistanceCalculee { get; private set; }

        // Constantes pour le calcul du prix
        private const double TAUX_BASE_KM = 1.5; // Prix de base par kilomètre
        private const double COEFFICIENT_SALAIRE = 0.001; // Coefficient pour le calcul du taux kilométrique basé sur le salaire

        // Constructeur par défaut nécessaire pour la désérialisation JSON
        [JsonConstructor]
        public Commande() { }

        /// <summary>
        /// Constructeur principal pour créer une nouvelle instance de Commande.
        /// </summary>
        public Commande(Client client, Salarie chauffeur, Vehicule vehicule, Ville villeDepart, Ville villeArrivee, DateTime dateLivraison)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Le client ne peut pas être null.");
            if (villeDepart == null)
                throw new ArgumentNullException(nameof(villeDepart), "La ville de départ ne peut pas être null.");
            if (villeArrivee == null)
                throw new ArgumentNullException(nameof(villeArrivee), "La ville d'arrivée ne peut pas être null.");
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

            // Calcul de la distance avec BellmanFord
            var grapheListe = new GrapheListe(true);
            var grapheService = new GrapheService(grapheListe);
            
            try 
            {
                Console.WriteLine($"Calcul de la distance entre {villeDepart.Nom} et {villeArrivee.Nom}");
                grapheService.ChargerGrapheDepuisXlsx("Ressources/distances_villes_france.xlsx");
                
                // Vérification que les villes sont bien dans le graphe
                var toutesLesVilles = grapheListe.GetToutesLesVilles().ToList();
                if (!toutesLesVilles.Any(v => v.Nom.Equals(villeDepart.Nom)) || 
                    !toutesLesVilles.Any(v => v.Nom.Equals(villeArrivee.Nom)))
                {
                    throw new InvalidOperationException($"Une des villes n'existe pas dans le graphe. Villes disponibles : {string.Join(", ", toutesLesVilles.Select(v => v.Nom))}");
                }

                var (chemin, distance) = grapheService.BellmanFord(villeDepart, villeArrivee);
                
                if (double.IsInfinity(distance) || distance <= 0)
                {
                    Console.WriteLine($"Chemin trouvé : {(chemin != null ? string.Join(" -> ", chemin.Select(v => v.Nom)) : "aucun")}");
                    Console.WriteLine($"Distance calculée : {distance}");
                    throw new InvalidOperationException($"Impossible de calculer un itinéraire valide entre {villeDepart.Nom} et {villeArrivee.Nom}. Vérifiez que les villes sont bien connectées dans le graphe.");
                }

                DistanceCalculee = distance;
                Console.WriteLine($"Distance calculée avec succès : {distance} km");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du calcul de la distance : {ex.Message}");
                throw;
            }

            // Calcul du prix
            CalculerPrix();
        }

        /// <summary>
        /// Constructeur pour la désérialisation JSON
        /// </summary>
        public Commande(int id, Client client, Salarie chauffeur, Vehicule vehicule, Ville villeDepart, 
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
            try
            {
                var finance = new FinanceSimple();
                finance.AjouterTransaction(
                    Prix,
                    "Crédit",
                    $"Commande #{Id} - {VilleDepart.Nom} à {VilleArrivee.Nom}",
                    "Transport"
                );
            }
            catch (Exception)
            {
                // Ignorer les erreurs pour ne pas bloquer la création de la commande
            }
        }

        /// <summary>
        /// Calcule le prix de la commande en fonction du véhicule et du chauffeur
        /// </summary>
        private void CalculerPrix()
        {
            double tauxKilometrique = TAUX_BASE_KM;

            // Ajustement en fonction du salaire du chauffeur si présent
            if (Chauffeur != null)
            {
                tauxKilometrique += (double)(Chauffeur.Salaire * (decimal)COEFFICIENT_SALAIRE);
            }

            // Ajustement en fonction du type de véhicule si présent
            if (Vehicule != null)
            {
                // Majoration en fonction du poids maximal du véhicule
                tauxKilometrique += Vehicule.PoidsMaximal * 0.1;

                // Majorations spécifiques selon le type de véhicule
                switch (Vehicule)
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
        /// Vérifie si un chauffeur est disponible pour une date de livraison donnée
        /// </summary>
        public static bool EstChauffeurDisponible(CommandeManager manager, Salarie chauffeur, DateTime dateLivraison)
        {
            if (chauffeur == null) return false;

            var commandes = manager.GetToutesLesCommandes();
            return !commandes.Any(c => 
                c.Chauffeur?.NumeroSecuriteSociale == chauffeur.NumeroSecuriteSociale && 
                c.DateLivraison.Date == dateLivraison.Date);
        }

        /// <summary>
        /// Fournit une représentation textuelle formatée de la commande.
        /// Utile pour l'affichage dans la console ou les logs.
        /// </summary>
        /// <returns>Une chaîne de caractères décrivant les détails de la commande.</returns>
        public override string ToString()
        {
            string clientInfo = Client?.ToString() ?? "Client non spécifié";
            string chauffeurInfo = Chauffeur?.ToString() ?? "Chauffeur non assigné";
            string vehiculeInfo = Vehicule?.GetDescription() ?? "Véhicule non assigné";
            string departInfo = VilleDepart?.Nom ?? "Ville départ invalide";
            string arriveeInfo = VilleArrivee?.Nom ?? "Ville arrivée invalide";

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

        /// <summary>
        /// Méthode interne permettant au CommandeManager d'assigner un ID unique à la commande.
        /// Ne devrait être appelée qu'une seule fois lorsque la commande est ajoutée au manager
        /// et que son ID est encore à la valeur par défaut (0).
        /// </summary>
        /// <param name="id">L'identifiant unique à assigner.</param>
        /// <exception cref="InvalidOperationException">Lancée si on tente d'assigner un ID alors qu'un ID > 0 existe déjà.</exception>
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
    }
}