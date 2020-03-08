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
        private const double BETA = 20;
        public string Name
        {
            get { return name; }
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
    }
}
