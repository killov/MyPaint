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
    public class Rectangle : Shape
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs;
        MovePoint p1, p2, p3, p4;
        public Rectangle(FileControl c, Layer la) : base(c, la)
        {

        }

        public Rectangle(FileControl c, Layer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            createVirtualShape();
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);

            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            
            

            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;

            addToCanvas(p);
            createPoints();
            
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
            PointCollection points = new PointCollection(4);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            p.Stroke = drawControl.getShapePrimaryColor();
            p.Fill = drawControl.getShapeSecondaryColor();
            p.StrokeThickness = drawControl.getShapeThickness();
            p.Points = points;
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            addToCanvas(p);
            startDraw();
        }

        override public void drawMouseMove(Point e)
        {
            p.Points[1] = new Point(p.Points[1].X, e.Y);
            p.Points[2] = e;
            p.Points[3] = new Point(e.X, p.Points[3].Y);
        }

        override public void drawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            stopDraw();
            createPoints();
            createVirtualShape();
            setActive();
        }

        override public void createVirtualShape()
        {

            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
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
            p1.show();
            p2.show();
            p3.show();
            p4.show();
        }

        override public void moveDrag(Point e)
        {
            base.moveDrag(e);
            p1.move(e);
            p2.move(e);
            p3.move(e);
            p4.move(e);
        }

        override public void stopDrag()
        {
            base.stopDrag();
            p1.stopDrag();
            p2.stopDrag();
            p3.stopDrag();
            p4.stopDrag();
        }

        override public void stopEdit()
        {
            base.stopEdit();
            p1.hide();
            p2.hide();
            p3.hide();
            p4.hide();
        }

        override public void moveShape(double x, double y)
        {
            base.moveShape(x, y);
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            p1.move(x, y);
            p2.move(p.Points[1].X, p.Points[1].Y);
            p3.move(p.Points[2].X, p.Points[2].Y);
            p4.move(p.Points[3].X, p.Points[3].Y);
        }

        override public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Rectangle ret = new jsonSerialize.Rectangle();
            ret.lineWidth = getThickness();
            ret.stroke = sPrimaryColor;
            ret.fill = sSecondaryColor;
            ret.A = new jsonSerialize.Point(p1.getPosition());
            ret.B = new jsonSerialize.Point(p3.getPosition());
            return ret;
        }

        override public Point getPosition()
        {
            return p.Points[0];
        }

        override public void createPoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, p.Points[0], drawControl.revScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.move(po.X, po.Y);
                p2.move(po.X, p.Points[1].Y);
                p4.move(p.Points[3].X, po.Y);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, p.Points[1], drawControl.revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.move(po.X, po.Y);
                p1.move(po.X, p.Points[0].Y);
                p3.move(p.Points[2].X, po.Y);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, p.Points[2], drawControl.revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.move(po.X, po.Y);
                p4.move(po.X, p.Points[3].Y);
                p2.move(p.Points[1].X, po.Y);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, p.Points[3], drawControl.revScale, (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p4.move(po.X, po.Y);
                p3.move(po.X, p.Points[2].Y);
                p1.move(p.Points[0].X, po.Y);
            });
        }

        override public void create(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(p1.getPosition());
            p.Points.Add(p2.getPosition());
            p.Points.Add(p3.getPosition());
            p.Points.Add(p4.getPosition());

            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }

    }
}
