using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        private ObservableCollection<IFilter> filters;
        public MainWindow()
        {
            InitializeComponent();
            filters = new ObservableCollection<IFilter>();
            filters.Add(new InversionFilter());
            filters.Add(new BrightnessCorrectionFilter());
            filters.Add(new GammaCorrectionFilter());
            filters.Add(new ContrastEnhancementFilter());
            functionalFiltersListBox.ItemsSource = filters;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(functionalFiltersListBox.SelectedItem is IFilter))
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
            //MessageBox.Show(bimg.ToString());
            Bitmap bmp = BitmapImage2Bitmap(bimg);
            IFilter xd = functionalFiltersListBox.SelectedItem as IFilter;
            xd.Filter(bmp);
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
        }

        private void functionalFiltersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox tmp = sender as ListBox;
            if (tmp.SelectedIndex == -1)
            {
                return;
            }
            convolutionFiltersListBox.UnselectAll();
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
    }
}
