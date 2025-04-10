
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Projet.Modules
{
    public class Organigramme
    {
        public Salarie Salarie { get; private set; }
        public List<Organigramme> Enfants { get; private set; }

        // Optionnel: Référence au parent pour faciliter certaines opérations (comme la suppression)
        // public Organigramme Parent { get; private set; }

        public Organigramme(Salarie salarie)
        {
            Salarie = salarie ?? throw new ArgumentNullException(nameof(salarie));
            Enfants = new List<Organigramme>();
        }

        /// <summary>
        /// Ajoute un nœud représentant un subordonné direct à ce nœud.
        /// </summary>
        public void AjouterSubordonne(Organigramme enfant)
        {
            if (enfant == null) throw new ArgumentNullException(nameof(enfant));
            if (!Enfants.Contains(enfant))
            {
                Enfants.Add(enfant);
                // Optionnel: Définir le parent de l'enfant
                // enfant.Parent = this;
            }
        }

        /// <summary>
        /// Supprime un nœud subordonné direct basé sur l'instance du nœud.
        /// </summary>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public bool SupprimerSubordonne(Organigramme enfant)
        {
            if (enfant == null) return false;
            bool removed = Enfants.Remove(enfant);
            // if (removed) {
            //    Optionnel: Réinitialiser le parent de l'enfant
            //    enfant.Parent = null;
            // }
            return removed;
        }

        /// <summary>
        /// Supprime un nœud subordonné direct basé sur le Salarie qu'il contient.
        /// </summary>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public bool SupprimerSubordonne(Salarie salarieEnfant)
        {
            if (salarieEnfant == null) return false;
            Organigramme noeudASupprimer = Enfants.FirstOrDefault(n => n.Salarie.Equals(salarieEnfant));
            if (noeudASupprimer != null)
            {
                return SupprimerSubordonne(noeudASupprimer);
            }
            return false;
        }


        public override string ToString()
        {
            return Salarie.ToString();
        }
    }
}