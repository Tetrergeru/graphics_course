using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GraphFunc.Tools
{
    class BezierTool : ITool
    {
        private PictureBox _picture = new PictureBox();

        private Graphics _drawer;

        private List<(Point, int)> _bearing_points = new List<(Point, int)>();

        private bool _move_flag = false;

        private bool _add_flag = true;

        private bool _delete_flag = false;

        private bool _button_down = false;

        private Button _move_button;

        private Button _add_button;

        private Button _delete_button;

        private PolygonContainer _polygonContainer = new PolygonContainer();

        private Point _from;

        private int _moving = -1;

        public void Add(Panel panel)
        {
            _picture = new PictureBox
            {
                Width = panel.Width,
                Height = panel.Height - 60,
                Top = 5,
                Left = 5,
                Image = new Bitmap(panel.Width, panel.Height),
            };
            _picture.MouseDown += (o, e) => Draw(drawer => OnMouseDown(e.Location));
            _picture.MouseUp += (o, e) => Draw(drawer => OnMouseUp(e.Location));
            _picture.MouseMove += (o, e) => Draw(drawer => OnMouseMove(e.Location));
            panel.Controls.Add(_picture);

            AddButtons(panel);
            BezierDraw();
        }

        private void AddButtons(Panel panel)
        {
            _move_button = new Button
            {
                BackColor = Color.White,
                Width = 150,
                Height = 20,
                Top = panel.Height - 40,
                Left = 30,
                Text = "Move point",
            };
            _move_button.Click += (o, e) =>
            {
                _move_flag = true;
                _add_flag = false;
                _delete_flag = false;
                _move_button.BackColor = Color.Aquamarine;
                _add_button.BackColor = Color.White;
                _delete_button.BackColor = Color.White;

            };
            panel.Controls.Add(_move_button);

            _add_button = new Button
            {
                BackColor = Color.Aquamarine,
                Width = 150,
                Height = 20,
                Top = panel.Height - 40,
                Left = 30 + 160,
                Text = "Add point",
            };
            _add_button.Click += (o, e) =>
            {
                _add_flag = true;
                _move_flag = false;
                _delete_flag = false;
                _add_button.BackColor = Color.Aquamarine;
                _move_button.BackColor = Color.White;
                _delete_button.BackColor = Color.White;
            };
            panel.Controls.Add(_add_button);

            _delete_button = new Button
            {
                BackColor = Color.White,
                Width = 150,
                Height = 20,
                Top = panel.Height - 40,
                Left = 30 + 320,
                Text = "Delete point",
            };
            _delete_button.Click += (o, e) =>
            {
                _delete_flag = true;
                _move_flag = false;
                _add_flag = false;
                _delete_button.BackColor = Color.Aquamarine;
                _add_button.BackColor = Color.White;
                _move_button.BackColor = Color.White;
            };
            panel.Controls.Add(_delete_button);
        }

        private void Draw(Action<Graphics> draw)
        {
            var image = new Bitmap(_picture.Width, _picture.Height);
            var drawer = Graphics.FromImage(image);
            draw(drawer);
            drawer.Flush();
        }

        public void OnMouseUp(Point coordinates)
        {
            _button_down = false;
            if (_move_flag)
            {
                _from = coordinates;
                _moving = -1;
            }
            BezierDraw();
        }

        public void OnMouseMove(Point coordinates)
        {
            if (_move_flag && _button_down && _moving >= 0)
            {
                PointF delta = new PointF(coordinates.X - _from.X, coordinates.Y - _from.Y);
                _polygonContainer.Selected.Load();
                _polygonContainer.Selected.Move(delta);

                var matrix = new Matrix
                {
                    [0, 0] = 1,
                    [1, 1] = 1,
                    [2, 2] = 1,
                    [0, 2] = delta.X,
                    [1, 2] = delta.Y
                };
                _bearing_points[_moving] = (coordinates, _moving);
            }
            BezierDraw();
        }

        public void OnMouseDown(Point coordinates)
        {
            if (_add_flag)
            {
                int ord = _bearing_points.Count;
                _bearing_points.Add((new Point(coordinates.X, coordinates.Y), ord + 1));
                _polygonContainer.AddPolygon();
                _polygonContainer.AddPoint(new Point(coordinates.X - 5, coordinates.Y - 5));
                _polygonContainer.AddPoint(new Point(coordinates.X + 5, coordinates.Y - 5));
                _polygonContainer.AddPoint(new Point(coordinates.X + 5, coordinates.Y + 5));
                _polygonContainer.AddPoint(new Point(coordinates.X - 5, coordinates.Y + 5));
            }
            if (_move_flag)
            {
                for (int i = 0; i < _polygonContainer.Polygons.Count; i++)
                    if (_polygonContainer.Polygons[i].HasPoint(coordinates))
                    {
                        _polygonContainer.Select(i);
                        _moving = i;
                        break;
                    }

                _from = coordinates;
                _button_down = true;
                _polygonContainer.Selected.Save();
            }
            if (_delete_flag)
            {
                for (int i = 0; i < _polygonContainer.Polygons.Count; i++)
                    if (_polygonContainer.Polygons[i].HasPoint(coordinates))
                    {
                        _polygonContainer.Select(i);
                        _polygonContainer.DeleteSelected();
                        _bearing_points.RemoveAt(i);
                        break;
                    }
            }
            BezierDraw();
        }

        private void BezierDraw()
        {
            var picture = new Bitmap(_picture.Image);
            _drawer = Graphics.FromImage(picture);
            _drawer.Clear(Color.White);
            if (_bearing_points.Count > 1)
                for (int i = 0; i < _bearing_points.Count - 1; i++)
                    LineWu(picture, Color.Green, (_bearing_points[i].Item1.X, _bearing_points[i].Item1.Y), (_bearing_points[i + 1].Item1.X, _bearing_points[i + 1].Item1.Y));
            
            for (int i = 0; i < _bearing_points.Count; i++)
            {
                if (i == 0 || i == (_bearing_points.Count - 1))
                    _polygonContainer.Polygons[i].Draw(_drawer, Color.Red);
                else
                    _polygonContainer.Polygons[i].Draw(_drawer, Color.Blue);
            }

            int index = 0;
            while (_bearing_points.Count - index > 2)
            {
                if (_bearing_points.Count - index > 3)
                {
                    CubicBezier(ref picture, index);
                    index += 3;
                }
                else
                {
                    SquareBezier(ref picture, index);
                    index += 2;
                }
            }
            
            _picture.Image = picture;
        }

        private void SquareBezier(ref Bitmap img, int ind)
        {
            for (float t = 0; t <= 1; t += (float)0.001)
            {
                PointF q_0 = new PointF(_bearing_points[ind].Item1.X * (1 - t) + _bearing_points[ind + 1].Item1.X * t, _bearing_points[ind].Item1.Y * (1 - t) + _bearing_points[ind + 1].Item1.Y * t);
                PointF q_1 = new PointF(_bearing_points[ind + 1].Item1.X * (1 - t) + _bearing_points[ind + 2].Item1.X * t, _bearing_points[ind + 1].Item1.Y * (1 - t) + _bearing_points[ind + 2].Item1.Y * t);
                PointF b = new PointF(q_0.X * (1 - t) + q_1.X * t, q_0.Y * (1 - t) + q_1.Y * t);
                img.SetPixel((int)b.X, (int)b.Y, Color.Black);
            }
        }

        private void CubicBezier(ref Bitmap img, int ind)
        {
            for (float t = 0; t <= 1; t += (float)0.001)
            {
                PointF q_0 = new PointF(_bearing_points[ind].Item1.X * (1 - t) + _bearing_points[ind + 1].Item1.X * t, _bearing_points[ind].Item1.Y * (1 - t) + _bearing_points[ind + 1].Item1.Y * t);
                PointF q_1 = new PointF(_bearing_points[ind + 1].Item1.X * (1 - t) + _bearing_points[ind + 2].Item1.X * t, _bearing_points[ind + 1].Item1.Y * (1 - t) + _bearing_points[ind + 2].Item1.Y * t);
                PointF q_2 = new PointF(_bearing_points[ind + 2].Item1.X * (1 - t) + _bearing_points[ind + 3].Item1.X * t, _bearing_points[ind + 2].Item1.Y * (1 - t) + _bearing_points[ind + 3].Item1.Y * t);
                PointF r_0 = new PointF(q_0.X * (1 - t) + q_1.X * t, q_0.Y * (1 - t) + q_1.Y * t);
                PointF r_1 = new PointF(q_1.X * (1 - t) + q_2.X * t, q_1.Y * (1 - t) + q_2.Y * t);
                PointF b = new PointF(r_0.X * (1 - t) + r_1.X * t, r_0.Y * (1 - t) + r_1.Y * t);

                img.SetPixel((int)b.X, (int)b.Y, Color.Black);
            }
        }

        public static void LineWu(Bitmap image, Color color, (int X, int Y) from, (int X, int Y) to)
        {
                var cl = (color.R, color.G, color.B);
                var steep = Math.Abs(to.Y - from.Y) > Math.Abs(to.X - from.X);
                if (steep)
                {
                    Swap(ref from.X, ref from.Y);
                    Swap(ref to.X, ref to.Y);
                }

                if (from.X > to.X)
                {
                    Swap(ref from.X, ref to.X);
                    Swap(ref from.Y, ref to.Y);
                }

                var dx = to.X - from.X;
                var dy = to.Y - from.Y;
                var gradient = dx == 0 ? 1.0 : dy * 1.0 / dx;
                var y = from.Y + gradient;
                for (var x = from.X; x != to.X; x += 1)
                {
                    if (steep)
                    {
                        SetPixel(image, new Point((int)y, x), cl, Fpart(y));
                        SetPixel(image, new Point((int)y + 1, x), cl, RFpart(y));
                    }
                    else
                    {
                        SetPixel(image, new Point(x, (int)y), cl, Fpart(y));
                        SetPixel(image, new Point(x, (int)y + 1), cl, RFpart(y));
                    }

                    y += gradient;
                }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        private static double Fpart(double y)
            => y - Math.Truncate(y);

        private static double RFpart(double y)
            => 1 - Fpart(y);

        private static (byte r, byte g, byte b) AdjustIntensity((byte r, byte g, byte b) cl, (byte r, byte g, byte b) bc, double y)
            => (
                (byte)(cl.r * (1 - y) + bc.r * y),
                (byte)(cl.g * (1 - y) + bc.g * y),
                (byte)(cl.b * (1 - y) + bc.b * y)
                );


        private static void SetPixel(Bitmap bitmap, Point point, (byte r, byte g, byte b) color, double y)
        {
            var cl = bitmap.GetPixel(point.X, point.Y);
            var res = AdjustIntensity(color, (cl.R, cl.G, cl.B), y);
            bitmap.SetPixel(point.X, point.Y, Color.FromArgb(res.r, res.g, res.b));
        }

        //public (int, int) Contains(Point p1, Point p2, Point p)
        //{
        //    float v1_x = p2.X - p1.X;
        //    float v1_y = p2.Y - p1.Y;
        //    float v2_x = p.X - p1.X;
        //    float v2_y = p.Y - p1.Y;
        //    if ((v2_x * v1_y - v2_y * v1_x) == 0)
        //        if ((p1.X < p.X && p.X < p2.X) || (p2.X < p.X && p.X < p1.X))
        //            return true;
        //    return false;
        //}
        public override string ToString() => "Bezier curve";
    }
}
