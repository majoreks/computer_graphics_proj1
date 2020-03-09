using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace cg1
{
    public class InversionFilter : IFilter
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set => this.name = value;
        }

        public PointCollection Pts
        {
            get;
            set;
        }
        public InversionFilter()
        {
            Name = "Inversion";
        }

        public void Filter(Bitmap bmp)
        {

            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {
                int* bytes = (int*)data.Scan0;
                //MessageBox.Show(bytes[0].ToString());
                var channelSize = 3;
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        row[x * channelSize] = Convert.ToByte(255 - row[x * channelSize]);
                        row[x * channelSize + 1] = Convert.ToByte(255 - row[x * channelSize + 1]);
                        row[x * channelSize + 2] = Convert.ToByte(255 - row[x * channelSize + 2]);
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
                Pts.Add(new System.Windows.Point(i,255-i));
            }
            return Pts;
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetPoints(PointCollection pts)
        {
            Pts = pts;
        }
    }
}
