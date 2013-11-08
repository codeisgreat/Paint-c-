using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Plugin.Interface
{
    public interface IPlugin
    {
        string Text { get; }
        Bitmap workOnPicture(Bitmap original);
    }
}
