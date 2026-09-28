using System;
using System.Collections.Generic;
using System.Drawing;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfSegmenter.Core
{
/*    public static class SegmentGenerator
    {
        // ---------------------------------------------------------
        // Rohsegment: ein Abschnitt zwischen zwei Punkten
        // ---------------------------------------------------------
        public class SegmentRaw
        {
            public int Index { get; set; }
            public PointF LeftStart { get; set; }
            public PointF LeftEnd { get; set; }
            public PointF RightStart { get; set; }
            public PointF RightEnd { get; set; }
        }

        // ---------------------------------------------------------
        // Schrittweiten-Segmentierung entlang der Trasse
        // ---------------------------------------------------------
        public static List<SegmentRaw> SchrittweitenSegmentierung(
            DxfLwPolyline left,
            DxfLwPolyline right,
            double maxSegmentLength)
        {
            var segs = new List<SegmentRaw>();

            // Beide Polylinien müssen gleiche Anzahl an Punkten haben
            int count = Math.Min(left.Vertices.Count, right.Vertices.Count);

            if (count < 2)
                return segs;

            double accumulated = 0.0;
            int segIndex = 0;

            PointF lastLeft = new PointF((float)left.Vertices[0].X, (float)left.Vertices[0].Y);
            PointF lastRight = new PointF((float)right.Vertices[0].X, (float)right.Vertices[0].Y);

            for (int i = 1; i < count; i++)
            {
                var curLeft = new PointF((float)left.Vertices[i].X, (float)left.Vertices[i].Y);
                var curRight = new PointF((float)right.Vertices[i].X, (float)right.Vertices[i].Y);

                double dx = curLeft.X - lastLeft.X;
                double dy = curLeft.Y - lastLeft.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                accumulated += dist;

                if (accumulated >= maxSegmentLength)
                {
                    segs.Add(new SegmentRaw
                    {
                        Index = segIndex++,
                        LeftStart = lastLeft,
                        LeftEnd = curLeft,
                        RightStart = lastRight,
                        RightEnd = curRight
                    });

                    accumulated = 0.0;
                }

                lastLeft = curLeft;
                lastRight = curRight;
            }

            return segs;
        }

        // ---------------------------------------------------------
        // DXF-Viereck aus SegmentRaw erzeugen
        // ---------------------------------------------------------
        public static DxfLwPolyline SegmentViereckErzeugen(SegmentRaw seg)
        {
            var verts = new List<DxfLwPolylineVertex>
            {
                new DxfLwPolylineVertex(seg.LeftStart.X,  seg.LeftStart.Y),
                new DxfLwPolylineVertex(seg.LeftEnd.X,    seg.LeftEnd.Y),
                new DxfLwPolylineVertex(seg.RightEnd.X,   seg.RightEnd.Y),
                new DxfLwPolylineVertex(seg.RightStart.X, seg.RightStart.Y)
            };

            var poly = new DxfLwPolyline(verts)
            {
                IsClosed = true
            };

            return poly;
        }
    }*/
}
