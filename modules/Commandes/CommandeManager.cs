using modules.Commandes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // Pour la sérialisation/désérialisation JSON

namespace modules.Commandes
{
    public class CommandeManager
    {
        private List<Commande> _commandes;
        private int _prochainId;

        public string FichierSauvegarde { get; set; } = "commandes.json";

        public CommandeManager()
        {
            _commandes = new List<Commande>();
            ChargerCommandes();
            _prochainId = _commandes.Any() ? _commandes.Max(c => c.Id) + 1 : 1;
        }

        /// <summary>
        /// Ajoute une nouvelle commande à la collection.
        /// Un ID unique lui est assigné.
        /// </summary>
        /// <param name="commande">La commande à ajouter.</param>
        /// <exception cref="ArgumentNullException">Lancée si la commande est null.</exception>
        /// <exception cref="InvalidOperationException">Lancée si la commande a déjà un ID ou si l'ajout échoue.</exception>
        public void AjouterCommande(Commande commande)
        {
            if (commande == null)
                throw new ArgumentNullException(nameof(commande), "La commande à ajouter ne peut pas être null.");

            if (commande.Id == 0)
            {
                commande.AssignerId(_prochainId++);
            }
            else
            {
                if (_commandes.Any(c => c.Id == commande.Id))
                {
                    throw new InvalidOperationException($"Une commande avec l'ID {commande.Id} existe déjà. Impossible d'ajouter.");
                }
                _prochainId = Math.Max(_prochainId, commande.Id + 1);
            }


            _commandes.Add(commande);
            Console.WriteLine($"Commande #{commande.Id} ajoutée avec succès.");
            SauvegarderCommandes();
        }

