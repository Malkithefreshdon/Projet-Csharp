using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    /// <summary>
    /// Gère la liste des véhicules et leur persistance.
    /// </summary>
    public class VehiculeManager
    {
        private readonly string JsonFilePath;
        private List<Vehicule> Vehicules;

        /// <summary>
        /// Initialise le gestionnaire de véhicules et charge les données depuis le fichier JSON.
        /// </summary>
        /// <param name="jsonFilePath">Chemin du fichier JSON (optionnel).</param>
        public VehiculeManager(string jsonFilePath = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            JsonFilePath = jsonFilePath ?? Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "vehicules.json");
            ChargerVehicules();
        }

        /// <summary>
        /// Charge les véhicules depuis le fichier JSON.
        /// </summary>
        private void ChargerVehicules()
        {
            try
            {
                string jsonContent = File.ReadAllText(JsonFilePath);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                VehiculesData vehiculesData = JsonSerializer.Deserialize<VehiculesData>(jsonContent, options);
                Vehicules = vehiculesData.Vehicules
                    .Select(v => ConvertirEnVehicule(v))
                    .Where(v => v != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement des véhicules: {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde la liste des véhicules dans le fichier JSON.
        /// </summary>
        private void SauvegarderVehicules()
        {
            try
            {
                VehiculesData vehiculesData = new VehiculesData
                {
                    Vehicules = Vehicules.Select(v => ConvertirEnVehiculeJSON(v)).ToList()
                };

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(vehiculesData, options);
                File.WriteAllText(JsonFilePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde des véhicules: {ex.Message}");
            }
        }

        /// <summary>
        /// Retourne la liste de tous les véhicules.
        /// </summary>
        /// <returns>Liste de tous les véhicules.</returns>
        public List<Vehicule> ObtenirTousLesVehicules()
        {
            return Vehicules;
        }

        /// <summary>
        /// Retourne la liste des véhicules d'un type donné.
        /// </summary>
        /// <typeparam name="T">Type de véhicule recherché.</typeparam>
        /// <returns>Liste des véhicules du type spécifié.</returns>
        public List<T> ObtenirVehiculesParType<T>() where T : Vehicule
        {
            return Vehicules.OfType<T>().ToList();
        }

        /// <summary>
        /// Recherche un véhicule par son immatriculation.
        /// </summary>
        /// <param name="immatriculation">Immatriculation du véhicule.</param>
        /// <returns>Le véhicule correspondant ou null.</returns>
        public Vehicule ObtenirVehiculeParImmatriculation(string immatriculation)
        {
            return Vehicules.FirstOrDefault(v => v.Immatriculation.Equals(immatriculation, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Ajoute un véhicule à la liste et sauvegarde la liste.
        /// </summary>
        /// <param name="vehicule">Véhicule à ajouter.</param>
        public void AjouterVehicule(Vehicule vehicule)
        {
            if (vehicule == null)
                throw new ArgumentNullException(nameof(vehicule));

            if (Vehicules.Any(v => v.Immatriculation.Equals(vehicule.Immatriculation, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"Un véhicule avec l'immatriculation {vehicule.Immatriculation} existe déjà.");

            Vehicules.Add(vehicule);
            SauvegarderVehicules();
        }

        /// <summary>
        /// Supprime un véhicule par son immatriculation.
        /// </summary>
        /// <param name="immatriculation">Immatriculation du véhicule à supprimer.</param>
        /// <returns>True si le véhicule a été supprimé, sinon False.</returns>
        public bool SupprimerVehicule(string immatriculation)
        {
            Vehicule vehicule = ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
                return false;

            Vehicules.Remove(vehicule);
            SauvegarderVehicules();
            return true;
        }

        /// <summary>
        /// Recherche les véhicules correspondant à un critère donné.
        /// </summary>
        /// <param name="critere">Fonction de filtrage.</param>
        /// <returns>Liste des véhicules correspondant au critère.</returns>
        public List<Vehicule> RechercherVehicules(Func<Vehicule, bool> critere)
        {
            return Vehicules.Where(critere).ToList();
        }

        /// <summary>
        /// Convertit un DTO en objet véhicule.
        /// </summary>
        /// <param name="dto">DTO à convertir.</param>
        /// <returns>Instance de véhicule correspondante.</returns>
        private Vehicule ConvertirEnVehicule(VehiculeJSON dto)
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

        /// <summary>
        /// Convertit un objet véhicule en DTO pour la sérialisation.
        /// </summary>
        /// <param name="vehicule">Véhicule à convertir.</param>
        /// <returns>DTO correspondant au véhicule.</returns>
        private VehiculeJSON ConvertirEnVehiculeJSON(Vehicule vehicule)
        {
            VehiculeJSON dto = new VehiculeJSON
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

    /// <summary>
    /// Structure de données pour la sérialisation de la liste des véhicules.
    /// </summary>
    public class VehiculesData
    {
        public List<VehiculeJSON> Vehicules { get; set; }
    }

    /// <summary>
    /// Data Transfer Object (DTO) pour la sérialisation/désérialisation des véhicules.
    /// </summary>
    public class VehiculeJSON
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