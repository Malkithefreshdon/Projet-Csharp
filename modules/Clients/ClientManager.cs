using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Projet.Modules
{
    public class ClientManager
    {
        private List<Client> _clients;
        private readonly string _jsonPath = "ressources/clients.json";

        public ClientManager()
        {
            ChargerClients();
        }

        public void ChargerClients()
        {
            if (File.Exists(_jsonPath))
            {
                var jsonString = File.ReadAllText(_jsonPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var clientsData = JsonSerializer.Deserialize<ClientsData>(jsonString, options);
                _clients = clientsData.Clients;
            }
            else
            {
                _clients = new List<Client>();
                SauvegarderClients();
            }
        }

        public void SauvegarderClients()
        {
            var clientsData = new ClientsData { Clients = _clients };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(clientsData, options);
            File.WriteAllText(_jsonPath, jsonString);
        }

        public void AjouterClient(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adresse)
        {
            if (string.IsNullOrEmpty(numeroSS) || string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(adresse))
                throw new ArgumentException("Les champs obligatoires ne peuvent pas être vides");

            if (_clients.Any(c => c.NumeroSS == numeroSS))
                throw new ArgumentException("Un client avec ce numéro de sécurité sociale existe déjà");

            var client = new Client(numeroSS, nom, prenom, dateNaissance, adresse);
            _clients.Add(client);
            SauvegarderClients();
        }

        public bool SupprimerClient(string numeroSS)
        {
            var client = _clients.FirstOrDefault(c => c.NumeroSS == numeroSS);
            if (client != null)
            {
                _clients.Remove(client);
                SauvegarderClients();
                return true;
            }
            return false;
        }

        public Client RechercherClient(string numeroSS)
        {
            return _clients.FirstOrDefault(c => c.NumeroSS == numeroSS);
        }

        public List<Client> ObtenirTousLesClients()
        {
            return new List<Client>(_clients);
        }

        public void MettreAJourClient(string numeroSS, string nom, string adresse, string email, string telephone)
        {
            var client = RechercherClient(numeroSS);
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

        public void AssocierCommande(string numeroSS, Commande commande)
        {
            var client = RechercherClient(numeroSS);
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

    public class ClientsData
    {
        public List<Client> Clients { get; set; }
    }
}
