using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Projet.Modules
{
    public class NoeudSalarie
    {
        private Salarie salarie;
        private NoeudSalarie pere;
        private NoeudSalarie frere;
        private NoeudSalarie successeur;

        public NoeudSalarie(Salarie salarie)
        {
            this.salarie = salarie;
            this.pere = null;
            this.frere = null;
            this.successeur = null;
        }

        public Salarie Salarie
        {
            get { return this.salarie; }
            set { this.salarie = value; }
        }

        public NoeudSalarie Pere
        {
            get { return this.pere; }
            set { this.pere = value; }
        }

        public NoeudSalarie Frere
        {
            get { return this.frere; }
            set { this.frere = value; }
        }

        public NoeudSalarie Successeur
        {
            get { return this.successeur; }
            set { this.successeur = value; }
        }

        public bool EstFeuille()
        {
            return this.successeur == null;
        }
    }

    public class OrganigrammeNaire
    {
        private NoeudSalarie racine;

        public OrganigrammeNaire()
        {
            this.racine = null;
        }

        public OrganigrammeNaire(Salarie salarie)
        {
            this.racine = new NoeudSalarie(salarie);
        }

        public NoeudSalarie Racine
        {
            get { return this.racine; }
            set { this.racine = value; }
        }

        // Méthode pour insérer un subordonné direct (successeur)
        public bool InsererSubordonne(NoeudSalarie manager, Salarie subordonne)
        {
            if (manager == null) return false;

            if (manager.Successeur == null)
            {
                NoeudSalarie nouveauSubordonne = new NoeudSalarie(subordonne);
                nouveauSubordonne.Pere = manager;
                manager.Successeur = nouveauSubordonne;
                return true;
            }

            return InsererFrere(manager.Successeur, subordonne);
        }

        // Méthode pour insérer un collègue (frère)
        public bool InsererFrere(NoeudSalarie collegue, Salarie nouveauCollegue)
        {
            if (collegue == null) return false;

            NoeudSalarie courant = collegue;
            while (courant.Frere != null)
            {
                courant = courant.Frere;
            }

            NoeudSalarie nouveauFrere = new NoeudSalarie(nouveauCollegue);
            nouveauFrere.Pere = collegue.Pere;
            courant.Frere = nouveauFrere;
            return true;
        }

        // Méthode pour construire l'organigramme à partir d'une liste de salariés
        public static OrganigrammeNaire ConstruireDepuisListe(List<Salarie> salaries)
        {
            if (salaries == null || salaries.Count == 0)
                return new OrganigrammeNaire();

            var dg = salaries.Find(s => string.IsNullOrEmpty(s.ManagerNumeroSS));
            if (dg == null)
            {
                dg = salaries.First();
                dg.ManagerNumeroSS = null;
                Console.WriteLine($"Attention: Aucun directeur général trouvé. {dg.Nom} défini comme racine temporaire.");
            }

            var organigramme = new OrganigrammeNaire(dg);
            var noeudsParSS = new Dictionary<string, NoeudSalarie>
            {
                { dg.NumeroSecuriteSociale, organigramme.Racine }
            };

            // Créer tous les nœuds et les lier
            foreach (var salarie in salaries)
            {
                if (salarie == dg) continue;

                // Si le manager n'existe pas, rattacher à la racine
                if (string.IsNullOrEmpty(salarie.ManagerNumeroSS) || !noeudsParSS.ContainsKey(salarie.ManagerNumeroSS))
                {
                    Console.WriteLine($"Attention: Manager non trouvé pour {salarie.Nom}, rattachement à la racine.");
                    salarie.ManagerNumeroSS = dg.NumeroSecuriteSociale;
                    organigramme.InsererSubordonne(organigramme.Racine, salarie);
                    
                    var nouveauNoeud = organigramme.Racine.Successeur;
                    while (nouveauNoeud.Frere != null)
                        nouveauNoeud = nouveauNoeud.Frere;
                    noeudsParSS[salarie.NumeroSecuriteSociale] = nouveauNoeud;
                }
                else
                {
                    var noeudManager = noeudsParSS[salarie.ManagerNumeroSS];
                    organigramme.InsererSubordonne(noeudManager, salarie);
                    
                    var nouveauNoeud = noeudManager.Successeur;
                    while (nouveauNoeud.Frere != null)
                        nouveauNoeud = nouveauNoeud.Frere;
                    noeudsParSS[salarie.NumeroSecuriteSociale] = nouveauNoeud;
                }
            }

            return organigramme;
        }

        // Méthode pour afficher l'organigramme
        public void AfficherOrganigramme(NoeudSalarie noeud = null, string prefixe = "")
        {
            if (noeud == null) noeud = racine;
            if (noeud == null) return;

            Console.WriteLine($"{prefixe}├── {noeud.Salarie}");

            if (noeud.Successeur != null)
            {
                AfficherOrganigramme(noeud.Successeur, prefixe + "│   ");
            }

            if (noeud.Frere != null)
            {
                AfficherOrganigramme(noeud.Frere, prefixe);
            }
        }

        // Méthode pour trouver un salarié par son numéro de sécurité sociale
        public NoeudSalarie TrouverSalarie(string numeroSS)
        {
            return TrouverSalarieRecursif(racine, numeroSS);
        }

        private NoeudSalarie TrouverSalarieRecursif(NoeudSalarie noeud, string numeroSS)
        {
            if (noeud == null) return null;
            if (noeud.Salarie.NumeroSecuriteSociale == numeroSS) return noeud;

            var dansSucesseurs = TrouverSalarieRecursif(noeud.Successeur, numeroSS);
            if (dansSucesseurs != null) return dansSucesseurs;

            return TrouverSalarieRecursif(noeud.Frere, numeroSS);
        }

        // Méthode pour obtenir tous les subordonnés directs d'un salarié
        public List<Salarie> ObtenirSubordonnesDirects(string numeroSS)
        {
            var noeud = TrouverSalarie(numeroSS);
            if (noeud == null) return new List<Salarie>();

            var subordonnes = new List<Salarie>();
            var successeur = noeud.Successeur;
            while (successeur != null)
            {
                subordonnes.Add(successeur.Salarie);
                successeur = successeur.Frere;
            }

            return subordonnes;
        }

        // Méthode pour obtenir tous les collègues d'un salarié
        public List<Salarie> ObtenirCollegues(string numeroSS)
        {
            var noeud = TrouverSalarie(numeroSS);
            if (noeud == null || noeud.Pere == null) return new List<Salarie>();

            var collegues = new List<Salarie>();
            var premierFrere = noeud.Pere.Successeur;

            while (premierFrere != null)
            {
                if (premierFrere.Salarie.NumeroSecuriteSociale != numeroSS)
                {
                    collegues.Add(premierFrere.Salarie);
                }
                premierFrere = premierFrere.Frere;
            }

            return collegues;
        }
    }
}