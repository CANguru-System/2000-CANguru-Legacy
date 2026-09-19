using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfSegmenter.Core
{
    public static class DxfConverter
    {
        public static TrackModel Load(string filename)
        {
            var file = DxfFile.Load(filename);
            var model = new TrackModel();

            var polys = new List<DxfLwPolyline>();

            // 1. Entities nach Layer gruppieren
            var groups = file.Entities.GroupBy(e => e.Layer);

            foreach (var g in groups)
            {
                string layer = g.Key;

                // 2. Trassenlayer → Segmentrekonstruktion
                if (IstTrassenLayer(layer))
                {
                    var trassenPolys = RekonstruiereKetten(g);
                    polys.AddRange(trassenPolys);
                    continue;
                }

                // 3. Normale LWPOLYLINE übernehmen
                foreach (var lw in g.OfType<DxfLwPolyline>())
                    polys.Add(lw);

                // 4. Klassische POLYLINE → LWPOLYLINE konvertieren
                foreach (var pl in g.OfType<DxfPolyline>())
                {
                    var verts = pl.Vertices
                        .Select(v => new DxfLwPolylineVertex
                        {
                            X = v.Location.X,
                            Y = v.Location.Y
                        })
                        .ToList();

                    polys.Add(new DxfLwPolyline(verts));
                }

                // Lines/Arcs auf Nicht‑Trassenlayer ignorieren
            }

            model.Polylines = polys;

            // Tracks + Trassen erzeugen
            DxfLoader.ErzeugeTracks(model);
            DxfLoader.ErzeugeTrassen(model);

            return model;
        }

        // ---------------------------------------------------------
        // Layer-Erkennung
        // ---------------------------------------------------------
        private static bool IstTrassenLayer(string layer)
        {
            return layer == "10" || layer == "12";
        }

        // ---------------------------------------------------------
        // Segment-Ketten rekonstruieren (Lines + Arcs)
        // ---------------------------------------------------------
        private static List<DxfLwPolyline> RekonstruiereKetten(IEnumerable<DxfEntity> entities, double tolerance = 0.5)
        {
            var segmente = new List<(PointF A, PointF B)>();

            foreach (var e in entities)
            {
                if (e is DxfLine ln)
                {
                    segmente.Add((
                        new PointF((float)ln.P1.X, (float)ln.P1.Y),
                        new PointF((float)ln.P2.X, (float)ln.P2.Y)
                    ));
                }
                else if (e is DxfArc arc)
                {
                    float cx = (float)arc.Center.X;
                    float cy = (float)arc.Center.Y;
                    float r = (float)arc.Radius;

                    double startDeg, endDeg;

                    var pStartAngle = arc.GetType().GetProperty("StartAngle");
                    var pEndAngle = arc.GetType().GetProperty("EndAngle");

                    if (pStartAngle != null && pEndAngle != null)
                    {
                        startDeg = (double)pStartAngle.GetValue(arc);
                        endDeg = (double)pEndAngle.GetValue(arc);
                    }
                    else
                    {
                        var pStartDeg = arc.GetType().GetProperty("StartAngleDegrees");
                        var pEndDeg = arc.GetType().GetProperty("EndAngleDegrees");

                        if (pStartDeg != null && pEndDeg != null)
                        {
                            startDeg = (double)pStartDeg.GetValue(arc);
                            endDeg = (double)pEndDeg.GetValue(arc);
                        }
                        else
                        {
                            var pStartParam = arc.GetType().GetProperty("StartParameter");
                            var pEndParam = arc.GetType().GetProperty("EndParameter");

                            startDeg = (double)pStartParam.GetValue(arc) * 180.0 / Math.PI;
                            endDeg = (double)pEndParam.GetValue(arc) * 180.0 / Math.PI;
                        }
                    }

                    float startRad = (float)(startDeg * Math.PI / 180.0);
                    float endRad = (float)(endDeg * Math.PI / 180.0);

                    var sp = new PointF(
                        cx + r * (float)Math.Cos(startRad),
                        cy + r * (float)Math.Sin(startRad)
                    );

                    var ep = new PointF(
                        cx + r * (float)Math.Cos(endRad),
                        cy + r * (float)Math.Sin(endRad)
                    );

                    segmente.Add((sp, ep));
                }
            }

            var lookup = new Dictionary<PointF, List<PointF>>(new PointFTolerantComparer(tolerance));

            foreach (var s in segmente)
            {
                if (!lookup.ContainsKey(s.A))
                    lookup[s.A] = new List<PointF>();

                lookup[s.A].Add(s.B);
            }

            var polylines = new List<DxfLwPolyline>();
            var visited = new HashSet<PointF>(new PointFTolerantComparer(tolerance));

            foreach (var s in segmente)
            {
                if (visited.Contains(s.A))
                    continue;

                var chain = new List<PointF>();
                var current = s.A;

                chain.Add(current);
                visited.Add(current);

                while (lookup.TryGetValue(current, out var nextList))
                {
                    var next = nextList.FirstOrDefault(p => !visited.Contains(p));
                    if (next == default)
                        break;

                    chain.Add(next);
                    visited.Add(next);
                    current = next;
                }

                if (chain.Count > 1)
                {
                    var verts = chain.Select(p => new DxfLwPolylineVertex
                    {
                        X = p.X,
                        Y = p.Y
                    }).ToList();

                    polylines.Add(new DxfLwPolyline(verts));
                }
            }

            return polylines;
        }

    }
}
