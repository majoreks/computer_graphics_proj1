using System;
using System.Collections.Generic;
using System.Text;

namespace cg1
{
    public class MyColor
    {
        public int red;
        public int blue;
        public int green;

        public MyColor(int r = 0, int g = 0, int b = 0)
        {
            red = r;
            blue = b;
            green = g;
        }
        public void Add(MyColor color)
        {
            red += color.red;
            blue += color.blue;
            green += color.green;
        }
        public MyColor clone()
        {
            return new MyColor(red, green, blue);
        }

        public MyColor Normalized(int pixelCount)
        {
            return new MyColor(red / pixelCount, green / pixelCount, blue / pixelCount);
        }
    }
}
