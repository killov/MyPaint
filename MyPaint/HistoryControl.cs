using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public class HistoryControl
    {
        MainControl control;
        Stack<HistoryNode> backStack = new Stack<HistoryNode>();
        Stack<HistoryNode> forwardStack = new Stack<HistoryNode>();

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

        public void add(MyShape s)
        {
            backStack.Push(new HistoryNode(s));
            forwardStack.Clear();
            redraw();
        }

        public void back()
        {
            if (backStack.Count > 0)
            {
                HistoryNode node = backStack.Pop();
                node.back();
                forwardStack.Push(node);
            }
            redraw();
        }

        public void forward()
        {
            if (forwardStack.Count > 0)
            {
                HistoryNode node = forwardStack.Pop();
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
