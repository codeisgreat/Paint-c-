using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaintLike
{
    class Rectangle : Shape
    {
        private Point startPoint;
        private Point endPoint;
        private Color color;
        private int penSize;
        private Pen drawingPen;
        private bool isFilled;
        private float[] dashValues;

        public Rectangle(Point startPoint, Point endPoint, Color color, int penSize, float[] dashValues, bool isFilled, bool isRounded)
        {
            // TODO: Complete member initialization
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.color = color;
            this.penSize = penSize;
            this.dashValues = dashValues;
            this.isFilled = isFilled;
            drawingPen = new Pen(color, penSize);
            if (dashValues[0] != 0) { drawingPen.DashPattern = this.dashValues; }
            if (isRounded) { drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Round; }
        }


        public void draw(Graphics g)
        {
            int x, y;
            int xStart, yStart, xEnd, yEnd;
            xStart = Math.Min(startPoint.X, endPoint.X);
            yStart = Math.Min(startPoint.Y, endPoint.Y);
            xEnd = Math.Max(startPoint.X, endPoint.X);
            yEnd = Math.Max(startPoint.Y, endPoint.Y);
            x = xEnd - xStart;
            y = yEnd - yStart;
            if (isFilled)
            {
                g.FillRectangle(new SolidBrush(color), Math.Min(startPoint.X, endPoint.X), yStart = Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
            }
            else
            {
                g.DrawRectangle(drawingPen, Math.Min(startPoint.X, endPoint.X), yStart = Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
            }
        }


    }
}
