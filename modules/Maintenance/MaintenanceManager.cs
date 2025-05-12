using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Projet.Modules
{
    public class MaintenanceManager
    {
        private List<MaintenanceRecord> _maintenanceRecords;
        private readonly string _maintenanceJsonPath;
        private readonly string _vehiculesJsonPath;

        public MaintenanceManager()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _maintenanceJsonPath = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "maintenance.json");
            _vehiculesJsonPath = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "vehicules.json");
            _maintenanceRecords = ChargerMaintenanceRecords();
        }

        private List<MaintenanceRecord> ChargerMaintenanceRecords()
        {
            if (File.Exists(_maintenanceJsonPath))
            {
                string jsonString = File.ReadAllText(_maintenanceJsonPath);
                return JsonSerializer.Deserialize<List<MaintenanceRecord>>(jsonString);
            }
            return new List<MaintenanceRecord>();
        }

        private void SauvegarderMaintenanceRecords()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_maintenanceRecords, options);
            File.WriteAllText(_maintenanceJsonPath, jsonString);
        }

        public void AjouterMaintenance(MaintenanceRecord maintenance)
        {
            // Vérifier si le véhicule existe
            VehiculesData vehicules = JsonSerializer.Deserialize<VehiculesData>(
                File.ReadAllText(_vehiculesJsonPath)
            );

            if (!vehicules.Vehicules.Any(v => v.Immatriculation == maintenance.Immatriculation))
            {
                throw new ArgumentException("Véhicule non trouvé");
            }

            _maintenanceRecords.Add(maintenance);
            SauvegarderMaintenanceRecords();
        }

        public List<MaintenanceRecord> ObtenirHistoriqueMaintenance(string immatriculation)
        {
            return _maintenanceRecords
                .Where(m => m.Immatriculation == immatriculation)
                .OrderByDescending(m => m.DateMaintenance)
                .ToList();
        }

        public List<MaintenanceRecord> ObtenirMaintenancesPlanifiees()
        {
            return _maintenanceRecords
                .Where(m => m.Statut == "Planifié")
                .OrderBy(m => m.DateMaintenance)
                .ToList();
        }

        public List<MaintenanceRecord> ObtenirMaintenancesAVenir(int joursAvant = 30)
        {
            DateTime dateLimit = DateTime.Now.AddDays(joursAvant);
            return _maintenanceRecords
                .Where(m => m.ProchaineMaintenance.HasValue && 
                           m.ProchaineMaintenance.Value <= dateLimit)
                .OrderBy(m => m.ProchaineMaintenance)
                .ToList();
        }

        public double CalculerCoutsTotaux(string immatriculation, DateTime? debut = null, DateTime? fin = null)
        {
            IEnumerable<MaintenanceRecord> maintenances = _maintenanceRecords
                .Where(m => m.Immatriculation == immatriculation &&
                           (!debut.HasValue || m.DateMaintenance >= debut) &&
                           (!fin.HasValue || m.DateMaintenance <= fin));

            return maintenances.Sum(m => m.Cout);
        }

        public void MettreAJourStatutMaintenance(string immatriculation, DateTime dateMaintenance, string nouveauStatut)
        {
            MaintenanceRecord? maintenance = _maintenanceRecords
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