using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using GraphFunc.Drawers;
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

        private readonly List<string> _models = new List<string>
        {
            //"Models/Cube.obj",
            "Models/Square.obj",
            //"Models/Tetrahedron.obj",
            //"Models/Hexahedron.obj",
            //"Models/Octahedron.obj",
            //"Models/Dodecahedron.obj",
            //"Models/Icosahedron.obj",
            //"Models/Skull.obj",
            //"Models/Prism.obj",
            //"Models/Cat.obj",
        };

        private int _currentModel;

        private Model _model;

        private readonly List<IProjection> _projection = new List<IProjection>
        {
            new ProjectionPerspective(),
            new ProjectionIsometric(),
           //new ProjectionOrthographic(Axis.Z),
            //new ProjectionOrthographic(Axis.Y),
            //new ProjectionOrthographic(Axis.X),
        };

        private int _currentProjection;
        
        private IDrawer _drawer = new StickDrawer();

        private (Point3 from, Point3 to) RotationLine = (new Point3(0, 0, 0), new Point3(0, 0, 75));

        public Form()
        {
            _model = Model.LoadFromObj(File.ReadLines(_models[_currentModel]), _models[_currentModel]);
            
            KeyPreview = true;
            Width = ScreenWidth + PointPanelWidth + 50 + 19;
            Height = ScreenHeight + 50 + 27;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.Beige;
            Text = _model.Name;

            AddScreen();
            ControlTools();
            controlPanel();

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
            KeyDown += (sender, args) =>
            {
                switch (args.KeyCode)
                {
                    case Keys.W:
                        ProjectionPerspective.Projector.Move(Axis.Z, 0.5f);
                        break;
                    case Keys.S:
                        ProjectionPerspective.Projector.Move(Axis.Z, -0.5f);
                        break;
                    case Keys.A:
                        ProjectionPerspective.Projector.Move(Axis.X, -0.5f);
                        break;
                    case Keys.D:
                        ProjectionPerspective.Projector.Move(Axis.X, 0.5f);
                        break;
                    case Keys.Q:
                        _model.Move(new Point3(0, 0, -1));
                        break;
                    case Keys.E:
                        _model.Move(new Point3(0, 0, 1));
                        break;
                    case Keys.Left:
                        ProjectionPerspective.Projector.Rotate(Axis.Y, -(float)Math.PI/100);
                        break;
                    case Keys.Right:
                        ProjectionPerspective.Projector.Rotate(Axis.Y, (float)Math.PI/100);
                        break;
                    case Keys.Up:
                        ProjectionPerspective.Projector.Rotate(Axis.X, -(float)Math.PI/100);
                        break;
                    case Keys.Down:
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

        private void DrawAll()
        {
            var coordinates = new Model();
            coordinates.Points = new List<Point3>
            {
                new Point3(-Max, 0, 0),
                new Point3(Max, 0, 0),
                new Point3(0, -Max, 0),
                new Point3(0, Max, 0),
                new Point3(0, 0, -Max),
                new Point3(0, 0, Max),
                RotationLine.from,
                RotationLine.to,
            };
            coordinates.Polygons = new List<Polygon>
            {
                new Polygon(Color.Red)
                {
                    Points = {0, 1}
                },
                new Polygon(Color.Blue)
                {
                    Points = {2, 3}
                },
                new Polygon(Color.Green)
                {
                    Points = {4, 5}
                },
                new Polygon(Color.Purple)
                {
                    Points = {6, 7}
                },
            };
            DrawModels(new[]
            {
                coordinates,
                _model,
            });
        }

        private void DrawModels(IEnumerable<Model> models)
        {
            var image = new Bitmap(_screen.Width, _screen.Height);
            var drawer = Graphics.FromImage(image);
            _drawer.Draw(drawer, new Point(_screen.Width, _screen.Height), models, _projection[_currentProjection]);
            _screen.Image = image;
        }

        private static TextBox ControlBox(int top, int idx, int width = 30, string defaultValue = "0")
        {
            var textBox = new TextBox
            {
                Left = ScreenWidth + 25 + 10 + 35 * idx,
                Width = width,
                Height = 15,
                Top = top,
                Text = defaultValue,
            };

            return textBox;
        }

        private Panel graphicPanel()
        {
            var panel = new Panel
            {
                Left = 25,
                Width = 919 - 50,
                Height = 827 - 100,
                Top = 45,
                Visible = true,
            };

            var x0FieldGraphic = ControlBox(25, 0);
            var x1FieldGraphic = ControlBox(25, 1);
            var y0FieldGraphic = ControlBox(75, 0);
            var y1FieldGraphic = ControlBox(75, 1);
            var stepFieldGraphic = ControlBox(125, 0);
            var funcFieldGraphic = ControlBox(175, 0, 400, "x cos y sin +");
            panel.Controls.Add(x0FieldGraphic);
            panel.Controls.Add(x1FieldGraphic);
            panel.Controls.Add(y0FieldGraphic);
            panel.Controls.Add(y1FieldGraphic);
            panel.Controls.Add(stepFieldGraphic);
            panel.Controls.Add(funcFieldGraphic);

            var createButton = new Button
            {
                Left = panel.Width - 120,
                Top = panel.Height - 500,
                Text = "Draw",
                Width = 120,
                Height = 25,

            };

            createButton.Click += (sender, args) =>
            {
                double x0, x1, y0, y1, step;
                Double.TryParse(x0FieldGraphic.Text, out x0);
                Double.TryParse(x1FieldGraphic.Text, out x1);
                Double.TryParse(y0FieldGraphic.Text, out y0);
                Double.TryParse(y1FieldGraphic.Text, out y1);
                Double.TryParse(stepFieldGraphic.Text, out step);

               _model = Model.MakeGraphic(GraphFunc.Utils.GetFunc(funcFieldGraphic.Text), (float)x0, (float)y0, (float)x1, (float)y1, (float)step);
            };

            panel.Controls.Add(createButton);
            return panel;
        }

        private Panel rotatePanel()
        {
            var panel = new Panel
            {
                Left = 25,
                Width = 919 - 50,
                Height = 827 - 100,
                Top = 45,
                Visible = true,
            };
            var x1Field = ControlBox(25, 0);
            var y1Field = ControlBox(25, 1);
            var z1Field = ControlBox(25, 2);
            var x2Field = ControlBox(100, 0);
            var y2Field = ControlBox(100, 1);
            var z2Field = ControlBox(100, 2);
          
            panel.Controls.Add(x1Field);
            panel.Controls.Add(y1Field);
            panel.Controls.Add(z1Field);
            panel.Controls.Add(x2Field);
            panel.Controls.Add(x1Field);
            panel.Controls.Add(y2Field);
            panel.Controls.Add(z2Field);

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

            panel.Controls.Add(axisSpin);


            return panel;
        }

        private Panel solidOfRevolutionPanel()
        {
            var panel = new Panel
            {
                Left = 25,
                Width = 919 - 50,
                Height = 827 - 100,
                Top = 45,
                Visible = true,
            };

            var segmentsSpin = ControlBox(25, 0);

            panel.Controls.Add(segmentsSpin);

            var axisSpin = new CheckedListBox()
            {
                Left = ScreenWidth + 25 + 10,
                Width = 100,
                Height = 50,
                Top = 80,
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

            panel.Controls.Add(axisSpin);

            var createButton = new Button
            {
                Left = panel.Width - 120,
                Top = panel.Height - 500,
                Text = "Draw",
                Width = 120,
                Height = 25,

            };

            createButton.Click += (sender, args) =>
            {
                Console.WriteLine("points - " + _model.Points.Count + " poly - " + _model.Polygons.Count);
                if (_model.Polygons.Count == 1)
                {
                    Axis ax;

                    if (axisSpin.CheckedItems[0].ToString() == "X")
                        ax = Axis.X;
                    else if (axisSpin.CheckedItems[0].ToString() == "Y")
                        ax = Axis.Y;
                    else
                        ax = Axis.Z;
                    Console.WriteLine(axisSpin.CheckedItems[0].ToString());
                    _model = _model.MakeSpinObj(_model, ax,
                        int.Parse(segmentsSpin.Text, 0));
                }
            };

            panel.Controls.Add(createButton);

            return panel;
        }

        private int _currentTool = 0;

        private readonly List<Button> _toolButtons = new List<Button>();

        private void clearControls(List<(Panel, String)> tools)
        {
            foreach (var x in tools)
                Controls.Remove(x.Item1);
        }


        public void controlPanel()
        {
            List<(Panel, String)> _tools = new List<(Panel, String)>()
                {
                    (graphicPanel(), "Draw func"),
                    (rotatePanel(), "Rotate"),
                    (solidOfRevolutionPanel(), "Spin obj")
                };

            for (var i = 0; i < _tools.Count; i++)
            {               
                var button = new Button
                {
                    BackColor = Color.White,
                    Width = 150,
                    Height = 20,
                    Top = 10,
                    Left = 30 + i * 160,
                    Text = _tools[i].Item2,
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    _toolButtons[_currentTool].BackColor = Color.White;
                    _currentTool = j;
                   // Console.WriteLine(_currentTool);
                    clearControls(_tools);
                    Controls.Add(_tools[_currentTool].Item1);
                    _toolButtons[_currentTool].BackColor = Color.Aquamarine;
                    
                };

                Controls.Add(_tools[_currentTool].Item1);
                _toolButtons.Add(button);
                Controls.Add(button);
            }
            _toolButtons[_currentTool].BackColor = Color.Aquamarine;
        }
    }
    
}

