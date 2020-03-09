using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace cg1
{
    public interface IFilter
    {
        public void Filter(Bitmap bmp);
        public PointCollection GeneratePoints();
    }
}
