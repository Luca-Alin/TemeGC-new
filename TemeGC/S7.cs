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

            if (points.Count > 1)
                segments.Add(
                    new Segment(points[0], points[points.Count - 1]
                ));

            for (int i = 0; i < segments.Count; i++)
                g.DrawLine(p, segments[i].p1, segments[i].p2);
            triangulation();

            PictureBox pictureBox = sender as PictureBox;
            pictureBox.MouseClick -= Pb_MouseClick;
            pictureBox.MouseClick -= Pb_MouseDoubleClick;
            points.Clear();
            segments.Clear();
        }

        private static void Pb_MouseClick(object? sender, MouseEventArgs e)
        {
            points.Add(new PointF(e.X, e.Y));
            if (points.Count > 1)
                segments.Add(
                    new Segment(points[points.Count - 2], points[points.Count - 1]
                ));
            g.FillEllipse(new SolidBrush(Color.Red), e.X - 5, e.Y - 5, 10, 10);
        }

        private static void triangulation()
        {
            List<Segment> diagonale = new List<Segment>();

            for (int i = 0; i < points.Count - 3; i++)
            {
                for (int j = i + 2; j < points.Count - 1; j++)
                {
                    if (i == 0 && j == points.Count - 1)
                        break;
                    bool good = true;
                    Segment aux = new Segment(points[i], points[j]);
                    for (int k = 0; k < diagonale.Count; k++)
                        if (doIntersect(aux, diagonale[k]))
                            good = false;


                    for (int k = 0; k < segments.Count; k++)
                        if (doIntersect(aux, segments[k]))
                            good = false;

                    good = IsInPolygon(i, j);


                    if (good)
                        diagonale.Add(aux);

                    if (diagonale.Count == points.Count - 3)
                        break;
                }
                if (diagonale.Count == points.Count - 3)
                    break;
            }

            for (int i = 0; i < diagonale.Count; i++)
                g.DrawLine(new Pen(new SolidBrush(Color.RoyalBlue), 2), diagonale[i].p1, diagonale[i].p2);

        }



        private static bool doIntersect(Segment s, Segment p)
        {
            PointF s1 = s.p1;
            PointF s2 = s.p2;
            PointF p1 = p.p1;
            PointF p2 = p.p2;

            return determinant(p2, p1, s1) * determinant(p2, p1, s2) < 0 && determinant(s2, s1, p1) * determinant(s2, s1, p2) < 0;
        }

        private static double determinant(PointF p, PointF q, PointF r)
        {
            return (p.X * q.Y) + (q.X * r.Y) + (r.X * p.Y) -
                    (q.Y * r.X) - (p.X * r.Y) - (p.Y * q.X);
        }

        private static double CalculateAngle(PointF A, PointF B, PointF C)
        {
            double angle = Math.Atan2(A.X - B.X, B.Y - A.Y) - Math.Atan2(C.X - B.X, B.Y - C.Y);

            // Calculate the angle between the two vectors
            // Convert the angle to degrees
            angle = angle * (180 / Math.PI);

            // Ensure the angle is between 0 and 360 degrees
            if (angle < 0)
                angle += 360;

            return angle;
        }

        private static bool IsInPolygon(int i, int j)
        {
            if (CalculateAngle(points[(i - 1 + points.Count) % points.Count], points[i], points[(i + 1 + points.Count) % points.Count]) < 180)
            {
                if (determinant(points[i], points[j], points[i + 1]) < 0 && determinant(points[i], points[(i - 1 + points.Count) % points.Count], points[j]) < 0)
                    return true;
                else if (determinant(points[i], points[j], points[i + 1]) > 0 && determinant(points[i], points[(i - 1 + points.Count) % points.Count], points[j]) > 0)
                    return true;
            }
            else
            {
                if (determinant(points[i], points[j], points[i + 1]) < 0 || determinant(points[i], points[(i - 1 + points.Count) % points.Count], points[j]) < 0)
                    return true;

                if (determinant(points[i], points[j], points[i + 1]) > 0 || determinant(points[i], points[(i - 1 + points.Count) % points.Count], points[j]) > 0)
                    return true;
            }
            return false;
        }




    }
}