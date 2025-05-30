using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public class SynchronizationChecker
    {
        private readonly Synchronizer _synchronizer;
        private readonly Repository _repository;
        private readonly PeriodicTimer _timer;
        public SynchronizationChecker(Repository repository)
        {
            _repository = repository;
            _synchronizer = new Synchronizer(_repository);
            _timer = new PeriodicTimer(_repository.CheckInterval);
        }
        /// <summary>
        /// Starts a periodic synchronization process for folders.
        /// </summary>
        /// <remarks>This method initiates a loop that waits for the next timer tick and then triggers
        /// folder synchronization. The loop continues until the timer is stopped or disposed.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task completes when the timer is stopped or disposed.</returns>
        public async Task StartChecking()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                await _synchronizer.SynchronizeFolders();
            }
        }
    }
}
