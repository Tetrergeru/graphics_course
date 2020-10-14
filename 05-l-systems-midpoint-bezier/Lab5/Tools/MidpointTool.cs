using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GraphFunc.Tools
{
    public class MidpointTool : ITool
    {
        private PictureBox _picture = new PictureBox();

        private int _seed;
        
        private Random _random;
        
        private Graphics _drawer;

        private int _desiredDepth = 5;

        private (int left, int right) _positions;

        private (int from, int to) _randomBounds = (200, 10);

        private bool DrawDebug = false;
        
        private const int ScrollWidth = 15;

        private const int ScrollHeight = 17;
        
        private const int BottomSpace = 50;
        
        public void Add(Panel panel)
        {
            _seed = DateTime.Now.GetHashCode();
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

            MidpointDraw();
        }

        private void AddSettings(Panel panel)
        {
            var iterationsLabel = new Label
            {
                Text = $"Iterations: {_desiredDepth}",
                Left = 0,
                Top = panel.Height - BottomSpace + 10,
            };
            panel.Controls.Add(iterationsLabel);
            var iterations = new HScrollBar
            {
                Minimum = 1,
                Maximum = 15 + 9,
                Left = 100,
                Top = panel.Height - BottomSpace + 10,
                Height = 15,
                Width = 150,
                Value = _desiredDepth,
            };
            iterations.Scroll += (sender, args) =>
            {
                _desiredDepth = args.NewValue;
                iterationsLabel.Text = $"Iterations: {_desiredDepth}";
                MidpointDraw();
            };
            panel.Controls.Add(iterations);
            
            var fromLabel = new Label
            {
                Text = $"From: {_randomBounds.from}",
                Left = 350,
                Top = panel.Height - BottomSpace + 10,
            };
            panel.Controls.Add(fromLabel);
            var from = new HScrollBar
            {
                Minimum = 50,
                Maximum = 500,
                Left = 450,
                Top = panel.Height - BottomSpace + 10,
                Height = 15,
                Width = 150,
                Value = _randomBounds.from,
            };
            from.Scroll += (sender, args) =>
            {
                _randomBounds.from = args.NewValue;
                fromLabel.Text = $"From: {_randomBounds.from}";
                MidpointDraw();
            };
            panel.Controls.Add(from);
            
            var toLabel = new Label
            {
                Text = $"To: {_randomBounds.to}",
                Left = 350,
                Top = panel.Height - BottomSpace + 35,
            };
            panel.Controls.Add(toLabel);
            var to = new HScrollBar
            {
                Minimum = 1,
                Maximum = 70,
                Left = 450,
                Top = panel.Height - BottomSpace + 35,
                Height = 15,
                Width = 150,
                Value = _randomBounds.to,
            };
            to.Scroll += (sender, args) =>
            {
                _randomBounds.to = args.NewValue;
                toLabel.Text = $"To: {_randomBounds.to}";
                MidpointDraw();
            };
            panel.Controls.Add(to);
            
            panel.Controls.Add(new Label
            {
                Text = "Debug:",
                Left = 0,
                Top = panel.Height - BottomSpace + 35,
            });
            var debug = new CheckBox
            {
                Width = 10,
                Left = 100,
                Top = panel.Height - BottomSpace + 35,
            };
            debug.Click += (sender, args) =>
            {
                DrawDebug = debug.Checked;
                MidpointDraw();
            };
            panel.Controls.Add(debug);

            var reRandom = new Button
            {
                Left = 160,
                Top = panel.Height - BottomSpace + 32,
                Text = "New random",
                Width = 100,
                Height = 21,
            };
            reRandom.Click += (sender, args) =>
            {
                _seed = DateTime.Now.GetHashCode();
                MidpointDraw();
            };
            panel.Controls.Add(reRandom);
        }

        private void AddScrollBars(Panel panel)
        {
            _positions = (_picture.Height / 2, _picture.Height/2);
            var scrollBar1 = new VScrollBar
            {
                Top = 0,
                Width = ScrollWidth,
                Height = panel.Height - BottomSpace,
                Left = 0,
                Minimum = 0,
                Maximum = _picture.Height,
                Value = _positions.left,
            };
            var scrollBar2 = new VScrollBar
            {
                Top = 0,
                Width = ScrollWidth,
                Height = panel.Height - BottomSpace,
                Left = panel.Width - ScrollWidth,
                Minimum = 0,
                Maximum = _picture.Height,
                Value = _positions.right,
            };
            
            scrollBar1.Scroll += (sender, args) =>
            {
                _positions.left = args.NewValue; 
                MidpointDraw(); 
            };
            scrollBar2.Scroll += (sender, args) => 
            {
                _positions.right = args.NewValue; 
                MidpointDraw(); 
            };
            panel.Controls.Add(scrollBar2);
            panel.Controls.Add(scrollBar1);
        }

        private void MidpointDraw()
        {
            _random = new Random(_seed);
            var picture = new Bitmap(_picture.Image);
            _drawer = Graphics.FromImage(picture);
            _drawer.Clear(Color.White);
            MidpointIteration(new PointF(0, _positions.left), new PointF(picture.Width - 1, _positions.right));
            _picture.Image = picture;
        }

        private void MidpointIteration(PointF from, PointF to, int depth = 0)
        {
            if (DrawDebug)
            {
                var cl = (int) ((_desiredDepth - depth) * (0.8 * 255 / _desiredDepth));
                _drawer.DrawLine(new Pen(Color.FromArgb(cl, 0, cl), _desiredDepth - depth + 1), from, to);
            }

            if (depth == _desiredDepth)
            {
                var cl = (int) ((_desiredDepth - depth) * (0.8 * 255 / _desiredDepth));
                _drawer.DrawLine(new Pen(Color.FromArgb(cl, 0, cl), _desiredDepth - depth + 1), from, to);
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
            => (float)((_randomBounds.to + (_randomBounds.from - _randomBounds.to) * (_desiredDepth - depth) / _desiredDepth) * (_random.NextDouble() -0.5));
        
        private float Middle(float from, float to)
            => from > to
                ? to + (from - to) / 2
                : from + (to - from) / 2;
        

        public override string ToString() => "Midpoint displacement";
    }
}