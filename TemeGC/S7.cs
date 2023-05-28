namespace TemeGC
{
    internal static class S7
    {
        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;

        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;

            _points = new List<PointF>();
            _segments = new List<Segment>();
            _g = pb.CreateGraphics();
            _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            return pb;
        }

        //Additional Classes #start
        struct Segment
        {
            public readonly PointF p1;
            public readonly PointF p2;

            public Segment(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        //Additional classes #end


        //Mouse click handlers #start
        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _points.Count; i++)
                _segments.Add(new Segment(_points[i], _points[(i + 1 + _points.Count) % _points.Count]));


            //The angle of each vertice
            List<double> angles = new List<double>();
            for (int i = 0; i < _points.Count; i++)
            {
                PointF lambda = _points[(i - 1 + _points.Count) % _points.Count];
                PointF alpha = _points[i];
                PointF tau = _points[(i + 1 + _points.Count) % _points.Count];
                double d = CalculateAngle(lambda, alpha, tau);
                angles.Add(d);
            }


            //Generating every possible diagonal
            List<Segment> diagonals = new List<Segment>();
            for (int i = 0; i < _points.Count - 3; i++)
            {
                PointF lambda = _points[(i - 1 + _points.Count) % _points.Count];
                for (int j = i + 2; j < _points.Count - 1; j++)
                {
                    Segment aux = new Segment(_points[i], _points[j]);
                    bool good = true;
                    //checking if a diagonal intersects with the border of the polygon
                    foreach (var segment in _segments)
                        if (DoIntersect(aux, segment))
                            good = false;

                    //checking if a diagonal intersects with a previusly generated one
                    foreach (var diagonal in diagonals)
                        if (DoIntersect(aux, diagonal))
                            good = false;

                    //making sure the new diagonal is inside the polygon
                    double d = CalculateAngle(lambda, _points[i], _points[j]);
                    if (d > angles[i])
                        good = false;

                    if (good)
                        diagonals.Add(aux);
                }
            }

            
            foreach (Segment st in _segments)
                DrawSegment(st, Color.Crimson);

            foreach (var segment in diagonals)
                DrawSegment(segment, Color.Indigo);
            
            for (int i = 0; i < _points.Count; i++)
                DrawPoint(_points[i], i);


            if (sender is PictureBox pictureBox)
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

            int i = _points.Count - 1;
            DrawPoint(_points[i], i);
        }
        //Mouse click handlers #end


        //Helper functions #start
        private static double CalculateAngle(PointF a, PointF b, PointF c)
        {
            double angle = Math.Atan2(a.X - b.X, b.Y - a.Y) - Math.Atan2(c.X - b.X, b.Y - c.Y);
            angle = angle * (180 / Math.PI);
            if (angle < 0)
                angle += 360;

            return angle;
        }

        private static bool DoIntersect(Segment s, Segment p)
        {
            PointF s1 = s.p1;
            PointF s2 = s.p2;
            PointF p1 = p.p1;
            PointF p2 = p.p2;

            return Determinant(p2, p1, s1) * Determinant(p2, p1, s2) < 0 &&
                   Determinant(s2, s1, p1) * Determinant(s2, s1, p2) < 0;
        }

        private static float Determinant(PointF p, PointF q, PointF r)
        {
            return (p.X * q.Y) + (q.X * r.Y) + (r.X * p.Y) -
                   (q.Y * r.X) - (p.X * r.Y) - (p.Y * q.X);
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