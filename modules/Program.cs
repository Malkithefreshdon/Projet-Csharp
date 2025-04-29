using System;
using System.Diagnostics;
using OfficeOpenXml;
using Projet.Modules;
using SkiaSharp;

namespace modules
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Initialisation des managers
            var clientManager = new ClientManager();
            var commandeManager = new CommandeManager();
            var salarieManager = new SalarieManager();

            // Initialisation du service de statistiques
            var statistiqueService = new StatistiqueService(clientManager, commandeManager, salarieManager);

            // Initialisation de l'interface utilisateur
            var menuPrincipal = new MenuPrincipal(clientManager, commandeManager, salarieManager, statistiqueService);

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
