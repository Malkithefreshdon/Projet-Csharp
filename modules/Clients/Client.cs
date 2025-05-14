using System;
using System.Collections.Generic;

namespace Projet.Modules
{
    /// <summary>
    /// Représente un client de l'entreprise TransConnect.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Obtient le numéro de sécurité sociale du client.
        /// </summary>
        public string NumeroSS { get;  set; }

        /// <summary>
        /// Obtient ou définit le nom du client.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Obtient le prénom du client.
        /// </summary>
        public string Prenom { get;  set; }

        /// <summary>
        /// Obtient la date de naissance du client.
        /// </summary>
        public DateTime DateNaissance { get;  set; }

        /// <summary>
        /// Obtient ou définit l'adresse du client.
        /// </summary>
        public string Adresse { get; set; }

        /// <summary>
        /// Obtient ou définit l'email du client. Peut être null si non renseigné.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Obtient ou définit le numéro de téléphone du client. Peut être null si non renseigné.
        /// </summary>
        public string? Telephone { get; set; }

        /// <summary>
        /// Obtient la liste des commandes effectuées par le client.
        /// </summary>
        public List<Commande> HistoriqueCommandes { get;  set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Client.
        /// </summary>
        /// <param name="numeroSS">Le numéro de sécurité sociale du client.</param>
        /// <param name="nom">Le nom du client.</param>
        /// <param name="prenom">Le prénom du client.</param>
        /// <param name="dateNaissance">La date de naissance du client.</param>
        /// <param name="adresse">L'adresse du client.</param>
        public Client(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adresse)
        {
            NumeroSS = numeroSS;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            Adresse = adresse;
            HistoriqueCommandes = new List<Commande>();
        }

        /// <summary>
        /// Ajoute une commande à l'historique du client.
        /// </summary>
        /// <param name="commande">La commande à ajouter.</param>
        public void AjouterCommande(Commande commande)
        {
            HistoriqueCommandes.Add(commande);
        }

        /// <summary>
        /// Retourne une représentation textuelle du client.
        /// </summary>
        /// <returns>Une chaîne de caractères décrivant le client.</returns>
        public override string ToString()
        {
            return $"Client: {Nom} {Prenom}\n" +
                   $"N°SS: {NumeroSS}\n" +
                   $"Date de naissance: {DateNaissance.ToShortDateString()}\n" +
                   $"Adresse: {Adresse}\n" +
                   $"Email: {Email ?? "Non renseigné"}\n" +
                   $"Téléphone: {Telephone ?? "Non renseigné"}\n" +
                   $"Nombre de commandes: {HistoriqueCommandes.Count}";
        }
    }
}
