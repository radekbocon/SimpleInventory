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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleInventory.Wpf.Services
{
    /// <summary>
    /// Interaction logic for ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        public ModalWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.2));
            anim.Completed += (s, args) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}
