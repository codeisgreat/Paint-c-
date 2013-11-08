using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plugin.Interface;
using System.IO;
using BlackWhite.Plugin;
using System.Reflection;

namespace PaintLike
{
    public partial class Form1 : Form
    {
        // -- VARIABLES --
       private Point startPoint;
       private Point endPoint;

       private Pen drawingPen;
       private int penSize = 1;
       private int eraserSize = 4;
       private Color color = Color.Black;

       private bool isFilled = false;
       private bool isRounded = false;
       private bool painting = false;

       private int shiftX, shiftY;

       private shape CurrentShape = shape.pen;
       private enum shape { pen, line, square, elipse, eraser }

       private String filename;
       private BackgroundWorker bw = new BackgroundWorker();
       private String operation;
       private Bitmap bmp;

       private List<Shape> drawings;
       private List<Shape> temp;
       private List<Point> penLocation;
       private List<Point> eraserLocation;

       float[] dashValues0 = { 2000, 2, 2, 2 };
       float[] dashValues1 = { 5, 2, 15, 4 };
       float[] dashValues2 = { 1, 1, 1, 1 };
       float[] dashValues; 

       private Graphics g;
       private PluginLoader _pluginLoader = new PluginLoader();
        // --

        // MAIN FORM
        public Form1()
        {
            InitializeComponent();
            drawings = new List<Shape>();
            temp = new List<Shape>();
            penLocation = new List<Point>();
            eraserLocation = new List<Point>();

            dashValues = new float[4];
            drawingPen = new Pen(color, penSize);

            

            Filling_Box.Visible = false;
            Eraser_Size_Box.Visible = false;

            ponówToolStripMenuItem.Enabled = false;

            Drawing_Box.Image = new Bitmap(Drawing_Box.Width, Drawing_Box.Height);
            g = Graphics.FromImage(Drawing_Box.Image);

            bw.DoWork +=
                new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

           // Drawing_Box.SizeMode = PictureBoxSizeMode.CenterImage;
            Drawing_Box.GetType().GetMethod("SetStyle",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic).Invoke(panel1,
                new object[]{ System.Windows.Forms.ControlStyles.UserPaint | 
	            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint | 
	            System.Windows.Forms.ControlStyles.DoubleBuffer, true });
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
 
        // DRAWING AREA
        private void Drawing_Box_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (painting)
            {
                switch (CurrentShape)
                {
                    case shape.pen:
                        foreach (Point p in penLocation)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(color), p.X, p.Y, penSize, penSize);
                        }
                        break;
                    case shape.line:
                        e.Graphics.DrawLine(drawingPen, startPoint, endPoint);
                        break;
                    case shape.square:
                        if (isFilled)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(color), Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(drawingPen, Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
                        }
                        break;
                    case shape.elipse:
                        if (isFilled)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(color), Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
                        }
                        else
                        {
                            e.Graphics.DrawEllipse(drawingPen, Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)), (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)));
                        }
                        break;
                    case shape.eraser:
                        foreach (Point p in eraserLocation)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.White), p.X, p.Y, eraserSize, eraserSize);
                        }
                        break;
                }
            }

            foreach (Shape s in drawings)
            {
                //s.draw(g);
                s.draw(e.Graphics);
            }
        }
        private void Drawing_Box_MouseUp(object sender, MouseEventArgs e)
        {
            switch (CurrentShape)
            {
                case shape.pen: drawings.Add(new PenDraw(penLocation, color, penSize)); penLocation.Clear(); break;
                case shape.line: drawings.Add(new Line(startPoint, endPoint, color, penSize, dashValues, isRounded)); break;
                case shape.square: drawings.Add(new Rectangle(startPoint, endPoint, color, penSize, dashValues, isFilled, isRounded)); break;
                case shape.elipse: drawings.Add(new Ellipse(startPoint, endPoint, color, penSize, dashValues, isFilled, isRounded)); break;
                case shape.eraser: drawings.Add(new Eraser(eraserLocation, eraserSize)); eraserLocation.Clear(); break;
            }
            painting = false;
        }
        private void Drawing_Box_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (CurrentShape == shape.pen) { penLocation.Add(e.Location); }
                if (CurrentShape == shape.eraser) { eraserLocation.Add(e.Location); }
                if (Control.ModifierKeys == Keys.Shift && CurrentShape == shape.line)
                {
                        endPoint = e.Location;
                        if ((endPoint.X > startPoint.X + 50) || endPoint.X < startPoint.X - 50)
                        {
                            shiftY = startPoint.Y;
                            shiftX = endPoint.X;
                        }
                        else
                        {
                            shiftY = endPoint.Y;
                            shiftX = startPoint.X;
                        }
                        endPoint = new Point(shiftX, shiftY);    
                }
                else if (Control.ModifierKeys == Keys.Shift && CurrentShape == shape.square)
                {
                    endPoint = e.Location;
                    endPoint = new Point(endPoint.X, endPoint.X);
                }
                else { endPoint = e.Location; }
            }
            Drawing_Box.Refresh();


            if (CurrentShape == shape.square || CurrentShape == shape.elipse)
            {
                Coords_Label.Text = e.X + "," + e.Y + "   [ " + (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)) + " x " + (Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)) + " ] ";
            }
            else
            {
                Coords_Label.Text = e.X + "," + e.Y;
            }
        }
        private void Drawing_Box_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                painting = true;
                startPoint = e.Location;
            }
        }

        // MENU PLUGIN LOADS
        private void załadujPluginyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plugin_Box.Visible = true;
            string path = GetExecutionFolder();
            var plugins = _pluginLoader.LoadPlugins(path);

            if (plugins.Count == 0)
                MessageBox.Show("No Plugins found!");

            else
            {
                foreach (IPlugin plugin in plugins)
                {
                    Button button = new Button() { Width = 195, Height = 30, Left = 2, Top =15+ Plugin_Box.Controls.Count * 30 };
                    button.Text = plugin.Text;
                    button.Tag = plugin;
                    button.Click += new EventHandler(button_Click);
                    Plugin_Box.Controls.Add(button);
                }
            }
        }
        public string GetExecutionFolder()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        void button_Click(object sender, EventArgs e)
        {
            IPlugin plugin = (sender as Button).Tag as IPlugin;
            Bitmap bmp1 = new Bitmap(Drawing_Box.Width, Drawing_Box.Height);
            panel1.DrawToBitmap(bmp1, Drawing_Box.Bounds);
            IPlugin plug= ((IPlugin)Activator.CreateInstance(plugin.GetType()));
            Drawing_Box.Image = plug.workOnPicture(bmp1);
            drawings.Clear();
            temp.Clear();
            Drawing_Box.Refresh();
        }

        // BUTTON COLOR
        private void Color_Button_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            Color_Box.BackColor = dialog.Color;
            color = dialog.Color;
            drawingPen.Color = dialog.Color;
        }

        // TYPES OF PEN - BUTTONS
        private void Pen_Button_Click(object sender, EventArgs e)
        {
            CurrentShape = shape.pen;
            Pen_Button.BackColor = SystemColors.GradientActiveCaption;
            Line_Button.BackColor = SystemColors.Control;
            Square_Button.BackColor = SystemColors.Control;
            Elipse_Button.BackColor = SystemColors.Control;
            Eraser.BackColor = SystemColors.Control;
            Filling_Box.Visible = false;
            Eraser_Size_Box.Visible = false;
            Line_Box.Visible = true;
            Line_Style_Box.Visible = true;
        }
        private void Line_Button_Click(object sender, EventArgs e)
        {
            CurrentShape = shape.line;
            Pen_Button.BackColor = SystemColors.Control;
            Line_Button.BackColor = SystemColors.GradientActiveCaption;
            Square_Button.BackColor = SystemColors.Control;
            Elipse_Button.BackColor = SystemColors.Control;
            Eraser.BackColor = SystemColors.Control;
            Filling_Box.Visible = false;
            Eraser_Size_Box.Visible = false;
            Line_Box.Visible = true;
            Line_Style_Box.Visible = true;
        }
        private void Square_Button_Click(object sender, EventArgs e)
        {
            CurrentShape = shape.square;
            Pen_Button.BackColor = SystemColors.Control;
            Line_Button.BackColor = SystemColors.Control;
            Square_Button.BackColor = SystemColors.GradientActiveCaption;
            Elipse_Button.BackColor = SystemColors.Control;
            Eraser.BackColor = SystemColors.Control;
            Filling_Box.Visible = true;
            Eraser_Size_Box.Visible = false;
            Line_Box.Visible = true;
            Line_Style_Box.Visible = true;
        }
        private void Elipse_Button_Click(object sender, EventArgs e)
        {
            CurrentShape = shape.elipse;
            Pen_Button.BackColor = SystemColors.Control;
            Line_Button.BackColor = SystemColors.Control;
            Square_Button.BackColor = SystemColors.Control;
            Elipse_Button.BackColor = SystemColors.GradientActiveCaption;
            Filling_Box.Visible = true;
            Eraser.BackColor = SystemColors.Control;
            Eraser_Size_Box.Visible = false;
            Line_Box.Visible = true;
            Line_Style_Box.Visible = true;
        }
        private void Eraser_Click(object sender, EventArgs e)
        {
            CurrentShape = shape.eraser;
            Eraser.BackColor = SystemColors.GradientActiveCaption;
            Pen_Button.BackColor = SystemColors.Control;
            Line_Button.BackColor = SystemColors.Control;
            Square_Button.BackColor = SystemColors.Control;
            Elipse_Button.BackColor = SystemColors.Control;
            Eraser_Size_Box.Visible = true;
            Filling_Box.Visible = false;
            Line_Box.Visible = false;
            Line_Style_Box.Visible = false;

        }

        // LINE SIZE - BUTTONS
        private void Line1_Button_Click(object sender, EventArgs e)
        {
            penSize = 1;
            drawingPen.Width = 1;
            Line1_Button.BackColor = SystemColors.GradientActiveCaption;
            Line2_Button.BackColor = SystemColors.Control;
            Line3_Button.BackColor = SystemColors.Control;
            Line4_Button.BackColor = SystemColors.Control;
            Line5_Button.BackColor = SystemColors.Control;
        }
        private void Line2_Button_Click(object sender, EventArgs e)
        {
            penSize = 2;
            drawingPen.Width = 2;
            Line1_Button.BackColor = SystemColors.Control;
            Line2_Button.BackColor = SystemColors.GradientActiveCaption;
            Line3_Button.BackColor = SystemColors.Control;
            Line4_Button.BackColor = SystemColors.Control;
            Line5_Button.BackColor = SystemColors.Control;
        }
        private void Line3_Button_Click(object sender, EventArgs e)
        {
            penSize = 3;
            drawingPen.Width = 3;
            Line1_Button.BackColor = SystemColors.Control;
            Line2_Button.BackColor = SystemColors.Control;
            Line3_Button.BackColor = SystemColors.GradientActiveCaption;
            Line4_Button.BackColor = SystemColors.Control;
            Line5_Button.BackColor = SystemColors.Control;
        }
        private void Line4_Button_Click(object sender, EventArgs e)
        {
            penSize = 4;
            drawingPen.Width = 4;
            Line1_Button.BackColor = SystemColors.Control;
            Line2_Button.BackColor = SystemColors.Control;
            Line3_Button.BackColor = SystemColors.Control;
            Line4_Button.BackColor = SystemColors.GradientActiveCaption;
            Line5_Button.BackColor = SystemColors.Control;
        }
        private void Line5_Button_Click(object sender, EventArgs e)
        {
            penSize = 5;
            drawingPen.Width = 5;
            Line1_Button.BackColor = SystemColors.Control;
            Line2_Button.BackColor = SystemColors.Control;
            Line3_Button.BackColor = SystemColors.Control;
            Line4_Button.BackColor = SystemColors.Control;
            Line5_Button.BackColor = SystemColors.GradientActiveCaption;
        }

        // LINE STYLING - BUTTONS
        private void NormalStyle_Button_Click(object sender, EventArgs e)
        {
            dashValues = dashValues0;
            drawingPen.DashPattern = dashValues;
            drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
            isRounded = false;
            NormalStyle_Button.BackColor = SystemColors.GradientActiveCaption;
            Dashed1_Button.BackColor = SystemColors.Control;
            Dashed2_Button.BackColor = SystemColors.Control;
            Dashed3_Button.BackColor = SystemColors.Control;
        }
        private void Dashed1_Button_Click(object sender, EventArgs e)
        {
            dashValues = dashValues1;
            drawingPen.DashPattern = dashValues;
            drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
            isRounded = false;
            NormalStyle_Button.BackColor = SystemColors.Control;
            Dashed1_Button.BackColor = SystemColors.GradientActiveCaption;
            Dashed2_Button.BackColor = SystemColors.Control;
            Dashed3_Button.BackColor = SystemColors.Control;
        }
        private void Dashed2_Button_Click(object sender, EventArgs e)
        {
            dashValues = dashValues2;
            drawingPen.DashPattern = dashValues;
            drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
            isRounded = false;
            NormalStyle_Button.BackColor = SystemColors.Control;
            Dashed1_Button.BackColor = SystemColors.Control;
            Dashed2_Button.BackColor = SystemColors.GradientActiveCaption;
            Dashed3_Button.BackColor = SystemColors.Control;
        }
        private void Dashed3_Button_Click(object sender, EventArgs e)
        {
            dashValues = dashValues2;
            drawingPen.DashPattern = dashValues;
            drawingPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            isRounded = true;
            NormalStyle_Button.BackColor = SystemColors.Control;
            Dashed1_Button.BackColor = SystemColors.Control;
            Dashed2_Button.BackColor = SystemColors.Control;
            Dashed3_Button.BackColor = SystemColors.GradientActiveCaption;
        }

        // FILLING - BUTTONS
        private void Empty_Button_Click(object sender, EventArgs e)
        {
            isFilled = false;
            Empty_Button.BackColor = SystemColors.GradientActiveCaption;
            Filled_Button.BackColor = SystemColors.Control;
        }
        private void Filled_Button_Click(object sender, EventArgs e)
        {
            isFilled = true;
            Empty_Button.BackColor = SystemColors.Control;
            Filled_Button.BackColor = SystemColors.GradientActiveCaption;
        }

        // MAIN OPERATIONS - MENU
        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Drawing_Box.Image = new Bitmap(Drawing_Box.Width, Drawing_Box.Height);
            drawings.Clear();
            temp.Clear();
            Drawing_Box.Refresh();  
        }
        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            operation = "open";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPG|*.jpg|PNG|*.png|BMP|*.bmp|GIF|*.gif|All files (*.*)|*.*";
            dialog.ShowDialog();
            filename = dialog.FileName;
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }
        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            operation = "save";
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPG|*.jpg|PNG|*.png|BMP|*.bmp|GIF|*.gif|All files (*.*)|*.*";
            dialog.ShowDialog();
            filename = dialog.FileName;


            if (dialog.FileName != "")
            {
                filename = dialog.FileName;
                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }
        }
        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
       
        // BACKGROUND WORKER
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;


            this.Invoke(new Action(() =>
            {
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar1.MarqueeAnimationSpeed = 50;
            }));


            System.Threading.Thread.Sleep(1000);
            if (operation.Equals("open"))
            {
                Drawing_Box.ImageLocation = filename;
            }
            else
            {

                this.Invoke(new Action(() =>
                {
                    bmp = new Bitmap(Drawing_Box.Width, Drawing_Box.Height);
                    panel1.DrawToBitmap(bmp, Drawing_Box.Bounds);
                }));

                bmp.Save(filename);
            }

        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            if (operation.Equals("open"))
            {
                Drawing_Box.Refresh();
                drawings.Clear();
                temp.Clear();
            }
        }

        // DO - UNDO - MENU
        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ponówToolStripMenuItem.Enabled = true;
            if (drawings.Count > 0)
            {
                temp.Add(drawings[drawings.Count - 1]);
                drawings.RemoveAt(drawings.Count - 1);
                Drawing_Box.Refresh();
            }
        }
        private void ponówToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (temp.Count > 0)
            {
                drawings.Add(temp[temp.Count - 1]);
                temp.RemoveAt(temp.Count - 1);
                Drawing_Box.Refresh();
                if (temp.Count == 0) { ponówToolStripMenuItem.Enabled = false; }
            }
        }
        // DO - UNDO SHORTCUTS
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Z))
            {
                ponówToolStripMenuItem.Enabled = true;
                if (drawings.Count > 0)
                {
                    temp.Add(drawings[drawings.Count - 1]);
                    drawings.RemoveAt(drawings.Count - 1);
                    Drawing_Box.Refresh();
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Y))
            {
                if (temp.Count > 0)
                {
                    drawings.Add(temp[temp.Count - 1]);
                    temp.RemoveAt(temp.Count - 1);
                    Drawing_Box.Refresh();
                    if (temp.Count == 0) { ponówToolStripMenuItem.Enabled = false; }
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Eraser_Small_Click(object sender, EventArgs e)
        {
            eraserSize = 4;
            Eraser_Small.BackColor = SystemColors.GradientActiveCaption;
            Eraser_Larger.BackColor = SystemColors.Control;
            Eraser_Medium.BackColor = SystemColors.Control;
            Eraser_Big.BackColor = SystemColors.Control;
        }

        private void Eraser_Larger_Click(object sender, EventArgs e)
        {
            eraserSize = 6;
            Eraser_Larger.BackColor = SystemColors.GradientActiveCaption;
            Eraser_Small.BackColor = SystemColors.Control;
            Eraser_Medium.BackColor = SystemColors.Control;
            Eraser_Big.BackColor = SystemColors.Control;
        }

        private void Eraser_Medium_Click(object sender, EventArgs e)
        {
            eraserSize = 8;
            Eraser_Medium.BackColor = SystemColors.GradientActiveCaption;
            Eraser_Small.BackColor = SystemColors.Control;
            Eraser_Larger.BackColor = SystemColors.Control;
            Eraser_Big.BackColor = SystemColors.Control;
        }

        private void Eraser_Big_Click(object sender, EventArgs e)
        {
            eraserSize = 10;
            Eraser_Big.BackColor = SystemColors.GradientActiveCaption;
            Eraser_Small.BackColor = SystemColors.Control;
            Eraser_Larger.BackColor = SystemColors.Control;
            Eraser_Medium.BackColor = SystemColors.Control;
 
        }


    }
}
