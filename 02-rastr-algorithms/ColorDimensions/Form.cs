using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Tools;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private readonly Bitmap _image;

        private readonly Button _colorPicker;

        private readonly List<Button> _toolButtons = new List<Button>();
        
        private readonly List<ITool> _tools;
        
        private int _currentTool;

        private void MakeButtons()
        {
            for (var i = 0; i < _tools.Count; i++)
            {
                var button = new Button
                {
                    BackColor = Color.White,
                    Width = 70,
                    Height = 20,
                    Top = 15,
                    Left = 100 + i * 100,
                    Text = _tools[i].Name(),
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    _toolButtons[_currentTool].BackColor = Color.White;
                    _tools[_currentTool].Stop();
                    _currentTool = j;
                    _toolButtons[_currentTool].BackColor = Color.Aquamarine;
                };
                _toolButtons.Add(button);
                Controls.Add(button);
            }
            _currentTool = 0;
            _toolButtons[_currentTool].BackColor = Color.Aquamarine;
        }

        public Form(List<ITool> tools)
        {
            _tools = tools;
            
            Width = 612;
            Height = 638;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.Bisque;
            
            var mainPicture = new PictureBox()
            {
                Width = 500,
                Height = 500,
                Top = 50,
                Left = 50,
            };
            mainPicture.MouseClick += (o, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _tools[_currentTool].Draw(_image, e.Location, _colorPicker.BackColor);
                        mainPicture.Image = _image;
                        break;
                    default:
                        _tools[_currentTool].Stop();
                        break;
                }
            };
            
            Controls.Add(mainPicture);

            _colorPicker = new Button()
            {
                Width = 50,
                Height = 30,
                Top = 10,
                Left = 10,
                BackColor = Color.Black,
            };
            _colorPicker.Click += (o, e) =>
            {
                var dialog = new ColorDialog
                {
                    AllowFullOpen = false,
                    ShowHelp = true,
                    Color = _colorPicker.BackColor
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                    _colorPicker.BackColor = dialog.Color;
            };
            Controls.Add(_colorPicker);
            
            _image = new Bitmap(500, 500);
            var drawer = Graphics.FromImage(_image);
            drawer.FillRectangle(new SolidBrush(Color.White), 0, 0, 500, 500);
            mainPicture.Image = _image;
            MakeButtons();
        }
    }
}