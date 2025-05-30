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
        public async Task StartChecking()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                await _synchronizer.SynchronizeFolders();
            }
        }
    }
}
