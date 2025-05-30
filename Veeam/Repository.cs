using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public record Repository(string SourceDirectory, string ReplicaDirectory, string LogDirectory, TimeSpan CheckInterval)
    {
        public override string ToString()
        {
            return $" Source Directory: {SourceDirectory}\n Replica Directory: {ReplicaDirectory}\n Log Directory: {LogDirectory}\n Check Interval: {CheckInterval}";
        }
    }
}
