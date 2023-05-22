namespace TemeGC
{
    internal class S9
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
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;



            points = new List<PointF>();
            segments = new List<Segment>();
            angles = new List<double>();
            g = pb.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;



            return pb;
        }

        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
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
            g.DrawString(points.Count - 1 + " ", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(e.X - 20, e.Y - 20));

        }
    }
}