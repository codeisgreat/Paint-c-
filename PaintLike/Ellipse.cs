using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaintLike
{
    class Ellipse : Shape
    {
        private Point startPoint;
        private Point endPoint;
        private Color color;
        private int penSize;
        private Pen drawingPen;
        private bool isFilled;
        float[] dashValues;



        public Ellipse(Point startPoint, Point endPoint, Color color, int penSize, float[] dashValues, bool isFilled, bool isRounded)
        {
            // TODO: Complete member initialization
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.color = color;
            this.penSize = penSize;
            this.dashValues = dashValues;
            this.isFilled = isFilled;
            drawingPen = new Pen(this.color, this.penSize);
            if (dashValues[0] != 0) { drawingPen.DashPattern = this.dashValues; }
            if (isRounded) { drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Round; }
        }


        public void draw(Graphics g)
        {
            if (isFilled)
            {
                g.FillEllipse(new SolidBrush(color), Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
            }
            else
            {
                g.DrawEllipse(drawingPen, Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
            }
        }


    }
}
