using System;
using System.Collections.Generic;
using System.Text;

namespace DxfSegmenter.Core
{
    // ---------------------------------------------------------
    // Punktvergleich mit Toleranz
    // ---------------------------------------------------------
    public class PointFTolerantComparer : IEqualityComparer<PointF>
    {
        private readonly double tol;

        public PointFTolerantComparer(double tolerance)
        {
            tol = tolerance;
        }

        public bool Equals(PointF a, PointF b)
        {
            return Math.Abs(a.X - b.X) < tol && Math.Abs(a.Y - b.Y) < tol;
        }

        public int GetHashCode(PointF p)
        {
            return 0;
        }
    }
}
