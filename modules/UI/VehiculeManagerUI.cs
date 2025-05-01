using System;
using System.Linq;
using Projet.Modules;

namespace Projet.Modules.UI
{
    public class VehiculeManagerUI
    {
        private readonly VehiculeManager _vehiculeManager;

        public VehiculeManagerUI(VehiculeManager vehiculeManager)
        {
            _vehiculeManager = vehiculeManager;
        }

        public void AfficherMenu()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                ConsoleHelper.AfficherTitre("Gestion des Véhicules");
                Console.WriteLine("1. Afficher tous les véhicules");
                Console.WriteLine("2. Afficher les véhicules par type");
                Console.WriteLine("3. Rechercher un véhicule par immatriculation");
                Console.WriteLine("4. Ajouter un véhicule");
                Console.WriteLine("5. Supprimer un véhicule");
                Console.WriteLine("6. Rechercher des véhicules par critères");
                Console.WriteLine("0. Retour");
                Console.WriteLine("\nVotre choix : ");

                var choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherTousLesVehicules();
                        break;
                    case "2":
                        AfficherVehiculesParType();
                        break;
                    case "3":
                        RechercherVehiculeParImmatriculation();
                        break;
                    case "4":
                        AjouterVehicule();
                        break;
                    case "5":
                        SupprimerVehicule();
                        break;
                    case "6":
                        RechercherVehiculeParCriteres();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AfficherTousLesVehicules()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Liste de tous les véhicules");

