using System;

namespace Projet.Modules
{
    public class MaintenanceRecord
    {
        public string Immatriculation { get; set; }
        public DateTime DateMaintenance { get; set; }
        public string TypeMaintenance { get; set; }  // "Preventive" ou "Reparation"
        public string Description { get; set; }
        public double Cout { get; set; }
        public DateTime? ProchaineMaintenance { get; set; }
        public string Statut { get; set; }  // "Planifié", "En cours", "Terminé"
        public string Technicien { get; set; }
    }
}