using System;
using System.Collections.Generic;
using System.Diagnostics;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfSegmenter.Core
{
    public static class DxfLoader
    {
        // ---------------------------------------------------------
        // Länge einer LWPOLYLINE berechnen
        // ---------------------------------------------------------
        private static double PolylineLength(DxfLwPolyline poly)
        {
            double len = 0.0;

            for (int i = 1; i < poly.Vertices.Count; i++)
            {
                double dx = poly.Vertices[i].X - poly.Vertices[i - 1].X;
                double dy = poly.Vertices[i].Y - poly.Vertices[i - 1].Y;
                len += Math.Sqrt(dx * dx + dy * dy);
            }

            return len;
        }

        // ---------------------------------------------------------
        // Tracks erzeugen (zwei längste Polylinien)
        // ---------------------------------------------------------
        public static void ErzeugeTracks(TrackModel model)
        {
            model.Tracks.Clear();

            if (model.Polylines.Count < 2)
                return;

            // Sortieren nach Länge (Trassenlinien sind am längsten)
            var sorted = model.Polylines
                .OrderByDescending(p => PolylineLength(p))
                .ToList();

            var leftOuter = sorted[0];
            var rightOuter = sorted[1];

            model.Tracks.Add(new List<DxfLwPolyline> { leftOuter });
            model.Tracks.Add(new List<DxfLwPolyline> { rightOuter });

            Debug.WriteLine($"Trassenlinien erkannt: {leftOuter.Vertices.Count} / {rightOuter.Vertices.Count}");
        }

        // ---------------------------------------------------------
        // Trassen erzeugen (Left/Right)
        // ---------------------------------------------------------
        public static void ErzeugeTrassen(TrackModel model)
        {
            model.Trassen.Clear();

            if (model.Tracks.Count < 2)
                return;

            model.Trassen.Add(new Trasse
            {
                Left = model.Tracks[0][0],
                Right = model.Tracks[1][0]
            });

            Debug.WriteLine("Trasse erzeugt.");
        }
    }
}
