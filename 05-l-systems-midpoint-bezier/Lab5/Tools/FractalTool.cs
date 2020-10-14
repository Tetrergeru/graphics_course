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

            parseFile();
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
                Minimum = 10,
                Maximum = 150,
                Left = 100,
                Top = panel.Height - BottomSpace + 10,
                Height = 15,
                Width = 150,
                Value = _desiredDepth * 10,
            };
            depth.Scroll += (sender, args) =>
            {
                _desiredDepth = args.NewValue / 10;
                depthLabel.Text = $"Depth: {_desiredDepth}";
                parseFile();
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
                openFileDialog.Filter = "TXT files (*.txt)|*.txt"; if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _fname = openFileDialog.FileName;
                parseFile();
            };

            panel.Controls.Add(fileLoad);
        }

        public void FractalDraw(string state, double angle, string direction)
        {

            List<Point> points = new List<Point>();

            int length = 100;
            double current_angle = 0;
            switch (direction)
            {
                case "left":
                    current_angle = 180;
                    break;
                case "up":
                    current_angle = 270;
                    break;
            }
            Point current_point = new Point(0, 0);
            points.Add(current_point);

            List<Stack<Point>> branches = new List<Stack<Point>>();
            branches.Add(new Stack<Point>());
            List<double> angles = new List<double>();
            angles.Add(current_angle);

            foreach (var c in state)
            {
                if (c == '[')
                {
                    Stack<Point> st = new Stack<Point>();
                    st.Push(current_point);
                    branches.Add(st);
                    angles.Add(current_angle);
                }
                else if (c == ']')
                {
                    int sz = branches.Count();
                    while (branches[sz - 1].Count() != 0)
                    {
                        var pt = branches[sz - 1].Pop();
                        points.Add(pt);
                        if (branches[sz - 1].Count() == 0)
                        {
                            current_angle = angles[sz - 1];
                            current_point = pt;
                        }
                    }
                    branches.RemoveAt(sz - 1);
                    angles.RemoveAt(sz - 1);
                }
                else if (c == 'F')
                {
                    int x_new = current_point.X + (int)(length * Math.Cos(current_angle / 180 * Math.PI));
                    int y_new = current_point.Y + (int)(length * Math.Sin(current_angle / 180 * Math.PI));
                    Point next_point = new Point(x_new, y_new);
                    current_point = next_point;
                    points.Add(current_point);
                    branches[branches.Count() - 1].Push(current_point);
                }
                else if (c == '-')
                    current_angle -= angle;
                else if (c == '+')
                    current_angle += angle;
            }

            int x_min = int.MaxValue;
            int x_max = int.MinValue;
            int y_min = int.MaxValue;
            int y_max = int.MinValue;

            for (int i = 0; i < points.Count(); ++i)
            {
                if (points[i].X < x_min)
                    x_min = points[i].X;
                if (points[i].X > x_max)
                    x_max = points[i].X;
                if (points[i].Y < y_min)
                    y_min = points[i].Y;
                if (points[i].Y > y_max)
                    y_max = points[i].Y;
            }

            Point point_middle = new Point((x_min + x_max) / 2, (y_min + y_max) / 2);
            Point window_middle = new Point(_picture.Width / 2, _picture.Height / 2);

            double coef_x = (double)_picture.Width / (x_max - x_min + 1); 
            double coef_y = (double)_picture.Height / (y_max - y_min);
            double coef = Math.Min(coef_x, coef_y);

            List<Point> new_points = new List<Point>();
            for (int i = 0; i < points.Count(); ++i)
            {
                int dist_x = points[i].X - point_middle.X;
                int dist_y = points[i].Y - point_middle.Y;
                dist_x = (int)(dist_x * coef);
                dist_y = (int)(dist_y * coef);

                Point np = window_middle;
                np.X += dist_x;
                np.Y += dist_y;
                new_points.Add(np);
            }

            Pen pen = new Pen(Color.Red, 2);
            var picture = new Bitmap(_picture.Image);
            _drawer = Graphics.FromImage(picture);
            _drawer.Clear(Color.White);
            _picture.Image = picture;
            for (int i = 0; i < new_points.Count() - 1; ++i)
                _drawer.DrawLine(pen, new_points[i], new_points[i + 1]);
        }

        public void parseFile()
        {
            if (_fname.Length == 0)
                return;
            System.IO.StreamReader sr = new System.IO.StreamReader(_fname);        

            Dictionary<char, string> rules = new Dictionary<char, string>();

            string[] strs = sr.ReadLine().Split(' ');
            string current_state = strs[0];
            double angle = double.Parse(strs[1]);
            string direction = strs[2];

            while (!sr.EndOfStream)
            {
                string str;
                str = sr.ReadLine();
                rules.Add(str[0], str.Substring(2));
            }
            sr.Close();

            for (int i = 0; i < _desiredDepth; i++)
            {
                string next_state = "";
                foreach (var c in current_state)
                {
                    if (rules.ContainsKey(c))
                        next_state += rules[c];
                    else
                        next_state += c;
                }
                current_state = next_state;
            }

            Console.WriteLine(current_state);
            FractalDraw(current_state, angle, direction);
        }
    

        public override string ToString() => "Fractal";
    }
}
