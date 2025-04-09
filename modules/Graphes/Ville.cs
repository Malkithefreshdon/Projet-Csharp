using System;

namespace modules.Graphes
{
    public class Ville : IEquatable<Ville>
    {
        public string Nom { get; private set; }

        // Optionnel: Coordonnées géographiques
        // public double Latitude { get; set; }
        // public double Longitude { get; set; }

        public Ville(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le nom de la ville ne peut pas être vide.", nameof(nom));
            Nom = nom;
        }

        public override string ToString()
        {
            return Nom;
        }

        public bool Equals(Ville other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Nom, other.Nom, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Ville)obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Nom);
        }

        public static bool operator ==(Ville left, Ville right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Ville left, Ville right)
        {
            return !Equals(left, right);
        }
    }
}