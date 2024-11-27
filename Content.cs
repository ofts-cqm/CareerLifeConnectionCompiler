using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC
{
    public struct Pos
    {
        public readonly int line, pos;

        public Pos(int line, int pos)
        {
            this.line = line;
            this.pos = pos;
        }
    }

    public class Content
    {
        private static string[] fileContent = Array.Empty<string>();
        private static int currentLine = 0;
        private static int currentPosition = 0;
        private static Stack<KeyValuePair<int, int>> stack = new();

        public static string Current => currentLine < fileContent.Length ? fileContent[currentLine] : "";
        public static char CurrentChar => currentPosition < Current.Length ? Current[currentPosition] : (char)0;

        public static void Init()
        {
            fileContent = Array.Empty<string>();
            currentLine = 0;
            currentPosition = 0;
            stack.Clear();
        }

        public static Pos GetPos() => new Pos(currentLine, currentPosition);

        public static void Advance()
        {
            currentPosition++;
            if (currentPosition == Current.Length)
            {
                currentPosition = 0;
                currentLine++;
            }
        }

        public static void AdvanceRow()
        {
            currentLine++;
        }

        public static void Push()
        {
            stack.Push(new KeyValuePair<int, int>(currentLine, currentPosition));
        }

        public static void Pop()
        {
            KeyValuePair<int, int> pair = stack.Pop();
            currentLine = pair.Key;
            currentPosition = pair.Value;
        }

        public static void Ignore()
        {
            stack.Pop();
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

            while (!IsEnd())
            {
                Fix();
                currentLine++;
            }
            currentLine = 0;
        }

        public static bool Analyze()
        {
            List<string> read = new();
            bool compile = false;
            while (true)
            {
                Console.Write(">");
                string readValue = Console.ReadLine() ?? "";

                if (readValue == "")
                {
                    continue;
                }

                if (!readValue.StartsWith('.'))
                {
                    read.Add(readValue);
                    continue;
                }
                
                if (readValue.StartsWith(".end"))
                {
                    break;
                }
                else if(readValue == ".clear")
                {
                    Lexer.Reset();
                    Console.Clear();
                    read.Clear();
                    continue;
                }
                else if (readValue == ".compile")
                {
                    compile = true;
                    break;
                }
            }

            fileContent = read.ToArray();
            while (currentLine < fileContent.Length)
            {
                Fix();
                currentLine++;
            }
            currentLine = 0;
            return compile;
        }

        public static void Fix()
        {
            while (true)
            {
                if (currentLine >= fileContent.Length) break;

                if (CurrentChar == ' ' || CurrentChar == '\n' || CurrentChar == '\r' || CurrentChar == '\t') Advance();
                else if (Get(2) == "//") AdvanceRow();
                else if (Get(2) == "/*")
                {
                    Advance();
                    Advance();
                    while (Get(2) == "*/") Advance();
                }
                else break;
            }
        }

        public static bool IsEnd() => currentLine >= fileContent.Length;

        public static bool Match(string content)
        {
            string get = Get(content.Length);
            if (get == content)
            {
                Cut(content.Length);
                return true;
            }
            return false;
        }

        public static void LogError(string message, Pos? position = null)
        {
            int curPos = position?.pos ?? currentPosition;
            int curLine = position?.line ?? currentLine;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.WriteLine($"At line {curLine} character {curPos}:");
            Console.WriteLine(fileContent[curLine]);
            char[] pos = new char[fileContent[curLine].Length];
            Array.Fill(pos, '~');
            pos[curPos] = '^';
            Console.WriteLine(pos);
            Console.ResetColor();
        }

        public static void LogWarn(string message, Pos? position = null)
        {
            int curPos = position?.pos ?? currentPosition;
            int curLine = position?.line ?? currentLine;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warn: {message}");
            Console.WriteLine($"At line {curLine} character {curPos}:");
            Console.WriteLine(fileContent[curLine]);
            char[] pos = new char[fileContent[curLine].Length];
            Array.Fill(pos, '~');
            pos[curPos] = '^';
            Console.WriteLine(pos);
            Console.ResetColor();
        }

        public static string Get(int length)
        {
            if (length + currentPosition <= Current.Length) 
                return Current.Substring(currentPosition, length);

            StringBuilder builder = new(Current[currentPosition..]);
            Push();
            while (length + currentPosition >= Current.Length)
            {
                builder.Append(Current).Append(' ');
                length -= (Current.Length - currentPosition);
                currentLine++;
                currentPosition = 0;
                if (currentLine >= fileContent.Length)
                {
                    Pop();
                    return "";
                }
            }
            builder.Append(Current.AsSpan(currentPosition, length));
            Pop();
            return builder.ToString();
        }

        public static void Cut(int length)
        {
            while (length-- > 0) 
                Advance();
            Fix();
        }
    }
}
