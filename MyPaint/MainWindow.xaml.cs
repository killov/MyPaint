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
using ColorBox;


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

            colorsInit();           
        }

        private void colorsInit()
        {
            addColor(0, 0, 255, 255, 255);
            addColor(1, 0, 255, 255, 0);
            addColor(2, 0, 255, 0, 255);
            addColor(3, 0, 0, 255, 255);
            addColor(4, 0, 255, 0, 0);
            addColor(5, 0, 0, 255, 0);
            addColor(6, 0, 0, 0, 255);
            addColor(7, 0, 0, 0, 0);
            addColor(0, 1, 0, 128, 0);
            addColor(1, 1, 128, 0, 0);
            addColor(2, 1, 128, 128, 0);
            addColor(3, 1, 128, 0, 128);
            addColor(4, 1, 128, 128, 128);
            addColor(5, 1, 0, 0, 128);
            addColor(6, 1, 0, 128, 128);
            addColor(7, 1, 192, 192, 192);
        }

        private void addColor(int x, int y, int r, int g, int b)
        {
            Rectangle rect = new Rectangle();
            rect.StrokeThickness = 1;
            rect.Stroke = Brushes.Black;
            rect.Width = 17;
            rect.Height = 17;
            Brush br = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
            rect.Fill = br;
            rect.RadiusX = 3;
            rect.RadiusY = 3;
            rect.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                control.setColor(br);
            };
            colors.Children.Add(rect);
            Canvas.SetLeft(rect, x * 19+2);
            Canvas.SetTop(rect, y * 19+2);
            rect.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                rect.Stroke = Brushes.Orange;
            };
            rect.MouseLeave += delegate (object sender, MouseEventArgs e)
            {
                rect.Stroke = Brushes.Black;
            };
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
            control.setDrawShape(MyEnum.LINE);
        }

        private void obdelnik_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.RECT);
        }

        private void poz_mouseDown(object sender, MouseButtonEventArgs e)
        {
           control.stopDraw();
        }

        private void elipsa_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.ELLIPSE);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if(control!=null) control.StrokeThickness = e.NewValue;
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            control.newC();
        }

        private void polygon_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.POLYGON);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_line_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.LINE);
        }

        private void button_ellipse_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.ELLIPSE);
        }

        private void button_rectangle_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.RECT);
        }

        private void button_polygon_Click(object sender, RoutedEventArgs e)
        {
            control.setDrawShape(MyEnum.POLYGON);
        }

        private void primaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            control.setActiveColor(MyEnum.PRIMARY); 
        }

        private void secondaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            control.setActiveColor(MyEnum.SECONDARY);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.Key.ToString());
            if (e.Key == Key.Delete)
            {
                control.delete();
            }
            
        }

        private void thickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(control != null) control.setThickness(e.NewValue);
        }

        private void moreColors_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CB_ColorChanged(object sender, ColorBox.ColorChangedEventArgs e)
        {
            control.setColor(CB.Brush);
        }
    }
}
