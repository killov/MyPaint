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

        public void Clear()
        {
            changeBack = null;
            backStack.Clear();
            forwardStack.Clear();
            Redraw();
        }

        public void Add(IHistoryNode node)
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
                            Redraw();
                            return;
                        }
                    }
                }
                backStack.Push(node);
                forwardStack.Clear();
                Redraw();
            }
        }

        public void Back()
        {
            if (backStack.Count > 0)
            {
                IHistoryNode node = backStack.Pop();
                node.Back();
                forwardStack.Push(node);
            }
            Redraw();
        }

        public void Forward()
        {
            if (forwardStack.Count > 0)
            {
                IHistoryNode node = forwardStack.Pop();
                node.Forward();
                backStack.Push(node);
            }
            Redraw();
        }

        public void SetNotChange()
        {
            if(backStack.Count == 0)
            {
                changeBack = null;
            }
            else
            {
                changeBack = backStack.First();
            }
        }

        public bool Change()
        {
            return !((backStack.Count == 0 && changeBack == null) || backStack.First().Equals(changeBack));
        }

        public void Redraw()
        {
            control.SetHistory(backStack.Count > 0, forwardStack.Count > 0);
        }

        public void Enable()
        {
            enable = true;
        }
    }
}
