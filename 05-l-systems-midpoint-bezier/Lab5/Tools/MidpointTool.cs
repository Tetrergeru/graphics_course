using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GraphFunc.Tools
{
    public class MidpointTool : ITool
    {
        private PictureBox _picture = new PictureBox();

        private Random _random = new Random();
        
        private Graphics _drawer;

        private int DesiredDepth = 5;
        
        private const int ScrollWidth = 15;

        private const int ScrollHeight = 17;
        
        private const int BottomSpace = 50;
        
        public void Add(Panel panel)
        {
            _picture = new PictureBox
            {
                Width = panel.Width - 2 * ScrollWidth,
                Height = panel.Height - BottomSpace - 2*ScrollHeight,
                Top = ScrollHeight,
                Left = ScrollWidth,
                Image = new Bitmap(panel.Width - 2 * ScrollWidth, panel.Height),
            };
            panel.Controls.Add(_picture);
            
            AddScrollBars(panel);
            AddSettings(panel);
            
            
            MidpointDraw(0, 0);
        }

        private void AddSettings(Panel panel)
        {
            panel.Controls.Add(new Label
            {
                Text = $"Iterations: {DesiredDepth}",
                Left = 0,
                Top = panel.Height - BottomSpace + 10,
            });
            var iterations = new HScrollBar
            {
                Minimum = 0,
                Maximum = 15,
                Left = 50,
                Top = panel.Height - BottomSpace + 10,
                Height = 15,
                Width = 100,
            };
            iterations.Scroll += (sender, args) =>
            {
                DesiredDepth = args.NewValue;
                panel.Text = $"Iterations: {DesiredDepth}";
            };
            panel.Controls.Add(iterations);
        }

        private void AddScrollBars(Panel panel)
        {
            var scrollBar1 = new VScrollBar
            {
                Top = 0,
                Width = ScrollWidth,
                Height = panel.Height - BottomSpace,
                Left = 0,
                Minimum = 0,
                Maximum = panel.Height,
            };
            var scrollBar2 = new VScrollBar
            {
                Top = 0,
                Width = ScrollWidth,
                Height = panel.Height - BottomSpace,
                Left = panel.Width - ScrollWidth,
                Minimum = 0,
                Maximum = panel.Height,
            };
            
            scrollBar1.Scroll += (sender, args) => { MidpointDraw(scrollBar1.Value, scrollBar2.Value); };
            scrollBar2.Scroll += (sender, args) => { MidpointDraw(scrollBar1.Value, scrollBar2.Value); };
            panel.Controls.Add(scrollBar2);
            panel.Controls.Add(scrollBar1);
        }

        private void MidpointDraw(int h1, int h2)
        {
            var picture = new Bitmap(_picture.Image);
            _drawer = Graphics.FromImage(picture);
            _drawer.Clear(Color.White);
            MidpointIteration(new PointF(0, h1), new PointF(picture.Width - 1, h2));
            _picture.Image = picture;
        }

        private void MidpointIteration(PointF from, PointF to, int depth = 0)
        {
            var cl = (int)((DesiredDepth - depth) * (0.8 * 255 / DesiredDepth));
            _drawer.DrawLine(new Pen(Color.FromArgb(cl, 0, cl), DesiredDepth - depth + 1), from, to);
            if (depth == DesiredDepth)
            {
                return;
            }
            var midpoint = Midpoint(from, to);
            midpoint.Y += Salt(depth);
            MidpointIteration(from, midpoint, depth + 1);
            MidpointIteration(midpoint, to, depth + 1);
        }

        private PointF Midpoint(PointF from, PointF to)
            => new PointF(Middle(from.X, to.X), Middle(from.Y, to.Y));

        private float Salt(int depth)
            => (float)(200 * (_random.NextDouble() -0.5) * (DesiredDepth - depth) / DesiredDepth);
        
        private float Middle(float from, float to)
            => from > to
                ? to + (from - to) / 2
                : from + (to - from) / 2;
        

        public override string ToString() => "Midpoint displacement";
    }
}