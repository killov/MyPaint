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

namespace MyPaint
{
    /// <summary>
    /// Interakční logika pro WindowChangeColor.xaml
    /// </summary>
    public partial class WindowChangeColor : Window
    {
        public  Control control;

        public WindowChangeColor()
        {
            InitializeComponent();
               
        }

        public void start()
        {
            MyBrush color = control.getColor();
            if(color.brush != null)
                color.brush.Freeze();
            CB.Brush = color.brush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyBrush color = new MyBrush(CB.Brush);
            control.setColor(color);
            Close();
        }
    }
}
