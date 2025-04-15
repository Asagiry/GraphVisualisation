using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphTools
{
    /// <summary>
    /// Class <c>Graph</c> создает на холсте граф с заданными ребрами и вершинами
    /// </summary>
    public class Graph 
    {
        private int _Nodes;
        private List<List<int>> _Edges;
        private Canvas _Canvas;
        private int _Radius;
        private Point[] _NodesXY;

        /// <summary>
        /// Поле представляющее координаты вершин на холсте
        /// </summary>
        public Point[] NodesXY
        {
            get => _NodesXY;

            private set => _NodesXY = value;
        }
        /// <summary>
        /// Свойство представляющее количество вершин <br></br>
        /// </summary>
        public int Nodes
        {
            get => _Nodes;
            private set
            { 
                _Nodes = value;
                DrawNodes();
            }
        }
        /// <summary>
        /// Свойство представляющее список смежности вершин <br></br>
        /// </summary>
        public List<List<int>> Edges
        {
            get => _Edges;

            private set
            {
                if (value.Count > 11)
                    throw new ArgumentException("Максимум 10 вершин");
                _Edges = value;
                DrawEdges();

            }
        }
        /// <summary>
        /// Свойство представляющее холст, на котором отрисован граф
        /// </summary>
        public Canvas Canvas
        {
            get => _Canvas; private set => _Canvas = value;
        }
        /// <summary>
        /// Свойство представляющее радиус отрисовки графа
        /// </summary>
        public int Radius
        {
            get => _Radius;
            private set
            {
                if (value < 75)
                    throw new ArgumentException("Слишком маленький радиус");
                _Radius = value;
            }
        }

        /// <summary>
        /// Отрисовка вершин на холсте
        /// </summary>
        private void DrawNodes()
        {
            double angle = (180 - (Nodes - 2) * 180 / Nodes) * Math.PI / 180;

            Point center = new Point(Radius, Radius);

            NodesXY = new Point[Nodes];

            double x;
            double y;

            for (int i = 0; i < Nodes; i++)
            {
                x = (Radius - 30) * Math.Cos(90 * Math.PI / 180 - angle * i);
                y = (Radius - 30) * Math.Sin(90 * Math.PI / 180 + angle * i);
                Point circle = new Point(center.X + x, center.Y - y);
                Ellipse node = new Ellipse()
                {
                    Width = 5,
                    Height = 5,
                    Stroke = Brushes.Black,
                    Margin = new Thickness(circle.X, circle.Y, 0, 0),
                };

                double namex = (Radius-15) * Math.Cos(90 * Math.PI / 180 - angle * i);
                double namey = (Radius-15) * Math.Sin(90 * Math.PI / 180 + angle * i);
                Point namePoint = new Point(center.X + namex, center.Y - namey);
                Label name = new Label
                {
                    Width = 40,
                    Height = 30,
                    FontSize = 15,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = (new Thickness(namePoint.X - 17.5, namePoint.Y - 10, 0, 0)),
                    Content = string.Format("{0}", i)
                };
                NodesXY[i] = circle;
                Canvas.Children.Add(name);
                Canvas.Children.Add(node);
            }


        }
        /// <summary>
        /// Отрисовка ребер на холсте
        /// </summary>
        private void DrawEdges()
        {
            for (int i = 0; i != Nodes; i++)
            {
                for (int j = 0; j != Edges[i].Count; j++)
                {
                    Point startPoint = new Point(NodesXY[i].X, NodesXY[i].Y);
                    Point endPoint = new Point(NodesXY[Edges[i][j]].X, NodesXY[Edges[i][j]].Y);
                    new ArrowLine(startPoint, endPoint, Canvas, i, Edges[i][j]);
                }

            }
        }

        /// <summary>
        /// Конструктор класса Graph<b></b>
        /// Создает экземпляр класса Graph, представляющий граф на холсте
        /// </summary>
        /// <param name="Edges">Список смежности вершин графа
        /// </param>
        /// <param name="Radius">Радиус отрисовки графа (по умолчанию 150)
        /// </param>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если радиус меньше 75 
        /// </exception>
        public Graph(List<List<int>> Edges, int Radius = 100)
        {
            this.Radius = Radius;

            this.Canvas = new Canvas
            {
                Height = Radius * 2,
                Width = Radius * 2,

            };
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                Height = Radius * 2,
                Width = Radius * 2,
                BorderBrush = Brushes.Black,
            };
            Canvas.Children.Add(border);

            this.Nodes = Edges.Count;
            this.Edges = Edges;
          
        }

        /// <summary>
        /// Конструктор копирования класса Graph
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="Radius"></param>
        public Graph(Graph graph, int Radius = 100)
        {
            this.Radius = Radius;
            this.Canvas = new Canvas
            {
                Height = Radius * 2,
                Width = Radius * 2,

            };
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                Height = Radius * 2,
                Width = Radius * 2,
                BorderBrush = Brushes.Black,
            };
            Canvas.Children.Add(border);

            this.Nodes = graph.Edges.Count;
            this.Edges = graph.Edges.Select(edge => edge.ToList()).ToList();

        }

        /// <summary>
        /// Индексатор для получения ребер конкретной вершины<br></br>
        /// Принимает на вход список вершин, к которым есть путь
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<int> this[int index]
        {
            get
            {
                if (_Edges == null)
                    throw new InvalidOperationException("Граф пустой");
                if (index < 0 || index >= _Edges.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне допустимого диапазона");

                return _Edges[index];
            }
        }
       

        public static implicit operator UIElement(Graph graph) => graph.Canvas;

    }
}
