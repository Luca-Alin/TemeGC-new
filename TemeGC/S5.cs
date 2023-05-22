namespace TemeGC
{
    internal class S5
    {
        static int n;
        static Point[] points;
        static Random random = new Random();
        static List<List<Point>> convexHullList = new List<List<Point>>();

        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            Console.WriteLine("Yes");

            n = 40;
            points = new Point[n];
            for (int i = 0; i < points.Length; i++)
            {
                int x = random.Next(40, Form1.WIDTH - 40);
                int y = random.Next(40, Form1.HEIGHT - 40);
                points[i] = new Point(x, y);
            }
            Console.WriteLine("Yes");

            for (int i = 0; i < points.Length; i++)
                g.FillEllipse(new SolidBrush(Color.Red), points[i].X - 6, points[i].Y - 6, 12, 12);

            ConvexHull.convexHull(points, points.Length);


            List<Point> awt = new List<Point>(convexHullList[convexHullList.Count - 1]);
            for (int i = 0; i < awt.Count; i++)
            {
                Point a = awt[i];
                Point b = awt[(i + 1 + awt.Count) % awt.Count];
                g.DrawLine(new Pen(new SolidBrush(Color.Yellow), 2), a, b);
            }

            return pb;
        }

        class ConvexHull
        {
            static int orientation(Point p, Point q, Point r)
            {
                int val = (q.Y - p.Y) * (r.X - q.X) -
                        (q.X - p.X) * (r.Y - q.Y);

                if (val == 0) return 0;

                return (val > 0) ? 1 : 2;
            }

            public static void convexHull(Point[] points, int n)
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
                    convexHullList.Add(new List<Point>(hull));

                    q = (p + 1) % n;

                    for (int i = 0; i < n; i++)
                    {
                        if (orientation(points[p], points[i], points[q]) == 2)
                            q = i;
                    }


                    p = q;

                } while (p != l);
            }
        }
    }
}