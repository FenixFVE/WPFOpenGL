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
using System.Windows.Shapes;

namespace WPFOpenGL
{
    public partial class Window1 : Window
    {
        public Color color { get; set; } = null;

        public Window1()
        {
            InitializeComponent();
        }

        private void Green_OnClick(object sender, RoutedEventArgs e)
        {
            color = new Color(0, 255, 0);
            Close();
        }

        private void Red_OnClick(object sender, RoutedEventArgs e)
        {
            color = new Color(255, 0, 0);
            Close();
        }

        private void Blue_OnClick(object sender, RoutedEventArgs e)
        {
            color = new Color(0, 0, 255);
            Close();
        }

        private void Yellow_OnClick(object sender, RoutedEventArgs e)
        {
            color = new Color(255, 255, 0);
            Close();
        }
    }
}
