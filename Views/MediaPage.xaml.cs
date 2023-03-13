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

namespace Melista.Views
{
    /// <summary>
    /// Логика взаимодействия для MediaPage.xaml
    /// </summary>
    public partial class MediaPage : Page
    {
        int a = 0;
        public MediaPage()
        {
            InitializeComponent();
        }

        private void Media_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (a != 0)
            {
                var height = MediaGrid.ActualHeight;
                int wh = (int)height / 9;
                Media.Height = wh * 9;
                Media.Width = wh * 16;
            }
            a = a+1;
        }
    }
}
