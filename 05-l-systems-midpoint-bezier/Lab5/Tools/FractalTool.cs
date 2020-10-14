using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphFunc.Tools
{
    class FractalTool : ITool
    {
        private PictureBox _picture = new PictureBox();

        private int _desiredDepth = 1;

        private Graphics _drawer;

        private string _fname = "";

        private const int BottomSpace = 50;

        private Random rand = new Random();

        public void Add(Panel panel)
        {

            _picture = new PictureBox
            {
                Width = panel.Width,
                Height = panel.Height - BottomSpace,
                Top = 5,
                Left = 5,
                Image = new Bitmap(panel.Width, panel.Height),
            };
            panel.Controls.Add(_picture);

            AddSettings(panel);

            ParseFile();
        }


        private void AddSettings(Panel panel)
        {
            var depthLabel = new Label
            {
                Text = $"Depth: {_desiredDepth}",
                Left = 0,
                Top = panel.Height - BottomSpace + 10,
            };
            panel.Controls.Add(depthLabel);
            var depth = new HScrollBar
            {
                Minimum = 1,
                Maximum = 15 + 9,
                Left = 100,
                Top = panel.Height - BottomSpace + 10,
                Height = 15,
                Width = 150,
                Value = _desiredDepth,
            };
            depth.Scroll += (sender, args) =>
            {
                _desiredDepth = args.NewValue;
                depthLabel.Text = $"Depth: {_desiredDepth}";
                ParseFile();
            };
            panel.Controls.Add(depth);

            var fileLoad = new Button
            {
                Left = 320,
                Top = panel.Height - BottomSpace + 10,
                Text = "New file",
                Width = 150,
                Height = 25,

            };
            fileLoad.Click += (sender, args) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "TXT files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    _fname = openFileDialog.FileName;
                ParseFile();
            };

            panel.Controls.Add(fileLoad);
        }

        private void FractalDraw(string state, (double, double) angle, string direction)
        {

            var points = new List<Point>();

            const int length = 100;
            var currentAngle = 0.0;
            switch (direction)
            {
                case "left":
                    currentAngle = 180;
                    break;
                case "up":
                    currentAngle = 270;
                    break;
            }
            var currentPoint = new Point(0, 0);
            points.Add(currentPoint);

            var branches = new List<Stack<Point>> {new Stack<Point>()};
            var angles = new List<double> {currentAngle};

            foreach (var c in state)
            {
                switch (c)
                {
                    case '[':
                    {
                        Stack<Point> st = new Stack<Point>();
                        st.Push(currentPoint);
                        branches.Add(st);
                        angles.Add(currentAngle);
                        break;
                    }
                    case ']':
                    {
                        var sz = branches.Count;
                        while (branches[sz - 1].Any())
                        {
                            var pt = branches[sz - 1].Pop();
                            points.Add(pt);
                            if (branches[sz - 1].Any()) 
                                continue;
                            currentAngle = angles[sz - 1];
                            currentPoint = pt;
                        }
                        branches.RemoveAt(sz - 1);
                        angles.RemoveAt(sz - 1);
                        break;
                    }
                    case 'F':
                    {
                        var xNew = currentPoint.X + (int)(length * Math.Cos(currentAngle / 180 * Math.PI));
                        var yNew = currentPoint.Y + (int)(length * Math.Sin(currentAngle / 180 * Math.PI));
                        var nextPoint = new Point(xNew, yNew);
                        currentPoint = nextPoint;
                        points.Add(currentPoint);
                        branches[branches.Count - 1].Push(currentPoint);
                        break;
                    }
                    case '-':
                        currentAngle -= angle.Item1 + rand.NextDouble() * (angle.Item2 - angle.Item1);
                        break;
                    case '+':
                        currentAngle += angle.Item1 + rand.NextDouble() * (angle.Item2 - angle.Item1);
                        break;
                }
            }

            var xMin = int.MaxValue;
            var xMax = int.MinValue;
            var yMin = int.MaxValue;
            var yMax = int.MinValue;

            foreach (var t in points)
            {
                if (t.X < xMin)
                    xMin = t.X;
                if (t.X > xMax)
                    xMax = t.X;
                if (t.Y < yMin)
                    yMin = t.Y;
                if (t.Y > yMax)
                    yMax = t.Y;
            }

            var pointMiddle = new Point((xMin + xMax) / 2, (yMin + yMax) / 2);
            var windowMiddle = new Point(_picture.Width / 2, _picture.Height / 2);

            var coefX = (double)_picture.Width / (xMax - xMin + 1); 
            var coefY = (double)_picture.Height / (yMax - yMin);
            var coef = Math.Min(coefX, coefY);

            var newPoints = new List<Point>();
            foreach (var t in points)
            {
                var distX = t.X - pointMiddle.X;
                var distY = t.Y - pointMiddle.Y;
                distX = (int)(distX * coef);
                distY = (int)(distY * coef);

                var np = windowMiddle;
                np.X += distX;
                np.Y += distY;
                newPoints.Add(np);
            }

            var pen = new Pen(Color.Red, 2);
            var picture = new Bitmap(_picture.Image);
            _drawer = Graphics.FromImage(picture);
            _drawer.Clear(Color.White);
            _picture.Image = picture;
            for (var i = 0; i < newPoints.Count - 1; i++)
                _drawer.DrawLine(pen, newPoints[i], newPoints[i + 1]);
        }

        private void ParseFile()
        {
            if (_fname.Length == 0)
                return;
            var sr = new System.IO.StreamReader(_fname);        

            Dictionary<char, string> rules = new Dictionary<char, string>();

            string[] strs = sr.ReadLine().Split(' ');
            string current_state = strs[0];
            (double, double) angle = (double.Parse(strs[1]), double.Parse(strs[2]));
            string direction = strs[3];

            while (!sr.EndOfStream)
            {
                string str;
                str = sr.ReadLine();
                rules.Add(str[0], str.Substring(2));
            }
            sr.Close();

            for (var i = 0; i < _desiredDepth; i++)
            {
                var nextState = "";
                foreach (var c in current_state)
                {
                    if (rules.ContainsKey(c))
                        nextState += rules[c];
                    else
                        nextState += c;
                }
                current_state = nextState;
            }

            //Console.WriteLine(current_state);
            FractalDraw(current_state, angle, direction);
        }
    

        public override string ToString() => "Fractal";
    }
}
