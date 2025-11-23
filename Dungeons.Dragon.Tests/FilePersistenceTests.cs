using Dungens.Dragon.Repository.Implementations;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Dungeons.Dragon.Tests
{
    public class FilePersistenceTests
    {
        [Fact]
        public void FolderIsCreated_WhenSavingData()
        {
            var logger = new LoggerFactory().CreateLogger<FilePersistence>();
            var persistence = new FilePersistence(logger);

            string fileName = "test.json";
            persistence.SaveData("[]", fileName);

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData");
            Assert.True(Directory.Exists(folderPath)); // Folder should exist
            Assert.True(File.Exists(Path.Combine(folderPath, fileName))); // File should exist
        }
    }
}
