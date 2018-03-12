using System;
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
    public class MyPolygon : MyShape
    {
        Polygon p = new Polygon(), vs;
        List<MovePoint> movepoints = new List<MovePoint>();
        bool start = false;
        List<Point> points = new List<Point>();

        Path path;
        PathFigure pf;
        LineSegment ls;
        public MyPolygon(DrawControl c, MyLayer la) : base(c, la)
        {

        }

        public MyPolygon(DrawControl c, MyLayer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);

            foreach (var point in s.points)
            {
                p.Points.Add(new Point(point.x, point.y));
            }
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;

            addToCanvas(p);
            createPoints();
            createVirtualShape();
        }

        override public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            base.setPrimaryColor(s, addHistory);
            p.Stroke = s;
        }

        override public void setSecondaryColor(Brush s, bool addHistory = false)
        {
            base.setSecondaryColor(s, addHistory);
            p.Fill = s;
        }

        override public void addToCanvas()
        {
            addToCanvas(p);
        }

        override public void removeFromCanvas()
        {
            removeFromCanvas(p);
        }

        override public void setThickness(double s, bool addHistory = false)
        {
            base.setThickness(s, addHistory);
            p.StrokeThickness = s;
            if(vs != null) vs.StrokeThickness = s;
        }

        override public void drawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            startDraw();
            path = new Path();
            path.Stroke = drawControl.getShapePrimaryColor();
            path.StrokeThickness = drawControl.getShapeThickness();
            path.ToolTip = null;
            PathGeometry p = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            p.Figures.Add(pf);
            path.Data = p;
            addToCanvas(path);
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);
        }

        override public void drawMouseMove(Point e)
        {
            if (start)
            {
                ls.Point = e;
            }
        }

        override public void drawMouseUp(Point e, MouseButtonEventArgs ee)
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
                    removeFromCanvas(path);
                    p = new Polygon();
                    p.Stroke = drawControl.getShapePrimaryColor();
                    p.Fill = drawControl.getShapeSecondaryColor();
                    p.StrokeThickness = drawControl.getShapeThickness();
                    p.Points = ppoints;
                    addToCanvas(p);

                    p.ToolTip = null;
                    p.Cursor = Cursors.SizeAll;


                    stopDraw();
                    createPoints();
                    createVirtualShape();
                    setActive();
                }
            }

            
        }

        override public void createVirtualShape()
        {

            vs = new Polygon();
            vs.Points = p.Points;
            vs.Stroke = drawControl.nullBrush;
            vs.Fill = drawControl.nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };

        }

        override public void showVirtualShape(MyOnMouseDown mouseDown)
        {
            base.showVirtualShape(mouseDown);
            hideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void hideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void setActive()
        {
            base.setActive();
            drawControl.setPrimaryColor(p.Stroke);
            drawControl.setSecondaryColor(p.Fill);
            drawControl.setThickness(p.StrokeThickness);
            foreach (MovePoint p in movepoints)
            {
                p.show();
            }
        }

        override public void moveDrag(Point e)
        {
            base.moveDrag(e);
            foreach (var p in movepoints)
            {
                p.move(e);
            }
        }

        override public void stopDrag()
        {
            base.stopDrag();
            foreach (var p in movepoints)
            {
                p.stopDrag();
            }
        }

        override public void stopEdit()
        {
            base.stopEdit();
            foreach (var p in movepoints)
            {
                p.hide();
            }
        }

        override public void moveShape(double x, double y)
        {
            base.moveShape(x, y);
            for (int i = 1; i < p.Points.Count; i++)
            {
                Point po = new Point(p.Points[i].X - p.Points[0].X + x, p.Points[i].Y - p.Points[0].Y + y);
                p.Points[i] = po;
                movepoints[i].movee(po);
            }
            p.Points[0] = new Point(x, y);
            movepoints[0].move(x, y);
        }

        override public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Polygon ret = new jsonSerialize.Polygon();
            ret.lineWidth = getThickness();
            ret.stroke = sPrimaryColor;
            ret.fill = sSecondaryColor;
            ret.points = new List<jsonSerialize.Point>();
            foreach (var point in movepoints)
            {
                ret.points.Add(new jsonSerialize.Point(point.getPosition()));
            }
            return ret;
        }

        override public Point getPosition()
        {
            return p.Points[0];
        }

        override public void createPoints()
        {
            movepoints = new List<MovePoint>();
            for (int i = 0; i < p.Points.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            MovePoint mp = new MovePoint(drawControl.topCanvas, this, p.Points[i], drawControl.revScale, (Point po) =>
            {
                p.Points[i] = po;
            });
            movepoints.Add(mp);
        }

        override public void create(Canvas canvas)
        {
            Polygon p = new Polygon();

            foreach (var point in movepoints)
            {
                p.Points.Add(point.getPosition());
            }
            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }
    }
}
