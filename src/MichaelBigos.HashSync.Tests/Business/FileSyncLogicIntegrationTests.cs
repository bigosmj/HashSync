using MichaelBigos.HashSync.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO.Abstractions;
using FluentAssertions;

namespace MichaelBigos.HashSync.Tests.Business
{
    [TestClass]
    public class FileSyncLogicIntegrationTests
    {
        private IFileSystem fileSystem;
        private FileSyncLogic underTest;
        private string source;
        private string target;

        private const string testDataPath = @"D:\users\bigosmj\Documents\appdev\products\MichaelBigos.HashSync\src\MichaelBigos.HashSync.Tests\TestData";

        [TestInitialize]
        public void Setup()
        {
            fileSystem = new FileSystem();
            underTest = new FileSyncLogic(fileSystem);            
            source = fileSystem.Path.Combine(testDataPath, "Source");
            target = fileSystem.Path.Combine(testDataPath, "Target");
        }

        [TestMethod]
        public void SyncFoldersIntegrationTest()
        {
            //Arrange
            if (fileSystem.Directory.Exists(target))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = testDataPath,
                    FileName = "cmd.exe",
                    Arguments = "/c reset.bat"
                };

                Process.Start(startInfo).WaitForExit();
            }

            //Act

            underTest.SyncFolders(source, target);

            //Assert

            //Convention:
            //ends in 1 = is the same in source and target
            //ends in 2 = is only in the source; will be copied to target
            //ends in 3 = is only in the target; will be deleted from the target
            //ends in 4 = files that are a different size in the source and target; will be copied
            //ends in 5 = files that are the same size but different contents in the source and target; will be copied

            fileSystem.Directory.Exists($"{target}\\sub1").Should().BeTrue();
            fileSystem.Directory.Exists($"{target}\\sub2").Should().BeTrue();
            fileSystem.Directory.Exists($"{target}\\sub3").Should().BeFalse();

            fileSystem.File.Exists($"{target}\\1-1.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\1-2.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\1-3.txt").Should().BeFalse();
            fileSystem.File.Exists($"{target}\\1-4.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\1-5.txt").Should().BeTrue();

            LastWriteTimesMatch("1-1.txt").Should().BeFalse();  //because it wasn't copied
            LastWriteTimesMatch("1-2.txt").Should().BeTrue();  
            LastWriteTimesMatch("1-4.txt").Should().BeTrue();  
            LastWriteTimesMatch("1-5.txt").Should().BeTrue();

            fileSystem.File.Exists($"{target}\\sub1\\1-1.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\sub1\\1-2.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\sub1\\1-3.txt").Should().BeFalse();
            fileSystem.File.Exists($"{target}\\sub1\\1-4.txt").Should().BeTrue();
            fileSystem.File.Exists($"{target}\\sub1\\1-5.txt").Should().BeTrue();

            LastWriteTimesMatch("sub1\\1-1.txt").Should().BeFalse();  //because it wasn't copied
            LastWriteTimesMatch("sub1\\1-2.txt").Should().BeTrue();
            LastWriteTimesMatch("sub1\\1-4.txt").Should().BeTrue();
            LastWriteTimesMatch("sub1\\1-5.txt").Should().BeTrue();

            fileSystem.File.Exists($"{target}\\sub2\\newfile.txt").Should().BeTrue();
        }

        public bool LastWriteTimesMatch(string filePath)
        {
            string sourceFile = $"{source}\\{filePath}";
            string targetFile = $"{target}\\{filePath}";

            return fileSystem.File.GetLastWriteTime(sourceFile) == fileSystem.File.GetLastWriteTime(targetFile);
        }
    }
}
