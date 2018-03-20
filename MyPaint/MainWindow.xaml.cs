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
using System.Diagnostics;


namespace MyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainControl control;
        bool resolutionDrag = false;
        MyEnum activeColor = MyEnum.PRIMARY;
        Brush CBLock;
        //public Canvas topCanvas = new Canvas();

        public MainWindow()
        {
            InitializeComponent();
            //topCanvas = topCanvass;
            control = new MainControl(this);
            colorsInit();
            setActiveColor(MyEnum.PRIMARY);
            setColor(Brushes.Black);
            
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
                setColor(br);
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
            control.Open();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            control.save();
        }

        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            control.SaveAs();
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            control.exit();
        }

        private void closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            control.closed(e);
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
            control.setTool(MyEnum.LINE);
        }

        private void obdelnik_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.RECT);
        }

        private void elipsa_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.ELLIPSE);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if(control!=null) control.StrokeThickness = e.NewValue;
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            control.NewC();
        }

        private void polygon_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.POLYGON);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_select_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.SELECT);
        }

        private void button_line_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.LINE);
        }

        private void button_ellipse_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.ELLIPSE);
        }

        private void button_rectangle_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.RECT);
        }

        private void button_polygon_Click(object sender, RoutedEventArgs e)
        {
            control.setTool(MyEnum.POLYGON);
        }

        private void primaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setActiveColor(MyEnum.PRIMARY); 
        }

        private void secondaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setActiveColor(MyEnum.SECONDARY);
        }

        private void backgroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setActiveColor(MyEnum.BACKGROUND);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            control.keyDown(sender, e);    
        }

        private void thickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(control != null) control.SetThickness(e.NewValue);
        }

        public void setActiveColor(MyEnum color)
        {
            activeColor = color;
            primaryColor.Stroke = color == MyEnum.PRIMARY ? Brushes.Orange : null;
            secondaryColor.Stroke = color == MyEnum.SECONDARY ? Brushes.Orange : null;
            backgroundColor.Stroke = color == MyEnum.BACKGROUND ? Brushes.Orange : null;
        }

        private void CB_ColorChanged(object sender, ColorBox.ColorChangedEventArgs e)
        {
            if(CBLock != null && CBLock.Equals(CB.Brush))
            {
                return;
            }
            updateColor(CB.Brush);
            CBLock = CB.Brush;
        }

        private void setColor(Brush color)
        {
            updateColor(color);
            if (color != null && color is SolidColorBrush)
            {
                color.Freeze();
            }
            CBLock = color;
            CB.Brush = color;            
        }

        private void updateColor(Brush color)
        {
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    control.SetPrimaryBrush(color);
                    primaryColor.Fill = color;
                    break;
                case MyEnum.SECONDARY:
                    control.SetSecondaryBrush(color);
                    secondaryColor.Fill = color;
                    break;
                case MyEnum.BACKGROUND:
                    control.SetBackgroundBrush(color);
                    backgroundColor.Fill = color;
                    break;
            }

        }

        private void resolution_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resolutionDrag = true;
        }

        private void canvas_outer_MouseMove(object sender, MouseEventArgs e)
        {
            if (resolutionDrag)
            {
                control.setResolution(e.GetPosition(canvas_out).X, e.GetPosition(canvas_out).Y);
            }
        }

        private void canvas_outer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (resolutionDrag)
            {
                resolutionDrag = false;
            }
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            control.Back();
        }

        private void button_forward_Click(object sender, RoutedEventArgs e)
        {
            control.Forward();
        }

        public void setHistory(bool b, bool f)
        {
            back.Fill = b ? Brushes.Azure : Brushes.Gray;
            forward.Fill = f ? Brushes.Azure : Brushes.Gray;
        }

        private void button_addLayer_Click(object sender, RoutedEventArgs e)
        {
            control.AddLayer();
        }

        private void layers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(control != null) control.LayerChanged(layers.SelectedIndex);
        }

        private void zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (control != null)  control.SetZoom(e.NewValue/100);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_layer_down_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Layer l = button.DataContext as Layer;
            l.Down();
        }

        private void Button_layer_up_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Layer l = button.DataContext as Layer;
            l.Up();
        }

        private void Button_layer_delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Layer l = button.DataContext as Layer;
            l.Remove();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabControl.SelectedItem as TabItem;

            control.TabControlChange(tab);
        }

        private void tabClose_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabControl.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).FirstOrDefault();

            TabItem tab = item as TabItem;
            control.TabControlDelete(tab);
        }

        public void setPrimaryBrush(Brush brush)
        {
            primaryColor.Fill = brush;
        }

        public void setSecondaryBrush(Brush brush)
        {
            secondaryColor.Fill = brush;
        }

        public void setBackgroundBrush(Brush brush)
        {
            backgroundColor.Fill = brush;
        }

        public void setThickness(double t)
        {
            thickness.Value = t;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
