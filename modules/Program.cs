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

            ClientManager clientManager = new ClientManager();
            CommandeManager commandeManager = new CommandeManager();
            SalarieManager salarieManager = new SalarieManager();

            StatistiqueService statistiqueService = new StatistiqueService(clientManager, commandeManager, salarieManager);

            MenuPrincipal menuPrincipal = new MenuPrincipal(clientManager, commandeManager, salarieManager, statistiqueService);

            Console.WriteLine("Bienvenue dans le système de gestion TransConnect");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();

            menuPrincipal.AfficherMenu();

            Console.WriteLine("\nMerci d'avoir utilisé notre système. Au revoir !");
        }

    }
}
