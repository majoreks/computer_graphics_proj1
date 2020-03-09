using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cg1
{
    /// <summary>
    /// Interaction logic for EditFiltersWindow.xaml
    /// </summary>
    public partial class EditFiltersWindow : Window
    {
        private Label dataLabel = null;
        private PointCollection pts;
        public EditFiltersWindow()
        {
            InitializeComponent();
            pts = pointsInit();
            functionPolyline.Points = pts;
            dataLabel = new Label();
            polylineWrapperCanvas.Children.Add(dataLabel);
        }

        private PointCollection pointsInit()
        {
            PointCollection pts = new PointCollection();
            for (int i = 0; i < 256; i++)
            {
                pts.Add(new Point(i, i));
            }
            return pts;
        }

        public (string name, PointCollection points) retVal
        {
            get;
            set;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text == null)
            {
                return;
            }
            string tmp = nameTextBox.Text;
            retVal = (tmp, pts);
            this.Close();
        }


        private Point findPosition(MouseEventArgs e)
        {
            double mouseX;
            double mouseY;
            Point tmp1 = e.GetPosition(polylineWrapperCanvas);
            Vector tmp2 = VisualTreeHelper.GetOffset(functionPolyline);
            mouseX = (int)(tmp1.X - tmp2.X - 1);
            mouseY = (int)(tmp1.Y - tmp2.Y - 1);
            if (mouseX > 255)
            {
                mouseX = 255;
            }
            if (mouseY > 255)
            {
                mouseY = 255;
            }
            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseY < 0)
            {
                mouseY = 0;
            }
            return new Point(mouseX, mouseY);
        }

        private void polylineWrapperCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = findPosition(e);
            if (Mouse.DirectlyOver == functionPolyline)
            {
                polylineWrapperCanvas.Cursor = Cursors.UpArrow;
                XD.Visibility = Visibility.Visible;
                XD.Content = $"({mousePosition.X} ; {mousePosition.Y})";
                XD.Measure(new Size(double.MaxValue, double.MaxValue));
                if (mousePosition.X < 35 && mousePosition.Y < 35)
                {
                    Canvas.SetLeft(XD, mousePosition.X + 20);
                    Canvas.SetTop(XD, mousePosition.Y + 20);
                }
                else if (mousePosition.X > 220 && mousePosition.Y > 220)
                {
                    Canvas.SetLeft(XD, mousePosition.X - 60);
                    Canvas.SetTop(XD, mousePosition.Y + 30);
                }
                else
                {
                    Canvas.SetLeft(XD, mousePosition.X - 40);
                    Canvas.SetTop(XD, mousePosition.Y + 30);
                }
            }
            else
            {
                XD.Content = "";
                polylineWrapperCanvas.Cursor = null;
            }
        }

        private void functionPolyline_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = findPosition(e);
            var w = new popupWindow();
            w.ShowDialog();
            int val = w.Val;
            if (val < 0) val = 0;

            pts[(int)mousePosition.X] = new Point((int)mousePosition.X, val);
            functionPolyline.Points = pts;
            //MessageBox.Show(pts.ToString()) ;

        }
    }
}
