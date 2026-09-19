using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using static DxfSegmenter.Core.TileManager;

namespace DxfSegmenter.Core
{
    public class SegmentRaw
    {
        public List<(double X, double Y)> LeftPoints { get; set; }
        public List<(double X, double Y)> RightPoints { get; set; }

        public SegmentRaw()
        {
            LeftPoints = new List<(double X, double Y)>();
            RightPoints = new List<(double X, double Y)>();
        }
        public RectangleF HandleBounds { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public RectangleF MoveHandleBounds { get; set; }
        public RectangleF RotateHandleBounds { get; set; }

        public float RotationAngle { get; set; }   // in Grad

        // Garantiert innen liegender Punkt für Segmentnummern
        public (double X, double Y) CenterPoint
        {
            get
            {
                int mid = LeftPoints.Count / 2;
                var pL = LeftPoints[mid];
                var pR = RightPoints[mid];

                return ((pL.X + pR.X) / 2.0, (pL.Y + pR.Y) / 2.0);
            }
        }
    }

    public static class SegmentGenerator
    {
        private static double DistPointToSegment(
    (double X, double Y) p,
    (double X, double Y) a,
    (double X, double Y) b)
        {
            double vx = b.X - a.X;
            double vy = b.Y - a.Y;

            double wx = p.X - a.X;
            double wy = p.Y - a.Y;

            double c1 = vx * wx + vy * wy;
            if (c1 <= 0)
                return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));

            double c2 = vx * vx + vy * vy;
            if (c2 <= c1)
                return Math.Sqrt((p.X - b.X) * (p.X - b.X) + (p.Y - b.Y) * (p.Y - b.Y));

            double t = c1 / c2;
            double projX = a.X + t * vx;
            double projY = a.Y + t * vy;

            double dx = p.X - projX;
            double dy = p.Y - projY;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double DistToPolyline((double X, double Y) p, DxfLwPolyline poly)
        {
            double best = double.MaxValue;

            for (int i = 0; i < poly.Vertices.Count - 1; i++)
            {
                var a = poly.Vertices[i];
                var b = poly.Vertices[i + 1];

                double d = DistPointToSegment(p, (a.X, a.Y), (b.X, b.Y));
                if (d < best) best = d;
            }

            return best;
        }

        private static (double nx, double ny) WähleNormale(
    (double X, double Y) p,
    (double X, double Y) tnorm,
    DxfLwPolyline right)
        {
            // zwei mögliche Normalen
            var n1 = (-tnorm.Y, tnorm.X);
            var n2 = (tnorm.Y, -tnorm.X);

            // Testpunkte 1 mm entlang der Normalen
            var test1 = (p.X + n1.Item1 * 1.0, p.Y + n1.Item2 * 1.0);
            var test2 = (p.X + n2.Item1 * 1.0, p.Y + n2.Item2 * 1.0);

            // Distanz zur rechten Linie
            double d1 = DistToPolyline(test1, right);
            double d2 = DistToPolyline(test2, right);

            // die Normale, die näher an der rechten Linie liegt, ist die richtige
            return d1 < d2 ? (n1.Item1, n1.Item2) : (n2.Item1, n2.Item2);
        }
        public static (double X, double Y) BerechneNormalenschnittpunktAtPoint(
    DxfLwPolyline left,
    DxfLwPolyline right,
    (double X, double Y) p,
    int segmentIndex)
        {
            // Segment für Tangente
            var v0 = left.Vertices[segmentIndex];
            var v1 = left.Vertices[segmentIndex + 1];

            // Tangentenvektor
            double tx = v1.X - v0.X;
            double ty = v1.Y - v0.Y;
            double tlen = Math.Sqrt(tx * tx + ty * ty);

            double tnx = tx / tlen;
            double tny = ty / tlen;

            // Richtige Normale wählen
            var normale = WähleNormale(p, (tnx, tny), right);

            double nx = normale.nx;
            double ny = normale.ny;

            // Gerade G(t) = p + t*N

            // Schnitt mit rechter Polyline
            for (int j = 0; j < right.Vertices.Count - 1; j++)
            {
                var r0 = right.Vertices[j];
                var r1 = right.Vertices[j + 1];

                double sx = r1.X - r0.X;
                double sy = r1.Y - r0.Y;

                double rx = r0.X - p.X;
                double ry = r0.Y - p.Y;

                double det = nx * (-sy) - ny * (-sx);
                if (Math.Abs(det) < 1e-9)
                    continue;

                double t = (rx * (-sy) - ry * (-sx)) / det;
                double u = (nx * ry - ny * rx) / det;

                if (t > 0 && u >= 0 && u <= 1)
                {
                    double ix = p.X + t * nx;
                    double iy = p.Y + t * ny;
                    return (ix, iy);
                }
            }

            return p;
        }

