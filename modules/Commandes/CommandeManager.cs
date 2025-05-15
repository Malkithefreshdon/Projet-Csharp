using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

#nullable enable

namespace Projet.Modules
{
    /// <summary>
    /// Gère la collection des commandes de l'entreprise TransConnect.
    /// </summary>
    public class CommandeManager
    {
        // Classe interne pour la désérialisation du format JSON spécifique
        private class CommandesWrapper
        {
            [JsonPropertyName("$id")]
            public string? Id { get; set; }
            
            [JsonPropertyName("$values")]
            public List<Commande>? Values { get; set; }
        }

        private readonly List<Commande> commandes;
        private readonly string jsonPath;
        private int prochainId;

        /// <summary>
        /// Initialise une nouvelle instance de la classe CommandeManager.
        /// </summary>
        public CommandeManager()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            jsonPath = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "commandes.json");
            commandes = new List<Commande>();
            ChargerCommandes();
            prochainId = commandes.Any() ? commandes.Max(c => c.Id) + 1 : 1;
        }

        /// <summary>
        /// Ajoute une nouvelle commande à la collection après vérification de la disponibilité du chauffeur.
        /// </summary>
        /// <param name="commande">La commande à ajouter.</param>
        /// <exception cref="ArgumentNullException">Levée si la commande est null.</exception>
        /// <exception cref="InvalidOperationException">Levée si la commande a déjà un ID ou si le chauffeur n'est pas disponible.</exception>
        public void AjouterCommande(Commande commande)
        {
            if (commande == null)
                throw new ArgumentNullException(nameof(commande), "La commande à ajouter ne peut pas être null.");

            // Vérifier la disponibilité du chauffeur
            if (commande.Chauffeur != null && !Commande.EstChauffeurDisponible(this, commande.Chauffeur, commande.DateLivraison))
            {
                throw new InvalidOperationException(
                    $"Le chauffeur {commande.Chauffeur.Nom} n'est pas disponible à la date {commande.DateLivraison:dd/MM/yyyy}.");
            }

            if (commande.Id == 0)
            {
                commande.AssignerId(prochainId++);
            }
            else
            {
                if (commandes.Any(c => c.Id == commande.Id))
                {
                    throw new InvalidOperationException($"Une commande avec l'ID {commande.Id} existe déjà.");
                }
                prochainId = Math.Max(prochainId, commande.Id + 1);
            }

            commandes.Add(commande);
            commande.Client.AjouterCommande(commande); 
            Console.WriteLine($"Commande #{commande.Id} ajoutée avec succès.");
            SauvegarderCommandes();
        }

        /// <summary>
        /// Supprime une commande par son ID.
        /// </summary>
        /// <param name="id">L'ID de la commande à supprimer.</param>
        /// <returns>True si la commande a été supprimée, False si elle n'a pas été trouvée.</returns>
        public bool SupprimerCommande(int id)
        {
            Commande? commandeASupprimer = TrouverCommandeParId(id);
            if (commandeASupprimer != null)
            {
                commandes.Remove(commandeASupprimer);
                Console.WriteLine($"Commande #{id} supprimée avec succès.");
                SauvegarderCommandes();
                return true;
            }
            else
            {
                Console.WriteLine($"Erreur : Aucune commande trouvée avec l'ID {id}.");
                return false;
            }
        }

        /// <summary>
        /// Modifie une commande existante après vérification de la disponibilité du chauffeur.
        /// </summary>
        /// <param name="id">L'ID de la commande à modifier.</param>
        /// <param name="nouvelleCommandeData">Un objet Commande contenant les nouvelles données.</param>
        /// <returns>True si la commande a été modifiée, False si elle n'a pas été trouvée.</returns>
        /// <exception cref="ArgumentNullException">Levée si nouvelleCommandeData est null.</exception>
        public bool ModifierCommande(int id, Commande nouvelleCommandeData)
        {
            if (nouvelleCommandeData == null)
                throw new ArgumentNullException(nameof(nouvelleCommandeData), "Les nouvelles données de commande ne peuvent pas être null.");

            Commande? commandeAModifier = TrouverCommandeParId(id);
            if (commandeAModifier != null)
            {
                // Vérifier la disponibilité du chauffeur si changé
                if (nouvelleCommandeData.Chauffeur != null &&
                    (commandeAModifier.Chauffeur?.NumeroSecuriteSociale != nouvelleCommandeData.Chauffeur.NumeroSecuriteSociale ||
                     commandeAModifier.DateLivraison.Date != nouvelleCommandeData.DateLivraison.Date))
                {
                    if (!Commande.EstChauffeurDisponible(this, nouvelleCommandeData.Chauffeur, nouvelleCommandeData.DateLivraison))
                    {
                        throw new InvalidOperationException(
                            $"Le chauffeur {nouvelleCommandeData.Chauffeur.Nom} n'est pas disponible à la date {nouvelleCommandeData.DateLivraison:dd/MM/yyyy}.");
                    }
                }

                commandeAModifier.Client = nouvelleCommandeData.Client;
                commandeAModifier.Chauffeur = nouvelleCommandeData.Chauffeur;
                commandeAModifier.Vehicule = nouvelleCommandeData.Vehicule;
                commandeAModifier.VilleDepart = nouvelleCommandeData.VilleDepart;
                commandeAModifier.VilleArrivee = nouvelleCommandeData.VilleArrivee;
                commandeAModifier.DateLivraison = nouvelleCommandeData.DateLivraison;

                Console.WriteLine($"Commande #{id} modifiée avec succès.");
                SauvegarderCommandes();
                return true;
            }

            Console.WriteLine($"Erreur : Aucune commande trouvée avec l'ID {id} pour modification.");
            return false;
        }

        /// <summary>
        /// Recherche une commande par son ID.
        /// </summary>
        /// <param name="id">L'ID de la commande à rechercher.</param>
        /// <returns>La commande trouvée ou null si aucune commande ne correspond.</returns>
        public Commande? TrouverCommandeParId(int id)
        {
            return commandes.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Obtient toutes les commandes gérées.
        /// </summary>
        /// <returns>Une liste de toutes les commandes.</returns>
        public List<Commande> GetToutesLesCommandes()
        {
            return commandes;
        }

        /// <summary>
        /// Affiche toutes les commandes dans la console.
        /// </summary>
        public void AfficherToutesCommandes()
        {
            Console.WriteLine("\n--- Liste de toutes les commandes ---");
            if (!commandes.Any())
            {
                Console.WriteLine("Aucune commande enregistrée.");
            }
            else
            {
                foreach (Commande commande in commandes)
                {
                    Console.WriteLine(commande);
                    Console.WriteLine("-----------------------------------");
                }
            }
        }

        /// <summary>
        /// Recherche les commandes associées à un client par son nom.
        /// </summary>
        /// <param name="nomClient">Le nom du client à rechercher.</param>
        /// <returns>Une liste des commandes trouvées.</returns>
        public List<Commande> RechercherCommandesParClient(string nomClient)
        {
            if (string.IsNullOrWhiteSpace(nomClient))
            {
                Console.WriteLine("Le nom du client ne peut pas être vide pour la recherche.");
                return new List<Commande>();
            }

            List<Commande> commandesClient = commandes
                .Where(c => c.Client != null && c.Client.Nom.Equals(nomClient, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!commandesClient.Any())
            {
                Console.WriteLine($"Aucune commande trouvée pour le client '{nomClient}'.");
            }

            return commandesClient;
        }

        /// <summary>
        /// Recherche les commandes associées à un véhicule par son immatriculation.
        /// </summary>
        /// <param name="immatriculation">L'immatriculation du véhicule à rechercher.</param>
        /// <returns>Une liste des commandes trouvées.</returns>
        public List<Commande> RechercherCommandesParVehicule(string immatriculation)
        {
            if (string.IsNullOrWhiteSpace(immatriculation))
            {
                Console.WriteLine("L'immatriculation ne peut pas être vide pour la recherche.");
                return new List<Commande>();
            }

            List<Commande> commandesVehicule = commandes
                .Where(c => c.Vehicule != null && c.Vehicule.Immatriculation.Equals(immatriculation, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!commandesVehicule.Any())
            {
                Console.WriteLine($"Aucune commande trouvée pour le véhicule '{immatriculation}'.");
            }

            return commandesVehicule;
        }

        /// <summary>
        /// Recherche les commandes prévues pour une date de livraison.
        /// </summary>
        /// <param name="dateLivraison">La date de livraison à rechercher.</param>
        /// <returns>Une liste des commandes trouvées.</returns>
        public List<Commande> RechercherCommandesParDateLivraison(DateTime dateLivraison)
        {
            List<Commande> commandesDate = commandes
                .Where(c => c.DateLivraison.Date == dateLivraison.Date)
                .ToList();

            if (!commandesDate.Any())
            {
                Console.WriteLine($"Aucune commande trouvée pour la date de livraison {dateLivraison:dd/MM/yyyy}.");
            }

            return commandesDate;
        }

        /// <summary>
        /// Sauvegarde les commandes dans un fichier JSON.
        /// </summary>
        /// <param name="cheminFichier">Chemin optionnel du fichier. Si null, utilise le chemin par défaut.</param>
        public void SauvegarderCommandes(string? cheminFichier = null)
        {
            string fichier = cheminFichier ?? jsonPath;
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve,
                    Converters = 
                    {
                        new JsonStringEnumConverter()
                    }
                };

                // Créer une structure correspondant au format attendu
                var wrapper = new CommandesWrapper
                {
                    Id = "1",
                    Values = commandes
                };
                
                string jsonString = JsonSerializer.Serialize(wrapper, options);
                File.WriteAllText(fichier, jsonString);
                Console.WriteLine($"Commandes sauvegardées avec succès dans {fichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des commandes dans {fichier}: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge les commandes depuis un fichier JSON.
        /// </summary>
        /// <param name="cheminFichier">Chemin optionnel du fichier. Si null, utilise le chemin par défaut.</param>
        public void ChargerCommandes(string? cheminFichier = null)
        {
            string fichier = cheminFichier ?? jsonPath;
            if (File.Exists(fichier))
            {
                try
                {
                    // Lire tout le fichier JSON en tant que texte
                    string jsonString = File.ReadAllText(fichier);
                    
                    // Convertir manuellement le JSON en éliminant les références circulaires
                    using JsonDocument doc = JsonDocument.Parse(jsonString);
                    
                    if (doc.RootElement.TryGetProperty("$values", out JsonElement valuesArray))
                    {
                        // Traiter le tableau de commandes
                        commandes.Clear();
                        
                        foreach (JsonElement commandeElement in valuesArray.EnumerateArray())
                        {
                            try
                            {
                                // Récupérer l'identifiant
                                int id = commandeElement.GetProperty("Id").GetInt32();
                                
                                // Récupérer le client
                                JsonElement clientElement = commandeElement.GetProperty("Client");
                                string numeroSS = clientElement.GetProperty("NumeroSS").GetString() ?? string.Empty;
                                string nom = clientElement.GetProperty("Nom").GetString() ?? string.Empty;
                                string prenom = clientElement.GetProperty("Prenom").GetString() ?? string.Empty;
                                DateTime dateNaissance = clientElement.GetProperty("DateNaissance").GetDateTime();
                                string adresse = clientElement.GetProperty("Adresse").GetString() ?? string.Empty;
                                string? email = null;
                                string? telephone = null;
                                
                                if (clientElement.TryGetProperty("Email", out JsonElement emailElement) && !emailElement.ValueKind.Equals(JsonValueKind.Null))
                                    email = emailElement.GetString();
                                
                                if (clientElement.TryGetProperty("Telephone", out JsonElement telElement) && !telElement.ValueKind.Equals(JsonValueKind.Null))
                                    telephone = telElement.GetString();
                                
                                Client client = new Client(numeroSS, nom, prenom, dateNaissance, adresse);
                                client.Email = email;
                                client.Telephone = telephone;
                                
                                // Récupérer les villes
                                JsonElement villeDepartElement = commandeElement.GetProperty("VilleDepart");
                                string nomVilleDepart = villeDepartElement.GetProperty("Nom").GetString() ?? string.Empty;
                                Ville villeDepart = new Ville(nomVilleDepart);
                                
                                JsonElement villeArriveeElement = commandeElement.GetProperty("VilleArrivee");
                                string nomVilleArrivee = villeArriveeElement.GetProperty("Nom").GetString() ?? string.Empty;
                                Ville villeArrivee = new Ville(nomVilleArrivee);
                                
                                // Récupérer les dates
                                DateTime dateCommande = commandeElement.GetProperty("DateCommande").GetDateTime();
                                DateTime dateLivraison = commandeElement.GetProperty("DateLivraison").GetDateTime();
                                
                                // Récupérer la distance et le prix
                                double distance = commandeElement.GetProperty("DistanceCalculee").GetDouble();
                                double prix = commandeElement.GetProperty("Prix").GetDouble();
                                
                                // Chauffeur (peut être null)
                                Salarie? chauffeur = null;
                                if (commandeElement.TryGetProperty("Chauffeur", out JsonElement chauffeurElement) && 
                                    !chauffeurElement.ValueKind.Equals(JsonValueKind.Null))
                                {
                                    string numeroSecuriteSociale = chauffeurElement.GetProperty("NumeroSecuriteSociale").GetString() ?? string.Empty;
                                    string nomChauffeur = chauffeurElement.GetProperty("Nom").GetString() ?? string.Empty;
                                    string prenomChauffeur = chauffeurElement.GetProperty("Prenom").GetString() ?? string.Empty;
                                    
                                    // Obtenir le chauffeur depuis le manager de salariés si possible
                                    try
                                    {
                                        SalarieManager salarieManager = new SalarieManager();
                                        chauffeur = salarieManager.RechercherParId(numeroSecuriteSociale);
                                    }
                                    catch
                                    {
                                        // Si le chauffeur n'est pas trouvé, créer un objet minimal
                                        chauffeur = new Salarie
                                        {
                                            NumeroSecuriteSociale = numeroSecuriteSociale,
                                            Nom = nomChauffeur,
                                            Prenom = prenomChauffeur,
                                            Poste = "Chauffeur"
                                        };
                                    }
                                }
                                
                                // Véhicule (peut être null)
                                Vehicule? vehicule = null;
                                if (commandeElement.TryGetProperty("Vehicule", out JsonElement vehiculeElement) && 
                                    !vehiculeElement.ValueKind.Equals(JsonValueKind.Null))
                                {
                                    string immatriculation = vehiculeElement.GetProperty("Immatriculation").GetString() ?? string.Empty;
                                    string marque = vehiculeElement.GetProperty("Marque").GetString() ?? string.Empty;
                                    string modele = vehiculeElement.GetProperty("Modele").GetString() ?? string.Empty;
                                    double poidsMaximal = vehiculeElement.GetProperty("PoidsMaximal").GetDouble();
                                    
                                    // Vérifier le type de véhicule
                                    if (vehiculeElement.TryGetProperty("$type", out JsonElement typeElement))
                                    {
                                        string typeVehicule = typeElement.GetString() ?? string.Empty;
                                        
                                        if (typeVehicule.Contains("Voiture"))
                                        {
                                            int nombrePassagers = vehiculeElement.GetProperty("NombrePassagers").GetInt32();
                                            vehicule = new Voiture(immatriculation, poidsMaximal, marque, modele, nombrePassagers);
                                        }
                                        else if (typeVehicule.Contains("PoidsLourd"))
                                        {
                                            double volumeRemorque = vehiculeElement.GetProperty("VolumeRemorque").GetDouble();
                                            string typeRemorque = vehiculeElement.GetProperty("TypeRemorque").GetString() ?? "Standard";
                                            string typeMarchandise = vehiculeElement.GetProperty("TypeMarchandise").GetString() ?? "Générale";
                                            bool hasHayon = vehiculeElement.GetProperty("HasHayon").GetBoolean();
                                            vehicule = new PoidsLourd(immatriculation, poidsMaximal, marque, modele, volumeRemorque, typeRemorque, typeMarchandise, hasHayon);
                                        }
                                        // Ajouter d'autres types si nécessaire
                                    }
                                }
                                
                                // Créer la commande avec les données extraites
                                Commande commande = new Commande(id, client, chauffeur, vehicule, villeDepart, villeArrivee, dateCommande, dateLivraison, distance, prix);
                                commandes.Add(commande);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erreur lors du traitement d'une commande: {ex.Message}");
                            }
                        }
                        
                        // Reconstruire les liens client-commandes
                        RebuildClientCommandeLinks();
                        
                        // Définir le prochain ID
                        prochainId = commandes.Any() ? commandes.Max(c => c.Id) + 1 : 1;
                        Console.WriteLine($"Commandes chargées avec succès depuis {fichier}. {commandes.Count} commandes récupérées.");
                    }
                    else
                    {
                        // Essayer de désérialiser directement comme une liste
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        
                        List<Commande>? commandesChargees = JsonSerializer.Deserialize<List<Commande>>(jsonString, options);
                        
                        if (commandesChargees != null)
                        {
                            commandes.Clear();
                            commandes.AddRange(commandesChargees);
                            RebuildClientCommandeLinks();
                            prochainId = commandes.Any() ? commandes.Max(c => c.Id) + 1 : 1;
                            Console.WriteLine($"Commandes chargées avec succès depuis {fichier}. {commandes.Count} commandes récupérées.");
                        }
                        else
                        {
                            Console.WriteLine($"Avertissement : Le fichier {fichier} semble vide ou mal formaté.");
                            commandes.Clear();
                            prochainId = 1;
                        }
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Erreur de format JSON lors du chargement depuis {fichier}: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur générale lors du chargement des commandes depuis {fichier}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Fichier de sauvegarde {fichier} non trouvé. Aucune commande chargée.");
                commandes.Clear();
                prochainId = 1;
            }
        }

        /// <summary>
        /// Reconstruit les liens entre les clients et leurs commandes après désérialisation.
        /// </summary>
        private void RebuildClientCommandeLinks()
        {
            // Dictionnaire pour regrouper les commandes par client (en utilisant NumeroSS comme clé)
            Dictionary<string, List<Commande>> commandesParClient = new Dictionary<string, List<Commande>>();
            
            // Regrouper toutes les commandes par client
            foreach (Commande commande in commandes)
            {
                if (commande.Client != null)
                {
                    string numeroSS = commande.Client.NumeroSS;
                    if (!commandesParClient.ContainsKey(numeroSS))
                    {
                        commandesParClient[numeroSS] = new List<Commande>();
                    }
                    commandesParClient[numeroSS].Add(commande);
                }
            }
            
            // Reconstruire l'historique des commandes pour chaque client
            foreach (Commande commande in commandes)
            {
                if (commande.Client != null && commandesParClient.ContainsKey(commande.Client.NumeroSS))
                {
                    // S'assurer que l'historique est bien initialisé
                    if (commande.Client.HistoriqueCommandes == null)
                    {
                        commande.Client.HistoriqueCommandes = new List<Commande>();
                    }
                    
                    // Vider l'historique existant et ajouter toutes les commandes associées
                    commande.Client.HistoriqueCommandes.Clear();
                    commande.Client.HistoriqueCommandes.AddRange(commandesParClient[commande.Client.NumeroSS]);
                }
            }
        }

        /// <summary>
        /// Synchronise les clients avec leurs commandes.
        /// À appeler après le chargement des clients et des commandes.
        /// </summary>
        /// <param name="clientManager">Le gestionnaire de clients à synchroniser</param>
        public void SynchroniserClientsAvecCommandes(ClientManager clientManager)
        {
            if (clientManager == null)
                return;
                
            List<Client> clients = clientManager.ObtenirTousLesClients();
            
            // Dictionnaire pour regrouper les commandes par client (en utilisant NumeroSS comme clé)
            Dictionary<string, List<Commande>> commandesParClient = new Dictionary<string, List<Commande>>();
            
            // Regrouper toutes les commandes par client
            foreach (Commande commande in commandes)
            {
                if (commande.Client != null)
                {
                    string numeroSS = commande.Client.NumeroSS;
                    if (!commandesParClient.ContainsKey(numeroSS))
                    {
                        commandesParClient[numeroSS] = new List<Commande>();
                    }
                    commandesParClient[numeroSS].Add(commande);
                }
            }
            
            // Mettre à jour l'historique des commandes pour chaque client
            foreach (Client client in clients)
            {
                if (commandesParClient.ContainsKey(client.NumeroSS))
                {
                    // S'assurer que l'historique est bien initialisé
                    if (client.HistoriqueCommandes == null)
                    {
                        client.HistoriqueCommandes = new List<Commande>();
                    }
                    
                    // Vider l'historique existant et ajouter toutes les commandes associées
                    client.HistoriqueCommandes.Clear();
                    client.HistoriqueCommandes.AddRange(commandesParClient[client.NumeroSS]);
                }
            }
            
            Console.WriteLine($"Synchronisation terminée : {clients.Count} clients et {commandes.Count} commandes synchronisés.");
        }
    }
}