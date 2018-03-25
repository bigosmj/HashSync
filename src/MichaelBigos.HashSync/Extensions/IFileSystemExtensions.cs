using System.IO.Abstractions;

namespace MichaelBigos.HashSync.Extensions
{
    public interface IFileSystemExtensions
    {
        bool CompareFiles(FileBase fileBase, string fullFileName1, string fullFileName2);
        byte[] GetFileHash(FileBase fileBase, string fullFileName);
    }
}