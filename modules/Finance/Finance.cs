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
        private string _jsonFilePath;
        private double _solde;
        private List<TransactionSimple> _transactions;
        private readonly CommandeManager _commandeManager;
        private readonly SalarieManager _salarieManager;

        public FinanceSimple(string jsonFilePath = null, CommandeManager commandeManager = null, SalarieManager salarieManager = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _jsonFilePath = jsonFilePath ?? Path.Combine(baseDirectory, "Ressources", "finance.json");
            _transactions = new List<TransactionSimple>();
            _commandeManager = commandeManager ?? new CommandeManager();
            _salarieManager = salarieManager ?? new SalarieManager();

            // Définir la culture pour utiliser l'euro comme devise
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

            // Charger ou initialiser les données
            ChargerDonnees();
        }

        /// <summary>
        /// Charge les données financières depuis le fichier JSON
        /// </summary>
        private void ChargerDonnees()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                {
                    // Initialiser avec un solde par défaut
                    _solde = 150000;
                    _transactions = new List<TransactionSimple>
                    {
                        new TransactionSimple
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

                string json = File.ReadAllText(_jsonFilePath);
                var donnees = JsonSerializer.Deserialize<DonneesFinancieres>(json);

                if (donnees != null)
                {
                    _solde = donnees.Solde;
                    _transactions = donnees.Transactions ?? new List<TransactionSimple>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données financières: {ex.Message}");
                // Initialiser avec des valeurs par défaut
                _solde = 0;
                _transactions = new List<TransactionSimple>();
            }
        }

        /// <summary>
        /// Sauvegarde les données financières dans le fichier JSON
        /// </summary>
        private void SauvegarderDonnees()
        {
            try
            {
                string directory = Path.GetDirectoryName(_jsonFilePath);
                if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var donnees = new DonneesFinancieres
                {
                    Solde = _solde,
                    Transactions = _transactions
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(donnees, options);
                File.WriteAllText(_jsonFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des données financières: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle transaction
        /// </summary>
        public void AjouterTransaction(double montant, string type, string description, string categorie = "Divers")
        {
            var transaction = new TransactionSimple
            {
                Date = DateTime.Now,
                Montant = Math.Abs(montant),
                Type = type,
                Description = description,
                Categorie = categorie
            };

            // Mettre à jour le solde
            if (type.ToLower() == "crédit" || type.ToLower() == "credit")
            {
                _solde += transaction.Montant;
            }
            else
            {
                _solde -= transaction.Montant;
            }

            // Ajouter la transaction à l'historique
            _transactions.Add(transaction);

            // Sauvegarder les modifications
            SauvegarderDonnees();
        }

        /// <summary>
        /// Affiche le tableau de bord financier
        /// </summary>
        public void AfficherTableauDeBord()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Tableau de Bord Financier");

            // Afficher le solde actuel
            Console.WriteLine($"Solde actuel: {_solde:N2} €");

            // Calculer le bilan du mois en cours
            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            double revenusMonth = _transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit"))
                .Sum(t => t.Montant);

            double depensesMonth = _transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit"))
                .Sum(t => t.Montant);

            Console.WriteLine($"Revenus du mois: {revenusMonth:N2} €");
            Console.WriteLine($"Dépenses du mois: {depensesMonth:N2} €");
            Console.WriteLine($"Bilan du mois: {(revenusMonth - depensesMonth):N2} €");

            // Afficher les 5 dernières transactions
            Console.WriteLine("\nDernières transactions:");
            foreach (var transaction in _transactions.OrderByDescending(t => t.Date).Take(5))
            {
                string typeSymbole = transaction.Type.ToLower() == "crédit" || transaction.Type.ToLower() == "credit" ? "+" : "-";
                ConsoleColor couleur = transaction.Type.ToLower() == "crédit" || transaction.Type.ToLower() == "credit" ? ConsoleColor.Green : ConsoleColor.Red;

                Console.ForegroundColor = couleur;
                Console.WriteLine($"{transaction.Date:dd/MM/yyyy} | {typeSymbole}{transaction.Montant:N2} € | {transaction.Description} | {transaction.Categorie}");
                Console.ResetColor();
            }

            // Afficher les statistiques par catégorie
            AfficherStatistiquesParCategorie();
        }

        /// <summary>
        /// Affiche les statistiques financières par catégorie
        /// </summary>
        private void AfficherStatistiquesParCategorie()
        {
            // Période pour les statistiques (mois en cours)
            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Grouper les transactions par catégorie
            var revenusParCategorie = _transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit"))
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant)
                .ToList();

            var depensesParCategorie = _transactions
                .Where(t => t.Date >= debutMois && (t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit"))
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant)
                .ToList();

            // Afficher les revenus par catégorie
            Console.WriteLine("\nRevenus du mois par catégorie:");
            if (revenusParCategorie.Any())
            {
                foreach (var revenu in revenusParCategorie)
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

            // Afficher les dépenses par catégorie
            Console.WriteLine("\nDépenses du mois par catégorie:");
            if (depensesParCategorie.Any())
            {
                foreach (var depense in depensesParCategorie)
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
        /// Affiche l'historique des transactions
        /// </summary>
        public void AfficherHistorique()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Historique des Transactions");

            Console.WriteLine("Date       | Type   | Montant      | Catégorie     | Description");
            Console.WriteLine("---------- | ------ | ------------ | ------------- | -----------");

            foreach (var transaction in _transactions.OrderByDescending(t => t.Date))
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
            var commandes = _commandeManager.GetToutesLesCommandes();
            int compteur = 0;

            foreach (var commande in commandes)
            {
                // Vérifier si la commande existe déjà dans les transactions
                bool transactionExiste = _transactions.Any(t =>
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
            var salaries = _salarieManager.GetTousLesSalaries();
            DateTime maintenant = DateTime.Now;
            string moisAnnee = $"{maintenant:MM/yyyy}";

            // Vérifier si les salaires ont déjà été payés ce mois-ci
            bool salairesMoisDejaPayes = _transactions.Any(t =>
                t.Type.ToLower() == "débit" &&
                t.Categorie == "Salaires" &&
                t.Description.Contains(moisAnnee));

            if (!salairesMoisDejaPayes)
            {
                double totalSalaires = 0;

                foreach (var salarie in salaries)
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
        private void GenererRapportPeriode(DateTime debut, DateTime fin)
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre($"Rapport du {debut:dd/MM/yyyy} au {fin:dd/MM/yyyy}");

            var transactionsPeriode = _transactions
                .Where(t => t.Date >= debut && t.Date <= fin)
                .ToList();

            double revenus = transactionsPeriode
                .Where(t => t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit")
                .Sum(t => t.Montant);

            double depenses = transactionsPeriode
                .Where(t => t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit")
                .Sum(t => t.Montant);

            double benefice = revenus - depenses;

            // Afficher le résumé
            Console.WriteLine($"Revenus totaux: {revenus:N2} €");
            Console.WriteLine($"Dépenses totales: {depenses:N2} €");
            Console.WriteLine($"Bénéfice: {benefice:N2} €");

            // Afficher les revenus par catégorie
            var revenusParCategorie = transactionsPeriode
                .Where(t => t.Type.ToLower() == "crédit" || t.Type.ToLower() == "credit")
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant);

            Console.WriteLine("\nRevenus par catégorie:");
            foreach (var revenu in revenusParCategorie)
            {
                Console.WriteLine($"{revenu.Categorie}: {revenu.Montant:N2} €");
            }

            // Afficher les dépenses par catégorie
            var depensesParCategorie = transactionsPeriode
                .Where(t => t.Type.ToLower() == "débit" || t.Type.ToLower() == "debit")
                .GroupBy(t => t.Categorie)
                .Select(g => new { Categorie = g.Key, Montant = g.Sum(t => t.Montant) })
                .OrderByDescending(x => x.Montant);

            Console.WriteLine("\nDépenses par catégorie:");
            foreach (var depense in depensesParCategorie)
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
        public double Solde { get; set; }
        public List<TransactionSimple> Transactions { get; set; }
    }

    /// <summary>
    /// Représente une transaction financière simplifiée
    /// </summary>
    public class TransactionSimple
    {
        public DateTime Date { get; set; }
        public double Montant { get; set; }
        public string Type { get; set; }  // "Crédit" ou "Débit"
        public string Description { get; set; }
        public string Categorie { get; set; } = "Divers";
    }
}
