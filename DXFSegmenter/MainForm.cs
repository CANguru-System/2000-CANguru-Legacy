using DxfSegmenter.Core;
using System;
using System.Windows.Forms;
using static DxfSegmenter.Core.TileManager;
using static DxfSegmenter.Core.TrackModel;
using System.Diagnostics;

namespace DxfSegmenter
{
    public partial class MainForm : Form
    {
        private TileManager _tileManager = new TileManager();
        private TrackView _trackView;

        public MainForm()
        {
            InitializeComponent();

            // TrackView aus Designer übernehmen
            _trackView = trackView1;

            // TileManager an TrackView übergeben
            _trackView.TileManager = _tileManager;
        }

        private List<PointF> ExtractAllPolylinePoints(TrackModel model)
        {
            var pts = new List<PointF>();

            foreach (var poly in model.Polylines)
            {
                foreach (var v in poly.Vertices)
                {
                    pts.Add(new PointF((float)v.X, (float)v.Y));
                }
            }

            return pts;
        }

        private List<PointF> ExtractPolylinePoints(TrackModel model)
        {
            // einfache Variante: nimm die linke Trasse
            var trasse = model.Trassen.FirstOrDefault();
            if (trasse == null)
                return new List<PointF>();

            var poly = trasse.Left; // DxfLwPolyline

            var pts = new List<PointF>();
            foreach (var v in poly.Vertices)
                pts.Add(new PointF((float)v.X, (float)v.Y));

            return pts;
        }

        private List<PointF> ExtractTrassenPoints(TrackModel model)
        {
            var pts = new List<PointF>();

            if (model.Trassen.Count == 0)
                return pts;

            var trasse = model.Trassen[0];

            foreach (var v in trasse.Left.Vertices)
                pts.Add(new PointF((float)v.X, (float)v.Y));

            foreach (var v in trasse.Right.Vertices)
                pts.Add(new PointF((float)v.X, (float)v.Y));

            return pts;
        }

        private List<PointF> NormalizeToView(List<PointF> pts, Control view)
        {
            if (pts.Count == 0)
                return pts;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var p in pts)
            {
                if (p.X < minX) minX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.X > maxX) maxX = p.X;
                if (p.Y > maxY) maxY = p.Y;
            }

            float dx = maxX - minX;
            float dy = maxY - minY;

            // Skalierung berechnen
            float scaleX = (view.Width - 40) / dx;
            float scaleY = (view.Height - 40) / dy;
            float scale = Math.Min(scaleX, scaleY);

            // Mittelpunkt des DXF
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;

            // Mittelpunkt des Fensters
            float viewCenterX = view.Width / 2f;
            float viewCenterY = view.Height / 2f;

            var normalized = new List<PointF>();

            foreach (var p in pts)
            {
                float nx = (p.X - centerX) * scale + viewCenterX;
                float ny = (p.Y - centerY) * scale + viewCenterY;
                normalized.Add(new PointF(nx, ny));
            }

            return normalized;
        }

        private List<PointF> Normalize(List<PointF> pts, Control view)
        {
            float minX = pts.Min(p => p.X);
            float maxX = pts.Max(p => p.X);
            float minY = pts.Min(p => p.Y);
            float maxY = pts.Max(p => p.Y);

            float dx = maxX - minX;
            float dy = maxY - minY;

            float scaleX = (view.Width - 40) / dx;
            float scaleY = (view.Height - 40) / dy;
            float scale = Math.Min(scaleX, scaleY);

            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;

            float viewCenterX = view.Width / 2f;
            float viewCenterY = view.Height / 2f;

            return pts
                .Select(p => new PointF(
                    (p.X - centerX) * scale + viewCenterX,
                    (p.Y - centerY) * scale + viewCenterY))
                .ToList();
        }

        // ---------------------------------------------------------
        // DXF LADEN
        // ---------------------------------------------------------
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "DXF Dateien (*.dxf)|*.dxf";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var model = DxfConverter.Load(dlg.FileName);
                Debug.WriteLine($"Polylines: {model.Polylines.Count}");
                Debug.WriteLine($"Trassen: {model.Trassen.Count}");
                Debug.WriteLine($"Left vertices: {model.Trassen[0].Left.Vertices.Count}");
                Debug.WriteLine($"Right vertices: {model.Trassen[0].Right.Vertices.Count}");

                _trackView.SetModel(model);
                _trackView.FitToWindow();

                (List<PointF> left, List<PointF> right) = model.GetTrassenLines();
                var leftNorm = Normalize(left, _trackView);
                var rightNorm = Normalize(right, _trackView);

                _trackView.ShowLines(new List<List<PointF>> { leftNorm, rightNorm });

                _trackView.Invalidate();
            }

        }

        // ---------------------------------------------------------
        // KACHEL ERZEUGEN (nach DXF-Laden)
        // ---------------------------------------------------------
        private void CreateTileInsideModel(TrackModel model)
        {
            (double minX, double minY, double maxX, double maxY) = model.GetBounds();
            double centerX = (minX + maxX) / 2.0;
            double centerY = (minY + maxY) / 2.0;

            var tile = new Tile(centerX - 50, centerY - 50, 410, 400);

            _tileManager.Tiles.Clear();
            _tileManager.Tiles.Add(tile);
        }

        private void segmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. Dialog öffnen
            var dlg = new TileSettingsDialog(_tileManager.Settings);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            // 2. Einstellungen übernehmen
            _tileManager.Settings.MaxSegmentLength = dlg.MaxSegmentLength;

            // 3. Segmentierung starten
            var rawSegments = SegmentGenerator.SchrittweitenSegmentierung(
                _trackView.Model.Trassen[0].Left,
                _trackView.Model.Trassen[0].Right,
                _tileManager.Settings.MaxSegmentLength
            );

            // 4. DXF‑Polygone erzeugen
            var dxfSegments = rawSegments
                .Select(seg => SegmentGenerator.SegmentViereckErzeugen(seg))
                .ToList();

            // 5. Anzeigen
            _trackView.ShowSegments(rawSegments, dxfSegments);
        }

        private void exportSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "DXF-Datei (*.dxf)|*.dxf",
                FileName = "Segmente.dxf"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _trackView.ExportSegmentsToDxf(dlg.FileName);
            }
        }
    }
}
