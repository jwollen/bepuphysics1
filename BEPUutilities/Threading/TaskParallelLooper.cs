using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPUutilities.Threading
{
    public class TaskParallelLooper : IParallelLooper
    {
        public void ForLoop(int startIndex, int endIndex, Action<int> loopBody)
        {
            Parallel.For(startIndex, endIndex, loopBody);
        }

        public int ThreadCount
        {
            get { return Environment.ProcessorCount; }
        }
    }
}
