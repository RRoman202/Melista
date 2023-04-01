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
        public MediaPage()
        {
            InitializeComponent();
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            MediaPageViewModel viewModel = (MediaPageViewModel)element.DataContext;
            ICommand command = viewModel.SliderDragStartedCommand;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            MediaPageViewModel viewModel = (MediaPageViewModel)element.DataContext;
            ICommand command = viewModel.SliderDragCompletedCommand;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        private void Video_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            MediaPageViewModel viewModel = (MediaPageViewModel)element.DataContext;
            ICommand command = viewModel.VideoLoaded;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
