﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace MyPaint.Shapes
{
    public class Polygon : Shape
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs = new System.Windows.Shapes.Polygon();
        List<MovePoint> movepoints = new List<MovePoint>();
        bool start = false;
        List<Point> points = new List<Point>();

        Path path = new Path();
        PathFigure pf;
        LineSegment ls;
        public Polygon(FileControl c, Layer la) : base(c, la)
        {
            MultiDraw = true;
            Element = p;
        }

        public Polygon(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            Element = p;
            SetThickness(s.lineWidth);

            foreach (var point in s.points)
            {
                p.Points.Add(new Point(point.x, point.y));
            }
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;

            AddToLayer();
            CreatePoints();
            CreateVirtualShape();
        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            base.SetPrimaryColor(s, addHistory);
            p.Stroke = s;
            if (path != null) path.Stroke = s;
        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            base.SetSecondaryColor(s, addHistory);
            p.Fill = s;
            if (path != null) path.Fill = s;
        }

        override public void SetThickness(double s, bool addHistory = false)
        {
            base.SetThickness(s, addHistory);
            p.StrokeThickness = s;
            if(vs != null) vs.StrokeThickness = s;
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            PathGeometry p = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            p.Figures.Add(pf);
            path.Data = p;
            Element = path;
            AddToLayer();
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);
        }

        override public void DrawMouseMove(Point e)
        {
            if (start)
            {
                ls.Point = e;
            }
        }

        override public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            start = true;
            ls.Point = e;
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);

            points.Add(e);
            
            if (ee.ChangedButton == MouseButton.Right)
            {
                if (start)
                {
                    PointCollection ppoints = new PointCollection();
                    foreach (var p in points)
                    {
                        ppoints.Add(p);
                    }
                    Element = p;
 

                    p.Points = ppoints;

                    StopDraw();
                    CreatePoints();
                    CreateVirtualShape();
                    SetActive();
                }
            }
        }

        void CreateVirtualShape()
        {
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(File.Canvas), this);
                hit = true;
            };
        }

        override public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            File.TopCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            File.TopCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            File.SetPrimaryColor(p.Stroke);
            File.SetSecondaryColor(p.Fill);
            File.SetThickness(p.StrokeThickness);
            foreach (MovePoint p in movepoints)
            {
                p.Show();
            }
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            foreach (var p in movepoints)
            {
                p.MoveDrag(e);
            }
        }

        override public void StopDrag()
        {
            base.StopDrag();
            foreach (var p in movepoints)
            {
                p.StopDrag();
            }
        }

        override public void StopEdit()
        {
            base.StopEdit();
            foreach (var p in movepoints)
            {
                p.Hide();
            }
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            for (int i = 1; i < p.Points.Count; i++)
            {
                Point po = new Point(p.Points[i].X - p.Points[0].X + x, p.Points[i].Y - p.Points[0].Y + y);
                p.Points[i] = po;
                movepoints[i].Move(po.X,po.Y);
            }
            p.Points[0] = new Point(x, y);
            movepoints[0].Move(x, y);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Polygon ret = new Serializer.Polygon();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryColor;
            ret.fill = SecondaryColor;
            ret.points = new List<Serializer.Point>();
            foreach (var point in movepoints)
            {
                ret.points.Add(new Serializer.Point(point.GetPosition()));
            }
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        void CreatePoints()
        {
            movepoints = new List<MovePoint>();
            for (int i = 0; i < p.Points.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            MovePoint mp = new MovePoint(File.TopCanvas, this, p.Points[i], File.RevScale, (Point po) =>
            {
                p.Points[i] = po;
                return true;
            });
            movepoints.Add(mp);
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            foreach (var point in movepoints)
            {
                p.Points.Add(point.GetPosition());
            }
            p.Stroke = PrimaryColor.CreateBrush();
            p.Fill = SecondaryColor.CreateBrush();
            p.StrokeThickness = GetThickness();
            canvas.Children.Add(p);
        }
    }
}
