using Xunit;

namespace KnowledgeHubV2.Tests
{
    public class SimpleTest
    {
        [Fact]
        public void BasicTest_ShouldPass()
        {
            // Arrange
            int a = 2;
            int b = 3;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(5, result);
        }
    }
} 