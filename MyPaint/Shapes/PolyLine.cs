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
    public class PolyLine : Shape
    {
        
        List<MovePoint> movepoints = new List<MovePoint>();
        bool start = false;

        Path path = new Path(), vs;
        PathFigure pf;
        LineSegment ls;
        bool fclick;
        public PolyLine(FileControl c, Layer la) : base(c, la)
        {

        }

        public PolyLine(FileControl c, Layer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetThickness(s.lineWidth);
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetSecondaryColor(s.fill == null ? null : s.fill.CreateBrush());
            SetThickness(s.lineWidth);

            PathGeometry p = new PathGeometry();
           
            ls = new LineSegment();
            
            bool f = true;
            foreach (var point in s.points)
            {
                if (f)
                {
                    pf = new PathFigure();
                    pf.StartPoint = new Point(point.x, point.y);
                    p.Figures.Add(pf);
                    path.Data = p;
                    f = false;
                    return;
                }
                ls.Point = new Point(point.x, point.y);
                pf.Segments.Add(ls);
            }
            path.ToolTip = null;

            AddToCanvas(path);
            CreatePoints();
            CreateVirtualShape();
        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            base.SetPrimaryColor(s, addHistory);
            path.Stroke = s;
        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            base.SetSecondaryColor(s, addHistory);
            path.Fill = s;
        }

        override public void AddToCanvas()
        {
            AddToCanvas(path);
        }

        override public void RemoveFromCanvas()
        {
            RemoveFromCanvas(path);
        }

        override public void SetThickness(double s, bool addHistory = false)
        {
            base.SetThickness(s, addHistory);
            path.StrokeThickness = s;
            if(vs != null) vs.StrokeThickness = s;
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            path.Stroke = drawControl.GetShapePrimaryColor();
            path.StrokeThickness = drawControl.GetShapeThickness();
            path.ToolTip = null;
            PathGeometry p = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            p.Figures.Add(pf);
            path.Data = p;
            AddToCanvas(path);
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);
            fclick = true;
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
            if (fclick)
            {
                fclick = false;
                start = true;
                return;
            }
            
            
            
            if (ee.ChangedButton == MouseButton.Right)
            {
                if (start)
                {

                    path.ToolTip = null;
                    path.Cursor = Cursors.SizeAll;


                    StopDraw();
                    CreatePoints();
                    CreateVirtualShape();
                    SetActive();
                }
            }
            else
            {
                ls.Point = e;
                ls = new LineSegment();
                ls.Point = e;
                pf.Segments.Add(ls);
            }

            
        }

        override public void CreateVirtualShape()
        {

            vs = new System.Windows.Shapes.Path();
            vs.Data = path.Data;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = path.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };

        }

        override public void ShowVirtualShape(MyOnMouseDown mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            drawControl.SetPrimaryColor(path.Stroke);
            drawControl.SetSecondaryColor(path.Fill);
            drawControl.SetThickness(path.StrokeThickness);
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
            for (int i = 0; i < pf.Segments.Count; i++)
            {
                LineSegment ls = ((LineSegment)pf.Segments[i]);
                Point po = new Point(ls.Point.X - pf.StartPoint.X + x, ls.Point.Y - pf.StartPoint.Y + y);
                
                ls.Point = po;
                movepoints[i+1].Move(po.X,po.Y);
            }
            pf.StartPoint = new Point(x, y);
            movepoints[0].Move(x, y);
        }

        override public jsonSerialize.Shape CreateSerializer()
        {
            jsonSerialize.Polygon ret = new jsonSerialize.Polygon();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryColor;
            ret.fill = SecondaryColor;
            ret.points = new List<jsonSerialize.Point>();
            foreach (var point in movepoints)
            {
                ret.points.Add(new jsonSerialize.Point(point.GetPosition()));
            }
            return ret;
        }

        override public Point GetPosition()
        {
            return pf.StartPoint;
        }

        override public void CreatePoints()
        {
            movepoints = new List<MovePoint>();
            MovePoint mp = new MovePoint(drawControl.topCanvas, this, pf.StartPoint, drawControl.revScale, (Point po) =>
            {
                pf.StartPoint = po;
                return true;
            });
            movepoints.Add(mp);
            for (int i = 0; i < pf.Segments.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            LineSegment ls = (LineSegment)pf.Segments[i];
            MovePoint mp = new MovePoint(drawControl.topCanvas, this, ls.Point, drawControl.revScale, (Point po) =>
            {
                ls.Point = po;
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
            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }
    }
}