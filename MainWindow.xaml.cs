using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cg1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<IFilter> functionalFiltersList;
        private ObservableCollection<IFilter> convolutionalFiltersList;
        private IFilter selectedFilter;
        private bool isGrayScale = false;

        public MainWindow()
        {
            InitializeComponent();

            functionalFiltersList = new ObservableCollection<IFilter>();
            functionalFiltersList.Add(new InversionFilter());
            functionalFiltersList.Add(new BrightnessCorrectionFilter());
            functionalFiltersList.Add(new GammaCorrectionFilter());
            functionalFiltersList.Add(new ContrastEnhancementFilter());
            functionalFiltersListBox.ItemsSource = functionalFiltersList;

            convolutionalFiltersList = new ObservableCollection<IFilter>();
            convolutionalFiltersList.Add(new ConvolutionFilter(
                "Blur",
                new double[,] { { 1, 1, 1, },
                                { 1, 1, 1, },
                                { 1, 1, 1, }, },
                10, 0));
            convolutionalFiltersList.Add(new ConvolutionFilter(
                "Gaussian blur",
                new double[,] { { 0, 1, 0, },
                                { 1, 4, 1, },
                                { 0, 1, 0, }, },
                10, 0));
            convolutionalFiltersList.Add(new ConvolutionFilter(
                "Sharpen",
                new double[,] { { -1, -1, -1, },
                                { -1, 9, -1, },
                                { -1, -1, -1, }, },
                1, 0));
            convolutionalFiltersList.Add(new ConvolutionFilter(
                "Emboss",
                new double[,] { { -1, 0, 1, },
                                { -1, 1, 1, },
                                { -1, 0, 1, }, },
                1, 0));
            convolutionalFiltersList.Add(new ConvolutionFilter(
                "Edge detection",
                new double[,] { { 0, -1, 0, },
                                { 0, 1, 0, },
                                { 0, 0, 0, }, },
                1, 128));
            convolutionFiltersListBox.ItemsSource = convolutionalFiltersList;
            buttonConvertToGrayscale.IsEnabled = !isGrayScale;
            textBoxGrayscale.IsEnabled = isGrayScale;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(selectedFilter is IFilter) || (filteredImage.Source == null && originalImage.Source == null))
            {
                //MessageBox.Show("error");
                return;
            }
            BitmapImage bimg = new BitmapImage();
            if (filteredImage.Source != null)
            {
                bimg = (BitmapImage)filteredImage.Source;
            }
            else
            {
                bimg = (BitmapImage)originalImage.Source;
            }
            //MessageBox.Show(convolutionFiltersListBox.SelectedIndex.ToString(), functionalFiltersListBox.SelectedIndex.ToString());
            //MessageBox.Show(selectedFilter.ToString());
            Bitmap bmp = BitmapImage2Bitmap(bimg);
            selectedFilter.Filter(bmp);
            ImageSource newImg = (ImageSource)Bitmap2BitmapImage(bmp);
            filteredImage.Source = newImg;

        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void convolutionFiltersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox tmp = sender as ListBox;
            if (tmp.SelectedIndex == -1)
            {
                return;
            }
            functionalFiltersListBox.UnselectAll();
            selectedFilter = tmp.SelectedItem as IFilter;

        }

        private void functionalFiltersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox tmp = sender as ListBox;
            if (tmp.SelectedIndex == -1)
            {
                return;
            }
            //MessageBox.Show(tmp.SelectedItem.ToString());
            convolutionFiltersListBox.UnselectAll();
            selectedFilter = tmp.SelectedItem as IFilter;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (filteredImage.Source != null)
            {
                MessageBoxResult result = MessageBox.Show("If you proceed you will lose filtered image, are you sure?", "Warning!",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    filteredImage.Source = null;
                }
                else
                {
                    return;
                }

            }
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                originalImage.Source = new BitmapImage(new Uri(op.FileName));
            }
            isGrayScale = false;
            textBoxR.IsEnabled = !isGrayScale;
            textBoxG.IsEnabled = !isGrayScale;
            textBoxB.IsEnabled = !isGrayScale;
            buttonConvertToGrayscale.IsEnabled = !isGrayScale;
            textBoxGrayscale.IsEnabled = isGrayScale;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (filteredImage.Source != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "filtered_image"; // Default file name
                dlg.DefaultExt = ".jpg"; // Default file extension
                dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png"; // Filter files by extension
                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)filteredImage.Source));
                    using (FileStream stream = new FileStream(filename, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            filteredImage.Source = null;
            isGrayScale = false;
            textBoxR.IsEnabled = !isGrayScale;
            textBoxG.IsEnabled = !isGrayScale;
            textBoxB.IsEnabled = !isGrayScale;
            buttonConvertToGrayscale.IsEnabled = !isGrayScale;
            textBoxGrayscale.IsEnabled = isGrayScale;
        }
        private bool delAtIndex = true;
        ObservableCollection<IFilter> tmp = new ObservableCollection<IFilter>();
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (delAtIndex)
            {
                foreach (var xd in functionalFiltersList)
                {
                    tmp.Add(xd);
                }
                tmp.RemoveAt(2);
                delAtIndex = false;
            }
            EditFiltersWindow editWindow = new EditFiltersWindow(tmp);
            if (editWindow.ShowDialog() == true)
            {
                var ret = editWindow.retVal;
                if (ret.name.Length == 0 || ret.points == null)
                {
                    return;
                }
                if (ret.index >= 0)
                {
                    //functionalFiltersList[ret.index].SetName(ret.name);
                    functionalFiltersList[ret.index].SetPoints(ret.points);
                    functionalFiltersListBox.ItemsSource = functionalFiltersList;
                    return;
                    //functionalFiltersList.RemoveAt(ret.index);
                    //functionalFiltersList.Add(xd);
                    //return;
                }
                EditedFunctionalFilter xd = new EditedFunctionalFilter(ret.name, ret.points);
                functionalFiltersList.Add(xd);
                tmp.Add(xd);
            }

            //functionalFiltersListBox.ItemsSource = functionalFiltersList;
            //MessageBox.Show(ret.name + " " + ret.points.ToString());
        }

        private void setGammaButton_Click(object sender, RoutedEventArgs e)
        {
            double val;
            if (double.TryParse(gammaTextBox.Text, out val))
            {
                if (val < 0)
                {
                    return;
                }
                else
                {
                    GammaCorrectionFilter xd = functionalFiltersList[2] as GammaCorrectionFilter;
                    xd.SetGamma(val);
                    MessageBox.Show("gamma changed");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (filteredImage.Source == null && originalImage.Source == null)
            {
                //MessageBox.Show("error");
                return;
            }
            if (isGrayScale)
            {
                return;
            }
            BitmapImage bimg = new BitmapImage();
            if (filteredImage.Source != null)
            {
                bimg = (BitmapImage)filteredImage.Source;
            }
            else
            {
                bimg = (BitmapImage)originalImage.Source;
            }
            //MessageBox.Show(convolutionFiltersListBox.SelectedIndex.ToString(), functionalFiltersListBox.SelectedIndex.ToString());
            //MessageBox.Show(selectedFilter.ToString());
            Bitmap bmp = BitmapImage2Bitmap(bimg);
            ConvertToGrayScale(bmp);
            ImageSource newImg = (ImageSource)Bitmap2BitmapImage(bmp);
            filteredImage.Source = newImg;
            isGrayScale = true;
            textBoxGrayscale.IsEnabled = isGrayScale;
            textBoxR.IsEnabled = !isGrayScale;
            textBoxG.IsEnabled = !isGrayScale;
            textBoxB.IsEnabled = !isGrayScale;
            buttonConvertToGrayscale.IsEnabled = !isGrayScale;
            //MessageBox.Show(isGrayScale.ToString());
        }

        private void ConvertToGrayScale(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

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
                        byte newVal = (byte)(row[x * channelSize] * .21 + row[x * channelSize + 1] * .71 + row[x * channelSize + 2] * .071);
                        for (int channel = 0; channel < 3; channel++)
                        {
                            row[x * channelSize + channel] = Convert.ToByte(newVal);
                        }
                    }
                }
            }
            bmp.UnlockBits(data);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == sv1)
            {
                sv2.ScrollToVerticalOffset(e.VerticalOffset);
                sv2.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else
            {
                sv1.ScrollToVerticalOffset(e.VerticalOffset);
                sv1.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        private void buttonApplyDithering_Click(object sender, RoutedEventArgs e)
        {
            if (filteredImage.Source == null && originalImage.Source == null)
            {
                //MessageBox.Show("error");
                return;
            }
            BitmapImage bimg = new BitmapImage();
            if (filteredImage.Source != null)
            {
                bimg = (BitmapImage)filteredImage.Source;
            }
            else
            {
                bimg = (BitmapImage)originalImage.Source;
            }
            Bitmap bmp = BitmapImage2Bitmap(bimg);

            //algorithm
            if (isGrayScale)
            {
                int[] levels = LinSpace(0, 255, int.Parse(textBoxGrayscale.Text));
                Random r = new Random();
                int threshold = r.Next(0, 255);
                BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

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
                            byte oldVal = row[x * channelSize];
                            byte newVal;
                            byte first = (byte)levels.OrderBy(x => Math.Abs((long)x - oldVal)).First();
                            byte second = (byte)levels.OrderBy(x => Math.Abs((long)x - oldVal)).Skip(1).First();
                            if (oldVal < threshold)
                            {
                                newVal = Math.Min(first, second);
                            }
                            else
                            {
                                newVal = Math.Max(first, second);
                            }
                            for (int channel = 0; channel < 3; channel++)
                            {
                                row[x * channelSize + channel] = Convert.ToByte(newVal);
                            }
                            threshold = r.Next(0, 255);
                        }
                    }
                }
                bmp.UnlockBits(data);
            }
            else
            {
                int[] levelsR = LinSpace(0, 255, int.Parse(textBoxR.Text));
                int[] levelsG = LinSpace(0, 255, int.Parse(textBoxG.Text));
                int[] levelsB = LinSpace(0, 255, int.Parse(textBoxB.Text));
                int[][] levels = { levelsB, levelsG, levelsR };
                Random r = new Random();
                int threshold = r.Next(0, 255);
                BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

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
                                byte oldVal = row[x * channelSize + channel];
                                byte newVal;
                                byte first = (byte)levels[channel].OrderBy(x => Math.Abs((long)x - oldVal)).First();
                                byte second = (byte)levels[channel].OrderBy(x => Math.Abs((long)x - oldVal)).Skip(1).First();
                                if (oldVal < threshold)
                                {
                                    newVal = Math.Min(first, second);
                                }
                                else
                                {
                                    newVal = Math.Max(first, second);
                                }

                                row[x * channelSize + channel] = Convert.ToByte(newVal);
                                threshold = r.Next(0, 255);
                            }

                        }
                    }
                }
                bmp.UnlockBits(data);

            }


            ImageSource newImg = (ImageSource)Bitmap2BitmapImage(bmp);
            filteredImage.Source = newImg;
        }

        private int[] LinSpace(int x1, int x2, int n)
        {
            if (n < 2)
            {
                return null;
            }
            int step = (x2 - x1) / (n - 1);
            int[] xd = new int[n];
            for (int i = 0; i < n; i++)
            {
                xd[i] = x1 + step * i;
            }
            return xd;
        }

        public static bool IsValid(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 0 && i <= 255;
        }

        private void textBoxR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("[^0-9]+");
            e.Handled = r.IsMatch(e.Text);
        }

        // in theory this is responsible for the color quantization, however I was unable to make it work, when I go through
        // colours to add them to octree, I end up with 0 colors in palette
        private void buttonOctree_Click(object sender, RoutedEventArgs e)
        {
            if (filteredImage.Source == null && originalImage.Source == null)
            {
                //MessageBox.Show("error");
                return;
            }
            BitmapImage bimg = new BitmapImage();
            if (filteredImage.Source != null)
            {
                bimg = (BitmapImage)filteredImage.Source;
            }
            else
            {
                bimg = (BitmapImage)originalImage.Source;
            }

            Bitmap bmp = BitmapImage2Bitmap(bimg);
            var xd = GetColors(bmp);
            MessageBox.Show($"{xd.Count}"); // shows number of colors in palette, couldnt make it work, its here just for testing

            ////algorithm

            //ImageSource newImg = (ImageSource)Bitmap2BitmapImage(bmp);
            //filteredImage.Source = newImg;
        }

        private List<MyColor> GetColors(Bitmap bmp)
        {
            List<(byte, byte, byte)> ps = new List<(byte, byte, byte)>();
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {
                int* bytes = (int*)data.Scan0;
                var channelSize = 3;
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);

                    for (int x = 0; x < bmp.Width; x++)
                    {
                        byte oldVal = row[x * channelSize];
                        ps.Add((row[x * channelSize], row[x * channelSize + 1], row[x * channelSize + 2]));

                    }
                }
            }
            bmp.UnlockBits(data);
            ps = ps.ToArray().Distinct().ToList();
            //MessageBox.Show(ps.Count.ToString());
            Quantizer quantizer = new Quantizer();
            foreach (var xd in ps)
            {
                quantizer.AddColor(new MyColor((int)xd.Item1, (int)xd.Item2, (int)xd.Item3));
            }
            return quantizer.MakePalette(int.Parse(textBoxOctree.Text));
        }

        private void buttonLabTask2_Click(object sender, RoutedEventArgs e)
        {
            if (filteredImage.Source == null && originalImage.Source == null)
            {
                //MessageBox.Show("error");
                return;
            }
            BitmapImage bimg = new BitmapImage();
            if (filteredImage.Source != null)
            {
                bimg = (BitmapImage)filteredImage.Source;
            }
            else
            {
                bimg = (BitmapImage)originalImage.Source;
            }
            Bitmap bmp = BitmapImage2Bitmap(bimg);

            Bitmap bmpY = (Bitmap)bmp.Clone();
            Bitmap bmpCr = (Bitmap)bmp.Clone();
            Bitmap bmpCb = (Bitmap)bmp.Clone();

            BitmapData bmpYData = bmpY.LockBits(new System.Drawing.Rectangle(0, 0, bmpY.Width, bmpY.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData dataCr = bmpCr.LockBits(new System.Drawing.Rectangle(0, 0, bmpCr.Width, bmpCr.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData dataCb = bmpCb.LockBits(new System.Drawing.Rectangle(0, 0, bmpCb.Width, bmpCb.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                int* bytes = (int*)data.Scan0;

                int* bytesY = (int*)bmpYData.Scan0;
                int* bytesCr = (int*)dataCr.Scan0;

                var channelSize = 3;
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    byte* rowY = (byte*)bmpYData.Scan0 + (y * bmpYData.Stride);
                    byte* rowCr = (byte*)dataCr.Scan0 + (y * dataCr.Stride);
                    byte* rowCb = (byte*)dataCb.Scan0 + (y * dataCb.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        byte b = row[x * channelSize];
                        byte g = row[x * channelSize + 1];
                        byte r = row[x * channelSize + 2];

                        var cr = (0.5 * r + 0.418688 * g + 0.081312 * b) / 255;
                        var cb = (0.168736 * r + 0.331264 * g + 0.5 * b) / 255;

                        rowCr[x * channelSize] = (byte)127;
                        rowCr[x * channelSize + 1] = (byte)Lerp(255, 0, cr);
                        rowCr[x * channelSize + 2] = (byte)Lerp(0, 255, cr);

                        rowCb[x * channelSize] = (byte)Lerp(0, 255, cb);
                        rowCb[x * channelSize + 1] = (byte)Lerp(255, 0, cb);
                        rowCb[x * channelSize + 2] = (byte)127;
                        for (int i = 0; i < channelSize; i++)
                        {
                            rowY[x * channelSize + i] = (byte)(16 + 65.738 / 256 * r + 129.057 / 256 * g + 25.064 / 256 * b);
                        }
                    }
                }
            }
            bmp.UnlockBits(data);
            bmpY.UnlockBits(bmpYData);
            bmpCr.UnlockBits(dataCr);
            bmpCb.UnlockBits(dataCb);
            //ImageSource newImg = (ImageSource)Bitmap2BitmapImage(bmpY);
            //filteredImage.Source = newImg;
            ShowImageWindow w = new ShowImageWindow((ImageSource)Bitmap2BitmapImage(bmpY));
            w.Title = "Y";
            w.Show();
            ShowImageWindow wCr = new ShowImageWindow((ImageSource)Bitmap2BitmapImage(bmpCr));
            wCr.Title = "Cr";
            wCr.Show();
            ShowImageWindow wCb = new ShowImageWindow((ImageSource)Bitmap2BitmapImage(bmpCb));
            wCb.Title = "Cb";
            wCb.Show();
        }

        private int Lerp(double a, double b, double t)
        {
            return (int)((1 - t) * a + t * b);
        }
    }


}
