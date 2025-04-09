using System;
using Projet.Modules.Clients;
using Projet.Modules.Commandes;
using Projet.Modules.Statistiques;

namespace Projet.UI
{
    public class MenuPrincipal
    {
        private readonly ClientManager _clientManager;
        private readonly CommandeManager _commandeManager;
        private readonly StatistiqueService _statistiqueService;
        private readonly SousMenus _sousMenus;

        public MenuPrincipal(ClientManager clientManager, CommandeManager commandeManager, StatistiqueService statistiqueService)
        {
            _clientManager = clientManager;
            _commandeManager = commandeManager;
            _statistiqueService = statistiqueService;
            _sousMenus = new SousMenus(clientManager, commandeManager, statistiqueService);
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Menu Principal");
                Console.WriteLine("1. Gestion des clients");
                Console.WriteLine("2. Gestion des commandes");
                Console.WriteLine("3. Statistiques");
                Console.WriteLine("4. Quitter");
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
                        _sousMenus.AfficherMenuStatistiques();
                        break;
                    case "4":
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