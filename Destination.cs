using CLCC.tokens;
using System.Diagnostics.CodeAnalysis;

namespace CLCC
{
    public struct Destination
    {
        public int OffSet { get; set; } = 0;
        public byte Type { get; set; } = CLOSE;

        public const int CLOSE = 0;
        public const int REGISTER = 1;
        public const int STACK = 2;
        public const int HEAP = 3;
        public const int DEREFERENCE = 4;

        public static string[] RegisterName = { "eax", "ebx", "ecx", "edx", "edi", "esi" };

        public Destination()
        {
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Destination dest) return false;
            return OffSet == dest.OffSet && Type == dest.Type;
        }
    }
}
