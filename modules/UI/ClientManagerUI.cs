using System;
using System.Linq;
using System.Collections.Generic;
using Projet.Modules;

namespace Projet.Modules.UI
{
    /// <summary>
    /// Interface utilisateur pour la gestion des clients
    /// </summary>
    public class ClientManagerUI
    {
        private readonly ClientManager clientManager;

        /// <summary>
        /// Initialise une nouvelle instance de la classe ClientManagerUI
        /// </summary>
        /// <param name="clientManager">Le gestionnaire de clients à utiliser</param>
        public ClientManagerUI(ClientManager clientManager)
        {
            this.clientManager = clientManager;
        }

        private class ClientMontant
        {
            public Client Client { get; set; }
            public double MontantTotal { get; set; }
        }

        /// <summary>
        /// Affiche le menu principal de gestion des clients
        /// </summary>
        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Clients");
                Console.WriteLine("1. Ajouter un client");
                Console.WriteLine("2. Modifier un client");
                Console.WriteLine("3. Supprimer un client");
                Console.WriteLine("4. Rechercher un client");
                Console.WriteLine("5. Afficher tous les clients");
                Console.WriteLine("6. Afficher les clients par ordre alphabétique");
                Console.WriteLine("7. Afficher les clients par ville");
                Console.WriteLine("8. Afficher les clients par montant d'achats cumulés");
                Console.WriteLine("9. Importer des clients depuis un fichier CSV");
                Console.WriteLine("10. Sauvegarder les modifications");
                Console.WriteLine("11. Recharger les données");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                string choix = Console.ReadLine() ?? "";
                switch (choix)
                {
                    case "1":
                        AjouterClient();
                        break;
                    case "2":
                        ModifierClient();
                        break;
                    case "3":
                        SupprimerClient();
                        break;
                    case "4":
                        RechercherClient();
                        break;
                    case "5":
                        AfficherTousLesClients();
                        break;
                    case "6":
                        AfficherClientsParOrdreAlphabetique();
                        break;
                    case "7":
                        AfficherClientsParVille();
                        break;
                    case "8":
                        AfficherClientsParMontantAchats();
                        break;
                    case "9":
                        ImporterClientsDepuisCSV();
                        break;
                    case "10":
                        try
                        {
                            clientManager.SauvegarderClients();
                            Console.WriteLine("Les modifications ont été sauvegardées avec succès.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
                        }
                        Console.ReadKey();
                        break;
                    case "11":
                        try
                        {
                            clientManager.ChargerClients();
                            Console.WriteLine("Les données ont été rechargées avec succès.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur lors du rechargement : {ex.Message}");
                        }
                        Console.ReadKey();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Ajoute un nouveau client au système
        /// </summary>
        private void AjouterClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un client");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(numeroSS))
            {
                Console.WriteLine("Le numéro de sécurité sociale ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            if (clientManager.RechercherClient(numeroSS) != null)
            {
                Console.WriteLine("Un client avec ce numéro de sécurité sociale existe déjà.");
                Console.ReadKey();
                return;
            }

            Console.Write("Nom : ");
            string nom = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine("Le nom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(prenom))
            {
                Console.WriteLine("Le prénom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateNaissance))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Adresse : ");
            string adresse = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(adresse))
            {
                Console.WriteLine("L'adresse ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            try
            {
                clientManager.AjouterClient(numeroSS, nom, prenom, dateNaissance, adresse);

                Console.Write("Email (optionnel) : ");
                string email = Console.ReadLine() ?? "";

                Console.Write("Téléphone (optionnel) : ");
                string telephone = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(telephone))
                {
                    clientManager.MettreAJourClient(numeroSS, nom, adresse, email, telephone);
                }

                Console.WriteLine("\nClient ajouté avec succès !");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du client : {ex.Message}");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Modifie les informations d'un client existant
        /// </summary>
        private void ModifierClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Modification d'un client");

            Console.Write("Numéro de sécurité sociale du client à modifier : ");
            string numeroSS = Console.ReadLine() ?? "";

            Client? client = clientManager.RechercherClient(numeroSS);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Modification du client : {client.Nom} {client.Prenom}");

            Console.Write($"Nouveau nom [{client.Nom}] (laisser vide pour ne pas modifier) : ");
            string nouveauNom = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nouveauNom))
            {
                nouveauNom = client.Nom;
            }

            Console.Write($"Nouvelle adresse [{client.Adresse}] (laisser vide pour ne pas modifier) : ");
            string nouvelleAdresse = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nouvelleAdresse))
            {
                nouvelleAdresse = client.Adresse;
            }

            Console.Write($"Nouvel email [{client.Email}] (laisser vide pour ne pas modifier) : ");
            string nouvelEmail = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nouvelEmail))
            {
                nouvelEmail = client.Email;
            }

            Console.Write($"Nouveau téléphone [{client.Telephone}] (laisser vide pour ne pas modifier) : ");
            string nouveauTelephone = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nouveauTelephone))
            {
                nouveauTelephone = client.Telephone;
            }

