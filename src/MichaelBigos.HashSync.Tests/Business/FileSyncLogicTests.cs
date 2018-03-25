using FakeItEasy;
using MichaelBigos.HashSync.Business;
using MichaelBigos.HashSync.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace MichaelBigos.HashSync.Tests.Business
{
    [TestClass]
    public class FileSyncLogicTests
    {
        private FileSyncLogic underTest;
        private IFileSystem fakeFileSystem;
        private IFileSystemExtensions fakeFileSystemExtensions;

        [TestInitialize]
        public void Setup()
        {
            fakeFileSystem = A.Fake<IFileSystem>();
            fakeFileSystemExtensions = A.Fake<IFileSystemExtensions>();
            underTest = new FileSyncLogic(fakeFileSystem);
        }

        [TestMethod]
        public void SyncFolders_DeleteTargetSubfolders()
        {
            //Arrange
            List<string> sourceSubfolders = new List<string>
            {
                "C:\\source\\first"
            };

            List<string> targetSubfolders = new List<string>
            {
                "C:\\target\\first",
                "C:\\target\\fourth"
            };

            A.CallTo(() => fakeFileSystem.Directory.Exists(A<string>._)).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\source")).Returns(sourceSubfolders);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\target")).Returns(targetSubfolders);
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "fourth")).Returns("C:\\target\\fourth");

            DirectoryInfoBase firstDirBase = A.Fake<DirectoryInfoBase>();
            A.CallTo(() => firstDirBase.Name).Returns("first");

            DirectoryInfoBase fourthDirBase = A.Fake<DirectoryInfoBase>();
            A.CallTo(() => fourthDirBase.Name).Returns("fourth");

            A.CallTo(() => fakeFileSystem.DirectoryInfo.FromDirectoryName("C:\\source\\first")).Returns(firstDirBase);
            A.CallTo(() => fakeFileSystem.DirectoryInfo.FromDirectoryName("C:\\target\\first")).Returns(firstDirBase);
            A.CallTo(() => fakeFileSystem.DirectoryInfo.FromDirectoryName("C:\\target\\fourth")).Returns(fourthDirBase);

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.Directory.Delete("C:\\target\\fourth", true)).MustHaveHappened();
            A.CallTo(() => fakeFileSystem.Directory.Delete(A<string>._, true)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void SyncFolders_CreateMissingTargetSubfolders()
        {
            //Arrange
            List<string> sourceSubfolders = new List<string>
            {
                "C:\\source\\first"
            };

            List<string> targetSubfolders = new List<string>
            {
            };

            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\source")).Returns(sourceSubfolders);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\target")).Returns(targetSubfolders);

            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source\\first")).Returns(true);

            DirectoryInfoBase dirBase = A.Fake<DirectoryInfoBase>();
            A.CallTo(() => dirBase.Name).Returns("first");

            A.CallTo(() => fakeFileSystem.DirectoryInfo.FromDirectoryName(A<string>._)).Returns(dirBase);

            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\source", "first")).Returns($"C:\\source\\first");
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "first")).Returns($"C:\\target\\first");

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.Directory.CreateDirectory("C:\\target\\first")).MustHaveHappened();
        }

        [TestMethod]
        public void SyncFolders_SubfolderRecursion()
        {
            //Arrange
            List<string> sourceSubfolders = new List<string>
            {
                "C:\\source\\first"
            };

            List<string> targetSubfolders = new List<string>
            {
                "C:\\target\\first"
            };

            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\source")).Returns(sourceSubfolders);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\target")).Returns(targetSubfolders);

            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source\\first")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target\\first")).Returns(true);

            DirectoryInfoBase dirBase = A.Fake<DirectoryInfoBase>();
            A.CallTo(() => dirBase.Name).Returns("first");

            A.CallTo(() => fakeFileSystem.DirectoryInfo.FromDirectoryName(A<string>._)).Returns(dirBase);

            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\source", "first")).Returns($"C:\\source\\first");
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "first")).Returns($"C:\\target\\first");

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\source\\first")).MustHaveHappened();
            A.CallTo(() => fakeFileSystem.Directory.EnumerateDirectories("C:\\target\\first")).MustHaveHappened();
        }

        [TestMethod]
        public void SyncFolders_CopyMissingFiles()
        {
            //Arrange
            List<string> sourceFiles = new List<string>
            {
                "first.pdf"
            };

            List<string> targetFiles = new List<string>
            {
                "second.pdf"
            };

            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\source")).Returns(sourceFiles);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\target")).Returns(targetFiles);

            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target")).Returns(true);

            A.CallTo(() => fakeFileSystem.File.Exists("C:\\target\\first.pdf")).Returns(false);

            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\source", "first.pdf")).Returns("C:\\source\\first.pdf");
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "first.pdf")).Returns("C:\\target\\first.pdf");

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.File.Copy(A<string>._, A<string>._, true)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void SyncFolders_CopyFilesWithDifferentSize()
        {
            //Arrange
            List<string> sourceAndTargetFiles = new List<string>
            {
                "first.pdf"
            };

            int fileSize = 1080;

            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\source")).Returns(sourceAndTargetFiles);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\target")).Returns(sourceAndTargetFiles);

            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target")).Returns(true);

            A.CallTo(() => fakeFileSystem.File.Exists("C:\\target\\first.pdf")).Returns(true);

            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\source", "first.pdf")).Returns("C:\\source\\first.pdf");
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "first.pdf")).Returns("C:\\target\\first.pdf");

            FileInfoBase fileInfoSource = A.Fake<FileInfoBase>();
            A.CallTo(() => fileInfoSource.Length).Returns(fileSize);

            FileInfoBase fileInfoTarget = A.Fake<FileInfoBase>();
            A.CallTo(() => fileInfoTarget.Length).Returns(fileSize * 2);


            A.CallTo(() => fakeFileSystem.FileInfo.FromFileName(A<string>._)).Returns(fileInfoSource).Once().Then.Returns(fileInfoTarget);

            A.CallTo(() => fakeFileSystemExtensions.CompareFiles(A<FileBase>._, A<string>._, A<string>._)).Returns(true);

            FileSystemExtensionsWrapper.fileSystemExtensions = fakeFileSystemExtensions;

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.File.Copy(A<string>._, A<string>._, true)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void SyncFolders_DontCopySameFiles()
        {
            //Arrange
            List<string> sourceAndTargetFiles = new List<string>
            {
                "first.pdf"
            };

            int fileSize = 1080;

            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\source")).Returns(sourceAndTargetFiles);
            A.CallTo(() => fakeFileSystem.Directory.EnumerateFiles("C:\\target")).Returns(sourceAndTargetFiles);

            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\source")).Returns(true);
            A.CallTo(() => fakeFileSystem.Directory.Exists($"C:\\target")).Returns(true);

            A.CallTo(() => fakeFileSystem.File.Exists("C:\\target\\first.pdf")).Returns(true);

            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\source", "first.pdf")).Returns("C:\\source\\first.pdf");
            A.CallTo(() => fakeFileSystem.Path.Combine("C:\\target", "first.pdf")).Returns("C:\\target\\first.pdf");

            FileInfoBase fileInfo = A.Fake<FileInfoBase>();
            A.CallTo(() => fileInfo.Length).Returns(fileSize);

            A.CallTo(() => fakeFileSystem.FileInfo.FromFileName(A<string>._)).Returns(fileInfo);

            A.CallTo(() => fakeFileSystemExtensions.CompareFiles(A<FileBase>._, A<string>._, A<string>._)).Returns(true);

            FileSystemExtensionsWrapper.fileSystemExtensions = fakeFileSystemExtensions;

            //Act
            underTest.SyncFolders("C:\\source", "C:\\target");

            //Assert
            A.CallTo(() => fakeFileSystem.File.Copy(A<string>._, A<string>._)).MustNotHaveHappened();
        }

    }
}
