using DxfSegmenter.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DxfSegmenter.Rendering
{
    public class SegmentDragController
    {
        private readonly TrackView _view;
        private bool _dragging;
        private Point _lastMouse;
        private readonly TrackModel _model;

        public SegmentDragController(TrackView view, TrackModel model)
        {
            _view = view;
            _model = model;
        }
        public void OnMouseDown(MouseEventArgs e)
        {
            _dragging = true;
            _lastMouse = e.Location;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (!_dragging)
                return;

            int dx = e.X - _lastMouse.X;
            int dy = e.Y - _lastMouse.Y;

            // einfache Verschiebung der Polyline
            ShiftPolyline(dx, dy);

            _lastMouse = e.Location;
            _view.Invalidate();
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
        }

        private void ShiftPolyline(int dx, int dy)
        {
            // Trassenlinien aus dem Modell holen
            (List<PointF> left, List<PointF> right) = _model.GetTrassenLines();

            // beide Linien verschieben
            for (int i = 0; i < left.Count; i++)
                left[i] = new PointF(left[i].X + dx, left[i].Y + dy);

            for (int i = 0; i < right.Count; i++)
                right[i] = new PointF(right[i].X + dx, right[i].Y + dy);

            // beide Linien anzeigen
            _view.ShowLines(new List<List<PointF>> { left, right });
        }
        private List<PointF> GetPoints()
        {
            var field = typeof(TrackView).GetField("_polyPoints",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            return field?.GetValue(_view) as List<PointF>;
        }
    }
}
