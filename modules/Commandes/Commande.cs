using System;



namespace Projet.Modules
{
    /// <summary>
    /// Représente une commande de livraison dans le système TransConnect.
    /// Contient les informations sur le client, le chauffeur, l'itinéraire,
    /// la distance, le prix et la date de la commande.
    /// </summary>
    public class Commande
    {
        public int Id { get; private set; }
        public Client Client { get; set; }
        public Chauffeur Chauffeur { get; set; }
        public Ville VilleDepart { get; set; }
        public Ville VilleArrivee { get; set; }
        public DateTime DateCommande { get; private set; }
        public double Prix { get; set; }
        public double DistanceCalculee { get; set; }

        /// <summary>
        /// Constructeur principal pour créer une nouvelle instance de Commande.
        /// L'ID est initialisé à 0 (sera défini par CommandeManager) et la DateCommande est mise à l'heure actuelle.
        /// </summary>
        /// <param name="client">Le client associé à la commande.</param>
        /// <param name="chauffeur">Le chauffeur assigné (peut être null initialement si assigné plus tard).</param>
        /// <param name="villeDepart">La ville de départ du trajet.</param>
        /// <param name="villeArrivee">La ville d'arrivée du trajet.</param>
        /// <param name="distance">La distance calculée pour ce trajet (ex: via Dijkstra).</param>
        /// <param name="prix">Le prix estimé ou final de la commande.</param>
        /// <exception cref="ArgumentNullException">Lancée si client, villeDepart ou villeArrivee est null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Lancée si distance ou prix sont négatifs.</exception>
        /// <exception cref="ArgumentException">Lancée si villeDepart et villeArrivee sont identiques.</exception>
        public Commande(Client client, Chauffeur chauffeur, Ville villeDepart, Ville villeArrivee, double distance, double prix)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Le client ne peut pas être null.");
            if (villeDepart == null)
                throw new ArgumentNullException(nameof(villeDepart), "La ville de départ ne peut pas être null.");
            if (villeArrivee == null)
                throw new ArgumentNullException(nameof(villeArrivee), "La ville d'arrivée ne peut pas être null.");

            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "La distance ne peut pas être négative.");
            if (prix < 0)
                throw new ArgumentOutOfRangeException(nameof(prix), "Le prix ne peut pas être négatif.");
            if (villeDepart.Equals(villeArrivee))
                throw new ArgumentException("La ville de départ et la ville d'arrivée ne peuvent pas être identiques.");

            Id = 0;
            Client = client;
            Chauffeur = chauffeur;
            VilleDepart = villeDepart;
            VilleArrivee = villeArrivee;
            DateCommande = DateTime.Now;
            DistanceCalculee = distance;
            Prix = prix;
        }

        /// <summary>
        /// Constructeur utilisé spécifiquement pour la désérialisation (ex: chargement depuis un fichier JSON).
        /// Permet de recréer un objet Commande avec son ID et sa DateCommande d'origine.
        /// Appelle le constructeur principal pour initialiser les autres champs et effectuer les validations.
        /// </summary>
        /// <param name="id">L'identifiant unique original de la commande.</param>
        /// <param name="client">Le client associé.</param>
        /// <param name="chauffeur">Le chauffeur assigné.</param>
        /// <param name="villeDepart">La ville de départ.</param>
        /// <param name="villeArrivee">La ville d'arrivée.</param>
        /// <param name="dateCommande">La date et heure originales de la commande.</param>
        /// <param name="distance">La distance calculée.</param>
        /// <param name="prix">Le prix de la commande.</param>
        public Commande(int id, Client client, Chauffeur chauffeur, Ville villeDepart, Ville villeArrivee, DateTime dateCommande, double distance, double prix)
            : this(client, chauffeur, villeDepart, villeArrivee, distance, prix) 
        {
            Id = id;
            DateCommande = dateCommande;
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
            string departInfo = VilleDepart?.Nom ?? "Ville départ invalide";
            string arriveeInfo = VilleArrivee?.Nom ?? "Ville arrivée invalide";

            return $"Commande #{Id} [{DateCommande:dd/MM/yyyy HH:mm}]\n" +
                   $"  Client:    {clientInfo}\n" +
                   $"  Chauffeur: {chauffeurInfo}\n" +
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