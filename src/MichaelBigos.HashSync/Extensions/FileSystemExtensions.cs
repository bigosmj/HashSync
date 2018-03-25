using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;


namespace MichaelBigos.HashSync.Extensions
{
    public static class FileSystemExtensionsWrapper
    {
        private static IFileSystemExtensions defaultFileSystemExtensions = new FileSystemExtensions();
        public static IFileSystemExtensions fileSystemExtensions { private get; set; } = defaultFileSystemExtensions;

        /// <summary>
        /// Calculate a SHA250 hash for a specified file. Used to compare to two files are the same
        /// </summary>
        /// <param name="fullFileName">The full path of the file for which to get the hash</param>
        /// <returns>The byte array of the file hash</returns>
        public static byte[] GetFileHash(this FileBase fileBase, string fullFileName)
        {
            return fileSystemExtensions.GetFileHash(fileBase, fullFileName);
        }//end GetFileHash

        /// <summary>
        /// Compare two files to see if they have the same contents
        /// </summary>
        /// <param name="fullFileName1">Full path of the first file</param>
        /// <param name="fullFileName2">Full path of the second file</param>
        /// <returns>True if the files are the same, false otherwise</returns>
        public static bool CompareFiles(this FileBase fileBase, string fullFileName1, string fullFileName2)
        {
            return fileSystemExtensions.CompareFiles(fileBase, fullFileName1, fullFileName2);
        }
    }

    public class FileSystemExtensions : IFileSystemExtensions
    {
        /// <summary>
        /// Calculate a SHA250 hash for a specified file. Used to compare to two files are the same
        /// </summary>
        /// <param name="fullFileName">The full path of the file for which to get the hash</param>
        /// <returns>The byte array of the file hash</returns>
        public byte[] GetFileHash(FileBase fileBase, string fullFileName)
        {

            using (Stream fileStream = fileBase.OpenRead(fullFileName))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(fileStream);
                return checksum;
            }
        }//end GetFileHash

        /// <summary>
        /// Compare two files to see if they have the same contents
        /// </summary>
        /// <param name="fullFileName1">Full path of the first file</param>
        /// <param name="fullFileName2">Full path of the second file</param>
        /// <returns>True if the files are the same, false otherwise</returns>
        public bool CompareFiles(FileBase fileBase, string fullFileName1, string fullFileName2)
        {
            byte[] hash1 = GetFileHash(fileBase, fullFileName1);
            byte[] hash2 = GetFileHash(fileBase, fullFileName2);
            return Enumerable.SequenceEqual(hash1, hash2);
        }
    }
}
