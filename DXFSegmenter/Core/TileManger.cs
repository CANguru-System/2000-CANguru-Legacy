using System;
using System.Collections.Generic;
using System.Text;

namespace DxfSegmenter.Core
{
    public class TileManager
    {
        // Liste aller erzeugten Kacheln
        public List<Tile> Tiles { get; set; } = new();

        // Einstellungen für Segmentierung / Kachelgröße
        public TileSettings Settings { get; set; } = new TileSettings();
    }
}
