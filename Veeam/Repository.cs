using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public record Repository(string SourceDirectory, string ReplicaDirectory, string LogDirectory, TimeSpan CheckInterval);
}
