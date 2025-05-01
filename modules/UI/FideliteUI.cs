using System;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class FideliteUI
    {
        private readonly SystemeFidelite _systemeFidelite;
        private readonly ClientManager _clientManager;

        public FideliteUI(ClientManager clientManager)
        {
            _systemeFidelite = new SystemeFidelite();
            _clientManager = clientManager;
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Programme de Fidélité");
                Console.WriteLine("1. Consulter le statut d'un client");
                Console.WriteLine("2. Simuler une remise");
                Console.WriteLine("0. Retour");
                Console.Write("\nVotre choix : ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ConsulterStatutClient();
                        break;
                    case "2":
                        SimulerRemise();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ConsulterStatutClient()
        {
            Console.Clear();
            Console.Write("Numéro de sécurité sociale du client : ");
            string numeroSS = Console.ReadLine();
            Client client = _clientManager.RechercherClient(numeroSS);

            if (client != null)
            {
                _systemeFidelite.AfficherInfos(client);
            }
            else
            {
                Console.WriteLine("Client non trouvé.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void SimulerRemise()
        {
            Console.Clear();
            Console.Write("Numéro de sécurité sociale du client : ");
            string numeroSS = Console.ReadLine();
            Client client = _clientManager.RechercherClient(numeroSS);

            if (client == null)
            {
                Console.WriteLine("Client non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.Write("Prix de la commande : ");
            if (double.TryParse(Console.ReadLine(), out double prix))
            {
                _systemeFidelite.AfficherRecapitulatif(client, prix);
            }
            else
            {
                Console.WriteLine("Prix invalide.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
} 