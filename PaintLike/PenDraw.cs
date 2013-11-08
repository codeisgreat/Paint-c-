using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaintLike
{
    class PenDraw : Shape
    {

        private Color color;
        private int penSize;
        private List<Point> pen;

        public PenDraw(List<Point> pen, Color color, int penSize)
        {
            // TODO: Complete member initialization
            this.color = color;
            this.penSize = penSize;
            this.pen = new List<Point>(pen);
        }

        public void draw(Graphics g)
        {
            foreach (Point p in pen)
            {
                g.FillEllipse(new SolidBrush(color), p.X, p.Y, penSize, penSize);
            }
        }
    }
}
