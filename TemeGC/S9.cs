using System.Drawing.Drawing2D;

namespace TemeGC
{
    internal class S9
    {
        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;
        private static List<Varf> _varfuri = null!;
        private static List<Varf> _monotoneChainA = null!;
        private static List<Varf> _monotoneChainB = null!;

        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;


            _g = pb.CreateGraphics();
            _varfuri = new List<Varf>();
            _points = new List<PointF>();
            _segments = new List<Segment>();
            _monotoneChainA = new List<Varf>();
            _monotoneChainB = new List<Varf>();

            _g.SmoothingMode = SmoothingMode.AntiAlias;


            return pb;
        }

        //Mouse actions #start
        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                pictureBox.MouseClick -= Pb_MouseClick;
                pictureBox.MouseClick -= Pb_MouseDoubleClick;
            }

            for (int i = 0; i < _points.Count; i++)
            {
                PointF p1 = _points[(i + _points.Count) % _points.Count];
                PointF p2 = _points[(i + 1 + _points.Count) % _points.Count];
                _segments.Add(new Segment(p1, p2));
            }


            for (int i = 0; i < _points.Count; i++)
            {
                PointF pm1 = _points[(i - 1 + _points.Count) % _points.Count];
                PointF p = _points[i];
                PointF pp1 = _points[(i + 1 + _points.Count) % _points.Count];
                _varfuri.Add(new Varf(pm1, p, pp1));
            }

            CreateMonotoneChains();
            
            _points.Clear();
            _segments.Clear();
            _varfuri.Clear();
        }

        private static void Pb_MouseClick(object? sender, MouseEventArgs e)
        {
            PointF pF = new PointF(e.X, e.Y);
            _points.Add(pF);
            DrawPoint(pF, _points.Count - 1);
        }

        //Mouse actions #end


        //Additional function #start
        private static void CreateMonotoneChains()
        {
            Console.WriteLine("SY: " + _varfuri.Count());
            int lowestPointIndex = 0, highestPointIndex = 0;
            PointF lowestPoint = _varfuri[0].p, highestPoint = _varfuri[0].p;

            for (int i = 0; i < _varfuri.Count; i++)
            {
                if (_varfuri[i].p.Y > lowestPoint.Y)
                {
                    lowestPoint = _varfuri[i].p;
                    lowestPointIndex = i;
                }
                else if ((_varfuri[i].p.Y < highestPoint.Y))
                {
                    highestPoint = _varfuri[i].p;
                    highestPointIndex = i;
                }
            }


            for (int i = highestPointIndex; i <= lowestPointIndex; i++)
            {
                _monotoneChainA.Add(_varfuri[i]);
            }

            for (int i = lowestPointIndex; i <= _varfuri.Count + highestPointIndex; i++)
            {
                _monotoneChainB.Add(_varfuri[(i + _varfuri.Count) % _varfuri.Count]);
            }

            DrawMonotoneChains(_monotoneChainA, Color.Blue);
            DrawMonotoneChains(_monotoneChainB, Color.Red);

            SortMonotoneChains(_monotoneChainA);
            SortMonotoneChains(_monotoneChainB);
            
            TriangulateMonotoneChains(_monotoneChainA);
            TriangulateMonotoneChains(_monotoneChainB);
        }

        private static void SortMonotoneChains(List<Varf> list)
        {
            list.Sort(
                (v1, v2) =>
                {
                    PointF p1 = v1.p;
                    PointF p2 = v2.p;

                    if (p1.Y > p2.Y)
                        return 1;
                    if ((int)p1.Y == (int)p2.Y)
                        if (p1.X > p2.X)
                            return 1;
                        else
                            return -1;
                    return -1;
                }
            );
        }

        private static void TriangulateMonotoneChains(List<Varf> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].esteVarfReflex)
                    if (list[i].pm1.Y > list[i].p.Y && list[i].pp1.Y > list[i].p.Y)
                    {
                        int j = i - 1;
                        while (DoIntersect(new Segment(list[i].p, list[j].p)))
                        {
                            j--;
                        }
                        DrawSegment(new Segment(list[i].p, list[j].p));
                    }
                    else if (list[i].pm1.Y < list[i].p.Y && list[i].pp1.Y < list[i].p.Y)
                    {
                        int j = i + 1;
                        while (DoIntersect(new Segment(list[i].p, list[j].p)))
                        {
                            j++;
                        }
                        DrawSegment(new Segment(list[i].p, list[i + 1].p));
                    }
            }
        }
        
        private static bool DoIntersect(Segment s)
        {
            bool good = false;
            for (int i = 0; i < _segments.Count; i++)
            {
                PointF s1 = s.p1;
                PointF s2 = s.p2;
                PointF p1 = _segments[i].p1;
                PointF p2 = _segments[i].p2;
                good = Determinant(p2, p1, s1) * Determinant(p2, p1, s2) < 0 &&
                       Determinant(s2, s1, p1) * Determinant(s2, s1, p2) < 0;
            }
            
            return good;
        }

        private static float Determinant(PointF p, PointF q, PointF r)
        {
            return (p.X * q.Y) + (q.X * r.Y) + (r.X * p.Y) -
                   (q.Y * r.X) - (p.X * r.Y) - (p.Y * q.X);
        }
        
        //Additional function #end

        //Draw functions #start

        private static void DrawMonotoneChains(List<Varf> list, Color color)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                Varf a = list[i];
                Varf b = list[i + 1];
                _g.DrawLine(new Pen(color, 4), a.p, b.p);
            }
        }

        private static void DrawSegment(Segment s)
        {
            _g.DrawLine(new Pen(new SolidBrush(Color.Pink), 4), s.p1, s.p2);
        }

        private static void DrawPoint(PointF pF, int number)
        {
            String numberToString = (number < 10) ? (" " + number) : (number + "");


            _g.FillEllipse(new SolidBrush(Color.Blue), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawEllipse(new Pen(new SolidBrush(Color.Pink), 2), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawString(numberToString, new Font("Arial", 6), new SolidBrush(Color.White),
                new PointF(pF.X - 8, pF.Y - 6));
        }
        //Draw functions #end


        //Additional classes #start
        private struct Segment
        {
            public PointF p1;
            public PointF p2;

            public Segment(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }

        private struct Varf
        {
            public PointF pm1;
            public PointF p;
            public PointF pp1;
            public bool esteVarfReflex;

            public Varf(PointF pm1, PointF p, PointF pp1)
            {
                this.pm1 = pm1;
                this.p = p;
                this.pp1 = pp1;

                esteVarfReflex = CalculateAngle(pm1, p, pp1);
            }

            private static bool CalculateAngle(PointF a, PointF b, PointF c)
            {
                double angle = Math.Atan2(a.X - b.X, b.Y - a.Y) - Math.Atan2(c.X - b.X, b.Y - c.Y);
                angle = angle * (180 / Math.PI);
                if (angle < 0)
                    angle += 360;

                return angle > 180;
            }
        }
        //Additional classes #end
    }
}