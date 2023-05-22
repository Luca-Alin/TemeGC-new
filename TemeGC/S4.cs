using System.Runtime.Intrinsics.X86;

namespace TemeGC
{
    internal class S4
    {
        static int n;
        static List<Point> points;
        static Random random = new Random();
        static int lowerHull;
        static List<List<Point>> myHull = new List<List<Point>>();
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            n = 40;
            points = new List<Point>();
            for (int i = 0; i < n; i++)
            {
                int x = random.Next(40, Form1.WIDTH - 40);
                int y = random.Next(40, Form1.HEIGHT - 40);
                points.Add(new Point(x, y));
            }

            ConvexHull.convexHull(points);



            for (int i = 0; i < points.Count; i++)
                g.FillEllipse(new SolidBrush(Color.Yellow), points[i].X - 6, points[i].Y - 6, 12, 12);


            SolidBrush solidBrush = new SolidBrush(Color.Red);
            List<Point> awt = myHull[myHull.Count - 1];
            for (int i = 0; i < awt.Count - 1; i++)
            {
                Console.WriteLine(lowerHull);
                if (i >= lowerHull - 1)
                    solidBrush = new SolidBrush(Color.Blue);
                Point p1 = awt[i];
                Point p2 = awt[i + 1];
                g.DrawLine(new Pen(solidBrush, 2), p1, p2);
            }



            return pb;
        }

        class ConvexHull
        {

            static long crossProduct(Point O, Point A, Point B)
            {
                return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X);
            }


            public static List<Point> convexHull(List<Point> points)
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

                    while (k >= 2 && crossProduct(hullPoints[k - 2], hullPoints[k - 1], points[i]) <= 0)
                    {
                        hullPoints.RemoveAt(--k);
                    }
                    hullPoints.Add(points[i]);
                    myHull.Add(new List<Point>(hullPoints));
                    k++;
                }

                lowerHull = hullPoints.Count;

                for (int i = n - 2, t = k; i >= 0; --i)
                {

                    while (k > t && crossProduct(hullPoints[k - 2], hullPoints[k - 1], points[i]) <= 0)
                    {
                        hullPoints.RemoveAt(--k);
                    }
                    hullPoints.Add(points[i]);
                    myHull.Add(new List<Point>(hullPoints));
                    k++;
                }
                return hullPoints;
            }
        }
    }
}