using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    public class ClientManager
    {
        private List<Client> _clients;

        public ClientManager()
        {
            _clients = new List<Client>();
        }

        public void AjouterClient(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adresse)
        {
            if (string.IsNullOrEmpty(numeroSS) || string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(adresse))
                throw new ArgumentException("Les champs obligatoires ne peuvent pas être vides");

            if (_clients.Any(c => c.NumeroSS == numeroSS))
                throw new ArgumentException("Un client avec ce numéro de sécurité sociale existe déjà");

            var client = new Client(numeroSS, nom, prenom, dateNaissance, adresse);
            _clients.Add(client);
        }

        public bool SupprimerClient(string numeroSS)
        {
            var client = _clients.FirstOrDefault(c => c.NumeroSS == numeroSS);
            if (client != null)
            {
                _clients.Remove(client);
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
            }
            else
            {
                throw new ArgumentException($"Client avec le N°SS {numeroSS} non trouvé");
            }
        }
    }
}
