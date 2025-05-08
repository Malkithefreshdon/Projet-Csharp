using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; 
using System.Text.Json;

namespace Projet.Modules
{
    /// <summary>
    /// Gère les opérations sur les salariés et l'organigramme de l'entreprise.
    /// </summary>
    public class SalarieManager
    {
        private readonly Dictionary<string, Salarie> _tousLesSalaries;
        private OrganigrammeNaire _organigramme;
        private readonly string _jsonPath;

        /// <summary>
        /// Initialise le gestionnaire de salariés et charge les données depuis le fichier JSON.
        /// </summary>
        public SalarieManager()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _jsonPath = Path.Combine(baseDirectory, "..", "..", "..", "Ressources", "salaries.json");
            _tousLesSalaries = new Dictionary<string, Salarie>(StringComparer.OrdinalIgnoreCase);
            _organigramme = new OrganigrammeNaire();
            ChargerSalariesEtOrganigramme();
        }

        /// <summary>
        /// Ajoute un nouveau salarié et l'intègre dans l'organigramme si un manager est spécifié.
        /// </summary>
        /// <param name="nouveauSalarie">Le salarié à ajouter.</param>
        /// <param name="managerNumeroSS">Numéro de sécurité sociale du manager (optionnel).</param>
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
                NoeudSalarie noeudManager = _organigramme.TrouverSalarie(managerNumeroSS);
                if (noeudManager != null)
                {
                    _organigramme.InsererSubordonne(noeudManager, nouveauSalarie);
                    Console.WriteLine($"Salarié ajouté sous le manager {noeudManager.Salarie.Nom}");
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
            
            // Sauvegarder et afficher l'organigramme
            try
            {
                SauvegarderSalariesEtOrganigramme();
                Console.WriteLine("\nNouvelle structure de l'organigramme :");
                AfficherOrganigramme();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attention: Erreur lors de la sauvegarde: {ex.Message}");
            }
            
            return true;
        }

