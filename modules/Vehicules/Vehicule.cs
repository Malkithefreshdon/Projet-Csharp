using System;
using System.Text.Json.Serialization;

namespace Projet.Modules
{
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

        [JsonConstructor]
        protected Vehicule() { }

        protected Vehicule(string immatriculation, double poidsMaximal, string marque, string modele)
        {
            Immatriculation = immatriculation;
            PoidsMaximal = poidsMaximal;
            Marque = marque;
            Modele = modele;
        }

        public abstract string GetDescription();
    }

    public class Voiture : Vehicule
    {
        [JsonInclude]
        public int NombrePassagers { get; private set; }

        [JsonConstructor]
        public Voiture() : base() { }

        public Voiture(string immatriculation, double poidsMaximal, string marque, string modele, int nombrePassagers) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            NombrePassagers = nombrePassagers;
        }

        public override string GetDescription()
        {
            return $"Voiture pour {NombrePassagers} passagers - {Marque} {Modele}";
        }
    }

    public class Camionnette : Vehicule
    {
        [JsonInclude]
        public string Usage { get; private set; }
        [JsonInclude]
        public bool TransportVerre { get; private set; }

        [JsonConstructor]
        public Camionnette() : base() { }

        public Camionnette(string immatriculation, double poidsMaximal, string marque, string modele, string usage, bool transportVerre) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            Usage = usage;
            TransportVerre = transportVerre;
        }

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

    public class CamionCiterne : Vehicule
    {
        [JsonInclude]
        public double CapaciteCuve { get; private set; }
        [JsonInclude]
        public string TypeProduit { get; private set; }

        [JsonConstructor]
        public CamionCiterne() : base() { }

        public CamionCiterne(string immatriculation, double poidsMaximal, string marque, string modele, double capaciteCuve, string typeProduit) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            CapaciteCuve = capaciteCuve;
            TypeProduit = typeProduit;
        }

        public override string GetDescription()
        {
            return $"Camion-citerne - Capacité: {CapaciteCuve}L - Type de produit: {TypeProduit}";
        }
    }

    public class CamionBenne : Vehicule
    {
        [JsonInclude]
        public int NombreBennes { get; private set; }
        [JsonInclude]
        public bool HasGrue { get; private set; }

        [JsonConstructor]
        public CamionBenne() : base() { }

        public CamionBenne(string immatriculation, double poidsMaximal, string marque, string modele, int nombreBennes, bool hasGrue) 
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            NombreBennes = nombreBennes;
            HasGrue = hasGrue;
        }

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

    public class CamionFrigorifique : Vehicule
    {
        [JsonInclude]
        public double TemperatureMin { get; private set; }
        [JsonInclude]
        public bool HasGroupeElectrogene { get; private set; }

        [JsonConstructor]
        public CamionFrigorifique() : base() { }

        public CamionFrigorifique(string immatriculation, double poidsMaximal, string marque, string modele, double temperatureMin, bool hasGroupeElectrogene)
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            TemperatureMin = temperatureMin;
            HasGroupeElectrogene = hasGroupeElectrogene;
        }

        public override string GetDescription()
        {
            return $"Camion frigorifique - Température minimale: {TemperatureMin}°C - {(HasGroupeElectrogene ? "Avec" : "Sans")} groupe électrogène";
        }
    }

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

        [JsonConstructor]
        public PoidsLourd() : base() { }

        public PoidsLourd(string immatriculation, double poidsMaximal, string marque, string modele, 
                         double volumeRemorque, string typeRemorque, string typeMarchandise, bool hasHayon)
            : base(immatriculation, poidsMaximal, marque, modele)
        {
            VolumeRemorque = volumeRemorque;
            TypeRemorque = typeRemorque;
            TypeMarchandise = typeMarchandise;
            HasHayon = hasHayon;
        }

        public override string GetDescription()
        {
            return $"Poids Lourd - {TypeRemorque} - Volume: {VolumeRemorque}m³ - Transport de {TypeMarchandise}" +
                   $"{(HasHayon ? " - Avec hayon" : "")}";
        }
    }
}