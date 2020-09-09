using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphFunc.Menus
{
    public class ShadesOfGrayMenu : IMenu
    {
        private static Bitmap GrayShade1(Bitmap b)
        {
            // TODO: First gray shader
            return b;
        }
        
        private static Bitmap GrayShade2(Bitmap b)
        {
            // TODO: Second gray shader
            return b;
        }
        
        private static Bitmap GrayDiff(Bitmap b1, Bitmap b2)
        {
            // TODO: Gray diff shower
            return b1;
        }

        private static int[] CalcGrayIntensity(Bitmap image)
        {
            var result = new int[256];
            for (var i = 0; i < image.Width; i++)
            for (var j = 0; j < image.Height; j++)
                result[image.GetPixel(i, j).R]++;
            return result;
        }

        private readonly PictureBox _gray1, _gray2, _plot1, _plot2, _diff;
        
        public ShadesOfGrayMenu()
        {
            _gray1 = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 376,
                Left = 50,
            };
            _plot1 = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 652,
                Left = 50,
            };
            _gray2 = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 376,
                Left = 356,
            };
            _plot2 = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 652,
                Left = 356,
            };
            _diff = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 376,
                Left = 662,
            };
        }

        public void Add(Form form)
        {
            Console.WriteLine("Adding");
            form.Controls.Add(_gray1);
            form.Controls.Add(_gray2);
            form.Controls.Add(_plot1);
            form.Controls.Add(_plot2);
            form.Controls.Add(_diff);
            Update(form);
        }

        public void Update(Form form)
        {
            var g1 =  GrayShade1(form.image).Scale(256, 256);
            _gray1.Image = g1;
            _plot1.Image = Program.DrawPlot(CalcGrayIntensity(g1), Color.Gray, 256);
            var g2 =  GrayShade2(form.image).Scale(256, 256);
            _gray2.Image = g2;
            _plot2.Image = Program.DrawPlot(CalcGrayIntensity(g2), Color.Gray, 256);
            _diff.Image = GrayDiff(g1, g2).Scale(256, 256);
        }

        public void Remove(Form form)
        {
            form.Controls.Remove(_plot1);
            form.Controls.Remove(_plot2);
            form.Controls.Remove(_gray1);
            form.Controls.Remove(_gray2);
            form.Controls.Remove(_diff);
        }

        public string Name() => "Grays";
    }
}