using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Tools;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private readonly Bitmap _image;

        private readonly PolygonContainer polygonContainer = new PolygonContainer();

        private readonly Button AddPolygon;

        private PictureBox mainPicture;
        
        private readonly List<Button> _toolButtons = new List<Button>();
        
        private List<ITool> _tools = new List<ITool>();

        private int _currentTool;

        public Form(List<ITool> tools)
        {
            _tools = tools;
            Width = 612;
            Height = 638;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            mainPicture = new PictureBox
            {
                Width = 500,
                Height = 500,
                Top = 70,
                Left = 50,
                BackColor = Color.White,
            };
            mainPicture.MouseClick += (o, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _tools[_currentTool].OnUse(polygonContainer, e.Location);
                        break;
                    case MouseButtons.Right:
                        break;
                }

                DrawPolygons();
            };
            Controls.Add(mainPicture);
            
            MakeButtons();
        }
        
        private void MakeButtons()
        {
            _currentTool = -1;
            for (var i = 0; i < _tools.Count; i++)
            {
                var button = new Button
                {
                    BackColor = Color.White,
                    Width = 80,
                    Height = 20,
                    Top = 10 + i / 5 * 30,
                    Left = 100 + i % 5 * 100,
                    Text = _tools[i].Name(),
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    _tools[j].OnSelect(polygonContainer);
                    DrawPolygons();
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
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);
            var drawer = Graphics.FromImage(image);
            draw(drawer);
            drawer.Flush();
            mainPicture.Image = image;
        }

        private void Clear()
            => Draw(drawer => drawer.Clear(Color.White));
        
        private void DrawPolygons()
            => Draw(drawer =>
            {
                Clear();
                polygonContainer.Draw(drawer);
            });
    }
}