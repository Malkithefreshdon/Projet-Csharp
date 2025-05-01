using System;
using System.Linq;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class MaintenanceManagerUI
    {
        private readonly MaintenanceManager _maintenanceManager;
        private readonly VehiculeManager _vehiculeManager;

        public MaintenanceManagerUI(MaintenanceManager maintenanceManager, VehiculeManager vehiculeManager)
        {
            _maintenanceManager = maintenanceManager;
            _vehiculeManager = vehiculeManager;
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion de la Maintenance");
                Console.WriteLine("1. Ajouter une maintenance");
                Console.WriteLine("2. Voir l'historique d'un véhicule");
                Console.WriteLine("3. Voir les maintenances planifiées");
                Console.WriteLine("4. Voir les maintenances à venir");
                Console.WriteLine("5. Calculer les coûts de maintenance");
                Console.WriteLine("6. Mettre à jour le statut d'une maintenance");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AjouterMaintenance();
                        break;
                    case "2":
                        AfficherHistoriqueMaintenance();
                        break;
                    case "3":
                        AfficherMaintenancesPlanifiees();
                        break;
                    case "4":
                        AfficherMaintenancesAVenir();
                        break;
                    case "5":
                        CalculerCoutsMaintenance();
                        break;
                    case "6":
                        MettreAJourStatutMaintenance();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Option invalide");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AjouterMaintenance()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'une maintenance");

            Console.Write("Immatriculation du véhicule : ");
            var immatriculation = Console.ReadLine();

            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
            {
                Console.WriteLine("Véhicule non trouvé.");
                Console.ReadKey();
                return;
            }

            Console.Write("Type de maintenance (Preventive/Reparation) : ");
            var type = Console.ReadLine();

            Console.Write("Description : ");
            var description = Console.ReadLine();

            Console.Write("Coût : ");
            if (!double.TryParse(Console.ReadLine(), out double cout))
            {
                Console.WriteLine("Coût invalide");
                Console.ReadKey();
                return;
            }

            Console.Write("Date prochaine maintenance (JJ/MM/AAAA ou vide) : ");
            var prochaineDateStr = Console.ReadLine();
            DateTime? prochaineDate = null;
            if (!string.IsNullOrEmpty(prochaineDateStr))
            {
                if (DateTime.TryParse(prochaineDateStr, out DateTime date))
                {
                    prochaineDate = date;
                }
            }

            Console.Write("Technicien : ");
            var technicien = Console.ReadLine();

            var maintenance = new MaintenanceRecord
            {
                Immatriculation = immatriculation,
                DateMaintenance = DateTime.Now,
                TypeMaintenance = type,
                Description = description,
                Cout = cout,
                ProchaineMaintenance = prochaineDate,
                Statut = "Planifié",
                Technicien = technicien
            };

            try
            {
                _maintenanceManager.AjouterMaintenance(maintenance);
                Console.WriteLine("Maintenance ajoutée avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherHistoriqueMaintenance()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Historique des maintenances");

            Console.Write("Immatriculation du véhicule : ");
            var immatriculation = Console.ReadLine();

            var historique = _maintenanceManager.ObtenirHistoriqueMaintenance(immatriculation);
            if (historique.Any())
            {
                foreach (var maintenance in historique)
                {
                    AfficherDetailsMaintenance(maintenance);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("Aucun historique de maintenance trouvé pour ce véhicule.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherMaintenancesPlanifiees()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Maintenances planifiées");

            var maintenances = _maintenanceManager.ObtenirMaintenancesPlanifiees();
            if (maintenances.Any())
            {
                foreach (var maintenance in maintenances)
                {
                    AfficherDetailsMaintenance(maintenance);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("Aucune maintenance planifiée.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherMaintenancesAVenir()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Maintenances à venir");

            Console.Write("Nombre de jours à prévoir (défaut: 30) : ");
            var joursStr = Console.ReadLine();
            int jours = string.IsNullOrEmpty(joursStr) ? 30 : int.Parse(joursStr);

            var maintenances = _maintenanceManager.ObtenirMaintenancesAVenir(jours);
            if (maintenances.Any())
            {
                foreach (var maintenance in maintenances)
                {
                    AfficherDetailsMaintenance(maintenance);
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine($"Aucune maintenance prévue dans les {jours} prochains jours.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void CalculerCoutsMaintenance()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Calcul des coûts de maintenance");

            Console.Write("Immatriculation du véhicule : ");
            var immatriculation = Console.ReadLine();

            Console.WriteLine("\n1. Coûts totaux");
            Console.WriteLine("2. Coûts sur une période");
            Console.Write("\nVotre choix : ");

            var choix = Console.ReadLine();
            DateTime? debut = null;
            DateTime? fin = null;

            if (choix == "2")
            {
                Console.Write("Date de début (JJ/MM/AAAA) : ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dateDebut))
                {
                    debut = dateDebut;
                }

                Console.Write("Date de fin (JJ/MM/AAAA) : ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dateFin))
                {
                    fin = dateFin;
                }
            }

            var couts = _maintenanceManager.CalculerCoutsTotaux(immatriculation, debut, fin);
            Console.WriteLine($"\nCoûts totaux : {couts:C2}");

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void MettreAJourStatutMaintenance()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Mise à jour du statut d'une maintenance");

            Console.Write("Immatriculation du véhicule : ");
            var immatriculation = Console.ReadLine();

            var maintenances = _maintenanceManager.ObtenirHistoriqueMaintenance(immatriculation);
            if (!maintenances.Any())
            {
                Console.WriteLine("Aucune maintenance trouvée pour ce véhicule.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nMaintenances disponibles :");
            foreach (var maintenance in maintenances)
            {
                Console.WriteLine($"Date: {maintenance.DateMaintenance:dd/MM/yyyy}, Type: {maintenance.TypeMaintenance}, Statut: {maintenance.Statut}");
            }

            Console.Write("\nDate de la maintenance à modifier (JJ/MM/AAAA) : ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateMaintenance))
            {
                Console.WriteLine("Format de date invalide.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nNouveau statut :");
            Console.WriteLine("1. Planifié");
            Console.WriteLine("2. En cours");
            Console.WriteLine("3. Terminé");
            Console.Write("\nVotre choix : ");

            string nouveauStatut = Console.ReadLine() switch
            {
                "1" => "Planifié",
                "2" => "En cours",
                "3" => "Terminé",
                _ => null
            };

            if (nouveauStatut != null)
            {
                _maintenanceManager.MettreAJourStatutMaintenance(immatriculation, dateMaintenance, nouveauStatut);
                Console.WriteLine("Statut mis à jour avec succès.");
            }
            else
            {
                Console.WriteLine("Statut invalide.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherDetailsMaintenance(MaintenanceRecord maintenance)
        {
            Console.WriteLine($"Véhicule: {maintenance.Immatriculation}");
            Console.WriteLine($"Date: {maintenance.DateMaintenance:dd/MM/yyyy}");
            Console.WriteLine($"Type: {maintenance.TypeMaintenance}");
            Console.WriteLine($"Description: {maintenance.Description}");
            Console.WriteLine($"Coût: {maintenance.Cout:C2}");
            Console.WriteLine($"Statut: {maintenance.Statut}");
            Console.WriteLine($"Technicien: {maintenance.Technicien}");
            if (maintenance.ProchaineMaintenance.HasValue)
            {
                Console.WriteLine($"Prochaine maintenance prévue: {maintenance.ProchaineMaintenance.Value:dd/MM/yyyy}");
            }
        }
    }
}