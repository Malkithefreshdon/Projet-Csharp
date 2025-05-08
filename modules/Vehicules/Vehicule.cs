using System;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
    /// <summary>
    /// Classe abstraite représentant un véhicule générique.
    /// </summary>
    [JsonPolymorphic]
    [JsonDerivedType(typeof(Voiture), typeDiscriminator: "Voiture")]
    [JsonDerivedType(typeof(Camionnette), typeDiscriminator: "Camionnette")]
    [JsonDerivedType(typeof(CamionCiterne), typeDiscriminator: "CamionCiterne")]
    [JsonDerivedType(typeof(CamionBenne), typeDiscriminator: "CamionBenne")]
    [JsonDerivedType(typeof(CamionFrigorifique), typeDiscriminator: "CamionFrigorifique")]
    [JsonDerivedType(typeof(PoidsLourd), typeDiscriminator: "PoidsLourd")]
    public abstract class Vehicule
    {
        [JsonInclude]
        public string Immatriculation { get; protected set; }
        [JsonInclude]
        public double PoidsMaximal { get; protected set; }
        [JsonInclude]
        public string Marque { get; protected set; }
        [JsonInclude]
        public string Modele { get; protected set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        protected Vehicule() { }

        /// <summary>
        /// Initialise un véhicule avec ses propriétés principales.
        /// </summary>
        /// <param name="immatriculation">Immatriculation du véhicule.</param>
        /// <param name="poidsMaximal">Poids maximal autorisé.</param>
        /// <param name="marque">Marque du véhicule.</param>
        /// <param name="modele">Modèle du véhicule.</param>
        protected Vehicule(string immatriculation, double poidsMaximal, string marque, string modele)
        {
            Immatriculation = immatriculation;
            PoidsMaximal = poidsMaximal;
            Marque = marque;
            Modele = modele;
        }

        /// <summary>
        /// Retourne une description textuelle du véhicule.
        /// </summary>
        /// <returns>Description du véhicule.</returns>
        public abstract string GetDescription();
    }

    /// <summary>
    /// Représente une voiture.
    /// </summary>
    public class Voiture : Vehicule
    {
        [JsonInclude]
        public int NombrePassagers { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public Voiture() : base() { }

        /// <summary>
        /// Initialise une voiture avec ses propriétés principales.
        /// </summary>
        public Voiture(string immatriculation, double poidsMaximal, string marque, string modele, int nombrePassagers) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            NombrePassagers = nombrePassagers;
        }

        /// <summary>
        /// Retourne une description de la voiture.
        /// </summary>
        /// <returns>Description de la voiture.</returns>
        public override string GetDescription()
        {
            return $"Voiture pour {NombrePassagers} passagers - {Marque} {Modele}";
        }
    }

    /// <summary>
    /// Représente une camionnette.
    /// </summary>
    public class Camionnette : Vehicule
    {
        [JsonInclude]
        public string Usage { get; private set; }
        [JsonInclude]
        public bool TransportVerre { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public Camionnette() : base() { }

        /// <summary>
        /// Initialise une camionnette avec ses propriétés principales.
        /// </summary>
        public Camionnette(string immatriculation, double poidsMaximal, string marque, string modele, string usage, bool transportVerre) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            Usage = usage;
            TransportVerre = transportVerre;
        }

        /// <summary>
        /// Retourne une description de la camionnette.
        /// </summary>
        /// <returns>Description de la camionnette.</returns>
        public override string GetDescription()
        {
            string description = $"Camionnette - {Usage}";
            if (TransportVerre)
            {
                description += " - Équipée pour le transport de verre";
            }
            return description;
        }
    }

    /// <summary>
    /// Représente un camion-citerne.
    /// </summary>
    public class CamionCiterne : Vehicule
    {
        [JsonInclude]
        public double CapaciteCuve { get; private set; }
        [JsonInclude]
        public string TypeProduit { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public CamionCiterne() : base() { }

        /// <summary>
        /// Initialise un camion-citerne avec ses propriétés principales.
        /// </summary>
        public CamionCiterne(string immatriculation, double poidsMaximal, string marque, string modele, double capaciteCuve, string typeProduit) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            CapaciteCuve = capaciteCuve;
            TypeProduit = typeProduit;
        }

        /// <summary>
        /// Retourne une description du camion-citerne.
        /// </summary>
        /// <returns>Description du camion-citerne.</returns>
        public override string GetDescription()
        {
            return $"Camion-citerne - Capacité: {CapaciteCuve}L - Type de produit: {TypeProduit}";
        }
    }

    /// <summary>
    /// Représente un camion benne.
    /// </summary>
    public class CamionBenne : Vehicule
    {
        [JsonInclude]
        public int NombreBennes { get; private set; }
        [JsonInclude]
        public bool HasGrue { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public CamionBenne() : base() { }

        /// <summary>
        /// Initialise un camion benne avec ses propriétés principales.
        /// </summary>
        public CamionBenne(string immatriculation, double poidsMaximal, string marque, string modele, int nombreBennes, bool hasGrue) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            NombreBennes = nombreBennes;
            HasGrue = hasGrue;
        }

        /// <summary>
        /// Retourne une description du camion benne.
        /// </summary>
        /// <returns>Description du camion benne.</returns>
        public override string GetDescription()
        {
            string description = $"Camion benne - {NombreBennes} benne(s)";
            if (HasGrue)
            {
                description += " - Équipé d'une grue auxiliaire";
            }
            return description;
        }
    }

    /// <summary>
    /// Représente un camion frigorifique.
    /// </summary>
    public class CamionFrigorifique : Vehicule
    {
        [JsonInclude]
        public double TemperatureMin { get; private set; }
        [JsonInclude]
        public bool HasGroupeElectrogene { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public CamionFrigorifique() : base() { }

        /// <summary>
        /// Initialise un camion frigorifique avec ses propriétés principales.
        /// </summary>
        public CamionFrigorifique(string immatriculation, double poidsMaximal, string marque, string modele, double temperatureMin, bool hasGroupeElectrogene)
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            TemperatureMin = temperatureMin;
            HasGroupeElectrogene = hasGroupeElectrogene;
        }

        /// <summary>
        /// Retourne une description du camion frigorifique.
        /// </summary>
        /// <returns>Description du camion frigorifique.</returns>
        public override string GetDescription()
        {
            return $"Camion frigorifique - Température minimale: {TemperatureMin}°C - {(HasGroupeElectrogene ? "Avec" : "Sans")} groupe électrogène";
        }
    }

    /// <summary>
    /// Représente un poids lourd.
    /// </summary>
    public class PoidsLourd : Vehicule
    {
        [JsonInclude]
        public double VolumeRemorque { get; private set; }
        [JsonInclude]
        public string TypeRemorque { get; private set; }
        [JsonInclude]
        public string TypeMarchandise { get; private set; }
        [JsonInclude]
        public bool HasHayon { get; private set; }

        /// <summary>
        /// Constructeur par défaut pour la désérialisation JSON.
        /// </summary>
        [JsonConstructor]
        public PoidsLourd() : base() { }

        /// <summary>
        /// Initialise un poids lourd avec ses propriétés principales.
        /// </summary>
        public PoidsLourd(string immatriculation, double poidsMaximal, string marque, string modele, 
                         double volumeRemorque, string typeRemorque, string typeMarchandise, bool hasHayon)
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            VolumeRemorque = volumeRemorque;
            TypeRemorque = typeRemorque;
            TypeMarchandise = typeMarchandise;
            HasHayon = hasHayon;
        }

        /// <summary>
        /// Retourne une description du poids lourd.
        /// </summary>
        /// <returns>Description du poids lourd.</returns>
        public override string GetDescription()
        {
            return $"Poids Lourd - {TypeRemorque} - Volume: {VolumeRemorque}m³ - Transport de {TypeMarchandise}" +
                   $"{(HasHayon ? " - Avec hayon" : "")}";
        }
    }
}