using OfficeOpenXml;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet.Modules
{
    public class GrapheVisualisation
    {
        private readonly Graphe _graphe;
        private readonly int _width;
        private readonly int _height;
        private readonly Dictionary<Ville, SKPoint> _positions;

        public GrapheVisualisation(Graphe graphe, int width = 1000, int height = 1000)
        {
            _graphe = graphe;
            _width = width;
            _height = height;
            _positions = new Dictionary<Ville, SKPoint>();
            GeneratePositions();
        }

        private void GeneratePositions()
        {
            var villes = _graphe.GetToutesLesVilles().ToList();
            var positionsPredefinies = ChargerPositionsDepuisXlsx("Ressources/distances_villes_france_2.xlsx");

            foreach (var ville in villes)
            {
                if (positionsPredefinies.TryGetValue(ville.Nom, out var position))
                {
                    // Utiliser les positions prédéfinies
                    _positions[ville] = new SKPoint(position.X, position.Y);
                }
                else
                {
                    Console.WriteLine($"Position non définie pour la ville : {ville.Nom}. Une position aléatoire sera utilisée.");
                    _positions[ville] = new SKPoint(new Random().Next(_width), new Random().Next(_height));
                }
            }

            // Normaliser les positions pour les adapter à la taille du canvas
            NormaliserPositions();
            AjouterEspacementMinimum(50); // Espacement minimum de 50 pixels
        }

        private Dictionary<string, SKPoint> ChargerPositionsDepuisXlsx(string cheminFichier)
        {
            var positions = new Dictionary<string, SKPoint>();

            FileInfo fichierInfo = new FileInfo(cheminFichier);
            if (!fichierInfo.Exists)
            {
                Console.WriteLine($"Erreur : Le fichier Excel '{cheminFichier}' n'a pas été trouvé.");
                return positions;
            }

            try
            {
                using (var package = new ExcelPackage(fichierInfo))
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

        private SKPoint ConvertirCoordonneesEnPosition(double latitude, double longitude)
        {
            // Convertir les coordonnées géographiques en positions dans le canvas
            // Exemple simple : normaliser les coordonnées dans les dimensions du canvas
            float x = (float)((longitude + 180) / 360 * _width); // Normalisation longitude
            float y = (float)((90 - latitude) / 180 * _height);  // Normalisation latitude
            return new SKPoint(x, y);
        }


        public void DrawGraphe(string filePath)
        {
            using (var bitmap = new SKBitmap(_width, _height))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Dessiner les liens
                foreach (var ville in _graphe.GetToutesLesVilles())
                {
                    foreach (var (voisin, poids) in _graphe.ObtenirVoisins(ville))
                    {
                        var start = _positions[ville];
                        var end = _positions[voisin];
                        canvas.DrawLine(start, end, new SKPaint { Color = SKColors.Gray, StrokeWidth = 2 });
                    }
                }

                // Dessiner les villes
                foreach (var ville in _positions.Keys)
                {
                    var point = _positions[ville];
                    canvas.DrawCircle(point, 5, new SKPaint { Color = SKColors.Blue });
                    canvas.DrawText(ville.Nom, point.X + 5, point.Y - 5, new SKPaint { Color = SKColors.Black, TextSize = 12 });
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }

        public void DrawPath(List<Ville> path, string filePath, SKColor color)
        {
            using (var bitmap = new SKBitmap(_width, _height))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Dessiner les liens
                foreach (var ville in _graphe.GetToutesLesVilles())
                {
                    foreach (var (voisin, poids) in _graphe.ObtenirVoisins(ville))
                    {
                        var start = _positions[ville];
                        var end = _positions[voisin];
                        canvas.DrawLine(start, end, new SKPaint { Color = SKColors.Gray, StrokeWidth = 2 });
                    }
                }

                // Dessiner le chemin
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var start = _positions[path[i]];
                    var end = _positions[path[i + 1]];
                    canvas.DrawLine(start, end, new SKPaint { Color = color, StrokeWidth = 4 });
                }

                // Dessiner les villes
                foreach (var ville in _positions.Keys)
                {
                    var point = _positions[ville];
                    canvas.DrawCircle(point, 5, new SKPaint { Color = SKColors.Blue });
                    canvas.DrawText(ville.Nom, point.X + 5, point.Y - 5, new SKPaint { Color = SKColors.Black, TextSize = 12 });
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }
        private void NormaliserPositions()
        {
            if (_positions.Count == 0) return;

            // Trouver les limites des positions actuelles
            float minX = _positions.Values.Min(p => p.X);
            float maxX = _positions.Values.Max(p => p.X);
            float minY = _positions.Values.Min(p => p.Y);
            float maxY = _positions.Values.Max(p => p.Y);

            // Calculer les facteurs d'échelle
            float scaleX = _width / (maxX - minX);
            float scaleY = _height / (maxY - minY);
            float scale = Math.Min(scaleX, scaleY) * 0.9f; // Réduction pour ajouter des marges

            // Normaliser les positions
            foreach (var ville in _positions.Keys.ToList())
            {
                var point = _positions[ville];
                float normalizedX = (point.X - minX) * scale + _width * 0.05f; // Ajouter une marge
                float normalizedY = (point.Y - minY) * scale + _height * 0.05f; // Ajouter une marge
                _positions[ville] = new SKPoint(normalizedX, normalizedY);
            }
        }
        private void AjouterEspacementMinimum(float espacement)
        {
            bool positionsAjustees;

            do
            {
                positionsAjustees = false;

                foreach (var ville1 in _positions.Keys.ToList())
                {
                    foreach (var ville2 in _positions.Keys.ToList())
                    {
                        if (ville1 == ville2) continue;

                        var point1 = _positions[ville1];
                        var point2 = _positions[ville2];

                        float distance = (float)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
                        if (distance < espacement)
                        {
                            // Ajuster les positions pour augmenter l'espacement
                            float deltaX = (point2.X - point1.X) / distance * (espacement - distance);
                            float deltaY = (point2.Y - point1.Y) / distance * (espacement - distance);

                            _positions[ville2] = new SKPoint(point2.X + deltaX, point2.Y + deltaY);
                            positionsAjustees = true;
                        }
                    }
                }
            } while (positionsAjustees);
        }
    }
}
