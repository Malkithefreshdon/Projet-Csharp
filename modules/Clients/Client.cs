using System;
using System.Collections.Generic;
using modules.Commandes;
using modules.Salariés;
namespace Projet.Modules.Clients
{
    public class Client
    {
        public string NumeroSS { get; private set; }
        public string Nom { get; set; }
        public string Prenom { get; private set; }
        public DateTime DateNaissance { get; private set; }
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public List<Commande> HistoriqueCommandes { get; private set; }

        public Client(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adresse)
        {
            NumeroSS = numeroSS;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            Adresse = adresse;
            HistoriqueCommandes = new List<Commande>();
        }

        public void AjouterCommande(Commande commande)
        {
            HistoriqueCommandes.Add(commande);
        }

        public override string ToString()
        {
            return $"Client: {Nom} {Prenom}\n" +
                   $"N°SS: {NumeroSS}\n" +
                   $"Date de naissance: {DateNaissance.ToShortDateString()}\n" +
                   $"Adresse: {Adresse}\n" +
                   $"Email: {Email}\n" +
                   $"Téléphone: {Telephone}\n" +
                   $"Nombre de commandes: {HistoriqueCommandes.Count}";
        }
    }
}
