namespace TemeGC
{
    internal class S7
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
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;

            points = new List<PointF>();
            segments = new List<Segment>();
            g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            return pb;
        }

        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {

            Pen p = new Pen(new SolidBrush(Color.RoyalBlue), 2);

            for (int i = 0; i < points.Count; i++)
                segments.Add(new Segment(points[i], points[(i + 1 + points.Count) % points.Count]));

            foreach (Segment st in segments)
                g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), st.p1, st.p2);


            List<double> angles = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                PointF lambda = points[(i - 1 + points.Count) % points.Count];
                PointF alpha = points[i];
                PointF tau = points[(i + 1 + points.Count) % points.Count];
                double d = CalculateAngle(lambda, alpha, tau);
                angles.Add(d);
            }

            for (int i = 0; i < points.Count; i++)
            {
                g.DrawString(angles[i] + " ", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(points[i].X - 10, points[i].Y - 10));
            }

            List<Segment> diagonals = new List<Segment>();
            for (int i = 0; i < points.Count - 3; i++)
            {
                PointF lambda = points[(i - 1 + points.Count) % points.Count];
                for (int j = i + 2; j < points.Count - 1; j++)
                {
                    Segment aux = new Segment(points[i], points[j]);
                    bool good = true;
                    for (int k = 0; k < segments.Count; k++)
                        if (doIntersect(aux, segments[k]))
                            good = false;

                    for (int k = 0; k < diagonals.Count; k++)
                        if (doIntersect(aux, diagonals[k]))
                            good = false;

                    double d = CalculateAngle(lambda, points[i], points[j]);

                    if (d > angles[i])
                        good = false;

                    if (good)
                        diagonals.Add(aux);
                }
            }


            for (int i = 0; i < diagonals.Count; i++)
            {
                Segment s = diagonals[i];
                g.DrawLine(new Pen(new SolidBrush(Color.Black), 2), s.p1, s.p2);
            }

            PictureBox pictureBox = sender as PictureBox;
            pictureBox.MouseClick -= Pb_MouseClick;
            pictureBox.MouseClick -= Pb_MouseDoubleClick;
            points.Clear();
            segments.Clear();
        }

        private static void Pb_MouseClick(object? sender, MouseEventArgs e)
        {
            points.Add(new PointF(e.X, e.Y));
            g.FillEllipse(new SolidBrush(Color.Red), e.X - 6, e.Y - 6, 12, 12);
        }

        private static double CalculateAngle(PointF A, PointF B, PointF C)
        {
            double angle = Math.Atan2(A.X - B.X, B.Y - A.Y) - Math.Atan2(C.X - B.X, B.Y - C.Y);
            angle = angle * (180 / Math.PI);
            if (angle < 0)
                angle += 360;

            return angle;
        }

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


    }
}