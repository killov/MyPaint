﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "587726B941CAB681D4C80B1589F7A279"
//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.42000
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
// </auto-generated>
//------------------------------------------------------------------------------

using ColorBox;
using MyPaint;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MyPaint {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_line;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_ellipse;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_rectangle;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_polygon;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_back;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path back;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_forward;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path forward;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle secondaryColor;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle primaryColor;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle backgroundColor;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider thickness;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas colors;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ColorBox.ColorBox CB;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_addLayer;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox layers;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_outer;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_out;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas res;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle resolution;
        
        #line default
        #line hidden
        
        
        #line 160 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelPath;
        
        #line default
        #line hidden
        
        
        #line 161 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelResolution;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MyPaint;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 11 "..\..\MainWindow.xaml"
            ((MyPaint.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.closed);
            
            #line default
            #line hidden
            
            #line 11 "..\..\MainWindow.xaml"
            ((MyPaint.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 23 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.newClick);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 24 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.openClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 25 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.saveClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 26 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.saveAsClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 27 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.exitClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.button_line = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\MainWindow.xaml"
            this.button_line.Click += new System.Windows.RoutedEventHandler(this.button_line_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.button_ellipse = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\MainWindow.xaml"
            this.button_ellipse.Click += new System.Windows.RoutedEventHandler(this.button_ellipse_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.button_rectangle = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\MainWindow.xaml"
            this.button_rectangle.Click += new System.Windows.RoutedEventHandler(this.button_rectangle_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.button_polygon = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\MainWindow.xaml"
            this.button_polygon.Click += new System.Windows.RoutedEventHandler(this.button_polygon_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.button_back = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\MainWindow.xaml"
            this.button_back.Click += new System.Windows.RoutedEventHandler(this.button_back_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.back = ((System.Windows.Shapes.Path)(target));
            return;
            case 13:
            this.button_forward = ((System.Windows.Controls.Button)(target));
            
            #line 62 "..\..\MainWindow.xaml"
            this.button_forward.Click += new System.Windows.RoutedEventHandler(this.button_forward_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.forward = ((System.Windows.Shapes.Path)(target));
            return;
            case 15:
            
            #line 78 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.secondaryColor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 16:
            this.secondaryColor = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 17:
            
            #line 84 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.primaryColor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 18:
            this.primaryColor = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 19:
            
            #line 90 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.backgroundColor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 20:
            this.backgroundColor = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 21:
            this.thickness = ((System.Windows.Controls.Slider)(target));
            
            #line 97 "..\..\MainWindow.xaml"
            this.thickness.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.thickness_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 22:
            this.colors = ((System.Windows.Controls.Canvas)(target));
            return;
            case 23:
            this.CB = ((ColorBox.ColorBox)(target));
            
            #line 103 "..\..\MainWindow.xaml"
            this.CB.ColorChanged += new ColorBox.ColorBox.ColorChangedEventHandler(this.CB_ColorChanged);
            
            #line default
            #line hidden
            return;
            case 24:
            this.button_addLayer = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\MainWindow.xaml"
            this.button_addLayer.Click += new System.Windows.RoutedEventHandler(this.button_addLayer_Click);
            
            #line default
            #line hidden
            return;
            case 25:
            this.layers = ((System.Windows.Controls.ListBox)(target));
            
            #line 107 "..\..\MainWindow.xaml"
            this.layers.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.layers_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 26:
            this.canvas_outer = ((System.Windows.Controls.Canvas)(target));
            
            #line 135 "..\..\MainWindow.xaml"
            this.canvas_outer.MouseMove += new System.Windows.Input.MouseEventHandler(this.canvas_outer_MouseMove);
            
            #line default
            #line hidden
            
            #line 135 "..\..\MainWindow.xaml"
            this.canvas_outer.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.canvas_outer_MouseUp);
            
            #line default
            #line hidden
            return;
            case 27:
            this.canvas_out = ((System.Windows.Controls.Canvas)(target));
            return;
            case 28:
            this.canvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 146 "..\..\MainWindow.xaml"
            this.canvas.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.mouseDown);
            
            #line default
            #line hidden
            
            #line 146 "..\..\MainWindow.xaml"
            this.canvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.mouseMove);
            
            #line default
            #line hidden
            
            #line 146 "..\..\MainWindow.xaml"
            this.canvas.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.mouseUp);
            
            #line default
            #line hidden
            return;
            case 29:
            this.res = ((System.Windows.Controls.Canvas)(target));
            
            #line 147 "..\..\MainWindow.xaml"
            this.res.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.poz_mouseDown);
            
            #line default
            #line hidden
            return;
            case 30:
            this.resolution = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 149 "..\..\MainWindow.xaml"
            this.resolution.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.resolution_MouseDown);
            
            #line default
            #line hidden
            return;
            case 31:
            this.labelPath = ((System.Windows.Controls.Label)(target));
            return;
            case 32:
            this.labelResolution = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

