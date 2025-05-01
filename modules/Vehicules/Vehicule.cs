using System;

namespace Projet.Modules
{
    public abstract class Vehicule
    {
        public string Immatriculation { get; protected set; }
        public double PoidsMaximal { get; protected set; }
        public string Marque { get; protected set; }
        public string Modele { get; protected set; }

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
        public int NombrePassagers { get; private set; }

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
        public string Usage { get; private set; }
        public bool TransportVerre { get; private set; }

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
        public double CapaciteCuve { get; private set; }
        public string TypeProduit { get; private set; }

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
        public int NombreBennes { get; private set; }
        public bool HasGrue { get; private set; }

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
        public double TemperatureMin { get; private set; }
        public bool HasGroupeElectrogene { get; private set; }

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
        public double VolumeRemorque { get; private set; }
        public string TypeRemorque { get; private set; }
        public string TypeMarchandise { get; private set; }
        public bool HasHayon { get; private set; }

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