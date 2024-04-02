using Shouldly;
using VP.Common.Helpers;
using Xunit;

namespace VP.Common.Test.Helpers
{
    public class FileHelperTests
    {
        [Fact]
        public void IsFileUsing_FileInUseNoShare()
        {
            // Arrange
            var filePath = @"Tests\Helpers\FileHelper\IsFileUsing_FileInUseNoShare";
            Directory.CreateDirectory(@"Tests\Helpers\FileHelper");
            File.Create(filePath).Dispose();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            try
            {
                // Act
                // Assert
                FileHelper.IsFileUsing(filePath).ShouldBeTrue();
            }
            finally
            {
                // Dispose
                fileStream.Dispose();
                File.Delete(filePath);
            }
        }

        [Fact]
        public void IsFileUsing_FileInUseWithShare()
        {
            // Arrange
            var filePath = @"Tests\Helpers\FileHelper\IsFileUsing_FileInUseWithShare";
            Directory.CreateDirectory(@"Tests\Helpers\FileHelper");
            File.Create(filePath).Dispose();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            try
            {
                // Act
                // Assert
                FileHelper.IsFileUsing(filePath).ShouldBeTrue();
            }
            finally
            {
                // Dispose
                fileStream.Dispose();
                File.Delete(filePath);
            }
        }

        [Fact]
        public void IsFileUsing_FileNotExists()
        {
            // Arrange
            var filePath = @"Tests\Helpers\FileHelper\IsFileUsing_FileNotExists";
            // Act
            // Assert
            FileHelper.IsFileUsing(filePath).ShouldBeFalse();
        }

        [Fact]
        public void IsFileUsing_FileNotInUse()
        {
            // Arrange
            var filePath = @"Tests\Helpers\FileHelper\IsFileUsing_FileNotInUse";
            Directory.CreateDirectory(@"Tests\Helpers\FileHelper");
            File.Create(filePath).Dispose();
            try
            {
                // Act
                // Assert
                FileHelper.IsFileUsing(filePath).ShouldBeFalse();
            }
            finally
            {
                // Dispose
                File.Delete(filePath);
            }
        }
    }
}