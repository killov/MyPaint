﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A5E1DA3DE6399DE9EB98404FE4AD1503"
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace MyPaint {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 43 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_line;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_ellipse;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_rectangle;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_polygon;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas colors;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse secondaryColor;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse primaryColor;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle moreColors;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider thickness;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle res;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelPath;
        
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
            
            #line 43 "..\..\MainWindow.xaml"
            this.button_line.Click += new System.Windows.RoutedEventHandler(this.button_line_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.button_ellipse = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\MainWindow.xaml"
            this.button_ellipse.Click += new System.Windows.RoutedEventHandler(this.button_ellipse_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.button_rectangle = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\MainWindow.xaml"
            this.button_rectangle.Click += new System.Windows.RoutedEventHandler(this.button_rectangle_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.button_polygon = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\MainWindow.xaml"
            this.button_polygon.Click += new System.Windows.RoutedEventHandler(this.button_polygon_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.colors = ((System.Windows.Controls.Canvas)(target));
            return;
            case 12:
            this.secondaryColor = ((System.Windows.Shapes.Ellipse)(target));
            
            #line 64 "..\..\MainWindow.xaml"
            this.secondaryColor.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.secondaryColor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 13:
            this.primaryColor = ((System.Windows.Shapes.Ellipse)(target));
            
            #line 65 "..\..\MainWindow.xaml"
            this.primaryColor.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.primaryColor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 14:
            this.moreColors = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 66 "..\..\MainWindow.xaml"
            this.moreColors.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.moreColors_MouseDown);
            
            #line default
            #line hidden
            return;
            case 15:
            this.thickness = ((System.Windows.Controls.Slider)(target));
            
            #line 91 "..\..\MainWindow.xaml"
            this.thickness.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.thickness_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 16:
            this.canvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 99 "..\..\MainWindow.xaml"
            this.canvas.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.mouseDown);
            
            #line default
            #line hidden
            
            #line 99 "..\..\MainWindow.xaml"
            this.canvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.mouseMove);
            
            #line default
            #line hidden
            
            #line 99 "..\..\MainWindow.xaml"
            this.canvas.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.mouseUp);
            
            #line default
            #line hidden
            return;
            case 17:
            this.res = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 100 "..\..\MainWindow.xaml"
            this.res.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.poz_mouseDown);
            
            #line default
            #line hidden
            return;
            case 18:
            this.labelPath = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

