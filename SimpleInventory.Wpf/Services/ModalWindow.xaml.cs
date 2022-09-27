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

        public bool CallbackAction
        {
            get { return (bool)GetValue(CallbackActionProperty); }
            set { SetValue(CallbackActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallbackAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallbackActionProperty =
            DependencyProperty.Register("CallbackAction", typeof(bool), typeof(Window), new PropertyMetadata(false));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
