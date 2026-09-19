using System.Collections.Generic;

namespace DxfSegmenter.Core
{

    // ---------------------------------------------------------
    // Tile: eine einzelne Kachel
    // ---------------------------------------------------------
    public class Tile
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Tile(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
