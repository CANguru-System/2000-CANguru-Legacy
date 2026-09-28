using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using DxfSegmenter.Core;
using System.Diagnostics;

namespace DxfSegmenter
{
    public class TrackView : Control
    {
        // mehrere Linien statt einer einzigen Polyline
        private List<List<PointF>> _lines = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TileManager TileManager { get; set; }

        public TrackModel Model { get; private set; }

        public TrackView()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        public void SetModel(TrackModel model)
        {
            Model = model;
            Invalidate();
        }

        public void FitToWindow()
        {
            Invalidate();
        }

        // neue Methode: mehrere Linien anzeigen
        public void ShowLines(List<List<PointF>> lines)
        {
            _lines = lines;
            Debug.WriteLine($"Anzahl Linien: {_lines.Count}");
            Invalidate();
        }

        // alte API beibehalten (für MainForm)
        public void ShowSegments<TSegment, TPolygon>(
            IEnumerable<TSegment> rawSegments,
            IEnumerable<TPolygon> dxfPolygons)
        {
            // später implementierbar
            Invalidate();
        }

        public void ExportSegmentsToDxf(string fileName)
        {
            // später implementierbar
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_lines == null || _lines.Count == 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (var line in _lines)
            {
                if (line.Count < 2)
                    continue;

                using var path = new GraphicsPath();

                for (int i = 0; i < line.Count - 1; i++)
                    path.AddLine(line[i], line[i + 1]);

                e.Graphics.DrawPath(Pens.Red, path);
            }
        }
    }
}
