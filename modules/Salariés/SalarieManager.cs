using modules.Graphes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace modules.Salariés
{
    public class SalarieManager
    {
        private readonly Dictionary<string, Salarie> _tousLesSalaries;
        private readonly Dictionary<string, Organigramme> _noeudsOrganigramme;

        public Organigramme Racine { get; private set; }

        public string FichierSauvegarde { get; set; } = "salaries.json";

        public SalarieManager()
        {
            _tousLesSalaries = new Dictionary<string, Salarie>(StringComparer.OrdinalIgnoreCase);
            _noeudsOrganigramme = new Dictionary<string, Organigramme>(StringComparer.OrdinalIgnoreCase);
            Racine = null;
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

            Organigramme nouveauNoeud = new Organigramme(nouveauSalarie);
            _noeudsOrganigramme.Add(nouveauSalarie.NumeroSecuriteSociale, nouveauNoeud);

            if (!string.IsNullOrWhiteSpace(managerNumeroSS))
            {
                nouveauSalarie.ManagerNumeroSS = managerNumeroSS;
                if (_noeudsOrganigramme.TryGetValue(managerNumeroSS, out Organigramme managerNoeud))
                {
                    // Vérifier que le manager est bien un Responsable ? Optionnel.
                    // if (managerNoeud.Salarie is Responsable)
                    // {
                    managerNoeud.AjouterSubordonne(nouveauNoeud);
                    // } else { Console.WriteLine($"Avertissement: {managerNoeud.Salarie.Nom} n'est pas un Responsable."); }
                }
                else
                {
                    Console.WriteLine($"Avertissement: Manager avec ID {managerNumeroSS} non trouvé pour {nouveauSalarie}. Sera détaché.");

                }
            }
            else
            {
                if (Racine == null && _noeudsOrganigramme.Count == 1)
                {
                    Racine = nouveauNoeud;
                    Console.WriteLine($"{nouveauSalarie} défini comme racine.");
                }
                nouveauSalarie.ManagerNumeroSS = null;
            }

            Console.WriteLine($"Salarié {nouveauSalarie.Prenom} {nouveauSalarie.Nom} ajouté.");
            return true;
        }


        /// <summary>
        /// Supprime un salarié par son numéro de sécurité sociale.
        /// Le supprime aussi de l'organigramme. Les subordonnés deviennent orphelins dans l'arbre.
        /// </summary>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public bool SupprimerSalarie(string numeroSS)
        {
            if (!_tousLesSalaries.TryGetValue(numeroSS, out Salarie salarieASupprimer))
            {
                Console.WriteLine($"Erreur: Aucun salarié trouvé avec le numéro SS {numeroSS}.");
                return false;
            }

            _tousLesSalaries.Remove(numeroSS);

            if (_noeudsOrganigramme.TryGetValue(numeroSS, out Organigramme noeudASupprimer))
            {
                if (!string.IsNullOrWhiteSpace(salarieASupprimer.ManagerNumeroSS) &&
                    _noeudsOrganigramme.TryGetValue(salarieASupprimer.ManagerNumeroSS, out Organigramme parentNoeud))
                {
                    parentNoeud.SupprimerSubordonne(noeudASupprimer);
                }
                if (Racine == noeudASupprimer)
                {
                    Racine = null;
                    Console.WriteLine("La racine de l'organigramme a été supprimée.");
                }
                _noeudsOrganigramme.Remove(numeroSS);
            }

            foreach (var salarie in _tousLesSalaries.Values)
            {
                if (salarie.ManagerNumeroSS == numeroSS)
                {
                    salarie.ManagerNumeroSS = null;
                    Console.WriteLine($"Le salarié {salarie} est maintenant orphelin suite à la suppression de {numeroSS}.");
                }
            }


            Console.WriteLine($"Salarié {salarieASupprimer.Prenom} {salarieASupprimer.Nom} supprimé.");
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
        /// Affiche l'organigramme textuel à partir de la racine.
        /// </summary>
        public void AfficherOrganigramme()
        {
            Console.WriteLine("\n--- Organigramme ---");
            if (Racine == null)
            {
                if (_noeudsOrganigramme.Any())
                    Console.WriteLine("Racine non définie. Employés existent mais structure hiérarchique incomplète ou multiple racines.");
                else
                    Console.WriteLine("Aucun salarié enregistré.");
                return;
            }
            AfficherNoeud(Racine, 0);
            Console.WriteLine("--- Fin Organigramme ---");
        }

        private void AfficherNoeud(Organigramme noeud, int niveauIndentation)
        {
            StringBuilder indentation = new StringBuilder();
            for (int i = 0; i < niveauIndentation; i++)
            {
                indentation.Append("  ");
            }
            if (niveauIndentation > 0)
            {
                indentation.Append("└── ");
            }

            Console.WriteLine($"{indentation}{noeud.Salarie}");

            foreach (var enfant in noeud.Enfants.OrderBy(n => n.Salarie.Nom))
            {
                AfficherNoeud(enfant, niveauIndentation + 1);
            }
        }

        /// <summary>
        /// Sauvegarde la liste de tous les salariés (incluant l'info du manager) en JSON.
        /// </summary>
        public void SauvegarderSalariesEtOrganigramme(string cheminFichier = null)
        {
            string fichier = cheminFichier ?? FichierSauvegarde;
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
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
                _noeudsOrganigramme.Clear();
                Racine = null;
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(fichier);
                var options = new JsonSerializerOptions
                {
                    // Gérer le polymorphisme si nécessaire (si pas d'attributs JsonDerivedType)
                };
                var salariesCharges = JsonSerializer.Deserialize<List<Salarie>>(jsonString, options);

                _tousLesSalaries.Clear();
                _noeudsOrganigramme.Clear();
                Racine = null;

                if (salariesCharges != null)
                {
                    foreach (var salarie in salariesCharges)
                    {
                        Salarie instanceCorrecte = salarie;

                        if (instanceCorrecte != null && !_tousLesSalaries.ContainsKey(instanceCorrecte.NumeroSecuriteSociale))
                        {
                            _tousLesSalaries.Add(instanceCorrecte.NumeroSecuriteSociale, instanceCorrecte);
                            _noeudsOrganigramme.Add(instanceCorrecte.NumeroSecuriteSociale, new Organigramme(instanceCorrecte));
                        }
                        else { Console.WriteLine($"Avertissement: Salarié dupliqué ou invalide ignoré lors du chargement (ID: {salarie?.NumeroSecuriteSociale})."); }
                    }

                    foreach (var noeud in _noeudsOrganigramme.Values)
                    {
                        if (!string.IsNullOrWhiteSpace(noeud.Salarie.ManagerNumeroSS))
                        {
                            if (_noeudsOrganigramme.TryGetValue(noeud.Salarie.ManagerNumeroSS, out Organigramme managerNoeud))
                            {
                                managerNoeud.AjouterSubordonne(noeud);
                            }
                            else
                            {
                                Console.WriteLine($"Avertissement lors du chargement: Manager {noeud.Salarie.ManagerNumeroSS} non trouvé pour {noeud.Salarie}.");
                                noeud.Salarie.ManagerNumeroSS = null; // Rendre orphelin
                            }
                        }
                        else
                        {
                            if (Racine == null)
                            {
                                Racine = noeud;
                                Console.WriteLine($"Racine définie: {Racine.Salarie}");
                            }
                            else
                            {
                                Console.WriteLine($"Avertissement: Multiples racines potentielles détectées. {noeud.Salarie} est aussi sans manager.");
                            }
                        }
                    }
                    Console.WriteLine($"Salariés chargés depuis {fichier}. {_tousLesSalaries.Count} salariés récupérés.");
                    if (Racine == null && _tousLesSalaries.Any())
                    {
                        Console.WriteLine("Avertissement: Aucun salarié racine trouvé après chargement.");
                    }
                }
                else
                {
                    Console.WriteLine($"Avertissement : Le fichier {fichier} semble vide ou mal formaté. Aucune donnée chargée.");
                }


            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Erreur de format JSON lors du chargement depuis {fichier}: {jsonEx.Message}");
                _tousLesSalaries.Clear();
                _noeudsOrganigramme.Clear();
                Racine = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur générale lors du chargement des salariés depuis {fichier}: {ex.Message}");
                _tousLesSalaries.Clear();
                _noeudsOrganigramme.Clear();
                Racine = null;
            }
        }

        /// <summary>
        /// Retourne une copie de la liste de tous les salariés.
        /// </summary>
        public List<Salarie> GetTousLesSalaries()
        {
            return _tousLesSalaries.Values.ToList();
        }
    }
}