        /// <summary>
        /// Supprime une commande de la collection en utilisant son ID.
        /// </summary>
        /// <param name="id">L'ID de la commande à supprimer.</param>
        /// <returns>True si la commande a été trouvée et supprimée, False sinon.</returns>
        public bool SupprimerCommande(int id)
        {
            Commande commandeASupprimer = TrouverCommandeParId(id);
            if (commandeASupprimer != null)
            {
                _commandes.Remove(commandeASupprimer);
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
        /// Modifie une commande existante en utilisant son ID.
        /// Les informations sont remplacées par celles de la nouvelle commande fournie,
        /// à l'exception de l'ID et de la DateCommande initiale.
        /// </summary>
        /// <param name="id">L'ID de la commande à modifier.</param>
        /// <param name="nouvelleCommandeData">Un objet Commande contenant les nouvelles données.</param>
        /// <returns>True si la commande a été trouvée et modifiée, False sinon.</returns>
        /// <exception cref="ArgumentNullException">Lancée si nouvelleCommandeData est null.</exception>
        public bool ModifierCommande(int id, Commande nouvelleCommandeData)
        {
            if (nouvelleCommandeData == null)
                throw new ArgumentNullException(nameof(nouvelleCommandeData), "Les nouvelles données de commande ne peuvent pas être null.");

            Commande commandeAModifier = TrouverCommandeParId(id);
            if (commandeAModifier != null)
            {
                commandeAModifier.Client = nouvelleCommandeData.Client;
                commandeAModifier.Chauffeur = nouvelleCommandeData.Chauffeur;
                commandeAModifier.VilleDepart = nouvelleCommandeData.VilleDepart;
                commandeAModifier.VilleArrivee = nouvelleCommandeData.VilleArrivee;
                commandeAModifier.DistanceCalculee = nouvelleCommandeData.DistanceCalculee;
                commandeAModifier.Prix = nouvelleCommandeData.Prix;

                Console.WriteLine($"Commande #{id} modifiée avec succès.");
                SauvegarderCommandes();
                return true;
            }
            else
            {
                Console.WriteLine($"Erreur : Aucune commande trouvée avec l'ID {id} pour modification.");
                return false;
            }
        }

        /// <summary>
        /// Recherche et retourne une commande par son ID.
        /// </summary>
        /// <param name="id">L'ID de la commande à rechercher.</param>
        /// <returns>L'objet Commande trouvé, ou null si aucune commande ne correspond.</returns>
        public Commande TrouverCommandeParId(int id)
        {
            return _commandes.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Affiche toutes les commandes actuellement gérées dans la console.
        /// </summary>
        public List<Commande> ObtenirToutesLesCommandes()
        {
            return _commandes;
        }
        public void AfficherToutesCommandes()
        {
            Console.WriteLine("\n--- Liste de toutes les commandes ---");
            if (!_commandes.Any())
            {
                Console.WriteLine("Aucune commande enregistrée.");
            }
            else
            {
                foreach (var commande in _commandes)
                {
                    Console.WriteLine(commande);
                    Console.WriteLine("-----------------------------------");
                }
            }
        }

        /// <summary>
        /// Recherche et retourne toutes les commandes associées à un client spécifique (par nom).
        /// </summary>
        /// <param name="nomClient">Le nom du client à rechercher.</param>
        /// <returns>Une liste des commandes trouvées pour ce client.</returns>
        /// <remarks>La comparaison des noms est insensible à la casse.</remarks>
        public List<Commande> RechercherCommandesParClient(string nomClient)
        {
            if (string.IsNullOrWhiteSpace(nomClient))
            {
                Console.WriteLine("Le nom du client ne peut pas être vide pour la recherche.");
                return new List<Commande>();
            }

            var commandesClient = _commandes
                .Where(c => c.Client != null && c.Client.Nom.Equals(nomClient, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!commandesClient.Any())
            {
                Console.WriteLine($"Aucune commande trouvée pour le client '{nomClient}'.");
            }

            return commandesClient;
        }



        /// <summary>
        /// Retourne une copie de la liste de toutes les commandes.
        /// Utile pour des traitements externes sans modifier la liste interne.
        /// </summary>
        /// <returns>Une nouvelle liste contenant toutes les commandes.</returns>
        public List<Commande> GetToutesLesCommandes()
        {
            return new List<Commande>(_commandes);
        }


        /// <summary>
        /// Sauvegarde la liste actuelle des commandes dans un fichier JSON.
        /// </summary>
        /// <param name="cheminFichier">Chemin optionnel du fichier. Si null, utilise FichierSauvegarde.</param>
        public void SauvegarderCommandes(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? FichierSauvegarde;
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // Gérer les références circulaires si Client/Chauffeur ont des liens complexes
                    // ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };

                string jsonString = JsonSerializer.Serialize(_commandes, options);
                File.WriteAllText(fichier, jsonString);
                Console.WriteLine($"Commandes sauvegardées avec succès dans {fichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des commandes dans {fichier}: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge la liste des commandes depuis un fichier JSON.
        /// Remplace la liste actuelle en mémoire.
        /// </summary>
        /// <param name="cheminFichier">Chemin optionnel du fichier. Si null, utilise FichierSauvegarde.</param>
        public void ChargerCommandes(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? FichierSauvegarde;
            if (File.Exists(fichier))
            {
                try
                {
                    string jsonString = File.ReadAllText(fichier);
                    var commandesChargees = JsonSerializer.Deserialize<List<Commande>>(jsonString);

                    if (commandesChargees != null)
                    {
                        _commandes = commandesChargees;
                        _prochainId = _commandes.Any() ? _commandes.Max(c => c.Id) + 1 : 1;
                        Console.WriteLine($"Commandes chargées avec succès depuis {fichier}. {_commandes.Count} commandes récupérées.");
                    }
                    else
                    {
                        Console.WriteLine($"Avertissement : Le fichier {fichier} semble vide ou mal formaté. Aucune commande chargée.");
                        _commandes = new List<Commande>(); // Réinitialise à une liste vide
                        _prochainId = 1;
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
                _commandes = new List<Commande>();
                _prochainId = 1;
            }
        }
    }
}