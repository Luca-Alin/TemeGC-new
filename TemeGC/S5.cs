namespace TemeGC
{
    internal class S5
    {
        private static int _n;
        private static Point[] _points;
        private static Random _random = new Random();
        private static List<List<Point>> _convexHullList = new List<List<Point>>();

        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            _n = 40;
            _points = new Point[_n];
            for (int i = 0; i < _points.Length; i++)
            {
                int x = _random.Next(40, Form1.width - 40);
                int y = _random.Next(40, Form1.height - 40);
                _points[i] = new Point(x, y);
            }

            for (int i = 0; i < _points.Length; i++)
                g.FillEllipse(new SolidBrush(Color.Red), _points[i].X - 6, _points[i].Y - 6, 12, 12);

            ConvexHullClass.ConvexHull(_points, _points.Length);


            List<Point> awt = new List<Point>(_convexHullList[_convexHullList.Count - 1]);
            for (int i = 0; i < awt.Count; i++)
            {
                Point a = awt[i];
                Point b = awt[(i + 1 + awt.Count) % awt.Count];
                g.DrawLine(new Pen(new SolidBrush(Color.Yellow), 2), a, b);
            }

            return pb;
        }

        private class ConvexHullClass
        {
            private static int Orientation(Point p, Point q, Point r)
            {
                int val = (q.Y - p.Y) * (r.X - q.X) -
                        (q.X - p.X) * (r.Y - q.Y);

                if (val == 0) return 0;

                return (val > 0) ? 1 : 2;
            }

            public static void ConvexHull(Point[] points, int n)
            {
                if (n < 3) return;

                List<Point> hull = new List<Point>();

                int l = 0;
                for (int i = 1; i < n; i++)
                    if (points[i].X < points[l].X)
                        l = i;

                int p = l, q;
                do
                {
                    hull.Add(points[p]);
                    _convexHullList.Add(new List<Point>(hull));

                    q = (p + 1) % n;

                    for (int i = 0; i < n; i++)
                    {
                        if (Orientation(points[p], points[i], points[q]) == 2)
                            q = i;
                    }


                    p = q;

                } while (p != l);
            }
        }
    }
}