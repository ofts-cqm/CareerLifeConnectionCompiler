using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    public interface IExpressionBaseToken: IToken
    {
        public DataType Type { get; set; }
    }
}
