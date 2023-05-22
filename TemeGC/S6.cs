using System;
using System.Collections.Generic;

namespace TemeGC
{
    internal class S6
    {
        static Random random = new Random();
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            List<PointF> points = new List<PointF>();
            for (int i = 0; i < 10; i++)
                points.Add(new PointF(random.Next(40, Form1.WIDTH - 40),
                                        random.Next(40, Form1.HEIGHT - 40)));

            //All possible segments
            List<Segment> APS = new List<Segment>();
            for (int i = 0; i < points.Count - 1; i++)
                for (int j = i + 1; j < points.Count; j++)
                    APS.Add(new Segment(points[i], points[j]));

            APS.Sort((s1, s2) =>
            {
                if (segmentSize(s1) > segmentSize(s2)) return 1;
                else if (segmentSize(s1) > segmentSize(s2)) return -1;
                return 0;
            });

            List<Segment> triangulation = new List<Segment>();
            for (int i = 0; i < APS.Count; i++)
            {
                Segment s = APS[i];
                bool ok = true;
                for (int j = 0; j < triangulation.Count; j++)
                    if (doIntersect(s, triangulation[j]))
                        ok = false;
                if (ok)
                    triangulation.Add(s);
            }


            triangulation.ForEach(s => g.DrawLine(
                new Pen(new SolidBrush(Color.Green), 4),
                s.p1,
                s.p2
                ));
            points.ForEach(p => g.FillEllipse(new SolidBrush(Color.Red), p.X - 6, p.Y - 6, 12, 12));

            return pb;
        }

        public static PictureBox P2(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            SolidBrush solidBrush = new SolidBrush(Color.Red);
            Point[] point = new Point[10];
            int i = 0;
            pb.MouseClick += (s, e) =>
            {
                if (i < 10)
                {
                    g.FillEllipse(solidBrush, e.X - 5, e.Y - 5, 10, 10);
                    point[i] = new Point(e.X, e.Y);
                    i++;
                }
                if (i == 10)
                {
                    solidBrush = new SolidBrush(Color.Blue);
                    g.DrawPolygon(new Pen(solidBrush, 4), point);
                    i++;
                }

            };
            return pb;
        }

        class Segment
        {
            public PointF p1;
            public PointF p2;
            public Segment(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        public static PictureBox P3(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);



            Graphics g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            SolidBrush solidBrush = new SolidBrush(Color.Red);
            List<PointF> points = new List<PointF>();
            List<Segment> segments = new List<Segment>();
            List<Segment> triangulation = new List<Segment>();
            int i = 0;
            pb.MouseClick += (s, e) =>
            {
                //drawing the points of the polygon, the variable n is the maximum number of points
                int n = 10;
                if (i < n)
                {
                    g.FillEllipse(solidBrush, e.X - 5, e.Y - 5, 10, 10);
                    points.Add(new PointF(e.X, e.Y));
                    i++;
                }
                //drawing the actual polygon
                if (i == 10)
                {
                    int p0I = 0;
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (points[i].X < points[p0I].X)
                        {
                            p0I = i;
                        }
                    }
                    int pMinus1I = (p0I - 1 + points.Count) % points.Count;
                    int pPlus1I = (p0I + 1 + points.Count) % points.Count;


                    for (int i = 0; i < points.Count; i++)
                    {
                        segments.Add(new Segment(points[i], points[(i + 1 + points.Count) % points.Count]));
                        g.DrawLine(new Pen(new SolidBrush(Color.Black), 2), points[i], points[(i + 1 + points.Count) % points.Count]);
                    }

                    PointF alpha = points[p0I];
                    PointF lambda = points[pMinus1I];
                    PointF tau = points[pPlus1I];

                    g.FillEllipse(new SolidBrush(Color.DeepPink), points[p0I].X - 5, points[p0I].Y - 5, 10, 10);
                    g.FillEllipse(new SolidBrush(Color.Yellow), points[pMinus1I].X - 5, points[pMinus1I].Y - 5, 10, 10);
                    g.FillEllipse(new SolidBrush(Color.Yellow), points[pPlus1I].X - 5, points[pPlus1I].Y - 5, 10, 10);

                    double angle = CalculateAngle(lambda, alpha, tau);
                    Console.WriteLine(angle);
                    g.DrawString(((int)angle + ""), new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(alpha.X + 5, alpha.Y + 5));
                    g.DrawString("Tau", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(tau.X + 5, tau.Y + 5));
                    g.DrawString("Lambda", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(lambda.X + 5, lambda.Y + 5));

                    for (int i = 0; i < points.Count; i++)
                    {
                        g.DrawString(i + " ", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(points[i].X - 10, points[i].Y - 10));
                    }


                    for (int i = 0; i < points.Count; i++)
                    {
                        if (i == p0I || i == pMinus1I || i == pPlus1I)
                            continue;
                        Segment segment = new Segment(alpha, points[i]);
                        bool good = true;

                        for (int j = 0; j < segments.Count; j++)
                            if (doIntersect(segment, segments[j]))
                                good = false;

                        double auxAngle;
                        if (points[i].Y <= alpha.Y)
                            auxAngle = CalculateAngle(lambda, alpha, points[i]);
                        else
                            auxAngle = CalculateAngle(lambda, alpha, points[i]);

                        if (auxAngle > angle)
                            good = false;

                        if (good)
                            g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), alpha, points[i]);
                    }
                }




            };
            return pb;
        }

        //Helper Functions
        private static bool doIntersect(Segment s, Segment p)
        {
            PointF s1 = s.p1;
            PointF s2 = s.p2;
            PointF p1 = p.p1;
            PointF p2 = p.p2;

            return determinant(p2, p1, s1) * determinant(p2, p1, s2) < 0 && determinant(s2, s1, p1) * determinant(s2, s1, p2) < 0;
        }

        private static float determinant(PointF p, PointF q, PointF r)
        {
            return (p.X * q.Y) + (q.X * r.Y) + (r.X * p.Y) -
                    (q.Y * r.X) - (p.X * r.Y) - (p.Y * q.X);
        }

        private static double segmentSize(Segment s)
        {
            return Math.Sqrt(
                        Math.Pow(s.p1.X - s.p2.X, 2) + Math.Pow(s.p1.Y - s.p2.Y, 2)
            );
        }
        private static double CalculateAngle(PointF A, PointF B, PointF C)
        {
            double angle = Math.Atan2(A.X - B.X, B.Y - A.Y) - Math.Atan2(C.X - B.X, B.Y - C.Y);
            angle = angle * (180 / Math.PI);
            if (angle < 0)
                angle += 360;

            return angle;
        }

        private static double CalculateDistance(PointF A, PointF B)
        {
            return Math.Sqrt(
                    Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2)
                );
        }

        private static List<PointF> GetTwoClosestTwoPoints(List<PointF> points, PointF point0)
        {
            int point0Index = -1;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X == point0.X && points[i].Y == point0.Y)
                    point0Index = i;
            }

            return new List<PointF> { points[((point0Index - 1 + points.Count) % points.Count)], points[((point0Index + 1 + points.Count) % points.Count)] };
        }
    }
}