using System.Windows;
using System.Drawing;
using System.Linq;
using System;
using System.Numerics;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;

namespace JuliaGen
{ public partial class MainWindow : Window
    {
        int maxIteration = 100;
        int h = 100;
        int w = 100;
        double rmin = -2;
        double rmax = 2;
        double imin = -2;
        double imax = 2;
        double bound = 2;
        int colourFactor = 0;
        int fr = 0;
        int fg = 0;
        int fb = 0;
        Polynomial func = new Polynomial();
        Bitmap image;
        public MainWindow()
        {
            InitializeComponent();

            //start
            readFormula(tbFormulaInput, null);
            renderFormula();
        }

        public void GenerateSet()
        {

            var calcpoints = Enumerable.Range(0, w * h).AsParallel().Select(n => getPoint(n)); //Calculate the escape time for each point

            image = new Bitmap(w, h);
            foreach (var c in calcpoints)
            {
                image.SetPixel(c.x, c.y, getColour(c.i));
            }
            var img = BitmapToImageSource(image);
            Screen.Source = img;
        }

        private Color getColour(int i)
        {
            if (i == -1)
            {
                return Color.Black;
            }
            else
            {
                i *= colourFactor;
                int r = Math.Abs(Math.Max(((i + fr) % 512), 1) - 256);
                int g = Math.Abs(Math.Max(((i + fg) % 512), 1) - 256);
                int b = Math.Abs(Math.Max(((i + fb) % 512), 1) - 256);

                return Color.FromArgb(r, g, b);
            }
        }

        private CalculatedPoint getPoint(int n)
        {
            //Converts pixel coord to corresponding z;
            int x = n % w;
            int y = n / w;
            var r = rmin + ((double)x / w) * (rmax - rmin);
            var i = imin + ((double)y / h) * (imax - imin);
            return new CalculatedPoint { x = x, y = y, i = get(new Complex(r, i)) };

        }

        int get(Complex z)
        {
            int iteration = 0;
            try
            {
                while (z.Real * z.Imaginary < bound * bound)
                {
                    z = func.calculate(z);
                    if (iteration >= maxIteration)
                    {
                        iteration = -1;
                        break;
                    }
                    ++iteration;
                }
            }
            catch (OverflowException)
            {
                return iteration;
            }
            return iteration;
        }

        struct CalculatedPoint
        {
            internal int x;
            internal int y;
            internal int i;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void generate(object sender, RoutedEventArgs e)
        {
            GenerateSet();
            btnSave.IsEnabled = true;
        }

        private void save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Portable Network Graphics (*.png)|*png";
            d.FileName = "julia set.png";
            if (d.ShowDialog() == true)
            {
                image.Save(d.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void vbound(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            double t;
            if (double.TryParse(((TextBox)sender).Text, out t) && t > 0)
            {
                bound = t;
            }
            else
            {
                ((TextBox)sender).Text = bound.ToString();
            }
        }

        private void vmaxiter(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t) && t > 0)
            {
                maxIteration = t;
            }
            else
            {
                ((TextBox)sender).Text = maxIteration.ToString();
            }
        }
        private void vrmin(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            double t;
            if (double.TryParse(((TextBox)sender).Text, out t) && t < rmax)
            {
                rmin = t;
            }
            else
            {
                ((TextBox)sender).Text = rmin.ToString();
            }
        }
        private void vrmax(object sender, RoutedEventArgs e)
        {
            double t;
            if (double.TryParse(((TextBox)sender).Text, out t) && t > rmin)
            {
                rmax = t;
            }
            else
            {
                ((TextBox)sender).Text = rmax.ToString();
            }
        }
        private void vimin(object sender, RoutedEventArgs e)
        {
            double t;
            if (double.TryParse(((TextBox)sender).Text, out t) && t < imax)
            {
                imin = t;
            }
            else
            {
                ((TextBox)sender).Text = imin.ToString();
            }
        }
        private void vimax(object sender, RoutedEventArgs e)
        {
            double t;
            if (double.TryParse(((TextBox)sender).Text, out t) && t > imin)
            {
                imax = t;
            }
            else
            {
                ((TextBox)sender).Text = imax.ToString();
            }
        }

        private void vwidth(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t) && t > 0)
            {
                w = t;
            }
            else
            {
                ((TextBox)sender).Text = w.ToString();
            }
        }

        private void vheight(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t) && t > 0)
            {
                h = t;
            }
            else
            {
                ((TextBox)sender).Text = h.ToString();
            }
        }

        private void vcolour(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t))
            {
                colourFactor = t;
            }
            else
            {
                ((TextBox)sender).Text = colourFactor.ToString();
            }
        }
        private void vred(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t))
            {
                fr = t;
            }
            else
            {
                ((TextBox)sender).Text = fr.ToString();
            }
        }
        private void vgreen(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t))
            {
                fg = t;
            }
            else
            {
                ((TextBox)sender).Text = fg.ToString();
            }
        }
        private void vblue(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            int t;
            if (int.TryParse(((TextBox)sender).Text, out t))
            {
                fb = t;
            }
            else
            {
                ((TextBox)sender).Text = fb.ToString();
            }
        }

        private void readFormula(object sender, TextChangedEventArgs e)
        {
            Dictionary<int, Complex> newPoly = new Dictionary<int, Complex>();
            var box = (TextBox)sender;
            var factors = box.Text.Split('|');
            foreach (var factor in factors)
            {
                factor.Trim(' ');
                var nums = factor.Split(' ');
                if (nums.Length == 3)
                {
                    double a;
                    double b;
                    int f;
                    if (double.TryParse(nums[0], out a) && double.TryParse(nums[1], out b) && int.TryParse(nums[2], out f) && f >= 0)
                    {
                        var constant = new Complex(a, b);
                        if (newPoly.ContainsKey(f))
                        {
                            newPoly[f] = newPoly[f] + constant;
                        }
                        else
                        {
                            newPoly.Add(f, constant);
                        }
                    }
                }
                if (newPoly.Count != 0)
                {
                    func.factors = newPoly;
                }
                renderFormula();
            }
        }

        private void renderFormula()
        {
            List<int> exponents = new List<int>();
            exponents.AddRange(func.factors.Keys);
            exponents.Sort();
            var text = "";
            for (int i = exponents.Count - 1; i >= 0; --i)
            {
                int f = exponents[i];
                var c = func.factors[f];
                var a = c.Real;
                var b = c.Imaginary;
                var constText = "";
                if (i != exponents.Count-1)
                {
                    text += " + ";
                }
                if (b == 0) //just real
                {
                    if (a != 1) 
                    {
                        constText = a.ToString();
                    }
                    //else nothing
                }
                else
                {
                    if (a == 0)//no real, no parenthes
                    {
                        if (b != 1)//dont write 1
                        {
                            constText += b.ToString();
                        }
                        constText += "i";
                    }
                    else //both real and imag, so with parentheses
                    {
                        constText += "("+ a.ToString() + " + ";
                        //imaginary part
                        if (b != 1)//dont write 1
                        {
                            constText += b.ToString();
                        }
                        constText += "i)";
                    }

                }
                text += constText;

                if (f!=0)//ignore z if it is a constant;
                {
                    text += "z";
                    if (f != 1)//add expo
                    {
                        text += "^" + f;
                    }
                }
                
            }
            if (text==string.Empty)//add 0 as nothing
            {
                text = "0";
            }
            text = text.Insert(0, "f(z) = ");
            lbFormulaFormatted.Content = text;
        }
    }
}
