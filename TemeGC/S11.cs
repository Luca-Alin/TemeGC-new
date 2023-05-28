namespace TemeGC
{
    internal class S11
    {
        public static GraphPanel gpanel = null!;

        public enum PhaseType
        {
            Draw,
            Triangle,
            Final
        }

        public static PictureBox pb = null!;
        public static PhaseType phase = PhaseType.Draw;
        public static Graphics g = null!;

        public static PictureBox P1(PictureBox pbi)
        {
            pb = pbi;

            pb.Size = new Size(Form1.width, Form1.height);
            g = pb.CreateGraphics();
            PaintEverything();

            InfoPanel infoPanel = new InfoPanel(pb);
            GraphPanel graphPanel = new GraphPanel(pb);

            return pb;
        }


        public static void PaintEverything()
        {
            pb.Image = null;
            pb.Update();

            //Lower Panel #start
            String str = "Phase:";
            switch (S11.phase)
            {
                case S11.PhaseType.Draw:
                    str += "Left Click to Add Vertices, Right Click to Remove!";
                    break;
                case S11.PhaseType.Triangle:
                    str += "Now we've triangulated it, click CONTINUE to see the edges we can remove";
                    break;
                case S11.PhaseType.Final:
                    str += "We've removed as many diagonals as we could! We have an 4-approximation!";
                    break;
            }

            g.DrawString(str, new Font("Arial", 6), new SolidBrush(Color.Red), new PointF(30, Form1.height - 30));
            g.FillRectangle(new SolidBrush(Color.Black), 700, Form1.height - 50, 130, 50);
            g.DrawString("CONTINUE", new Font("Arial", 6), new SolidBrush(Color.Red),
                new PointF(730, Form1.height - 30));

            g.FillRectangle(new SolidBrush(Color.Purple), 850, Form1.height - 50, 130, 50);
            g.DrawString("CLEAR", new Font("Arial", 6), new SolidBrush(Color.Red), new PointF(880, Form1.height - 30));
            //Lower Panel #end
        }


        //Additional functions #start
        public static void PhaseAdd()
        {
            phase = (phase.Equals(PhaseType.Draw)) ? PhaseType.Triangle :
                (phase.Equals(PhaseType.Triangle)) ? PhaseType.Final : PhaseType.Draw;

            if (phase == PhaseType.Draw)
            {
                GraphPanel.polygon = new SimplePolygon(GraphPanel.vertices);
                GraphPanel.edgesList = new List<Edge>();
            }

            if (phase == PhaseType.Triangle)
            {
                GraphPanel.edgesList = GraphPanel.polygon.Triangulate();
                GraphPanel.polygon.Incorporate();
            }

            if (phase == PhaseType.Final)
            {
                Console.WriteLine("General");
                GraphPanel.Decompose();
                Console.WriteLine("Kenobi");
            }
            
            GraphPanel.PaintComponent();
        }

        public static void PhaseClear()
        {
            GraphPanel.polygon = new SimplePolygon(new Vertex[] { });
            GraphPanel.vertices = new List<Vertex>();
            GraphPanel.edgesList = new List<Edge>();
            phase = PhaseType.Draw;
            
            GraphPanel.PaintComponent();

        }
        //Additional functions #end


        //Drawing functions #start
        private static void DrawPoint(PointF pF, int number)
        {
            String numberToString = (number < 10) ? (" " + number) : (number + "");

            g.FillEllipse(new SolidBrush(Color.Blue), pF.X - 8, pF.Y - 8, 16, 16);
            g.DrawEllipse(new Pen(new SolidBrush(Color.Red), 2), pF.X - 8, pF.Y - 8, 16, 16);
            g.DrawString(numberToString, new Font("Arial", 6), new SolidBrush(Color.White),
                new PointF(pF.X - 8, pF.Y - 6));
        }

        //Drawing functions #end
    }

    //Additional Classes #start
    class InfoPanel
    {
        public InfoPanel(PictureBox pictureBox)
        {
            pictureBox.MouseClick += InfoPanelMc;
        }


        public static void InfoPanelMc(object? sender, MouseEventArgs e)
        {
            if (e.X > 700 && e.X < 830 && e.Y > Form1.height - 50 && e.Y < Form1.height)
                S11.PhaseAdd();
            if (e.X > 850 && e.X < 980 && e.Y > Form1.height - 50 && e.Y < Form1.height)
                S11.PhaseClear();
        }
    } //Working Well

    class GraphPanel
    {
        public static SimplePolygon polygon = null!;
        public static List<Edge> edgesList = null!;
        public static List<Vertex> vertices = null!;


        public GraphPanel(PictureBox pb)
        {
            pb.MouseClick += GraphPanelMc;

            polygon = new SimplePolygon(new Vertex[] { });
            vertices = new List<Vertex>();
            edgesList = new List<Edge>();
        }

        public static void PaintComponent()
        {
            S11.PaintEverything();

            polygon.Paint(S11.g);
            if (S11.phase != S11.PhaseType.Draw)
            {
                int i = 0;
                foreach (Edge edge in edgesList)
                {
                    edge.Paint(S11.g, i++);
                }
            }
        }

        public static void Decompose()
        {
            Console.WriteLine(@"hello");
            foreach (Vertex vertex in vertices)
            {
                vertex.GetEdges().Sort((o1, o2) =>
                {
                    if ((o1.GetCenter()[1] - vertex.GetY()) * (o2.GetCenter()[1] - vertex.GetY()) < 0)
                        return o1.GetCenter()[1] - o2.GetCenter()[1];
                    int crossProduct = SimplePolygon.CrossProduct(vertex.GetCoordsArr(), o1.GetCenter(),
                        vertex.GetCoordsArr(), o2.GetCenter());
                    //return Integer.compare(crossProduct, 0);
                    return crossProduct == 0 ? 0 : crossProduct < 0 ? -1 : 1;
                });
            }

            for (int i = 0; i < edgesList.Count && edgesList.Count != 0; i++)
            {
                Edge edge = edgesList[i];
                if (VertexClearance(edge, edge.GetEnd()) && VertexClearance(edge, edge.GetStart()))
                {
                    RemoveEdge(edge);
                    i--;
                }
            }

            Console.WriteLine(@"there");
        }

        public static bool VertexClearance(Edge edge, Vertex vertex)
        {
            List<Edge> edges = vertex.GetEdges();
            if (edges.Count == 3)
            {
                return vertex.SinAngle(vertex.GetPrev(), vertex.GetNext()) <= 0;
            }

            Edge prev = edges[(edges.IndexOf(edge) - 1 + edges.Count) % edges.Count];
            Edge next = edges[(edges.IndexOf(edge) + 1) % edges.Count];
            return vertex.SinAngle(prev, next) <= 0;
        }

        public static void RemoveEdge(Edge edge)
        {
            edge.GetStart().RemoveEdge(edge);
            edge.GetEnd().RemoveEdge(edge);
            edgesList.Remove(edge);
        }


        private static void GraphPanelMc(object? sender, MouseEventArgs e)
        {
            if (e.Y < Form1.height - 100)
                if (S11.phase == S11.PhaseType.Draw)
                {
                    vertices.Add(new Vertex(e.X, e.Y));
                    polygon = new SimplePolygon(vertices);

                    PaintComponent();

                    /*if (e.getButton() == MouseEvent.BUTTON3)
                    {
                        Vertex closest = vertices[0);
                        double dist = Math.hypot(closest.getX() - e.X, closest.getY() - e.Y);
                        for (int i = 1; i < vertices.Count; i++)
                        {
                            Vertex vertex = vertices[i);
                            if (Math.hypot(vertex.getX() - e.X, vertex.getY() - e.Y) < dist)
                            {
                                dist = Math.hypot(vertex.getX() - e.X, vertex.getY() - e.Y);
                                closest = vertex;
                            }
                        }
    
                        vertices.Remove(closest);
                        polygon = new SimplePolygon(vertices);
                    }*/
                }
        }
    }

    class SimplePolygon
    {
        List<Vertex> _vertices;
        List<Edge> _edges = null!;
        bool _clockwise;
        int[,] _coordsArray;

        public static Color[] colors = { Color.Gray, Color.Crimson, Color.LawnGreen, Color.DodgerBlue };

        public SimplePolygon(Vertex[] vertices)
        {
            this._vertices = new List<Vertex>(vertices);
            this._edges = new List<Edge>();
            if (vertices.Length == 0)
            {
                return;
            }

            for (int i = 0; i < this._vertices.Count; i++)
            {
                _edges.Add(Edge.PolygonalEdge(this._vertices[i], this._vertices[(i + 1) % this._vertices.Count]));
            }

            Vertex lowest = this._vertices[0];
            foreach (Vertex vertex in this._vertices)
            {
                if (vertex.GetY() < lowest.GetY() || (vertex.GetY() == lowest.GetY() && vertex.GetX() < lowest.GetX()))
                    lowest = vertex;
                vertex.SetColor(colors[0]);
            }

            _clockwise = Left(lowest.GetCoordsArr(), lowest.GetPrev().GetCoordsArr(), lowest.GetNext().GetCoordsArr());
        }

        public SimplePolygon(List<Vertex> vertices)
        {
            this._vertices = new List<Vertex>(vertices);
            this._edges = new List<Edge>();
            for (int i = 0; i < this._vertices.Count; i++)
            {
                _edges.Add(Edge.PolygonalEdge(this._vertices[i], this._vertices[(i + 1) % this._vertices.Count]));
            }

            Vertex lowest = this._vertices[0];
            foreach (Vertex vertex in this._vertices)
            {
                if (vertex.GetX() < lowest.GetX())
                    lowest = vertex;
                vertex.SetColor(colors[0]);
            }

            _clockwise = Left(lowest.GetCoordsArr(), lowest.GetPrev().GetCoordsArr(), lowest.GetNext().GetCoordsArr());
        }


        public void Paint(Graphics g)
        {
            int[,] gca = GetCoordsArray();
            List<Point> listPoint = new List<Point>();
            int i;
            for (i = 0; i < gca.GetLength(1); i++)
            {
                listPoint.Add(new Point(gca[0, i], gca[1, i]));
            }


            if (listPoint.Count > 1)
            {
                g.FillPolygon(new SolidBrush(Color.Aqua), listPoint.ToArray());
                g.DrawPolygon(new Pen(new SolidBrush(Color.Blue), 4), listPoint.ToArray());
            }

            i = 0;
            foreach (Vertex vertex in _vertices)
            {
                g.FillEllipse(new SolidBrush(vertex.GetColor()), vertex.GetX() - 5, vertex.GetY() - 5, 10, 10);
                g.DrawString("" + i++, new Font("Arial", 6), new SolidBrush(vertex.GetColor()),
                    new PointF(vertex.GetX() + 5, vertex.GetY() + 5));
            }
        }

        public int[,] GetCoordsArray()
        {
            if (_coordsArray != null && _coordsArray.Length == _vertices.Count)
                return _coordsArray;
            _coordsArray = new int[2, _vertices.Count];
            int i = 0;
            foreach (Vertex vertex in _vertices)
            {
                _coordsArray[0, i] = vertex.GetX();
                _coordsArray[1, i++] = vertex.GetY();
            }

            return _coordsArray;
        }

        public Edge ClipEar(Vertex v0)
        {
            if (!Diagonal(v0.GetPrev(), v0.GetNext()))
            {
                return null;
            }

            return Edge.PolygonalEdge(v0.GetPrev(), v0.GetNext());
        }

        public List<Edge> Triangulate()
        {
            if (_vertices.Count <= 2)
                return new List<Edge>();
            if (_clockwise)
            {
                foreach (Edge edge in this._edges)
                {
                    edge.Invert();
                }

                foreach (Vertex vertex in _vertices)
                {
                    vertex.Invert();
                }

                _vertices.Reverse();
                this._edges.Reverse();
                _clockwise = false;
            }

            List<Edge> edges = new List<Edge>();
            int i = 0;
            Vertex pointer = _vertices[0];
            while (pointer.GetNext() != pointer.GetPrev() && i < 200)
            {
                Edge edge = ClipEar(pointer);
                pointer = pointer.GetPrev();
                if (edge != null)
                {
                    edges.Add(edge);
                }

                i++;
            }

            if (this._edges.Count > 0)
                edges.RemoveAt(edges.Count - 1);
            for (i = 0; i < this._vertices.Count; i++)
            {
                Edge.OrderVertex(this._vertices[i], this._vertices[(i + 1) % this._vertices.Count]);
            }

            foreach (Edge edge in edges)
            {
                edge.Incorporate();
            }

            return edges;
        }

        public void Incorporate()
        {
            foreach (Edge edge in _edges)
            {
                edge.Incorporate();
            }
        }

        public bool Diagonal(Vertex v0, Vertex v1)
        {
            return InCone(v0, v1) && InCone(v1, v0) && Diagonalie(v0, v1);
        }

        public bool Diagonalie(Vertex v0, Vertex v1)
        {
            foreach (Edge edge in _edges)
            {
                if (edge.Contains(v0) || edge.Contains(v1))
                {
                    continue;
                }

                if (IntersectsProp(edge.GetStart().GetCoordsArr(), edge.GetEnd().GetCoordsArr(), v0.GetCoordsArr(),
                        v1.GetCoordsArr()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool InCone(Vertex v0, Vertex v1)
        {
            Vertex next = _vertices[(_vertices.IndexOf(v0) + 1) % _vertices.Count];
            Vertex prev = _vertices[(_vertices.IndexOf(v0) - 1 + _vertices.Count) % _vertices.Count];
            if (leftOn(v0.GetCoordsArr(), next.GetCoordsArr(), prev.GetCoordsArr()))
            {
                return Left(v0.GetCoordsArr(), v1.GetCoordsArr(), prev.GetCoordsArr()) &&
                       Left(v1.GetCoordsArr(), v0.GetCoordsArr(), next.GetCoordsArr());
            }

            return !(leftOn(v0.GetCoordsArr(), v1.GetCoordsArr(), next.GetCoordsArr()) &&
                     leftOn(v1.GetCoordsArr(), v0.GetCoordsArr(), prev.GetCoordsArr()));
        }

        public static bool IntersectsProp(int[] a, int[] b, int[] c, int[] d)
        {
            if (Collinear(a, b, c) && Collinear(a, b, d))
            {
                return (Between(a, b, c) || Between(a, b, d));
            }

            if ((Between(a, b, c) || Between(a, b, d) || Between(c, d, a) || Between(c, d, b)))
            {
                return true;
            }

            if (Collinear(a, b, c) || Collinear(a, b, d) || Collinear(c, d, a) || Collinear(c, d, b))
            {
                return false;
            }

            return (Left(a, b, c) != Left(a, b, d)) && (Left(c, d, a) != Left(c, d, b));
        }

        public static bool Between(int[] a, int[] b, int[] c)
        {
            if (!Collinear(a, b, c))
                return false;
            if (a[0] != b[0])
                return ((a[0] <= c[0]) && (c[0] <= b[0])) ||
                       ((a[0] >= c[0]) && (c[0] >= b[0]));
            return ((a[1] <= c[1]) && (c[1] <= b[1])) ||
                   ((a[1] >= c[1]) && (c[1] >= b[1]));
        }

        public static bool Left(int[] a, int[] b, int[] c)
        {
            return CrossProduct(a, b, a, c) > 0;
        }

        public static bool leftOn(int[] a, int[] b, int[] c)
        {
            return CrossProduct(a, b, a, c) >= 0;
        }

        public static bool Collinear(int[] a, int[] b, int[] c)
        {
            return CrossProduct(a, b, a, c) == 0;
        }

        public static int CrossProduct(int[] a, int[] b, int[] c, int[] d)
        {
            return (b[0] - a[0]) * (d[1] - c[1]) - (b[1] - a[1]) * (d[0] - c[0]);
        }
    }

    class Vertex
    {
        private int _x, _y;
        private Vertex _next, _prev;
        private Color _color;
        private List<Edge> _edges;

        public Vertex(int x, int y)
        {
            this._x = x;
            this._y = y;
            _color = Color.Gray;
            _edges = new List<Edge>();
        }


        public int[] GetCoordsArr()
        {
            return new int[] { _x, _y };
        }

        public int GetX()
        {
            return _x;
        }

        public int GetY()
        {
            return _y;
        }

        public Vertex GetNext()
        {
            return _next;
        }

        public Vertex GetPrev()
        {
            return _prev;
        }

        public Color GetColor()
        {
            return _color;
        }

        public void SetColor(Color color)
        {
            this._color = color;
        }

        public void SetNext(Vertex next)
        {
            this._next = next;
        }

        public void SetPrev(Vertex prev)
        {
            this._prev = prev;
        }

        public void Invert()
        {
            (this._prev, this._next) = (this._next, this._prev);
        }

        public void AddEdge(Edge edge)
        {
            if (_edges.Contains(edge))
                return;
            _edges.Add(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            _edges.Remove(edge);
        }

        public List<Edge> GetEdges()
        {
            return _edges;
        }

        public double SinAngle(Edge firstEdge, Edge secondEdge)
        {
            int crossProduct = SimplePolygon.CrossProduct(this.GetCoordsArr(), firstEdge.GetOther(this).GetCoordsArr(),
                this.GetCoordsArr(), secondEdge.GetOther(this).GetCoordsArr());
            return crossProduct / firstEdge.Length() / secondEdge.Length();
        }

        public double SinAngle(Vertex firstVertex, Vertex secondVertex)
        {
            int crossProduct = SimplePolygon.CrossProduct(this.GetCoordsArr(), firstVertex.GetCoordsArr(),
                this.GetCoordsArr(), secondVertex.GetCoordsArr());
            return crossProduct / Distance(firstVertex) / Distance(secondVertex);
        }

        public double Distance(Vertex other)
        {
            return Math.Sqrt(Math.Pow(this._x - other._x, 2) + Math.Pow(this._y - other._y, 2));
        }
    }


    class Edge
    {
        private Vertex _start, _end;
        private readonly int[] _center;

        public Edge(Vertex start, Vertex end)
        {
            this._start = start;
            this._end = end;
            _center = new int[] { (start.GetX() + end.GetX()) / 2, (start.GetY() + end.GetY()) / 2 };
        }

        public static Edge PolygonalEdge(Vertex start, Vertex end)
        {
            Edge edge = new Edge(start, end);
            start.SetNext(end);
            end.SetPrev(start);
            return edge;
        }

        public static void OrderVertex(Vertex start, Vertex end)
        {
            start.SetNext(end);
            end.SetPrev(start);
        }

        public bool Contains(Vertex vertex)
        {
            return _start.Equals(vertex) || _end.Equals(vertex);
        }

        public Vertex GetEnd()
        {
            return _end;
        }

        public Vertex GetStart()
        {
            return _start;
        }

        public void Invert()
        {
            (_start, _end) = (_end, _start);
        }


        public void Paint(Graphics g, int i)
        {
            g.DrawString("" + i, new Font("Arial", 5), new SolidBrush(Color.Black),
                new PointF(_center[0] + 1, _center[1]));
            g.DrawLine(new Pen(Color.Black, 4), _start.GetX(), _start.GetY(), _end.GetX(), _end.GetY());
        }

        public int[] GetCenter()
        {
            return _center;
        }

        public Vertex GetOther(Vertex vertex)
        {
            if (vertex.Equals(_start))
            {
                return _end;
            }

            return _start;
        }

        public double Length()
        {
            return Math.Sqrt(
                Math.Pow(_start.GetX() - _end.GetX(), 2) + Math.Pow(_start.GetY() - _end.GetY(), 2)
            );
        }

        public void Incorporate()
        {
            _start.AddEdge(this);
            _end.AddEdge(this);
        }
    }


    //Additional classes #end
}