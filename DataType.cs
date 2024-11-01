using CLCC.tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC
{
    public struct DataType
    {
        public bool isPrimitive { get; set; }
        public string name { get; set; }

        public DataType(string name, bool isPrimitive = true)
        {
            this.isPrimitive = isPrimitive;
            this.name = name;
        }

        public static bool operator==(DataType left, DataType right)
        {
            return left.isPrimitive == right.isPrimitive && left.name == right.name;
        }

        public static bool operator!=(DataType left, DataType right) => !(left == right);

        public override string ToString()
        {
            return isPrimitive ? name : "class " + name;
        }
    }
}
