using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GraphFunc.Menus
{
    public class RGBHSVMenu : IMenu
    {
        private static Bitmap Adjust(Bitmap image, int h, int s, int v)
        {
            // TODO: Чего там Демьяненко от нас хочет.
            return image;
        }

        private PictureBox ResultPicture;

        private TextBox[] ParamContollers; 
        
        public RGBHSVMenu()
        {
            ResultPicture = new PictureBox()
            {
                Width = 256,
                Height = 256,
                Top = 376,
                Left = 200,
            };
            ParamContollers = new TextBox[3];
            for (var i = 0; i < 3; i++)
            {
                var text = new TextBox()
                {
                    Text = "0",
                    Width = 50,
                    Left = 50,
                    Top = 380 + i * 40,
                };
                ParamContollers[i] = text;
            }
        }

        public void Add(Form form)
        {
            form.Controls.Add(ResultPicture);

            for (var i = 0; i < 3; i++)
            {
                form.Controls.Add(ParamContollers[i]);
            }

            Update(form);
        }

        public void Update(Form form)
        {
            ResultPicture.Image = Adjust(form.image,
                int.Parse(ParamContollers[0].Text),
                int.Parse(ParamContollers[1].Text),
                int.Parse(ParamContollers[2].Text)).Scale(256, 256);
        }

        public void Remove(Form form)
        {
            form.Controls.Remove(ResultPicture);

            for (var i = 0; i < 3; i++)
            {
                form.Controls.Remove(ParamContollers[i]);
            }
        }

        public string Name() => "RGB/HSV";
    }
}