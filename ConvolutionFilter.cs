using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cg1
{
    public class ConvolutionFilter : IFilter
    {
        private string name;
        private double[,] kernel;
        private int factor;
        public string Name
        {
            get { return name; }
        }

        public ConvolutionFilter(string name, double[,] kernel, int factor)
        {
            this.name = name;
            this.kernel = (double[,])kernel.Clone();
            this.factor = factor;
        }
        public void Filter(Bitmap bmp)
        {
            Bitmap bmpTmp = (Bitmap)bmp.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                ImageLockMode.ReadWrite,
                                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bmpTmpData = bmpTmp.LockBits(new Rectangle(0, 0, bmpTmp.Width, bmpTmp.Height),
                               ImageLockMode.ReadWrite,
                               System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int stride = bmpData.Stride;

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                byte* ptrSrc = (byte*)bmpTmpData.Scan0;
                int dx = stride - bmp.Width * 3;
                int width = bmp.Width - 2;
                int height = bmp.Height - 2;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            double newVal = (
                                (ptrSrc[channel] * kernel[0, 0]) +
                                (ptrSrc[channel + 3] * kernel[0, 1]) +
                                (ptrSrc[channel + 6] * kernel[0, 2]) +
                                (ptrSrc[channel + stride] * kernel[1, 0]) +
                                (ptrSrc[channel + 3 + stride] * kernel[1, 1]) +
                                (ptrSrc[channel + 6 + stride] * kernel[1, 2]) +
                                (ptrSrc[channel + stride * 2] * kernel[2, 0]) +
                                (ptrSrc[channel + 3 + stride * 2] * kernel[2, 1]) +
                                (ptrSrc[channel + 6 + stride * 2] * kernel[2, 2]))
                                / factor;

                            if (newVal < 0)
                            {
                                newVal = 0;
                            }
                            else if (newVal > 255)
                            {
                                newVal = 255;
                            }
                            ptr[3 + channel + stride] = (byte)newVal;
                        }

                        ptr += 3;
                        ptrSrc += 3;
                    }

                    ptr += dx;
                    ptrSrc += dx;
                }
            }

            bmp.UnlockBits(bmpData);
            bmpTmp.UnlockBits(bmpTmpData);

        }
    }
}
