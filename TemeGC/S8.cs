using System.Drawing.Drawing2D;
using System.Numerics;

namespace TemeGC
{
    internal static class S8
    {
        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;
        private static List<double> _angles = null!;
        private static double _area = 0;

        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;


            _points = new List<PointF>();
            _segments = new List<Segment>();
            _angles = new List<double>();
            _area = 0;
            _g = pb.CreateGraphics();
            _g.SmoothingMode = SmoothingMode.AntiAlias;


            return pb;
        }


        //Additional Classes #start
        private struct Segment
        {
            public readonly PointF p1;
            public readonly PointF p2;

            public Segment(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        //Additional Classes #end

        //Mouse actions #start
        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                PointF pm1 = _points[(i - 1 + _points.Count) % _points.Count];
                PointF p = _points[i];
                PointF pp1 = _points[(i + 1 + _points.Count) % _points.Count];
                _angles.Add(CalculateAngle(pm1, p, pp1));
            }


            for (int i = 0; i < _points.Count; i++)
                _segments.Add(new Segment(_points[i], _points[(i + 1 + _points.Count) % _points.Count]));

            for (int i = 0; i < _points.Count; i++)
            {
                _g.DrawString(_angles[i] + " ", new Font("Arial", 10), new SolidBrush(Color.Red),
                    new PointF(_points[i].X + 10, _points[i].Y + 10));
                if (_angles[i] < 180)
                    _g.FillEllipse(new SolidBrush(Color.Yellow), _points[i].X - 6, _points[i].Y - 6, 12, 12);
            }


            foreach (Segment st in _segments)
                _g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), st.p1, st.p2);

            List<int> indexList = new List<int>();
            for (int i = 0; i < _points.Count; i++)
                indexList.Add(i);

            int totalTriangleCount = _points.Count - 2;
            int totalTriangleIndexCount = totalTriangleCount * 3;

            int[] triangles = new int[totalTriangleIndexCount];
            Array.ForEach(triangles, n => Console.Write(n + " "));

            int trianglesIndexCount = 0;

            while (indexList.Count > 3)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    int a = indexList[i];
                    int b = indexList[(i - 1 + indexList.Count) % indexList.Count];
                    int c = indexList[(i + 1 + indexList.Count) % indexList.Count];

                    Console.WriteLine(@"abc" + a + @" " + b + @" " + c);
                    PointF pointA = _points[a];
                    PointF pointB = _points[b];
                    PointF pointC = _points[c];

                    if (_angles[a] >= 180)
                    {
                        Console.WriteLine(_angles[a] + @" >= 180");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(_angles[a] + @" < 180");
                    }

                    double triangleArea = TriangleArea(pointA, pointB, pointC);
                    bool isEar = true;
                    for (int j = 0; j < _points.Count; j++)
                    {
                        if (j == a || j == b || j == c)
                            continue;

                        double triangleArea1 = TriangleArea(_points[j], _points[a], _points[b]);
                        double triangleArea2 = TriangleArea(_points[j], _points[a], _points[c]);
                        double triangleArea3 = TriangleArea(_points[j], _points[b], _points[c]);

                        if (IsPointInTriangle(_points[a], _points[b], _points[c], _points[j]))
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
                        Console.WriteLine(@"trianglesIndexCount: " + trianglesIndexCount);

                        indexList.RemoveAt(i);
                        Array.ForEach(triangles, n => Console.Write(n + " "));
                        Console.WriteLine();
                        break;
                    }
                }
            }


            triangles[trianglesIndexCount++] = indexList[0];
            triangles[trianglesIndexCount++] = indexList[1];
            triangles[trianglesIndexCount] = indexList[2];
            Array.ForEach(triangles, n => Console.Write(n + @" "));
            Console.WriteLine();


            for (int i = 0; i < triangles.Length; i += 3)
            {
                int A = triangles[i];
                int B = triangles[i + 1];
                int C = triangles[i + 2];
                PointF a = _points[A];
                PointF b = _points[B];
                PointF c = _points[C];
                _area += TriangleArea(a, b, c);
                _g.DrawLine(new Pen(new SolidBrush(Color.HotPink), 2), a, c);
                _g.FillEllipse(new SolidBrush(Color.Orange), a.X - 6, a.Y - 6, 12, 12);
                _g.FillEllipse(new SolidBrush(Color.Violet), b.X - 6, b.Y - 6, 12, 12);
                _g.FillEllipse(new SolidBrush(Color.Brown), c.X - 6, c.Y - 6, 12, 12);
            }

            _g.DrawString(("Area: " + _area), new Font("Arial", 10), new SolidBrush(Color.Black), 10, 10);

            PictureBox? pictureBox = sender as PictureBox;
            if (pictureBox != null)
            {
                pictureBox.MouseClick -= Pb_MouseClick;
                pictureBox.MouseClick -= Pb_MouseDoubleClick;
            }

            _points.Clear();
            _segments.Clear();
        }

        private static void Pb_MouseClick(object? sender, MouseEventArgs e)
        {
            _points.Add(new PointF(e.X, e.Y));
            _g.FillEllipse(new SolidBrush(Color.Red), e.X - 6, e.Y - 6, 12, 12);
            _g.DrawString(_points.Count - 1 + " ", new Font("Arial", 10), new SolidBrush(Color.Black),
                new PointF(e.X - 20, e.Y - 20));
        }
        //Mouse actions #end

        //Helper functions #start
        private static double CalculateAngle(PointF A, PointF B, PointF C)
        {
            double angle = Math.Atan2(A.X - B.X, B.Y - A.Y) - Math.Atan2(C.X - B.X, B.Y - C.Y);
            angle = angle * (180 / Math.PI);
            if (angle < 0)
                angle += 360;

            return angle;
        }

        private static bool IsPointInTriangle(PointF pa, PointF pb, PointF pc, PointF pn)
        {
            Vector2 a = new Vector2(pa.X, pa.Y);
            Vector2 b = new Vector2(pb.X, pb.Y);
            Vector2 c = new Vector2(pc.X, pc.Y);
            Vector2 n = new Vector2(pn.X, pn.Y);

            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;
            
            Vector2 an = n - a;
            Vector2 bn = n - b;
            Vector2 cn = n - c;

            float cross1 = ab.X * an.Y - ab.Y * an.X;
            float cross2 = bc.X * bn.Y - bc.Y * bn.X;
            float cross3 = ca.X * cn.Y - ca.Y * cn.X;

            return (cross1 > 0f || cross2 > 0f || cross3 > 0f) && false;
        }
        
        private static double TriangleArea(PointF a, PointF b, PointF c)
        {
            return Math.Abs(
                (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2
            );
        }
        //Helper functions #end


        //Drawing functions #start
        private static void DrawPoint(PointF pF, int number)
        {
            String numberToString = (number < 10) ? (" " + number) : (number + "");

            _g.FillEllipse(new SolidBrush(Color.Blue), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawEllipse(new Pen(new SolidBrush(Color.Red), 2), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawString(numberToString, new Font("Arial", 6), new SolidBrush(Color.White),
                new PointF(pF.X - 8, pF.Y - 6));
        }

        private static void DrawSegment(Segment s, Color color)
        {
            _g.DrawLine(new Pen(new SolidBrush(color), 4), s.p1, s.p2);
        }
        //Drawing functions #end
    }
}