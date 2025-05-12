using System;

namespace Projet.Modules
{
    public static class ConsoleHelper
    {
        public static void AfficherTitre(string titre)
        {
            Console.WriteLine(new string('=', titre.Length + 4));
            Console.WriteLine($"| {titre} |");
            Console.WriteLine(new string('=', titre.Length + 4));
            Console.WriteLine();
        }

        public static void AfficherMessage(string message, ConsoleColor couleur = ConsoleColor.White)
        {
            ConsoleColor couleurOriginale = Console.ForegroundColor;
            Console.ForegroundColor = couleur;
            Console.WriteLine(message);
            Console.ForegroundColor = couleurOriginale;
        }

        public static void AfficherErreur(string message)
        {
            AfficherMessage(message, ConsoleColor.Red);
        }

        public static void AfficherSucces(string message)
        {
            AfficherMessage(message, ConsoleColor.Green);
        }

        public static void AfficherAvertissement(string message)
        {
            AfficherMessage(message, ConsoleColor.Yellow);
        }

        public static void AfficherTableau(string[] enTetes, string[][] donnees)
        {
            int[] largeurs = new int[enTetes.Length];
            for (int i = 0; i < enTetes.Length; i++)
            {
                largeurs[i] = enTetes[i].Length;
                foreach (string[] ligne in donnees)
                {
                    if (i < ligne.Length && ligne[i].Length > largeurs[i])
                    {
                        largeurs[i] = ligne[i].Length;
                    }
                }
            }

            Console.Write("|");
            for (int i = 0; i < enTetes.Length; i++)
            {
                Console.Write($" {enTetes[i].PadRight(largeurs[i])} |");
            }
            Console.WriteLine();

            Console.Write("+");
            for (int i = 0; i < enTetes.Length; i++)
            {
                Console.Write(new string('-', largeurs[i] + 2) + "+");
            }
            Console.WriteLine();

            foreach (string[] ligne in donnees)
            {
                Console.Write("|");
                for (int i = 0; i < enTetes.Length; i++)
                {
                    string valeur = i < ligne.Length ? ligne[i] : "";
                    Console.Write($" {valeur.PadRight(largeurs[i])} |");
                }
                Console.WriteLine();
            }
        }
    }
}