using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cg1
{
    class ContrastEnhancementFilter : IFilter
    {
        private string name = "Contrast Enhancement";
        // changable variables
        private const double ALPHA = 1.5;
        private const double BETA = 60;
        public PointCollection Pts
        {
            get;
            set;
        }
        public string Name
        {
            get { return name; }
            set => name = value;
        }
        public void Filter(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
               ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* bytes = (byte*)data.Scan0;
                var channelSize = 3;
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            double newVal = ALPHA * row[x * channelSize + channel] + BETA;
                            if (newVal < 0)
                            {
                                newVal = 0;
                            }
                            else if (newVal > 255)
                            {
                                newVal = 255;
                            }
                            row[x * channelSize + channel] = Convert.ToByte(newVal);
                        }
                    }
                }
            }
            bmp.UnlockBits(data);
        }

        public PointCollection GeneratePoints()
        {
            if (Pts != null)
            {
                return Pts;
            }
            Pts = new PointCollection();
            for (int i = 0; i < 256; i++)
            {
                int newVal = (int)(ALPHA*i-BETA);
                // this if, or it's else are unreachable depending on DX's value
                if (newVal > 255)
                {
                    newVal = 255;
                }

                else if (newVal < 0)
                {
                    newVal = 0;
                }
                Pts.Add(new System.Windows.Point(i, newVal));
            }
            //MessageBox.Show(Pts.ToString());
            return Pts;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public void SetPoints(PointCollection pts)
        {
            Pts = pts;
        }

        public string GetName()
        {
            return Name;
        }
    }
}
