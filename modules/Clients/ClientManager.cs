using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    /// <summary>
    /// Gère la collection des clients de l'entreprise TransConnect.
    /// </summary>
    public class ClientManager
    {
        private readonly List<Client> clients;
        private readonly string jsonPath;

        /// <summary>
        /// Initialise une nouvelle instance de la classe ClientManager.
        /// </summary>
        public ClientManager()
        {
            clients = new List<Client>();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            jsonPath = Path.Combine(baseDirectory, "..", "..", "..", "ressources", "clients.json");
            ChargerClients();
        }

        /// <summary>
        /// Charge les clients depuis le fichier JSON.
        /// </summary>
        public void ChargerClients()
        {
            if (File.Exists(jsonPath))
            {
                string jsonString = File.ReadAllText(jsonPath);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                ClientsData? clientsData = JsonSerializer.Deserialize<ClientsData>(jsonString, options);
                clients.Clear();
                if (clientsData?.Clients != null)
                {
                    clients.AddRange(clientsData.Clients);
                }
            }
            else
            {
                clients.Clear();
                SauvegarderClients();
            }
        }

        /// <summary>
        /// Sauvegarde les clients dans le fichier JSON.
        /// </summary>
        public void SauvegarderClients()
        {
            ClientsData clientsData = new ClientsData { Clients = clients };
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string jsonString = JsonSerializer.Serialize(clientsData, options);
            File.WriteAllText(jsonPath, jsonString);
        }

        /// <summary>
        /// Ajoute un nouveau client.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale du client.</param>
        /// <param name="nom">Le nom du client.</param>
        /// <param name="prenom">Le prénom du client.</param>
        /// <param name="dateNaissance">La date de naissance du client.</param>
        /// <param name="adresse">L'adresse du client.</param>
        /// <exception cref="ArgumentException">Levée si les champs obligatoires sont vides ou si le numéro de sécurité sociale existe déjà.</exception>
        public void AjouterClient(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adresse)
        {
            if (string.IsNullOrEmpty(numeroSS) || string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(adresse))
                throw new ArgumentException("Les champs obligatoires ne peuvent pas être vides");

            if (clients.Any(c => c.NumeroSS == numeroSS))
                throw new ArgumentException("Un client avec ce numéro de sécurité sociale existe déjà");

            Client client = new Client(numeroSS, nom, prenom, dateNaissance, adresse);
            clients.Add(client);
            SauvegarderClients();
        }

        /// <summary>
        /// Supprime un client par son numéro de sécurité sociale.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale du client à supprimer.</param>
        /// <returns>True si le client a été supprimé, False sinon.</returns>
        public bool SupprimerClient(string numeroSS)
        {
            Client? client = clients.FirstOrDefault(c => c.NumeroSS == numeroSS);
            if (client != null)
            {
                clients.Remove(client);
                SauvegarderClients();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recherche un client par son numéro de sécurité sociale.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale à rechercher.</param>
        /// <returns>Le client trouvé ou null si aucun client ne correspond.</returns>
        public Client? RechercherClient(string numeroSS)
        {
            return clients.FirstOrDefault(c => c.NumeroSS == numeroSS);
        }

        /// <summary>
        /// Méthode pour obtenir tous les éléments d'une collection.
        /// </summary>
        /// <typeparam name="T">Le type d'éléments dans la collection.</typeparam>
        /// <param name="collection">La collection source.</param>
        /// <returns>Une nouvelle liste contenant tous les éléments.</returns>
        public List<T> ObtenirTousLesElements<T>(List<T> collection)
        {
            return new List<T>(collection);
        }

        /// <summary>
        /// Retourne tous les clients enregistrés.
        /// </summary>
        /// <returns>Une nouvelle liste contenant tous les clients.</returns>
        public List<Client> ObtenirTousLesClients()
        {
            return ObtenirTousLesElements(clients);
        }

        /// <summary>
        /// Met à jour les informations d'un client.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale du client.</param>
        /// <param name="nom">Le nouveau nom.</param>
        /// <param name="adresse">La nouvelle adresse.</param>
        /// <param name="email">Le nouvel email.</param>
        /// <param name="telephone">Le nouveau numéro de téléphone.</param>
        /// <exception cref="ArgumentException">Levée si le client n'est pas trouvé.</exception>
        public void MettreAJourClient(string numeroSS, string nom, string adresse, string email, string telephone)
        {
            Client? client = RechercherClient(numeroSS);
            if (client != null)
            {
                client.Nom = nom;
                client.Adresse = adresse;
                client.Email = email;
                client.Telephone = telephone;
                SauvegarderClients();
            }
            else
            {
                throw new ArgumentException($"Client avec le N°SS {numeroSS} non trouvé");
            }
        }

        /// <summary>
        /// Associe une commande à un client.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale du client.</param>
        /// <param name="commande">La commande à associer.</param>
        /// <exception cref="ArgumentException">Levée si le client n'est pas trouvé.</exception>
        public void AssocierCommande(string numeroSS, Commande commande)
        {
            Client? client = RechercherClient(numeroSS);
            if (client != null)
            {
                client.AjouterCommande(commande);
                SauvegarderClients();
            }
            else
            {
                throw new ArgumentException($"Client avec le N°SS {numeroSS} non trouvé");
            }
        }
    }

    /// <summary>
    /// Structure de données pour la sérialisation JSON des clients.
    /// </summary>
    public class ClientsData
    {
        /// <summary>
        /// Obtient ou définit la liste des clients.
        /// </summary>
        public List<Client> Clients { get; set; } = new List<Client>();
    }
}
