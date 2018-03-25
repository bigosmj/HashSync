using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace MichaelBigos.HashSync.Business
{
    public class FileNameFromFullPathComparer : IEqualityComparer<string>
    {
        private IFileSystem fileSystem;

        public FileNameFromFullPathComparer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool Equals(string fullPath1, string fullPath2)
        {
            string name1 = fileSystem.Path.GetFileName(fullPath1);
            string name2 = fileSystem.Path.GetFileName(fullPath2);
            return name1 == name2;
        }

        public int GetHashCode(string fullPath)
        {
            return fileSystem.Path.GetFileName(fullPath).GetHashCode();
        }
    }
}
