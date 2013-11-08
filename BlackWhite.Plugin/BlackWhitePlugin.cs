using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plugin.Interface;
using System.Drawing;

namespace BlackWhite.Plugin
{
   public class BlackWhitePlugin : IPlugin
    {

        public string Text
        {
            get { return "Black-White-Plugin"; }
        }

        public Bitmap workOnPicture(Bitmap original)
        {
            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {

                for (int j = 0; j < original.Height; j++)
                {

                    Color c = original.GetPixel(i, j);

                    int average = ((c.R + c.B + c.G) / 3);

                    if (average < 200)
                        output.SetPixel(i, j, Color.Black);

                    else
                        output.SetPixel(i, j, Color.White);

                }
            }

            return output; 
        }


    }
}
