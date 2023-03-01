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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleInventory.Wpf.Components
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        private bool _isCollapsed = false;
        private Storyboard storyboard;

        private const int COLLAPSED_WIDTH = 50;
        private const int EXPANDED_WIDTH = 200;


        public NavigationBar()
        {
            InitializeComponent();
        }

        private void CreateBarAnimation(int from, int to)
        {
            var animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.15));
            animation.AccelerationRatio = 1;
            storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTargetName(animation, navigationBar.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(NavigationBar.WidthProperty));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_isCollapsed)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }

        private void Expand()
        {
            _isCollapsed = false;
            CreateBarAnimation(COLLAPSED_WIDTH, EXPANDED_WIDTH);
            storyboard.Begin(this);
        }

        private void Collapse()
        {
            _isCollapsed = true;
            CreateBarAnimation(EXPANDED_WIDTH, COLLAPSED_WIDTH);
            storyboard.Begin(this);
        }
    }
}
