using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaintLike
{
    class Line : Shape
    {
        private Point startPoint;
        private Point endPoint;
        private Color color;
        private int penSize;
        private float[] dashValues;
        private Pen drawingPen;

        public Line(Point startPoint, Point endPoint, Color color, int penSize, float[] dashValues, bool isRounded)
        {
            // TODO: Complete member initialization
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.color = color;
            this.penSize = penSize;
            this.dashValues = dashValues;
            drawingPen = new Pen(this.color, this.penSize);
            if(dashValues[0] != 0 ) { drawingPen.DashPattern = this.dashValues;}
            if (isRounded) { drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Round; }
           // drawingPen.DashPattern = this.dashValues;
        }



        public void draw(Graphics g)
        {
            g.DrawLine(drawingPen, startPoint, endPoint);
        }
    }
}
