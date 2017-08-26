using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public interface IHistoryNode
    {
        void back();

        void forward();
    }
}
