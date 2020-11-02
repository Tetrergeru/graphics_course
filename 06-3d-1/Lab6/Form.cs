using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            "Models/Cube.obj",
            //"Models/Square.obj",
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
            //new ProjectionIsometric(),
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
    }
}