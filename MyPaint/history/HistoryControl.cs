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
        IHistoryNode changeBack;
        private bool enable = false;

        public HistoryControl(MainControl c)
        {
            control = c;
        }

        public void clear()
        {
            changeBack = null;
            backStack.Clear();
            forwardStack.Clear();
            redraw();
        }

        public void add(IHistoryNode node)
        {
            if (enable)
            {   
                if(backStack.Count > 0 && (node is IHistoryNodeSkipped))
                {
                    IHistoryNode last = backStack.First();
                    if(last is IHistoryNodeSkipped)
                    {
                        IHistoryNodeSkipped l = (IHistoryNodeSkipped)last;
                        IHistoryNodeSkipped n = (IHistoryNodeSkipped)node;
                        if (l.Optimal(n))
                        {
                            l.Skip(n);
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
        }

        public void back()
        {
            if (backStack.Count > 0)
            {
                IHistoryNode node = backStack.Pop();
                node.Back();
                forwardStack.Push(node);
            }
            redraw();
        }

        public void forward()
        {
            if (forwardStack.Count > 0)
            {
                IHistoryNode node = forwardStack.Pop();
                node.Forward();
                backStack.Push(node);
            }
            redraw();
        }

        public void setNotChange()
        {
            if(backStack.Count == 0)
            {
                changeBack = null;
            }
            else
            {
                changeBack = backStack.Last();
            }
        }

        public bool change()
        {
            return !((backStack.Count == 0 && changeBack == null) || backStack.Last().Equals(changeBack));
        }

        public void redraw()
        {
            control.SetHistory(backStack.Count > 0, forwardStack.Count > 0);
        }

        public void Enable()
        {
            enable = true;
        }
    }
}
