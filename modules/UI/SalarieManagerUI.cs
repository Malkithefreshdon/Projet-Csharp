using System;
using System.Linq;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class SalarieManagerUI
    {
        private readonly SalarieManager salarieManager;

        public SalarieManagerUI(SalarieManager salarieManager)
        {
            this.salarieManager = salarieManager;
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Salariés");
                Console.WriteLine("1. Afficher l'organigramme");
                Console.WriteLine("2. Rechercher un salarié");
                Console.WriteLine("3. Ajouter un salarié");
                Console.WriteLine("4. Supprimer un salarié");
                Console.WriteLine("5. Afficher les subordonnés d'un salarié");
                Console.WriteLine("6. Afficher les collègues d'un salarié");
                Console.WriteLine("7. Retour");
                Console.WriteLine("\nVotre choix : ");

                string choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherOrganigramme();
                        break;
                    case "2":
                        RechercherSalarie();
                        break;
                    case "3":
                        AjouterSalarie();
                        break;
                    case "4":
                        SupprimerSalarie();
                        break;
                    case "5":
                        AfficherSubordonnes();
                        break;
                    case "6":
                        AfficherCollegues();
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

        private void AfficherOrganigramme()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Organigramme");
            salarieManager.AfficherOrganigramme();
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche de Salarié");
            Console.WriteLine("1. Par nom");
            Console.WriteLine("2. Par numéro de sécurité sociale");
            Console.Write("\nVotre choix : ");

            string choix = Console.ReadLine();
            if (choix == "1")
            {
                Console.Write("\nNom du salarié : ");
                string nom = Console.ReadLine();
                List<Salarie> resultats = salarieManager.RechercherParNom(nom);
                if (resultats.Any())
                {
                    Console.WriteLine("\nSalariés trouvés :");
                    foreach (Salarie salarie in resultats)
                    {
                        AfficherDetailsSalarie(salarie);
                        Console.WriteLine("-----------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("\nAucun salarié trouvé.");
                }
            }
            else if (choix == "2")
            {
                Console.Write("\nNuméro de sécurité sociale : ");
                string numeroSS = Console.ReadLine();
                Salarie salarie = salarieManager.RechercherParId(numeroSS);
                if (salarie != null)
                {
                    Console.WriteLine("\nSalarié trouvé :");
                    AfficherDetailsSalarie(salarie);
                }
                else
                {
                    Console.WriteLine("\nAucun salarié trouvé.");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un Salarié");

            Console.WriteLine("1. Ajout complet d'un salarié");
            Console.WriteLine("2. Ajout rapide d'un salarié de test");
            Console.Write("\nVotre choix : ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AjouterSalarieComplet();
                    break;
                case "2":
                    AjouterSalarieTest();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    break;
            }
        }

        private void AjouterSalarieTest()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout Rapide d'un Salarié de Test");

            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine("Le nom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prenom))
            {
                Console.WriteLine("Le prénom ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nChoisissez le poste :");
            Console.WriteLine("1. Directeur");
            Console.WriteLine("2. Chef d'Équipe");
            Console.WriteLine("3. Chauffeur");
            Console.WriteLine("4. Autre (à saisir)");
            Console.Write("\nVotre choix : ");

            string poste;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    poste = "Directeur";
                    break;
                case "2":
                    poste = "Chef d'Équipe";
                    break;
                case "3":
                    poste = "Chauffeur";
                    break;
                case "4":
                    Console.Write("Saisissez le poste : ");
                    poste = Console.ReadLine()?.Trim();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    return;
            }

            if (string.IsNullOrWhiteSpace(poste))
            {
                Console.WriteLine("Le poste ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            string managerNumeroSS = null;
            Console.WriteLine("\nChoisissez un manager :");
            Console.WriteLine("1. Ajouter sous un manager existant");
            Console.WriteLine("2. Sans manager (racine)");
            Console.Write("\nVotre choix : ");

            bool isRoot = false;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    List<Salarie> managers = salarieManager.GetTousLesSalaries()
                        .Where(s => s.Poste.Contains("Directeur", StringComparison.OrdinalIgnoreCase) ||
                                  s.Poste.Contains("Chef", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (!managers.Any())
                    {
                        Console.WriteLine("Aucun manager disponible.");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("\nManagers disponibles :");
                    for (int i = 0; i < managers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {managers[i].Nom} ({managers[i].Poste}) - SS: {managers[i].NumeroSecuriteSociale}");
                    }

                    Console.Write("\nChoisissez le numéro du manager (0 pour annuler) : ");
                    if (int.TryParse(Console.ReadLine(), out int choixManager) && choixManager > 0 && choixManager <= managers.Count)
                    {
                        managerNumeroSS = managers[choixManager - 1].NumeroSecuriteSociale;
                    }
                    else if (choixManager != 0)
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.ReadKey();
                        return;
                    }
                    break;

                case "2":
                    isRoot = true;
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                    return;
            }

            string numeroSS = $"TEST{DateTime.Now.ToString("yyMMddHHmmss")}";

            Salarie nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = numeroSS,
                Nom = nom,
                Prenom = prenom,
                Poste = poste,
                DateNaissance = new DateTime(1990, 1, 1),
                DateEntreeSociete = DateTime.Now,
                AdressePostale = "1 rue de Test, 75000 Paris",
                AdresseMail = $"{prenom.ToLower()}.{nom.ToLower()}@test.com",
                Telephone = "0123456789",
                Salaire = 30000
            };

            if (isRoot)
            {
                Salarie racineExistante = salarieManager.GetTousLesSalaries()
                    .FirstOrDefault(s => string.IsNullOrEmpty(s.ManagerNumeroSS));

                if (racineExistante != null)
                {
                    Console.WriteLine($"\nIl existe déjà une racine : {racineExistante.Nom} ({racineExistante.Poste})");
                    Console.WriteLine("Voulez-vous :");
                    Console.WriteLine("1. Définir ce salarié comme nouvelle racine");
                    Console.WriteLine("2. Ajouter sous la racine existante");
                    Console.Write("\nVotre choix : ");

                    switch (Console.ReadLine()?.Trim())
                    {
                        case "1":
                            racineExistante.ManagerNumeroSS = numeroSS;
                            break;
                        case "2":
                            managerNumeroSS = racineExistante.NumeroSecuriteSociale;
                            break;
                        default:
                            Console.WriteLine("Choix invalide. Opération annulée.");
                            Console.ReadKey();
                            return;
                    }
                }
            }

            if (salarieManager.AjouterSalarie(nouveauSalarie, managerNumeroSS))
            {
                Console.WriteLine("\nSalarié de test ajouté avec succès !");
                Console.WriteLine("\nDétails du salarié créé :");
                AfficherDetailsSalarie(nouveauSalarie);
            }
            else
            {
                Console.WriteLine("\nErreur lors de l'ajout du salarié de test.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterSalarieComplet()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout Complet d'un Salarié");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(numeroSS))
            {
                Console.WriteLine("Le numéro de sécurité sociale ne peut pas être vide.");
                Console.ReadKey();
                return;
            }

            if (salarieManager.RechercherParId(numeroSS) != null)
            {
                Console.WriteLine("Un salarié avec ce numéro existe déjà.");
                Console.ReadKey();
                return;
            }

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();

            Console.Write("Poste : ");
            string poste = Console.ReadLine();

            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateNaissance))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Date d'entrée dans la société (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateEntree))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Adresse postale : ");
            string adressePostale = Console.ReadLine();

            Console.Write("Email : ");
            string email = Console.ReadLine();

            Console.Write("Téléphone : ");
            string telephone = Console.ReadLine();

            Console.Write("Salaire : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salaire))
            {
                Console.WriteLine("Format de salaire invalide.");
                Console.ReadKey();
                return;
            }

            Console.Write("Numéro de sécurité sociale du manager (laisser vide si aucun) : ");
            string managerNumeroSS = Console.ReadLine();

            Salarie nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = numeroSS,
                Nom = nom,
                Prenom = prenom,
                Poste = poste,
                DateNaissance = dateNaissance,
                DateEntreeSociete = dateEntree,
                AdressePostale = adressePostale,
                AdresseMail = email,
                Telephone = telephone,
                Salaire = salaire,
                ManagerNumeroSS = string.IsNullOrWhiteSpace(managerNumeroSS) ? null : managerNumeroSS
            };

            bool ajoutOk = salarieManager.AjouterSalarie(nouveauSalarie, managerNumeroSS);
            if (ajoutOk)
            {
                Console.WriteLine("Salarié ajouté avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'ajout du salarié.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void SupprimerSalarie()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un Salarié");

            Console.Write("Numéro de sécurité sociale : ");
            string numeroSS = Console.ReadLine();

            Salarie salarie = salarieManager.RechercherParId(numeroSS);
            if (salarie == null)
            {
                Console.WriteLine("Salarié non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nVous allez supprimer le salarié suivant :");
            AfficherDetailsSalarie(salarie);

            Console.Write("\nÊtes-vous sûr de vouloir supprimer ce salarié ? (O/N) : ");
            if (Console.ReadLine()?.ToUpper() != "O")
            {
                Console.WriteLine("Suppression annulée.");
                Console.ReadKey();
                return;
            }

            bool suppOk = salarieManager.SupprimerSalarie(numeroSS);
            if (suppOk)
            {
                Console.WriteLine("Salarié supprimé avec succès.");
            }
            else
            {
                Console.WriteLine("Erreur lors de la suppression du salarié.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherSubordonnes()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Affichage des Subordonnés");

            Console.Write("Numéro de sécurité sociale du salarié : ");
            string numeroSS = Console.ReadLine();

            List<Salarie> subordonnes = salarieManager.ObtenirSubordonnesDirects(numeroSS);
            if (subordonnes.Any())
            {
                Console.WriteLine("\nSubordonnés directs :");
                foreach (Salarie subordonne in subordonnes)
                {
                    AfficherDetailsSalarie(subordonne);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun subordonné trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherCollegues()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Affichage des Collègues");

            Console.Write("Numéro de sécurité sociale du salarié : ");
            string numeroSS = Console.ReadLine();

            List<Salarie> collegues = salarieManager.ObtenirCollegues(numeroSS);
            if (collegues.Any())
            {
                Console.WriteLine("\nCollègues :");
                foreach (Salarie collegue in collegues)
                {
                    AfficherDetailsSalarie(collegue);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun collègue trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherDetailsSalarie(Salarie salarie)
        {
            if (salarie == null) return;

            Console.WriteLine($"Nom : {salarie.Nom} {salarie.Prenom}");
            Console.WriteLine($"Poste : {salarie.Poste}");
            Console.WriteLine($"N° SS : {salarie.NumeroSecuriteSociale}");
            Console.WriteLine($"Date de naissance : {salarie.DateNaissance:dd/MM/yyyy}");
            Console.WriteLine($"Date d'entrée : {salarie.DateEntreeSociete:dd/MM/yyyy}");
            Console.WriteLine($"Adresse : {salarie.AdressePostale}");
            Console.WriteLine($"Email : {salarie.AdresseMail ?? "Non renseigné"}");
            Console.WriteLine($"Téléphone : {salarie.Telephone ?? "Non renseigné"}");
            Console.WriteLine($"Salaire : {salarie.Salaire:C2}");
            Console.WriteLine($"Manager : {salarie.ManagerNumeroSS ?? "Aucun"}");
        }
    }
} 