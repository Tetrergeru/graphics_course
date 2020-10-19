using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private const int ScreenWidth = 1000;
        
        private const int ScreenHeight = 1000;
        
        private const float Max = 100000;
        
        private PictureBox _screen;
        
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
                Height = ScreenWidth,
                Width = ScreenHeight,
            };
            _screen.Image = new Bitmap(_screen.Width, _screen.Height);
            _screen.Click += (sender, args) =>
            {
                _currentProjection = (_currentProjection + 1) % _projection.Count;
                DrawAll();
            };
            
            Controls.Add(_screen);

            ControlTools();
            
            DrawAll();
        }

        private void ControlTools()
        {
            var scrollBar = new VScrollBar
            {
                Left = 10,
                Top = 25,
                Width = 15,
                Height = 1000,
                Minimum = 10,
                Maximum = 50,
                Value = 10,
            };
            scrollBar.Scroll += (sender, args) =>
            {
                ProjectionPerspective.Matrix = Matrix3d
                    .One
                    .Rotate(Axis.X, Math.PI / 2 + Math.PI / 12)
                    .Rotate(Axis.Y, -Math.PI / 12)
                    .Move(new Point3(0, 0, scrollBar.Value));
                DrawAll();
            };
            Controls.Add(scrollBar);
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
                    .Scale(new PointF(0, 0), (30, 30))
                    .Move(new PointF(_screen.Image.Width / 2f, _screen.Image.Height / 2f))
                    .Draw(drawer, polygon.Color);

        }
    }
}