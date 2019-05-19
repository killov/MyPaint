using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeBrush : IHistoryNodeSkipped
    {
        public Shapes.Shape shape;
        public Brush oldBrush, newBrush;
        public BrushEnum brushEnum;
        public HistoryShapeBrush(Shapes.Shape shape, BrushEnum brushEnum, Brush oldBrush, Brush newBrush)
        {
            this.shape = shape;
            this.oldBrush = oldBrush;
            this.newBrush = newBrush;
            this.brushEnum = brushEnum;
        }

        public void Back()
        {
            shape.SetBrush(brushEnum, oldBrush);
        }

        public void Forward()
        {
            shape.SetBrush(brushEnum, newBrush);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeBrush n = (HistoryShapeBrush)node;
            this.newBrush = n.newBrush;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeBrush) && ((HistoryShapeBrush)node).shape.Equals(shape) && ((HistoryShapeBrush)node).brushEnum.Equals(brushEnum);
        }
    }
}
