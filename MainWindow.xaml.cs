using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(convolutionFiltersListBox.SelectedIndex.ToString(), functionalFiltersListBox.SelectedIndex.ToString());
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
    }
}
