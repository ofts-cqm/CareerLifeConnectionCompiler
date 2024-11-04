using System.Text;

namespace CLCC
{
    public class Content
    {
        private static string[] fileContent = Array.Empty<string>();
        private static int currentLine = 0;
        private static int currentPosition = 0;

        public static string Current => fileContent[currentLine];

        public static void Init()
        {
            fileContent = Array.Empty<string>();
            currentLine = 0;
            currentPosition = 0;
        }

        public static void Analyze(string path)
        {
            try
            {
                fileContent = File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Cannot read file {path}");
                Console.WriteLine(ex.ToString());
                return;
            }

            for (int i = 0; i < fileContent.Length; i++)
            {
                Fix();
                currentLine++;
            }
            currentLine = 0;
        }

        public static void Fix()
        {
            throw new NotImplementedException();
        }

        public static bool IsEnd() => currentLine == fileContent.Length;

        public static bool Match(string content)
        {
            if (Get(content.Length) == content)
            {
                Cut(content.Length);
                return true;
            }
            return false;
        }

        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.WriteLine($"At line {currentLine} character {currentPosition}:");
            Console.WriteLine(Current);
            char[] pos = new char[Current.Length];
            Array.Fill(pos, '~');
            pos[currentPosition] = '^';
            Console.WriteLine(pos);
            Console.ResetColor();
        }

        public static void LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warn: {message}");
            Console.WriteLine($"At line {currentLine} character {currentPosition}:");
            Console.WriteLine(Current);
            char[] pos = new char[Current.Length];
            Array.Fill(pos, '~');
            pos[currentPosition] = '^';
            Console.WriteLine(pos);
            Console.ResetColor();
        }

        public static string Get(int length)
        {
            if (length <= Current.Length) return Current.Substring(currentPosition, length);

            StringBuilder builder = new(Current[currentPosition..]);
            int oldPosition = currentPosition, oldLine = currentLine;
            while (length >= Current.Length)
            {
                builder.Append(Current).Append(' ');
                length -= Current.Length;
                currentLine++;
            }
            builder.Append(Current.AsSpan(currentPosition, length));
            currentPosition = oldPosition;
            currentLine = oldLine;
            return builder.ToString();
        }

        public static void Cut(int length)
        {
            currentPosition += length;
            while (currentPosition >= Current.Length)
            {
                currentPosition -= Current.Length;
                currentLine++;
            }
        }
    }
}
