using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryControl
    {
        MainControl control;
        Stack<IHistoryNode> backStack = new Stack<IHistoryNode>();
        Stack<IHistoryNode> forwardStack = new Stack<IHistoryNode>();

        public HistoryControl(MainControl c)
        {
            control = c;
        }

        public void clear()
        {
            backStack.Clear();
            forwardStack.Clear();
            redraw();
        }

        public void add(IHistoryNode node)
        {
            if(backStack.Count > 0 && (node is IHistoryNodeSkipped))
            {
                IHistoryNode last = backStack.First();
                if(last is IHistoryNodeSkipped)
                {
                    IHistoryNodeSkipped l = (IHistoryNodeSkipped)last;
                    IHistoryNodeSkipped n = (IHistoryNodeSkipped)node;
                    if (l.optimal(n))
                    {
                        l.skip(n);
                        forwardStack.Clear();
                        redraw();
                        return;
                    }
                }
            }
            backStack.Push(node);
            forwardStack.Clear();
            redraw();
        }

        public void back()
        {
            if (backStack.Count > 0)
            {
                IHistoryNode node = backStack.Pop();
                node.back();
                forwardStack.Push(node);
            }
            redraw();
        }

        public void forward()
        {
            if (forwardStack.Count > 0)
            {
                IHistoryNode node = forwardStack.Pop();
                node.forward();
                backStack.Push(node);
            }
            redraw();
        }

        void redraw()
        {
            control.setHistory(backStack.Count > 0, forwardStack.Count > 0);
        }
    }
}
