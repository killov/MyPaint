using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public interface IHistoryNodeSkipped : IHistoryNode
    {
        void Skip(IHistoryNodeSkipped node);

        bool Optimal(IHistoryNodeSkipped node);
    }
}
