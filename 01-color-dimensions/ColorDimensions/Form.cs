using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using GraphFunc.Menus;

namespace GraphFunc
{
    public class Form : System.Windows.Forms.Form
    {
        public Bitmap image;
        
        private readonly List<IMenu> _menus;

        private IMenu currentMenu;

        private PictureBox MainPictre;
        
        private void MakeButtons()
        {
            for(var i = 0; i < _menus.Count; i++)
            {
                var button = new Button
                {
                    Text = _menus[i].Name(),
                    Width = 50,
                    Height = 30,
                    Left = 50 + i * 100,
                    Top = 326,
                };
                var j = i;
                button.Click += (o, e) =>
                {
                    if (currentMenu == _menus[j])
                        return;
                    currentMenu.Remove(this);
                    Console.WriteLine($"{j}");
                    _menus[j].Add(this);
                    currentMenu = _menus[j];
                };
                Controls.Add(button);
            }
        }

        public Form(List<IMenu> menus)
        {
            Width = 1000;
            Height = 1000;
            _menus = menus;

            MainPictre = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 50,
                Left = 50,
            };
            Controls.Add(MainPictre);
            MakeButtons();
            
            image = new Bitmap("image-1.jpeg");
            currentMenu = _menus[0];
            _menus[0].Add(this);
            MainPictre.Image = image.Scale(256, 256);
        }
    }
}