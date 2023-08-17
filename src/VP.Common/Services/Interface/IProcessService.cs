using System.Diagnostics;

namespace VP.Common.Services.Interface
{
    public interface IProcessService
    {//todo 注释

        public IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter);

        public int GetParentProcessId(int processId);
        public int GetParentProcessId(Process process);

        public IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<int> processIdList);
        public IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<Process> processList);
    }
}
