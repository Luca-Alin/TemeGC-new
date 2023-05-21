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

            Console.WriteLine(points.Count);
            for (int i = 0; i < points.Count; i++)
            {

                double d = CalculateAngle(points[(i - 1 + points.Count) % points.Count], points[i], points[(i + 1 + points.Count) % points.Count]);
                //g.DrawString(d + "", new System.Drawing.Font("Arial", 10), new SolidBrush(Color.Black), points[i]);
                if (d >= 180)
                {
                    g.DrawString("Convex", new System.Drawing.Font("Arial", 10), new SolidBrush(Color.Black), points[i]);
                }
                else
                {
                    g.DrawString("Reflex", new System.Drawing.Font("Arial", 10), new SolidBrush(Color.Black), points[i]);
                }
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
            if (points.Count > 1)
                segments.Add(
                    new Segment(points[points.Count - 2], points[points.Count - 1]
                ));
            g.FillEllipse(new SolidBrush(Color.Red), e.X - 5, e.Y - 5, 10, 10);
            //g.DrawString((points.Count - 1) + " ", new System.Drawing.Font("Arial", 10), new SolidBrush(Color.Black), new PointF(e.X + 5, e.Y + 5));
        }

        private static double determinant(PointF p, PointF q, PointF r)
        {
            return (p.X * q.Y) + (q.X * r.Y) + (r.X * p.Y) -
                    (q.Y * r.X) - (p.X * r.Y) - (p.X * q.Y);
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



    }
}