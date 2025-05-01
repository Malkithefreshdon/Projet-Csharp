using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; 
using System.Text.Json;

namespace Projet.Modules
{
    public class SalarieManager
    {
        private readonly Dictionary<string, Salarie> _tousLesSalaries;
        private OrganigrammeNaire _organigramme;

        public string FichierSauvegarde { get; set; } = "Ressources/salaries.json";

        public SalarieManager()
        {
            _tousLesSalaries = new Dictionary<string, Salarie>(StringComparer.OrdinalIgnoreCase);
            _organigramme = new OrganigrammeNaire();
            ChargerSalariesEtOrganigramme();
        }

        /// <summary>
        /// Ajoute un nouveau salarié et l'intègre dans l'organigramme si un manager est spécifié.
        /// </summary>
        /// <returns>True si l'ajout a réussi, False si le salarié existe déjà.</returns>
        public bool AjouterSalarie(Salarie nouveauSalarie, string managerNumeroSS = null)
        {
            if (nouveauSalarie == null) throw new ArgumentNullException(nameof(nouveauSalarie));
            if (_tousLesSalaries.ContainsKey(nouveauSalarie.NumeroSecuriteSociale))
            {
                Console.WriteLine($"Erreur: Un salarié avec le numéro SS {nouveauSalarie.NumeroSecuriteSociale} existe déjà.");
                return false;
            }

            _tousLesSalaries.Add(nouveauSalarie.NumeroSecuriteSociale, nouveauSalarie);

            if (!string.IsNullOrWhiteSpace(managerNumeroSS))
            {
                nouveauSalarie.ManagerNumeroSS = managerNumeroSS;
                var noeudManager = _organigramme.TrouverSalarie(managerNumeroSS);
                if (noeudManager != null)
                {
                    _organigramme.InsererSubordonne(noeudManager, nouveauSalarie);
                }
                else
                {
                    Console.WriteLine($"Avertissement: Manager avec ID {managerNumeroSS} non trouvé pour {nouveauSalarie}. Sera détaché.");
                }
            }
            else if (_organigramme.Racine == null)
            {
                _organigramme = new OrganigrammeNaire(nouveauSalarie);
                Console.WriteLine($"{nouveauSalarie} défini comme racine.");
            }

            Console.WriteLine($"Salarié {nouveauSalarie.Prenom} {nouveauSalarie.Nom} ajouté.");
            
            // Sauvegarder les modifications dans le fichier JSON
            try
            {
                SauvegarderSalariesEtOrganigramme();
                Console.WriteLine("Les modifications ont été sauvegardées dans le fichier JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attention: Erreur lors de la sauvegarde dans le fichier JSON: {ex.Message}");
            }
            
            return true;
        }

