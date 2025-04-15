using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphTools;
namespace GraphTools
{
    public static class GraphUtils
    {
        /// <summary>
        /// Выделяет рамку данного графа данным цветом
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="color">Цвет выделения графа</param>
        public static void HighlightGraph(Graph graph,Brush color)
        { 
           var highlight = from Border item in graph.Canvas.Children
                           select item;
            Border border = highlight.FirstOrDefault();
            border.BorderBrush = color;
            if (color == Brushes.Black)
            {
                border.BorderThickness = new Thickness(1);
            }
            else
            {
                border.BorderThickness = new Thickness(3);
            }
        }
        /// <summary>
        /// Убирает выделение рамок всех графов
        /// </summary>
        /// <param name="collection">Коллекция в которой содержатся графы</param>
        public static void ClearSelection(UIElementCollection collection)
        {
            var clearSelectionGraph = from Canvas graph in collection
                                      from UIElement border in graph.Children
                                      where border is Border
                                      select border;
            foreach (Border border in clearSelectionGraph.Cast<Border>())
            {
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
            }
        }
        /// <summary>
        /// Сложение ребер и вершин первого графа со вторым<br></br>
        ///Ребра первого и второго графа складываются<br></br>
        ///Получается третий граф с ребрами как первого так и второго графа<br></br>
        /// </summary>
        /// <param name="graph1">Первый граф</param>
        /// <param name="graph2">Второй граф</param>
        /// <returns>Возвращает суммирующий граф</returns>
        public static Graph Addiction(Graph graph1, Graph graph2)
        {
            List<List<int>> resultEdges = new List<List<int>>();
            if (graph1.Edges.Count > graph2.Edges.Count)
            {
                for (int i = 0; i != graph1.Edges.Count; i++)
                {
                    resultEdges.Add(new List<int>());
                    for (int j = 0; j != graph1.Edges[i].Count; j++)
                    {
                        resultEdges[i].Add(graph1.Edges[i][j]);
                    }
                }
                for (int i = 0; i != graph2.Edges.Count; i++)
                {
                    for (int j = 0; j != graph2.Edges[i].Count; j++)
                    {
                        if (!resultEdges[i].Contains(graph2.Edges[i][j]))
                            resultEdges[i].Add(graph2.Edges[i][j]);
                    }
                }
            }
            else
            {
                for (int i = 0; i != graph2.Edges.Count; i++)
                {
                    resultEdges.Add(new List<int>());
                    for (int j = 0; j != graph2.Edges[i].Count; j++)
                    {
                        resultEdges[i].Add(graph2.Edges[i][j]);
                    }
                }
                for (int i = 0; i != graph1.Edges.Count; i++)
                {
                    for (int j = 0; j != graph1.Edges[i].Count; j++)
                    {
                        if (!resultEdges[i].Contains(graph1.Edges[i][j]))
                            resultEdges[i].Add(graph1.Edges[i][j]);
                    }
                }
            }

            return new Graph(resultEdges);

        }
        /// <summary>
        ///Вычитание ребер первого графа из второго<br></br>
        ///Ребра второго графа вычитаются из ребер первого<br></br>
        ///В результате если и у текущего и у выбранного графа есть такое ребро<br></br>
        ///То оно удаляется, а если у уменьшаемого такого ребра нет<br></br>
        ///Появляется обратное ребро, т.е с другим направлением<br></br>
        /// </summary>
        /// <param name="graph1">Уменьшаемый граф</param>
        /// <param name="graph2">Вычитаемый граф</param>
        /// <returns>Возвращает Разностный граф</returns>
        public static Graph Substraction(Graph graph1, Graph graph2)
        {
            List<List<int>> ResultEgdes = graph1.Edges.Select(edge => edge.ToList()).ToList();
            for (int i = 0;i!= ResultEgdes.Count;i++)
            {
                if (i == graph2.Edges.Count)
                    break;
                for (int j = 0; j != graph2.Edges[i].Count;j++)
                {
                    if (graph2.Edges[i][j] >= ResultEgdes.Count)
                        break;
                    if (ResultEgdes[i].Contains(graph2.Edges[i][j]))
                    {
                        ResultEgdes[i].Remove(graph2.Edges[i][j]);

                    }
                    else
                    {
                        ResultEgdes[i].Remove(graph2.Edges[i][j]);
                        ResultEgdes[graph2.Edges[i][j]].Add(i);
                    }
                }
            }
            return new Graph(ResultEgdes);
        }
        /// <summary>
        /// Запускает поиск в глубину из начальной вершины 
        /// </summary>
        /// <param name="newGraph">Граф для поиска в глубину</param>
        /// <param name="StartNode">Начальная вершина</param>
        /// <returns>Возвращает дерево построенное из начальной вершины</returns>
        public static Graph DeepFirstSearch(Graph graph,int StartNode)
        {
            Graph newGraph = new Graph(graph,graph.Radius);
            bool[] Visited = new bool[newGraph.Nodes];
            int[] Fathers = Enumerable.Repeat(-1, newGraph.Nodes).ToArray();
            DFS(StartNode, newGraph.Edges, Visited, Fathers);
            List<List<int>> paths = new List<List<int>>();
            for (int i = 0;i!=newGraph.Nodes;i++)
            {
                List<int> path = new List<int>();
                if (i == StartNode)
                {
                    paths.Add(path);
                    continue;
                }    
                path = FindPath(Fathers, StartNode, i);
                if (path == null)
                    continue;
                HighLightPath(newGraph, path);
            }
            var pattern = @"..";
            var regex = new Regex(pattern);
            newGraph.Canvas.Children
                .OfType<Shape>()
                .Where(item => regex.IsMatch(item.Uid))
                .Where(item => (item.Stroke as SolidColorBrush)?.Color == Colors.Black) 
                .ToList()
                .ForEach(item => 
                { 
                    newGraph.Canvas.Children.Remove(item);
                    string temp = item.Uid;
                    int[] digits = item.Uid.Select(c => int.Parse(c.ToString())).ToArray();
                    newGraph.Edges[digits[0]].Remove(digits[1]);
                });
            return newGraph;

        }
        private static void DFS(int s, List<List<int>>Edges, bool[] Visited, int[] Fathers)
        {
            for (int i = 0; i < Edges[s].Count; i++)
            {
                if (!Visited[Edges[s][i]])
                {
                    Visited[Edges[s][i]] = true;
                    Fathers[Edges[s][i]] = s;
                    DFS(Edges[s][i], Edges, Visited, Fathers);
                }
            }
        }
        private static List<int> FindPath(int[] fathers, int start, int end)
        {
            List<int> path = new List<int>();
            int current = end;
            while (current != start)
            {
                if (current == -1)
                    return null;
                path.Add(current);
                current = fathers[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
        private static void HighLightPath(Graph graph,List<int> path)
        {
            for (int i = 0; i != path.Count - 1; i++)
            {

                var arrowsFind = (from UIElement item in graph.Canvas.Children
                                  where item.Uid == path[i].ToString() + path[i + 1].ToString() 
                                  select item).ToList();

                foreach (Shape arrow in arrowsFind)
                    arrow.Stroke = Brushes.Red;
            }
        }

    


    }
}
