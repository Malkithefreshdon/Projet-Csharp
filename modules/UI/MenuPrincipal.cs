using System;
using Projet.Modules;
using Projet.Modules.UI;

namespace Projet.Modules
{
    public class MenuPrincipal
    {
        private readonly ClientManagerUI _clientManagerUI;
        private readonly CommandeManagerUI _commandeManagerUI;
        private readonly SalarieManagerUI _salarieManagerUI;
        private readonly StatistiqueServiceUI _statistiqueServiceUI;
        private readonly GrapheUI _grapheUI;
        private readonly VehiculeManagerUI _vehiculeManagerUI;
        private readonly MaintenanceManagerUI _maintenanceManagerUI;
        private readonly FideliteUI _fideliteUI;
        private readonly FinanceUI _financeUI;

        public MenuPrincipal(ClientManager clientManager, CommandeManager commandeManager, SalarieManager salarieManager, StatistiqueService statistiqueService)
        {
            _clientManagerUI = new ClientManagerUI(clientManager);
            _commandeManagerUI = new CommandeManagerUI(commandeManager, clientManager, salarieManager, new VehiculeManager(), new GrapheListe(true));
            _salarieManagerUI = new SalarieManagerUI(salarieManager);
            _statistiqueServiceUI = new StatistiqueServiceUI(statistiqueService);
            _grapheUI = new GrapheUI(new GrapheListe(true), new GrapheMatrice(true));
            _vehiculeManagerUI = new VehiculeManagerUI(new VehiculeManager());
            _maintenanceManagerUI = new MaintenanceManagerUI(new MaintenanceManager(), new VehiculeManager());
            _fideliteUI = new FideliteUI(clientManager);
            _financeUI = new FinanceUI(commandeManager, salarieManager);
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
                Console.WriteLine("7. Programme de Fidélité");
                Console.WriteLine("8. Finance");
                Console.WriteLine("9. Maintenance");
                Console.WriteLine("0. Quitter");
                Console.WriteLine("\nVotre choix : ");

                string choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        _clientManagerUI.AfficherMenu();
                        break;
                    case "2":
                        _commandeManagerUI.AfficherMenu();
                        break;
                    case "3":
                        _salarieManagerUI.AfficherMenu();
                        break;
                    case "4":
                        _grapheUI.AfficherMenu();
                        break;
                    case "5":
                        _vehiculeManagerUI.AfficherMenu();
                        break;
                    case "6":
                        _statistiqueServiceUI.AfficherMenu();
                        break;
                    case "7":
                        _fideliteUI.AfficherMenu();
                        break;
                    case "8":
                        _financeUI.AfficherMenu();
                        break;
                    case "9":
                        _maintenanceManagerUI.AfficherMenu();
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
    }
}
