using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace DxfSegmenter.Core
{
    public class Trasse
    {
        public DxfLwPolyline Left { get; set; }
        public DxfLwPolyline Right { get; set; }
    }

    public class TrackModel
    {
        public List<DxfLwPolyline> Polylines { get; set; } = new();
        public List<List<DxfLwPolyline>> Tracks { get; set; } = new();
        public List<Trasse> Trassen { get; set; } = new();

        public TileManager TileManager { get; set; }
        public int SegmentIndex { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public RectangleF HandleBounds { get; set; }

        // ---------------------------------------------------------
        // TRASSEN-LINIEN ALS LISTEN VON POINTF
        // ---------------------------------------------------------
        public (List<PointF> left, List<PointF> right) GetTrassenLines()
        {
            var left = new List<PointF>();
            var right = new List<PointF>();

            if (Trassen.Count == 0)
                return (left, right);

            var trasse = Trassen[0];

            foreach (var v in trasse.Left.Vertices)
                left.Add(new PointF((float)v.X, (float)v.Y));

            foreach (var v in trasse.Right.Vertices)
                right.Add(new PointF((float)v.X, (float)v.Y));

            return (left, right);
        }

        // ---------------------------------------------------------
        // GESAMT-BOUNDING-BOX
        // ---------------------------------------------------------
        public (double minX, double minY, double maxX, double maxY) GetBounds()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var poly in Polylines)
            {
                foreach (var v in poly.Vertices)
                {
                    if (v.X < minX) minX = v.X;
                    if (v.Y < minY) minY = v.Y;
                    if (v.X > maxX) maxX = v.X;
                    if (v.Y > maxY) maxY = v.Y;
                }
            }

            if (TileManager != null)
            {
                foreach (var tile in TileManager.Tiles)
                {
                    if (tile.X < minX) minX = tile.X;
                    if (tile.Y < minY) minY = tile.Y;
                    if (tile.X + tile.Width > maxX) maxX = tile.X + tile.Width;
                    if (tile.Y + tile.Height > maxY) maxY = tile.Y + tile.Height;
                }
            }

            return (minX, minY, maxX, maxY);
        }
    }
}
