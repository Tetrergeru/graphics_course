using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        
        private const float Max = 20;
        
        private PictureBox _screen;
        
        private readonly List<String> _models = new List<string>
        {
            "Models/Cube.obj",
            "Models/Skull.obj",
            "Models/Prism.obj",
            "Models/Cat.obj",
        };
        
        private int _currentModel;
        
        private Model _model = Model.LoadFromObj(File.ReadLines("Models/Cube.obj"));
        
        private readonly List<IProjection> _projection = new List<IProjection>
        {
            new ProjectionPerspective(),
            new ProjectionIsometric(),
            new ProjectionOrthographic(Axis.Z),
            new ProjectionOrthographic(Axis.Y),
            new ProjectionOrthographic(Axis.X),
        };

        private int _currentProjection;
        
        public Form()
        {
            BackColor = Color.Beige;
            Width = ScreenWidth + 50 + 19;
            Height = ScreenHeight + 50 + 27;
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
                    _model = Model.LoadFromObj(File.ReadLines(_models[_currentModel]));
                }
                DrawAll();
            };
            
            ControlTools();
            
            Controls.Add(_screen);

            DrawAll();
        }

        private void ControlTools()
        {
            KeyUp += (sender, args) =>
            {
                switch (args.KeyCode)
                {
                    case Keys.W:
                        _model.Move(new Point3(0, -1, 0));
                        break;
                    case Keys.S:
                        _model.Move(new Point3(0, 1, 0));
                        break;
                    case Keys.A:
                        _model.Move(new Point3(-1, 0, 0));
                        break;
                    case Keys.D:
                        _model.Move(new Point3(1, 0, 0));
                        break;
                    case Keys.Q:
                        _model.Move(new Point3(0, 0, -1));
                        break;
                    case Keys.E:
                        _model.Move(new Point3(0, 0, 1));
                        break;
                    case Keys.Left:
                        _model.Rotate(Axis.Y, (float)Math.PI/12);
                        break;
                    case Keys.Right:
                        _model.Rotate(Axis.Y, -(float)Math.PI/12);
                        break;
                    case Keys.Up:
                        _model.Rotate(Axis.X, (float)Math.PI/12);
                        break;
                    case Keys.Down:
                        _model.Rotate(Axis.X, -(float)Math.PI/12);
                        break;
                    case Keys.PageUp:
                        _model.Rotate(Axis.Z, (float)Math.PI/12);
                        break;
                    case Keys.PageDown:
                        _model.Rotate(Axis.Z, -(float)Math.PI/12);
                        break;
                    case Keys.F1:
                        _model.Scale(1.1f);
                        break;
                    case Keys.F2:
                        _model.Scale(0.9f);
                        break;
                }
                DrawAll();
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
                    new Polygon(Color.Red)
                    {
                        Points =
                        {
                            new Point3(-Max, 0, 0), 
                            new Point3(Max, 0, 0)
                        }
                    },
                    new Polygon(Color.Blue)
                    {
                        Points =
                        {
                            new Point3(0, -Max,  0), 
                            new Point3(0, Max, 0)
                        }
                    },
                    new Polygon(Color.Green)
                    {
                        Points =
                        {
                            new Point3(0, 0, -Max), 
                            new Point3(0, 0, Max)
                        }
                    },
                }
            }, drawer);
            DrawPolygon(_model, drawer);
            _screen.Image = _screen.Image;
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