using CLCC.tokens;

namespace CLCC
{
    public class DataType
    {
        public static HashSet<DataType> RegisteredDataType { get; set; } = new();

        public static readonly DataType INT = new("int");
        public static readonly DataType FLOAT = new("float");
        public static readonly DataType BOOL = new("bool");
        public static readonly DataType NULL = new("null");
        public static readonly DataType NULLPTR = new("nullptr", false);

        public bool isPrimitive { get; set; }
        public string name { get; set; }

        public static void Init()
        {
            RegisteredDataType = new() { INT, FLOAT, BOOL, NULL, NULLPTR };
        }

        public DataType(string name, bool isPrimitive = true)
        {
            this.isPrimitive = isPrimitive;
            this.name = name;
            RegisteredDataType.Add(this);
        }

        public static bool operator==(DataType left, DataType right)
        {
            return left.isPrimitive == right.isPrimitive && left.name == right.name;
        }

        public static bool operator!=(DataType left, DataType right) => !(left == right);

        public override string ToString()
        {
            return isPrimitive ? name : "struct_" + name;
        }

        public override bool Equals(object? obj)
        {
            return obj is DataType data && data == this;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() + (isPrimitive?1:0);
        }

        public static bool TryParseDataType(out DataType parsedDatatype)
        {
            Content.Match("struct ");
            if (Content.Match("void "))
            {
                parsedDatatype = NULL;
                return true;
            }

            foreach (DataType type in RegisteredDataType)
            {
                if (Content.Match(type.name + ' ') || Content.Match(type.name + ')'))
                {
                    parsedDatatype = type;
                    return true;
                }
            }
            parsedDatatype = NULL;
            return false;
        }
    }
}
