using System.Runtime.Intrinsics.X86;

namespace TemeGC
{
    internal static class S4
    {
        private static int _n;
        private static List<Point> _points = null!;
        private static Random _random = new Random();
        private static int _lowerHull;
        private static List<List<Point>> _myHull = new List<List<Point>>();
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            _n = 40;
            _points = new List<Point>();
            for (int i = 0; i < _n; i++)
            {
                int x = _random.Next(40, Form1.width - 40);
                int y = _random.Next(40, Form1.height - 40);
                _points.Add(new Point(x, y));
            }

            ConvexHullClass.ConvexHull(_points);



            for (int i = 0; i < _points.Count; i++)
                g.FillEllipse(new SolidBrush(Color.Yellow), _points[i].X - 6, _points[i].Y - 6, 12, 12);


            SolidBrush solidBrush = new SolidBrush(Color.Red);
            List<Point> awt = _myHull[_myHull.Count - 1];
            for (int i = 0; i < awt.Count - 1; i++)
            {
                Console.WriteLine(_lowerHull);
                if (i >= _lowerHull - 1)
                    solidBrush = new SolidBrush(Color.Blue);
                Point p1 = awt[i];
                Point p2 = awt[i + 1];
                g.DrawLine(new Pen(solidBrush, 2), p1, p2);
            }



            return pb;
        }

        private class ConvexHullClass
        {
            private static long CrossProduct(Point o, Point a, Point b)
            {
                return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
            }


            public static List<Point> ConvexHull(List<Point> points)
            {
                int n = points.Count(), k = 0;

                if (n <= 3) return points;

                List<Point> hullPoints = new List<Point>(2 * n);

                points.Sort((p1, p2) => {
                    if (p1.X == p2.X) return p1.Y - p2.Y;
                    return p1.X - p2.X;
                });

                for (int i = 0; i < n; ++i)
                {

                    while (k >= 2 && CrossProduct(hullPoints[k - 2], hullPoints[k - 1], points[i]) <= 0)
                    {
                        hullPoints.RemoveAt(--k);
                    }
                    hullPoints.Add(points[i]);
                    _myHull.Add(new List<Point>(hullPoints));
                    k++;
                }

                _lowerHull = hullPoints.Count;

                for (int i = n - 2, t = k; i >= 0; --i)
                {

                    while (k > t && CrossProduct(hullPoints[k - 2], hullPoints[k - 1], points[i]) <= 0)
                    {
                        hullPoints.RemoveAt(--k);
                    }
                    hullPoints.Add(points[i]);
                    _myHull.Add(new List<Point>(hullPoints));
                    k++;
                }
                return hullPoints;
            }
        }
    }
}