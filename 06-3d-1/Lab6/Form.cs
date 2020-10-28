using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private const int ScreenWidth = 750;

        private const int ScreenHeight = 750;

        private const int PointPanelWidth = 100;

        private const float Max = 20;

        private PictureBox _screen;

        private readonly List<String> _models = new List<string>
        {
            //"Models/Square.obj",
            //"Models/Tetrahedron.obj",
            //"Models/Hexahedron.obj",
            //"Models/Octahedron.obj",
            //"Models/Dodecahedron.obj",
            //"Models/Icosahedron.obj",
            "Models/Cube.obj",
            //"Models/Skull.obj",
            //"Models/Prism.obj",
            //"Models/Cat.obj",
        };

        private int _currentModel;

        private Model _model;

        private readonly List<IProjection> _projection = new List<IProjection>
        {
            new ProjectionPerspective(),
            //new ProjectionIsometric(),
            //new ProjectionOrthographic(Axis.Z),
            //new ProjectionOrthographic(Axis.Y),
            //new ProjectionOrthographic(Axis.X),
        };

        private int _currentProjection;

        private (Point3 from, Point3 to) RotationLine = (new Point3(0, 0, 0), new Point3(0, 0, 75));

        public Form()
        {
            KeyPreview = true;
            Width = ScreenWidth + PointPanelWidth + 50 + 19;
            Height = ScreenHeight + 50 + 27;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.Beige;
            _model = Model.LoadFromObj(File.ReadLines(_models[_currentModel]), _models[_currentModel]);
            Text = _model.Name;

            AddScreen();
            ControlTools();

            DrawAll();
        }

        private void AddScreen()
        {
            _screen = new PictureBox
            {
                Left = 25,
                Top = 25,
                Height = ScreenHeight,
                Width = ScreenWidth,
            };
            _screen.Image = new Bitmap(_screen.Width, _screen.Height);
            _screen.MouseUp += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    _currentProjection = (_currentProjection + 1) % _projection.Count;
                else
                {
                    _currentModel = (_currentModel + 1) % _models.Count;
                    Text = _models[_currentModel];
                    _model = Model.LoadFromObj(File.ReadLines(_models[_currentModel]), _models[_currentModel]);
                }

                DrawAll();
            };

            Controls.Add(_screen);
        }

        private void ControlTools()
        {
            var x1Field = ControlBox(25, 0);
            var y1Field = ControlBox(25, 1);
            var z1Field = ControlBox(25, 2);
            var x2Field = ControlBox(100, 0);
            var y2Field = ControlBox(100, 1);
            var z2Field = ControlBox(100, 2);
            var segmentsSpin = ControlBox(250, 1);
            
            var x0FieldGraphic = ControlBox(350, 0);
            var x1FieldGraphic = ControlBox(350, 1);
            var y0FieldGraphic = ControlBox(375, 0);
            var y1FieldGraphic = ControlBox(375, 1);
            var stepFieldGraphic = ControlBox(425, 0);
            var funcFieldGraphic = ControlBox2(500, 0);
            
            var axisSpin = new CheckedListBox()
            {
                Left = ScreenWidth + 25 + 10,
                Width = 100,
                Height = 50,
                Top = 190,
            };
            string[] myAxis = { "X", "Y", "Z" };
            axisSpin.Items.AddRange(myAxis);
            axisSpin.CheckOnClick = true;
            axisSpin.SelectionMode = SelectionMode.One;
            axisSpin.SelectedIndexChanged += (sender, args) =>
            {
                if (axisSpin.CheckedItems.Count > 1)
                {
                    for (int i = 0; i < axisSpin.Items.Count; i++)
                        axisSpin.SetItemChecked(i, false);
                    axisSpin.SetItemChecked(axisSpin.SelectedIndex, true);
                }
            };
            
            Controls.Add(axisSpin);

            var button = new Button
            {
                Left = ScreenWidth + 25 + 10,
                Width = 75,
                Height = 20,
                Top = 130,
                Text = "Set Points",
            };
            button.Click += (sender, args) =>
            {
                RotationLine = (
                    new Point3(
                        IntParse(x1Field.Text, 0),
                        IntParse(y1Field.Text, 0),
                        IntParse(z1Field.Text, 0)),
                    new Point3(
                        IntParse(x2Field.Text, 0),
                        IntParse(y2Field.Text, 0),
                        IntParse(z2Field.Text, 75))
                );
                DrawAll();
            };
            Controls.Add(button);
            KeyDown += (sender, args) =>
            {
                switch (args.KeyCode)
                {
                    case Keys.W:
                        //_model.Move(new Point3(0, -1, 0));
                        ProjectionPerspective.Projector.Move(Axis.Z, 0.5f);
                        break;
                    case Keys.S:
                        //_model.Move(new Point3(0, 1, 0));
                        ProjectionPerspective.Projector.Move(Axis.Z, -0.5f);
                        break;
                    case Keys.A:
                        //_model.Move(new Point3(-1, 0, 0));
                        ProjectionPerspective.Projector.Move(Axis.X, -0.5f);
                        break;
                    case Keys.D:
                        //_model.Move(new Point3(1, 0, 0));
                        ProjectionPerspective.Projector.Move(Axis.X, 0.5f);
                        break;
                    case Keys.Q:
                        _model.Move(new Point3(0, 0, -1));
                        break;
                    case Keys.E:
                        _model.Move(new Point3(0, 0, 1));
                        break;
                    case Keys.Left:
                        //_model.RotateCenter(Axis.Y, (float) Math.PI / 12);
                        ProjectionPerspective.Projector.Rotate(Axis.Y, -(float)Math.PI/100);
                        break;
                    case Keys.Right:
                        //_model.RotateCenter(Axis.Y, -(float) Math.PI / 12);
                        ProjectionPerspective.Projector.Rotate(Axis.Y, (float)Math.PI/100);
                        break;
                    case Keys.Up:
                        //_model.RotateCenter(Axis.X, (float) Math.PI / 12);
                        ProjectionPerspective.Projector.Rotate(Axis.X, -(float)Math.PI/100);
                        break;
                    case Keys.Down:
                        //_model.RotateCenter(Axis.X, -(float) Math.PI / 12);
                        ProjectionPerspective.Projector.Rotate(Axis.X, (float)Math.PI/100);
                        break;
                    case Keys.PageUp:
                        _model.RotateCenter(Axis.Z, (float) Math.PI / 12);
                        break;
                    case Keys.PageDown:
                        _model.RotateCenter(Axis.Z, -(float) Math.PI / 12);
                        break;
                    case Keys.F1:
                        _model.Reflect(Axis.X);
                        break;
                    case Keys.F2:
                        _model.Reflect(Axis.Y);
                        break;
                    case Keys.F3:
                        _model.Reflect(Axis.Z);
                        break;
                    case Keys.Z:
                        _model.Move(new Point3(RotationLine.from.X * (-1), RotationLine.from.Y * (-1),
                            RotationLine.from.Z * (-1)));
                        _model.RotateLine(RotationLine.from, RotationLine.to, (float) Math.PI / 12);
                        _model.Move(new Point3(RotationLine.from.X, RotationLine.from.Y, RotationLine.from.Z));
                        break;
                    case Keys.X:
                        _model.Move(new Point3(RotationLine.from.X * (-1), RotationLine.from.Y * (-1),
                            RotationLine.from.Z * (-1)));
                        _model.RotateLine(RotationLine.from, RotationLine.to, -(float) Math.PI / 12);
                        _model.Move(new Point3(RotationLine.from.X, RotationLine.from.Y, RotationLine.from.Z));
                        break;
                    case Keys.P:
                        if (_model.Polygons.Count == 1)
                        {
                            var temp = _model.MakeSpinObj(_model, axisSpin.CheckedItems[0].ToString(),
                                IntParse(segmentsSpin.Text, 0));
                            _model = temp;
                        }
                        break;
                    case Keys.G:
                    {
                        double x0, x1, y0, y1, step;
                        Double.TryParse(x0FieldGraphic.Text, out x0);
                        Double.TryParse(x1FieldGraphic.Text, out x1);
                        Double.TryParse(y0FieldGraphic.Text, out y0);
                        Double.TryParse(y1FieldGraphic.Text, out y1);
                        Double.TryParse(stepFieldGraphic.Text, out step);

                        _model = Model.MakeGraphic(GetFunc(funcFieldGraphic.Text), (float) x0, (float) y0, (float) x1, (float) y1, (float) step);
                        break;
                    }
                    case Keys.L:
                    {
                        var dialog = new OpenFileDialog
                        {
                            InitialDirectory = "c:\\",
                            RestoreDirectory = true,
                            ShowHelp = true,
                        };
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK || dialog.SafeFileName == null)
                            break;
                        _model = Model.LoadFromObj(File.ReadLines(dialog.FileName), dialog.FileName);
                        break;
                    }
                    case Keys.K:
                    {
                        var dialog = new SaveFileDialog
                        {
                            InitialDirectory = "c:\\",
                            RestoreDirectory = true,
                            ShowHelp = true,
                        };
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK)
                            break;
                        File.WriteAllLines(dialog.FileName, _model.SaveToObj());
                        break;
                    }
                }

                DrawAll();
            };

            MouseWheel += (sender, args) =>
            {
                _model.ScaleCenter(args.Delta > 0 ? 1.1f : 0.9f);
                DrawAll();
            };
        }

        private Func<float, float, float> GetFunc(string func)
        {
            var list = func.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return (x, y) =>
            {
                var stack = new Stack<float>();
                foreach (var command in list)
                {
                    switch (command.ToLower())
                    {
                        case "x":
                            stack.Push(x);
                            break;
                        case "y":
                            stack.Push(y);
                            break;
                        case "e":
                            stack.Push((float) Math.E);
                            break;
                        case "pi":
                            stack.Push((float) Math.PI);
                            break;
                        case "+":
                            stack.Push(stack.Pop() + stack.Pop());
                            break;
                        case "--":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(a - b);
                            break;
                        }
                        case "*":
                            stack.Push(stack.Pop() * stack.Pop());
                            break;
                        case "-":
                            stack.Push(-stack.Pop());
                            break;
                        case "/":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(a / b);
                            break;
                        }
                        case "^":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push((float) Math.Pow(a, b));
                            break;
                        }
                        case "sin":
                            stack.Push((float) Math.Sin(stack.Pop()));
                            break;
                        case "cos":
                            stack.Push((float) Math.Cos(stack.Pop()));
                            break;
                        case "tg":
                            stack.Push((float) Math.Tan(stack.Pop()));
                            break;
                        case "lg":
                            stack.Push((float) Math.Log(stack.Pop(), 2));
                            break;
                        case "log":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push((float) Math.Log(a, b));
                            break;
                        }
                        default:
                            stack.Push(float.Parse(command, new CultureInfo("en-US")));
                            break;
                    }
                }

                return stack.Pop();
            };
        }

        private void DrawAll()
        {
            var drawer = Graphics.FromImage(_screen.Image);
            drawer.Clear(Color.White);
            DrawPolygon(new Model
            {
                Polygons =
                {
                    new Polygon(Color.Red, new List<Point3>
                    {
                        new Point3(-Max, 0, 0),
                        new Point3(Max, 0, 0)
                    })
                    {
                        Points = {0, 1}
                    },
                    new Polygon(Color.Blue, new List<Point3>
                    {
                        new Point3(0, -Max, 0),
                        new Point3(0, Max, 0)
                    })
                    {
                        Points = {0, 1}
                    },
                    new Polygon(Color.Green, new List<Point3>
                    {
                        new Point3(0, 0, -Max),
                        new Point3(0, 0, Max)
                    })
                    {
                        Points = {0, 1}
                    },
                    new Polygon(Color.Purple, new List<Point3>
                    {
                        RotationLine.from,
                        RotationLine.to,
                    })
                    {
                        Points = {0, 1}
                    },
                }
            }, drawer);
            DrawPolygon(_model, drawer);
            _screen.Image = _screen.Image;
        }

        private static int IntParse(string str, int def)
            => !int.TryParse(str, out var result) ? def : result;

        private TextBox ControlBox(int top, int idx)
        {
            var textBox = new TextBox
            {
                Left = ScreenWidth + 25 + 10 + 35 * idx,
                Width = 30,
                Height = 15,
                Top = top,
                Text = (idx != 2 ? 0 : top - 25).ToString(),
            };
            Controls.Add(textBox);
            return textBox;
        }
        
        private TextBox ControlBox2(int top, int idx)
        {
            var textBox = new TextBox
            {
                Left = ScreenWidth + 25 + 10 + 35 * idx,
                Width = 120,
                Height = 15,
                Top = top,
                Text = "x cos y sin +",
            };
            Controls.Add(textBox);
            return textBox;
        }
        
        private void DrawPolygon(Model model, Graphics drawer)
        {
            foreach (var polygon in model.Polygons)
                polygon
                    .Project(_projection[_currentProjection])
                    .Scale(new PointF(0, 0), (20, 20))
                    .Move(new PointF(_screen.Image.Width / 2f, _screen.Image.Height / 2f))
                    .Draw(drawer, polygon.Color);

        }
    }
}