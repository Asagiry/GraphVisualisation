using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphTools
{
    public class ArrowLine
    {
        /// <summary>
        /// Начальная точка стрелки
        /// </summary>
        private Point StartPoint { get; set; }
        /// <summary>
        /// Конец стрелки
        /// </summary>
        private Point EndPoint { get; set; }
        /// <summary>
        /// Стартовая вершина
        /// </summary>
        private int Start {  get; set; }
        /// <summary>
        /// Конечная вершина
        /// </summary>
        private int End { get; set; }
        public ArrowLine(Point StartPoint, Point EndPoint, Canvas Canvas, int Start = 0, int End = 0)
        {
            this.StartPoint = StartPoint;
            this.EndPoint = EndPoint;

            this.Start = Start;
            this.End = End;

            DrawArrowLine(Canvas);
        }

        private void DrawArrowLine(Canvas canvas)
        {
            if (StartPoint == EndPoint)
                return;

            Line line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = StartPoint.X,
                Y1 = StartPoint.Y,
                X2 = EndPoint.X,
                Y2 = EndPoint.Y
            };

            double arrowSize = 10;
            double angle = Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);
            double angleOffset = Math.PI / 6;

            Polyline arrow = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            arrow.Points.Add(new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2));
            arrow.Points.Add(new Point(
                (StartPoint.X + EndPoint.X) / 2 - arrowSize * Math.Cos(angle - angleOffset),
                (StartPoint.Y + EndPoint.Y) / 2 - arrowSize * Math.Sin(angle - angleOffset)));
            arrow.Points.Add(new Point(
                (StartPoint.X + EndPoint.X) / 2 - arrowSize * Math.Cos(angle + angleOffset),
                (StartPoint.Y + EndPoint.Y) / 2 - arrowSize * Math.Sin(angle + angleOffset)));
            arrow.Points.Add(new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2));

            arrow.Uid = Start.ToString()+ End.ToString();
            line.Uid = Start.ToString() + End.ToString();

            canvas.Children.Add(line);
            canvas.Children.Add(arrow);
        }
    }
}
