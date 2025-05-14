using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Globalization;

namespace Projet.Modules
{
    /// <summary>
    /// Module simplifié de gestion financière pour TransConnect
    /// </summary>
    public class FinanceSimple
    {
        private readonly string jsonFilePath;
        private double solde;
        private List<Transaction> transactions;
        private readonly CommandeManager commandeManager;
        private readonly SalarieManager salarieManager;

        /// <summary>
        /// Initialise une nouvelle instance du module de gestion financière
        /// </summary>
        /// <param name="jsonFilePath">Chemin du fichier JSON pour stocker les données</param>
        /// <param name="commandeManager">Gestionnaire de commandes</param>
        /// <param name="salarieManager">Gestionnaire de salariés</param>
        public FinanceSimple(string jsonFilePath = null, CommandeManager commandeManager = null, SalarieManager salarieManager = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.jsonFilePath = jsonFilePath ?? Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "finance.json");
            this.transactions = new List<Transaction>();
            this.commandeManager = commandeManager ?? new CommandeManager();
            this.salarieManager = salarieManager ?? new SalarieManager();

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

            ChargerDonnees();
        }

        /// <summary>
        /// Charge les données financières depuis le fichier JSON
        /// </summary>
        private void ChargerDonnees()
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    solde = 150000;
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            Date = DateTime.Now.AddDays(-30),
                            Montant = 150000,
                            Type = "Crédit",
                            Description = "Capital initial",
                            Categorie = "Capital"
                        }
                    };

                    SauvegarderDonnees();
                    return;
                }

                string json = File.ReadAllText(jsonFilePath);
                DonneesFinancieres donnees = JsonSerializer.Deserialize<DonneesFinancieres>(json);

                if (donnees != null)
                {
                    solde = donnees.Solde;
                    transactions = donnees.Transactions ?? new List<Transaction>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données financières: {ex.Message}");
                
                solde = 0;
                transactions = new List<Transaction>();
            }
        }

        /// <summary>
        /// Sauvegarde les données financières dans le fichier JSON
        /// </summary>
        private void SauvegarderDonnees()
        {
            try
            {
                string directory = Path.GetDirectoryName(jsonFilePath);
                if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                DonneesFinancieres donnees = new DonneesFinancieres
                {
                    Solde = solde,
                    Transactions = transactions
                };

                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(donnees, options);
                File.WriteAllText(jsonFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des données financières: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle transaction financière
        /// </summary>
        /// <param name="montant">Montant de la transaction</param>
        /// <param name="type">Type de transaction (crédit ou débit)</param>
        /// <param name="description">Description de la transaction</param>
        /// <param name="categorie">Catégorie de la transaction</param>
        public void AjouterTransaction(double montant, string type, string description, string categorie = "Divers")
        {
            Transaction transaction = new Transaction
            {
                Date = DateTime.Now,
                Montant = Math.Abs(montant),
                Type = type,
                Description = description,
                Categorie = categorie
            };

            if (type.ToLower() == "crédit" || type.ToLower() == "credit")
            {
                solde += transaction.Montant;
            }
            else
            {
                solde -= transaction.Montant;
            }

            transactions.Add(transaction);

            SauvegarderDonnees();
        }

        /// <summary>
        /// Affiche le tableau de bord financier avec les informations principales
        /// </summary>
        public void AfficherTableauDeBord()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Tableau de Bord Financier");

            Console.WriteLine($"Solde actuel: {solde:N2} €");

            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            double revenusMois = transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit"))
                .Sum(t => t.Montant);

            double depensesMois = transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit"))
                .Sum(t => t.Montant);

            Console.WriteLine($"Revenus du mois: {revenusMois:N2} €");
            Console.WriteLine($"Dépenses du mois: {depensesMois:N2} €");
            Console.WriteLine($"Bilan du mois: {revenusMois - depensesMois:N2} €");

            Console.WriteLine("\nDernières transactions:");
            foreach (Transaction transaction in transactions.OrderByDescending(t => t.Date).Take(5))
            {
                string typeSymbole = transaction.Type.ToLower() == "crédit" || transaction.Type.ToLower() == "credit" ? "+" : "-";
                ConsoleColor couleur = transaction.Type.ToLower() == "crédit" || transaction.Type.ToLower() == "credit" ? ConsoleColor.Green : ConsoleColor.Red;

                Console.ForegroundColor = couleur;
                Console.WriteLine($"{transaction.Date:dd/MM/yyyy} | {typeSymbole}{transaction.Montant:N2} € | {transaction.Description} | {transaction.Categorie}");
                Console.ResetColor();
            }

            AfficherStatistiquesParCategorie();
        }

        /// <summary>
        /// Affiche les statistiques financières par catégorie
        /// </summary>
        private void AfficherStatistiquesParCategorie()
        {
            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var revenusParCategorie = transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit"))
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant)
                .ToList();

            var depensesParCategorie = transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit"))
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant)
                .ToList();

            Console.WriteLine("\nRevenus du mois par catégorie:");
            if (revenusParCategorie.Any())
            {
                foreach (dynamic revenu in revenusParCategorie)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{revenu.Categorie}: {revenu.Montant:N2} €");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("Aucun revenu ce mois-ci.");
            }

            Console.WriteLine("\nDépenses du mois par catégorie:");
            if (depensesParCategorie.Any())
            {
                foreach (dynamic depense in depensesParCategorie)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{depense.Categorie}: {depense.Montant:N2} €");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("Aucune dépense ce mois-ci.");
            }
        }

        /// <summary>
        /// Affiche l'historique complet des transactions
        /// </summary>
        public void AfficherHistorique()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Historique des Transactions");

            Console.WriteLine("Date       | Type   | Montant      | Catégorie     | Description");
            Console.WriteLine("---------- | ------ | ------------ | ------------- | -----------");

            foreach (Transaction transaction in transactions.OrderByDescending(t => t.Date))
            {
                ConsoleColor couleur = transaction.Type.ToLower() == "crédit" || transaction.Type.ToLower() == "credit" ? ConsoleColor.Green : ConsoleColor.Red;

                Console.ForegroundColor = couleur;
                Console.WriteLine($"{transaction.Date:dd/MM/yyyy} | {transaction.Type} | {transaction.Montant:N2} € | {transaction.Categorie,-13} | {transaction.Description}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Synchronise les transactions avec les commandes existantes
        /// </summary>
        public void SynchroniserAvecCommandes()
        {
            List<Commande> commandes = commandeManager.GetToutesLesCommandes();
            int compteur = 0;

            foreach (Commande commande in commandes)
            {
                bool transactionExiste = transactions.Any(t =>
                    t.Type.ToLower() == "crédit" &&
                    t.Description.Contains($"Commande #{commande.Id}"));

                if (!transactionExiste)
                {
                    AjouterTransaction(
                        commande.Prix,
                        "Crédit",
                        $"Commande #{commande.Id} - {commande.VilleDepart.Nom} à {commande.VilleArrivee.Nom}",
                        "Transport"
                    );
                    compteur++;
                }
            }

            Console.WriteLine($"{compteur} nouvelles transactions créées à partir des commandes.");
        }

        /// <summary>
        /// Génère les transactions de salaires pour le mois en cours
        /// </summary>
        public void GenererTransactionsSalaires()
        {
            List<Salarie> salaries = salarieManager.GetTousLesSalaries();
            DateTime maintenant = DateTime.Now;
            string moisAnnee = $"{maintenant:MM/yyyy}";

            bool salairesMoisDejaPayes = transactions.Any(t =>
                t.Type.ToLower() == "débit" &&
                t.Categorie == "Salaires" &&
                t.Description.Contains(moisAnnee));

            if (!salairesMoisDejaPayes)
            {
                double totalSalaires = 0;

                foreach (Salarie salarie in salaries)
                {
                    totalSalaires += (double)salarie.Salaire;
                }

                if (totalSalaires > 0)
                {
                    AjouterTransaction(
                        totalSalaires,
                        "Débit",
                        $"Paiement des salaires - {moisAnnee}",
                        "Salaires"
                    );

                    Console.WriteLine($"Transaction de salaires générée pour {salaries.Count} employés: {totalSalaires:N2} €");
                }
            }
            else
            {
                Console.WriteLine($"Les salaires pour {moisAnnee} ont déjà été payés.");
            }
        }

        /// <summary>
        /// Menu pour générer des rapports financiers
        /// </summary>
        public void GenererRapports()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Rapports Financiers");

            Console.WriteLine("1. Rapport mensuel");
            Console.WriteLine("2. Rapport trimestriel");
            Console.WriteLine("3. Rapport annuel");
            Console.WriteLine("4. Rapport personnalisé");
            Console.WriteLine("0. Retour");
            Console.Write("\nVotre choix : ");

            switch (Console.ReadLine())
            {
                case "1":
                    GenererRapportPeriode(DateTime.Now.AddMonths(-1), DateTime.Now);
                    break;
                case "2":
                    GenererRapportPeriode(DateTime.Now.AddMonths(-3), DateTime.Now);
                    break;
                case "3":
                    GenererRapportPeriode(DateTime.Now.AddYears(-1), DateTime.Now);
                    break;
                case "4":
                    Console.Write("\nDate de début (JJ/MM/AAAA) : ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime debut))
                    {
                        Console.Write("Date de fin (JJ/MM/AAAA) : ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime fin))
                        {
                            GenererRapportPeriode(debut, fin);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Génère un rapport financier pour une période donnée
        /// </summary>
        /// <param name="debut">Date de début de la période</param>
        /// <param name="fin">Date de fin de la période</param>
        private void GenererRapportPeriode(DateTime debut, DateTime fin)
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre($"Rapport du {debut:dd/MM/yyyy} au {fin:dd/MM/yyyy}");

            List<Transaction> transactionsPeriode = transactions
                .Where(t => t.Date >= debut && t.Date <= fin)
                .ToList();

            double revenus = transactionsPeriode
                .Where(t => t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit")
                .Sum(t => t.Montant);

            double depenses = transactionsPeriode
                .Where(t => t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit")
                .Sum(t => t.Montant);

            double benefice = revenus - depenses;

            Console.WriteLine($"Revenus totaux: {revenus:N2} €");
            Console.WriteLine($"Dépenses totales: {depenses:N2} €");
            Console.WriteLine($"Bénéfice: {benefice:N2} €");

            IEnumerable<dynamic> revenusParCategorie = transactionsPeriode
                .Where(t => t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit")
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant);

            Console.WriteLine("\nRevenus par catégorie:");
            foreach (dynamic revenu in revenusParCategorie)
            {
                Console.WriteLine($"{revenu.Categorie}: {revenu.Montant:N2} €");
            }
            IEnumerable<dynamic> depensesParCategorie = transactionsPeriode
                .Where(t => t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit")
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant);

            Console.WriteLine("\nDépenses par catégorie:");
            foreach (dynamic depense in depensesParCategorie)
            {
                Console.WriteLine($"{depense.Categorie}: {depense.Montant:N2} €");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Structure pour stocker les données financières
    /// </summary>
    public class DonneesFinancieres
    {
        /// <summary>
        /// Solde actuel du compte
        /// </summary>
        public double Solde { get; set; }

        /// <summary>
        /// Liste des transactions
        /// </summary>
        public List<Transaction> Transactions { get; set; }
    }

    /// <summary>
    /// Représente une transaction financière
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Date de la transaction
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Montant de la transaction
        /// </summary>
        public double Montant { get; set; }

        /// <summary>
        /// Type de transaction (Crédit ou Débit)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Description de la transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Catégorie de la transaction
        /// </summary>
        public string Categorie { get; set; } = "Divers";
    }
}
