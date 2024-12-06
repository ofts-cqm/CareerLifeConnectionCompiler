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
        void DumpValue();
        bool ProxyDecodeDestination(Destination destination, StringBuilder file, out string value, int pos = 3);
    }
}