            try
            {
                clientManager.MettreAJourClient(numeroSS, nouveauNom, nouvelleAdresse, nouvelEmail, nouveauTelephone);
                Console.WriteLine("Client modifié avec succès !");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification du client : {ex.Message}");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Supprime un client du système
        /// </summary>
        private void SupprimerClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un client");

            Console.Write("Numéro de sécurité sociale du client à supprimer : ");
            string numeroSS = Console.ReadLine() ?? "";

            Client? client = clientManager.RechercherClient(numeroSS);
            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Êtes-vous sûr de vouloir supprimer le client {client.Nom} {client.Prenom} ? (O/N)");
            string confirmation = Console.ReadLine()?.ToUpper() ?? "";

            if (confirmation == "O")
            {
                if (clientManager.SupprimerClient(numeroSS))
                {
                    Console.WriteLine("Client supprimé avec succès !");
                }
                else
                {
                    Console.WriteLine("Échec de la suppression du client.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annulée.");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche un client dans le système
        /// </summary>
        private void RechercherClient()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche d'un client");

            Console.WriteLine("1. Rechercher par numéro de sécurité sociale");
            Console.WriteLine("2. Rechercher par nom");
            Console.Write("\nVotre choix : ");

            string choix = Console.ReadLine() ?? "";

            switch (choix)
            {
                case "1":
                    RechercherClientParNumeroSS();
                    break;
                case "2":
                    RechercherClientParNom();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    break;
            }
        }

        /// <summary>
        /// Recherche un client par son numéro de sécurité sociale
        /// </summary>
        private void RechercherClientParNumeroSS()
        {
            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine() ?? "";

            Client? client = clientManager.RechercherClient(numeroSS);
            if (client != null)
            {
                Console.WriteLine("\nClient trouvé :");
                AfficherDetailsClient(client);
            }
            else
            {
                Console.WriteLine("\nClient non trouvé !");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Recherche un client par son nom
        /// </summary>
        private void RechercherClientParNom()
        {
            Console.Write("Nom du client : ");
            string nom = Console.ReadLine() ?? "";

            List<Client> clients = clientManager.ObtenirTousLesClients()
                .Where(c => c.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (clients.Any())
            {
                Console.WriteLine($"\n{clients.Count} client(s) trouvé(s) :");
                foreach (Client client in clients)
                {
                    AfficherDetailsClient(client);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun client trouvé avec ce nom !");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Affiche tous les clients enregistrés
        /// </summary>
        private void AfficherTousLesClients()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste de tous les clients");

            List<Client> clients = clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            foreach (Client client in clients)
            {
                AfficherDetailsClient(client);
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine($"Total : {clients.Count} client(s)");
            Console.ReadKey();
        }

        /// <summary>
        /// Affiche les clients par ordre alphabétique
        /// </summary>
        private void AfficherClientsParOrdreAlphabetique()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par ordre alphabétique");

            List<Client> clients = clientManager.ObtenirTousLesClients()
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToList();

            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            foreach (Client client in clients)
            {
                AfficherDetailsClient(client);
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine($"Total : {clients.Count} client(s)");
            Console.ReadKey();
        }

        /// <summary>
        /// Affiche les clients regroupés par ville
        /// </summary>
        private void AfficherClientsParVille()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par ville");

            List<Client> clients = clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            IEnumerable<IGrouping<string, Client>> clientsParVille = clients
                .GroupBy(c => ExtraireVille(c.Adresse))
                .OrderBy(g => g.Key);

            foreach (IGrouping<string, Client> groupe in clientsParVille)
            {
                Console.WriteLine($"\n=== VILLE : {groupe.Key} ===");

                foreach (Client client in groupe.OrderBy(c => c.Nom).ThenBy(c => c.Prenom))
                {
                    AfficherDetailsClient(client);
                    Console.WriteLine("-----------------------------------");
                }

                Console.WriteLine($"Total pour {groupe.Key} : {groupe.Count()} client(s)");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Affiche les clients triés par montant d'achats cumulés
        /// </summary>
        private void AfficherClientsParMontantAchats()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Clients par montant d'achats cumulés");

            List<Client> clients = clientManager.ObtenirTousLesClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                Console.ReadKey();
                return;
            }

            List<ClientMontant> clientsAvecMontant = clients
                .Select(c => new ClientMontant
                {
                    Client = c,
                    MontantTotal = c.HistoriqueCommandes.Sum(cmd => cmd.Prix)
                })
                .OrderByDescending(x => x.MontantTotal)
                .ToList();

            foreach (ClientMontant item in clientsAvecMontant)
            {
                Console.WriteLine($"Montant total d'achats : {item.MontantTotal:C2}");
                AfficherDetailsClient(item.Client);
                Console.WriteLine("-----------------------------------");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Importe des clients depuis un fichier CSV
        /// </summary>
        private void ImporterClientsDepuisCSV()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Importation de clients depuis un fichier CSV");

            Console.Write("Chemin du fichier CSV : ");
            string cheminFichier = Console.ReadLine() ?? "";

            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Le fichier spécifié n'existe pas.");
                Console.ReadKey();
                return;
            }

            try
            {
                int compteur = 0;
                string[] lignes = File.ReadAllLines(cheminFichier);

                bool premiereEstEntete = lignes.Length > 0 &&
                                        (lignes[0].Contains("Nom") ||
                                         lignes[0].Contains("NumeroSS") ||
                                         lignes[0].Contains("Prenom"));

                int debutLecture = premiereEstEntete ? 1 : 0;

                for (int i = debutLecture; i < lignes.Length; i++)
                {
                    string ligne = lignes[i];
                    if (string.IsNullOrWhiteSpace(ligne)) continue;

                    string[] colonnes = ligne.Split(';');
                    if (colonnes.Length < 5)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Format incorrect, ignorée.");
                        continue;
                    }

                    string numeroSS = colonnes[0].Trim();
                    string nom = colonnes[1].Trim();
                    string prenom = colonnes[2].Trim();

                    if (!DateTime.TryParse(colonnes[3].Trim(), out DateTime dateNaissance))
                    {
                        Console.WriteLine($"Ligne {i + 1} : Format de date incorrect, ignorée.");
                        continue;
                    }

                    string adresse = colonnes[4].Trim();

                    if (clientManager.RechercherClient(numeroSS) != null)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Client avec NumeroSS {numeroSS} existe déjà, ignoré.");
                        continue;
                    }

                    try
                    {
                        clientManager.AjouterClient(numeroSS, nom, prenom, dateNaissance, adresse);

                        if (colonnes.Length > 5)
                        {
                            string email = colonnes[5].Trim();
                            string telephone = colonnes.Length > 6 ? colonnes[6].Trim() : "";

                            clientManager.MettreAJourClient(numeroSS, nom, adresse, email, telephone);
                        }

                        compteur++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ligne {i + 1} : Erreur lors de l'ajout du client : {ex.Message}");
                    }
                }

                Console.WriteLine($"{compteur} client(s) importé(s) avec succès.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation du fichier : {ex.Message}");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Affiche les détails d'un client
        /// </summary>
        /// <param name="client">Le client dont les détails doivent être affichés</param>
        private void AfficherDetailsClient(Client client)
        {
            if (client == null) return;

            double montantTotal = client.HistoriqueCommandes.Sum(c => c.Prix);

            Console.WriteLine($"Client: {client.Nom} {client.Prenom}");
            Console.WriteLine($"N°SS: {client.NumeroSS}");
            Console.WriteLine($"Date de naissance: {client.DateNaissance.ToShortDateString()}");
            Console.WriteLine($"Adresse: {client.Adresse}");
            Console.WriteLine($"Email: {client.Email ?? "Non renseigné"}");
            Console.WriteLine($"Téléphone: {client.Telephone ?? "Non renseigné"}");
            Console.WriteLine($"Nombre de commandes: {client.HistoriqueCommandes.Count}");
            Console.WriteLine($"Montant total des achats: {montantTotal:C2}");
        }

        /// <summary>
        /// Extrait la ville d'une adresse
        /// </summary>
        /// <param name="adresse">L'adresse complète</param>
        /// <returns>La ville extraite</returns>
        private string ExtraireVille(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return "Inconnue";

            string[] parties = adresse.Split(new[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parties.Length - 1; i++)
            {
                if (parties[i].All(char.IsDigit) && parties[i].Length == 5)
                {
                    return string.Join(" ", parties.Skip(i + 1));
                }
            }

            return parties.Length > 0 ? parties[parties.Length - 1] : "Inconnue";
        }
    }
}