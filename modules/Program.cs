using System;
using Projet.Modules.Clients;
using modules.Commandes;
using modules.Salariés;
using Projet.Modules.Statistiques;
using Projet.UI;

namespace modules
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialisation des managers
            var clientManager = new ClientManager();
            var commandeManager = new CommandeManager();
            var salarieManager = new SalarieManager();

            // Initialisation du service de statistiques
            var statistiqueService = new StatistiqueService(clientManager, commandeManager, salarieManager);

            // Initialisation de l'interface utilisateur
            var menuPrincipal = new MenuPrincipal(clientManager, commandeManager, statistiqueService);

            // Démarrage de l'application
            Console.WriteLine("Bienvenue dans le système de gestion TransConnect");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();

            // Affichage du menu principal
            menuPrincipal.AfficherMenu();

            Console.WriteLine("\nMerci d'avoir utilisé notre système. Au revoir !");
        }
    }
}
