using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphic3D
{
    public partial class Form1 : Form
    {
        double fi = 80 * 0.0174532925;  
        double psi = 340 * 0.0174532925;
        Func<double, double, double> last;

        int upDown = 80;
        int leftRight = 340;

        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(700, 500); 

        }


        List<double> maxF;
        List<double> minF;

        public static Func<double, double, double> GetFunc(string func)
        {
            var list = func.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return (x, y) =>
            {
                var stack = new Stack<double>();
                foreach (var command in list)
                {
                    switch (command.ToLower())
                    {
                        case "x":
                            stack.Push(x);
                            break;
                        case "y":
                            stack.Push(y);
                            break;
                        case "e":
                            stack.Push((double)Math.E);
                            break;
                        case "pi":
                            stack.Push((double)Math.PI);
                            break;
                        case "+":
                            stack.Push(stack.Pop() + stack.Pop());
                            break;
                        case "--":
                            {
                                var b = stack.Pop();
                                var a = stack.Pop();
                                stack.Push(a - b);
                                break;
                            }
                        case "*":
                            stack.Push(stack.Pop() * stack.Pop());
                            break;
                        case "-":
                            stack.Push(-stack.Pop());
                            break;
                        case "/":
                            {
                                var b = stack.Pop();
                                var a = stack.Pop();
                                stack.Push(a / b);
                                break;
                            }
                        case "^":
                            {
                                var b = stack.Pop();
                                var a = stack.Pop();
                                stack.Push((float)Math.Pow(a, b));
                                break;
                            }
                        case "sin":
                            stack.Push((double)Math.Sin(stack.Pop()));
                            break;
                        case "cos":
                            stack.Push((double)Math.Cos(stack.Pop()));
                            break;
                        case "tg":
                            stack.Push((double)Math.Tan(stack.Pop()));
                            break;
                        case "lg":
                            stack.Push((double)Math.Log(stack.Pop(), 2));
                            break;
                        case "log":
                            {
                                var b = stack.Pop();
                                var a = stack.Pop();
                                stack.Push((double)Math.Log(a, b));
                                break;
                            }
                        default:
                            stack.Push(double.Parse(command, new CultureInfo("en-US")));
                            break;
                    }
                }

                return stack.Pop();
            };
        }

        void DrawGraphic(Func<double, double, double> func)
        {
            bmp = new Bitmap(700, 500);
            maxF = new List<double>(720);
            minF = new List<double>(720);

            List<Point> predLine = new List<Point>(40);
            List<Point> currentLine = new List<Point>(40);

            for (int i = 0; i < 720; i++)
            {
                maxF.Add(0);
                minF.Add(500);
            }

            Pen p = new Pen(Color.Indigo);
            for (double y = -6; y < 6; y += 0.2)
            {
                currentLine = new List<Point>(40);
                for (double x = -6; x < 6; x += 0.2)
                {
                    double z;

                    z = func(x, y);

                    double fx = x * Math.Cos(psi) - (-Math.Sin(fi) * y + Math.Cos(fi) * z) * Math.Sin(psi);
                    double fy = y * Math.Cos(fi) + z * Math.Sin(fi);
                    double fz = (-Math.Sin(fi) * y + Math.Cos(fi) * z);

                    int xx2 = (int)Math.Round(fx * 50 + (bmp.Width / 2));
                    int yy2 = (int)Math.Round(fy * 50 + (bmp.Height / 2) - 50);

                    currentLine.Add(new Point(xx2, yy2));

                    if (currentLine.Count > 1)
                        DrawLine(currentLine[currentLine.Count - 1], currentLine[currentLine.Count - 2]);

                    if (predLine.Count > 0)
                    {
                        DrawLine(currentLine[currentLine.Count - 1], predLine[currentLine.Count - 1]);
                        if (predLine.Count > 1 && currentLine.Count > 1)
                            DrawLine(currentLine[currentLine.Count - 2], predLine[currentLine.Count - 2]);
                    }

                }
                predLine = currentLine;
            }
            pictureBox1.Image = bmp;
        }

        void DrawLine(Point LineP1, Point LineP2)
        {
            var steep = Math.Abs(LineP2.Y - LineP1.Y) > Math.Abs(LineP2.X - LineP1.X);
            if (steep)
            {
                int k = LineP1.X;
                LineP1.X = LineP1.Y;
                LineP1.Y = k;

                k = LineP2.X;
                LineP2.X = LineP2.Y;
                LineP2.Y = k;
            }
            if (LineP1.X > LineP2.X)
            {
                int k = LineP1.X;
                LineP1.X = LineP2.X;
                LineP2.X = k;

                k = LineP1.Y;
                LineP1.Y = LineP2.Y;
                LineP2.Y = k;
            }

            float dx = LineP2.X - LineP1.X;
            float dy = LineP2.Y - LineP1.Y;

            float gradient = dy / dx;

            float y = LineP1.Y + gradient;

            for (int x = LineP1.X + 1; x < LineP2.X; x++)
            {
                int xx1 = steep ? (int)Math.Round(y) : x;
                int xx2 = steep ? (int)Math.Round(y) : x;
                int yy1 = steep ? x : (int)Math.Round(y);
                int yy2 = steep ? x : (int)Math.Round(y + 1);

                xx1 = Math.Max(Math.Min(xx1, bmp.Width - 1), 0);
                xx2 = Math.Max(Math.Min(xx2, bmp.Width - 1), 0);
                yy1 = Math.Max(Math.Min(yy1, bmp.Height - 1), 0);
                yy2 = Math.Max(Math.Min(yy2, bmp.Height - 1), 0);

                if ((yy1 >= maxF[xx1] && yy2 >= maxF[xx2]))
                {
                    bmp.SetPixel(xx2, yy2, Color.Blue);
                    maxF[xx1] = yy1;
                    maxF[xx2] = yy2;
                }
                if (yy1 <= minF[xx1] && yy2 <= minF[xx2])
                {
                    bmp.SetPixel(xx1, yy1, Color.Red);
                    bmp.SetPixel(xx2, yy2, Color.Red);
                    minF[xx1] = yy1;
                    minF[xx2] = yy2;

                }
                maxF[xx1] = Math.Max(yy1, maxF[xx1]);
                minF[xx1] = Math.Min(yy1, minF[xx1]);
                maxF[xx2] = Math.Max(yy2, maxF[xx2]);
                minF[xx2] = Math.Min(yy2, minF[xx2]);

                y = y + gradient;

            }
        }

        private static TextBox ControlBox(int top, int idx, int width = 30, string defaultValue = "0")
        {
            var textBox = new TextBox
            {
                Left = 200,
                Width = width,
                Height = 400,
                Top = top,
                Text = defaultValue,
            };

            return textBox;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            upDown += 10;
            fi = upDown * 0.0174532925;
            DrawGraphic(last);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            upDown -= 10;
            fi = upDown * 0.0174532925;
            DrawGraphic(last);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            leftRight -= 10;
            psi = leftRight * 0.0174532925;

            DrawGraphic(last);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            leftRight += 10;
            psi = leftRight * 0.0174532925;

            DrawGraphic(last);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                button2_Click_1(null, null);
            else if (e.KeyCode == Keys.D)
                button3_Click(null, null);
            else if (e.KeyCode == Keys.W)
                button1_Click_1(null, null);
            else if (e.KeyCode == Keys.S)
                button4_Click(null, null);

        }
    }
}
