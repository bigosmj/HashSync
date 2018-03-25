using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using MichaelBigos.HashSync.Extensions;
using System.Text;
using System.Linq;
using MichaelBigos.HashSync.Models;
using MichaelBigos.HashSync.Services;

namespace MichaelBigos.HashSync.Business
{
    public class FileSyncLogic
    {
        private readonly IFileSystem fileSystem;

        public FileSyncLogic(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void SyncFolders(CommandLineArgumentInput input)
        {
            SyncFolders(input.sourcePath, input.targetPath);
        }

        public void SyncFolders(string sourceFolder, string targetFolder)
        {
            try
            {
                bool sourceFolderExists = fileSystem.Directory.Exists(sourceFolder);
                bool targetFolderExists = fileSystem.Directory.Exists(targetFolder);

                //if only right folder exists
                if (!sourceFolderExists && targetFolderExists)
                {
                    //TODO - receive from command line the list of folders to ignore.
                    if (fileSystem.Path.GetDirectoryName(targetFolder) != ".git")
                    {
                        fileSystem.Directory.Delete(targetFolder);
                        Console.WriteLine($"Deleted {targetFolder}");
                    }
                }

                //if only right folder doesn't exist
                if (!targetFolderExists)
                {
                    fileSystem.Directory.CreateDirectory(targetFolder);
                    Console.WriteLine($"Created Folder {targetFolder}");
                }

                List<string> sourceSubfolders = new List<string>(fileSystem.Directory.EnumerateDirectories(sourceFolder));
                List<string> sourceFiles = new List<string>(fileSystem.Directory.EnumerateFiles(sourceFolder));

                List<string> targetSubfolders = new List<string>(fileSystem.Directory.EnumerateDirectories(targetFolder));
                List<string> targetFiles = new List<string>(fileSystem.Directory.EnumerateFiles(targetFolder));

                var folderNameComparer = new FolderNameFromFullPathComparer(fileSystem);
                var fileNameComparer = new FileNameFromFullPathComparer(fileSystem);

                //delete target subfolders not in source
                var foldersToDelete = targetSubfolders.Except(sourceSubfolders, folderNameComparer).ToList();
                foldersToDelete.ForEach(subfolder => {
                    fileSystem.Directory.Delete(subfolder, true);
                    Console.WriteLine($"Deleted {subfolder}");
                });

                //delete target files not in source
                var filesToDelete = targetFiles.Except(sourceFiles, fileNameComparer).ToList();
                filesToDelete.ForEach(file => {
                    fileSystem.File.Delete(file);
                    Console.WriteLine($"Deleted {file}");
                });

                //compare subfolders
                foreach (string sourceSubfolder in sourceSubfolders)
                {
                    string subFolderName = fileSystem.DirectoryInfo.FromDirectoryName(sourceSubfolder).Name;
                    string targetSubfolder = fileSystem.Path.Combine(targetFolder, subFolderName);
                    SyncFolders(sourceSubfolder, targetSubfolder);
                }

                //compare files
                foreach (string sourceFileFullPath in sourceFiles)
                {
                    FileInfoBase sourceFileInfo = fileSystem.FileInfo.FromFileName(sourceFileFullPath);
                    string filename = sourceFileInfo.Name;
                    string targetFileFullPath = fileSystem.Path.Combine(targetFolder, filename);

                    if (fileSystem.File.Exists(targetFileFullPath))
                    {
                        long sourceSize = sourceFileInfo.Length;
                        long targetSize = fileSystem.FileInfo.FromFileName(targetFileFullPath).Length;

                        if (sourceSize == targetSize)
                        {
                            bool filesMatch = fileSystem.File.CompareFiles(sourceFileFullPath, targetFileFullPath);

                            if (filesMatch)
                            {
                                continue;
                            }
                        }
                    }

                    fileSystem.File.Copy(sourceFileFullPath, targetFileFullPath, true);
                    //for now, only log to the console. Replace with NLog later
                    Console.WriteLine($"Copied {sourceFileFullPath}");
                }//end foreach file in folder
            }//end try
            catch (Exception ex)
            {
                throw;
            }
        }//end SyncFolders
    }
}
