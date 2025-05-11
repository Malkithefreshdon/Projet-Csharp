using OfficeOpenXml;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    /// <summary>
    /// Classe permettant de visualiser un graphe et ses chemins.
    /// </summary>
    public class GrapheVisualisation
    {
        private readonly Graphe graphe;
        private readonly int largeur;
        private readonly int hauteur;
        private readonly Dictionary<Ville, SKPoint> positions;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GrapheVisualisation.
        /// </summary>
        /// <param name="graphe">Le graphe à visualiser.</param>
        /// <param name="largeur">La largeur de l'image en pixels.</param>
        /// <param name="hauteur">La hauteur de l'image en pixels.</param>
        public GrapheVisualisation(Graphe graphe, int largeur = 1000, int hauteur = 1000)
        {
            this.graphe = graphe;
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.positions = new Dictionary<Ville, SKPoint>();
            GenererPositions();
        }

        /// <summary>
        /// Génère les positions des villes sur le canvas.
        /// </summary>
        private void GenererPositions()
        {
            List<Ville> villes = this.graphe.GetToutesLesVilles().ToList();
            Dictionary<string, SKPoint> positionsPredefinies = ChargerPositionsDepuisXlsx("Ressources/distances_villes_france_2.xlsx");

            foreach (Ville ville in villes)
            {
                if (positionsPredefinies.TryGetValue(ville.Nom, out SKPoint position))
                {
                    // Utiliser les positions prédéfinies
                    this.positions[ville] = new SKPoint(position.X, position.Y);
                }
                else
                {
                    Console.WriteLine($"Position non définie pour la ville : {ville.Nom}. Une position aléatoire sera utilisée.");
                    this.positions[ville] = new SKPoint(new Random().Next(this.largeur), new Random().Next(this.hauteur));
                }
            }

            // Normaliser les positions pour les adapter à la taille du canvas
            NormaliserPositions();
            AjouterEspacementMinimum(50); // Espacement minimum de 50 pixels
        }

        /// <summary>
        /// Charge les positions des villes depuis un fichier Excel.
        /// </summary>
        /// <param name="cheminFichier">Le chemin du fichier Excel.</param>
        /// <returns>Un dictionnaire associant les noms de villes à leurs positions.</returns>
        private Dictionary<string, SKPoint> ChargerPositionsDepuisXlsx(string cheminFichier)
        {
            Dictionary<string, SKPoint> positions = new Dictionary<string, SKPoint>();

            FileInfo fichierInfo = new FileInfo(cheminFichier);
            if (!fichierInfo.Exists)
            {
                Console.WriteLine($"Erreur : Le fichier Excel '{cheminFichier}' n'a pas été trouvé.");
                return positions;
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage(fichierInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        Console.WriteLine($"Erreur : Le fichier Excel '{cheminFichier}' est vide ou ne contient aucune feuille.");
                        return positions;
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow + 1; row <= endRow; row++)
                    {
                        string nomVilleA = worksheet.Cells[row, 1].Text?.Trim();
                        string nomVilleB = worksheet.Cells[row, 2].Text?.Trim();

                        if (double.TryParse(worksheet.Cells[row, 4].Text?.Trim(), out double latitudeA) &&
                            double.TryParse(worksheet.Cells[row, 5].Text?.Trim(), out double longitudeA))
                        {
                            if (!positions.ContainsKey(nomVilleA))
                            {
                                positions[nomVilleA] = ConvertirCoordonneesEnPosition(latitudeA, longitudeA);
                            }
                        }

                        if (double.TryParse(worksheet.Cells[row, 6].Text?.Trim(), out double latitudeB) &&
                            double.TryParse(worksheet.Cells[row, 7].Text?.Trim(), out double longitudeB))
                        {
                            if (!positions.ContainsKey(nomVilleB))
                            {
                                positions[nomVilleB] = ConvertirCoordonneesEnPosition(latitudeB, longitudeB);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des positions depuis le fichier Excel : {ex.Message}");
            }

            return positions;
        }

        /// <summary>
        /// Convertit des coordonnées géographiques en position sur le canvas.
        /// </summary>
        /// <param name="latitude">La latitude.</param>
        /// <param name="longitude">La longitude.</param>
        /// <returns>La position sur le canvas.</returns>
        private SKPoint ConvertirCoordonneesEnPosition(double latitude, double longitude)
        {
            // Convertir les coordonnées géographiques en positions dans le canvas
            float x = (float)((longitude + 180) / 360 * this.largeur); // Normalisation longitude
            float y = (float)((90 - latitude) / 180 * this.hauteur);   // Normalisation latitude
            return new SKPoint(x, y);
        }

        /// <summary>
        /// Dessine le graphe et l'enregistre dans un fichier.
        /// </summary>
        /// <param name="cheminFichier">Le chemin du fichier où enregistrer l'image.</param>
        public void DrawGraphe(string cheminFichier)
        {
            using (SKBitmap bitmap = new SKBitmap(this.largeur, this.hauteur))
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Dessiner les liens
                foreach (Ville ville in this.graphe.GetToutesLesVilles())
                {
                    foreach ((Ville voisin, double poids) in this.graphe.ObtenirVoisins(ville))
                    {
                        SKPoint debut = this.positions[ville];
                        SKPoint fin = this.positions[voisin];
                        canvas.DrawLine(debut, fin, new SKPaint { Color = SKColors.Gray, StrokeWidth = 2 });
                    }
                }

                // Dessiner les villes
                foreach (KeyValuePair<Ville, SKPoint> entry in this.positions)
                {
                    SKPoint point = entry.Value;
                    canvas.DrawCircle(point, 5, new SKPaint { Color = SKColors.Blue });
                    canvas.DrawText(entry.Key.Nom, point.X + 5, point.Y - 5, new SKPaint { Color = SKColors.Black, TextSize = 12 });
                }

                using (SKImage image = SKImage.FromBitmap(bitmap))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (System.IO.FileStream stream = System.IO.File.OpenWrite(cheminFichier))
                {
                    data.SaveTo(stream);
                }
            }
        }

        /// <summary>
        /// Dessine un chemin sur le graphe et l'enregistre dans un fichier.
        /// </summary>
        /// <param name="chemin">La liste des villes formant le chemin.</param>
        /// <param name="cheminFichier">Le chemin du fichier où enregistrer l'image.</param>
        /// <param name="couleur">La couleur du chemin.</param>
        public void DrawPath(List<Ville> chemin, string cheminFichier, SKColor couleur)
        {
            using (SKBitmap bitmap = new SKBitmap(this.largeur, this.hauteur))
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Dessiner les liens
                foreach (Ville ville in this.graphe.GetToutesLesVilles())
                {
                    foreach ((Ville voisin, double poids) in this.graphe.ObtenirVoisins(ville))
                    {
                        SKPoint debut = this.positions[ville];
                        SKPoint fin = this.positions[voisin];
                        canvas.DrawLine(debut, fin, new SKPaint { Color = SKColors.Gray, StrokeWidth = 2 });
                    }
                }

                // Dessiner le chemin
                for (int i = 0; i < chemin.Count - 1; i++)
                {
                    SKPoint debut = this.positions[chemin[i]];
                    SKPoint fin = this.positions[chemin[i + 1]];
                    canvas.DrawLine(debut, fin, new SKPaint { Color = couleur, StrokeWidth = 4 });
                }

                // Dessiner les villes
                foreach (KeyValuePair<Ville, SKPoint> entry in this.positions)
                {
                    SKPoint point = entry.Value;
                    canvas.DrawCircle(point, 5, new SKPaint { Color = SKColors.Blue });
                    canvas.DrawText(entry.Key.Nom, point.X + 5, point.Y - 5, new SKPaint { Color = SKColors.Black, TextSize = 12 });
                }

                using (SKImage image = SKImage.FromBitmap(bitmap))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (System.IO.FileStream stream = System.IO.File.OpenWrite(cheminFichier))
                {
                    data.SaveTo(stream);
                }
            }
        }

        /// <summary>
        /// Normalise les positions des villes pour les adapter à la taille du canvas.
        /// </summary>
        private void NormaliserPositions()
        {
            if (this.positions.Count == 0) return;

            // Trouver les limites des positions actuelles
            float minX = this.positions.Values.Min(p => p.X);
            float maxX = this.positions.Values.Max(p => p.X);
            float minY = this.positions.Values.Min(p => p.Y);
            float maxY = this.positions.Values.Max(p => p.Y);

            // Calculer les facteurs d'échelle
            float scaleX = this.largeur / (maxX - minX);
            float scaleY = this.hauteur / (maxY - minY);
            float scale = Math.Min(scaleX, scaleY) * 0.9f; // Réduction pour ajouter des marges

            // Normaliser les positions
            foreach (Ville ville in this.positions.Keys.ToList())
            {
                SKPoint point = this.positions[ville];
                float normalizedX = (point.X - minX) * scale + this.largeur * 0.05f; // Ajouter une marge
                float normalizedY = (point.Y - minY) * scale + this.hauteur * 0.05f; // Ajouter une marge
                this.positions[ville] = new SKPoint(normalizedX, normalizedY);
            }
        }

        /// <summary>
        /// Ajoute un espacement minimum entre les villes pour éviter les superpositions.
        /// </summary>
        /// <param name="espacement">L'espacement minimum souhaité entre les villes.</param>
        private void AjouterEspacementMinimum(float espacement)
        {
            bool positionsAjustees;

            do
            {
                positionsAjustees = false;

                foreach (Ville ville1 in this.positions.Keys.ToList())
                {
                    foreach (Ville ville2 in this.positions.Keys.ToList())
                    {
                        if (ville1 == ville2) continue;

                        SKPoint point1 = this.positions[ville1];
                        SKPoint point2 = this.positions[ville2];

                        float distance = (float)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
                        if (distance < espacement)
                        {
                            // Ajuster les positions pour augmenter l'espacement
                            float deltaX = (point2.X - point1.X) / distance * (espacement - distance);
                            float deltaY = (point2.Y - point1.Y) / distance * (espacement - distance);

                            this.positions[ville2] = new SKPoint(point2.X + deltaX, point2.Y + deltaY);
                            positionsAjustees = true;
                        }
                    }
                }
            } while (positionsAjustees);
        }
    }
}
