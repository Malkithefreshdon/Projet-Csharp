using System;
using Projet.Modules;

namespace Projet.Modules
{
    public class MenuPrincipal
    {
        private readonly SousMenus _sousMenus;

        public MenuPrincipal(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager, StatistiqueService statistiqueService)
        {
            _sousMenus = new SousMenus(clientManager, commandeManager, salarieManager, statistiqueService);
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Menu Principal");
                Console.WriteLine("1. Gestion des Clients");
                Console.WriteLine("2. Gestion des Commandes");
                Console.WriteLine("3. Gestion des Salariés");
                Console.WriteLine("4. Gestion des Graphes");
                Console.WriteLine("5. Gestion des Véhicules");
                Console.WriteLine("6. Statistiques");
                Console.WriteLine("7. Quitter");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        _sousMenus.AfficherMenuClients();
                        break;
                    case "2":
                        _sousMenus.AfficherMenuCommandes();
                        break;
                    case "3":
                        _sousMenus.AfficherMenuSalaries();
                        break;
                    case "4":
                        _sousMenus.AfficherMenuGraphes();
                        break;
                    case "5":
                        _sousMenus.AfficherMenuVehicules();
                        break;
                    case "6":
                        _sousMenus.AfficherMenuStatistiques();
                        break;
                    case "7":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}