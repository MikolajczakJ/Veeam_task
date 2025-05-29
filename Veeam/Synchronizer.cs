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

        public async Task SynchronizeFolders()
        {
            var sourceFiles = GetFiles(_repository.SourceDirectory);
            await CheckFiles(sourceFiles, CheckForAdded);

            var replicaFiles = GetFiles(_repository.ReplicaDirectory);
            await CheckFiles(replicaFiles, CheckForRemoved);
        }

        private async Task CheckFiles(string[] files,Func<string,Task> action)
        {
            foreach (var file in files)
                await action(file);
        }
        private string[] GetFiles(string directory) => Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

        //TODO: Implement logging to the log file specified in the repository
        private async Task CheckForAdded(string sourceFile)
        {
            string relativePath = Path.GetRelativePath(_repository.SourceDirectory, sourceFile);
            string replicaFile = Path.Combine(_repository.ReplicaDirectory, relativePath);
            if (!File.Exists(replicaFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(replicaFile));
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

        private string ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

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
    }
}
