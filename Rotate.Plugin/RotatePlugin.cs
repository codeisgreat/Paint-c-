using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.Interface;
using System.Drawing;

namespace Rotate.Plugin
{
    public class RotatePlugin : IPlugin
    {
        public string Text
        {
            get { return "Rotate-Plugin"; }
        }

        public Bitmap workOnPicture(Bitmap original)
        {
            Bitmap returnBitmap = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(returnBitmap);
            graphics.TranslateTransform((float)original.Width / 2, (float)original.Height / 2);
            graphics.RotateTransform(90);
            graphics.TranslateTransform(-(float)original.Width / 2, -(float)original.Height / 2);
            graphics.DrawImage(original, new Point(0, 0));
            return returnBitmap;
        }
    }
}
