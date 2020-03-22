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
        private int offset;
        public string Name
        {
            get { return name; }
            set => name = value;
        }
        public PointCollection Pts
        {
            get;
            set;
        }

        public ConvolutionFilter(string name, double[,] kernel, int factor, int offset)
        {
            this.offset = offset;
            this.name = name;
            this.kernel = (double[,])kernel.Clone();
            this.factor = factor;
        }
        public void Filter(Bitmap bmp)
        {
            Bitmap bmpTmp = (Bitmap)bmp.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bmpTmpData = bmpTmp.LockBits(new Rectangle(0, 0, bmpTmp.Width, bmpTmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int stride = bmpData.Stride;

            unsafe
            {
                byte* bytes = (byte*)bmpData.Scan0;
                byte* bytesTmp = (byte*)bmpTmpData.Scan0;
                //MessageBox.Show(rowChange.ToString() + " " + bmp.Width.ToString() + " " + stride.ToString());
                int width = bmp.Width - 3;
                int height = bmp.Height - 3;

                for (int y = 0; y < height; y++)
                {
                    int rowChange = stride - bmp.Width * 3;
                    for (int x = 0; x < width; x++)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            double newVal = (
                                (bytesTmp[channel] * kernel[0, 0]) + (bytesTmp[channel + 3] * kernel[0, 1]) + (bytesTmp[channel + 6] * kernel[0, 2]) +
                                (bytesTmp[channel + stride] * kernel[1, 0]) + (bytesTmp[channel + 3 + stride] * kernel[1, 1]) + (bytesTmp[channel + 6 + stride] * kernel[1, 2]) +
                                (bytesTmp[channel + stride * 2] * kernel[2, 0]) + (bytesTmp[channel + 3 + stride * 2] * kernel[2, 1]) + (bytesTmp[channel + 6 + stride * 2] * kernel[2, 2]))
                                / factor + offset;

                            if (newVal < 0)
                            {
                                newVal = 0;
                            }
                            else if (newVal > 255)
                            {
                                newVal = 255;
                            }
                            bytes[channel + stride] = Convert.ToByte(newVal);
                        }

                        // go to next pixel
                        bytes = bytes + 3;
                        bytesTmp = bytesTmp + 3;
                    }

                    // go to next row
                    bytes = bytes + rowChange;
                    bytesTmp = bytesTmp + rowChange;
                }
            }

            bmp.UnlockBits(bmpData);
            bmpTmp.UnlockBits(bmpTmpData);

        }

        // function are here just to be compatible with IFilter interface
        public PointCollection GeneratePoints()
        {
            throw new NotImplementedException();
        }

        public void SetName(string name)
        {
            throw new NotImplementedException();
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
