using System.Drawing;

namespace DxfSegmenter.Utils
{
    public static class SegmentColors
    {
        private static readonly byte[] aciPalette =
        {
            1,  // Rot
            3,  // Grün
            5,  // Blau
            2,  // Gelb
            6,  // Magenta
            4,  // Cyan
            7,  // Weiß
            8,  // Grau
            9,  // Hellblau
            10, // Hellgrün
            11, // Hellrot
            12  // Orange
        };

        public static byte Get(int index)
        {
            return aciPalette[index % aciPalette.Length];
        }
    }
}
