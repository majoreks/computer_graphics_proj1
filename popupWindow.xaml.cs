using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for popupWindow.xaml
    /// </summary>
    public partial class popupWindow : Window
    {
        public popupWindow()
        {
            InitializeComponent();
            textbox.Focus();
        }
        public int Val
        {
            get;
            set;
        }

        private void textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int val;
                if (int.TryParse(textbox.Text, out val))
                {
                    if (val > 255)
                    {
                        return;
                    }
                    else if (val < 0)
                    {
                        return;
                    }
                    else
                    {
                        Val = val;
                        this.Close();
                    }

                }
            }
        }
    }
}
