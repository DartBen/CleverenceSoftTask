using System.Text;

namespace FirstTask
{
    /// <summary>
    /// Утилита для сжатия и восстановления строк.
    /// Группы одинаковых символов заменяются на "sc" (символ + количество).
    /// Если символ один — количество не указывается.
    /// Пример: "aaabbcccdde" → "a3b2c3d2e"
    /// </summary>
    public static class StringCompressionUtility
    {
        private readonly static HashSet<char> digits = new HashSet<char>()
            {'0','1','2','3', '4', '5', '6', '7', '8', '9'};

        /// <summary>
        /// Сжимает строку.
        /// </summary>
        public static string Compress(string input)
        {
            if (input == null)
                return null;
            if (input == string.Empty)
                return string.Empty;

            StringBuilder output = new StringBuilder();
            var inputSpan = input.AsSpan();

            uint charCount = 1;

            for (int i = 0; i < inputSpan.Length - 1; i++)
            {
                if (inputSpan[i].Equals(inputSpan[i + 1]))
                {
                    charCount++;
                    continue;
                }
                else if (charCount > 1)
                {
                    output.Append(inputSpan[i]);
                    output.Append(charCount);
                    charCount = 1;
                    continue;
                }
                else
                {
                    output.Append(inputSpan[i]);
                    continue;
                }
            }

            // Обработка последней группы
            if (charCount > 1)
            {
                output.Append(inputSpan[inputSpan.Length - 1]);
                output.Append(charCount);
            }
            else
            {
                output.Append(inputSpan[inputSpan.Length - 1]);
            }

            return output.ToString();
        }

        /// <summary>
        /// Восстанавливает строку из сжатого.
        /// </summary>
        public static string Decompress(string input)
        {
            if (input == null)
                return null;
            if (input == string.Empty)
                return string.Empty;

            StringBuilder output = new StringBuilder();
            var inputSpan = input.AsSpan();

            int startDigit = 0;

            for (int i = 0; i < inputSpan.Length;)
            {
                if (digits.Contains(inputSpan[i]))
                {
                    if (i == 0)
                    {
                        throw new FormatException("Invalid compressed string format: cannot start with a digit.");
                    }

                    startDigit = i;

                    while (i < inputSpan.Length && digits.Contains(inputSpan[i]))
                    {
                        i++;
                    }

                    var countSlice = inputSpan.Slice(startDigit, i - startDigit);

                    if (int.TryParse(countSlice, out int count))
                    {
                        // Записываем (count - 1) раз, т.к. один раз символ уже записан при встрече буквы
                        output.Append(inputSpan[startDigit - 1], count - 1);
                    }
                }
                else
                {
                    output.Append(inputSpan[i]);
                    i++;
                }
            }

            return output.ToString();
        }
    }
}