using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace MyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainControl control;
        bool resolutionDrag = false;
        BrushEnum activeColor = BrushEnum.PRIMARY;
        Brush CBLock;
        bool fontLock = false;
        //public Canvas topCanvas = new Canvas();

        public MainWindow()
        {
            InitializeComponent();
            control = new MainControl(this);
            colorsInit();
            SetActiveColor(BrushEnum.PRIMARY);
            setColor(Brushes.Black);
            control.SetWindowTextFont(new FontFamily("Arial"));
            control.SetWindowTextSize(12);
            ShowFontPanel(false);
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
            Canvas.SetLeft(rect, x * 19 + 2);
            Canvas.SetTop(rect, y * 19 + 2);
            rect.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                rect.Stroke = Brushes.Orange;
            };
            rect.MouseLeave += delegate (object sender, MouseEventArgs e)
            {
                rect.Stroke = Brushes.Black;
            };
        }

        private async void openClick(object sender, RoutedEventArgs e)
        {
            t = control.Open();

            await t;
        }
        Task t;
        private void saveClick(object sender, RoutedEventArgs e)
        {
            control.Save();
        }

        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            control.SaveAs();
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            control.Exit();
        }

        private void closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            control.Closed(e);
        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            control.MouseDown(e);
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            //control.MouseMove(e);
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            //control.MouseUp(e);
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            control.NewC();
        }

        private void button_select_area_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.SELECTAREA);
        }

        private void button_select_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.SELECT);
        }

        private void button_line_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.LINE);
        }

        private void button_polyline_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.POLYLINE);
        }

        private void button_ellipse_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.ELLIPSE);
        }

        private void button_rectangle_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.RECT);
        }

        private void button_polygon_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.POLYGON);
        }

        private void button_q_cueve_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.QUADRATICCURVE);
        }

        private void button_text_Click(object sender, RoutedEventArgs e)
        {
            control.SetTool(ToolEnum.TEXT);
        }

        private void primaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveColor(BrushEnum.PRIMARY);
        }

        private void secondaryColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveColor(BrushEnum.SECONDARY);
        }

        private void backgroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveColor(BrushEnum.BACKGROUND);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            control.KeyDown(sender, e);
        }

        private void thickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (control != null) control.SetThickness(e.NewValue);
        }

        public void SetActiveColor(BrushEnum color)
        {
            activeColor = color;
            primaryColor.Stroke = color == BrushEnum.PRIMARY ? Brushes.Orange : null;
            secondaryColor.Stroke = color == BrushEnum.SECONDARY ? Brushes.Orange : null;
            backgroundColor.Stroke = color == BrushEnum.BACKGROUND ? Brushes.Orange : null;
            switch (activeColor)
            {
                case BrushEnum.PRIMARY:
                    CBSetBrush(primaryColor.Fill);
                    break;
                case BrushEnum.SECONDARY:
                    CBSetBrush(secondaryColor.Fill);
                    break;
                case BrushEnum.BACKGROUND:
                    CBSetBrush(backgroundColor.Fill);
                    break;
            }
        }

        private void CB_ColorChanged(object sender, ColorBox.ColorChangedEventArgs e)
        {
            if (CBLock != null && CBLock.Equals(CB.Brush))
            {
                return;
            }
            Brush brush = CB.Brush;
            if (brush != null && !brush.IsFrozen)
            {
                brush.Changed += Brush_Changed;
            }
            brush = Serializer.Brush.Create(brush).CreateBrush();

            updateColor(brush);
            CBLock = CB.Brush;
        }

        public void Brush_Changed(object sender, EventArgs e)
        {
            if (sender is Brush && !(sender as Brush).IsFrozen)
            {
                updateColor(Serializer.Brush.Create(sender as Brush).CreateBrush());
            }
        }

        private void setColor(Brush color)
        {
            updateColor(color);
            CBSetBrush(color);
        }

        void CBSetBrush(Brush brush)
        {
            if (brush != null)
            {
                if (brush is SolidColorBrush)
                {
                    brush.Freeze();
                }
                else
                {
                    brush = Serializer.Brush.Create(brush).CreateBrush();
                    if (!brush.IsFrozen)
                    {
                        brush.Changed += Brush_Changed;
                    }
                }
            }
            CB.Brush = brush;
        }

        private void updateColor(Brush color)
        {
            switch (activeColor)
            {
                case BrushEnum.PRIMARY:
                    control.SetPrimaryBrush(color);
                    primaryColor.Fill = color;
                    break;
                case BrushEnum.SECONDARY:
                    control.SetSecondaryBrush(color);
                    secondaryColor.Fill = color;
                    break;
                case BrushEnum.BACKGROUND:
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
                double x = e.GetPosition(canvas_out).X;
                double y = e.GetPosition(canvas_out).Y;
                control.SetResolution(x > 0 ? x : 1, y > 0 ? y : 1);
            }
            else
            {
                control.MouseMove(e);
            }
        }

        private void canvas_outer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (resolutionDrag)
            {
                resolutionDrag = false;
                control.SetResolutionEnd();
            }
            else
            {
                control.MouseUp(e);
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

        public void SetHistory(bool b, bool f)
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
            int i = layers.SelectedIndex;
            if (control != null) control.LayerChanged(i);
        }

        private void zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (control != null)
            {
                double x = canvas_outer.ContentHorizontalOffset / canvas_outer.ScrollableWidth;
                double y = canvas_outer.ContentVerticalOffset / canvas_outer.ScrollableHeight;
                control.SetZoom(e.NewValue / 100);
                canvas_outer.ScrollToHorizontalOffset(canvas_outer.ScrollableWidth * (double.IsNaN(x) ? 0 : x));
                canvas_outer.ScrollToVerticalOffset(canvas_outer.ScrollableHeight * (double.IsNaN(y) ? 0 : y));
            }
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


        private void Button_layer_rename(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Layer l = button.DataContext as Layer;
            var dialog = new Dialog();
            dialog.ResponseText = l.Name;
            Point pos = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
            dialog.Left = pos.X;
            dialog.Top = pos.Y;

            if (dialog.ShowDialog() == true)
            {
                l.SetName(dialog.ResponseText, true);
            }
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

        public void SetPrimaryBrush(Brush brush)
        {
            primaryColor.Fill = brush;
            if (activeColor == BrushEnum.PRIMARY)
            {
                CBSetBrush(brush);
            }
        }

        public void SetSecondaryBrush(Brush brush)
        {
            secondaryColor.Fill = brush;
            if (activeColor == BrushEnum.SECONDARY)
            {
                CBSetBrush(brush);
            }
        }

        public void SetBackgroundBrush(Brush brush)
        {
            backgroundColor.Fill = brush;
            if (activeColor == BrushEnum.BACKGROUND)
            {
                CBSetBrush(brush);
            }
        }

        public void SetThickness(double t)
        {
            thickness.Value = t;
        }

        public void SetFont(FontFamily f)
        {
            if (font.SelectedValue != f)
            {
                fontLock = true;
                font.SelectedValue = f;
            }
        }

        public void SetFontSize(double s)
        {
            font_size.Value = s;
        }

        private void font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (!fontLock)
            {
                FontFamily f = (FontFamily)font.SelectedValue;
                control.SetTextFont(f);

            }
            else
            {
                fontLock = false;
            }

        }

        private void font_size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (control != null) control.SetTextFontSize(e.NewValue);
        }

        public void ShowFontPanel(bool t)
        {
            font_panel.Visibility = t ? Visibility.Visible : Visibility.Hidden;
        }

        private void mouseRightUp(object sender, MouseButtonEventArgs e)
        {
            if (control.ContextMenuShape())
            {
                ContextMenu cm = FindResource("cmShape") as ContextMenu;
                cm.IsOpen = true;
            }
        }

        private void cut(object sender, RoutedEventArgs e)
        {
            control.Cut();
        }

        private void delete(object sender, RoutedEventArgs e)
        {
            control.Delete();
        }

        private void copy(object sender, RoutedEventArgs e)
        {
            control.Copy();
        }

        private void shape_top(object sender, RoutedEventArgs e)
        {
            control.SetShapeTop();
        }

        private void shape_bottom(object sender, RoutedEventArgs e)
        {
            control.SetShapeBottom();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (e.Delta > 0)
                {
                    scrollviewer.LineLeft();
                    scrollviewer.LineLeft();
                }
                else
                {
                    scrollviewer.LineRight();
                    scrollviewer.LineRight();
                }
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                {
                    zoom.Value = zoom.Value + 10;
                }
                else
                {
                    zoom.Value = zoom.Value - 10;
                }
                e.Handled = true;
            }
        }

        private void canvas_outer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            control.MouseMove();
        }
    }
}
