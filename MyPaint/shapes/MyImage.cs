using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MyPaint
{
    class MyImage : MyShape
    {
        MovePoint p1, p2, p3, p4;
        DrawControl drawControl;
        Polygon p = new Polygon(), lv;
        ImageBrush image;
        MyLayer layer;
        bool hit = false;

        public MyImage(DrawControl c, MyLayer la, ImageBrush im, Point start, double w, double h)
        {
            drawControl = c;
            layer = la;

            p.Points.Add(new Point(start.X, start.Y));
            p.Points.Add(new Point(start.X, start.Y + h));
            p.Points.Add(new Point(start.X + w, start.Y + h));
            p.Points.Add(new Point(start.X+w, start.Y));
            
            

            p.Fill = im;
            layer.canvas.Children.Add(p);
        }

        public MyImage(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;

            byte[] imageBytes = Convert.FromBase64String(s.b64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            ImageBrush brush = new ImageBrush(bmi);

            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));

            p.Fill = brush;
            layer.canvas.Children.Add(p);
        }

        public void delete()
        {
            layer.shapes.Remove(this);
            layer.canvas.Children.Remove(p);
            if (p1 != null)
            {
                stopDraw();
            }
            else
            {
                deleteVirtualShape();
            }
        }

        public bool hitTest()
        {
            return hit;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void mouseMove(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void moveDrag(MouseEventArgs e)
        {
            p1.move(e);
            p2.move(e);
            p3.move(e);
            p4.move(e);
        }

        public void moveShape(double x, double y)
        {
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            p1.move(x, y);
            p2.move(p.Points[1].X, p.Points[1].Y);
            p3.move(p.Points[2].X, p.Points[2].Y);
            p4.move(p.Points[3].X, p.Points[3].Y);
        }

        public void setHit(bool h)
        {
            hit = h;
        }

        public void setPrimaryColor(System.Windows.Media.Brush b)
        {
            
        }

        public void setSecondaryColor(System.Windows.Media.Brush b)
        {
            
        }

        public void changeLayer(MyLayer newLayer)
        {
            if (layer != null)
            {
                layer.canvas.Children.Remove(p);
                layer.shapes.Remove(this);
            }
            layer = newLayer;
            if (layer != null)
            {
                layer.canvas.Children.Add(p);
                layer.shapes.Add(this);
            }
        }

        public void setThickness(double b)
        {
            
        }

        public void stopDrag()
        {
            hit = false;
            p1.drag = false;
            p2.drag = false;
            p3.drag = false;
            p4.drag = false;
        }

        public void stopDraw()
        {
            deleteVirtualShape();
            p1.delete();
            p2.delete();
            p3.delete();
            p4.delete();
        }

        jsonSerialize.Shape MyShape.renderShape()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)p.RenderSize.Width,
            (int)p.RenderSize.Height, 96, 96, PixelFormats.Default);
            rtb.Render(p);
            

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] f = null;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                f = stream.ToArray();
            }
            
            string base64String = Convert.ToBase64String(f);
            jsonSerialize.Image ret = new jsonSerialize.Image();
            ret.A = new jsonSerialize.Point(0, 0);
            ret.b64 = base64String;
            return ret;
        }

        public void refresh()
        {
            layer.shapes.Add(this);
            layer.canvas.Children.Add(p);
            drawControl.lockDraw();
        }

        public void createVirtualShape(MyOnMouseDown mouseDown)
        {
            lv = new Polygon();
            lv.Points = p.Points;
            lv.Stroke = drawControl.nullBrush;
            lv.Fill = drawControl.nullBrush;
            lv.Cursor = Cursors.SizeAll;
            lv.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                mouseDown(ee, this);
                hit = true;
            };
            drawControl.topCanvas.Children.Add(lv);
        }

        public void deleteVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(lv);
            lv = null;
        }

        public void setActive()
        {
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(p.Points[0], e.GetPosition(layer.canvas));
            });
            drawControl.candraw = false;
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

        public void startMove(MouseButtonEventArgs e)
        {
            drawControl.startMoveShape(p.Points[0], e.GetPosition(layer.canvas));
        }
    }
}