            var vehicules = _vehiculeManager.ObtenirTousLesVehicules();
            if (!vehicules.Any())
            {
                Console.WriteLine("Aucun véhicule enregistré.");
            }
            else
            {
                foreach (var vehicule in vehicules)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
                Console.WriteLine($"\nTotal : {vehicules.Count} véhicule(s)");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AfficherVehiculesParType()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Véhicules par type");
            Console.WriteLine("1. Voitures");
            Console.WriteLine("2. Camionnettes");
            Console.WriteLine("3. Camions Citernes");
            Console.WriteLine("4. Camions Bennes");
            Console.WriteLine("5. Camions Frigorifiques");
            Console.WriteLine("6. Poids Lourds");
            Console.Write("\nChoisissez un type : ");

            var choix = Console.ReadLine();
            Console.Clear();

            List<Vehicule> vehicules = choix switch
            {
                "1" => _vehiculeManager.ObtenirVehiculesParType<Voiture>().Cast<Vehicule>().ToList(),
                "2" => _vehiculeManager.ObtenirVehiculesParType<Camionnette>().Cast<Vehicule>().ToList(),
                "3" => _vehiculeManager.ObtenirVehiculesParType<CamionCiterne>().Cast<Vehicule>().ToList(),
                "4" => _vehiculeManager.ObtenirVehiculesParType<CamionBenne>().Cast<Vehicule>().ToList(),
                "5" => _vehiculeManager.ObtenirVehiculesParType<CamionFrigorifique>().Cast<Vehicule>().ToList(),
                "6" => _vehiculeManager.ObtenirVehiculesParType<PoidsLourd>().Cast<Vehicule>().ToList(),
                _ => new List<Vehicule>()
            };

            if (!vehicules.Any())
            {
                Console.WriteLine("Aucun véhicule de ce type trouvé.");
            }
            else
            {
                foreach (var vehicule in vehicules)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
                Console.WriteLine($"\nTotal : {vehicules.Count} véhicule(s)");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherVehiculeParImmatriculation()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche par immatriculation");

            Console.Write("Entrez l'immatriculation : ");
            string immatriculation = Console.ReadLine();

            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule != null)
            {
                Console.WriteLine("\nVéhicule trouvé :");
                Console.WriteLine(vehicule.GetDescription());
            }
            else
            {
                Console.WriteLine("\nAucun véhicule trouvé avec cette immatriculation.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void AjouterVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Ajout d'un véhicule");

            Console.WriteLine("Type de véhicule :");
            Console.WriteLine("1. Voiture");
            Console.WriteLine("2. Camionnette");
            Console.WriteLine("3. Camion Citerne");
            Console.WriteLine("4. Camion Benne");
            Console.WriteLine("5. Camion Frigorifique");
            Console.WriteLine("6. Poids Lourd");
            Console.Write("\nChoisissez un type : ");

            var choix = Console.ReadLine();

            // Informations communes
            Console.Write("\nImmatriculation : ");
            string immatriculation = Console.ReadLine();
            Console.Write("Poids maximal (tonnes) : ");
            double.TryParse(Console.ReadLine(), out double poidsMaximal);
            Console.Write("Marque : ");
            string marque = Console.ReadLine();
            Console.Write("Modèle : ");
            string modele = Console.ReadLine();

            try
            {
                Vehicule nouveauVehicule = null;

                switch (choix)
                {
                    case "1": // Voiture
                        Console.Write("Nombre de passagers : ");
                        int.TryParse(Console.ReadLine(), out int nombrePassagers);
                        nouveauVehicule = new Voiture(immatriculation, poidsMaximal, marque, modele, nombrePassagers);
                        break;

                    case "2": // Camionnette
                        Console.Write("Usage : ");
                        string usage = Console.ReadLine();
                        Console.Write("Transport de verre (O/N) : ");
                        bool transportVerre = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new Camionnette(immatriculation, poidsMaximal, marque, modele, usage, transportVerre);
                        break;

                    case "3": // Camion Citerne
                        Console.Write("Capacité de la cuve (litres) : ");
                        double.TryParse(Console.ReadLine(), out double capaciteCuve);
                        Console.Write("Type de produit : ");
                        string typeProduit = Console.ReadLine();
                        nouveauVehicule = new CamionCiterne(immatriculation, poidsMaximal, marque, modele, capaciteCuve, typeProduit);
                        break;

                    case "4": // Camion Benne
                        Console.Write("Nombre de bennes : ");
                        int.TryParse(Console.ReadLine(), out int nombreBennes);
                        Console.Write("Équipé d'une grue (O/N) : ");
                        bool hasGrue = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new CamionBenne(immatriculation, poidsMaximal, marque, modele, nombreBennes, hasGrue);
                        break;

                    case "5": // Camion Frigorifique
                        Console.Write("Température minimale : ");
                        double.TryParse(Console.ReadLine(), out double temperatureMin);
                        Console.Write("Équipé d'un groupe électrogène (O/N) : ");
                        bool hasGroupeElectrogene = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new CamionFrigorifique(immatriculation, poidsMaximal, marque, modele, temperatureMin, hasGroupeElectrogene);
                        break;

                    case "6": // Poids Lourd
                        Console.Write("Volume de la remorque (m³) : ");
                        double.TryParse(Console.ReadLine(), out double volumeRemorque);
                        Console.Write("Type de remorque : ");
                        string typeRemorque = Console.ReadLine();
                        Console.Write("Type de marchandise : ");
                        string typeMarchandise = Console.ReadLine();
                        Console.Write("Équipé d'un hayon (O/N) : ");
                        bool hasHayon = Console.ReadLine()?.ToUpper() == "O";
                        nouveauVehicule = new PoidsLourd(immatriculation, poidsMaximal, marque, modele, volumeRemorque, typeRemorque, typeMarchandise, hasHayon);
                        break;
                }

                if (nouveauVehicule != null)
                {
                    _vehiculeManager.AjouterVehicule(nouveauVehicule);
                    Console.WriteLine("\nVéhicule ajouté avec succès !");
                }
                else
                {
                    Console.WriteLine("\nType de véhicule non reconnu.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErreur lors de l'ajout du véhicule : {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void SupprimerVehicule()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Suppression d'un véhicule");

            Console.Write("Entrez l'immatriculation du véhicule à supprimer : ");
            string immatriculation = Console.ReadLine();

            var vehicule = _vehiculeManager.ObtenirVehiculeParImmatriculation(immatriculation);
            if (vehicule == null)
            {
                Console.WriteLine("\nAucun véhicule trouvé avec cette immatriculation.");
            }
            else
            {
                Console.WriteLine("\nVéhicule trouvé :");
                Console.WriteLine(vehicule.GetDescription());
                Console.Write("\nÊtes-vous sûr de vouloir supprimer ce véhicule ? (O/N) : ");

                if (Console.ReadLine()?.ToUpper() == "O")
                {
                    if (_vehiculeManager.SupprimerVehicule(immatriculation))
                    {
                        Console.WriteLine("\nVéhicule supprimé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("\nErreur lors de la suppression du véhicule.");
                    }
                }
                else
                {
                    Console.WriteLine("\nSuppression annulée.");
                }
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void RechercherVehiculeParCriteres()
        {
            Console.Clear();
            ConsoleHelper.AfficherTitre("Recherche par critères");

            Console.WriteLine("1. Rechercher par marque");
            Console.WriteLine("2. Rechercher par poids maximal");
            Console.WriteLine("3. Retour");
            Console.Write("\nVotre choix : ");

            var choix = Console.ReadLine();
            List<Vehicule> resultats = new List<Vehicule>();

            switch (choix)
            {
                case "1":
                    Console.Write("\nEntrez la marque : ");
                    string marque = Console.ReadLine();
                    resultats = _vehiculeManager.RechercherVehicules(v =>
                        v.Marque.Contains(marque, StringComparison.OrdinalIgnoreCase));
                    break;

                case "2":
                    Console.Write("\nPoids maximal minimum (tonnes) : ");
                    if (double.TryParse(Console.ReadLine(), out double poidsMin))
                    {
                        resultats = _vehiculeManager.RechercherVehicules(v => v.PoidsMaximal >= poidsMin);
                    }
                    break;
            }

            if (resultats.Any())
            {
                Console.WriteLine($"\n{resultats.Count} véhicule(s) trouvé(s) :");
                foreach (var vehicule in resultats)
                {
                    Console.WriteLine(vehicule.GetDescription());
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine("\nAucun véhicule trouvé.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
} 