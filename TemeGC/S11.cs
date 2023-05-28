namespace TemeGC
{
    internal class S11
    {
        public static GraphPanel gpanel;

        public enum PhaseType
        {
            DRAW,
            TRIANGLE,
            FINAL
        }

        public static PictureBox pb = null!;
        public static PhaseType phase = PhaseType.DRAW;
        public static Graphics _g = null!;

        public static PictureBox P1(PictureBox pbi)
        {
            pb = pbi;

            pb.Size = new Size(Form1.width, Form1.height);
            _g = pb.CreateGraphics();
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
                case S11.PhaseType.DRAW:
                    str += "Left Click to Add Vertices, Right Click to Remove!";
                    break;
                case S11.PhaseType.TRIANGLE:
                    str += "Now we've triangulated it, click CONTINUE to see the edges we can remove";
                    break;
                case S11.PhaseType.FINAL:
                    str += "We've removed as many diagonals as we could! We have an 4-approximation!";
                    break;
            }

            _g.DrawString(str, new Font("Arial", 6), new SolidBrush(Color.Red), new PointF(30, Form1.height - 30));
            _g.FillRectangle(new SolidBrush(Color.Black), 700, Form1.height - 50, 130, 50);
            _g.DrawString("CONTINUE", new Font("Arial", 6), new SolidBrush(Color.Red),
                new PointF(730, Form1.height - 30));

            _g.FillRectangle(new SolidBrush(Color.Purple), 850, Form1.height - 50, 130, 50);
            _g.DrawString("CLEAR", new Font("Arial", 6), new SolidBrush(Color.Red), new PointF(880, Form1.height - 30));
            //Lower Panel #end
        }


        //Additional functions #start
        public static void phaseAdd()
        {
            phase = (phase.Equals(PhaseType.DRAW)) ? PhaseType.TRIANGLE :
                (phase.Equals(PhaseType.TRIANGLE)) ? PhaseType.FINAL : PhaseType.DRAW;

            if (phase == PhaseType.DRAW)
            {
                GraphPanel.polygon = new SimplePolygon(GraphPanel.vertices);
                GraphPanel.edges = new List<Edge>();
            }

            if (phase == PhaseType.TRIANGLE)
            {
                GraphPanel.edges = GraphPanel.polygon.triangulate();
                GraphPanel.polygon.incorporate();
            }

            if (phase == PhaseType.FINAL)
            {
                Console.WriteLine("General");
                GraphPanel.decompose();
                Console.WriteLine("Kenobi");
            }
            
            GraphPanel.paintComponent();
        }

        public static void phaseClear()
        {
            GraphPanel.polygon = new SimplePolygon(new Vertex[] { });
            GraphPanel.vertices = new List<Vertex>();
            GraphPanel.edges = new List<Edge>();
            phase = PhaseType.DRAW;
        }
        //Additional functions #end


        //Drawing functions #start
        private static void DrawPoint(PointF pF, int number)
        {
            String numberToString = (number < 10) ? (" " + number) : (number + "");

            _g.FillEllipse(new SolidBrush(Color.Blue), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawEllipse(new Pen(new SolidBrush(Color.Red), 2), pF.X - 8, pF.Y - 8, 16, 16);
            _g.DrawString(numberToString, new Font("Arial", 6), new SolidBrush(Color.White),
                new PointF(pF.X - 8, pF.Y - 6));
        }

        //Drawing functions #end
    }

    //Additional Classes #start
    class InfoPanel
    {
        public InfoPanel(PictureBox pictureBox)
        {
            pictureBox.MouseClick += InfoPanelMC;
        }


        public static void InfoPanelMC(object? sender, MouseEventArgs e)
        {
            if (e.X > 700 && e.X < 830 && e.Y > Form1.height - 50 && e.Y < Form1.height)
                S11.phaseAdd();
            if (e.X > 850 && e.X < 980 && e.Y > Form1.height - 50 && e.Y < Form1.height)
                S11.phaseClear();
        }
    } //Working Well

    class GraphPanel
    {
        public static SimplePolygon polygon = null!;
        public static List<Edge> edges = null!;
        public static List<Vertex> vertices = null!;


        public GraphPanel(PictureBox pb)
        {
            pb.MouseClick += GraphPanelMC;

            polygon = new SimplePolygon(new Vertex[] { });
            vertices = new List<Vertex>();
            edges = new List<Edge>();
        }

        public static void paintComponent()
        {
            S11.PaintEverything();

            polygon.paint(S11._g);
            if (S11.phase != S11.PhaseType.DRAW)
            {
                int i = 0;
                foreach (Edge edge in edges)
                {
                    edge.paint(S11._g, i++);
                }
            }
        }

        public static void decompose()
        {
            Console.WriteLine("hello");
            foreach (Vertex vertex in vertices)
            {
                vertex.getEdges().Sort((o1, o2) =>
                {
                    if ((o1.getCenter()[1] - vertex.getY()) * (o2.getCenter()[1] - vertex.getY()) < 0)
                        return o1.getCenter()[1] - o2.getCenter()[1];
                    int crossProduct = SimplePolygon.crossProduct(vertex.getCoordsArr(), o1.getCenter(),
                        vertex.getCoordsArr(), o2.getCenter());
                    //return Integer.compare(crossProduct, 0);
                    return crossProduct == 0 ? 0 : crossProduct < 0 ? -1 : 1;
                });
            }

            for (int i = 0; i < edges.Count && edges.Count != 0; i++)
            {
                Edge edge = edges[i];
                if (vertexClearance(edge, edge.getEnd()) && vertexClearance(edge, edge.getStart()))
                {
                    removeEdge(edge);
                    i--;
                }
            }

            Console.WriteLine("there");
        }

        public static bool vertexClearance(Edge edge, Vertex vertex)
        {
            List<Edge> edges = vertex.getEdges();
            if (edges.Count == 3)
            {
                return vertex.sinAngle(vertex.getPrev(), vertex.getNext()) <= 0;
            }

            Edge prev = edges[(edges.IndexOf(edge) - 1 + edges.Count) % edges.Count];
            Edge next = edges[(edges.IndexOf(edge) + 1) % edges.Count];
            return vertex.sinAngle(prev, next) <= 0;
        }

        public static void removeEdge(Edge edge)
        {
            edge.getStart().removeEdge(edge);
            edge.getEnd().removeEdge(edge);
            edges.Remove(edge);
        }


        private static void GraphPanelMC(object? sender, MouseEventArgs e)
        {
            if (e.Y < Form1.height - 100)
                if (S11.phase == S11.PhaseType.DRAW)
                {
                    vertices.Add(new Vertex(e.X, e.Y));
                    polygon = new SimplePolygon(vertices);

                    paintComponent();

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
        List<Vertex> vertices;
        List<Edge> edges = null!;
        bool clockwise;
        int[,] coordsArray;

        public static Color[] colors = { Color.Gray, Color.Crimson, Color.LawnGreen, Color.DodgerBlue };

        public SimplePolygon(Vertex[] vertices)
        {
            this.vertices = new List<Vertex>(vertices);
            this.edges = new List<Edge>();
            if (vertices.Length == 0)
            {
                return;
            }

            for (int i = 0; i < this.vertices.Count; i++)
            {
                edges.Add(Edge.polygonalEdge(this.vertices[i], this.vertices[(i + 1) % this.vertices.Count]));
            }

            Vertex lowest = this.vertices[0];
            foreach (Vertex vertex in this.vertices)
            {
                if (vertex.getY() < lowest.getY() || (vertex.getY() == lowest.getY() && vertex.getX() < lowest.getX()))
                    lowest = vertex;
                vertex.setColor(colors[0]);
            }

            clockwise = left(lowest.getCoordsArr(), lowest.getPrev().getCoordsArr(), lowest.getNext().getCoordsArr());
        }

        public SimplePolygon(List<Vertex> vertices)
        {
            this.vertices = new List<Vertex>(vertices);
            this.edges = new List<Edge>();
            for (int i = 0; i < this.vertices.Count; i++)
            {
                edges.Add(Edge.polygonalEdge(this.vertices[i], this.vertices[(i + 1) % this.vertices.Count]));
            }

            Vertex lowest = this.vertices[0];
            foreach (Vertex vertex in this.vertices)
            {
                if (vertex.getX() < lowest.getX())
                    lowest = vertex;
                vertex.setColor(colors[0]);
            }

            clockwise = left(lowest.getCoordsArr(), lowest.getPrev().getCoordsArr(), lowest.getNext().getCoordsArr());
        }


        public void paint(Graphics _g)
        {
            int[,] gca = getCoordsArray();
            List<Point> listPoint = new List<Point>();
            int i;
            for (i = 0; i < gca.GetLength(1); i++)
            {
                listPoint.Add(new Point(gca[0, i], gca[1, i]));
            }


            if (listPoint.Count > 1)
            {
                _g.FillPolygon(new SolidBrush(Color.Aqua), listPoint.ToArray());
                _g.DrawPolygon(new Pen(new SolidBrush(Color.Blue), 4), listPoint.ToArray());
            }

            i = 0;
            foreach (Vertex vertex in vertices)
            {
                _g.FillEllipse(new SolidBrush(vertex.getColor()), vertex.getX() - 5, vertex.getY() - 5, 10, 10);
                _g.DrawString("" + i++, new Font("Arial", 6), new SolidBrush(vertex.getColor()),
                    new PointF(vertex.getX() + 5, vertex.getY() + 5));
            }
        }

        public int[,] getCoordsArray()
        {
            if (coordsArray != null && coordsArray.Length == vertices.Count)
                return coordsArray;
            coordsArray = new int[2, vertices.Count];
            int i = 0;
            foreach (Vertex vertex in vertices)
            {
                coordsArray[0, i] = vertex.getX();
                coordsArray[1, i++] = vertex.getY();
            }

            return coordsArray;
        }

        public Edge clipEar(Vertex v0)
        {
            if (!diagonal(v0.getPrev(), v0.getNext()))
            {
                return null;
            }

            return Edge.polygonalEdge(v0.getPrev(), v0.getNext());
        }

        public List<Edge> triangulate()
        {
            if (vertices.Count <= 2)
                return new List<Edge>();
            if (clockwise)
            {
                foreach (Edge edge in this.edges)
                {
                    edge.invert();
                }

                foreach (Vertex vertex in vertices)
                {
                    vertex.invert();
                }

                vertices.Reverse();
                this.edges.Reverse();
                clockwise = false;
            }

            List<Edge> edges = new List<Edge>();
            int i = 0;
            Vertex pointer = vertices[0];
            while (pointer.getNext() != pointer.getPrev() && i < 200)
            {
                Edge edge = clipEar(pointer);
                pointer = pointer.getPrev();
                if (edge != null)
                {
                    edges.Add(edge);
                }

                i++;
            }

            if (this.edges.Count > 0)
                edges.RemoveAt(edges.Count - 1);
            for (i = 0; i < this.vertices.Count; i++)
            {
                Edge.orderVertex(this.vertices[i], this.vertices[(i + 1) % this.vertices.Count]);
            }

            foreach (Edge edge in edges)
            {
                edge.incorporate();
            }

            return edges;
        }

        public void incorporate()
        {
            foreach (Edge edge in edges)
            {
                edge.incorporate();
            }
        }

        public bool diagonal(Vertex v0, Vertex v1)
        {
            return inCone(v0, v1) && inCone(v1, v0) && diagonalie(v0, v1);
        }

        public bool diagonalie(Vertex v0, Vertex v1)
        {
            foreach (Edge edge in edges)
            {
                if (edge.contains(v0) || edge.contains(v1))
                {
                    continue;
                }

                if (intersectsProp(edge.getStart().getCoordsArr(), edge.getEnd().getCoordsArr(), v0.getCoordsArr(),
                        v1.getCoordsArr()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool inCone(Vertex v0, Vertex v1)
        {
            Vertex next = vertices[(vertices.IndexOf(v0) + 1) % vertices.Count];
            Vertex prev = vertices[(vertices.IndexOf(v0) - 1 + vertices.Count) % vertices.Count];
            if (leftOn(v0.getCoordsArr(), next.getCoordsArr(), prev.getCoordsArr()))
            {
                return left(v0.getCoordsArr(), v1.getCoordsArr(), prev.getCoordsArr()) &&
                       left(v1.getCoordsArr(), v0.getCoordsArr(), next.getCoordsArr());
            }

            return !(leftOn(v0.getCoordsArr(), v1.getCoordsArr(), next.getCoordsArr()) &&
                     leftOn(v1.getCoordsArr(), v0.getCoordsArr(), prev.getCoordsArr()));
        }

        public static bool intersectsProp(int[] a, int[] b, int[] c, int[] d)
        {
            if (collinear(a, b, c) && collinear(a, b, d))
            {
                return (between(a, b, c) || between(a, b, d));
            }

            if ((between(a, b, c) || between(a, b, d) || between(c, d, a) || between(c, d, b)))
            {
                return true;
            }

            if (collinear(a, b, c) || collinear(a, b, d) || collinear(c, d, a) || collinear(c, d, b))
            {
                return false;
            }

            return (left(a, b, c) != left(a, b, d)) && (left(c, d, a) != left(c, d, b));
        }

        public static bool between(int[] a, int[] b, int[] c)
        {
            if (!collinear(a, b, c))
                return false;
            if (a[0] != b[0])
                return ((a[0] <= c[0]) && (c[0] <= b[0])) ||
                       ((a[0] >= c[0]) && (c[0] >= b[0]));
            return ((a[1] <= c[1]) && (c[1] <= b[1])) ||
                   ((a[1] >= c[1]) && (c[1] >= b[1]));
        }

        public static bool left(int[] a, int[] b, int[] c)
        {
            return crossProduct(a, b, a, c) > 0;
        }

        public static bool leftOn(int[] a, int[] b, int[] c)
        {
            return crossProduct(a, b, a, c) >= 0;
        }

        public static bool collinear(int[] a, int[] b, int[] c)
        {
            return crossProduct(a, b, a, c) == 0;
        }

        public static int crossProduct(int[] a, int[] b, int[] c, int[] d)
        {
            return (b[0] - a[0]) * (d[1] - c[1]) - (b[1] - a[1]) * (d[0] - c[0]);
        }
    }

    class Vertex
    {
        private int x, y;
        private Vertex next, prev;
        private Color color;
        private List<Edge> edges;

        public Vertex(int x, int y)
        {
            this.x = x;
            this.y = y;
            color = Color.Gray;
            edges = new List<Edge>();
        }


        public int[] getCoordsArr()
        {
            return new int[] { x, y };
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public Vertex getNext()
        {
            return next;
        }

        public Vertex getPrev()
        {
            return prev;
        }

        public Color getColor()
        {
            return color;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        public void setNext(Vertex next)
        {
            this.next = next;
        }

        public void setPrev(Vertex prev)
        {
            this.prev = prev;
        }

        public void invert()
        {
            (this.prev, this.next) = (this.next, this.prev);
        }

        public void addEdge(Edge edge)
        {
            if (edges.Contains(edge))
                return;
            edges.Add(edge);
        }

        public void removeEdge(Edge edge)
        {
            edges.Remove(edge);
        }

        public List<Edge> getEdges()
        {
            return edges;
        }

        public double sinAngle(Edge firstEdge, Edge secondEdge)
        {
            int crossProduct = SimplePolygon.crossProduct(this.getCoordsArr(), firstEdge.getOther(this).getCoordsArr(),
                this.getCoordsArr(), secondEdge.getOther(this).getCoordsArr());
            return crossProduct / firstEdge.length() / secondEdge.length();
        }

        public double sinAngle(Vertex firstVertex, Vertex secondVertex)
        {
            int crossProduct = SimplePolygon.crossProduct(this.getCoordsArr(), firstVertex.getCoordsArr(),
                this.getCoordsArr(), secondVertex.getCoordsArr());
            return crossProduct / distance(firstVertex) / distance(secondVertex);
        }

        public double distance(Vertex other)
        {
            return Math.Sqrt(Math.Pow(this.x - other.x, 2) + Math.Pow(this.y - other.y, 2));
        }
    }


    class Edge
    {
        private Vertex start, end;
        private int[] center;

        public Edge(Vertex start, Vertex end)
        {
            this.start = start;
            this.end = end;
            center = new int[] { (start.getX() + end.getX()) / 2, (start.getY() + end.getY()) / 2 };
        }

        public static Edge polygonalEdge(Vertex start, Vertex end)
        {
            Edge edge = new Edge(start, end);
            start.setNext(end);
            end.setPrev(start);
            return edge;
        }

        public static void orderVertex(Vertex start, Vertex end)
        {
            start.setNext(end);
            end.setPrev(start);
        }

        public bool contains(Vertex vertex)
        {
            return start.Equals(vertex) || end.Equals(vertex);
        }

        public Vertex getEnd()
        {
            return end;
        }

        public Vertex getStart()
        {
            return start;
        }

        public void invert()
        {
            (start, end) = (end, start);
        }


        public void paint(Graphics g, int i)
        {
            g.DrawString("" + i, new Font("Arial", 5), new SolidBrush(Color.Black),
                new PointF(center[0] + 1, center[1]));
            g.DrawLine(new Pen(Color.Black, 4), start.getX(), start.getY(), end.getX(), end.getY());
        }

        public int[] getCenter()
        {
            return center;
        }

        public Vertex getOther(Vertex vertex)
        {
            if (vertex.Equals(start))
            {
                return end;
            }

            return start;
        }

        public double length()
        {
            return Math.Sqrt(
                Math.Pow(start.getX() - end.getX(), 2) + Math.Pow(start.getY() - end.getY(), 2)
            );
        }

        public void incorporate()
        {
            start.addEdge(this);
            end.addEdge(this);
        }
    }


    //Additional classes #end
}