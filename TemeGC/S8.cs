using System.Numerics;
using System.Runtime.CompilerServices;

namespace TemeGC
{
    internal class S8
    {
        struct Segment
        {
            public PointF p1;
            public PointF p2;
            public Segment(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }

        static Graphics g;
        static List<PointF> points;
        static List<Segment> segments;
        static List<double> angles;
        static double area = 0;
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;



            points = new List<PointF>();
            segments = new List<Segment>();
            angles = new List<double>();
            area = 0;
            g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;



            return pb;
        }

        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {


            try
            {
                Pen p = new Pen(new SolidBrush(Color.RoyalBlue), 2);

                for (int i = 0; i < points.Count; i++)
                {
                    PointF lambda = points[(i - 1 + points.Count) % points.Count];
                    PointF alpha = points[i];
                    PointF tau = points[(i + 1 + points.Count) % points.Count];
                    angles.Add(CalculateAngle(lambda, alpha, tau));
                }


                for (int i = 0; i < points.Count; i++)
                    segments.Add(new Segment(points[i], points[(i + 1 + points.Count) % points.Count]));

                for (int i = 0; i < points.Count; i++)
                {
                    g.DrawString(angles[i] + " ", new Font("Arial", 10), new SolidBrush(Color.Red), new PointF(points[i].X + 10, points[i].Y + 10));
                    if (angles[i] < 180)
                        g.FillEllipse(new SolidBrush(Color.Yellow), points[i].X - 6, points[i].Y - 6, 12, 12);

                }


                foreach (Segment st in segments)
                    g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), st.p1, st.p2);

                List<int> indexList = new List<int>();
                for (int i = 0; i < points.Count; i++)
                    indexList.Add(i);

                int totalTriangleCount = points.Count - 2;
                int totalTriangleIndexCount = totalTriangleCount * 3;

                int[] triangles = unchecked(new int[totalTriangleIndexCount]);
                Array.ForEach(triangles, n => Console.Write(n + " "));

                int trianglesIndexCount = 0;

                while (indexList.Count > 3)
                {
                    for (int i = 0; i < indexList.Count; i++)
                    {
                        int a = indexList[i];
                        int b = indexList[(i - 1 + indexList.Count) % indexList.Count];
                        int c = indexList[(i + 1 + indexList.Count) % indexList.Count];

                        Console.WriteLine("abc" + a + " " + b + " " + c);
                        PointF pointA = points[a];
                        PointF pointB = points[b];
                        PointF pointC = points[c];

                        if (angles[a] >= 180)
                        {
                            Console.WriteLine(angles[a] + " >= 180");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine(angles[a] + " < 180");
                        }

                        double triangleArea = TriangleArea(pointA, pointB, pointC);
                        bool isEar = true;
                        for (int j = 0; j < points.Count; j++)
                        {
                            if (j == a || j == b || j == c)
                                continue;

                            double triangleArea1 = TriangleArea(points[j], points[a], points[b]);
                            double triangleArea2 = TriangleArea(points[j], points[a], points[c]);
                            double triangleArea3 = TriangleArea(points[j], points[b], points[c]);

                            if (triangleArea - (triangleArea * 15) / 100 >= triangleArea1 + triangleArea2 + triangleArea3 &&
                                triangleArea + (triangleArea * 15) / 100 <= triangleArea1 + triangleArea2 + triangleArea3)
                            {
                                isEar = false;
                                break;
                            }
                        }

                        if (isEar)
                        {
                            triangles[trianglesIndexCount++] = b;
                            triangles[trianglesIndexCount++] = a;
                            triangles[trianglesIndexCount++] = c;
                            Console.WriteLine("trianglesIndexCount: " + trianglesIndexCount);

                            indexList.RemoveAt(i);
                            Array.ForEach(triangles, n => Console.Write(n + " "));
                            Console.WriteLine();
                            break;
                        }


                    }
                }



                triangles[trianglesIndexCount++] = indexList[0];
                triangles[trianglesIndexCount++] = indexList[1];
                triangles[trianglesIndexCount++] = indexList[2];
                Array.ForEach(triangles, n => Console.Write(n + " "));
                Console.WriteLine();


                for (int i = 0; i < triangles.Length; i += 3)
                {

                    int A = triangles[i];
                    int B = triangles[i + 1];
                    int C = triangles[i + 2];
                    PointF a = points[A];
                    PointF b = points[B];
                    PointF c = points[C];
                    area += TriangleArea(a, b, c);
                    g.DrawLine(new Pen(new SolidBrush(Color.HotPink), 2), a, c);
                    g.FillEllipse(new SolidBrush(Color.Orange), a.X - 6, a.Y - 6, 12, 12);
                    g.FillEllipse(new SolidBrush(Color.Violet), b.X - 6, b.Y - 6, 12, 12);
                    g.FillEllipse(new SolidBrush(Color.Brown), c.X - 6, c.Y - 6, 12, 12);
                    Console.WriteLine("MC: " + i + ", " + (i + 1) + ", " + (i + 2) + "; " + triangles.Length);



                }

                g.DrawString(("Area: " + area), new Font("Arial", 10), new SolidBrush(Color.Black), 10, 10);

                PictureBox pictureBox = sender as PictureBox;
                pictureBox.MouseClick -= Pb_MouseClick;
                pictureBox.MouseClick -= Pb_MouseDoubleClick;
                points.Clear();
                segments.Clear();
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.StackTrace);
            }
        }

        private static void Pb_MouseClick(object? sender, MouseEventArgs e)
        {
            points.Add(new PointF(e.X, e.Y));
            g.FillEllipse(new SolidBrush(Color.Red), e.X - 6, e.Y - 6, 12, 12);
            g.DrawString(points.Count - 1 + " ", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(e.X - 20, e.Y - 20));

        }



        private static double CalculateAngle(PointF A, PointF B, PointF C)
        {
            double angle = Math.Atan2(A.X - B.X, B.Y - A.Y) - Math.Atan2(C.X - B.X, B.Y - C.Y);
            angle = angle * (180 / Math.PI);
            if (angle < 0)
                angle += 360;

            return angle;
        }

        private static double TriangleArea(PointF a, PointF b, PointF c)
        {
            return Math.Abs(
                        (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2
                    );
        }
    }
}