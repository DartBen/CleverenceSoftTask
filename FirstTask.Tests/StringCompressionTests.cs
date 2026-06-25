namespace FirstTask.Tests
{
    public class StringCompressionTests
    {
        [Fact]
        public void Compress_InputIsNull_ReturnsNull()
        {
            string input = null;

            var result = StringCompressionUtility.Compress(input);

            Assert.Null(result);
        }

        [Fact]
        public void Compress_InputIsEmpty_ReturnsEmpty()
        {
            string input = "";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("", result);
        }

        [Fact]
        public void Compress_InputIsSingleCharacter_ReturnsSameCharacter()
        {
            string input = "a";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a", result);
        }

        [Fact]
        public void Compress_InputHasTwoSameCharacters_ReturnsCharacterAndCount()
        {
            string input = "aa";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a2", result);
        }

        [Fact]
        public void Compress_InputHasTwoDifferentCharacters_ReturnsBothCharacters()
        {
            string input = "ab";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("ab", result);
        }

        [Fact]
        public void Compress_InputHasMultipleGroups_ReturnsCompressedString()
        {
            string input = "aaabbcccdde";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a3b2c3d2e", result);
        }

        [Fact]
        public void Compress_InputHasGroupsAndSingleChars_ReturnsCorrectlyCompressedString()
        {
            // Пример: "aabbbccccdddddeeeee" -> "a2b3c4d5e5"
            string input = "aabbbccccdddddeeeee";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a2b3c4d5e5", result);
        }

        [Fact]
        public void Compress_InputHasLongSingleSequence_ReturnsCharacterAndCount()
        {
            string input = "aaaaaaaaaa"; // 10 'a'

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a10", result);
        }

        [Fact]
        public void Compress_InputHasMixedLongAndShortSequences_ReturnsCorrectlyCompressedString()
        {
            // Пример: "aaabbbcccaaaaaaazzz" -> "a3b3c3a7z3"
            string input = "aaabbbcccaaaaaaazzz";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a3b3c3a7z3", result);
        }

        [Fact]
        public void Compress_InputHasSingleCharAtEnd_ReturnsCorrectlyCompressedString()
        {
            // Пример: "aaabbbcccdde" -> "a3b2c3d2e" (из примера задачи)
            string input = "aaabbbcccdde";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a3b3c3d2e", result);
        }

        [Fact]
        public void Compress_InputHasSingleCharAtBeginning_ReturnsCorrectlyCompressedString()
        {
            // Пример: "efffggg" -> "ef3g3"
            string input = "efffggg";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("ef3g3", result);
        }

        [Fact]
        public void Compress_InputHasSingleCharInMiddle_ReturnsCorrectlyCompressedString()
        {
            // Пример: "aabbbcfffggg" -> "a2b3cf3g3"
            string input = "aabbbcfffggg";

            var result = StringCompressionUtility.Compress(input);

            Assert.Equal("a2b3cf3g3", result);
        }
    }
}
