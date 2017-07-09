using System;
using System.IO;
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
using System.Reflection;


namespace MyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Control control;
        
        public MainWindow()
        {          
            InitializeComponent();
            control = new Control(this);
            int i = 0, j = 0;
            
            
        }

        private void openClick(object sender, RoutedEventArgs e)
        {
            control.open();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            control.save();
        }

        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            control.saveAs();
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            control.exit();
        }

        private void closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !control.saveDialog();
        }


        
        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            control.mouseDown(e);
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            control.mouseMove(e);
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            control.mouseUp(e);
        }

        private void cara_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(DrawShape.LINE);
        }

        private void obdelnik_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(DrawShape.RECT);
        }

        private void poz_mouseDown(object sender, MouseButtonEventArgs e)
        {
           control.stopDraw();
        }

        private void elipsa_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(DrawShape.ELLIPSE);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(control!=null) control.StrokeThickness = e.NewValue;
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            control.newC();
        }

        private void colors_MouseUp(object sender, MouseButtonEventArgs e)
        {
            control.setfColor(null);
        }

        private void polygon_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(DrawShape.POLYGON);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
