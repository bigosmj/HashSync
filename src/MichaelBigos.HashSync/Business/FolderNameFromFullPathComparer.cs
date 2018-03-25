using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Abstractions;

namespace MichaelBigos.HashSync.Business
{
    public class FolderNameFromFullPathComparer : IEqualityComparer<string>
    {
        private IFileSystem fileSystem;

        public FolderNameFromFullPathComparer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool Equals(string fullPath1, string fullPath2)
        {
            string name1 = fileSystem.DirectoryInfo.FromDirectoryName(fullPath1).Name;
            string name2 = fileSystem.DirectoryInfo.FromDirectoryName(fullPath2).Name;
            return name1 == name2;
        }

        public int GetHashCode(string fullPath)
        {
            return fileSystem.DirectoryInfo.FromDirectoryName(fullPath).Name.GetHashCode();
        }
    }
}
