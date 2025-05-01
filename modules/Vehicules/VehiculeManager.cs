using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    public class VehiculeManager
    {
        private readonly string _jsonFilePath;
        private List<Vehicule> _vehicules;

        public VehiculeManager(string jsonFilePath = "modules/Ressources/vehicules.json")
        {
            _jsonFilePath = jsonFilePath;
            ChargerVehicules();
        }

        private void ChargerVehicules()
        {
            try
            {
                var jsonContent = File.ReadAllText(_jsonFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var vehiculesData = JsonSerializer.Deserialize<VehiculesData>(jsonContent, options);
                _vehicules = vehiculesData.Vehicules
                    .Select(v => ConvertirEnVehicule(v))
                    .Where(v => v != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement des véhicules: {ex.Message}");
            }
        }

        private void SauvegarderVehicules()
        {
            try
            {
                var vehiculesData = new VehiculesData
                {
                    Vehicules = _vehicules.Select(v => ConvertirEnVehiculeDTO(v)).ToList()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(vehiculesData, options);
                File.WriteAllText(_jsonFilePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde des véhicules: {ex.Message}");
            }
        }

        public List<Vehicule> ObtenirTousLesVehicules()
        {
            return _vehicules;
        }

        public List<T> ObtenirVehiculesParType<T>() where T : Vehicule
        {
            return _vehicules.OfType<T>().ToList();
        }

        public Vehicule ObtenirVehiculeParImmatriculation(string immatriculation)
        {
            return _vehicules.FirstOrDefault(v => v.Immatriculation.Equals(immatriculation, StringComparison.OrdinalIgnoreCase));
        }

        public void AjouterVehicule(Vehicule vehicule)
        {
            if (vehicule == null)
                throw new ArgumentNullException(nameof(vehicule));

            if (_vehicules.Any(v => v.Immatriculation.Equals(vehicule.Immatriculation, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"Un véhicule avec l'immatriculation {vehicule.Immatriculation} existe déjà.");

            _vehicules.Add(vehicule);
            SauvegarderVehicules();
        }

        public bool SupprimerVehicule(string immatriculation)
        {
            var vehicule = ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
                return false;

            _vehicules.Remove(vehicule);
            SauvegarderVehicules();
            return true;
        }

        public List<Vehicule> RechercherVehicules(Func<Vehicule, bool> critere)
        {
            return _vehicules.Where(critere).ToList();
        }

        private Vehicule ConvertirEnVehicule(VehiculeDTO dto)
        {
            try
            {
                return dto.Type switch
                {
                    "Voiture" => new Voiture(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.NombrePassagers),
                    "Camionnette" => new Camionnette(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.Usage, dto.TransportVerre),
                    "CamionCiterne" => new CamionCiterne(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.CapaciteCuve, dto.TypeProduit),
                    "CamionBenne" => new CamionBenne(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.NombreBennes, dto.HasGrue),
                    "CamionFrigorifique" => new CamionFrigorifique(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.TemperatureMin, dto.HasGroupeElectrogene),
                    "PoidsLourd" => new PoidsLourd(dto.Immatriculation, dto.PoidsMaximal, dto.Marque, dto.Modele, dto.VolumeRemorque, dto.TypeRemorque, dto.TypeMarchandise, dto.HasHayon),
                    _ => throw new Exception($"Type de véhicule inconnu: {dto.Type}")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la conversion du véhicule {dto.Immatriculation}: {ex.Message}");
                return null;
            }
        }

        private VehiculeDTO ConvertirEnVehiculeDTO(Vehicule vehicule)
        {
            var dto = new VehiculeDTO
            {
                Type = vehicule.GetType().Name,
                Immatriculation = vehicule.Immatriculation,
                PoidsMaximal = vehicule.PoidsMaximal,
                Marque = vehicule.Marque,
                Modele = vehicule.Modele
            };

            switch (vehicule)
            {
                case Voiture v:
                    dto.NombrePassagers = v.NombrePassagers;
                    break;
                case Camionnette c:
                    dto.Usage = c.Usage;
                    dto.TransportVerre = c.TransportVerre;
                    break;
                case CamionCiterne cc:
                    dto.CapaciteCuve = cc.CapaciteCuve;
                    dto.TypeProduit = cc.TypeProduit;
                    break;
                case CamionBenne cb:
                    dto.NombreBennes = cb.NombreBennes;
                    dto.HasGrue = cb.HasGrue;
                    break;
                case CamionFrigorifique cf:
                    dto.TemperatureMin = cf.TemperatureMin;
                    dto.HasGroupeElectrogene = cf.HasGroupeElectrogene;
                    break;
                case PoidsLourd pl:
                    dto.VolumeRemorque = pl.VolumeRemorque;
                    dto.TypeRemorque = pl.TypeRemorque;
                    dto.TypeMarchandise = pl.TypeMarchandise;
                    dto.HasHayon = pl.HasHayon;
                    break;
            }

            return dto;
        }
    }

    public class VehiculesData
    {
        public List<VehiculeDTO> Vehicules { get; set; }
    }

    public class VehiculeDTO
    {
        public string Type { get; set; }
        public string Immatriculation { get; set; }
        public double PoidsMaximal { get; set; }
        public string Marque { get; set; }
        public string Modele { get; set; }

        // Propriétés pour Voiture
        public int NombrePassagers { get; set; }

        // Propriétés pour Camionnette
        public string Usage { get; set; }
        public bool TransportVerre { get; set; }

        // Propriétés pour CamionCiterne
        public double CapaciteCuve { get; set; }
        public string TypeProduit { get; set; }

        // Propriétés pour CamionBenne
        public int NombreBennes { get; set; }
        public bool HasGrue { get; set; }

        // Propriétés pour CamionFrigorifique
        public double TemperatureMin { get; set; }
        public bool HasGroupeElectrogene { get; set; }

        // Propriétés pour PoidsLourd
        public double VolumeRemorque { get; set; }
        public string TypeRemorque { get; set; }
        public string TypeMarchandise { get; set; }
        public bool HasHayon { get; set; }
    }
} 