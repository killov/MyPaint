using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public interface IHistoryNode
    {
        void Back();

        void Forward();
    }
}
