﻿using System;
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

        private const int PointPanelWidth = 100;
        
        private const float Max = 20;
        
        private PictureBox _screen;
        
        private readonly List<String> _models = new List<string>
        {
            "Models/Cube.obj",
            "Models/Skull.obj",
            "Models/Prism.obj",
            "Models/Cat.obj",
            "Models/Tetrahedron.obj",
            "Models/Octahedron.obj",
            "Models/Dodecahedron.obj.obj",
            "Models/Icosahedron.obj",
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

        private (Point3 from, Point3 to) RotationLine = (new Point3(0, 0, 0), new Point3(0, 0, 75));
        
        public Form()
        {
            KeyPreview = true;
            BackColor = Color.Beige;
            Width = ScreenWidth + PointPanelWidth + 50 + 19;
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
            var x1Field = ControlBox(25, 0);
            var y1Field = ControlBox(25, 1);
            var z1Field = ControlBox(25, 2);
            var x2Field = ControlBox(100, 0);
            var y2Field = ControlBox(100, 1);
            var z2Field = ControlBox(100, 2);
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
                    case Keys.Z:
                        Console.WriteLine("Z");
                        _model.RotateLine(RotationLine.from, RotationLine.to, (float)Math.PI/12);
                        break;
                    case Keys.X:
                        Console.WriteLine("X");
                        _model.RotateLine(RotationLine.from, RotationLine.to, -(float)Math.PI/12);
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
                    new Polygon(Color.Purple)
                    {
                        Points =
                        {
                            RotationLine.from, 
                            RotationLine.to,
                        }
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
                Left = ScreenWidth + 25 + 10 + 25 * idx,
                Width = 20,
                Height = 15,
                Top = top,
                Text = (idx != 2 ? 0 : top - 25).ToString(),
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