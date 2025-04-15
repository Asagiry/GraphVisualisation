using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using GraphTools;

namespace GraphVisualisation
{
    public class EditableGraph : Graph
    {
        /// <summary>
        /// Событие для удаления графа
        /// </summary>
        public event EventHandler RemoveGraph;
        /// <summary>
        /// Событие для выбора графа
        /// </summary>
        public event EventHandler SelectGraph;
        /// <summary>
        /// Событие чтобы выбрать граф текущим
        /// </summary>
        public event EventHandler CurrentGraph;

        public EditableGraph(List<List<int>> Edges, int radius = 100) : base(Edges, radius)
        {
            Initialize();
        }

        public EditableGraph(Graph graph, int radius = 100) : base(graph, radius)
        {
            Initialize();
        }

        private void Initialize()
        {
            Label background = new Label
            {
                Opacity = 0,
                Height = Canvas.Height,
                Width = Canvas.Width
            };
            Canvas.Children.Add(background);
            background.MouseRightButtonDown += (sender, e) => RemoveGraph?.Invoke(this, EventArgs.Empty);
            background.MouseLeftButtonDown += (sender, e) => SelectGraph?.Invoke(this, EventArgs.Empty);
            background.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount > 1) CurrentGraph?.Invoke(this, EventArgs.Empty);
            };
        }

        public static implicit operator UIElement(EditableGraph graph) => graph.Canvas;
    }

}
