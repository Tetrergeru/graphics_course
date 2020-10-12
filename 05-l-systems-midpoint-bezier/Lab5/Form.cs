using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Tools;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        private readonly PolygonContainer _polygonContainer = new PolygonContainer();

        private readonly List<Button> _toolButtons = new List<Button>();
        
        private readonly List<Panel> _toolPanels = new List<Panel>();
        
        private readonly List<ITool> _tools;

        private int _currentTool;

        public Form(List<ITool> tools)
        {
            _tools = tools;
            Width = 700;
            Height = 700;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            MakeButtons();
        }
        
        private void MakeButtons()
        {
            for (var i = 0; i < _tools.Count; i++)
            {
                var panel = new Panel
                {
                    Left = 25,
                    Width = Width - 50,
                    Height = Height - 100,
                    Top = 45,
                };
                _tools[i].Add(panel);
                var button = new Button
                {
                    BackColor = Color.White,
                    Width = 150,
                    Height = 20,
                    Top = 10 + i * 30,
                    Left = 30 + i * 100,
                    Text = _tools[i].ToString(),
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    _toolPanels[_currentTool].Visible = false;
                    _toolButtons[_currentTool].BackColor = Color.White;
                    _currentTool = j;
                    _toolButtons[_currentTool].BackColor = Color.Aquamarine;
                    _toolPanels[_currentTool].Visible = true;
                };
                _toolButtons.Add(button);
                _toolPanels.Add(panel);
                Controls.Add(button);
                Controls.Add(panel);
            }
            _toolButtons[_currentTool].BackColor = Color.Aquamarine;
        }
    }
}