        public static (double X, double Y) BerechneNormalenschnittpunkt(
            DxfLwPolyline left,
            DxfLwPolyline right,
            int index)
        {
            // Punkt P(0,i)
            var p0 = left.Vertices[index];
            var p1 = left.Vertices[index + 1];

            // Tangentenvektor
            double tx = p1.X - p0.X;
            double ty = p1.Y - p0.Y;
            double tlen = Math.Sqrt(tx * tx + ty * ty);

            double tnx = tx / tlen;
            double tny = ty / tlen;

            // Zwei mögliche Normalen
            var n1 = (-tny, tnx);
            var n2 = (tny, -tnx);

            // Richtige Normale wählen
            var normale = WähleNormale((p0.X, p0.Y), (tnx, tny), right);

            double nx = normale.nx;
            double ny = normale.ny;

            // Gerade G(t) = P0 + t*N

            // Schnitt mit rechter Polyline
            for (int j = 0; j < right.Vertices.Count - 1; j++)
            {
                var r0 = right.Vertices[j];
                var r1 = right.Vertices[j + 1];

                double sx = r1.X - r0.X;
                double sy = r1.Y - r0.Y;

                // Lösen:
                // P0 + t*N = R0 + u*S
                // => t*N - u*S = R0 - P0

                double rx = r0.X - p0.X;
                double ry = r0.Y - p0.Y;

                double det = nx * (-sy) - ny * (-sx);
                if (Math.Abs(det) < 1e-9)
                    continue; // Parallel

                double t = (rx * (-sy) - ry * (-sx)) / det;
                double u = (nx * ry - ny * rx) / det;

                if (t > 0 && u >= 0 && u <= 1)
                {
                    // Schnittpunkt
                    double ix = p0.X + t * nx;
                    double iy = p0.Y + t * ny;
                    return (ix, iy);
                }
            }

            // Falls kein Schnittpunkt gefunden wird (sollte nicht passieren)
            return (p0.X, p0.Y);
        }

        public static List<SegmentRaw> SchrittweitenSegmentierung(
            DxfLwPolyline left,
            DxfLwPolyline right,
            double maxSegmentLength,
            double stepSize = 1.0)
        {
            var result = new List<SegmentRaw>();
            var current = new SegmentRaw();

            int index = 0;
            double walked = 0.0;

            // Startpunkt
            var p0 = left.Vertices[0];
            var p1 = BerechneNormalenschnittpunktAtPoint(left, right, (p0.X, p0.Y), 0);

            current.LeftPoints.Add((p0.X, p0.Y));
            current.RightPoints.Add(p1);

            while (index < left.Vertices.Count - 1)
            {
                var v0 = left.Vertices[index];
                var v1 = left.Vertices[index + 1];

                double dx = v1.X - v0.X;
                double dy = v1.Y - v0.Y;
                double segLen = Math.Sqrt(dx * dx + dy * dy);

                // Solange wir innerhalb dieses Segments noch Schritte machen können
                while (walked + segLen >= stepSize)
                {
                    double rest = stepSize - walked;
                    double t = rest / segLen;

                    double x = v0.X + dx * t;
                    double y = v0.Y + dy * t;

                    var pLeft = (x, y);
                    var pRight = BerechneNormalenschnittpunktAtPoint(left, right, pLeft, index);

                    current.LeftPoints.Add(pLeft);
                    current.RightPoints.Add(pRight);

                    walked = 0.0; // Schritt verbraucht

                    // Segmentgrenze prüfen
                    var startLeft = current.LeftPoints[0];
                    var startRight = current.RightPoints[0];

                    double diag1 = Dist(startLeft, pRight);
                    double diag2 = Dist(startRight, pLeft);
                    double maxDiag = Math.Max(diag1, diag2);

                    if (maxDiag > maxSegmentLength)
                    {
                        result.Add(current);

                        current = new SegmentRaw();
                        current.LeftPoints.Add(pLeft);
                        current.RightPoints.Add(pRight);
                    }

                    // Wir bleiben im gleichen Segment, aber segLen schrumpft
                    segLen -= rest;

                    // Neuer Startpunkt innerhalb des Segments
                    v0 = new DxfLwPolylineVertex { X = x, Y = y };
                    dx = v1.X - v0.X;
                    dy = v1.Y - v0.Y;
                }

                // Wenn wir hier sind, reicht das Segment nicht mehr für einen Schritt
                walked += segLen;
                index++;
            }

            if (current.LeftPoints.Count > 1)
                result.Add(current);

            return result;
        }

        private static double Dist((double X, double Y) a, (double X, double Y) b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static DxfLwPolyline SegmentViereckErzeugen(SegmentRaw raw)
        {
            int last = raw.LeftPoints.Count - 1;

            var vertices = new List<DxfLwPolylineVertex>();

            // 1. Linke Außenlinie: alle Punkte P(0,i) in Reihenfolge
            for (int i = 0; i <= last; i++)
            {
                var pL = raw.LeftPoints[i];
                vertices.Add(new DxfLwPolylineVertex { X = pL.X, Y = pL.Y });
            }

            // 2. Querlinie: von letztem linken Punkt zu letztem rechten Punkt
            var pLastRight = raw.RightPoints[last];
            vertices.Add(new DxfLwPolylineVertex { X = pLastRight.X, Y = pLastRight.Y });

            // 3. Rechte Außenlinie: alle Punkte P(1,i) rückwärts
            for (int i = last - 1; i >= 0; i--)
            {
                var pR = raw.RightPoints[i];
                vertices.Add(new DxfLwPolylineVertex { X = pR.X, Y = pR.Y });
            }

            // 4. Querlinie zurück zum ersten linken Punkt
            var pFirstLeft = raw.LeftPoints[0];
            vertices.Add(new DxfLwPolylineVertex { X = pFirstLeft.X, Y = pFirstLeft.Y });

            return new DxfLwPolyline(vertices)
            {
                IsClosed = true
            };
        }

        private static double BerechnePolylineLaenge(DxfLwPolyline poly)
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
    }
}