        /// <summary>
        /// Supprime un salarié par son numéro de sécurité sociale.
        /// </summary>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public bool SupprimerSalarie(string numeroSS)
        {
            if (!_tousLesSalaries.TryGetValue(numeroSS, out Salarie salarieASupprimer))
            {
                Console.WriteLine($"Erreur: Aucun salarié trouvé avec le numéro SS {numeroSS}.");
                return false;
            }

            // Supprimer de la liste des salariés
            _tousLesSalaries.Remove(numeroSS);

            // Reconstruire l'organigramme sans le salarié supprimé
            var salaries = _tousLesSalaries.Values.ToList();
            _organigramme = OrganigrammeNaire.ConstruireDepuisListe(salaries);

            Console.WriteLine($"Salarié {salarieASupprimer.Prenom} {salarieASupprimer.Nom} supprimé.");

            // Sauvegarder les modifications dans le fichier JSON
            try
            {
                SauvegarderSalariesEtOrganigramme();
                Console.WriteLine("Les modifications ont été sauvegardées dans le fichier JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attention: Erreur lors de la sauvegarde dans le fichier JSON: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// Recherche un salarié par son numéro de sécurité sociale.
        /// </summary>
        public Salarie RechercherParId(string numeroSS)
        {
            _tousLesSalaries.TryGetValue(numeroSS, out Salarie salarie);
            return salarie;
        }

        /// <summary>
        /// Recherche des salariés par leur nom de famille (insensible à la casse).
        /// </summary>
        public List<Salarie> RechercherParNom(string nom)
        {
            return _tousLesSalaries.Values
                .Where(s => s.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Affiche l'organigramme textuel.
        /// </summary>
        public void AfficherOrganigramme()
        {
            Console.WriteLine("\n--- Organigramme ---");
            if (_organigramme.Racine == null)
            {
                if (_tousLesSalaries.Any())
                    Console.WriteLine("Racine non définie. Employés existent mais structure hiérarchique incomplète ou multiple racines.");
                else
                    Console.WriteLine("Aucun salarié enregistré.");
                return;
            }
            _organigramme.AfficherOrganigramme();
            Console.WriteLine("--- Fin Organigramme ---");
        }

        /// <summary>
        /// Sauvegarde la liste de tous les salariés en JSON.
        /// </summary>
        public void SauvegarderSalariesEtOrganigramme(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? FichierSauvegarde;
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(_tousLesSalaries.Values.ToList(), options);
                File.WriteAllText(fichier, jsonString);
                Console.WriteLine($"Salariés sauvegardés avec succès dans {fichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des salariés dans {fichier}: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge les salariés depuis un fichier JSON et reconstruit l'organigramme.
        /// </summary>
        public void ChargerSalariesEtOrganigramme(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? FichierSauvegarde;
            if (!File.Exists(fichier))
            {
                Console.WriteLine($"Fichier de sauvegarde {fichier} non trouvé. Aucun salarié chargé.");
                _tousLesSalaries.Clear();
                _organigramme = new OrganigrammeNaire();
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(fichier);
                var options = new JsonSerializerOptions();
                var salariesCharges = JsonSerializer.Deserialize<List<Salarie>>(jsonString, options);

                _tousLesSalaries.Clear();

                if (salariesCharges != null)
                {
                    foreach (var salarie in salariesCharges)
                    {
                        if (!_tousLesSalaries.ContainsKey(salarie.NumeroSecuriteSociale))
                        {
                            _tousLesSalaries.Add(salarie.NumeroSecuriteSociale, salarie);
                        }
                        else
                        {
                            Console.WriteLine($"Avertissement: Salarié dupliqué ignoré lors du chargement (ID: {salarie.NumeroSecuriteSociale}).");
                        }
                    }

                    // Reconstruire l'organigramme
                    _organigramme = OrganigrammeNaire.ConstruireDepuisListe(_tousLesSalaries.Values.ToList());
                    Console.WriteLine($"Salariés chargés depuis {fichier}. {_tousLesSalaries.Count} salariés récupérés.");
                }
                else
                {
                    Console.WriteLine($"Avertissement : Le fichier {fichier} semble vide ou mal formaté. Aucune donnée chargée.");
                    _organigramme = new OrganigrammeNaire();
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Erreur de format JSON lors du chargement depuis {fichier}: {jsonEx.Message}");
                _tousLesSalaries.Clear();
                _organigramme = new OrganigrammeNaire();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur générale lors du chargement des salariés depuis {fichier}: {ex.Message}");
                _tousLesSalaries.Clear();
                _organigramme = new OrganigrammeNaire();
            }
        }

        /// <summary>
        /// Retourne une copie de la liste de tous les salariés.
        /// </summary>
        public List<Salarie> GetTousLesSalaries()
        {
            return _tousLesSalaries.Values.ToList();
        }

        /// <summary>
        /// Obtient les subordonnés directs d'un salarié.
        /// </summary>
        public List<Salarie> ObtenirSubordonnesDirects(string numeroSS)
        {
            return _organigramme.ObtenirSubordonnesDirects(numeroSS);
        }

        /// <summary>
        /// Obtient les collègues d'un salarié.
        /// </summary>
        public List<Salarie> ObtenirCollegues(string numeroSS)
        {
            return _organigramme.ObtenirCollegues(numeroSS);
        }
    }
}