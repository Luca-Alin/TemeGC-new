namespace TemeGC
{
    internal class S10
    {
        private static Graphics _g = null!;
        private static List<PointF> _points = null!;
        private static List<Segment> _segments = null!;

        private static PictureBox _pb = null!;


        public static PictureBox P1(PictureBox pictureBox)
        {
            _pb = pictureBox;
            _pb.Size = new Size(Form1.width, Form1.height);
            _pb.MouseClick += Pb_MouseClick;
            _pb.MouseDoubleClick += Pb_MouseDoubleClick;

            _g = _pb.CreateGraphics();
            _points = new List<PointF>();
            _segments = new List<Segment>();


            return _pb;
        }


        //Mouse click handlers #start
        private static void Pb_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                pictureBox.MouseClick -= Pb_MouseClick;
                pictureBox.MouseClick -= Pb_MouseDoubleClick;
            }

            for (int i = 0; i < _points.Count; i++)
                _segments.Add(
                    new Segment(
                        _points[i],
                        _points[(i + 1 + _points.Count) % _points.Count]
                    )
                );

            foreach (Segment segment in _segments)
                DrawSegment(segment, Color.Black);
                
            Triangulate(_points);
            
            
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


        //Additional functions #start
        public static void Triangulate(List<PointF> points)
        {
            List<int> monotoneChainA = new List<int>();
            List<int> monotoneChainB = new List<int>();
            List<int> HT = new List<int>();
            for (int i = 0; i < points.Count; i++)
                HT.Add(i);
            Console.Write(HT.Count + ": ");
            HT.ForEach(n => Console.Write(n + " "));
            Console.WriteLine();
            
            
            int lowestPointIndex = 0, highestPointIndex = 0;
            PointF lowestPoint = points[0], highestPoint = points[0];

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y > lowestPoint.Y)
                {
                    lowestPoint = points[i];
                    lowestPointIndex = i;
                }
                else if (points[i].Y < highestPoint.Y)
                {
                    highestPoint = points[i];
                    highestPointIndex = i;
                }
            }


            for (int i = highestPointIndex; i <= lowestPointIndex; i++)
                monotoneChainA.Add(i);
            

            for (int i = lowestPointIndex; i <= points.Count + highestPointIndex; i++)
                monotoneChainB.Add((i + points.Count) % points.Count);


            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].Y > points[j].Y)
                    {
                        (HT[i], HT[j]) = (HT[j], HT[i]);
                    }

                }
            }

            Stack<int> stack = new Stack<int>();
            stack.Push(0);
            stack.Push(1);

            Console.Write(HT.Count + ": ");
            HT.ForEach(n => Console.Write(n + " "));


            Console.WriteLine();
            
            
            
            for (int i = 3; i < points.Count - 1; i++)
            {
                foreach (var i1 in stack)
                {
                    Console.Write(i1 + " ");
                }

                Console.WriteLine();
                if (monotoneChainA.Contains(stack.Peek()) && monotoneChainB.Contains(HT[i]) ||
                    monotoneChainA.Contains(HT[i]) && monotoneChainB.Contains(stack.Peek()))
                {

                    int aux = stack.Peek();
                    while (stack.Count > 1)
                    {
                        int kappa = stack.Pop();
                        DrawSegment(
                            new Segment(points[kappa], points[HT[i]]), Color.Pink    
                        );
                    }
                    stack.Pop();
                    stack.Push(aux);
                    stack.Push(HT[i]);
                }
                else
                    stack.Push(HT[i]);
                
            }



            for (int i = 0; i < points.Count; i++)
            {
                DrawPoint(points[HT[i]], HT[i]);
            }
            
            
        }
        
        
        
        //Additional functions #end


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