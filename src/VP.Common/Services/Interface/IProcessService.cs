using System.Diagnostics;

namespace VP.Common.Services.Interface
{
    public interface IProcessService
    {//todo 注释

        public IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter);
    }
}
