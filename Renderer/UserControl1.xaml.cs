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

namespace Renderer
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        Form1 Parent;
        public UserControl1(Form1 parent)
        {
            InitializeComponent();
            Parent = parent;
        }
        public void start()
        {
            
            
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Renderer.Program.rotateImageY(ref Parent,1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Renderer.Program.rotateImageX(ref Parent,1);
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Renderer.Program.rotateImageX(ref Parent,-1);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Renderer.Program.rotateImageY(ref Parent, -1);
        }
    }
}