        /// <summary>
        /// Supprime un salarié par son numéro de sécurité sociale.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du salarié à supprimer.</param>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public bool SupprimerSalarie(string numeroSS)
        {
            if (!_tousLesSalaries.TryGetValue(numeroSS, out Salarie salarieASupprimer))
            {
                Console.WriteLine($"Erreur: Aucun salarié trouvé avec le numéro SS {numeroSS}.");
                return false;
            }

            // Stocker les informations importantes avant la suppression
            string ancienManagerSS = salarieASupprimer.ManagerNumeroSS;
            List<Salarie> subordonnes = ObtenirSubordonnesDirects(numeroSS);
            Salarie manager = string.IsNullOrEmpty(ancienManagerSS) ? null : RechercherParId(ancienManagerSS);
            string ancienPoste = salarieASupprimer.Poste;
            decimal ancienSalaire = salarieASupprimer.Salaire;

            // Supprimer d'abord le salarié
            _tousLesSalaries.Remove(numeroSS);

            Console.WriteLine($"\nVous allez supprimer : {salarieASupprimer.Nom} ({ancienPoste})");

            if (!string.IsNullOrEmpty(ancienPoste))
            {
                Console.Write($"\nVoulez-vous remplacer le poste de {ancienPoste} ? (O/N) : ");
                if (Console.ReadLine()?.ToUpper() == "O")
                {
                    Console.WriteLine("\nComment souhaitez-vous remplacer ce poste ?");
                    Console.WriteLine("1. Promouvoir un subordonné existant");
                    Console.WriteLine("2. Ajouter un nouveau salarié");
                    Console.Write("\nVotre choix : ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            if (subordonnes.Any())
                            {
                                Console.WriteLine("\nSubordonnés disponibles :");
                                for (int i = 0; i < subordonnes.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}. {subordonnes[i].Nom} ({subordonnes[i].Poste})");
                                }
                                Console.Write("\nChoisissez le numéro du subordonné à promouvoir : ");
                                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= subordonnes.Count)
                                {
                                    Salarie promu = subordonnes[choix - 1];
                                    promu.Poste = ancienPoste;
                                    promu.ManagerNumeroSS = ancienManagerSS;

                                    // Gérer récursivement les subordonnés du promu
                                    GererSubordonnesRecursif(promu);

                                    // Réaffecter les autres subordonnés au promu
                                    foreach (Salarie sub in subordonnes)
                                    {
                                        if (sub != promu)
                                        {
                                            sub.ManagerNumeroSS = promu.NumeroSecuriteSociale;
                                        }
                                    }
                                    Console.WriteLine($"\n{promu.Nom} a été promu au poste de {promu.Poste}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\nAucun subordonné disponible pour la promotion.");
                            }
                            break;

                        case "2":
                            Console.WriteLine("\nEntrez les informations du nouveau salarié :");
                            
                            Console.Write("Numéro de sécurité sociale : ");
                            string newNumeroSS = Console.ReadLine();
                            
                            if (string.IsNullOrWhiteSpace(newNumeroSS))
                            {
                                Console.WriteLine("Numéro SS invalide.");
                                break;
                            }

                            Console.Write("Nom : ");
                            string newNom = Console.ReadLine();
                            
                            Console.Write("Prénom : ");
                            string newPrenom = Console.ReadLine();

                            Console.Write("Date de naissance (JJ/MM/AAAA) : ");
                            if (!DateTime.TryParse(Console.ReadLine(), out DateTime newDateNaissance))
                            {
                                Console.WriteLine("Format de date invalide.");
                                break;
                            }

                            Console.Write("Date d'entrée dans la société (JJ/MM/AAAA) : ");
                            if (!DateTime.TryParse(Console.ReadLine(), out DateTime newDateEntree))
                            {
                                Console.WriteLine("Format de date invalide.");
                                break;
                            }

                            // Créer et ajouter le nouveau salarié
                            Salarie nouveauSalarie = new Salarie
                            {
                                NumeroSecuriteSociale = newNumeroSS,
                                Nom = newNom,
                                Prenom = newPrenom,
                                Poste = ancienPoste,
                                DateNaissance = newDateNaissance,
                                DateEntreeSociete = newDateEntree,
                                ManagerNumeroSS = ancienManagerSS,
                                Salaire = ancienSalaire // Hérite du même salaire
                            };

                            _tousLesSalaries.Add(newNumeroSS, nouveauSalarie);

                            // Gérer récursivement les subordonnés existants
                            GererSubordonnesRecursif(nouveauSalarie);

                            // Réaffecter les subordonnés qui n'ont pas été réaffectés ailleurs
                            foreach (Salarie sub in subordonnes)
                            {
                                if (sub.ManagerNumeroSS == numeroSS)
                                {
                                    sub.ManagerNumeroSS = newNumeroSS;
                                    Console.WriteLine($"- {sub.Nom} réaffecté à {newNom}");
                                }
                            }

                            Console.WriteLine($"\nNouveau salarié {newNom} ajouté en remplacement de {salarieASupprimer.Nom}");
                            break;
                    }
                }
                else
                {
                    Console.Write("\nVoulez-vous réaffecter les subordonnés au manager supérieur ? (O/N) : ");
                    bool reaffecterAuManager = Console.ReadLine()?.ToUpper() == "O";

                    if (reaffecterAuManager && manager != null)
                    {
                        foreach (Salarie subordonne in subordonnes)
                        {
                            subordonne.ManagerNumeroSS = manager.NumeroSecuriteSociale;
                            Console.WriteLine($"- {subordonne.Nom} réaffecté à {manager.Nom}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nLes subordonnés deviendront indépendants.");
                        foreach (Salarie subordonne in subordonnes)
                        {
                            subordonne.ManagerNumeroSS = null;
                            Console.WriteLine($"- {subordonne.Nom} devient indépendant");
                        }
                    }
                }
            }

            // Reconstruire et afficher l'organigramme
            _organigramme = OrganigrammeNaire.ConstruireDepuisListe(_tousLesSalaries.Values.ToList());
            try
            {
                SauvegarderSalariesEtOrganigramme();
                Console.WriteLine("\nNouvelle structure de l'organigramme :");
                AfficherOrganigramme();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attention: Erreur lors de la sauvegarde: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// Gère récursivement les subordonnés d'un salarié.
        /// </summary>
        /// <param name="salarie">Le salarié dont on gère les subordonnés.</param>
        private void GererSubordonnesRecursif(Salarie salarie)
        {
            List<Salarie> subordonnes = ObtenirSubordonnesDirects(salarie.NumeroSecuriteSociale);
            if (!subordonnes.Any()) return;

            Console.WriteLine($"\nGestion des subordonnés de {salarie.Nom} ({salarie.Poste}) :");
            Console.WriteLine($"Il y a {subordonnes.Count} subordonné(s) à réaffecter.");
            Console.Write("Voulez-vous gérer leur réaffectation ? (O/N) : ");
            
            if (Console.ReadLine()?.ToUpper() == "O")
            {
                foreach (Salarie subordonne in subordonnes)
                {
                    Console.WriteLine($"\nGestion de : {subordonne.Nom} ({subordonne.Poste})");
                    Console.WriteLine("1. Promouvoir à un autre poste");
                    Console.WriteLine("2. Réaffecter à un autre manager");
                    Console.WriteLine("3. Laisser sous le nouveau manager");
                    Console.Write("\nVotre choix : ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Write("Nouveau poste : ");
                            string nouveauPoste = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(nouveauPoste))
                            {
                                subordonne.Poste = nouveauPoste;
                                Console.WriteLine($"{subordonne.Nom} promu au poste de {nouveauPoste}");
                                // Gérer récursivement ses propres subordonnés
                                GererSubordonnesRecursif(subordonne);
                            }
                            break;

                        case "2":
                            Console.Write("Numéro SS du nouveau manager : ");
                            string nouveauManagerSS = Console.ReadLine();
                            Salarie nouveauManager = RechercherParId(nouveauManagerSS);
                            if (nouveauManager != null)
                            {
                                subordonne.ManagerNumeroSS = nouveauManagerSS;
                                Console.WriteLine($"{subordonne.Nom} réaffecté sous {nouveauManager.Nom}");
                            }
                            else
                            {
                                Console.WriteLine("Manager non trouvé, le subordonné reste sous le même manager");
                            }
                            break;

                        case "3":
                            Console.WriteLine($"{subordonne.Nom} reste sous le même manager");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Les subordonnés restent sous leur manager actuel.");
            }
        }

        /// <summary>
        /// Recherche un salarié par son numéro de sécurité sociale.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du salarié recherché.</param>
        /// <returns>Le salarié correspondant ou null.</returns>
        public Salarie RechercherParId(string numeroSS)
        {
            _tousLesSalaries.TryGetValue(numeroSS, out Salarie salarie);
            return salarie;
        }

        /// <summary>
        /// Recherche des salariés par leur nom de famille (insensible à la casse).
        /// </summary>
        /// <param name="nom">Nom de famille à rechercher.</param>
        /// <returns>Liste des salariés trouvés.</returns>
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
        /// <param name="cheminFichier">Chemin du fichier de sauvegarde (optionnel).</param>
        public void SauvegarderSalariesEtOrganigramme(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? _jsonPath;
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
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
        /// <param name="cheminFichier">Chemin du fichier à charger (optionnel).</param>
        public void ChargerSalariesEtOrganigramme(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? _jsonPath;
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
                JsonSerializerOptions options = new JsonSerializerOptions();
                List<Salarie> salariesCharges = JsonSerializer.Deserialize<List<Salarie>>(jsonString, options);

                _tousLesSalaries.Clear();

                if (salariesCharges != null)
                {
                    foreach (Salarie salarie in salariesCharges)
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
        /// <returns>Liste de tous les salariés.</returns>
        public List<Salarie> GetTousLesSalaries()
        {
            return _tousLesSalaries.Values.ToList();
        }

        /// <summary>
        /// Obtient les subordonnés directs d'un salarié.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du manager.</param>
        /// <returns>Liste des subordonnés directs.</returns>
        public List<Salarie> ObtenirSubordonnesDirects(string numeroSS)
        {
            return _organigramme.ObtenirSubordonnesDirects(numeroSS);
        }

        /// <summary>
        /// Obtient les collègues d'un salarié.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du salarié.</param>
        /// <returns>Liste des collègues.</returns>
        public List<Salarie> ObtenirCollegues(string numeroSS)
        {
            return _organigramme.ObtenirCollegues(numeroSS);
        }
    }
}