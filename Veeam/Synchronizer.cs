using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public class Synchronizer
    {
        private readonly Logger _logger;
        private readonly Repository _repository;
        public Synchronizer(Repository repository)
        {
            _repository = repository;
            _logger = new Logger(_repository);
        }
        /// <summary>
        /// Synchronizes the contents of the source and replica directories.
        /// </summary>
        /// <remarks>This method ensures that the replica directory reflects the current state of the
        /// source directory. Files present in the source directory but missing in the replica directory are added,  and
        /// files present in the replica directory but missing in the source directory are removed.</remarks>
        public async Task SynchronizeFolders()
        {
            var sourceFiles = GetFiles(_repository.SourceDirectory);
            await CheckFiles(sourceFiles, CheckForAdded);

            var replicaFiles = GetFiles(_repository.ReplicaDirectory);
            await CheckFiles(replicaFiles, CheckForRemoved);
        }
        /// <summary>
        /// Executes the specified asynchronous action on each file in the provided collection.
        /// </summary>
        /// <remarks>The method iterates through the provided file paths and invokes the specified action
        /// for each file. The action is executed asynchronously for each file in sequence.</remarks>
        /// <param name="files">An array of file paths to process. Cannot be null.</param>
        /// <param name="action">A delegate representing the asynchronous action to perform on each file. Cannot be null.</param>
        private async Task CheckFiles(string[] files,Func<string,Task> action)
        {
            foreach (var file in files)
                await action(file);
        }
        private string[] GetFiles(string directory) => Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        /// <summary>
        /// Checks if a file has been added or updated in the source directory and ensures it is replicated in the
        /// replica directory.
        /// </summary>
        /// <remarks>If the file does not exist in the replica directory, it is copied to the replica
        /// directory.  If the file exists but its content differs, the file in the replica directory is updated with
        /// the source file's content.</remarks>
        /// <param name="sourceFile">The full path of the file in the source directory to check for addition or updates.</param>
        private async Task CheckForAdded(string sourceFile)
        {
            string relativePath = Path.GetRelativePath(_repository.SourceDirectory, sourceFile);
            string replicaFile = Path.Combine(_repository.ReplicaDirectory, relativePath);
            if (!File.Exists(replicaFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(replicaFile)!);
                File.Copy(sourceFile, replicaFile);
                await _logger.Log($"Copied new file: {relativePath}");
            }
            else
            {
                string sourceHash = ComputeMD5(sourceFile);
                string replicaHash = ComputeMD5(replicaFile);
                if (sourceHash != replicaHash)
                {
                    File.Copy(sourceFile, replicaFile, true);
                    await _logger.Log($"Updated file: {relativePath}");
                }
            }
        }
        /// <summary>
        /// Checks if the specified replica file has been removed from the source directory and deletes it if necessary.
        /// </summary>
        /// <remarks>If the corresponding file in the source directory does not exist, the method deletes
        /// the file  from the replica directory and logs the deletion.</remarks>
        /// <param name="replicaFile">The full path to the file in the replica directory to check.</param>
        /// <returns></returns>
        private async Task CheckForRemoved(string replicaFile)
        {
            string relativePath = Path.GetRelativePath(_repository.ReplicaDirectory, replicaFile);
            string sourceFile = Path.Combine(_repository.SourceDirectory, relativePath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(replicaFile);
                await _logger.Log($"Deleted file: {relativePath}");
            }
        }
        private string ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
