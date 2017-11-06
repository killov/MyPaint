using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public interface IHistoryNodeSkipped : IHistoryNode
    {
        void skip(IHistoryNodeSkipped node);

        bool optimal(IHistoryNodeSkipped node);
    }
}
