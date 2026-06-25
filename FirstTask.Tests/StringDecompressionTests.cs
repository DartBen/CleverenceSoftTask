namespace FirstTask.Tests
{
    public class StringDecompressionTests
    {
        [Fact]
        public void Decompress_InputIsNull_ReturnsNull()
        {
            string input = null;

            var result = StringCompressionUtility.Decompress(input);

            Assert.Null(result);
        }

        [Fact]
        public void Decompress_InputIsEmpty_ReturnsEmpty()
        {
            string input = "";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("", result);
        }

        [Fact]
        public void Decompress_InputIsSingleCharacter_ReturnsSameCharacter()
        {
            string input = "a";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("a", result);
        }

        [Fact]
        public void Decompress_InputIsCharacterAndCount_ReturnsRepeatedCharacter()
        {
            string input = "a2";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aa", result);
        }

        [Fact]
        public void Decompress_InputHasTwoDifferentCharacters_ReturnsBothCharacters()
        {
            string input = "ab";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("ab", result);
        }

        [Fact]
        public void Decompress_InputHasMultipleGroups_ReturnsOriginalString()
        {
            // Сжатая строка: a3b2c3d2e
            string input = "a3b2c3d2e";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aaabbcccdde", result);
        }

        [Fact]
        public void Decompress_InputHasGroupsAndSingleChars_ReturnsOriginalString()
        {
            // Сжатая строка: a2b3c4d5e5
            string input = "a2b3c4d5e5";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aabbbccccdddddeeeee", result);
        }

        [Fact]
        public void Decompress_InputHasLongSingleSequence_ReturnsOriginalString()
        {
            string input = "a10"; // Сжатая форма 10 'a'

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aaaaaaaaaa", result);
        }

        [Fact]
        public void Decompress_InputHasMixedLongAndShortSequences_ReturnsOriginalString()
        {
            // Сжатая строка: a3b3c3a7z3
            string input = "a3b3c3a7z3";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aaabbbcccaaaaaaazzz", result);
        }

        [Fact]
        public void Decompress_InputHasSingleCharAtEnd_ReturnsOriginalString()
        {
            // Сжатая строка: a3b2c3d2e (из примера задачи)
            string input = "a3b2c3d2e";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aaabbcccdde", result);
        }

        [Fact]
        public void Decompress_InputHasSingleCharAtBeginning_ReturnsOriginalString()
        {
            // Сжатая строка: ef3g3
            string input = "ef3g3";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("efffggg", result);
        }

        [Fact]
        public void Decompress_InputHasSingleCharInMiddle_ReturnsOriginalString()
        {
            // Сжатая строка: a2b3cf3g3
            string input = "a2b3cf3g3";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("aabbbcfffggg", result);
        }

        [Fact]
        public void Decompress_InputHasSingleDigitAfterCharacter_ReturnsOriginalString()
        {
            // Хотя сжатие не генерирует "a1", декомпрессия должна его обработать
            string input = "a1b";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal("ab", result);
        }

        [Fact]
        public void Decompress_InputHasMultiDigitNumber_ReturnsOriginalString()
        {
            // Пример с числом больше 9
            string input = "a12";

            var result = StringCompressionUtility.Decompress(input);

            Assert.Equal(new string('a', 12), result); // "aaaaaaaaaaaa"
        }
    }
}
