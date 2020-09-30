using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.DrawingTool;
using GraphFunc.Tools;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private readonly PolygonContainer _polygonContainer = new PolygonContainer();

        private readonly PictureBox _mainPicture;
        
        private readonly List<Button> _toolButtons = new List<Button>();
        
        private readonly List<IDrawingTool> _tools;

        private int _currentTool;

        public Form(List<IDrawingTool> tools)
        {
            _tools = tools;
            Width = 612;
            Height = 638;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            _mainPicture = new PictureBox
            {
                Width = 500,
                Height = 500,
                Top = 70,
                Left = 50,
                BackColor = Color.White,
            };
            _mainPicture.MouseDown +=
                (o, e) =>
                    Draw(drawer =>
                        _tools[_currentTool].OnMouseDown(e.Location, drawer));
            _mainPicture.MouseUp +=
                (o, e) =>
                    Draw(drawer =>
                        _tools[_currentTool].OnMouseUp(e.Location, drawer));
            _mainPicture.MouseMove +=
                (o, e) =>
                    Draw(drawer =>
                        _tools[_currentTool].OnMouseMove(e.Location, drawer));
            Controls.Add(_mainPicture);
            
            MakeButtons();
        }
        
        private void MakeButtons()
        {
            _currentTool = -1;
            for (var i = 0; i < _tools.Count; i++)
            {
                _tools[i].Init(_polygonContainer);
                var button = new Button
                {
                    BackColor = Color.White,
                    Width = 80,
                    Height = 20,
                    Top = 10 + i / 6 * 30,
                    Left = 10 + i % 6 * 100,
                    Text = _tools[i].ToString(),
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    Draw(drawer => _tools[j].OnSelect(drawer));
                    if (!_tools[j].CanUseInField())
                        return;
                    _toolButtons[_currentTool].BackColor = Color.White;
                    _currentTool = j;
                    _toolButtons[_currentTool].BackColor = Color.Aquamarine;
                };
                _toolButtons.Add(button);
                Controls.Add(button);
                if (_currentTool == -1 && _tools[i].CanUseInField())
                    _currentTool = i;
            }
            _toolButtons[_currentTool].BackColor = Color.Aquamarine;
        }
        
        private void Draw(Action<Graphics> draw)
        {
            var image = new Bitmap(_mainPicture.Width, _mainPicture.Height);
            var drawer = Graphics.FromImage(image);
            draw(drawer);
            drawer.Flush();
            _mainPicture.Image = image;
        }
    }
}