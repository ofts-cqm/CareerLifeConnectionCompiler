namespace CLCC
{
    public struct Destination
    {
        public int OffSet { get; set; }
        public byte Type { get; set; }

        public const int CLOSE = 0;
        public const int REGISTER = 1;
        public const int STACK = 2;
        public const int HEAP = 3;

        public static string[] RegisterName = { "eax", "ebx", "ecx", "edx", "edi", "esi" };
    }
}
