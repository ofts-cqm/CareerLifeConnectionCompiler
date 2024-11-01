﻿using CLCC.tokens;

namespace CLCC
{
    public class DataType
    {
        public static readonly DataType INT = new("int");
        public static readonly DataType FLOAT = new("float");
        public static readonly DataType BOOL = new("bool");
        public static readonly DataType NULL = new("null");

        public static List<DataType> RegisteredDataType = new() {INT, FLOAT, BOOL, NULL };

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

        public override bool Equals(object? obj)
        {
            return obj is DataType data && data == this;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() + (isPrimitive?1:0);
        }

        public static bool TryParseDataType(ref string str, out DataType parsedDatatype)
        {
            if (str.StartsWith("var "))
            {
                str = str[4..];
                Tokens.fixString(ref str);
                parsedDatatype = NULL;
                return true;
            }

            else if (str.StartsWith("void "))
            {
                str = str[5..];
                Tokens.fixString(ref str);
                parsedDatatype = NULL;
                return true;
            }

            foreach (DataType type in RegisteredDataType)
            {
                if (str.StartsWith(type.name + " "))
                {
                    str = str[(type.name.Length + 1)..];
                    Tokens.fixString(ref str);
                    parsedDatatype = type;
                    return true;
                }
            }
            parsedDatatype = NULL;
            return false;
        }
    }
}
