using modules.Graphes;
using System;

namespace modules.Graphes
{
    public class Lien
    {
        public Ville Origine { get; }
        public Ville Destination { get; }
        public double Poids { get; set; } 

        public Lien(Ville origine, Ville destination, double poids)
        {
            if (origine == null) throw new ArgumentNullException(nameof(origine));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (poids < 0) throw new ArgumentOutOfRangeException(nameof(poids), "Le poids d'un lien ne peut être négatif dans ce contexte.");

            Origine = origine;
            Destination = destination;
            Poids = poids;
        }

        public override string ToString()
        {
            return $"{Origine} -> {Destination} ({Poids})";
        }
    }
}