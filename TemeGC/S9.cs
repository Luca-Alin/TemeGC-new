
using System.Drawing.Drawing2D;

namespace TemeGC
{
    internal class S9
    {
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

        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;
        private static List<double> _angles = null!;
        private static List<Varf> _varfuri = null!;
        public static PictureBox P1(PictureBox pb)
        {
            
            pb.Size = new Size(Form1.width, Form1.height);
            pb.MouseClick += Pb_MouseClick;
            pb.MouseDoubleClick += Pb_MouseDoubleClick;

            
            _varfuri = new List<Varf>();
            _points = new List<PointF>();
            _segments = new List<Segment>();
            
            _g = pb.CreateGraphics();
            _g.SmoothingMode = SmoothingMode.AntiAlias;



            return pb;
        }

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
                Segment s = new Segment(p1, p2);
                DrawSegment(s);
            }




            for (int i = 0; i < _points.Count; i++)
            {
                PointF pm1 = _points[(i - 1 + _points.Count) % _points.Count];
                PointF p = _points[i];
                PointF pp1 = _points[(i + 1 + _points.Count) % _points.Count];
                _varfuri.Add(new Varf(pm1, p, pp1));
            }
            
            
            _varfuri.Sort(
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
            
            for (int i = 0; i < _varfuri.Count; i++)
            {
                Varf varf = _varfuri[i];
                
                DrawPoint(varf.p, i);
                
                if (varf.esteVarfReflex)
                {
                    if (varf.pm1.Y > varf.p.Y && varf.pp1.Y > varf.p.Y)
                        DrawSegment(
                            new Segment(
                                varf.p, _varfuri[(i + 1 + _varfuri.Count) % _varfuri.Count].p    
                            )    
                        );
                    else if (varf.pm1.Y < varf.p.Y && varf.pp1.Y < varf.p.Y)
                        DrawSegment(
                            new Segment(
                                varf.p, _varfuri[(i - 1 + _varfuri.Count) % _varfuri.Count].p    
                            )    
                        );
                }
                
            }

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

        private static void DrawSegment(Segment s)
        {
            _g.DrawLine(new Pen(new SolidBrush(Color.Pink), 4), s.p1, s.p2);
        }

        private static void DrawPoint(PointF pF, int number)
        {
            String numberToString = (number < 10) ? (" " + number) : (number + "");
            
            
            _g.FillEllipse(new SolidBrush(Color.Blue), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawEllipse(new Pen(new SolidBrush(Color.Pink), 2), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawString(numberToString, new Font("Arial", 6), new SolidBrush(Color.White), new PointF(pF.X - 8, pF.Y - 6));
        }
        

        
    }
}