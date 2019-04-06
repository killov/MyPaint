using System.Collections.Generic;
using System.Linq;

namespace MyPaint.History
{
    public class HistoryControl
    {
        MainControl control;
        FileControl file;
        Stack<IHistoryNode> backStack = new Stack<IHistoryNode>();
        Stack<IHistoryNode> forwardStack = new Stack<IHistoryNode>();
        IHistoryNode changeBack;
        bool enable = false;
        bool change = false;

        public HistoryControl(FileControl file, MainControl c)
        {
            this.file = file;
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
                if (backStack.Count > 0 && (node is IHistoryNodeSkipped))
                {
                    IHistoryNode last = backStack.First();
                    if (last is IHistoryNodeSkipped)
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
            if (backStack.Count == 0)
            {
                changeBack = null;
            }
            else
            {
                changeBack = backStack.First();
            }
            RefreshChange();
        }

        private void RefreshChange()
        {
            bool newChange = !((backStack.Count == 0 && changeBack == null) || (backStack.Count != 0 && backStack.First().Equals(changeBack)));
            bool changed = newChange != change;

            if (changed)
            {
                change = newChange;
                file.RefreshTab();
            }
        }

        public bool Change()
        {
            return change;
        }

        public void Redraw()
        {
            RefreshChange();
            control.SetHistory(backStack.Count > 0, forwardStack.Count > 0);
        }

        public void Enable()
        {
            enable = true;
        }
    }
}
