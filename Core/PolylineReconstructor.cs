using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DxfSegmenter.Core
{
    public class LineSegment
    {
        public PointF Start { get; }
        public PointF End { get; }

        public LineSegment(PointF start, PointF end)
        {
            Start = start;
            End = end;
        }
    }

    public static class PolylineReconstructor
    {
        private static PointF Normalize(PointF p)
        {
            return new PointF(
                (float)Math.Round(p.X, 2),
                (float)Math.Round(p.Y, 2)
            );
        }

        public static List<PointF> Reconstruct(List<LineSegment> segments)
        {
            if (segments == null || segments.Count == 0)
                return new List<PointF>();

            // Normalisieren
            var normalized = segments
                .Select(s => new LineSegment(Normalize(s.Start), Normalize(s.End)))
                .ToList();

            // Startsegment wählen (beliebig)
            var current = normalized[0];
            normalized.RemoveAt(0);

            var result = new List<PointF>();
            result.Add(current.Start);
            result.Add(current.End);

            PointF currentEnd = current.End;

            // Solange Segmente übrig sind
            while (normalized.Count > 0)
            {
                // Nächstes Segment suchen
                LineSegment next = FindNextSegment(normalized, currentEnd);

                if (next == null)
                    break; // keine Verbindung mehr

                // Segment entfernen
                normalized.Remove(next);

                // Reihenfolge prüfen
                if (Distance(next.Start, currentEnd) < 0.5f)
                {
                    result.Add(next.End);
                    currentEnd = next.End;
                }
                else
                {
                    result.Add(next.Start);
                    currentEnd = next.Start;
                }
            }

            return result;
        }

        private static LineSegment FindNextSegment(List<LineSegment> segments, PointF currentEnd)
        {
            foreach (var s in segments)
            {
                if (Distance(s.Start, currentEnd) < 0.5f) return s;
                if (Distance(s.End, currentEnd) < 0.5f) return s;
            }
            return null;
        }

        private static float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
