namespace FirstTask.Tests
{
    public class CompressAndDecompressTests
    {
        [Fact]
        public void Decompress_CompressAndDecompressOriginalString_ReturnsOriginal()
        {
            // Интеграционный тест: сжимаем строку, затем декомпрессим
            string original = "aaabbcccdde";
            string compressed = StringCompressionUtility.Compress(original);
            string decompressed = StringCompressionUtility.Decompress(compressed);

            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Decompress_CompressAndDecompressEmptyString_ReturnsEmpty()
        {
            string original = "";
            string compressed = StringCompressionUtility.Compress(original);
            string decompressed = StringCompressionUtility.Decompress(compressed);

            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Decompress_CompressAndDecompressNullString_ReturnsNull()
        {
            string original = null;
            string compressed = StringCompressionUtility.Compress(original);
            string decompressed = StringCompressionUtility.Decompress(compressed);

            Assert.Equal(original, decompressed);
        }
    }
}
