using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cg1
{
    class BrightnessCorrectionFilter : IFilter
    {
        private string name = "Brightness Correction";
        // changable correction
        public PointCollection Pts
        {
            get;
            set;
        }

        private const int DX = -55;
        public string Name
        {
            get { return name; }
            set => name = value;
        }
        public void Filter(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            if (Pts != null)
            {
                unsafe
                {
                    int* bytes = (int*)data.Scan0;
                    var channelSize = 3;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        byte* row = (byte*)data.Scan0 + (y * data.Stride);
                        //if (y == 0)
                        //{
                        //    MessageBox.Show(row[0].ToString());
                        //}
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            for (int channel = 0; channel < 3; channel++)
                            {
                                row[x * channelSize + channel] = Convert.ToByte(Pts[row[x * channelSize + channel]].Y);
                            }
                        }
                    }
                }
            }
            else
            {
                unsafe
                {
                    int* bytes = (int*)data.Scan0;
                    var channelSize = 3;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        byte* row = (byte*)data.Scan0 + (y * data.Stride);
                        //if (y == 0)
                        //{
                        //    MessageBox.Show(row[0].ToString());
                        //}
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            for (int channel = 0; channel < 3; channel++)
                            {
                                int newVal = row[x * channelSize + channel] + DX;
                                // this if, or it's else are unreachable depending on DX's value

                                if (newVal > 255)
                                {
                                    newVal = 255;
                                }

                                else if (newVal < 0)
                                {
                                    newVal = 0;
                                }

                                row[x * channelSize + channel] = Convert.ToByte(newVal);
                            }
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
                int newVal = i + DX;
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
