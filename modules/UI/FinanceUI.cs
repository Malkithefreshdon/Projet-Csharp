using System;
using Projet.Modules;

namespace Projet.Modules.UI
{
    /// <summary>
    /// Interface utilisateur pour la gestion financière
    /// </summary>
    public class FinanceUI
    {
        private readonly FinanceSimple financeService;

        /// <summary>
        /// Initialise une nouvelle instance de l'interface utilisateur financière
        /// </summary>
        /// <param name="commandeManager">Gestionnaire de commandes</param>
        /// <param name="salarieManager">Gestionnaire de salariés</param>
        public FinanceUI(CommandeManager commandeManager, SalarieManager salarieManager)
        {
            financeService = new FinanceSimple(null, commandeManager, salarieManager);
        }

        /// <summary>
        /// Affiche le menu principal de gestion financière
        /// </summary>
        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion Financière");
                Console.WriteLine("1. Tableau de bord financier");
                Console.WriteLine("2. Historique des transactions");
                Console.WriteLine("3. Ajouter une entrée d'argent");
                Console.WriteLine("4. Ajouter une sortie d'argent");
                Console.WriteLine("5. Synchroniser avec les commandes");
                Console.WriteLine("6. Générer transactions de salaires");
                Console.WriteLine("7. Générer rapports financiers");
                Console.WriteLine("0. Retour");
                Console.Write("\nVotre choix : ");

                switch (Console.ReadLine())
                {
                    case "1":
                        financeService.AfficherTableauDeBord();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                    case "2":
                        financeService.AfficherHistorique();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                    case "3":
                        AjouterEntreeArgent();
                        break;
                    case "4":
                        AjouterSortieArgent();
                        break;
                    case "5":
                        financeService.SynchroniserAvecCommandes();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                    case "6":
                        financeService.GenererTransactionsSalaires();
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                    case "7":
                        financeService.GenererRapports();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Interface pour ajouter une entrée d'argent
        /// </summary>
        private void AjouterEntreeArgent()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajouter une Entrée d'Argent");

            Console.Write("Montant (€) : ");
            if (!double.TryParse(Console.ReadLine(), out double montant) || montant <= 0)
            {
                Console.WriteLine("Montant invalide. Opération annulée.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nCatégorie :");
            Console.WriteLine("1. Transport");
            Console.WriteLine("2. Vente de véhicule");
            Console.WriteLine("3. Subvention");
            Console.WriteLine("4. Autre");
            Console.Write("\nVotre choix : ");

            string categorie;
            switch (Console.ReadLine())
            {
                case "1": categorie = "Transport"; break;
                case "2": categorie = "Vente de véhicule"; break;
                case "3": categorie = "Subvention"; break;
                case "4":
                    Console.Write("Précisez la catégorie : ");
                    categorie = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(categorie))
                        categorie = "Divers";
                    break;
                default: categorie = "Divers"; break;
            }

            Console.Write("Description : ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = $"Entrée d'argent - {categorie}";
            }

            financeService.AjouterTransaction(montant, "Crédit", description, categorie);

            Console.WriteLine($"\nTransaction de {montant:N2} € ajoutée avec succès!");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// <summary>
        /// Interface pour ajouter une sortie d'argent
        /// </summary>
        private void AjouterSortieArgent()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajouter une Sortie d'Argent");

            Console.Write("Montant (€) : ");
            if (!double.TryParse(Console.ReadLine(), out double montant) || montant <= 0)
            {
                Console.WriteLine("Montant invalide. Opération annulée.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nCatégorie :");
            Console.WriteLine("1. Salaires");
            Console.WriteLine("2. Carburant");
            Console.WriteLine("3. Maintenance");
            Console.WriteLine("4. Achat de véhicule");
            Console.WriteLine("5. Assurance");
            Console.WriteLine("6. Taxes");
            Console.WriteLine("7. Autre");
            Console.Write("\nVotre choix : ");

            string categorie;
            switch (Console.ReadLine())
            {
                case "1": categorie = "Salaires"; break;
                case "2": categorie = "Carburant"; break;
                case "3": categorie = "Maintenance"; break;
                case "4": categorie = "Achat de véhicule"; break;
                case "5": categorie = "Assurance"; break;
                case "6": categorie = "Taxes"; break;
                case "7":
                    Console.Write("Précisez la catégorie : ");
                    categorie = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(categorie))
                        categorie = "Divers";
                    break;
                default: categorie = "Divers"; break;
            }

            Console.Write("Description : ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = $"Sortie d'argent - {categorie}";
            }

            financeService.AjouterTransaction(montant, "Débit", description, categorie);

            Console.WriteLine($"\nTransaction de {montant:N2} € ajoutée avec succès!");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
}
