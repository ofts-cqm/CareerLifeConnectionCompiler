using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    public interface IAssignable: IExpressionBaseToken
    {
        Destination GetDestination();
        void PrepareValue(StringBuilder file);
        void DumpValue(StringBuilder file);
    }
}
