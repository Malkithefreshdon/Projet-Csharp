using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Projet.Modules
{
    public class MaintenanceManager
    {
        private List<MaintenanceRecord> maintenanceRecords;
        private readonly string maintenanceJsonChemin;
        private readonly string vehiculesJsonChemin;

        public MaintenanceManager()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            maintenanceJsonChemin = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "maintenance.json");
            vehiculesJsonChemin = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "vehicules.json");
            maintenanceRecords = ChargerMaintenanceRecords();
        }

        private List<MaintenanceRecord> ChargerMaintenanceRecords()
        {
            if (File.Exists(maintenanceJsonChemin))
            {
                string jsonString = File.ReadAllText(maintenanceJsonChemin);
                return JsonSerializer.Deserialize<List<MaintenanceRecord>>(jsonString);
            }
            return new List<MaintenanceRecord>();
        }

        private void SauvegarderMaintenanceRecords()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(maintenanceRecords, options);
            File.WriteAllText(maintenanceJsonChemin, jsonString);
        }

        public void AjouterMaintenance(MaintenanceRecord maintenance)
        {
            VehiculesData vehicules = JsonSerializer.Deserialize<VehiculesData>(
                File.ReadAllText(vehiculesJsonChemin)
            );

            if (!vehicules.Vehicules.Any(v => v.Immatriculation == maintenance.Immatriculation))
            {
                throw new ArgumentException("Véhicule non trouvé");
            }

            maintenanceRecords.Add(maintenance);
            SauvegarderMaintenanceRecords();
        }

        public List<MaintenanceRecord> ObtenirHistoriqueMaintenance(string immatriculation)
        {
            return maintenanceRecords
                .Where(m => m.Immatriculation == immatriculation)
                .OrderByDescending(m => m.DateMaintenance)
                .ToList();
        }

        public List<MaintenanceRecord> ObtenirMaintenancesPlanifiees()
        {
            return maintenanceRecords
                .Where(m => m.Statut == "Planifié")
                .OrderBy(m => m.DateMaintenance)
                .ToList();
        }

        public List<MaintenanceRecord> ObtenirMaintenancesAVenir(int joursAvant = 30)
        {
            DateTime dateLimit = DateTime.Now.AddDays(joursAvant);
            return maintenanceRecords
                .Where(m => m.ProchaineMaintenance.HasValue && 
                           m.ProchaineMaintenance.Value <= dateLimit)
                .OrderBy(m => m.ProchaineMaintenance)
                .ToList();
        }

        public double CalculerCoutsTotaux(string immatriculation, DateTime? debut = null, DateTime? fin = null)
        {
            IEnumerable<MaintenanceRecord> maintenances = maintenanceRecords
                .Where(m => m.Immatriculation == immatriculation &&
                           (!debut.HasValue || m.DateMaintenance >= debut) &&
                           (!fin.HasValue || m.DateMaintenance <= fin));

            return maintenances.Sum(m => m.Cout);
        }

        public void MettreAJourStatutMaintenance(string immatriculation, DateTime dateMaintenance, string nouveauStatut)
        {
            MaintenanceRecord? maintenance = maintenanceRecords
                .FirstOrDefault(m => m.Immatriculation == immatriculation && 
                                   m.DateMaintenance == dateMaintenance);

            if (maintenance != null)
            {
                maintenance.Statut = nouveauStatut;
                SauvegarderMaintenanceRecords();
            }
        }
    }
}