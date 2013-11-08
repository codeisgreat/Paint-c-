using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaintLike
{
    class Eraser : Shape
    {
        private int eraserSize;
        private List<Point> eraserLocation;

        public Eraser(List<Point> eraserLocation, int eraserSize)
        {
            // TODO: Complete member initialization
            this.eraserLocation = new List<Point>(eraserLocation);
            this.eraserSize = eraserSize;
        }

        public void draw(Graphics g)
        {
            foreach (Point p in eraserLocation)
            {
                g.FillEllipse(new SolidBrush(Color.White), p.X, p.Y, eraserSize, eraserSize);
            }
        }

    }
}
