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
        //public bool ButtonEnabled
        //{
        //    get { return !isGrayScale; }
        //    set { isGrayScale = value; }
        //}
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
            buttonConvertToGrayscale.IsEnabled = !isGrayScale;
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
    }
}
