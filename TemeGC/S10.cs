namespace TemeGC
{
    internal class S10
    {
        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;

        private static PictureBox pb;
        private static int kappa = 0;

        public static PictureBox P1(PictureBox picbox)
        {
            pb = picbox;
            pb.Size = new Size(Form1.width, Form1.height);
            _g = pb.CreateGraphics();
            _points = new List<PointF>();
            _segments = new List<Segment>();

            for (int i = 0; i < 10; i++)
            {
                pain();
                kappa++;
            }
            
            return pb;
        }


        public static void pain()
        {
            pb.Image = null;
            pb.Update();
            _g.FillEllipse(new SolidBrush(Color.Red), 20 * kappa, 400, 10, 10);            
        }


        //Mouse click handlers #start
        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
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