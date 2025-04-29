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
            Random rand = new Random();
            foreach (var ville in villes)
            {
                _positions[ville] = new SKPoint(rand.Next(_width), rand.Next(_height));
            }
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
    }
}
