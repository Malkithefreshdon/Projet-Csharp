using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Projet.Modules
{
    /// <summary>
    /// Représente un nœud de l'organigramme contenant un salarié et ses liens hiérarchiques.
    /// </summary>
    public class NoeudSalarie
    {
        private Salarie salarie;
        private NoeudSalarie pere;
        private NoeudSalarie frere;
        private NoeudSalarie successeur;

        /// <summary>
        /// Initialise un nouveau nœud avec le salarié donné.
        /// </summary>
        /// <param name="salarie">Le salarié associé à ce nœud.</param>
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

        /// <summary>
        /// Indique si ce nœud est une feuille (n'a pas de subordonné).
        /// </summary>
        /// <returns>True si le nœud est une feuille, sinon False.</returns>
        public bool EstFeuille()
        {
            return this.successeur == null;
        }
    }

    /// <summary>
    /// Représente un organigramme n-aire de l'entreprise.
    /// </summary>
    public class OrganigrammeNaire
    {
        private NoeudSalarie racine;

        /// <summary>
        /// Initialise un organigramme vide.
        /// </summary>
        public OrganigrammeNaire()
        {
            this.racine = null;
        }

        /// <summary>
        /// Initialise un organigramme avec un salarié racine.
        /// </summary>
        /// <param name="salarie">Le salarié racine.</param>
        public OrganigrammeNaire(Salarie salarie)
        {
            this.racine = new NoeudSalarie(salarie);
        }

        public NoeudSalarie Racine
        {
            get { return this.racine; }
            set { this.racine = value; }
        }

        /// <summary>
        /// Insère un subordonné direct (successeur) sous un manager donné.
        /// </summary>
        /// <param name="manager">Le nœud manager.</param>
        /// <param name="subordonne">Le salarié subordonné à ajouter.</param>
        /// <returns>True si l'insertion a réussi, sinon False.</returns>
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

        /// <summary>
        /// Insère un collègue (frère) à la suite d'un nœud donné.
        /// </summary>
        /// <param name="collegue">Le nœud de départ.</param>
        /// <param name="nouveauCollegue">Le salarié collègue à ajouter.</param>
        /// <returns>True si l'insertion a réussi, sinon False.</returns>
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

        /// <summary>
        /// Construit un organigramme à partir d'une liste de salariés.
        /// </summary>
        /// <param name="salaries">Liste des salariés.</param>
        /// <returns>Un organigramme n-aire construit à partir de la liste.</returns>
        public static OrganigrammeNaire ConstruireDepuisListe(List<Salarie> salaries)
        {
            if (salaries == null || salaries.Count == 0)
                return new OrganigrammeNaire();

            Salarie dg = salaries.Find(s => string.IsNullOrEmpty(s.ManagerNumeroSS));
            if (dg == null)
            {
                dg = salaries[0];
                dg.ManagerNumeroSS = null;
                Console.WriteLine($"Attention: Aucun directeur général trouvé. {dg.Nom} défini comme racine temporaire.");
            }

            OrganigrammeNaire organigramme = new OrganigrammeNaire(dg);
            Dictionary<string, NoeudSalarie> noeudsParSS = new Dictionary<string, NoeudSalarie>
            {
                { dg.NumeroSecuriteSociale, organigramme.Racine }
            };

            // Créer tous les nœuds et les lier
            foreach (Salarie salarie in salaries)
            {
                if (salarie == dg) continue;

                // Si le manager n'existe pas, rattacher à la racine
                if (string.IsNullOrEmpty(salarie.ManagerNumeroSS) || !noeudsParSS.ContainsKey(salarie.ManagerNumeroSS))
                {
                    Console.WriteLine($"Attention: Manager non trouvé pour {salarie.Nom}, rattachement à la racine.");
                    salarie.ManagerNumeroSS = dg.NumeroSecuriteSociale;
                    organigramme.InsererSubordonne(organigramme.Racine, salarie);
                    
                    NoeudSalarie nouveauNoeud = organigramme.Racine.Successeur;
                    while (nouveauNoeud.Frere != null)
                        nouveauNoeud = nouveauNoeud.Frere;
                    noeudsParSS[salarie.NumeroSecuriteSociale] = nouveauNoeud;
                }
                else
                {
                    NoeudSalarie noeudManager = noeudsParSS[salarie.ManagerNumeroSS];
                    organigramme.InsererSubordonne(noeudManager, salarie);
                    
                    NoeudSalarie nouveauNoeud = noeudManager.Successeur;
                    while (nouveauNoeud.Frere != null)
                        nouveauNoeud = nouveauNoeud.Frere;
                    noeudsParSS[salarie.NumeroSecuriteSociale] = nouveauNoeud;
                }
            }

            return organigramme;
        }

        /// <summary>
        /// Affiche l'organigramme à partir d'un nœud donné (ou de la racine si non précisé).
        /// </summary>
        /// <param name="noeud">Le nœud de départ.</param>
        /// <param name="prefixe">Le préfixe d'affichage (indentation).</param>
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

        /// <summary>
        /// Trouve un salarié dans l'organigramme à partir de son numéro de sécurité sociale.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du salarié recherché.</param>
        /// <returns>Le nœud correspondant au salarié, ou null si non trouvé.</returns>
        public NoeudSalarie TrouverSalarie(string numeroSS)
        {
            return TrouverSalarieRecursif(racine, numeroSS);
        }

        /// <summary>
        /// Méthode récursive pour trouver un salarié dans l'organigramme.
        /// </summary>
        /// <param name="noeud">Nœud courant.</param>
        /// <param name="numeroSS">Numéro de sécurité sociale recherché.</param>
        /// <returns>Le nœud correspondant ou null.</returns>
        private NoeudSalarie TrouverSalarieRecursif(NoeudSalarie noeud, string numeroSS)
        {
            if (noeud == null) return null;
            if (noeud.Salarie.NumeroSecuriteSociale == numeroSS) return noeud;

            NoeudSalarie dansSucesseurs = TrouverSalarieRecursif(noeud.Successeur, numeroSS);
            if (dansSucesseurs != null) return dansSucesseurs;

            return TrouverSalarieRecursif(noeud.Frere, numeroSS);
        }

        /// <summary>
        /// Retourne la liste des subordonnés directs d'un salarié.
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du manager.</param>
        /// <returns>Liste des salariés subordonnés directs.</returns>
        public List<Salarie> ObtenirSubordonnesDirects(string numeroSS)
        {
            NoeudSalarie noeud = TrouverSalarie(numeroSS);
            if (noeud == null) return new List<Salarie>();

            List<Salarie> subordonnes = new List<Salarie>();
            NoeudSalarie successeur = noeud.Successeur;
            while (successeur != null)
            {
                subordonnes.Add(successeur.Salarie);
                successeur = successeur.Frere;
            }

            return subordonnes;
        }

        /// <summary>
        /// Retourne la liste des collègues d'un salarié (ayant le même manager).
        /// </summary>
        /// <param name="numeroSS">Numéro de sécurité sociale du salarié.</param>
        /// <returns>Liste des collègues.</returns>
        public List<Salarie> ObtenirCollegues(string numeroSS)
        {
            NoeudSalarie noeud = TrouverSalarie(numeroSS);
            if (noeud == null || noeud.Pere == null) return new List<Salarie>();

            List<Salarie> collegues = new List<Salarie>();
            NoeudSalarie premierFrere = noeud.Pere.Successeur;

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